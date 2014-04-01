using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
using log4net;
using System.Reflection;

namespace tikumo
{
    public class DirectoryController : DataController
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static List<DataObjectColumn> ActualColumns;
        private readonly ListView ListView;
        private readonly GridView GridView;
        private string CurrentDirectory;

        public DirectoryController(ListView listView, GridView gridView)
        {
            ListView = listView;
            GridView = gridView;

            // create builtin bindings
            ListView.MouseDoubleClick += ListView_MouseDoubleClick;
            ListView.SelectionChanged += ListView_SelectionChanged;
            ListView.PreviewMouseWheel += ListView_PreviewMouseWheel;
            ListView.KeyDown += ListView_KeyDown;

            // set data sources
            ListView.ItemsSource = Items;
        }

        void ListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            
        }

        void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
        }

        void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        void ListView_GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
        }

        private void ListView_KeyDown(object sender, KeyEventArgs e)
        {
        }

        public override IEnumerable<DataObjectColumn> Columns
        {
            get
            {
                if (ActualColumns == null)
                {
                    ActualColumns = new List<DataObjectColumn>();
                    ActualColumns.Add(new DataObjectColumn("Filename", "Name"));
                }
                return ActualColumns;
            }
        }

        public void ChangeDirectory(string newDirectory)
        {
            Log.InfoFormat("ChangeDirectory: {0}", newDirectory);
            if (CurrentDirectory != newDirectory)
            {
                CurrentDirectory = newDirectory;

                using(RefreshManager<DirectoryItem> refreshManager = new RefreshManager<DirectoryItem>(Items))
                {
                    foreach(string filename in Directory.GetFiles(newDirectory))
                    {
                        DirectoryItem directoryItem;
                        string key = filename.ToLower();
                        if( refreshManager.Contains(key, out directoryItem))
                        {
                            // todo: refresh this
                        }
                        else
                        {
                            Items.Add(new DirectoryItem(newDirectory, filename, key));
                        }
                    }
                }
            }
            Log.InfoFormat("ChangeDirectory: Items has now {0} elements", Items.Count);
        }
    }
}
