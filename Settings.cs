using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vMonitor
{
    [Serializable]
    public class Settings
    {
        public String Email { get; set; }

        public String VMonitorId { get; set; }

        public DateTime? LastCheckinDate { get; set; }
    }
}
