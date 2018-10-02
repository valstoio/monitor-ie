using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vMonitor
{
    internal static class Constants
    {
#if LOCAL
        public static string BASE_VMONITOR_URL = "***API***";
#else

        public static string BASE_VMONITOR_URL = "***API***";
#endif
        public static int MINUTES = 20;
    }
}
