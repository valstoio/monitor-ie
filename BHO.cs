using Microsoft.Win32;
using mshtml;
using Newtonsoft.Json;
using SHDocVw;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using vMonitor.UrlHistory;

namespace vMonitor
{

    [
    ComVisible(true)
    , Guid("CD04D1E5-6416-45E7-85F4-2A3DC324EC7A")
    , ClassInterface(ClassInterfaceType.None)
    ]
    public class BHO : IObjectWithSite
    {
        public static string BHO_KEY_NAME = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Browser Helper Objects";
        private WebBrowser webBrowser;

        public BHO()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(MyResolveEventHandler);
            Debug.WriteLine("Initialize Valsto Monitor");
            InitializeTimer();
        }
        private static Assembly MyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            return typeof(BHO).Assembly;
        }

        private void InitializeTimer()
        {
            Debug.WriteLine("Initialize Timer");
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 1000 * 60 * Constants.MINUTES;
            aTimer.Enabled = true;

            Debug.WriteLine("Timer Initialized");
        }


        // Specify what you want to happen when the Elapsed event is raised.
        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Debug.WriteLine("Send Data to valsto server!");
  
            try
            {
                UrlHistoryTask task = new UrlHistoryTask();
                task.Run();
            }
            catch (Exception ex)
            {
                Debug.Fail("Error loading setting", ex.Message);
            }
        }

        public void OnDocumentComplete(object pDisp, ref object URL)
        {

            if (URL != null && URL.ToString() == "about:blank")
                return;

            try
            {
                HTMLDocument document = (HTMLDocument)webBrowser.Document;

                Debug.WriteLine(document.url);

                if (document.url.Contains("valsto.com"))
                {
                    var cookies = CookieHelpers.GetUriCookieContainer(document.url);
                    var cookieList = cookies.GetCookies(new Uri(document.url));
                    //httpWebRequest.CookieContainer.SetCookies(webBrowser.Document.Url,GetGlobalCookies(webBrowser.Document.Url.AbsoluteUri));

                    if (cookieList.Count > 0)
                    {
                        string vLogin = (cookieList["vLogin"] != null && !String.IsNullOrEmpty(cookieList["vLogin"].Value)) ? cookieList["vLogin"].Value : "";

                        if (!String.IsNullOrEmpty(vLogin))
                        {
                            Debug.WriteLine("vLogin found: " + vLogin);

                            SettingsManager manager = new SettingsManager();
                            Settings setting = manager.Load();

                            if (setting == null || setting.Email != vLogin || String.IsNullOrEmpty(setting.VMonitorId))
                            {

                                Debug.WriteLine("Create a new vMonitor id for" + vLogin);
                                var result = ApiServices.RegisterMonitor();

                                setting = new Settings { Email = vLogin, VMonitorId = result.Data };
                                Debug.WriteLine("Save Setting with email " + vLogin);

                                manager.Save(setting);

                                Debug.WriteLine("Setting saved");

                            }

                            Debug.WriteLine("VMonitorId found: " + setting.VMonitorId);


                        }
                    
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }


        [ComRegisterFunction]
        public static void RegisterBHO(Type type)
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(BHO_KEY_NAME, true);
            RegistryKey ourKey = null;

            try
            {
                if (registryKey == null)
                {
                    registryKey = Registry.LocalMachine.CreateSubKey(BHO_KEY_NAME);

                    if (registryKey == null)
                    {
                        throw new Exception(string.Format("Error opening registry key {0}", BHO_KEY_NAME));
                    }
                }

                string guid = type.GUID.ToString("B");
                ourKey = registryKey.OpenSubKey(guid);
                if (ourKey == null)
                {
                    ourKey = registryKey.CreateSubKey(guid);

                    if (ourKey == null)
                    {
                        throw new Exception(string.Format("Error creating registry subkey {0}", guid));
                    }
                }

                ourKey.SetValue("NoExplorer", 1, RegistryValueKind.DWord);
            }
            finally
            {
                if (registryKey != null)
                {
                    registryKey.Close();
                }

                if (ourKey != null)
                {
                    ourKey.Close();
                }
            }
        }

        [ComUnregisterFunction]
        public static void UnregisterBHO(Type type)
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(BHO_KEY_NAME, true);

            string guid = type.GUID.ToString("B");

            if (registryKey != null)
            {
                registryKey.DeleteSubKey(guid, false);
            }
        }
         
 
        #region IObjectWithSite  Members
        public int SetSite(object site)
        {
            if (site != null)
            {
                webBrowser = (SHDocVw.WebBrowser)site;
                webBrowser.DocumentComplete +=
                    new DWebBrowserEvents2_DocumentCompleteEventHandler(this.OnDocumentComplete);
            }
            else
            {
                try
                {
                    if (webBrowser != null)
                    {
                        webBrowser.DocumentComplete -=
                            new DWebBrowserEvents2_DocumentCompleteEventHandler(this.OnDocumentComplete);
                    }

                }
                catch (Exception ex)
                {

                }

                webBrowser = null;
            }

            return 0;
        }

        public int GetSite(ref Guid guid, out IntPtr ppvSite)
        {
            IntPtr iUnknownForObject = Marshal.GetIUnknownForObject(this.webBrowser);
            int hr = Marshal.QueryInterface(iUnknownForObject, ref guid, out ppvSite);

            Marshal.Release(iUnknownForObject);

            return hr;
        }
        #endregion
    }
}
