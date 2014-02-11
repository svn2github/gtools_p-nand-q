using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                tabitem.Content = MainWindow.CurrentController.CreateDetailsPage(o);
                if (tabitem.Content != null)
                {
                    IDataObjectDetails details = tabitem.Content as IDataObjectDetails;
                    if (details != null)
                    {
                        details.BindTabItem(tabitem);
                        tabitem.Header = details.Caption;
                        MyTabControl.Items.Add(tabitem);
                    }
                }
                
            }
            MyTabControl.SelectedIndex = 0;
        }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            List<IDataObjectDetails> changedItems = new List<IDataObjectDetails>();

            foreach(TabItem tabitem in MyTabControl.Items)
            {
                IDataObjectDetails details = tabitem.Content as IDataObjectDetails;
                if (details != null)
                {
                    if(details.HasAnyChanges())
                    {
                        changedItems.Add(details);
                    }
                }
            }

            MainWindow.CurrentController.ApplyChanges(changedItems);
            Close();
        }
        
        private void OnRevert(object sender, RoutedEventArgs e)
        {
            TabItem tabitem = MyTabControl.SelectedItem as TabItem;
            if( tabitem != null )
            {
                IDataObjectDetails details = tabitem.Content as IDataObjectDetails;
                if( details != null )
                {
                    details.RevertChanges();
                }
            }
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
