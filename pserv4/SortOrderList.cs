using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace pserv4
{
    internal class SortOrderListElement
    {
        public readonly ListSortDirection Direction;
        public readonly string Key;

        public SortOrderListElement(string key, ListSortDirection direction)
        {
            Key = key;
            Direction = direction;
        }
    }

    internal class SortOrderList
    {
        public readonly List<SortOrderListElement> Items = new List<SortOrderListElement>();

        public ListSortDirection LastSortDirection
        {
            get
            {
                if( Items.Count > 0 )
                {
                    return Items[0].Direction;
                }
                return ListSortDirection.Ascending;
            }
        }

        private string SafeGetString(int n)
        {
            if( n < Items.Count )
            {
                SortOrderListElement item = Items[n];
                
                string header = (item.Direction == ListSortDirection.Ascending) ? "1" : "0";
                return header + item.Key;
            }
            return "";
        }

        public void Clear()
        {
            if( Items.Count > 0 )
            {
                Items.Clear();
                App.Settings.LastSortOrder0 = "";
                App.Settings.LastSortOrder1 = "";
                App.Settings.LastSortOrder2 = "";
                App.Settings.LastSortOrder3 = "";
                App.Settings.LastSortOrder4 = "";
            }
        }

        private void SaveToSettings()
        {
            App.Settings.LastSortOrder0 = SafeGetString(0);
            App.Settings.LastSortOrder1 = SafeGetString(1);
            App.Settings.LastSortOrder2 = SafeGetString(2);
            App.Settings.LastSortOrder3 = SafeGetString(3);
            App.Settings.LastSortOrder4 = SafeGetString(4);
        }

        public void Push(string key, ListSortDirection direction)
        {
            SortOrderListElement item = new SortOrderListElement(key, direction);
            RemoveExisting(item);
            Items.Insert(0, item);
            while( Items.Count > 5 )
            {
                Items.RemoveAt(Items.Count - 1);
            }
            SaveToSettings();
        }
                        
       private void RemoveExisting(SortOrderListElement element)
        {
            for (int i = 0; i < Items.Count; ++i)
            {
                if (Items[i].Key.Equals(element.Key))
                {
                    Items.RemoveAt(i);
                    i--;
                }
            }
        }

        private void AddUnique(SortOrderListElement element)
        {
            RemoveExisting(element);
            Items.Add(element);
        }

        public void LoadFromSettings()
        {
            AddPackedItem(App.Settings.LastSortOrder0);
            AddPackedItem(App.Settings.LastSortOrder1);
            AddPackedItem(App.Settings.LastSortOrder2);
            AddPackedItem(App.Settings.LastSortOrder3);
            AddPackedItem(App.Settings.LastSortOrder4);
        }

        private void AddPackedItem(string s)
        {
            if( !string.IsNullOrEmpty(s) )
            {
                string key = s.Substring(1);
                ListSortDirection direction = (s[0] == '1') ? ListSortDirection.Ascending : ListSortDirection.Descending;

                AddUnique(new SortOrderListElement(key, direction));
            }
        }
    }
}
