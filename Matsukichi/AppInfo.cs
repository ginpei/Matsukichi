using System;
using System.Linq;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;

namespace Matsukichi
{
    public class CommandItem
    {
        public string Path;
        public string ScreenName;

        protected void SetByPath(string path)
        {
            Path = path;
            ScreenName = GetAppName(Path);
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

        internal bool IsMatch(string loweredText)
        {
            if (string.IsNullOrEmpty(ScreenName))
            {
                return false;
            }

            if (Path.ToLower().IndexOf(loweredText) >= 0)
            {
                return true;
            }

            if (ScreenName.ToLower().IndexOf(loweredText) >= 0)
            {
                return true;
            }

            return false;
        }

        virtual public void Run()
        {
            throw new NotImplementedException();
        }
    }

    public partial class CommandList : System.Collections.Generic.List<CommandItem>
    {
        virtual public CommandList Filter(string loweredText)
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

            string path = GetProcPath(proc);
            SetByPath(path);
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

        override public void Run()
        {
            SetForegroundWindow(Process.MainWindowHandle);
        }
    }

    class RunningAppList : CommandList
    {
        public void Update(Process[] processes)
        {
            Clear();

            foreach (Process proc in processes)
            {
                CommandItem app = CreateItem(proc);
                if (app != null)
                {
                    Add(app);
                }
            }
        }

        private static CommandItem CreateItem(Process proc)
        {
            CommandItem app = null;

            if (proc.MainWindowTitle.Length > 1)
            {
                app = new RunningAppItem(proc);
            }

            return app;
        }
    }

    class AppLinkItem : CommandItem
    {
        public AppLinkItem(string path) : base()
        {
            SetByPath(path);
        }

        override public void Run()
        {
            Process proc = new Process();
            proc.StartInfo.FileName = Path;
            proc.Start();
        }
    }

    class StartMenuAppList : CommandList
    {
        public void Update()
        {
            Clear();

            AddAppsFromShortcuts(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu));
            AddAppsFromShortcuts(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu));
        }

        private void AddAppsFromShortcuts(string dir)
        {
            string[] paths = Directory.GetFiles(dir + "\\Programs", "*.lnk", SearchOption.AllDirectories);

            // add apps to the list if a link heads to an exe file
            foreach (string path in paths)
            {
                if (path.ToLower().Contains("uninstall"))
                {
                    continue;
                }

                AppLinkItem app = CreateItem(path);
                if (app != null)
                {
                    Add(app);

                }
            }
        }

        private static AppLinkItem CreateItem(string path)
        {
            AppLinkItem app = null;

            if (path.Length > 1)
            {
                app = new AppLinkItem(path);
            }

            return app;
        }
    }
}
