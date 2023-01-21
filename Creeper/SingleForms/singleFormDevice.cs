using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Creeper.Connection;
using Creeper.Helper;
using Creeper.Properties;
using MessagePack;

namespace Creeper.singleForms
{
    public partial class singleFormDevice : Form
    {
        public Clients Client { get; set; }

        public List<DeviceInfo> deviceInfos { get; set; }

        public singleFormDevice()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            InitBorder();
            SetTheme();
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

            listViewDevice.BackColor = darkTheme ? Settings.Default.colorlistviewdark : Settings.Default.colorlistview;
            listViewDevice.ForeColor = darkTheme ? Settings.Default.colorlistviewtextdark : Settings.Default.colorlistviewtext;
            foreach (ListViewItem item in listViewDevice.Items)
            {
                item.ForeColor = Settings.Default.darkTheme ? Settings.Default.colorlistviewtextdark : Settings.Default.colorlistviewtext;
                item.BackColor = Settings.Default.darkTheme ? Settings.Default.colorlistviewdark : Settings.Default.colorlistview;
            }
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
            int maxwidth = 750;
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
            Close();
        }

        private void listViewDevice_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            int tColumnCount;
            Rectangle tRect = new Rectangle();
            Point tPoint = new Point();
            Font tFont = new Font("Segoe UI", 9, FontStyle.Regular);
            SolidBrush tBackBrush = new SolidBrush(Settings.Default.darkTheme ? Settings.Default.colorlistviewdark : Settings.Default.colorlistview);
            SolidBrush tFtontBrush;
            tFtontBrush = new SolidBrush(Settings.Default.darkTheme ? Settings.Default.colortextdark : Settings.Default.colortext);
            if (listViewDevice.Columns.Count == 0)
            {
                return;
            }

            tColumnCount = listViewDevice.Columns.Count;
            tRect.Y = 0;
            tRect.Height = e.Bounds.Height - 1;
            tPoint.Y = 3;
            for (int i = 0; i < tColumnCount; i++)
            {
                if (i == 0)
                {
                    tRect.X = 0;
                    tRect.Width = listViewDevice.Columns[i].Width;
                }
                else
                {
                    tRect.X += tRect.Width;
                    tRect.X += 1;
                    tRect.Width = listViewDevice.Columns[i].Width - 1;
                }
                e.Graphics.FillRectangle(tBackBrush, tRect);
                tPoint.X = tRect.X + 3;
                e.Graphics.DrawString(listViewDevice.Columns[i].Text, tFont, tFtontBrush, tPoint);
            }
        }

        private void listViewDevice_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void enableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count > 0)
            {
                foreach (ListViewItem P in listViewDevice.SelectedItems)
                {
                    MsgPack pack = new MsgPack();
                    pack.ForcePathObject("Packet").AsString = "deviceSet";
                    pack.ForcePathObject("ID").AsString = P.SubItems[columnHeaderhardwareId.Index].Text;
                    pack.ForcePathObject("Enable").AsString = "true";
                    ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
                }
            }
        }

        private void disableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count > 0)
            {
                foreach (ListViewItem P in listViewDevice.SelectedItems)
                {
                    MsgPack pack = new MsgPack();
                    pack.ForcePathObject("Packet").AsString = "deviceSet";
                    pack.ForcePathObject("ID").AsString = P.SubItems[columnHeaderhardwareId.Index].Text;
                    pack.ForcePathObject("Enable").AsString = "false";
                    ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
                }
            }
        }



        public static void GetAllDevices(string DeviceInfoList, ListView listView)
        {
            List<DeviceInfo> list = new List<DeviceInfo>();
            string[] _NextDevice = DeviceInfoList.Split(new[] { "-=>" }, StringSplitOptions.None);
            for (int i = 0; i < _NextDevice.Length; i++)
            {
                if (_NextDevice[i].Length > 0)
                {
                    DeviceInfo deviceInfo = new DeviceInfo
                    {
                        Name = _NextDevice[i + 0],
                        DeviceId = _NextDevice[i + 1],
                        Description = _NextDevice[i + 2],
                        Manufacturer = _NextDevice[i + 3],
                        Category = (DeviceCategory)Enum.Parse(typeof(DeviceCategory), _NextDevice[i + 4]),
                        CustomCategory = _NextDevice[i + 5],
                        StatusCode = Device.StatusCodeString(Convert.ToUInt32(_NextDevice[i + 6])),
                        HardwareId = _NextDevice[i + 7],
                        DriverName = _NextDevice[i + 8],
                        DriverBuildDate = _NextDevice[i + 9],
                        DriverDescription = _NextDevice[i + 10],
                        DriverVersion = _NextDevice[i + 11],
                        DriverProviderName = _NextDevice[i + 12],
                        DriverSigner = _NextDevice[i + 13],
                        DriverInfName = _NextDevice[i + 14],
                    };
                    list.Add(deviceInfo);
                }
                i += 14;
            }
            var newList = list.GroupBy(u => u.Category);
            foreach (var item in newList)
            {
                if (item.ToList()[0].Category.ToString() == "None")
                {
                    var newList2 = item.GroupBy(u => u.CustomCategory);
                    foreach (var item2 in newList2)
                    {
                        ListViewGroup group = new ListViewGroup(item2.ToList()[0].CustomCategory);
                        listView.Groups.Add(group);
                        foreach (var device in item2.ToList())
                        {
                            ListViewItem lv = new ListViewItem
                            {
                                Text = device.Name
                            };
                            lv.SubItems.Add(device.DeviceId);
                            lv.SubItems.Add(device.Description);
                            lv.SubItems.Add(device.Manufacturer);
                            lv.SubItems.Add(device.StatusCode);
                            lv.SubItems.Add(device.HardwareId);
                            lv.SubItems.Add(device.DriverName);
                            lv.SubItems.Add(device.DriverDescription);
                            lv.SubItems.Add(device.DriverVersion);
                            lv.SubItems.Add(device.DriverProviderName);
                            lv.SubItems.Add(device.DriverBuildDate);
                            lv.SubItems.Add(device.DriverSigner);
                            lv.SubItems.Add(device.DriverInfName);

                            lv.Group = group;
                            listView.Items.Add(lv);
                        }
                    }
                }
                else
                {
                    ListViewGroup group = new ListViewGroup(item.ToList()[0].Category.ToString());
                    listView.Groups.Add(group);
                    foreach (var device in item.ToList())
                    {
                        ListViewItem lv = new ListViewItem
                        {
                            Text = device.Name
                        };
                        lv.SubItems.Add(device.DeviceId);
                        lv.SubItems.Add(device.Description);
                        lv.SubItems.Add(device.Manufacturer);
                        lv.SubItems.Add(device.StatusCode);
                        lv.SubItems.Add(device.HardwareId);
                        lv.SubItems.Add(device.DriverName);
                        lv.SubItems.Add(device.DriverDescription);
                        lv.SubItems.Add(device.DriverVersion);
                        lv.SubItems.Add(device.DriverProviderName);
                        lv.SubItems.Add(device.DriverBuildDate);
                        lv.SubItems.Add(device.DriverSigner);
                        lv.SubItems.Add(device.DriverInfName);

                        lv.Group = group;
                        listView.Items.Add(lv);
                    }
                }
            }
        }
    }
}
