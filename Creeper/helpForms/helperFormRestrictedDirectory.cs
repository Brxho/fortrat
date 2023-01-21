using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Creeper.helpForms
{
    public partial class helperFormRestrictedDirectory : Form
    {
        public helperFormRestrictedDirectory()
        {
            InitializeComponent();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var Msgbox = Interaction.InputBox("Type in the path", "Restricted Directory", "");
            if (string.IsNullOrEmpty(Msgbox))
            {
                return;
            }
            ListViewItem item = new ListViewItem(Msgbox);
            listView.Items.Add(item);
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView.SelectedItems)
            {
                item.Remove();
            }
        }

        private void buttonclose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
{
            List<string> paths = new List<string>();
            
            foreach (ListViewItem item in listView.Items)
            {
                paths.Add(item.Text);
            }
            
            if (paths.Count > 0)
            {
                try
                {
                    string[] str = paths.ToArray();
                    DialogResult = DialogResult.OK;
                    Tag = str;
                }
                catch
                {
                    DialogResult = DialogResult.None;
                }
            }
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
