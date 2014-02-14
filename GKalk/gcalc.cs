using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;

namespace GKalk
{
    public static class gcalc
    {
        [DllImport("gcalc.dll")]
        public extern static void gcalc_object_release(UIntPtr o);

        [DllImport("gcalc.dll")]
        public extern static UIntPtr gcalc_create();

        [DllImport("gcalc.dll", CharSet=CharSet.Ansi)]
        public extern static void gcalc_cplusplus_bridge_for_gkalk(UIntPtr calc, string input_text, StringBuilder output, int maxsize);

    }
}
