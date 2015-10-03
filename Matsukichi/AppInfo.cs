using System;
using System.Linq;
using System.Diagnostics;  // Process
using System.Management;  // ManagementObjectSearcher (System.Management.dll)
using System.Runtime.InteropServices;  // DllImport
using System.Windows.Forms;


namespace Matsukichi
{
    public class CommandItem
    {
        public string Path;
        public string AppName;
        public string ScreenName;

        internal bool IsMatch(string loweredText)
        {
            if (string.IsNullOrEmpty(ScreenName))
            {
                return false;
            }

            if (Path.IndexOf(loweredText) < 1)
            {
                return false;
            }

            return true;
        }

        internal void Run()
        {
            throw new NotImplementedException();
        }

        //public Process process;
        //public string path;
        //public string appName;
        //public string screenName;

        //public AppInfo(Process proc)
        //{
        //    process = proc;

        //    path = GetProcPath(proc);
        //    screenName = GetAppName(path);
        //    if (!string.IsNullOrEmpty(screenName))
        //    {
        //        appName = screenName.ToLower();
        //    }
        //}

        //public AppInfo(string path)
        //{
        //    this.path = path;
        //    screenName = GetAppName(path);
        //    if (!string.IsNullOrEmpty(screenName))
        //    {
        //        appName = screenName.ToLower();
        //    }
        //}

        //private string GetProcPath(Process proc)
        //{
        //    // get process information using WMI (Windows Management Instrumentation)
        //    ManagementObjectSearcher searcher = new ManagementObjectSearcher(string.Format(
        //        "SELECT ProcessId, ExecutablePath " +
        //        "FROM Win32_Process " +
        //        "WHERE ProcessId LIKE '{0}'",
        //        proc.Id.ToString()
        //    ));
        //    ManagementObjectCollection searchResult = searcher.Get();
        //    ManagementObject procData = searchResult.Cast<ManagementObject>().FirstOrDefault();

        //    string path = (string)procData["ExecutablePath"];

        //    return path;
        //}

        //private string GetAppName(string path)
        //{
        //    string name = null;

        //    if (path.EndsWith(".lnk"))
        //    {
        //        name = System.IO.Path.GetFileName(path);
        //        name = name.Substring(0, name.Length - 4);
        //        return name;
        //    }

        //    try
        //    {
        //        // 'System.IO.FileNotFoundException' when path is "C:\\WINDOWS\\system32\\ApplicationFrameHost.exe"
        //        FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(path);
        //        name = myFileVersionInfo.FileDescription;

        //        return name;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        ///**
        // * @see http://stackoverflow.com/questions/22201752/how-to-get-active-window-app-name-as-shown-in-task-manager
        // */
        //#region not used
        ////private string getAppName(Process p)
        ////{
        ////    string name = "";
        ////    var processname = p.ProcessName;
        ////    string fileName = "";
        ////    int pid = p.Id;

        ////    if (String.IsNullOrEmpty(processname))
        ////    {
        ////        return "";
        ////    }

        ////    switch (processname)
        ////    {
        ////        case "explorer": //metro processes
        ////        case "WWAHost":
        ////            return "";
        ////        default:
        ////            break;
        ////    }
        ////    string wmiQuery = string.Format("SELECT ProcessId, ExecutablePath FROM Win32_Process WHERE ProcessId LIKE '{0}'", pid.ToString());
        ////    var pro = new ManagementObjectSearcher(wmiQuery).Get().Cast<ManagementObject>().FirstOrDefault();
        ////    fileName = (string)pro["ExecutablePath"];

        ////    if (String.IsNullOrEmpty(fileName))
        ////    {
        ////        return "";
        ////    }

        ////    FileVersionInfo myFileVersionInfo;
        ////    try
        ////    {
        ////        // Get the file version
        ////        myFileVersionInfo = FileVersionInfo.GetVersionInfo(fileName);
        ////    }
        ////    catch
        ////    {
        ////        return "";
        ////    }

        ////    // Get the file description
        ////    name = myFileVersionInfo.FileDescription;
        ////    //if (name == "")
        ////    //    name = GetTitle(handle);

        ////    if (String.IsNullOrEmpty(name))
        ////    {
        ////        return "";
        ////    }

        ////    return name;
        ////}
        //#endregion

        //public bool IsMatch(string filter)
        //{
        //    if (
        //        appName.IndexOf(filter.ToLower()) >= 0 ||
        //        (process != null &&
        //            (process.ProcessName.IndexOf(filter.ToLower()) >= 0 ||
        //            process.MainWindowTitle.ToLower().IndexOf(filter.ToLower()) >= 0)
        //        )
        //    ) {
        //        return true;
        //    }

        //    return false;
        //}

        //public void Focus()
        //{
        //    if (process == null)
        //    {
        //        Process proc = new Process();
        //        proc.StartInfo.FileName = path;
        //        proc.Start();
        //    }
        //    else
        //    {
        //        SetForegroundWindow(process.MainWindowHandle);
        //    }
        //}

        //static public bool IsValid(Process proc)
        //{
        //    return (proc.MainWindowTitle.Length != 0);
        //}
    }

    public partial class CommandList : System.Collections.Generic.List<CommandItem>
    {
    }

    internal class FilteredCommandList : CommandList
    {
        ListBox.ObjectCollection Items;

        public ListBox.ObjectCollection SetUIItems(ListBox.ObjectCollection items)
        {
            return Items = items;
        }

        public new void Clear()
        {
            Items.Clear();
            base.Clear();
        }

        public new void Add(CommandItem item)
        {
            Items.Add(item.ScreenName);
            base.Add(item);
        }
    }

    class RunningAppItem : CommandItem
    {
        public Process Process;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public RunningAppItem(Process proc) : base()
        {
            Process = proc;

            Path = GetProcPath(proc);
            ScreenName = GetAppName(Path);
            if (!string.IsNullOrEmpty(ScreenName))
            {
                AppName = ScreenName.ToLower();
            }
        }

        private string GetProcPath(Process proc)
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

        private string GetAppName(string path)
        {
            string name = null;

            if (path.EndsWith(".lnk"))
            {
                name = System.IO.Path.GetFileName(path);
                name = name.Substring(0, name.Length - 4);
                return name;
            }

            try
            {
                // 'System.IO.FileNotFoundException' when path is "C:\\WINDOWS\\system32\\ApplicationFrameHost.exe"
                FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(path);
                name = myFileVersionInfo.FileDescription;

                return name;
            }
            catch
            {
                return null;
            }
        }

        public new void Run()
        {
            //if (Process == null)
            //{
            //    Process proc = new Process();
            //    proc.StartInfo.FileName = Path;
            //    proc.Start();
            //}
            //else
            //{
            //}
            SetForegroundWindow(Process.MainWindowHandle);
        }
    }

    class RunningAppList : CommandList
    {
        public void Update(Process[] processes)
        {
            foreach (Process proc in processes)
            {
                CommandItem app = createItem(proc);
                if (app != null)
                {
                    Add(app);
                }
            }
        }

        private static CommandItem createItem(Process proc)
        {
            CommandItem app = null;

            if (proc.MainWindowTitle.Length > 1)
            {
                app = new RunningAppItem(proc);
            }

            return app;
        }

        public CommandList Filter(string loweredText)
        {
            CommandList list = new CommandList();

            foreach (CommandItem app in this)
            {
                if (app.IsMatch(loweredText))
                {
                    list.Add(app);
                }
            }

            return list;
        }
    }
}
