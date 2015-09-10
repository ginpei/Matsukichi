using System;
using System.Collections.Generic;
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
            uiCommandList.Items.Clear();
            updateAppList();
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

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                openApp();
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                if (uiFilterText.Text.Length > 0)
                {
                    uiFilterText.Text = "";
                    uiFilterText.Focus();
                }
                else
                {
                    hide();
                }
                e.SuppressKeyPress = true;
            }
        }

        private void uiFilterText_TextChanged(object sender, EventArgs e)
        {
            filterAppList(uiFilterText.Text);

            if (uiCommandList.Items.Count > 0)
            {
                uiCommandList.SelectedIndex = 0;
            }
        }

        private void uiFilterText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || (e.Control && e.KeyCode == Keys.K))
            {
                selectPrevCommand();
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Down || (e.Control && e.KeyCode == Keys.J))
            {
                selectNextCommand();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.M)
            {
                openApp();
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

        private void selectPrevCommand()
        {
            if (uiCommandList.SelectedIndex > 0)
            {
                uiCommandList.SelectedIndex--;
            }
        }

        private void selectNextCommand()
        {
            if (uiCommandList.SelectedIndex < uiCommandList.Items.Count - 1)
            {
                uiCommandList.SelectedIndex++;
            }
        }

        private void openApp()
        {
            if (uiCommandList.SelectedIndex >= 0)
            {
                filteredAppList[uiCommandList.SelectedIndex].focus();
            }
            uiFilterText.Text = "";
            hide();
        }
    }
}
