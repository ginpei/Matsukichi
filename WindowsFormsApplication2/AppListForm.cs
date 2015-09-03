using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;  // DllImport
using System.Management;  // ManagementObjectSearcher
using System.Windows.Automation;  // AutomationElement 

namespace AppList
{
    using ConsoleHotKey;
    public partial class AppListForm : Form
    {
        private List<AppInfo> appListCache = new List<AppInfo>();
        private List<AppInfo> selectedAppList = new List<AppInfo>();
        //private GlobalKeyboardListener listener = new GlobalKeyboardListener();

        public AppListForm()
        {
            InitializeComponent();
        }

        private void updateAppList()
        {
            appListCache.Clear();

            foreach (Process p in Process.GetProcesses())
            {
                if (p.MainWindowTitle.Length != 0)
                {
                    AppInfo app = new AppInfo(p);
                    if (app.screenName.Length > 0)
                    {
                        appListCache.Add(app);
                    }
                }
            }

            filterAppList("");
        }

        private void filterAppList(string filter)
        {
            appList.Items.Clear();
            selectedAppList.Clear();

            foreach (AppInfo app in appListCache) {
                if (app.isMatch(filter))
                {
                    selectedAppList.Add(app);
                    appList.Items.Add(app.screenName);
                }
            }
        }

        private delegate void ObjectDelegate(object obj);

        private void AppListForm_Load(object sender, EventArgs e)
        {
            Visible = false;
            updateAppList();

            HotKeyManager.RegisterHotKey(Keys.Space, KeyModifiers.Control);
            HotKeyManager.HotKeyPressed += new EventHandler<HotKeyEventArgs>(HotKeyManager_HotKeyPressed);
            CheckForIllegalCrossThreadCalls = false;  // FIXME
        }

        public void HotKeyManager_HotKeyPressed(object sender, HotKeyEventArgs e)
        {
            if (e.Modifiers == KeyModifiers.Control && e.Key == Keys.Space)
            {
                // FIXME: couldn't get focus
                Visible = true;
                Focus();
                filterText.Focus();
            }
        }

        private void AppListForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            updateAppList();
            filterText.Clear();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            filterAppList(filterText.Text);

            if (appList.Items.Count > 0)
            {
                appList.SelectedIndex = 0;
            }
        }

        private void filterText_KeyDown(object sender, KeyEventArgs e)
        {
            //Debug.WriteLine(e.KeyCode.ToString());

            switch (e.KeyCode.ToString())
            {
                case "Up":
                    if (appList.SelectedIndex > 0)
                    {
                        appList.SelectedIndex--;
                    }
                    break;

                case "Down":
                    if (appList.SelectedIndex+1 < appList.Items.Count)
                    {
                        appList.SelectedIndex++;
                    }
                    break;

                case "Escape":
                    if (String.IsNullOrEmpty(filterText.Text))
                    {
                        Visible = false;
                    }
                    else
                    {
                        filterText.Clear();
                    }
                    e.SuppressKeyPress = true;
                    break;

                case "Return":
                    if (appList.SelectedIndex >= 0)
                    {
                        selectedAppList[appList.SelectedIndex].focus();
                    }

                    filterText.Clear();
                    appList.SelectedIndex = -1;

                    e.SuppressKeyPress = true;
                    break;
            }
        }
    }

    public partial class AppInfo
    {
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
            // TODO: search apps which throws an error on SetFocus
            AutomationElement element = AutomationElement.FromHandle(process.MainWindowHandle);
            if (element != null)
            {
                element.SetFocus();
            }
        }
    }

    ///**
    // * @see http://blogs.msdn.com/b/toub/archive/2006/05/03/589423.aspx
    // */
    //public class GlobalKeyboardListener
    //{
    //    private const int WH_KEYBOARD_LL = 13;
    //    private const int WM_KEYDOWN = 0x0100;
    //    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    //    private static IntPtr _hookID;

    //    public GlobalKeyboardListener()
    //    {
    //        _hookID = SetHook(HookCallback);
    //    }

    //    ~GlobalKeyboardListener()
    //    {
    //        UnhookWindowsHookEx(_hookID);
    //    }

    //    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    //    {
    //        using (Process curProcess = Process.GetCurrentProcess())
    //        using (ProcessModule curModule = curProcess.MainModule)
    //        {
    //            return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
    //                GetModuleHandle(curModule.ModuleName), 0);
    //        }
    //    }

    //    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    //    {
    //        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
    //        {
    //            int vkCode = Marshal.ReadInt32(lParam);
    //            Debug.WriteLine((Keys)vkCode);
    //        }
    //        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    //    }

    //    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    //    private static extern IntPtr SetWindowsHookEx(int idHook,
    //        LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    //    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    //    [return: MarshalAs(UnmanagedType.Bool)]
    //    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    //    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    //    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
    //        IntPtr wParam, IntPtr lParam);

    //    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    //    private static extern IntPtr GetModuleHandle(string lpModuleName);
    //}

    ///**
    // * @see http://www.pinvoke.net/default.aspx/user32/RegisterHotKey.html
    // */
    //public class WindowsShell
    //{
    //    #region fields
    //    public static int MOD_ALT = 0x1;
    //    public static int MOD_CONTROL = 0x2;
    //    public static int MOD_SHIFT = 0x4;
    //    public static int MOD_WIN = 0x8;
    //    public static int WM_HOTKEY = 0x312;
    //    #endregion

    //    [DllImport("user32.dll")]
    //    private static extern bool RegisterHotKey(IntPtr hWnd, IntPtr id, IntPtr fsModifiers, int vlc);

    //    [DllImport("user32.dll")]
    //    private static extern bool UnregisterHotKey(IntPtr hWnd, IntPtr id);

    //    private static IntPtr keyId;
    //    public static void RegisterHotKey(Form f, Keys key)
    //    {
    //        int modifiers = 0;

    //        if ((key & Keys.Alt) == Keys.Alt)
    //            modifiers = modifiers | WindowsShell.MOD_ALT;

    //        if ((key & Keys.Control) == Keys.Control)
    //            modifiers = modifiers | WindowsShell.MOD_CONTROL;

    //        if ((key & Keys.Shift) == Keys.Shift)
    //            modifiers = modifiers | WindowsShell.MOD_SHIFT;

    //        Keys k = key & ~Keys.Control & ~Keys.Shift & ~Keys.Alt;
    //        keyId = (IntPtr)f.GetHashCode(); // this should be a key unique ID, modify this if you want more than one hotkey
    //        RegisterHotKey((IntPtr)f.Handle, keyId, (IntPtr)modifiers, (int)k);
    //    }

    //    private delegate void Func();

    //    public static void UnregisterHotKey(Form f)
    //    {
    //        try
    //        {
    //            UnregisterHotKey(f.Handle, keyId); // modify this if you want more than one hotkey
    //        }
    //        catch (Exception ex)
    //        {
    //            MessageBox.Show(ex.ToString());
    //        }
    //    }
    //}

    //public partial class Form1 : Form, IDisposable
    //{
    //    protected override void OnLoad(EventArgs e)
    //    {
    //        base.OnLoad(e);
    //        Keys k = Keys.A | Keys.Control;
    //        WindowsShell.RegisterHotKey(this, k);
    //    }

    //    // CF Note: The WndProc is not present in the Compact Framework (as of vers. 3.5)! please derive from the MessageWindow class in order to handle WM_HOTKEY
    //    protected override void WndProc(ref Message m)
    //    {
    //        base.WndProc(ref m);

    //        if (m.Msg == WindowsShell.WM_HOTKEY)
    //            this.Visible = !this.Visible;
    //    }

    //    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    //    {
    //        WindowsShell.UnregisterHotKey(this);
    //    }
    //}
}

// http://stackoverflow.com/questions/3654787/global-hotkey-in-console-application
namespace ConsoleHotKey
{
    using System.Threading;

    public static class HotKeyManager
    {
        public static event EventHandler<HotKeyEventArgs> HotKeyPressed;

        public static int RegisterHotKey(Keys key, KeyModifiers modifiers)
        {
            _windowReadyEvent.WaitOne();
            int id = System.Threading.Interlocked.Increment(ref _id);
            _wnd.Invoke(new RegisterHotKeyDelegate(RegisterHotKeyInternal), _hwnd, id, (uint)modifiers, (uint)key);
            return id;
        }

        public static void UnregisterHotKey(int id)
        {
            _wnd.Invoke(new UnRegisterHotKeyDelegate(UnRegisterHotKeyInternal), _hwnd, id);
        }

        delegate void RegisterHotKeyDelegate(IntPtr hwnd, int id, uint modifiers, uint key);
        delegate void UnRegisterHotKeyDelegate(IntPtr hwnd, int id);

        private static void RegisterHotKeyInternal(IntPtr hwnd, int id, uint modifiers, uint key)
        {
            RegisterHotKey(hwnd, id, modifiers, key);
        }

        private static void UnRegisterHotKeyInternal(IntPtr hwnd, int id)
        {
            UnregisterHotKey(_hwnd, id);
        }

        private static void OnHotKeyPressed(HotKeyEventArgs e)
        {
            if (HotKeyManager.HotKeyPressed != null)
            {
                HotKeyManager.HotKeyPressed(null, e);
            }
        }

        private static volatile MessageWindow _wnd;
        private static volatile IntPtr _hwnd;
        private static ManualResetEvent _windowReadyEvent = new ManualResetEvent(false);
        static HotKeyManager()
        {
            Thread messageLoop = new Thread(delegate()
            {
                Application.Run(new MessageWindow());
            });
            messageLoop.Name = "MessageLoopThread";
            messageLoop.IsBackground = true;
            messageLoop.Start();
        }

        private class MessageWindow : Form
        {
            public MessageWindow()
            {
                _wnd = this;
                _hwnd = this.Handle;
                _windowReadyEvent.Set();
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_HOTKEY)
                {
                    HotKeyEventArgs e = new HotKeyEventArgs(m.LParam);
                    HotKeyManager.OnHotKeyPressed(e);
                }

                base.WndProc(ref m);
            }

            protected override void SetVisibleCore(bool value)
            {
                // Ensure the window never becomes visible
                base.SetVisibleCore(false);
            }

            private const int WM_HOTKEY = 0x312;
        }

        [DllImport("user32", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private static int _id = 0;
    }


    public class HotKeyEventArgs : EventArgs
    {
        public readonly Keys Key;
        public readonly KeyModifiers Modifiers;

        public HotKeyEventArgs(Keys key, KeyModifiers modifiers)
        {
            this.Key = key;
            this.Modifiers = modifiers;
        }

        public HotKeyEventArgs(IntPtr hotKeyParam)
        {
            uint param = (uint)hotKeyParam.ToInt64();
            Key = (Keys)((param & 0xffff0000) >> 16);
            Modifiers = (KeyModifiers)(param & 0x0000ffff);
        }
    }

    [Flags]
    public enum KeyModifiers
    {
        Alt = 1,
        Control = 2,
        Shift = 4,
        Windows = 8,
        NoRepeat = 0x4000
    }
}
