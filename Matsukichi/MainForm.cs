using System;
using System.Windows.Forms;
using Matsukichi.GlobalHotkey;

namespace Matsukichi
{
    public partial class MainForm : Form
    {
        public const int MAX_SUGGESTION = 5;

        private FilteredCommandList FilteredCommandList = new FilteredCommandList();
        RunningAppList RunningAppList = new RunningAppList();
        StartMenuAppList StartMenuAppList = new StartMenuAppList();

        private void Initialize()
        {
            RegisterGlobalHotKey();
            FilteredCommandList.SetUIItems(uiCommandList.Items);
            UpdateStartMenuAppList();
            ShowMainWindow();
        }

        private void RegisterGlobalHotKey()
        {
            GlobalHotkeyManager.RegisterHotKey(Keys.Space, KeyModifiers.Control);
            GlobalHotkeyManager.HotKeyPressed += new EventHandler<HotKeyEventArgs>(GlobalHotkeyManager_HotKeyPressed);
            CheckForIllegalCrossThreadCalls = false;  // FIXME
        }

        private void ShowMainWindow()
        {
            uiIconPlace.Image = null;
            uiCommandList.Items.Clear();

            Visible = true;
            SetForegroundWindow(Handle);

            UpdateRunningAppList();
        }

        private void HideMainWindow()
        {
            Visible = false;
        }

        private void ResetCommandSelection()
        {
            if (uiCommandList.Items.Count > 0)
            {
                uiCommandList.SelectedIndex = 0;
            }
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Initialize();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                RunSelectedCommand();
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
                    HideMainWindow();
                }
                e.SuppressKeyPress = true;
            }
        }

        private void uiCommandList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowCommandIcon();
        }

        private void uiFilterText_TextChanged(object sender, EventArgs e)
        {
            FilterAvailableCommandList();
        }

        private void uiFilterText_KeyDown(object sender, KeyEventArgs e)
        {
            // Up
            // C-K
            if (e.KeyCode == Keys.Up || (e.Control && e.KeyCode == Keys.K))
            {
                SelectPrevCommand();
                e.SuppressKeyPress = true;
            }
            // Down
            // C-J
            else if (e.KeyCode == Keys.Down || (e.Control && e.KeyCode == Keys.J))
            {
                SelectNextCommand();
                e.SuppressKeyPress = true;
            }
            // C-M
            // (Enter is specified at MainForm_KeyDown())
            else if (e.Control && e.KeyCode == Keys.M)
            {
                RunSelectedCommand();
                e.SuppressKeyPress = true;
            }
        }

        public void GlobalHotkeyManager_HotKeyPressed(object sender, HotKeyEventArgs e)
        {
            // C-Space
            if (e.Modifiers == KeyModifiers.Control && e.Key == Keys.Space)
            {
                ShowMainWindow();
            }
        }
    }
}
