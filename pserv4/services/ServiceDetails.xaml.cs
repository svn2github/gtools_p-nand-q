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
    public partial class ServiceDetails : UserControl
    {
        private readonly ServiceDataObject SDO;

        public ServiceDetails(ServiceDataObject sdo)
        {
            InitializeComponent();
            SDO = sdo;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TbDisplayName.Text = SDO.DisplayName;
            TbDescription.Text = SDO.Description;
            TbImagePath.Text = SDO.BinaryPathName;
            CbStartupType.SelectedIndex = (int)SDO.StartType;
        }
    }
}
