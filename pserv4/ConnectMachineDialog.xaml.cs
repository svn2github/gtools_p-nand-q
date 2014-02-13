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
    /// Interaction logic for ConnectMachineDialog.xaml
    /// </summary>
    public partial class ConnectMachineDialog : Window
    {
        public readonly string CurrentMachine;
        public string SelectedMachine;

        public ConnectMachineDialog(string currentMachine)
        {
            InitializeComponent();
            CurrentMachine = currentMachine;
            if(Environment.MachineName.Equals(currentMachine, StringComparison.OrdinalIgnoreCase))
            {
                RbLocalMachine.IsChecked = true;
            }
            else
            {
                RbRemoteMachine.IsChecked = true;
                TbRemoteMachine.Text = currentMachine;
            }
        }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            if (RbLocalMachine.IsChecked.HasValue && RbLocalMachine.IsChecked.Value )
            {
                SelectedMachine = Environment.MachineName;
            }
            else
            {
                SelectedMachine = TbRemoteMachine.Text;
            }

            Close();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RbRemoteMachine_Click(object sender, RoutedEventArgs e)
        {
            RbLocalMachine.IsChecked = false;
            TbRemoteMachine.IsEnabled = true;
        }

        private void RbLocalMachine_Click(object sender, RoutedEventArgs e)
        {
            RbRemoteMachine.IsChecked = false;
            TbRemoteMachine.IsEnabled = false;
        }
    }
}
