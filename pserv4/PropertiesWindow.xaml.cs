using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace pserv4
{
    /// <summary>
    /// Interaction logic for PropertiesWindow.xaml
    /// </summary>
    public partial class PropertiesWindow : Window
    {
        public PropertiesWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach(DataObject o in MainWindow.CurrentController.MainListView.SelectedItems)
            {
                TabItem tabitem = new TabItem();
                tabitem.Header = o.InternalID;
                tabitem.Content = MainWindow.CurrentController.CreateDetailsPage(o);
                if (tabitem.Content != null)
                {
                    MyTabControl.Items.Add(tabitem);
                }
            }
            MyTabControl.SelectedIndex = 0;
        }


    }
}
