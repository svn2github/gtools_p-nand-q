using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using log4net;
using System.Reflection;

namespace dllusage
{
    public class TreeViewItemModel : INotifyPropertyChanged
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public event PropertyChangedEventHandler PropertyChanged;

        public object Tag;
        
        public void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public bool IsValid;
        public bool IsNew;

        public bool IsRunning { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsDuplicate { get; set; }

        public string Name { get; set; }

        public readonly string Key;


        public bool AddItem(string displayName, string key, object tag)
        {
            foreach(TreeViewItemModel existingItem in Items)
            {
                if (existingItem.Key.Equals(key))
                {
                    existingItem.IsValid = true;

                    // todo: maybe display name changed ?
                    return false;
                }
            }
            Items.Add(new TreeViewItemModel(displayName, key, tag));
            return true;
        }

        private readonly ObservableCollection<TreeViewItemModel> _Items = new ObservableCollection<TreeViewItemModel>();

        public ObservableCollection<TreeViewItemModel> Items 
        {
            get
            {
                return _Items;
            }
        }

        public TreeViewItemModel(string name, string key, object tag)
        {
            IsDuplicate = false;
            IsValid = true;
            IsNew = true;
            Name = name;
            Tag = tag;
            Key = key;
        }
    }
}
