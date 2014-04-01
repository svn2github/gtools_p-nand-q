using System;
using System.Collections;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Xml;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using tikumo.Properties;
using log4net;
using System.Reflection;
using GSharpTools;

namespace tikumo
{
    public abstract class DataController
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public abstract IEnumerable<DataObjectColumn> Columns { get; }

        public readonly ObservableCollection<DataObject> Items = new ObservableCollection<DataObject>();

        public DataController()
        {
        }


    }
}

