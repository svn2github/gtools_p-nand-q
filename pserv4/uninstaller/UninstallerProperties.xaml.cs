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
using System.Diagnostics;
using Microsoft.Win32;

namespace pserv4.uninstaller
{
    /// <summary>
    /// Interaction logic for UninstallerProperties.xaml
    /// </summary>
    public partial class UninstallerProperties : UserControl, IDataObjectDetails
    {
        private SolidColorBrush UnmodifiedForeground;
        private SolidColorBrush ModifiedForeground;

        private bool First;
        private readonly UninstallerDataObject UDO;
        private TabItem TabItem;
        private string ApplicationName;
        private string InstallLocation;
        private string Version;
        private string Publisher;
        private string HelpLink;
        private string AboutLink;
        private string Action;
        private string ModifyPath;

        public string Caption
        {
            get
            {
                return UDO.ApplicationName;
            }
        }

        public UninstallerProperties(UninstallerDataObject udo)
        {
            InitializeComponent();
            First = true;
            UDO = udo;
            ModifiedForeground = new SolidColorBrush(Colors.Blue);
            UnmodifiedForeground = new SolidColorBrush(Colors.Black);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (First)
            {
                TbUniqueID.Text = UDO.InternalID;
                RevertChanges();
                First = false;
            }
            else
            {
                TbApplicationName.Text = ApplicationName;
                TbInstallLocation.Text = InstallLocation;
                TbVersion.Text = Version;
                TbPublisher.Text = Publisher;
                TbHelpLink.Text = HelpLink;
                TbAboutLink.Text = AboutLink;
                TbAction.Text = Action;
                TbModifyPath.Text = ModifyPath;
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            ApplicationName = TbApplicationName.Text;
            InstallLocation = TbInstallLocation.Text;
            Version = TbVersion.Text;
            Publisher = TbPublisher.Text;
            HelpLink = TbHelpLink.Text;
            AboutLink = TbAboutLink.Text;
            Action = TbAction.Text;
            ModifyPath = TbModifyPath.Text;
        }

        private void OnBrowseFilename(object sender, RoutedEventArgs e)
        {
        }

        private void OnShowHelpLink(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(TbHelpLink.Text);
            }
            catch (Exception)
            {

            }
        }

        private void OnShowAboutLink(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(TbAboutLink.Text);
            }
            catch (Exception)
            {

            }
        }

        private void OnShowRegistry(object sender, RoutedEventArgs e)
        {
            string key = string.Format("{0}\\{1}\\{2}", 
                (UDO.BasedInLocalMachine ? "HKEY_LOCAL_MACHINE" : "HKEY_CURRENT_USER"),
                UninstallerDataController.UNINSTALLER_SECTION,
                UDO.InternalID);

            using (RegistryKey hkKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Applets\Regedit", true))
            {
                hkKey.SetValue("Lastkey", key);
            }
            try
            {
                Process.Start("regedit.exe");
            }
            catch (Exception)
            {

            }
        }

        private void TbApplicationName_TextChanged(object sender, TextChangedEventArgs e)
        {
            TbApplicationName.Foreground = TbApplicationName.Text.Equals(UDO.ApplicationName) ? UnmodifiedForeground : ModifiedForeground;
            TabItem.Foreground = HasAnyChanges() ? ModifiedForeground : UnmodifiedForeground;
        }

        private void TbModifyPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            TbModifyPath.Foreground = TbModifyPath.Text.Equals(UDO.ModifyPath) ? UnmodifiedForeground : ModifiedForeground;
            TabItem.Foreground = HasAnyChanges() ? ModifiedForeground : UnmodifiedForeground;
        }

        private void TbInstallLocation_TextChanged(object sender, TextChangedEventArgs e)
        {
            TbInstallLocation.Foreground = TbInstallLocation.Text.Equals(UDO.InstallLocation) ? UnmodifiedForeground : ModifiedForeground;
            BrowseFilename.IsEnabled = TbInstallLocation.Text.Length > 0;
            TabItem.Foreground = HasAnyChanges() ? ModifiedForeground : UnmodifiedForeground;
        }

        private void TbVersion_TextChanged(object sender, TextChangedEventArgs e)
        {
            TbVersion.Foreground = TbVersion.Text.Equals(UDO.Version) ? UnmodifiedForeground : ModifiedForeground;
            TabItem.Foreground = HasAnyChanges() ? ModifiedForeground : UnmodifiedForeground;
        }

        private void TbPublisher_TextChanged(object sender, TextChangedEventArgs e)
        {
            TbPublisher.Foreground = TbPublisher.Text.Equals(UDO.Publisher) ? UnmodifiedForeground : ModifiedForeground;
            TabItem.Foreground = HasAnyChanges() ? ModifiedForeground : UnmodifiedForeground;
        }

        private void TbHelpLink_TextChanged(object sender, TextChangedEventArgs e)
        {
            TbHelpLink.Foreground = TbHelpLink.Text.Equals(UDO.HelpLink) ? UnmodifiedForeground : ModifiedForeground;
            ShowHelpLink.IsEnabled = TbHelpLink.Text.Length > 0;
            TabItem.Foreground = HasAnyChanges() ? ModifiedForeground : UnmodifiedForeground;
        }

        private void TbAboutLink_TextChanged(object sender, TextChangedEventArgs e)
        {
            TbAboutLink.Foreground = TbAboutLink.Text.Equals(UDO.AboutLink) ? UnmodifiedForeground : ModifiedForeground;
            ShowAboutLink.IsEnabled = TbAboutLink.Text.Length > 0;
            TabItem.Foreground = HasAnyChanges() ? ModifiedForeground : UnmodifiedForeground;
        }

        private void TbAction_TextChanged(object sender, TextChangedEventArgs e)
        {
            TbAction.Foreground = TbAction.Text.Equals(UDO.Action) ? UnmodifiedForeground : ModifiedForeground;
            TabItem.Foreground = HasAnyChanges() ? ModifiedForeground : UnmodifiedForeground;
        }

        public void RevertChanges()
        {
            ApplicationName = TbApplicationName.Text = UDO.ApplicationName;
            InstallLocation = TbInstallLocation.Text = UDO.InstallLocation;
            BrowseFilename.IsEnabled = TbInstallLocation.Text.Length > 0;

            Version = TbVersion.Text = UDO.Version;
            Publisher = TbPublisher.Text = UDO.Publisher;
            
            HelpLink = TbHelpLink.Text = UDO.HelpLink;
            ShowHelpLink.IsEnabled = TbHelpLink.Text.Length > 0;

            AboutLink = TbAboutLink.Text = UDO.AboutLink;
            ShowAboutLink.IsEnabled = TbAboutLink.Text.Length > 0;
            Action = TbAction.Text = UDO.Action;

            ModifyPath = TbModifyPath.Text = UDO.ModifyPath;
        }

        public void BindTabItem(TabItem tabItem)
        {
            TabItem = tabItem;
        }

        public bool HasAnyChanges()
        {
            return
                (TbApplicationName.Foreground == ModifiedForeground) ||
                (TbInstallLocation.Foreground == ModifiedForeground) ||
                (TbVersion.Foreground == ModifiedForeground) ||
                (TbPublisher.Foreground == ModifiedForeground) ||
                (TbHelpLink.Foreground == ModifiedForeground) ||
                (TbAboutLink.Foreground == ModifiedForeground) ||
                (TbModifyPath.Foreground == ModifiedForeground) ||
                (TbAction.Foreground == ModifiedForeground);
        }

        public void ApplyChanges(object context)
        {

        }
    }
}
