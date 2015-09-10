﻿namespace Matsukichi
{
    partial class MainForm
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
            this.uiFilterText = new System.Windows.Forms.TextBox();
            this.uiCommandList = new System.Windows.Forms.ListBox();
            this.uiIconPlace = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.uiIconPlace)).BeginInit();
            this.SuspendLayout();
            // 
            // uiFilterText
            // 
            this.uiFilterText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uiFilterText.Font = new System.Drawing.Font("MS UI Gothic", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.uiFilterText.Location = new System.Drawing.Point(68, 12);
            this.uiFilterText.Name = "uiFilterText";
            this.uiFilterText.Size = new System.Drawing.Size(673, 51);
            this.uiFilterText.TabIndex = 0;
            this.uiFilterText.TextChanged += new System.EventHandler(this.uiFilterText_TextChanged);
            this.uiFilterText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.uiFilterText_KeyDown);
            // 
            // uiCommandList
            // 
            this.uiCommandList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uiCommandList.FormattingEnabled = true;
            this.uiCommandList.ItemHeight = 18;
            this.uiCommandList.Location = new System.Drawing.Point(12, 69);
            this.uiCommandList.Name = "uiCommandList";
            this.uiCommandList.Size = new System.Drawing.Size(729, 94);
            this.uiCommandList.TabIndex = 1;
            this.uiCommandList.SelectedIndexChanged += new System.EventHandler(this.uiCommandList_SelectedIndexChanged);
            // 
            // uiIconPlace
            // 
            this.uiIconPlace.ImageLocation = "";
            this.uiIconPlace.Location = new System.Drawing.Point(12, 12);
            this.uiIconPlace.Name = "uiIconPlace";
            this.uiIconPlace.Size = new System.Drawing.Size(50, 50);
            this.uiIconPlace.TabIndex = 2;
            this.uiIconPlace.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(753, 175);
            this.Controls.Add(this.uiIconPlace);
            this.Controls.Add(this.uiFilterText);
            this.Controls.Add(this.uiCommandList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Matsukichi";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.uiIconPlace)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox uiFilterText;
        private System.Windows.Forms.ListBox uiCommandList;
        private System.Windows.Forms.PictureBox uiIconPlace;
    }
}

