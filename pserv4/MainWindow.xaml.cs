using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace pserv4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DataController CurrentController;
        private DataController[] Controllers = {
            new services.ServicesDataController(), // services
            new devices.DevicesDataController(), // devices
            new windows.WindowsDataController(), // windows 
            new uninstaller.UninstallerDataController(), // uninstaller
            new processes.ProcessesDataController(), // processes
            new modules.ModulesDataController(), // modules
        };

        private readonly Button[] SwitchButtons;
        private int LastActiveButton = -1;

        private ObservableCollection<DataObject> Items = new ObservableCollection<DataObject>();

        public MainWindow()
        {
            InitializeComponent();

            SwitchButtons = new Button[6];
            SwitchButtons[0] = ButtonServices;
            SwitchButtons[1] = ButtonDevices;
            SwitchButtons[2] = ButtonWindows;
            SwitchButtons[3] = ButtonUninstaller;
            SwitchButtons[4] = ButtonProcesses;
            SwitchButtons[5] = ButtonModules;
            
        }

        private void SwitchController(int index)
        {
            if (LastActiveButton == index)
                return;

            using (new WaitCursor())
            {
                Items.Clear();
                MainGridView.Columns.Clear();
                ICollectionView dataView = CollectionViewSource.GetDefaultView(Items);
                dataView.SortDescriptions.Clear();

                if (LastActiveButton >= 0)
                {
                    SwitchButtons[LastActiveButton].Background = new SolidColorBrush(Colors.White);
                }

                // cleanup
                CurrentController = Controllers[index];

                // create columns
                CurrentController.Refresh(Items);
                FindThisText.Text = "";

                MainListView.ItemsSource = Items;   //your query result 

                foreach (ObjectColumn oc in CurrentController.Columns)
                {
                    GridViewColumn column = new GridViewColumn();
                    column.Header = oc.DisplayName;
                    column.DisplayMemberBinding = new Binding(oc.BindingName);
                    column.Width = System.Double.NaN;
                    MainGridView.Columns.Add(column);
                }
                CurrentController.MainListView = MainListView;
                MainListView.ContextMenu = CurrentController.ContextMenu;
                MainListView.ContextMenuOpening += MainListView_ContextMenuOpening;

                CreateInitialSort();
                LastActiveButton = index;
                SwitchButtons[LastActiveButton].Background = new SolidColorBrush(Colors.LightGray);
            }
        }

        void MainListView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            try
            {
                CurrentController.OnContextMenuOpening(MainListView.SelectedItems, MainListView.ContextMenu);
            }
            catch(Exception)
            {
            }
        }

        private void SwitchToServices(object sender, RoutedEventArgs e)
        {
            SwitchController(0);
        }

        private void SwitchToDevices(object sender, RoutedEventArgs e)
        {
            SwitchController(1);
        }

        private void SwitchToWindows(object sender, RoutedEventArgs e)
        {
            SwitchController(2);
        }

        private void SwitchToUninstaller(object sender, RoutedEventArgs e)
        {
            SwitchController(3);
        }

        private void SwitchToProcesses(object sender, RoutedEventArgs e)
        {
            SwitchController(4);
        }

        private void SwitchToModules(object sender, RoutedEventArgs e)
        {
            SwitchController(5);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SwitchController(0);
        }

        private void Zoom_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            bool handle = (Keyboard.Modifiers & ModifierKeys.Control) > 0;
            if (handle)
            {
                if (e.Delta > 0)
                    MainListView.FontSize += 1.0;
                else
                    MainListView.FontSize -= 1.0;

                if (MainListView.FontSize < 4.0)
                    MainListView.FontSize = 4.0;

                if (MainListView.FontSize > 60)
                    MainListView.FontSize = 60;
            }
            AutoResizeAllColumns();
        }

        private void AutoResizeAllColumns()
        {
            GridView gridView = MainListView.View as GridView;

            foreach (GridViewColumn column in gridView.Columns)
            {
                column.Width = System.Double.NaN;
            }
        }

        private void MainListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataObject item = ((FrameworkElement)e.OriginalSource).DataContext as DataObject;
            if (item != null)
            {
                //EditItem(item); TODO: pass on to controller for editing
            }
        }

        private void MainListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool enabled = MainListView.SelectedItems.Count > 0;
            //BtEdit.IsEnabled = enabled;
            //BtDelete.IsEnabled = enabled;
        }

        private void FindThisText_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(Items);
            if (FindThisText.Text.Trim().Length == 0)
            {
                view.Filter = null;
            }
            else
            {
                view.Filter = new Predicate<object>(FilterDataObjectItem);
            }
        }

        private bool FilterDataObjectItem(object obj)
        {
            DataObject item = obj as DataObject;
            if (item == null) 
                return false;


            string findThisText = FindThisText.Text.Trim().ToLower();

            Type actualType = obj.GetType();

            foreach(ObjectColumn oc in CurrentController.Columns)
            {
                object actualValue = actualType.GetProperty(oc.BindingName).GetValue(obj, null);
                if (actualValue != null)
                {
                    string sValue = actualValue as string;
                    if (sValue == null)
                        sValue = actualValue.ToString();

                    if( sValue.ToLower().Contains(findThisText) )
                        return true;
                }
            }
            return false;
        }

        private string LastHeaderClicked;
        private ListSortDirection LastSortDirection;

        private void MainListView_GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            string headerClicked = e.OriginalSource as string;
            if (headerClicked == null)
            {
                GridViewColumnHeader header = e.OriginalSource as GridViewColumnHeader;
                if (header != null)
                {
                    Binding binding = header.Column.DisplayMemberBinding as Binding;
                    headerClicked = binding.Path.Path;
                }
            }
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked != LastHeaderClicked)
                {
                    direction = ListSortDirection.Ascending;
                }
                else
                {
                    if (LastSortDirection == ListSortDirection.Ascending)
                    {
                        direction = ListSortDirection.Descending;
                    }
                    else
                    {
                        direction = ListSortDirection.Ascending;
                    }
                }

                Sort(headerClicked, direction);
                LastHeaderClicked = headerClicked;
                LastSortDirection = direction;
            }
        }

        private void CreateInitialSort()
        {
            Binding b = MainGridView.Columns[0].DisplayMemberBinding as Binding;
            MainListView_GridViewColumnHeaderClickedHandler(this, new RoutedEventArgs(null, b.Path.Path));
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(Items);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }
    }
}
