using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using Creeper.Properties;

namespace Creeper
{
    public partial class FormInit : Form
    {
        public FormInit()
        {
            InitializeComponent();
            SetTheme();
        }

        #region Theme

        public void SetTheme()
        {
            var darkTheme = Settings.Default.darkTheme;

            var colorSide = darkTheme ? Settings.Default.colorsidedark : Settings.Default.colorside;
            var colorText = darkTheme ? Settings.Default.colortextdark : Settings.Default.colortext;

            BackColor = colorSide;
            ForeColor = colorText;

            buttonclose.BackColor = colorSide;
            buttonclose.Image = darkTheme ? Resources.close : Resources.close_dark;

            labelCreeper.BackColor = colorSide;
            labelCreeper.ForeColor = colorText;

            paneltop.BackColor = colorSide;

            pictureBox.Image = darkTheme ? Resources.Creeper : Resources.Creeper_dark;

            //textBoxxor.BackColor = colorSide;
            //textBoxxor.ForeColor = colorText;

            //textBoxaddress.BackColor = colorSide;
            //textBoxaddress.ForeColor = colorText;

            //textBoxpassword.BackColor = colorSide;
            //textBoxpassword.ForeColor = colorText;
        }

        #endregion

        private void buttonclose_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void buttonconnect_Click(object sender, EventArgs e)
        {
            if (textBoxPort.Text == "")
            {
                MessageBox.Show("Please fill in your port");
            }
            else
            {
                try
                {
                    int port = int.Parse(textBoxPort.Text);
                    Settings.Default.Port = port.ToString();
                    Settings.Default.Save();

                    string FullPath = Path.Combine(Application.StartupPath, "Clients Folder");
                    if (!Directory.Exists(FullPath)) Directory.CreateDirectory(FullPath);
                    if (!File.Exists(Path.Combine(Application.StartupPath, "Clients Folder", "Note.xml")))
                    {
                        XElement myDoc = new XElement("Clients", new XElement("Client", new XAttribute("HWID", ""), new XElement("Note", "qwqdanchun"))); ;
                        myDoc.Save(Path.Combine(Application.StartupPath, "Clients Folder", "Note.xml"));
                    }
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
            }
        }

        private void FormInit_Load(object sender, EventArgs e)
        {
            textBoxPort.Text = Settings.Default.Port;
        }

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
    }
}