using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

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

        private void UpdateStartMenuAppList()
        {
            StartMenuAppList.Update();
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
                loopCount = AddNecessaryCommand(loopCount, loweredText, RunningAppList);
                loopCount = AddNecessaryCommand(loopCount, loweredText, StartMenuAppList);

                if (FilteredCommandList.Count < 1)
                {
                    ClearIcon();
                }
            }
            else
            {
                ClearIcon();
            }

            ResetCommandSelection();
        }

        private int AddNecessaryCommand(int loopCount, string loweredText, CommandList list)
        {
            foreach (CommandItem app in list.Filter(loweredText))
            {
                if (loopCount++ >= MAX_SUGGESTION)
                {
                    break;
                }
                FilteredCommandList.Add(app);
            }

            return loopCount;
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
            CommandItem command = GetSelectedCommand();
            if (command != null)
            {
                try
                {
                    command.Run();
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    string title = "Matsukichi (´・ω・`)";
                    string message = string.Format("Failed to start app: {0}", command.ScreenName);
                    MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            HideMainWindow();
        }

        private void ClearIcon()
        {
            uiIconPlace.Image = null;
        }
    }
}
