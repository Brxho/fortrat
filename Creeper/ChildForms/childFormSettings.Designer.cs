
using System.ComponentModel;
using System.Windows.Forms;

namespace Creeper.ChildForms
{
    partial class childFormSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(childFormSettings));
            this.panelSettingsBack = new System.Windows.Forms.Panel();
            this.panelSettings = new System.Windows.Forms.Panel();
            this.groupBoxtheme = new System.Windows.Forms.GroupBox();
            this.comboBoxlanguage = new System.Windows.Forms.ComboBox();
            this.labellanguage = new System.Windows.Forms.Label();
            this.trackBaropacity = new System.Windows.Forms.TrackBar();
            this.radioButtonlight = new System.Windows.Forms.RadioButton();
            this.radioButtondark = new System.Windows.Forms.RadioButton();
            this.labelopacity = new System.Windows.Forms.Label();
            this.labelcolor = new System.Windows.Forms.Label();
            this.panelSettingsBack.SuspendLayout();
            this.panelSettings.SuspendLayout();
            this.groupBoxtheme.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBaropacity)).BeginInit();
            this.SuspendLayout();
            // 
            // panelSettingsBack
            // 
            resources.ApplyResources(this.panelSettingsBack, "panelSettingsBack");
            this.panelSettingsBack.Controls.Add(this.panelSettings);
            this.panelSettingsBack.Name = "panelSettingsBack";
            // 
            // panelSettings
            // 
            this.panelSettings.Controls.Add(this.groupBoxtheme);
            resources.ApplyResources(this.panelSettings, "panelSettings");
            this.panelSettings.Name = "panelSettings";
            // 
            // groupBoxtheme
            // 
            resources.ApplyResources(this.groupBoxtheme, "groupBoxtheme");
            this.groupBoxtheme.Controls.Add(this.comboBoxlanguage);
            this.groupBoxtheme.Controls.Add(this.labellanguage);
            this.groupBoxtheme.Controls.Add(this.trackBaropacity);
            this.groupBoxtheme.Controls.Add(this.radioButtonlight);
            this.groupBoxtheme.Controls.Add(this.radioButtondark);
            this.groupBoxtheme.Controls.Add(this.labelopacity);
            this.groupBoxtheme.Controls.Add(this.labelcolor);
            this.groupBoxtheme.Name = "groupBoxtheme";
            this.groupBoxtheme.TabStop = false;
            // 
            // comboBoxlanguage
            // 
            this.comboBoxlanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxlanguage.FormattingEnabled = true;
            this.comboBoxlanguage.Items.AddRange(new object[] {
            resources.GetString("comboBoxlanguage.Items"),
            resources.GetString("comboBoxlanguage.Items1")});
            resources.ApplyResources(this.comboBoxlanguage, "comboBoxlanguage");
            this.comboBoxlanguage.Name = "comboBoxlanguage";
            this.comboBoxlanguage.SelectedIndexChanged += new System.EventHandler(this.comboBoxlanguage_SelectedIndexChanged);
            // 
            // labellanguage
            // 
            resources.ApplyResources(this.labellanguage, "labellanguage");
            this.labellanguage.Name = "labellanguage";
            // 
            // trackBaropacity
            // 
            this.trackBaropacity.LargeChange = 1;
            resources.ApplyResources(this.trackBaropacity, "trackBaropacity");
            this.trackBaropacity.Maximum = 100;
            this.trackBaropacity.Minimum = 80;
            this.trackBaropacity.Name = "trackBaropacity";
            this.trackBaropacity.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBaropacity.Value = 97;
            this.trackBaropacity.Scroll += new System.EventHandler(this.trackBaropacity_Scroll);
            // 
            // radioButtonlight
            // 
            resources.ApplyResources(this.radioButtonlight, "radioButtonlight");
            this.radioButtonlight.Checked = true;
            this.radioButtonlight.Name = "radioButtonlight";
            this.radioButtonlight.TabStop = true;
            this.radioButtonlight.UseVisualStyleBackColor = true;
            this.radioButtonlight.CheckedChanged += new System.EventHandler(this.radioButtonlight_CheckedChanged);
            // 
            // radioButtondark
            // 
            resources.ApplyResources(this.radioButtondark, "radioButtondark");
            this.radioButtondark.Name = "radioButtondark";
            this.radioButtondark.UseVisualStyleBackColor = true;
            // 
            // labelopacity
            // 
            resources.ApplyResources(this.labelopacity, "labelopacity");
            this.labelopacity.Name = "labelopacity";
            // 
            // labelcolor
            // 
            resources.ApplyResources(this.labelcolor, "labelcolor");
            this.labelcolor.Name = "labelcolor";
            // 
            // childFormSettings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelSettingsBack);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "childFormSettings";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.childFormSettings_Load);
            this.panelSettingsBack.ResumeLayout(false);
            this.panelSettings.ResumeLayout(false);
            this.groupBoxtheme.ResumeLayout(false);
            this.groupBoxtheme.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBaropacity)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panelSettingsBack;
        private Panel panelSettings;
        private GroupBox groupBoxtheme;
        private Label labelopacity;
        private Label labelcolor;
        private TrackBar trackBaropacity;
        private RadioButton radioButtonlight;
        private RadioButton radioButtondark;
        private ComboBox comboBoxlanguage;
        private Label labellanguage;
    }
}