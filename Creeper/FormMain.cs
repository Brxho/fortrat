using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Creeper.ChildForms;
using Creeper.Connection;
using Creeper.Properties;


namespace Creeper
{
    public partial class FormMain : Form
    {

        private Listener listener;
        #region Childforms

        private static childFormAbout childForm_About;
        private static childFormBuilder childForm_Builder;
        private static childFormSettings childForm_Settings;
        public static childFormHome childForm_Home = new childFormHome();
        private static bool childFormAbout_on;
        private static bool childFormBuilder_on;
        private static bool childFormSettings_on;

        #endregion

        public FormMain()
        {
            InitializeComponent();
            InitBorder();
            openChildForm(childForm_Home);
            SetTheme();
        }

        private async void FormMain_Load(object sender, EventArgs e)
        {
            using (var formInit = new FormInit())
            {
                formInit.ShowDialog();
            }
            await Task.Run(() => Connect());
        }

        private void Connect()
        {
            try
            {
                listener = new Listener();
                Thread thread = new Thread(new ParameterizedThreadStart(listener.Connect));
                thread.IsBackground = true;
                thread.Start(Convert.ToInt32(Settings.Default.Port));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(0);
            }
        }

        #region Theme

        public void SetTheme()
        {
            var darkTheme = Settings.Default.darkTheme;

            var colorSide = darkTheme ? Settings.Default.colorsidedark : Settings.Default.colorside;
            var colorText = darkTheme ? Settings.Default.colortextdark : Settings.Default.colortext;

            BackColor = colorSide;
            ForeColor = colorText;

            panelleft.BackColor = colorSide;
            paneltop.BackColor = colorSide;
            buttonabout.BackColor = colorSide;
            buttonbuilder.BackColor = colorSide;
            buttonclose.BackColor = colorSide;
            buttonhome.BackColor = colorSide;
            buttonmax.BackColor = colorSide;
            buttonmin.BackColor = colorSide;
            buttonsettings.BackColor = colorSide;
            buttonside.BackColor = colorSide;
            labelCreeper.BackColor = colorSide;

            buttonabout.ForeColor = colorText;
            buttonbuilder.ForeColor = colorText;
            buttonhome.ForeColor = colorText;
            buttonsettings.ForeColor = colorText;
            labelCreeper.ForeColor = colorText;

            buttonabout.Image = darkTheme ? Resources.about : Resources.about_dark;
            buttonbuilder.Image = darkTheme ? Resources.builder : Resources.builder_dark;
            buttonclose.Image = darkTheme ? Resources.close : Resources.close_dark;
            buttonhome.Image = darkTheme ? Resources.home : Resources.home_dark;
            buttonmax.Image = darkTheme ? Resources.max : Resources.max_dark;
            buttonmin.Image = darkTheme ? Resources.min : Resources.min_dark;
            buttonsettings.Image = darkTheme ? Resources.setting : Resources.setting_dark;
            buttonside.Image = darkTheme ? Resources.side : Resources.side_dark;
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

        private MouseDirection direction;
        private Point rectangle;

        private void InitBorder()
        {
            MouseDown += BorderMouseDown;

            MouseMove += BorderMouseMove;

            MouseLeave += BorderMouseLeave;
        }

        private void BorderMouseMove(object sender, MouseEventArgs e)
        {
            var tempEndPoint = e.Location;

            if (e.Button == MouseButtons.Left)
            {
                if (direction == MouseDirection.None)
                {
                }
                else if (direction == MouseDirection.East)
                {
                    if (tempEndPoint.X <= 1000)
                        Width = 1000;
                    else
                        Width = tempEndPoint.X;
                }
                else if (direction == MouseDirection.South)
                {
                    if (tempEndPoint.Y <= 500)
                        Height = 500;
                    else
                        Height = tempEndPoint.Y;
                }
                else if (direction == MouseDirection.West)
                {
                    var x = tempEndPoint.X - rectangle.X;
                    if (Width - x <= 1000)
                    {
                        Width = 1000;
                    }
                    else
                    {
                        Width = Width - x;
                        Location = new Point(Location.X + x, Location.Y);
                    }
                }
                else if (direction == MouseDirection.North)
                {
                    var y = tempEndPoint.Y - rectangle.Y;
                    if (Height - y <= 500)
                    {
                        Height = 500;
                    }
                    else
                    {
                        Height = Height - y;
                        Location = new Point(Location.X, Location.Y + y);
                    }
                }
                else if (direction == MouseDirection.Southeast)
                {
                    if (tempEndPoint.X <= 1000)
                        Width = 1000;
                    else
                        Width = tempEndPoint.X;
                    if (tempEndPoint.Y <= 500)
                        Height = 500;
                    else
                        Height = tempEndPoint.Y;
                }
                else if (direction == MouseDirection.Northeast)
                {
                    var y = tempEndPoint.Y - rectangle.Y;
                    if (Height - y <= 500)
                    {
                        Height = 500;
                    }
                    else
                    {
                        Height = Height - y;
                        Location = new Point(Location.X, Location.Y + y);
                    }

                    if (tempEndPoint.X <= 1000)
                        Width = 1000;
                    else
                        Width = tempEndPoint.X;
                }
                else if (direction == MouseDirection.Southwest)
                {
                    var x = tempEndPoint.X - rectangle.X;
                    if (Width - x <= 1000)
                    {
                        Width = 1000;
                    }
                    else
                    {
                        Width = Width - x;
                        Location = new Point(Location.X + x, Location.Y);
                    }

                    if (tempEndPoint.Y <= 500)
                        Height = 500;
                    else
                        Height = tempEndPoint.Y;
                }
                else if (direction == MouseDirection.Northwest)
                {
                    var x = tempEndPoint.X - rectangle.X;
                    if (Width - x <= 1000)
                    {
                        Width = 1000;
                    }
                    else
                    {
                        Width = Width - x;
                        Location = new Point(Location.X + x, Location.Y);
                    }

                    var y = tempEndPoint.Y - rectangle.Y;
                    if (Height - y <= 500)
                    {
                        Height = 500;
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

        #region ChildForm

        private void openChildForm(Form frm)
        {
            //关闭原有的子窗体
            foreach (Control item in panelchild.Controls)
                if (item is Form)
                    ((Form) item).Hide();
            frm.TopLevel = false;
            frm.Parent = panelchild;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;
            frm.Show();
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

        private void buttonclose_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void Move_panel_side(Control btn)
        {
            panelside.Top = btn.Top;
            panelside.Height = btn.Height;
        }

        private void buttonabout_Click(object sender, EventArgs e)
        {
            Move_panel_side(buttonabout);
            if (!childFormAbout_on)
            {
                childForm_About = new childFormAbout();
                childFormAbout_on = true;
            }

            openChildForm(childForm_About);
        }

        private void buttonhome_Click(object sender, EventArgs e)
        {
            Move_panel_side(buttonhome);
            openChildForm(childForm_Home);
        }

        private void buttonsettings_Click(object sender, EventArgs e)
        {
            Move_panel_side(buttonsettings);
            if (!childFormSettings_on)
            {
                childForm_Settings = new childFormSettings();
                childFormSettings_on = true;
            }

            openChildForm(childForm_Settings);
        }

        private void buttonbuilder_Click(object sender, EventArgs e)
        {
            Move_panel_side(buttonbuilder);
            if (!childFormBuilder_on)
            {
                childForm_Builder = new childFormBuilder();
                childFormBuilder_on = true;
            }

            openChildForm(childForm_Builder);
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

        private void buttonmin_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private bool previoustheme;
        private double previousopacity = 97;

        private void updateUI_Tick(object sender, EventArgs e)
        {
            if (previoustheme != Settings.Default.darkTheme)
            {
                previoustheme = Settings.Default.darkTheme;
                SetTheme();
            }

            if (previousopacity != Settings.Default.opacity)
            {
                previousopacity = Settings.Default.opacity;

                Opacity = Settings.Default.opacity / 100;
            }
        }

        private void buttonside_Click(object sender, EventArgs e)
        {
            if (panelleft.Width == 0)
                panelleft.Width = 165;
            else
                panelleft.Width = 0;
        }
    }
}