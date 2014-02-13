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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace pserv4.services
{
    /// <summary>
    /// Interaction logic for ServiceDetails.xaml
    /// </summary>
    public partial class ServiceProperties : UserControl, IDataObjectDetails
    {
        private SolidColorBrush UnmodifiedForeground;
        private SolidColorBrush ModifiedForeground;

        private readonly string INTERNAL_PASSWORD_TEXT = "\x01\x08\x06\x00";

        private bool First;
        private readonly ServiceDataObject SDO;
        private TabItem TabItem;
        private string DisplayName;
        private string Description;
        private string BinaryPathName;
        private SC_START_TYPE StartType; 

        public ServiceProperties(ServiceDataObject sdo)
        {
            InitializeComponent();
            First = true;
            SDO = sdo;
            ModifiedForeground = new SolidColorBrush(Colors.Blue);
            UnmodifiedForeground = new SolidColorBrush(Colors.Black);
        }

        public string Caption
        {
            get
            {
                return SDO.InternalID;
            }
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
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".exe";
            dlg.Filter = "Executables (.exe)|*.exe";
            dlg.InitialDirectory = SDO.InstallLocation;
            dlg.FileName = SDO.BinaryPathName;

            bool? result = dlg.ShowDialog();
            if( result.HasValue && result.Value )
            {
                TbImagePath.Text = dlg.FileName;
            }
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

            if (SDO.IsSystemAccount)
            {
                RbSystemAccount.IsChecked = true;
                RbSystemAccount_Click(null, null);
            }
            else
            {
                RbLogonAs.IsChecked = true;
                TbAccountName.Text = SDO.User;
                RbLogonAs_Click(null, null);
            }

            BtInteractWithDesktop.IsChecked = ((SDO.ServiceType & SC_SERVICE_TYPE.SERVICE_INTERACTIVE_PROCESS) != 0);
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

        private void RbSystemAccount_Click(object sender, RoutedEventArgs e)
        {
            TbAccountName.IsEnabled = false;
            TbPassword.IsEnabled = false;
            TbPasswordConfirm.IsEnabled = false;
            TbPassword.Password = INTERNAL_PASSWORD_TEXT;
            TbPasswordConfirm.Password = INTERNAL_PASSWORD_TEXT;
        }

        private void RbLogonAs_Click(object sender, RoutedEventArgs e)
        {
            TbAccountName.IsEnabled = true;
            TbPassword.IsEnabled = true;
            TbPasswordConfirm.IsEnabled = true;
        }

        public void ApplyChanges(object context)
        {

        }
    }
}
