namespace AppList
{
    partial class AppListForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.appList = new System.Windows.Forms.ListBox();
            this.updateButton = new System.Windows.Forms.Button();
            this.filterText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // appList
            // 
            this.appList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.appList.FormattingEnabled = true;
            this.appList.ItemHeight = 18;
            this.appList.Location = new System.Drawing.Point(12, 48);
            this.appList.Name = "appList";
            this.appList.Size = new System.Drawing.Size(877, 364);
            this.appList.TabIndex = 0;
            // 
            // updateButton
            // 
            this.updateButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.updateButton.Location = new System.Drawing.Point(12, 427);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(877, 75);
            this.updateButton.TabIndex = 1;
            this.updateButton.Text = "Update";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // filterText
            // 
            this.filterText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filterText.Location = new System.Drawing.Point(76, 12);
            this.filterText.Name = "filterText";
            this.filterText.Size = new System.Drawing.Size(813, 25);
            this.filterText.TabIndex = 0;
            this.filterText.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.filterText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.filterText_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 18);
            this.label1.TabIndex = 2;
            this.label1.Text = "Search";
            // 
            // AppListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(901, 514);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.filterText);
            this.Controls.Add(this.appList);
            this.Controls.Add(this.updateButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AppListForm";
            this.ShowInTaskbar = false;
            this.Text = "Running Apps";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AppListForm_FormClosing);
            this.Load += new System.EventHandler(this.AppListForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox appList;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.TextBox filterText;
        private System.Windows.Forms.Label label1;
    }
}

