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

namespace pserv4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IObjectController CurrentController;
        private IObjectController[] Controllers = {
            new services.ServicesDataController(), // services
            new devices.DevicesDataController(), // devices
            new windows.WindowsDataController(), // windows 
            new devices.DevicesDataController(), // uninstaller
            new processes.ProcessesDataController(), // processes
            new devices.DevicesDataController(), // modules
        };

        private readonly Button[] SwitchButtons;
        private int LastActiveButton = -1;

        private ObservableCollection<IObject> Items = new ObservableCollection<IObject>();

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

                if (LastActiveButton >= 0)
                {
                    SwitchButtons[LastActiveButton].Background = new SolidColorBrush(Colors.LightGray);
                    SwitchButtons[LastActiveButton].Foreground = new SolidColorBrush(Colors.Black);
                }

                // cleanup

                CurrentController = Controllers[index];

                // create columns
                CurrentController.Refresh(Items);
                MainListView.ItemsSource = Items;   //your query result 

                foreach (ObjectColumn oc in CurrentController.Columns)
                {
                    GridViewColumn column = new GridViewColumn();
                    column.Header = oc.DisplayName;
                    column.DisplayMemberBinding = new Binding(oc.BindingName);
                    MainGridView.Columns.Add(column);
                }
                LastActiveButton = index;
                SwitchButtons[index].Background = new SolidColorBrush(Colors.SteelBlue);
                SwitchButtons[index].Foreground = new SolidColorBrush(Colors.White);
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
    }
}
