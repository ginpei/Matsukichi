using System;
using System.Linq;
using System.Diagnostics;  // Process
using System.Management;  // ManagementObjectSearcher (System.Management.dll)
using System.Runtime.InteropServices;  // DllImport

namespace Matsukichi
{
    public partial class AppInfo
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public Process process;
        public string processName;
        public string appName;
        public string screenName;

        public AppInfo(Process proc)
        {
            process = proc;
            processName = proc.ProcessName.ToLower();
            screenName = getAppName(proc);
            appName = screenName.ToLower();
        }

        /**
         * @see http://stackoverflow.com/questions/22201752/how-to-get-active-window-app-name-as-shown-in-task-manager
         */
        private string getAppName(Process p)
        {
            string name = "";
            var processname = p.ProcessName;
            string fileName = "";
            int pid = p.Id;

            if (String.IsNullOrEmpty(processname))
            {
                return "";
            }

            switch (processname)
            {
                case "explorer": //metro processes
                case "WWAHost":
                    return "";
                default:
                    break;
            }
            string wmiQuery = string.Format("SELECT ProcessId, ExecutablePath FROM Win32_Process WHERE ProcessId LIKE '{0}'", pid.ToString());
            var pro = new ManagementObjectSearcher(wmiQuery).Get().Cast<ManagementObject>().FirstOrDefault();
            fileName = (string)pro["ExecutablePath"];

            if (String.IsNullOrEmpty(fileName))
            {
                return "";
            }

            FileVersionInfo myFileVersionInfo;
            try
            {
                // Get the file version
                myFileVersionInfo = FileVersionInfo.GetVersionInfo(fileName);
            }
            catch
            {
                return "";
            }

            // Get the file description
            name = myFileVersionInfo.FileDescription;
            //if (name == "")
            //    name = GetTitle(handle);

            if (String.IsNullOrEmpty(name))
            {
                return "";
            }

            return name;
        }

        public bool isMatch(string filter)
        {
            if (appName.IndexOf(filter.ToLower()) >= 0 || processName.IndexOf(filter.ToLower()) >= 0)
            {
                return true;
            }

            return false;
        }

        /**
         * @see http://stackoverflow.com/questions/2315561/correct-way-in-net-to-switch-the-focus-to-another-application
         */
        public void focus()
        {
            SetForegroundWindow(process.MainWindowHandle);
        }
    }
}
