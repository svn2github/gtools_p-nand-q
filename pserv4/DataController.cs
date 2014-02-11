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
using pserv4.Properties;

namespace pserv4
{
    public abstract class DataController
    {
        public abstract IEnumerable<DataObjectColumn> Columns { get; }
        private readonly string ControllerName;
        private readonly string ItemName;
        protected bool HasProperties;

        public bool IsControlStartEnabled { get; protected set; }
        public bool IsControlStopEnabled { get; protected set; }
        public bool IsControlRestartEnabled { get; protected set; }
        public bool IsControlPauseEnabled { get; protected set; }
        public bool IsControlContinueEnabled { get; protected set; }

        public readonly string ControlStartDescription;
        public readonly string ControlStopDescription;
        public readonly string ControlRestartDescription;
        public readonly string ControlPauseDescription;
        public readonly string ControlContinueDescription;

        public DataController(
            string controllerName, 
            string itemName,
            string controlStartDescription,
            string controlStopDescription,
            string controlRestartDescription,
            string controlPauseDescription,
            string controlContinueDescription
            )
        {
            ItemName = itemName;
            HasProperties = false;
            ControllerName = controllerName;
            ControlStartDescription = controlStartDescription;
            ControlStopDescription = controlStopDescription;
            ControlRestartDescription = controlRestartDescription;
            ControlPauseDescription = controlPauseDescription;
            ControlContinueDescription = controlContinueDescription;
        }

        public ListView MainListView;

        /// <summary>
        /// Given the input list, refresh it with the current object list 
        /// </summary>
        /// <param name="objects"></param>
        public abstract void Refresh(ObservableCollection<DataObject> objects);

        protected void SetMenuItemEnabled(ContextMenu menu, int index, bool enabled)
        {
            MenuItem mi = menu.Items[index] as MenuItem;
            if (mi != null)
            {
                mi.IsEnabled = enabled;
            }
        }

        protected void AppendMenuItem(ContextMenu menu, string header, BitmapImage[] images, bool enabled, RoutedEventHandler callback)
        {
            MenuItem mi = new MenuItem();
            mi.Header = header;
            if (images != null)
            {
                Image i = new Image();
                i.Source = enabled ? images[1] : images[0];
                mi.Icon = i;
            }
            mi.IsEnabled = enabled;
            mi.Click += callback;
            menu.Items.Add(mi);
        }

        public virtual void ApplyChanges(List<IDataObjectDetails> changedItems)
        {
            // default implementation: do nothing
        }

        protected void AppendMenuItem(ContextMenu menu, string header, string imageName, RoutedEventHandler callback)
        {
            MenuItem mi = new MenuItem();
            mi.Header = header;
            Image i = new Image();
            string filename = string.Format(@"pack://application:,,,/images/{0}.png", imageName);
            i.Source = new BitmapImage(new Uri(filename));
            mi.Icon = i;
            mi.Click += callback;
            menu.Items.Add(mi);
        }


        /// <summary>
        /// Return the context menu used for all items on this list
        /// </summary>
        public virtual ContextMenu ContextMenu
        {
            get
            {
                if (string.IsNullOrEmpty(ControlStartDescription))
                    return null;

                ContextMenu menu = new ContextMenu();
                AppendMenuItem(
                    menu,
                    ControlStartDescription,
                    MainWindow.BIStart,
                    IsControlStartEnabled,
                    OnControlStart);
                AppendMenuItem(
                    menu,
                    ControlStopDescription,
                    MainWindow.BIStop,
                    IsControlStopEnabled,
                    OnControlStop);
                AppendMenuItem(
                    menu,
                    ControlRestartDescription,
                    MainWindow.BIRestart,
                    IsControlRestartEnabled,
                    OnControlRestart);
                AppendMenuItem(
                    menu,
                    ControlPauseDescription,
                    MainWindow.BIPause,
                    IsControlPauseEnabled,
                    OnControlPause);
                AppendMenuItem(
                    menu,
                    ControlContinueDescription,
                    MainWindow.BIContinue,
                    IsControlContinueEnabled,
                    OnControlContinue);

                return menu;
            }
        }

        public virtual void OnSelectionChanged(IList selectedItems)
        {
        }

        public virtual void OnContextMenuOpening(IList selectedItems, ContextMenu menu)
        {
        }

        public virtual void SaveAsXml(string filename, IList items)
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

        public void ShowProperties(object sender, RoutedEventArgs e)
        {
            if(HasProperties && (MainListView.SelectedItems.Count > 0))
            {
                new PropertiesWindow().Show();
            }
        }

        public virtual UserControl CreateDetailsPage(DataObject o)
        {
            return null;
        }

        public virtual void OnControlStart(object sender, RoutedEventArgs e)
        {
            Trace.TraceWarning("Warning: OnControlStart not implemented for {0}", this);
        }

        public virtual void OnControlStop(object sender, RoutedEventArgs e)
        {
            Trace.TraceWarning("Warning: OnControlStop not implemented for {0}", this);
        }

        public virtual void OnControlRestart(object sender, RoutedEventArgs e)
        {
            Trace.TraceWarning("Warning: OnControlRestart not implemented for {0}", this);
        }

        public virtual void OnControlPause(object sender, RoutedEventArgs e)
        {
            Trace.TraceWarning("Warning: OnControlPause not implemented for {0}", this);
        }

        public virtual void OnControlContinue(object sender, RoutedEventArgs e)
        {
            Trace.TraceWarning("Warning: OnControlContinue not implemented for {0}", this);
        }


    }
}

