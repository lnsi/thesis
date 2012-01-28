namespace VncSharpExampleCS
{
    partial class form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendKeysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cTRLALTDELToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aLTF4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cTRLESCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cTRLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aLTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clippedViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scaledViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.fullScreenRefreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewOnlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rd = new VncSharp.RemoteDesktop();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(724, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectToolStripMenuItem,
            this.disconnectToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.connectToolStripMenuItem.Text = "&Connect...";
            this.connectToolStripMenuItem.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // disconnectToolStripMenuItem
            // 
            this.disconnectToolStripMenuItem.Enabled = false;
            this.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            this.disconnectToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.disconnectToolStripMenuItem.Text = "&Disconnect";
            this.disconnectToolStripMenuItem.Click += new System.EventHandler(this.disconnectToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sendKeysToolStripMenuItem,
            this.pasteToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // sendKeysToolStripMenuItem
            // 
            this.sendKeysToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cTRLALTDELToolStripMenuItem,
            this.aLTF4ToolStripMenuItem,
            this.cTRLESCToolStripMenuItem,
            this.cTRLToolStripMenuItem,
            this.aLTToolStripMenuItem});
            this.sendKeysToolStripMenuItem.Name = "sendKeysToolStripMenuItem";
            this.sendKeysToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.sendKeysToolStripMenuItem.Text = "Send Keys";
            // 
            // cTRLALTDELToolStripMenuItem
            // 
            this.cTRLALTDELToolStripMenuItem.Enabled = false;
            this.cTRLALTDELToolStripMenuItem.Name = "cTRLALTDELToolStripMenuItem";
            this.cTRLALTDELToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.cTRLALTDELToolStripMenuItem.Text = "CTRL + ALT + DEL";
            this.cTRLALTDELToolStripMenuItem.Click += new System.EventHandler(this.cTRLALTDELToolStripMenuItem_Click);
            // 
            // aLTF4ToolStripMenuItem
            // 
            this.aLTF4ToolStripMenuItem.Enabled = false;
            this.aLTF4ToolStripMenuItem.Name = "aLTF4ToolStripMenuItem";
            this.aLTF4ToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.aLTF4ToolStripMenuItem.Text = "ALT + F4";
            this.aLTF4ToolStripMenuItem.Click += new System.EventHandler(this.aLTF4ToolStripMenuItem_Click);
            // 
            // cTRLESCToolStripMenuItem
            // 
            this.cTRLESCToolStripMenuItem.Enabled = false;
            this.cTRLESCToolStripMenuItem.Name = "cTRLESCToolStripMenuItem";
            this.cTRLESCToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.cTRLESCToolStripMenuItem.Text = "CTRL + ESC";
            this.cTRLESCToolStripMenuItem.Click += new System.EventHandler(this.cTRLESCToolStripMenuItem_Click);
            // 
            // cTRLToolStripMenuItem
            // 
            this.cTRLToolStripMenuItem.Enabled = false;
            this.cTRLToolStripMenuItem.Name = "cTRLToolStripMenuItem";
            this.cTRLToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.cTRLToolStripMenuItem.Text = "CTRL";
            this.cTRLToolStripMenuItem.Click += new System.EventHandler(this.cTRLToolStripMenuItem_Click);
            // 
            // aLTToolStripMenuItem
            // 
            this.aLTToolStripMenuItem.Enabled = false;
            this.aLTToolStripMenuItem.Name = "aLTToolStripMenuItem";
            this.aLTToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.aLTToolStripMenuItem.Text = "ALT";
            this.aLTToolStripMenuItem.Click += new System.EventHandler(this.aLTToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Enabled = false;
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.pasteToolStripMenuItem.Text = "Copy local clipboard to host";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clippedViewToolStripMenuItem,
            this.scaledViewToolStripMenuItem,
            this.toolStripMenuItem2,
            this.fullScreenRefreshToolStripMenuItem,
            this.viewOnlyToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // clippedViewToolStripMenuItem
            // 
            this.clippedViewToolStripMenuItem.Checked = true;
            this.clippedViewToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.clippedViewToolStripMenuItem.Name = "clippedViewToolStripMenuItem";
            this.clippedViewToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.clippedViewToolStripMenuItem.Text = "C&lipped View";
            this.clippedViewToolStripMenuItem.Click += new System.EventHandler(this.clippedViewToolStripMenuItem_Click);
            // 
            // scaledViewToolStripMenuItem
            // 
            this.scaledViewToolStripMenuItem.Name = "scaledViewToolStripMenuItem";
            this.scaledViewToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.scaledViewToolStripMenuItem.Text = "Sc&aled View";
            this.scaledViewToolStripMenuItem.Click += new System.EventHandler(this.scaledViewToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(170, 6);
            // 
            // fullScreenRefreshToolStripMenuItem
            // 
            this.fullScreenRefreshToolStripMenuItem.Enabled = false;
            this.fullScreenRefreshToolStripMenuItem.Name = "fullScreenRefreshToolStripMenuItem";
            this.fullScreenRefreshToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.fullScreenRefreshToolStripMenuItem.Text = "Full Screen Refresh";
            this.fullScreenRefreshToolStripMenuItem.Click += new System.EventHandler(this.fullScreenRefreshToolStripMenuItem_Click);
            // 
            // viewOnlyToolStripMenuItem
            // 
            this.viewOnlyToolStripMenuItem.Name = "viewOnlyToolStripMenuItem";
            this.viewOnlyToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.viewOnlyToolStripMenuItem.Text = "View Only";
            this.viewOnlyToolStripMenuItem.Click += new System.EventHandler(this.viewOnlyToolStripMenuItem_Click);
            // 
            // rd
            // 
            this.rd.AutoScroll = true;
            this.rd.AutoScrollMinSize = new System.Drawing.Size(608, 427);
            this.rd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rd.Location = new System.Drawing.Point(0, 24);
            this.rd.Name = "rd";
            this.rd.Size = new System.Drawing.Size(724, 551);
            this.rd.TabIndex = 1;
            this.rd.ConnectComplete += new VncSharp.ConnectCompleteHandler(this.rd_ConnectComplete);
            this.rd.ClipboardChanged += new System.EventHandler(this.rd_ClipboardChanged);
            this.rd.ConnectionLost += new System.EventHandler(this.rd_ConnectionLost);
            // 
            // form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(724, 575);
            this.Controls.Add(this.rd);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VncSharp Client Example";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private VncSharp.RemoteDesktop rd;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disconnectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clippedViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scaledViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sendKeysToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cTRLALTDELToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem fullScreenRefreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aLTF4ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cTRLESCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cTRLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aLTToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewOnlyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
    }
}

