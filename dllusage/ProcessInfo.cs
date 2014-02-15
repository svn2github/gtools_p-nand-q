using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace dllusage
{
    public class ProcessInfo : IComparable<ProcessInfo>
    {
        public readonly string DisplayName;
        public readonly string PathName;
        public readonly string Directory;
        public readonly string FileName;
        public List<string> Dependencies;

        public ProcessInfo(string name, int processid, MainViewType dm)
        {
            PathName = name;
            Directory = Path.GetDirectoryName(name);
            FileName = Path.GetFileName(name);
            Dependencies = new List<string>();
            if (Directory.Length == 0)
            {
                DisplayName = string.Format("{0} [{1}]",
                        FileName,
                        processid);
            }
            else
            {
                if (dm == MainViewType.ListByEXEPath)
                {
                    DisplayName = string.Format("{1}\\{0} [{2}]",
                            FileName,
                            Directory,
                            processid);
                }
                else
                {
                    DisplayName = string.Format("{0} in {1} [{2}]",
                            FileName,
                            Directory,
                            processid);
                }
            }
        }

        public ProcessInfo(string name, MainViewType dm)
        {
            PathName = name;
            Directory = Path.GetDirectoryName(name);
            FileName = Path.GetFileName(name);
            Dependencies = new List<string>();
            if (dm == MainViewType.ListByDLLPath)
            {
                DisplayName = string.Format("{1}\\{0}",
                        FileName,
                        Directory);
            }
            else
            {
                DisplayName = string.Format("{0} in {1}",
                        FileName,
                        Directory);
            }
        }

        public int CompareTo(ProcessInfo obj)
        {
            return DisplayName.CompareTo(obj.DisplayName);
        }
    }

}
