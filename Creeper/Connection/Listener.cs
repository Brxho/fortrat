using System;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;

namespace Creeper.Connection
{
    class Listener
    {
        private Socket Server { get; set; }

        public void Connect(object port)
        {
            try
            {
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, Convert.ToInt32(port));
                Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    SendBufferSize = 50 * 1024,
                    ReceiveBufferSize = 50 * 1024,
                };
                Server.Bind(ipEndPoint);
                Server.Listen(30);
                FormMain.childForm_Home.Invoke((MethodInvoker)(() =>
                {
                    FormMain.childForm_Home.AppandLog($"Listenning {port}");
                }));
                Server.BeginAccept(EndAccept, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(0);
            }
        }

        private void EndAccept(IAsyncResult ar)
        {
            try
            {
                new Clients(Server.EndAccept(ar));
            }
            finally
            {
                Server.BeginAccept(EndAccept, null);
            }
        }
    }
}
