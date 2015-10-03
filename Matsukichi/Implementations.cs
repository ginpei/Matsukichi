using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;  // Process
using System.Runtime.InteropServices;  // DllImport
using System.Drawing;
using System.IO;
using Matsukichi.GlobalHotkey;

namespace Matsukichi
{
    public partial class MainForm
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private void UpdateRunningAppList()
        {
            RunningAppList.Update(Process.GetProcesses());
            FilterAvailableCommandList();
        }

        private void FilterAvailableCommandList(string text=null)
        {
            FilteredCommandList.Clear();

            if (text == null)
            {
                text = uiFilterText.Text;
            }

            if (text.Length > 0)
            {

                int loopCount = 0;
                string loweredText = text.ToLower();
                foreach (CommandItem app in RunningAppList.Filter(loweredText))
                {
                    FilteredCommandList.Add(app);
                    if (++loopCount >= MAX_SUGGESTION)
                    {
                        break;
                    }
                }
            }
            else
            {
                uiIconPlace.Image = null;
            }

            ResetCommandSelection();
        }

        private void ShowCommandIcon()
        {
            CommandItem command = GetSelectedCommand();
            Icon icon = Icon.ExtractAssociatedIcon(command.Path);
            Bitmap bitmap = Bitmap.FromHicon(icon.Handle);

            uiIconPlace.Image = bitmap;
        }

        private CommandItem GetSelectedCommand()
        {
            CommandItem command = null;
            int index = uiCommandList.SelectedIndex;

            if (index >= 0)
            {
                command = FilteredCommandList[index];

            }

            return command;
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

        private void RunSelectedCommand()
        {
            RunningAppItem command = (RunningAppItem)GetSelectedCommand();

            if (command != null)
            {
                command.Run();
            }

            HideMainWindow();
        }

        //private void registerHotkeys()
        //{
        //    GlobalHotkeyManager.RegisterHotKey(Keys.Space, KeyModifiers.Control);
        //    GlobalHotkeyManager.HotKeyPressed += new EventHandler<HotKeyEventArgs>(GlobalHotkeyManager_HotKeyPressed);
        //    CheckForIllegalCrossThreadCalls = false;  // FIXME
        //}

        //private void Show()
        //{
        //    Visible = true;
        //    SetForegroundWindow(Handle);
        //    uiIconPlace.Image = null;
        //    uiCommandList.Items.Clear();
        //    UpdateAppList();
        //}

        //private void Hide()
        //{
        //    Visible = false;
        //}

        //private void UpdateAppList()
        //{
        //    uiCommandList.Items.Add("(updating...)");

        //    appListCache.Clear();

        //    UpdateCurrentAppList();

        //    UpdateInstalledAppList();

        //    FilterAppList(uiFilterText.Text);
        //}

        //private void UpdateCurrentAppList()
        //{
        //    foreach (Process p in Process.GetProcesses())
        //    {
        //        if (AppInfo.IsValid(p))
        //        {
        //            AppInfo app = new AppInfo(p);
        //            if (!String.IsNullOrEmpty(app.screenName))
        //            {
        //                appListCache.Add(app);
        //            }
        //        }
        //    }
        //}

        //private void UpdateInstalledAppList()
        //{
        //    // get all paths of link
        //    AddAppsFromShortcuts(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu));
        //    AddAppsFromShortcuts(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu));
        //}

        //private void AddAppsFromShortcuts(string dir)
        //{
        //    string[] lnkPaths = Directory.GetFiles(dir + "\\Programs", "*.lnk", SearchOption.AllDirectories);

        //    // add apps to the list if a link heads to an exe file
        //    foreach (string linkPath in lnkPaths)
        //    {
        //        AppInfo app = new AppInfo(linkPath);
        //        appListCache.Add(app);
        //    }
        //}

        //private void FilterAppList(string filter)
        //{
        //    uiCommandList.Items.Clear();
        //    filteredAppList.Clear();

        //    foreach (AppInfo app in appListCache)
        //    {
        //        if (app.IsMatch(filter))
        //        {
        //            filteredAppList.Add(app);
        //            uiCommandList.Items.Add(app.screenName);
        //        }
        //    }

        //    if (uiCommandList.Items.Count > 0)
        //    {
        //        uiCommandList.SelectedIndex = 0;
        //    }
        //}
    }
}
