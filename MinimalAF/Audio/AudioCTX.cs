using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;

namespace MinimalAF.Audio {
    /// <summary>
    /// Class that enables audio.
    /// The following only applies if you are using this code outside of Minimal Application Framework:
    /// 
    /// Needs to be Initialized with Init(),
    /// Updated every frame with Update(),
    /// and disposed of when the program is done with Cleanup().
    /// </summary>
    public static class AudioCTX {
        private static ALDevice s_device;
        private static ALContext s_context;


        public static unsafe void Init() {
            InitializeOpenAL();
            ALAudioSourcePool.Init();
        }


        public static void Update() {
            ALAudioListener.Update();
            ALAudioSourcePool.Update();
        }

        public static void Cleanup() {
            if (s_context != ALContext.Null) {
                ALC.MakeContextCurrent(ALContext.Null);
                ALC.DestroyContext(s_context);
            }
            s_context = ALContext.Null;

            if (s_device != ALDevice.Null) {
                ALC.CloseDevice(s_device);
            }

            s_device = ALDevice.Null;

            AudioMap.UnloadAll();

            ALAudioSourcePool.Cleanup();
            ALAudioListener.Cleanup();
        }

        private static unsafe void InitializeOpenAL() {
            s_device = ALC.OpenDevice(null);

            if (!ALCall(out s_context, () => { return ALC.CreateContext(s_device, (int*)null); })) {
                Console.WriteLine("Error: Could not create audio context");
                return;
            }

            bool contextMadeCurrent = false;
            if (!ALCall(out contextMadeCurrent, () => { return ALC.MakeContextCurrent(s_context); }) || !contextMadeCurrent) {
                Console.WriteLine("Error: Could not make audio context current");
                return;
            }

            ALC.MakeContextCurrent(s_context);

            var version = AL.Get(ALGetString.Version);
            var vendor = AL.Get(ALGetString.Vendor);
            var renderer = AL.Get(ALGetString.Renderer);

            if (version == null || vendor == null || renderer == null) {
                Console.Write("OpenAL Version, vendor or renderer were null. Audio Engine was unable to initialize.");
            } else {
                Console.WriteLine("Audio powered by OpenAL v" + version + " from" + vendor + ". Audio rendered with " + renderer);
            }
        }


        internal static bool ALCheckErrors() {
            ALError error = AL.GetError();

            if (error != ALError.NoError) {
                string message = "***ERROR***\n";

                switch (error) {
                    case ALError.InvalidName:
                        message += "AL_INVALID_NAME: a bad name (ID) was passed to an OpenAL function";
                        break;
                    case ALError.InvalidEnum:
                        message += "AL_INVALID_ENUM: an invalid enum value was passed to an OpenAL function";
                        break;
                    case ALError.InvalidValue:
                        message += "AL_INVALID_VALUE: an invalid value was passed to an OpenAL function";
                        break;
                    case ALError.InvalidOperation:
                        message += "AL_INVALID_OPERATION: the requested operation is not valid";
                        break;
                    case ALError.OutOfMemory:
                        message += "AL_OUT_OF_MEMORY: the requested operation resulted in OpenAL running out of memory";
                        break;
                    default:
                        message += "UNKNOWN AL ERROR: " + error.ToString();
                        break;
                }

                Console.WriteLine(message);
                return false;
            }
            return true;
        }

        internal static bool ALCall(Action method) {
            method();
            return ALCheckErrors();
        }

        internal static bool ALCall<T>(out T res, Func<T> method) {
            res = method();
            return ALCheckErrors();
        }

        private static IEnumerable<string> ALGetDevices() {
            IEnumerable<string> devices = null;
            if (!ALCall(out devices,
                () => { return ALC.GetStringList(GetEnumerationStringList.CaptureDeviceSpecifier); }))
                return null;

            return devices;
        }

        public static IEnumerable<string> GetAllDevices() {
            return ALGetDevices();
        }

        public static void SetDeviceListener(string name) {
            ALC.OpenDevice(name);
        }

    }
}