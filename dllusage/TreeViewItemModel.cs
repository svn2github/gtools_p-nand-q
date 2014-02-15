using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace dllusage
{
    public class TreeViewItemModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public object Tag;
        
        public void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public string Key { get; private set; }

        public bool IsValid;
        public bool IsNew;

        public string Name { get; private set; }

        public bool AddItem(string key, object tag)
        {
            foreach(TreeViewItemModel existingItem in Items)
            {
                if( existingItem.Key.Equals(key) )
                {
                    existingItem.IsValid = true;
                    return false;
                }
            }
            Items.Add(new TreeViewItemModel(key, tag));
            NotifyPropertyChanged("Items");
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

        public TreeViewItemModel(string name, object tag)
        {
            IsValid = true;
            IsNew = true;
            Name = name;
            Tag = tag;
            Key = Name;
        }
    }
}
