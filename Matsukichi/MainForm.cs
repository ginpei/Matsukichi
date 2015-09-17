using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;  // Process
using System.Runtime.InteropServices;  // DllImport
using System.Drawing;
using System.IO;

namespace Matsukichi
{
    using GlobalHotkey;

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

        private void Show()
        {
            Visible = true;
            SetForegroundWindow(Handle);
            uiIconPlace.Image = null;
            uiCommandList.Items.Clear();
            UpdateAppList();
        }

        private void Hide()
        {
            Visible = false;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            registerHotkeys();
            UpdateAppList();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OpenApp();
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
                    Hide();
                }
                e.SuppressKeyPress = true;
            }
        }

        private void uiCommandList_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppInfo info = filteredAppList[uiCommandList.SelectedIndex];
            Icon icon = Icon.ExtractAssociatedIcon(info.path);
            Bitmap bitmap = Bitmap.FromHicon(icon.Handle);
            uiIconPlace.Image = bitmap;
        }

        private void uiFilterText_TextChanged(object sender, EventArgs e)
        {
            FilterAppList(uiFilterText.Text);

            if (uiCommandList.Items.Count > 0)
            {
                uiCommandList.SelectedIndex = 0;
            }
        }

        private void uiFilterText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || (e.Control && e.KeyCode == Keys.K))
            {
                SelectPrevCommand();
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Down || (e.Control && e.KeyCode == Keys.J))
            {
                SelectNextCommand();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.M)
            {
                OpenApp();
                e.SuppressKeyPress = true;
            }
        }

        public void GlobalHotkeyManager_HotKeyPressed(object sender, HotKeyEventArgs e)
        {
            if (e.Modifiers == KeyModifiers.Control && e.Key == Keys.Space)
            {
                Show();
            }
        }

        private void UpdateAppList()
        {
            uiCommandList.Items.Add("(updating...)");

            appListCache.Clear();

            UpdateCurrentAppList();

            UpdateInstalledAppList();

            FilterAppList(uiFilterText.Text);
        }

        private void UpdateCurrentAppList()
        {
            foreach (Process p in Process.GetProcesses())
            {
                if (AppInfo.IsValid(p))
                {
                    AppInfo app = new AppInfo(p);
                    if (!String.IsNullOrEmpty(app.screenName))
                    {
                        appListCache.Add(app);
                    }
                }
            }
        }

        private void UpdateInstalledAppList()
        {
            // get all paths of link
            AddAppsFromShortcuts(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu));
            AddAppsFromShortcuts(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu));
        }

        private void AddAppsFromShortcuts(string dir)
        {
            string[] lnkPaths = Directory.GetFiles(dir + "\\Programs", "*.lnk", SearchOption.AllDirectories);

            // add apps to the list if a link heads to an exe file
            foreach (string linkPath in lnkPaths)
            {
                AppInfo app = new AppInfo(linkPath);
                appListCache.Add(app);
            }
        }

        private void FilterAppList(string filter)
        {
            uiCommandList.Items.Clear();
            filteredAppList.Clear();

            foreach (AppInfo app in appListCache)
            {
                if (app.IsMatch(filter))
                {
                    filteredAppList.Add(app);
                    uiCommandList.Items.Add(app.screenName);
                }
            }

            if (uiCommandList.Items.Count > 0)
            {
                uiCommandList.SelectedIndex = 0;
            }
        }

        private void SelectPrevCommand()
        {
            if (uiCommandList.SelectedIndex > 0)
            {
                uiCommandList.SelectedIndex--;
            }
        }

        private void SelectNextCommand()
        {
            if (uiCommandList.SelectedIndex < uiCommandList.Items.Count - 1)
            {
                uiCommandList.SelectedIndex++;
            }
        }

        private void OpenApp()
        {
            if (uiCommandList.SelectedIndex >= 0)
            {
                filteredAppList[uiCommandList.SelectedIndex].Focus();
            }
            uiFilterText.Text = "";
            Hide();
        }
    }
}
