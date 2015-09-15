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
        public string path;
        public string appName;
        public string screenName;

        public AppInfo(Process proc)
        {
            process = proc;

            path = getProcPath(proc);
            screenName = getAppName(path);
            if (!string.IsNullOrEmpty(screenName))
            {
                appName = screenName.ToLower();
            }
        }

        public AppInfo(string path)
        {
            this.path = path;
            screenName = getAppName(path);
            appName = screenName.ToLower();
        }

        private string getProcPath(Process proc)
        {
            // get process information using WMI (Windows Management Instrumentation)
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(string.Format(
                "SELECT ProcessId, ExecutablePath " +
                "FROM Win32_Process " +
                "WHERE ProcessId LIKE '{0}'",
                proc.Id.ToString()
            ));
            ManagementObjectCollection searchResult = searcher.Get();
            ManagementObject procData = searchResult.Cast<ManagementObject>().FirstOrDefault();

            string path = (string)procData["ExecutablePath"];

            return path;
        }

        private string getAppName(string path)
        {
            try
            {
                // 'System.IO.FileNotFoundException' when path is "C:\\WINDOWS\\system32\\ApplicationFrameHost.exe"
                FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(path);
                string name = myFileVersionInfo.FileDescription;

                return name;
            }
            catch
            {
                return null;
            }
        }

        /**
         * @see http://stackoverflow.com/questions/22201752/how-to-get-active-window-app-name-as-shown-in-task-manager
         */
        #region not used
        //private string getAppName(Process p)
        //{
        //    string name = "";
        //    var processname = p.ProcessName;
        //    string fileName = "";
        //    int pid = p.Id;

        //    if (String.IsNullOrEmpty(processname))
        //    {
        //        return "";
        //    }

        //    switch (processname)
        //    {
        //        case "explorer": //metro processes
        //        case "WWAHost":
        //            return "";
        //        default:
        //            break;
        //    }
        //    string wmiQuery = string.Format("SELECT ProcessId, ExecutablePath FROM Win32_Process WHERE ProcessId LIKE '{0}'", pid.ToString());
        //    var pro = new ManagementObjectSearcher(wmiQuery).Get().Cast<ManagementObject>().FirstOrDefault();
        //    fileName = (string)pro["ExecutablePath"];

        //    if (String.IsNullOrEmpty(fileName))
        //    {
        //        return "";
        //    }

        //    FileVersionInfo myFileVersionInfo;
        //    try
        //    {
        //        // Get the file version
        //        myFileVersionInfo = FileVersionInfo.GetVersionInfo(fileName);
        //    }
        //    catch
        //    {
        //        return "";
        //    }

        //    // Get the file description
        //    name = myFileVersionInfo.FileDescription;
        //    //if (name == "")
        //    //    name = GetTitle(handle);

        //    if (String.IsNullOrEmpty(name))
        //    {
        //        return "";
        //    }

        //    return name;
        //}
        #endregion

        public bool isMatch(string filter)
        {
            if (
                appName.IndexOf(filter.ToLower()) >= 0 ||
                process.ProcessName.IndexOf(filter.ToLower()) >= 0 ||
                process.MainWindowTitle.ToLower().IndexOf(filter.ToLower()) >= 0
            ) {
                return true;
            }

            return false;
        }

        public void focus()
        {
            SetForegroundWindow(process.MainWindowHandle);
        }

        static public bool isValid(Process proc)
        {
            return (proc.MainWindowTitle.Length != 0);
        }
    }
}
