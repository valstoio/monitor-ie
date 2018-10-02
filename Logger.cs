using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace vMonitor
{

    public enum LogPrefix
    {
        ERROR, WARNING, INFO
    };

    public class Logger
    {
        private static Logger instance = new Logger();

        private Logger()
        {
            
        }

        public static Logger Instance { get { return instance; } }

        private void WriteToFile(string content)
        {
            try
            {
                var path = Path.Combine(Utilities.GetLocalAppDataLowPath(), "Valsto"); 
                var dirInfo = new DirectoryInfo(path);
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                }
                path = Path.Combine(path, "vmonitor.log");

                File.WriteAllText(path, content);
            }
            catch (IOException ex)
            {
                Debug.WriteLine("Exception caught while loggin message: {0}", ex.Message);
            }
        }

        private void LogMessage(LogPrefix prefix, string message, params object[] args)
        {
            string msg = string.Format(message, args);
            WriteToFile(string.Format("{0}: {1}\n", prefix.ToString(), msg));

        }

        private void LogMessage(LogPrefix prefix, string message)
        {
            WriteToFile(string.Format("{0}: {1}\n", prefix.ToString(), message));
        }

        public void LogError(string message)
        {
            this.LogMessage(LogPrefix.ERROR, message);
        }

        public void LogError(string message, params object[] args)
        {
            this.LogMessage(LogPrefix.ERROR, message, args);
        }

        public void LogInfo(string message)
        {
            this.LogMessage(LogPrefix.INFO, message);
        }

        public void LogInfo(string message, params object[] args)
        {
            this.LogMessage(LogPrefix.INFO, message, args);
        }

        public void LogWarning(string message)
        {
            this.LogMessage(LogPrefix.WARNING, message);
        }

        public void LogWarning(string message, params object[] args)
        {
            this.LogMessage(LogPrefix.WARNING, message, args);
        }
    }
}
