using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace tikumo
{
    public class DirectoryItem : DataObject
    {

        public DirectoryItem(string directory, string filename, string key)
            :   base(key)
        {
            string pathname = Path.Combine(directory, filename);
        }
    }
}
