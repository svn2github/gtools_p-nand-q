using System;
using System.ComponentModel;
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
using System.Diagnostics;
using log4net;
using System.Reflection;

namespace pserv4
{
    /// <summary>
    /// Interaction logic for LongRunningFunctionWindow.xaml
    /// </summary>
    public partial class LongRunningFunctionWindow : Window
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly BackgroundWorker Worker = new BackgroundWorker();
        private readonly BackgroundAction Action;

        public LongRunningFunctionWindow(BackgroundAction action, string title)
        {
            InitializeComponent();
            Title = title;
            Action = action;            
            Worker.DoWork += worker_DoWork;
            Worker.WorkerSupportsCancellation = true;
            Action.Bind(Worker);
            Worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Log.InfoFormat("BEGIN LongRunningFunctionWindow.worker_DoWork()");
            Action.Setup(SetOutputText);
            try
            {
                Action.DoWork();
            }
            catch(Exception ex)
            {
                Log.Error(string.Format("Exception caught while processing '{0}'", Action), ex);
            }
            if (Worker.CancellationPending)
            {
                e.Cancel = true;
            }
            Log.InfoFormat("END LongRunningFunctionWindow.worker_DoWork()");
        }

        private void SetOutputText(string message)
        {
            Log.InfoFormat("SetOutputText: {0}", message);
            this.Dispatcher.Invoke(
                new BackgroundAction.SetOutputTextDelegate(SetOutputTextInUIThread),
                message);
        }

        private void SetOutputTextInUIThread(string message)
        {
            TbOutput.Text = message;
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Log.Info("worker_RunWorkerCompleted() called");
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Worker.RunWorkerAsync();
        }

        private void OnCancelButton_Click(object sender, RoutedEventArgs e)
        {
            Log.Info("OnCancelButton_Click() called");
            try
            {
                Worker.CancelAsync();
                CancelButton.IsEnabled = false;
            }
            catch (Exception ex)
            {
                Log.Error("Exception caught while pressing the cancel button", ex);
            }
            Log.Info("OnCancelButton_Click() done");
        }
    }
}
