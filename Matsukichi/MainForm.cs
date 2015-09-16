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

        private void show()
        {
            Visible = true;
            SetForegroundWindow(Handle);
            uiIconPlace.Image = null;
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

        private void uiCommandList_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppInfo info = filteredAppList[uiCommandList.SelectedIndex];
            Icon icon = Icon.ExtractAssociatedIcon(info.path);
            Bitmap bitmap = Bitmap.FromHicon(icon.Handle);
            uiIconPlace.Image = bitmap;
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
            uiCommandList.Items.Add("(updating...)");

            appListCache.Clear();

            updateCurrentAppList();

            updateInstalledAppList();

            filterAppList(uiFilterText.Text);
        }

        private void updateCurrentAppList()
        {
            foreach (Process p in Process.GetProcesses())
            {
                if (AppInfo.isValid(p))
                {
                    AppInfo app = new AppInfo(p);
                    if (!String.IsNullOrEmpty(app.screenName))
                    {
                        appListCache.Add(app);
                    }
                }
            }
        }

        private void updateInstalledAppList()
        {
            // get all paths of link
            //string startMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
            string startMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
            string[] lnkPaths = Directory.GetFiles(startMenuPath+"\\Programs", "*.lnk", SearchOption.AllDirectories);

            // add apps to the list if a link heads to an exe file
            foreach (string linkPath in lnkPaths)
            {
                Debug.WriteLine(linkPath);

                //string exePath = "getPathFromTheLink";  // FIXME
                //bool thePathHeadsToExe = false;  // FIXME
                //if (thePathHeadsToExe)
                //{
                //    AppInfo app = new AppInfo(exePath);
                //    if (!String.IsNullOrEmpty(app.screenName))
                //    {
                //        appListCache.Add(app);
                //    }
                //}
            }
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

            if (uiCommandList.Items.Count > 0)
            {
                uiCommandList.SelectedIndex = 0;
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
