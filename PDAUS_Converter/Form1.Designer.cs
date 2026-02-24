namespace PDAUS_Converter
{
    partial class Form1
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            timer1 = new System.Windows.Forms.Timer(components);
            notifyIcon1 = new System.Windows.Forms.NotifyIcon(components);
            contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(components);
            openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            exitProgramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            panelMain1 = new System.Windows.Forms.Panel();
            txtStatus = new System.Windows.Forms.TextBox();
            labelHeader = new System.Windows.Forms.Label();
            timeNow = new System.Windows.Forms.Label();
            timer2 = new System.Windows.Forms.Timer(components);
            contextMenuStrip1.SuspendLayout();
            panelMain1.SuspendLayout();
            SuspendLayout();
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 10000;
            timer1.Tick += timer1_Tick;
            // 
            // notifyIcon1
            // 
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            notifyIcon1.Icon = (System.Drawing.Icon)resources.GetObject("notifyIcon1.Icon");
            notifyIcon1.Text = "PDAU Background Worker";
            notifyIcon1.Visible = true;
            notifyIcon1.MouseDoubleClick += notifyIcon1_MouseDoubleClick;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { openToolStripMenuItem, exitProgramToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new System.Drawing.Size(142, 48);
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // exitProgramToolStripMenuItem
            // 
            exitProgramToolStripMenuItem.Name = "exitProgramToolStripMenuItem";
            exitProgramToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            exitProgramToolStripMenuItem.Text = "Exit Program";
            // 
            // panelMain1
            // 
            panelMain1.Controls.Add(txtStatus);
            panelMain1.Controls.Add(labelHeader);
            panelMain1.Controls.Add(timeNow);
            panelMain1.Dock = System.Windows.Forms.DockStyle.Fill;
            panelMain1.Location = new System.Drawing.Point(0, 0);
            panelMain1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelMain1.Name = "panelMain1";
            panelMain1.Size = new System.Drawing.Size(493, 347);
            panelMain1.TabIndex = 4;
            // 
            // txtStatus
            // 
            txtStatus.BorderStyle = System.Windows.Forms.BorderStyle.None;
            txtStatus.Font = new System.Drawing.Font("Arial", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            txtStatus.Location = new System.Drawing.Point(4, 233);
            txtStatus.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtStatus.Name = "txtStatus";
            txtStatus.ReadOnly = true;
            txtStatus.Size = new System.Drawing.Size(486, 34);
            txtStatus.TabIndex = 0;
            txtStatus.TabStop = false;
            txtStatus.Text = "IDLE";
            txtStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelHeader
            // 
            labelHeader.AutoSize = true;
            labelHeader.Font = new System.Drawing.Font("Arial", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            labelHeader.ForeColor = System.Drawing.Color.Red;
            labelHeader.Location = new System.Drawing.Point(180, 171);
            labelHeader.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelHeader.Name = "labelHeader";
            labelHeader.Size = new System.Drawing.Size(122, 42);
            labelHeader.TabIndex = 5;
            labelHeader.Text = "Status";
            labelHeader.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // timeNow
            // 
            timeNow.AutoSize = true;
            timeNow.Font = new System.Drawing.Font("Arial", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            timeNow.Location = new System.Drawing.Point(6, 31);
            timeNow.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            timeNow.Name = "timeNow";
            timeNow.Size = new System.Drawing.Size(391, 72);
            timeNow.TabIndex = 3;
            timeNow.Text = "12:00:00 am";
            timeNow.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // timer2
            // 
            timer2.Enabled = true;
            timer2.Interval = 1000;
            timer2.Tick += timer2_Tick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(493, 347);
            Controls.Add(panelMain1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "PDAU Background Worker";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            SizeChanged += Form1_SizeChanged;
            contextMenuStrip1.ResumeLayout(false);
            panelMain1.ResumeLayout(false);
            panelMain1.PerformLayout();
            ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitProgramToolStripMenuItem;
        private System.Windows.Forms.Panel panelMain1;
        private System.Windows.Forms.Label timeNow;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Label labelHeader;
        private System.Windows.Forms.TextBox txtStatus;
    }
}

