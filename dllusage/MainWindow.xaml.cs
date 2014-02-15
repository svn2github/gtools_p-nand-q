using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
using System.Collections;
using Microsoft.Win32;
using System.Windows.Threading;
using log4net;
using GSharpTools;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;

namespace dllusage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private MainViewType CurrentViewType = MainViewType.Invalid;
        protected class DataView
        {
            public readonly DataController Controller;
            public readonly Button Button;

            public DataView(DataController controller, Button button)
            {
                Controller = controller;
                Button = button;
            }
        }

        private readonly Dictionary<MainViewType, DataView> KnownViews = new Dictionary<MainViewType, DataView>();
        private readonly ObservableCollection<TreeViewItemModel> Items = new ObservableCollection<TreeViewItemModel>();
        private readonly SolidColorBrush SelectedBackgroundColor;
        private readonly SolidColorBrush SelectedForegroundColor;
        private readonly SolidColorBrush UnselectedBackgroundColor;
        private readonly SolidColorBrush UnselectedForegroundColor;
        private readonly RefreshManager DataController = new RefreshManager();

        public MainWindow()
        {
            InitializeComponent();
            KnownViews[MainViewType.ListByDLLName] = new DataView(new GetModulesByName(), BtDllName );
            KnownViews[MainViewType.ListByDLLPath] = new DataView(new GetModulesByPath(), BtDllPath );
            KnownViews[MainViewType.ListByEXEName] = new DataView(new GetProcessesByName(), BtExeName );
            KnownViews[MainViewType.ListByEXEPath] = new DataView(new GetProcessesByPath(), BtExePath );
            UnselectedBackgroundColor = new SolidColorBrush(Color.FromArgb(255, 0xF5, 0xF5, 0xF5));
            UnselectedForegroundColor = new SolidColorBrush(Colors.Black);
            SelectedBackgroundColor = new SolidColorBrush(Colors.CornflowerBlue);
            SelectedForegroundColor = new SolidColorBrush(Colors.White);
            Title = string.Format("dllusage {0}", AppVersion.Get());
            MainTreeView.ItemsSource = Items;
            MainTreeView.ContextMenu = CreateContextMenu();
        }
        
        protected MenuItem AppendMenuItem(ContextMenu menu, string header, string imageName, RoutedEventHandler callback)
        {
            MenuItem mi = new MenuItem();
            mi.Header = header;
            Image i = new Image();
            string filename = string.Format(@"pack://application:,,,/images/{0}.png", imageName);
            i.Source = new BitmapImage(new Uri(filename));
            mi.Icon = i;
            mi.Click += callback;
            menu.Items.Add(mi);
            return mi;
        }

        private ContextMenu CreateContextMenu()
        {
            ContextMenu menu = new ContextMenu();
            AppendMenuItem(menu, resource.IDS_SAVE_AS_XML, "database_save", OnSaveAsXML);
            AppendMenuItem(menu, resource.IDS_COPY_TO_CLIPBOARD, "database_lightning", OnCopyToClipboard);
            menu.Items.Add(new Separator());
            AppendMenuItem(menu, resource.IDS_PROPERTIES, "database_gear", OnShowProperties);
            return menu;
        }

        private void OnCopyToClipboard(object sender, RoutedEventArgs e)
        {
            DoSaveAsXml(null);
        }

        private void OnShowProperties(object sender, RoutedEventArgs e)
        {
            string filename;
            if( GetSelectedFileName(out filename))
            {
                ProcessInfoTools.ShowFileProperties(filename);
            }
        }

        private bool GetSelectedFileName(out string filename)
        {
            filename = null;

            TreeViewItemModel model = MainTreeView.SelectedItem as TreeViewItemModel;
            if (model != null)
            {
                Process p = model.Tag as Process;
                if (p != null)
                {
                    filename = p.GetSafeProcessName();
                    return true;
                }
                else
                {
                    ProcessModule m = model.Tag as ProcessModule;
                    if (m != null)
                    {
                        filename = m.GetSafeModuleName();
                        return true;
                    }
                }
            }
            return false;
        }

        private void OnShowConsole(object sender, RoutedEventArgs e)
        {
            string filename;
            if (GetSelectedFileName(out filename))
            {
                ProcessInfoTools.ShowTerminal(System.IO.Path.GetDirectoryName(filename));
            }
        }

        private void OnShowExplorer(object sender, RoutedEventArgs e)
        {
            string filename;
            if (GetSelectedFileName(out filename))
            {
                ProcessInfoTools.ShowExplorer(System.IO.Path.GetDirectoryName(filename));
            }
        }

        private void OnShowDependencies(object sender, RoutedEventArgs e)
        {
            string dependencyViewer;
            if (ProcessInfoTools.FindExecutable("depends.exe", out dependencyViewer))
            {
                string filename;
                if (GetSelectedFileName(out filename))
                {
                    Process.Start(dependencyViewer, string.Format("\"{0}\"", filename));
                }
            }
            else
            {
                MessageBox.Show("Unable to find DEPENDS.EXE", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnRefreshDisplay(object sender, RoutedEventArgs e)
        {
            
        }
        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            if (item != null)
            {
                item.Focus();
                e.Handled = true;
            }
        }

        private void OnSaveAsXML(object sender, RoutedEventArgs e)
        {
            string filename;
            if ((e == null) && (sender is string))
            {
                filename = sender as string;
            }
            else
            {
                SaveFileDialog dialog = new SaveFileDialog();
                // Set filter for file extension and default file extension 
                dialog.DefaultExt = ".xml";
                dialog.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";

                // Display OpenFileDialog by calling ShowDialog method 
                bool? result = dialog.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    filename = dialog.FileName;
                }
                else return;
            }
            DoSaveAsXml(filename);
        }

        private void DoSaveAsXml(string filename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = Encoding.UTF8;
            settings.IndentChars = "\t";
            settings.NewLineChars = "\r\n";

            StringBuilder buffer = new StringBuilder();
            using (XmlWriter xtw = (filename == null) ? XmlWriter.Create(buffer, settings) : XmlWriter.Create(filename, settings))
            {
                xtw.WriteStartDocument();
                xtw.WriteStartElement(KnownViews[CurrentViewType].Controller.ToString());
                xtw.WriteAttributeString("version", "4.0");
                xtw.WriteAttributeString("count", Items.Count.ToString());
                OnSaveAsXML(xtw);
                xtw.WriteEndElement();
                xtw.Close();
            }
            if (filename == null)
            {
                Clipboard.SetText(buffer.ToString());
            }      
        }

        private void OnSaveAsXML(XmlWriter xtw)
        {
            foreach(TreeViewItemModel o in Items)
            {
                xtw.WriteStartElement(o.XmlItemName);
                xtw.WriteAttributeString("id", o.XmlItemID);

                string childName = null;

                foreach (TreeViewItemModel c in o.Items)
                {
                    if (childName == null )
                    {
                        childName = c.XmlItemName;
                    }
                    xtw.WriteElementString(childName, c.XmlItemID);
                }
                xtw.WriteEndElement();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SwitchController(MainViewType.ListByDLLName);
            DispatcherTimer dt = new DispatcherTimer();
            dt.Tick += new EventHandler(timer_Tick);
            dt.Interval = new TimeSpan(0, 0, 5); // execute every hour
            dt.Start();
        }

        private void Zoom_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            bool handle = (Keyboard.Modifiers & ModifierKeys.Control) > 0;
            if (handle)
            {
                if (e.Delta > 0)
                    MainTreeView.FontSize += 1.0;
                else
                    MainTreeView.FontSize -= 1.0;

                if (MainTreeView.FontSize < 4.0)
                    MainTreeView.FontSize = 4.0;

                if (MainTreeView.FontSize > 60)
                    MainTreeView.FontSize = 60;
            }
        }

        private void timer_Tick(object sender, EventArgs args)
        {
            Log.InfoFormat("*** BEGIN TIMER TICK: {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);

            using (new WaitCursor())
            {
                DateTime startTime = DateTime.Now;
                try
                {
                    DataController.Refresh(Items, KnownViews[CurrentViewType].Controller);
                }
                catch (Exception e)
                {
                    Log.Error(string.Format("{0}.Refresh()", DataController), e);
                }
                Log.InfoFormat("Total time for DataController.Refresh: {0}", DateTime.Now - startTime);
                Log.InfoFormat("Total items count: {0}", Items.Count);
            }
            Log.InfoFormat("*** END TIMER TICK: {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
        }

        private string TextToFind;

        private void FindThisText_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(Items);
            if (FindThisText.Text.Trim().Length == 0)
            {
                view.Filter = null;
            }
            else
            {
                TextToFind = FindThisText.Text.Trim().ToLower();
                view.Filter = new Predicate<object>(FilterDataObjectItem);
            }
        }

        private bool FilterDataObjectItem(object obj)
        {
            TreeViewItemModel item = obj as TreeViewItemModel;
            if (item == null)
                return false;

            return item.Name.ToLower().Contains(TextToFind);
        }

        private void SwitchController(MainViewType displayMode, bool visible = true)
        {
            if (CurrentViewType == displayMode)
                return;

            Log.InfoFormat("**** BEGIN SwitchController(): switch from {0} to {1} **** ", CurrentViewType, displayMode);

            using (new WaitCursor())
            {
                Items.Clear();
                if (visible)
                {
                    ICollectionView dataView = CollectionViewSource.GetDefaultView(Items);
                    dataView.SortDescriptions.Clear();

                    if (CurrentViewType != MainViewType.Invalid)
                    {
                        KnownViews[CurrentViewType].Button.Background = UnselectedBackgroundColor;
                        KnownViews[CurrentViewType].Button.Foreground = UnselectedForegroundColor;
                    }
                }
                // create columns
                DateTime startTime = DateTime.Now;
                try
                {
                    DataController.Refresh(Items, KnownViews[displayMode].Controller);
                }
                catch (Exception e)
                {
                    Log.Error(string.Format("{0}.Refresh()", DataController), e);
                }
                Log.InfoFormat("Total time for DataController.Refresh: {0}", DateTime.Now - startTime);
                Log.InfoFormat("Total items count: {0}", Items.Count);

                FindThisText.Text = "";

                if (visible)
                {
                    MainTreeView.ItemsSource = Items;   //your query result 
                    


                    CreateInitialSort();
                    KnownViews[displayMode].Button.Background = SelectedBackgroundColor;
                    KnownViews[displayMode].Button.Foreground = SelectedForegroundColor;
                }
                CurrentViewType = displayMode;
            }
            Log.Info("**** END SwitchController() ****");
        }

        private void CreateInitialSort()
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(Items);
            if (dataView.SortDescriptions.Count == 0)
            {
                SortDescription sd = new SortDescription("Name", ListSortDirection.Ascending);
                dataView.SortDescriptions.Add(sd);
                dataView.Refresh();
            }
        }

        private void SwitchToDllName(object sender, RoutedEventArgs e)
        {
            SwitchController(MainViewType.ListByDLLName);
        }

        private void SwitchToDllPath(object sender, RoutedEventArgs e)
        {
            SwitchController(MainViewType.ListByDLLPath);
        }

        private void SwitchToExeName(object sender, RoutedEventArgs e)
        {
            SwitchController(MainViewType.ListByEXEName);
        }

        private void SwitchToExePath(object sender, RoutedEventArgs e)
        {
            SwitchController(MainViewType.ListByEXEPath);
        }

        private void MainTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            bool enabled = (MainTreeView.SelectedItem != null);

            BtShowDependencies.IsEnabled = enabled;
            BtShowExplorer.IsEnabled = enabled;
            BtShowConsole.IsEnabled = enabled;
            BtShowProperties.IsEnabled = enabled;
        }
    }
}

