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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace pserv4.services
{
    /// <summary>
    /// Interaction logic for ServiceDetails.xaml
    /// </summary>
    public partial class ServiceDetails : UserControl, IDataObjectDetails
    {
        private SolidColorBrush UnmodifiedForeground;
        private SolidColorBrush ModifiedForeground;

        private bool First;
        private readonly ServiceDataObject SDO;
        private TabItem TabItem;
        private string DisplayName;
        private string Description;
        private string BinaryPathName;
        private SC_START_TYPE StartType; 

        public ServiceDetails(ServiceDataObject sdo)
        {
            InitializeComponent();
            First = true;
            SDO = sdo;
            ModifiedForeground = new SolidColorBrush(Colors.Blue);
            UnmodifiedForeground = new SolidColorBrush(Colors.Black);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if( First )
            {
                RevertChanges();
                First = false;
            }
            else
            {
                TbDisplayName.Text = DisplayName;
                TbDescription.Text = Description;
                TbImagePath.Text = BinaryPathName;
                CbStartupType.SelectedIndex = (int)StartType;
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            DisplayName = TbDisplayName.Text;
            Description = TbDescription.Text;
            BinaryPathName = TbImagePath.Text;
            StartType = (SC_START_TYPE)CbStartupType.SelectedIndex;
        }

        private void OnBrowseFilename(object sender, RoutedEventArgs e)
        {
        }

        private void TbDisplayName_TextChanged(object sender, TextChangedEventArgs e)
        {
            TbDisplayName.Foreground = TbDisplayName.Text.Equals(SDO.DisplayName) ? UnmodifiedForeground : ModifiedForeground;
            TabItem.Foreground = HasAnyChanges() ? ModifiedForeground : UnmodifiedForeground;
        }

        private void TbDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            TbDescription.Foreground = TbDescription.Text.Equals(SDO.Description) ? UnmodifiedForeground : ModifiedForeground;
            TabItem.Foreground = HasAnyChanges() ? ModifiedForeground : UnmodifiedForeground;
        }

        private void TbImagePath_TextChanged(object sender, TextChangedEventArgs e)
        {
            TbImagePath.Foreground = TbImagePath.Text.Equals(SDO.BinaryPathName) ? UnmodifiedForeground : ModifiedForeground;
            TabItem.Foreground = HasAnyChanges() ? ModifiedForeground : UnmodifiedForeground;
        }

        private void CbStartupType_SelectionChanged(object sender, RoutedEventArgs e)
        {
            CbStartupType.Foreground = (((SC_START_TYPE)CbStartupType.SelectedIndex) == SDO.StartType) ? UnmodifiedForeground : ModifiedForeground;
            TabItem.Foreground = HasAnyChanges() ? ModifiedForeground : UnmodifiedForeground;
        }
        
        public void RevertChanges()
        {
            DisplayName = TbDisplayName.Text = SDO.DisplayName;
            Description = TbDescription.Text = SDO.Description;
            BinaryPathName = TbImagePath.Text = SDO.BinaryPathName;
            CbStartupType.SelectedIndex = (int)SDO.StartType;
            StartType = SDO.StartType;
        }

        public void BindTabItem(TabItem tabItem)
        {
            TabItem = tabItem;
        }

        public bool HasAnyChanges()
        {
            return
                (TbDisplayName.Foreground == ModifiedForeground) ||
                (TbDescription.Foreground == ModifiedForeground) ||
                (TbImagePath.Foreground == ModifiedForeground) ||
                (CbStartupType.Foreground == ModifiedForeground);
        }

        public void ApplyChanges(object context)
        {

        }
    }
}
