
using System.ComponentModel;
using System.Windows.Forms;

namespace Creeper.singleForms
{
    partial class singleFormHVNC
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(singleFormHVNC));
            this.paneltop = new System.Windows.Forms.Panel();
            this.labelCreeper = new System.Windows.Forms.Label();
            this.buttonmin = new System.Windows.Forms.Button();
            this.buttonmax = new System.Windows.Forms.Button();
            this.buttonclose = new System.Windows.Forms.Button();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.startExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startCmdToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startPowershellToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonclose = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonmax = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonmin = new System.Windows.Forms.ToolStripButton();
            this.pictureBoxHVNC = new System.Windows.Forms.PictureBox();
            this.paneltop.SuspendLayout();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHVNC)).BeginInit();
            this.SuspendLayout();
            // 
            // paneltop
            // 
            this.paneltop.BackColor = System.Drawing.SystemColors.Control;
            this.paneltop.Controls.Add(this.labelCreeper);
            this.paneltop.Controls.Add(this.buttonmin);
            this.paneltop.Controls.Add(this.buttonmax);
            this.paneltop.Controls.Add(this.buttonclose);
            resources.ApplyResources(this.paneltop, "paneltop");
            this.paneltop.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(81)))), ((int)(((byte)(81)))));
            this.paneltop.Name = "paneltop";
            this.paneltop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.paneltop_MouseDown);
            // 
            // labelCreeper
            // 
            resources.ApplyResources(this.labelCreeper, "labelCreeper");
            this.labelCreeper.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(81)))), ((int)(((byte)(81)))));
            this.labelCreeper.Name = "labelCreeper";
            this.labelCreeper.MouseDown += new System.Windows.Forms.MouseEventHandler(this.labelCreeper_MouseDown);
            // 
            // buttonmin
            // 
            resources.ApplyResources(this.buttonmin, "buttonmin");
            this.buttonmin.BackColor = System.Drawing.SystemColors.Control;
            this.buttonmin.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.buttonmin.FlatAppearance.BorderSize = 0;
            this.buttonmin.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.ControlLight;
            this.buttonmin.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.ControlDark;
            this.buttonmin.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.buttonmin.Image = global::Creeper.Properties.Resources.min_dark;
            this.buttonmin.Name = "buttonmin";
            this.buttonmin.UseVisualStyleBackColor = false;
            this.buttonmin.Click += new System.EventHandler(this.buttonmin_Click);
            // 
            // buttonmax
            // 
            resources.ApplyResources(this.buttonmax, "buttonmax");
            this.buttonmax.BackColor = System.Drawing.SystemColors.Control;
            this.buttonmax.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.buttonmax.FlatAppearance.BorderSize = 0;
            this.buttonmax.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.ControlLight;
            this.buttonmax.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.ControlDark;
            this.buttonmax.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.buttonmax.Image = global::Creeper.Properties.Resources.max_dark;
            this.buttonmax.Name = "buttonmax";
            this.buttonmax.UseVisualStyleBackColor = false;
            this.buttonmax.Click += new System.EventHandler(this.buttonmax_Click);
            // 
            // buttonclose
            // 
            resources.ApplyResources(this.buttonclose, "buttonclose");
            this.buttonclose.BackColor = System.Drawing.SystemColors.Control;
            this.buttonclose.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.buttonclose.FlatAppearance.BorderSize = 0;
            this.buttonclose.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.ControlLight;
            this.buttonclose.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.ControlDark;
            this.buttonclose.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.buttonclose.Image = global::Creeper.Properties.Resources.close_dark;
            this.buttonclose.Name = "buttonclose";
            this.buttonclose.UseVisualStyleBackColor = false;
            this.buttonclose.Click += new System.EventHandler(this.buttonclose_Click);
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton,
            this.toolStripButtonclose,
            this.toolStripButtonmax,
            this.toolStripButtonmin});
            this.toolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            resources.ApplyResources(this.toolStrip, "toolStrip");
            this.toolStrip.Name = "toolStrip";
            // 
            // toolStripDropDownButton
            // 
            this.toolStripDropDownButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startExplorerToolStripMenuItem,
            this.startCmdToolStripMenuItem,
            this.startPowershellToolStripMenuItem});
            resources.ApplyResources(this.toolStripDropDownButton, "toolStripDropDownButton");
            this.toolStripDropDownButton.Name = "toolStripDropDownButton";
            // 
            // startExplorerToolStripMenuItem
            // 
            this.startExplorerToolStripMenuItem.Name = "startExplorerToolStripMenuItem";
            resources.ApplyResources(this.startExplorerToolStripMenuItem, "startExplorerToolStripMenuItem");
            this.startExplorerToolStripMenuItem.Click += new System.EventHandler(this.startExplorerToolStripMenuItem_Click);
            // 
            // startCmdToolStripMenuItem
            // 
            this.startCmdToolStripMenuItem.Name = "startCmdToolStripMenuItem";
            resources.ApplyResources(this.startCmdToolStripMenuItem, "startCmdToolStripMenuItem");
            this.startCmdToolStripMenuItem.Click += new System.EventHandler(this.startCmdToolStripMenuItem_Click);
            // 
            // startPowershellToolStripMenuItem
            // 
            this.startPowershellToolStripMenuItem.Name = "startPowershellToolStripMenuItem";
            resources.ApplyResources(this.startPowershellToolStripMenuItem, "startPowershellToolStripMenuItem");
            this.startPowershellToolStripMenuItem.Click += new System.EventHandler(this.startPowershellToolStripMenuItem_Click);
            // 
            // toolStripButtonclose
            // 
            this.toolStripButtonclose.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonclose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonclose.Image = global::Creeper.Properties.Resources.close_dark;
            resources.ApplyResources(this.toolStripButtonclose, "toolStripButtonclose");
            this.toolStripButtonclose.Name = "toolStripButtonclose";
            this.toolStripButtonclose.Click += new System.EventHandler(this.toolStripButtonclose_Click);
            // 
            // toolStripButtonmax
            // 
            this.toolStripButtonmax.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonmax.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonmax.Image = global::Creeper.Properties.Resources.max_dark;
            resources.ApplyResources(this.toolStripButtonmax, "toolStripButtonmax");
            this.toolStripButtonmax.Name = "toolStripButtonmax";
            this.toolStripButtonmax.Click += new System.EventHandler(this.toolStripButtonmax_Click);
            // 
            // toolStripButtonmin
            // 
            this.toolStripButtonmin.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonmin.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonmin.Image = global::Creeper.Properties.Resources.min_dark;
            resources.ApplyResources(this.toolStripButtonmin, "toolStripButtonmin");
            this.toolStripButtonmin.Name = "toolStripButtonmin";
            this.toolStripButtonmin.Click += new System.EventHandler(this.toolStripButtonmin_Click);
            // 
            // pictureBoxHVNC
            // 
            this.pictureBoxHVNC.BackColor = System.Drawing.SystemColors.Desktop;
            resources.ApplyResources(this.pictureBoxHVNC, "pictureBoxHVNC");
            this.pictureBoxHVNC.Name = "pictureBoxHVNC";
            this.pictureBoxHVNC.TabStop = false;
            this.pictureBoxHVNC.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxHVNC_MouseDown);
            this.pictureBoxHVNC.MouseEnter += new System.EventHandler(this.pictureBoxHVNC_MouseEnter);
            this.pictureBoxHVNC.MouseLeave += new System.EventHandler(this.pictureBoxHVNC_MouseLeave);
            this.pictureBoxHVNC.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxHVNC_MouseMove);
            this.pictureBoxHVNC.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxHVNC_MouseUp);
            // 
            // singleFormHVNC
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBoxHVNC);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.paneltop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "singleFormHVNC";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.singleFormHVNC_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.singleFormHVNC_KeyUp);
            this.paneltop.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHVNC)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Panel paneltop;
        private Label labelCreeper;
        private Button buttonmin;
        private Button buttonmax;
        private Button buttonclose;
        private ToolStrip toolStrip;
        private ToolStripDropDownButton toolStripDropDownButton;
        private ToolStripMenuItem startExplorerToolStripMenuItem;
        private ToolStripMenuItem startCmdToolStripMenuItem;
        private ToolStripMenuItem startPowershellToolStripMenuItem;
        public PictureBox pictureBoxHVNC;
        private ToolStripButton toolStripButtonclose;
        private ToolStripButton toolStripButtonmax;
        private ToolStripButton toolStripButtonmin;
    }
}