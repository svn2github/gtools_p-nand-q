using System;
using System.Collections;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Xml;

namespace pserv4
{
    public abstract class DataController
    {
        public abstract IEnumerable<DataObjectColumn> Columns { get; }
        private readonly string ControllerName;
        private readonly string ItemName;

        public DataController(string controllerName, string itemName)
        {
            ItemName = itemName;
            ControllerName = controllerName;
        }

        public ListView MainListView;

        /// <summary>
        /// Given the input list, refresh it with the current object list 
        /// </summary>
        /// <param name="objects"></param>
        public abstract void Refresh(ObservableCollection<DataObject> objects);
        
        /// <summary>
        /// Return the context menu used for all items on this list
        /// </summary>
        public virtual ContextMenu ContextMenu
        {
            get
            {
                return null;
            }
        }

        public virtual void OnContextMenuOpening(System.Collections.IList selectedItems, ContextMenu menu)
        {
            // default implementation: do nothing
        }

        public virtual void SaveAsXml(string filename, System.Collections.IList items)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = Encoding.UTF8;
            settings.IndentChars = "\t";
            settings.NewLineChars = "\r\n";

            StringBuilder buffer = new StringBuilder();
            using (XmlWriter xtw = (filename == null) ? XmlWriter.Create(buffer, settings) : XmlWriter.Create(filename, settings))
            {
                xtw.WriteStartDocument();
                xtw.WriteStartElement(ControllerName);
                xtw.WriteAttributeString("version", "3.0");
                xtw.WriteAttributeString("count", items.Count.ToString());
                OnSaveAsXML(xtw, items);
                xtw.WriteEndElement();
                xtw.Close();
            }
            if (filename == null)
            {
                Clipboard.SetText(buffer.ToString());
            }        
        }

        protected virtual void OnSaveAsXML(XmlWriter xtw, System.Collections.IList items)
        {
            foreach(DataObject o in items)
            {
                xtw.WriteStartElement(ItemName);
                xtw.WriteAttributeString("id", o.InternalID);

                Type t = o.GetType();

                foreach (DataObjectColumn c in Columns)
                {
                    if( !c.BindingName.Equals("InternalID"))
                    {
                        object item = t.GetProperty(c.BindingName).GetValue(o, null);
                        if( item != null)
                        {
                            string value = item as string;
                            if (value == null)
                                value = item.ToString();

                            xtw.WriteElementString(c.BindingName, value);
                        }
                    }
                }
                xtw.WriteEndElement();
            }
        }
    }
}
