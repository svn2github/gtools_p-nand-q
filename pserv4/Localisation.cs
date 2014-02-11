using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pserv4
{
    public static class Localisation
    {
        public static string TimeSpanToString(TimeSpan ts)
        {
            return string.Format("{0}:{1}:{2}",
                ts.Hours.ToString("D").PadLeft(2, '0'),
                ts.Minutes.ToString("D").PadLeft(2, '0'),
                ts.Seconds.ToString("D").PadLeft(2, '0'));
        }

        public static string BytesToSize(long bytes)
        {
            if (bytes < 1024)
            {
                return string.Format("{0} Bytes", bytes);
            }

            decimal v = ((decimal)bytes) / 1024.0m;
            if (v < 1024)
            {
                return string.Format("{0} KB", v.ToString("F2"));
            }
            v /= 1024;
            if (v < 1024)
            {
                return string.Format("{0} MB", v.ToString("F2"));
            }
            v /= 1024;
            return string.Format("{0} GB", v.ToString("F2"));
        }
    }
}
