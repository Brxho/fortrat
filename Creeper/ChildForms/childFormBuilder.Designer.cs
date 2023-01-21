
using System.ComponentModel;
using System.Windows.Forms;
using Creeper.Controls;

namespace Creeper.ChildForms
{
    partial class childFormBuilder
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(childFormBuilder));
            this.panelbuilderback = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBoxothers = new System.Windows.Forms.GroupBox();
            this.labelversion = new System.Windows.Forms.Label();
            this.radioButtonnet35 = new System.Windows.Forms.RadioButton();
            this.radioButtonnet40 = new System.Windows.Forms.RadioButton();
            this.checkBoxshellcode = new System.Windows.Forms.CheckBox();
            this.buttonbuild = new System.Windows.Forms.Button();
            this.numericUpDownsleep = new System.Windows.Forms.NumericUpDown();
            this.toggleButtonofflinekeylogger = new Creeper.Controls.ToggleButton();
            this.toggleButtonantivm = new Creeper.Controls.ToggleButton();
            this.labelsleep = new System.Windows.Forms.Label();
            this.labelmutex = new System.Windows.Forms.Label();
            this.textBoxmutex = new System.Windows.Forms.TextBox();
            this.labelgroup = new System.Windows.Forms.Label();
            this.textBoxgroup = new System.Windows.Forms.TextBox();
            this.labelofflinekeylogger = new System.Windows.Forms.Label();
            this.labelantivm = new System.Windows.Forms.Label();
            this.groupBoxipport = new System.Windows.Forms.GroupBox();
            this.labellink = new System.Windows.Forms.Label();
            this.toggleButtonip = new Creeper.Controls.ToggleButton();
            this.textBoxlink = new System.Windows.Forms.TextBox();
            this.textBoxip = new System.Windows.Forms.TextBox();
            this.labelport = new System.Windows.Forms.Label();
            this.textBoxport = new System.Windows.Forms.TextBox();
            this.labelgetipbylink = new System.Windows.Forms.Label();
            this.labelip = new System.Windows.Forms.Label();
            this.updateUI = new System.Windows.Forms.Timer(this.components);
            this.panelbuilderback.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBoxothers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownsleep)).BeginInit();
            this.groupBoxipport.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelbuilderback
            // 
            resources.ApplyResources(this.panelbuilderback, "panelbuilderback");
            this.panelbuilderback.Controls.Add(this.panel1);
            this.panelbuilderback.Name = "panelbuilderback";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBoxothers);
            this.panel1.Controls.Add(this.groupBoxipport);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // groupBoxothers
            // 
            resources.ApplyResources(this.groupBoxothers, "groupBoxothers");
            this.groupBoxothers.Controls.Add(this.labelversion);
            this.groupBoxothers.Controls.Add(this.radioButtonnet35);
            this.groupBoxothers.Controls.Add(this.radioButtonnet40);
            this.groupBoxothers.Controls.Add(this.checkBoxshellcode);
            this.groupBoxothers.Controls.Add(this.buttonbuild);
            this.groupBoxothers.Controls.Add(this.numericUpDownsleep);
            this.groupBoxothers.Controls.Add(this.toggleButtonofflinekeylogger);
            this.groupBoxothers.Controls.Add(this.toggleButtonantivm);
            this.groupBoxothers.Controls.Add(this.labelsleep);
            this.groupBoxothers.Controls.Add(this.labelmutex);
            this.groupBoxothers.Controls.Add(this.textBoxmutex);
            this.groupBoxothers.Controls.Add(this.labelgroup);
            this.groupBoxothers.Controls.Add(this.textBoxgroup);
            this.groupBoxothers.Controls.Add(this.labelofflinekeylogger);
            this.groupBoxothers.Controls.Add(this.labelantivm);
            this.groupBoxothers.Name = "groupBoxothers";
            this.groupBoxothers.TabStop = false;
            // 
            // labelversion
            // 
            resources.ApplyResources(this.labelversion, "labelversion");
            this.labelversion.Name = "labelversion";
            // 
            // radioButtonnet35
            // 
            resources.ApplyResources(this.radioButtonnet35, "radioButtonnet35");
            this.radioButtonnet35.Name = "radioButtonnet35";
            this.radioButtonnet35.UseVisualStyleBackColor = true;
            // 
            // radioButtonnet40
            // 
            resources.ApplyResources(this.radioButtonnet40, "radioButtonnet40");
            this.radioButtonnet40.Checked = true;
            this.radioButtonnet40.Name = "radioButtonnet40";
            this.radioButtonnet40.TabStop = true;
            this.radioButtonnet40.UseVisualStyleBackColor = true;
            // 
            // checkBoxshellcode
            // 
            resources.ApplyResources(this.checkBoxshellcode, "checkBoxshellcode");
            this.checkBoxshellcode.Name = "checkBoxshellcode";
            this.checkBoxshellcode.UseVisualStyleBackColor = true;
            // 
            // buttonbuild
            // 
            resources.ApplyResources(this.buttonbuild, "buttonbuild");
            this.buttonbuild.Name = "buttonbuild";
            this.buttonbuild.UseVisualStyleBackColor = true;
            this.buttonbuild.Click += new System.EventHandler(this.buttonbuild_Click);
            // 
            // numericUpDownsleep
            // 
            resources.ApplyResources(this.numericUpDownsleep, "numericUpDownsleep");
            this.numericUpDownsleep.Name = "numericUpDownsleep";
            // 
            // toggleButtonofflinekeylogger
            // 
            this.toggleButtonofflinekeylogger.BackColor = System.Drawing.Color.Transparent;
            this.toggleButtonofflinekeylogger.Checked = false;
            this.toggleButtonofflinekeylogger.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.toggleButtonofflinekeylogger, "toggleButtonofflinekeylogger");
            this.toggleButtonofflinekeylogger.Name = "toggleButtonofflinekeylogger";
            // 
            // toggleButtonantivm
            // 
            this.toggleButtonantivm.BackColor = System.Drawing.Color.Transparent;
            this.toggleButtonantivm.Checked = false;
            this.toggleButtonantivm.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.toggleButtonantivm, "toggleButtonantivm");
            this.toggleButtonantivm.Name = "toggleButtonantivm";
            // 
            // labelsleep
            // 
            resources.ApplyResources(this.labelsleep, "labelsleep");
            this.labelsleep.Name = "labelsleep";
            // 
            // labelmutex
            // 
            resources.ApplyResources(this.labelmutex, "labelmutex");
            this.labelmutex.Name = "labelmutex";
            // 
            // textBoxmutex
            // 
            resources.ApplyResources(this.textBoxmutex, "textBoxmutex");
            this.textBoxmutex.Name = "textBoxmutex";
            // 
            // labelgroup
            // 
            resources.ApplyResources(this.labelgroup, "labelgroup");
            this.labelgroup.Name = "labelgroup";
            // 
            // textBoxgroup
            // 
            resources.ApplyResources(this.textBoxgroup, "textBoxgroup");
            this.textBoxgroup.Name = "textBoxgroup";
            // 
            // labelofflinekeylogger
            // 
            resources.ApplyResources(this.labelofflinekeylogger, "labelofflinekeylogger");
            this.labelofflinekeylogger.Name = "labelofflinekeylogger";
            // 
            // labelantivm
            // 
            resources.ApplyResources(this.labelantivm, "labelantivm");
            this.labelantivm.Name = "labelantivm";
            // 
            // groupBoxipport
            // 
            resources.ApplyResources(this.groupBoxipport, "groupBoxipport");
            this.groupBoxipport.Controls.Add(this.labellink);
            this.groupBoxipport.Controls.Add(this.toggleButtonip);
            this.groupBoxipport.Controls.Add(this.textBoxlink);
            this.groupBoxipport.Controls.Add(this.textBoxip);
            this.groupBoxipport.Controls.Add(this.labelport);
            this.groupBoxipport.Controls.Add(this.textBoxport);
            this.groupBoxipport.Controls.Add(this.labelgetipbylink);
            this.groupBoxipport.Controls.Add(this.labelip);
            this.groupBoxipport.Name = "groupBoxipport";
            this.groupBoxipport.TabStop = false;
            // 
            // labellink
            // 
            resources.ApplyResources(this.labellink, "labellink");
            this.labellink.Name = "labellink";
            // 
            // toggleButtonip
            // 
            this.toggleButtonip.BackColor = System.Drawing.Color.Transparent;
            this.toggleButtonip.Checked = false;
            this.toggleButtonip.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.toggleButtonip, "toggleButtonip");
            this.toggleButtonip.Name = "toggleButtonip";
            this.toggleButtonip.MouseClick += new System.Windows.Forms.MouseEventHandler(this.toggleButtonip_MouseClick);
            // 
            // textBoxlink
            // 
            resources.ApplyResources(this.textBoxlink, "textBoxlink");
            this.textBoxlink.Name = "textBoxlink";
            // 
            // textBoxip
            // 
            resources.ApplyResources(this.textBoxip, "textBoxip");
            this.textBoxip.Name = "textBoxip";
            // 
            // labelport
            // 
            resources.ApplyResources(this.labelport, "labelport");
            this.labelport.Name = "labelport";
            // 
            // textBoxport
            // 
            resources.ApplyResources(this.textBoxport, "textBoxport");
            this.textBoxport.Name = "textBoxport";
            // 
            // labelgetipbylink
            // 
            resources.ApplyResources(this.labelgetipbylink, "labelgetipbylink");
            this.labelgetipbylink.Name = "labelgetipbylink";
            // 
            // labelip
            // 
            resources.ApplyResources(this.labelip, "labelip");
            this.labelip.Name = "labelip";
            // 
            // updateUI
            // 
            this.updateUI.Enabled = true;
            this.updateUI.Interval = 1000;
            this.updateUI.Tick += new System.EventHandler(this.updateUI_Tick);
            // 
            // childFormBuilder
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelbuilderback);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "childFormBuilder";
            this.ShowInTaskbar = false;
            this.panelbuilderback.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBoxothers.ResumeLayout(false);
            this.groupBoxothers.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownsleep)).EndInit();
            this.groupBoxipport.ResumeLayout(false);
            this.groupBoxipport.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panelbuilderback;
        private Panel panel1;
        private GroupBox groupBoxipport;
        private TextBox textBoxip;
        private Label labelport;
        private TextBox textBoxport;
        private Label labelip;
        private ToggleButton toggleButtonip;
        private TextBox textBoxlink;
        private Label labelgetipbylink;
        private Label labellink;
        private Timer updateUI;
        private GroupBox groupBoxothers;
        private ToggleButton toggleButtonantivm;
        private Label labelgroup;
        private TextBox textBoxgroup;
        private Label labelantivm;
        private Button buttonbuild;
        private NumericUpDown numericUpDownsleep;
        private Label labelsleep;
        private ToggleButton toggleButtonofflinekeylogger;
        private Label labelofflinekeylogger;
        private Label labelmutex;
        private TextBox textBoxmutex;
        private CheckBox checkBoxshellcode;
        private Label labelversion;
        private RadioButton radioButtonnet35;
        private RadioButton radioButtonnet40;
    }
}