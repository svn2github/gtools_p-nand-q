using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Collections;
using Microsoft.Win32;
using System.Windows.Threading;
using log4net;
using GSharpTools;
using GSharpTools.WPF;
using pserv4.Properties;

namespace pserv4
{
    /// <summary>
    /// This class is used to sort the list.
    /// The main argument is a stack of sort order arguments, to dis-ambiguate comparisons
    /// </summary>
    internal class SortDataObjects : IComparer
    {
        private readonly SortOrderList SortOrderList;

        public SortDataObjects(SortOrderList sortOrderList)
        {
            SortOrderList = sortOrderList;
        }

        public int Compare(object x, object y)
        {
            DataObject X = x as DataObject;
            DataObject Y = y as DataObject;
            if ((X == null) || (Y == null))
            {
                return 0;
            }
            int result = 0;
            ListSortDirection direction = ListSortDirection.Ascending;
            foreach (SortOrderListElement item in SortOrderList.Items)
            {
                object a = X.GetValueNamed(item.Key);
                object b = Y.GetValueNamed(item.Key);
                direction = item.Direction;

                string a_s = (a == null) ? "" : a.ToString();
                string b_s = (b == null) ? "" : b.ToString();

                int a_i = 0;
                int b_i = 0;

                if (int.TryParse(a_s, out a_i) && int.TryParse(b_s, out b_i))
                {
                    result = b_i - a_i;
                    if (result != 0)
                        break;
                }
                else
                {
                    result = a_s.CompareTo(b_s);
                    if (result != 0)
                        break;
                }
            }
            if (result < 0)
            {
                return (direction == ListSortDirection.Ascending) ? -1 : 1;
            }
            if (result > 0)
            {
                return (direction == ListSortDirection.Ascending) ? 1 : -1;
            }
            return 0;
        }
    }
}
