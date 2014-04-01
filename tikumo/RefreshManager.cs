using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;
using log4net;
using System.Reflection;

namespace tikumo
{
    public class RefreshManager<T> : IDisposable
        where T: DataObject
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public readonly Dictionary<string, T> ExistingObjects = new Dictionary<string, T>();
        public readonly ObservableCollection<DataObject> Objects;

        public RefreshManager(ObservableCollection<DataObject> objects)
        {
            Objects = objects;
            foreach (DataObject o in objects)
            {
                T sdo = o as T;
                if (sdo != null)
                {
                    ExistingObjects[sdo.InternalID] = sdo;
                }
            }
        }

        public bool Contains(string internalID, out T result)
        {
            if (ExistingObjects.TryGetValue(internalID, out result))
            {
                ExistingObjects.Remove(internalID);
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            foreach (string key in ExistingObjects.Keys)
            {
                T sdo = ExistingObjects[key];
                Log.WarnFormat("Removing stale object {0}", sdo);
                Objects.Remove(sdo);
            }
        }
    }
}
