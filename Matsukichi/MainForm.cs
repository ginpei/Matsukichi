using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;  // Process
using System.Runtime.InteropServices;  // DllImport

namespace Matsukichi
{
    public partial class MainForm : Form
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private List<AppInfo> appListCache = new List<AppInfo>();
        private List<AppInfo> filteredAppList = new List<AppInfo>();

        public MainForm()
        {
            InitializeComponent();
        }

        private void registerHotkeys()
        {
            GlobalHotkeyManager.RegisterHotKey(Keys.Space, KeyModifiers.Control);
            GlobalHotkeyManager.HotKeyPressed += new EventHandler<HotKeyEventArgs>(GlobalHotkeyManager_HotKeyPressed);
            CheckForIllegalCrossThreadCalls = false;  // FIXME
        }

        private void show()
        {
            Visible = true;
            SetForegroundWindow(Handle);
        }

        private void hide()
        {
            Visible = false;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            registerHotkeys();
            updateAppList();
        }

        private void uiFilterText_TextChanged(object sender, EventArgs e)
        {
            filterAppList(uiFilterText.Text);

            if (uiCommandList.Items.Count > 0)
            {
                uiCommandList.SelectedIndex = 0;
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                openApp();
                uiFilterText.Text = "";
                hide();
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                hide();
                e.SuppressKeyPress = true;
            }
        }

        public void GlobalHotkeyManager_HotKeyPressed(object sender, HotKeyEventArgs e)
        {
            if (e.Modifiers == KeyModifiers.Control && e.Key == Keys.Space)
            {
                show();
            }
        }
    }

    partial class MainForm
    {
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
            uiCommandList.Items.Clear();
            filteredAppList.Clear();

            foreach (AppInfo app in appListCache)
            {
                if (app.isMatch(filter))
                {
                    filteredAppList.Add(app);
                    uiCommandList.Items.Add(app.screenName);
                }
            }
        }

        private void openApp()
        {
            if (uiCommandList.SelectedIndex >= 0)
            {
                filteredAppList[uiCommandList.SelectedIndex].focus();
            }
        }
    }
}
