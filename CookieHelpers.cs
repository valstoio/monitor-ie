using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace vMonitor
{
    class CookieHelpers
    {
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool InternetGetCookieEx(string pchURL, string pchCookieName, StringBuilder pchCookieData, ref uint pcchCookieData, int dwFlags, IntPtr lpReserved);
        const int INTERNET_COOKIE_HTTPONLY = 0x00002000;

        private static StringBuilder GetGlobalCookies(string uri)
        {
            uint datasize = 1024;
            StringBuilder cookieData = new StringBuilder((int)datasize);
            if (InternetGetCookieEx(uri, null, cookieData, ref datasize, INTERNET_COOKIE_HTTPONLY, IntPtr.Zero)
                && cookieData.Length > 0)
            {
                return cookieData;
            }
            else
            {
                return new StringBuilder();
            }
        }

        public static CookieContainer GetUriCookieContainer(string url)
        {
            CookieContainer cookies = null;
            StringBuilder cookieData = GetGlobalCookies(url);
            if (cookieData.Length > 0)
            {
                cookies = new CookieContainer();
                cookies.SetCookies(new Uri(url), cookieData.ToString().Replace(';', ','));
            }

            return cookies;

        }
    }
}
