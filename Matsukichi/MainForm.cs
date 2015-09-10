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

namespace Matsukichi
{
    public partial class MainForm : Form
    {
        private List<AppInfo> appListCache = new List<AppInfo>();
        private List<AppInfo> filteredAppList = new List<AppInfo>();

        public MainForm()
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

        private void MainForm_Load(object sender, EventArgs e)
        {
            updateAppList();
        }
    }
}
