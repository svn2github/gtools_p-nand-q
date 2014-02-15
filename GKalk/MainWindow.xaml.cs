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
using System.IO;
using System.Diagnostics;
using GSharpTools;

namespace GKalk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int MAX_CAPACITY = 102400;
        private readonly StringBuilder Output;
        private readonly UIntPtr Calculator;
        private string LastKnownFileName;

        public MainWindow()
        {
            InitializeComponent();

            Output = new StringBuilder();
            Output.EnsureCapacity(MAX_CAPACITY);
            LastKnownFileName = "";
            Calculator = gcalc.gcalc_create();
            Title = string.Format("gkalk {0}", AppVersion.Get());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TbInput.Focus();
        }

        private void BtNew_Click(object sender, RoutedEventArgs e)
        {
            TbInput.Text = "";
            TbInput_Changed(TbInput, null);
        }

        private void BtOpen_Click(object sender, RoutedEventArgs args)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            // Set filter for file extension and default file extension 
            dialog.FileName = LastKnownFileName;
            dialog.DefaultExt = ".gkalk";
            dialog.Filter = "GKalk Files (*.gkalk)|*.gkalk|All Files (*.*)|*.*";

            // Display OpenFileDialog by calling ShowDialog method 
            bool? result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                try
                {
                    TbInput.Text = File.ReadAllText(dialog.FileName);
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.ToString());
                    Trace.TraceError(e.StackTrace);
                    Trace.TraceError("Unable to read text from {0}", dialog.FileName);
                }
            }
        }

        private void BtSave_Click(object sender, RoutedEventArgs args)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            // Set filter for file extension and default file extension 
            dialog.DefaultExt = ".gkalk";
            dialog.Filter = "GKalk Files (*.gkalk)|*.gkalk|All Files (*.*)|*.*";

            // Display OpenFileDialog by calling ShowDialog method 
            bool? result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                try
                {
                    File.WriteAllText(dialog.FileName, TbInput.Text);
                }
                catch(Exception e)
                {
                    Trace.TraceError(e.ToString());
                    Trace.TraceError(e.StackTrace);
                    Trace.TraceError("Unable to write text to {0}", dialog.FileName);
                }
            }
        }

        private void Zoom_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            bool handle = (Keyboard.Modifiers & ModifierKeys.Control) > 0;
            if (handle)
            {
                if (e.Delta > 0)
                    TbInput.FontSize += 1.0;
                else
                    TbInput.FontSize -= 1.0;

                if (TbInput.FontSize < 4.0)
                    TbInput.FontSize = 4.0;

                if (TbInput.FontSize > 60)
                    TbInput.FontSize = 60;

                TbOutput.FontSize = TbInput.FontSize;
            }
        }
        private string ProcessInputData(string line)
        {
            StringBuilder output = new StringBuilder();
            for( int i = line.Length-1; i >= 0; --i)
            {
                if (line[i] != '\r')
                {
                    output.Append(line[i]);
                }
                
            }
            return output.ToString();
        }

        private void TbInput_Changed(object sender, TextChangedEventArgs args)
        {
            gcalc.gcalc_cplusplus_bridge_for_gkalk(Calculator, TbInput.Text, Output, MAX_CAPACITY);
            TbOutput.Text = Output.ToString();
        }
    }
}
