using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Creeper.Connection;
using Creeper.Helper.StreamLibrary.UnsafeCodecs;
using Creeper.Properties;
using MessagePack;

namespace Creeper.singleForms
{
    public partial class singleFormHVNC : Form
    {
        public Clients Client { get; set; }
        public Size rdSize;
        private readonly List<Keys> _keysPressed;
        public object syncPicbox = new object();
        public Image GetImage { get; set; }

        public singleFormHVNC()
        {
            _keysPressed = new List<Keys>();
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            InitBorder();
            SetTheme();
        }

        #region Theme

        private void SetTheme()
        {
            var darkTheme = Settings.Default.darkTheme;

            var colorSide = darkTheme ? Settings.Default.colorsidedark : Settings.Default.colorside;
            var colorText = darkTheme ? Settings.Default.colortextdark : Settings.Default.colortext;

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

            toolStrip.ForeColor = colorText;
            toolStrip.BackColor = colorSide;

            toolStripButtonmin.Image = darkTheme ? Resources.min : Resources.min_dark;
            toolStripButtonmax.Image = darkTheme ? Resources.max : Resources.max_dark;
            toolStripButtonclose.Image = darkTheme ? Resources.close : Resources.close_dark;

            startExplorerToolStripMenuItem.ForeColor = colorText;
            startCmdToolStripMenuItem.ForeColor = colorText;
            startPowershellToolStripMenuItem.ForeColor = colorText;

            startExplorerToolStripMenuItem.BackColor = colorSide;
            startCmdToolStripMenuItem.BackColor = colorSide;
            startPowershellToolStripMenuItem.BackColor = colorSide;
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
            int maxwidth = 700;
            int maxheight = 450;

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
            if (Client != null)
            {
                var pack = new MsgPack();
                pack.ForcePathObject("Packet").AsString = "hvncStop";
                ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
            }

            Close();
        }

        private void pictureBoxHVNC_MouseDown(object sender, MouseEventArgs e)
        {
            var p = new Point(e.X * rdSize.Width / pictureBoxHVNC.Width, e.Y * rdSize.Height / pictureBoxHVNC.Height);
            var button = 0;
            if (e.Button == MouseButtons.Left) button = 2;

            if (e.Button == MouseButtons.Right) button = 8;
            var pack = new MsgPack();
            pack.ForcePathObject("Packet").AsString = "hvncMouse";
            pack.ForcePathObject("X").AsInteger = p.X;
            pack.ForcePathObject("Y").AsInteger = p.Y;
            pack.ForcePathObject("Button").AsInteger = button;
            ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
        }

        private void pictureBoxHVNC_MouseUp(object sender, MouseEventArgs e)
        {
            var p = new Point(e.X * rdSize.Width / pictureBoxHVNC.Width, e.Y * rdSize.Height / pictureBoxHVNC.Height);
            var button = 0;
            if (e.Button == MouseButtons.Left) button = 4;

            if (e.Button == MouseButtons.Right) button = 16;
            var pack = new MsgPack();
            pack.ForcePathObject("Packet").AsString = "hvncMouse";
            pack.ForcePathObject("X").AsInteger = p.X;
            pack.ForcePathObject("Y").AsInteger = p.Y;
            pack.ForcePathObject("Button").AsInteger = button;
            ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
        }

        private void pictureBoxHVNC_MouseMove(object sender, MouseEventArgs e)
        {
            var p = new Point(e.X * rdSize.Width / pictureBoxHVNC.Width, e.Y * rdSize.Height / pictureBoxHVNC.Height);

            var pack = new MsgPack();
            pack.ForcePathObject("Packet").AsString = "hvncMouseMove";
            pack.ForcePathObject("X").AsInteger = p.X;
            pack.ForcePathObject("Y").AsInteger = p.Y;
            ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
        }

        private bool IsLockKey(Keys key)
        {
            return (key & Keys.CapsLock) == Keys.CapsLock
                   || (key & Keys.NumLock) == Keys.NumLock
                   || (key & Keys.Scroll) == Keys.Scroll;
        }

        private void singleFormHVNC_KeyDown(object sender, KeyEventArgs e)
        {
            if (pictureBoxHVNC.Image != null)
            {
                if (!IsLockKey(e.KeyCode)) e.Handled = true;

                if (_keysPressed.Contains(e.KeyCode)) return;

                _keysPressed.Add(e.KeyCode);

                var pack = new MsgPack();
                pack.ForcePathObject("Packet").AsString = "hvncKeyboard";
                pack.ForcePathObject("key").AsInteger = Convert.ToInt32(e.KeyCode);
                ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
            }
        }

        private void singleFormHVNC_KeyUp(object sender, KeyEventArgs e)
        {
            if (pictureBoxHVNC.Image != null)
            {
                if (!IsLockKey(e.KeyCode)) e.Handled = true;

                _keysPressed.Remove(e.KeyCode);
            }
        }

        private void startExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pack = new MsgPack();
            pack.ForcePathObject("Packet").AsString = "executeNewDesktop";
            pack.ForcePathObject("File").AsString = @"C:\Windows\explorer.exe";
            ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
        }

        private void startCmdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pack = new MsgPack();
            pack.ForcePathObject("Packet").AsString = "executeNewDesktop";
            pack.ForcePathObject("File").AsString = @"C:\Windows\System32\cmd.exe";
            ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
        }

        private void startPowershellToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pack = new MsgPack();
            pack.ForcePathObject("Packet").AsString = "executeNewDesktop";
            pack.ForcePathObject("File").AsString = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";
            ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
        }

        private void toolStripButtonmin_Click(object sender, EventArgs e)
        {
            var pack = new MsgPack();
            pack.ForcePathObject("Packet").AsString = "hvncCommand";
            pack.ForcePathObject("Command").AsString = "min";
            ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
        }

        private void toolStripButtonmax_Click(object sender, EventArgs e)
        {
            var pack = new MsgPack();
            pack.ForcePathObject("Packet").AsString = "hvncCommand";
            pack.ForcePathObject("Command").AsString = "max";
            ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
        }

        private void toolStripButtonclose_Click(object sender, EventArgs e)
        {
            var pack = new MsgPack();
            pack.ForcePathObject("Packet").AsString = "hvncCommand";
            pack.ForcePathObject("Command").AsString = "close";
            ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
        }

        private void pictureBoxHVNC_MouseEnter(object sender, EventArgs e)
        {
            Focus();
        }

        private void pictureBoxHVNC_MouseLeave(object sender, EventArgs e)
        {
            FindForm().ActiveControl = null;
        }
    }
}