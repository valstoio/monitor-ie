using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace vMonitor.UrlHistory
{
    class UrlHistoryTask
    {
        public UrlHistoryTask()
        {
        }

        public void Run()
        {
            SettingsManager manager = new SettingsManager();
            var setting = manager.Load();

            if (setting != null && !String.IsNullOrEmpty(setting.Email))
            {
                Debug.WriteLine("Send Data for: " + setting.Email);
                Debug.WriteLine("Last Checking Date: " + (setting.LastCheckinDate ?? DateTime.Now).ToString());

                UrlHistoryWrapperClass urlHistory = new UrlHistoryWrapperClass();
                UrlHistoryWrapperClass.STATURLEnumerator enumerator = urlHistory.GetEnumerator();
                enumerator.SetFilter("", STATURLFLAGS.STATURLFLAG_ISTOPLEVEL);

                List<STATURL> list = new List<STATURL>();
                enumerator.GetUrlHistory(list);

                Debug.WriteLine("Total data found: " + list.Count);

                DateTime lastCheckingDate =
                    setting.LastCheckinDate.HasValue ? setting.LastCheckinDate.Value.AddMinutes(-Constants.MINUTES) : DateTime.Now.AddMinutes(-Constants.MINUTES);

                var filteredList = list.Where(r => r.LastVisited >= lastCheckingDate).ToList();

                ApiServices.SendHistoryToValsto(setting.Email, setting.VMonitorId, filteredList);

                setting.LastCheckinDate = DateTime.Now;

                manager.Save(setting);
            }
        }

    }
}
