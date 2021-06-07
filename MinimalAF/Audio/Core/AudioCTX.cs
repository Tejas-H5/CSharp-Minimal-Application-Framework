using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Audio.OpenAL;

namespace MinimalAF.Audio.Core
{
    public static class AudioCTX
    {
        public static AudioListener CurrentSelectedInstance {
            get {
                return _currentSelectedListener;
            }

            set {
                SetCurrentListener(value);
            }
        }

        private static AudioListener _currentSelectedListener;


        internal static bool ALCheckErrors()
        {
            ALError error = AL.GetError();

            if (error != ALError.NoError)
            {
                string message = "***ERROR***\n";

                switch (error)
                {
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


        internal static bool ALCall(Action method)
        {
            method();
            return ALCheckErrors();
        }

        internal static bool ALCall<T>(out T res, Func<T> method)
        {
            res = method();
            return ALCheckErrors();
        }

        private static IEnumerable<string> ALGetDevices()
        {
            IEnumerable<string> devices = null;
            if (!ALCall(out devices,
                () => { return ALC.GetStringList(GetEnumerationStringList.CaptureDeviceSpecifier); }))
                return null;

            return devices;
        }

        public static IEnumerable<string> GetAllDevices()
        {
            return ALGetDevices();
        }

        public static void SetDeviceListener(string name)
        {
            ALC.OpenDevice(name);
        }


        private static ALDevice _device;
        private static ALContext _context;

        public static unsafe void Init()
        {
            _device = ALC.OpenDevice(null);

            if (!ALCall(out _context, () => { return ALC.CreateContext(_device, (int*)null); }))
            {
                Console.WriteLine("Error: Could not create audio context");
                return;
            }

            bool contextMadeCurrent = false;
            if (!ALCall(out contextMadeCurrent, () => { return ALC.MakeContextCurrent(_context); }) || !contextMadeCurrent)
            {
                Console.WriteLine("Error: Could not make audio context current");
                return;
            }

            ALC.MakeContextCurrent(_context);

            var version = AL.Get(ALGetString.Version);
            var vendor = AL.Get(ALGetString.Vendor);
            var renderer = AL.Get(ALGetString.Renderer);

            if (version == null || vendor == null || renderer == null)
            {
                Console.Write("OpenAL Version, vendor or renderer were null. Audio Engine was unable to initialize.");
            }
            else
            {
                Console.WriteLine("Audio powered by OpenAL v" + version + " from" + vendor + " rendered with " + renderer);
            }
        }

        public static void SetCurrentListener(AudioListener instance)
        {
            _currentSelectedListener = instance;
            UpdateListener();
        }

        private static void UpdateListener()
        {
            if (_currentSelectedListener == null)
                return;

            AL.Listener(ALListenerf.Gain, _currentSelectedListener.Gain);
            AL.Listener(ALListenerf.EfxMetersPerUnit, _currentSelectedListener.EfxMetersPerUnit);
            AL.Listener(ALListener3f.Position, _currentSelectedListener.Position.X, _currentSelectedListener.Position.Y, _currentSelectedListener.Position.Z);
            AL.Listener(ALListener3f.Velocity, _currentSelectedListener.Velocity.X, _currentSelectedListener.Velocity.Y, _currentSelectedListener.Velocity.Z);

            var at = _currentSelectedListener.OrientationLookAt;
            var up = _currentSelectedListener.OrientationUp;
            AL.Listener(ALListenerfv.Orientation, ref at, ref up);
        }

        public static void Cleanup()
        {
            if (_context != ALContext.Null)
            {
                ALC.MakeContextCurrent(ALContext.Null);
                ALC.DestroyContext(_context);
            }
            _context = ALContext.Null;

            if (_device != ALDevice.Null)
            {
                ALC.CloseDevice(_device);
            }

            _device = ALDevice.Null;

            CurrentSelectedInstance = null;
        }

        public static void UpdateListenerGain(AudioListener instance)
        {
            if (instance != CurrentSelectedInstance)
                return;

            AL.Listener(ALListenerf.Gain, instance.Gain);
        }

        public static void UpdateListenerEfxMetersPerUnit(AudioListener instance)
        {
            if (instance != CurrentSelectedInstance)
                return;

            AL.Listener(ALListenerf.EfxMetersPerUnit, instance.EfxMetersPerUnit);
        }

        public static void UpdateListenerOrientation(AudioListener instance)
        {
            if (instance != CurrentSelectedInstance)
                return;


            var at = instance.OrientationLookAt;
            var up = instance.OrientationUp;
            AL.Listener(ALListenerfv.Orientation, ref at, ref up);
        }

        public static void UpdateListenerPosition(AudioListener instance)
        {
            if (instance != CurrentSelectedInstance)
                return;

            AL.Listener(ALListener3f.Position, instance.Position.X, instance.Position.Y, -instance.Position.Z);
        }

        public static void UpdateListenerVelocity(AudioListener instance)
        {
            if (instance != CurrentSelectedInstance)
                return;

            AL.Listener(ALListener3f.Velocity, instance.Velocity.X, instance.Velocity.Y, -instance.Velocity.Z);
        }
    }
}
