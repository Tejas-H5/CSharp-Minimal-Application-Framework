using System;
using System.Collections.Generic;
using Silk.NET.OpenAL;
using Silk.NET.OpenAL.Extensions;

namespace MinimalAF.Audio
{
	/// <summary>
	/// Class that enables audio.
	/// The following only applies if you are using this code outside of Minimal Application Framework:
	/// 
	/// Needs to be Initialized with Init(),
	/// Updated every frame with Update(),
	/// and disposed of when the program is done with Cleanup().
	/// </summary>
	public static unsafe class AudioCTX
    {
        private static Device* _device;
        private static Context* _context;

		public static AL AL => _al;
		private static AL _al;


		public static unsafe void Init()
        {
            InitializeOpenAL();
            ALAudioSourcePool.Init();
        }


        public static void Update()
        {
            ALAudioListener.Update();
            ALAudioSourcePool.Update();
        }

        public static void Cleanup()
        {
			var alc = ALContext.GetApi();

			if (_context != null)
            {
				alc.MakeContextCurrent(null);
                alc.DestroyContext(_context);
            }
            _context = null;

            if (_device != null)
            {
                alc.CloseDevice(_device);
            }

            _device = null;

            AudioMap.UnloadAllCachedAudio();

            ALAudioSourcePool.Cleanup();
            ALAudioListener.Cleanup();
        }

        private static unsafe void InitializeOpenAL()
        {
			var alc = ALContext.GetApi();
			_al = AL.GetApi();

            _device = alc.OpenDevice(null);

			if (_device == null)
			{
				Console.WriteLine("Could not create audio device");
				return;
			}

			_context = alc.CreateContext(_device, null);

			if(_context == null)
			{
				Console.WriteLine("Could not create audio context");
				return;
			}

			alc.MakeContextCurrent(_context);

			alc.GetError(_device);


            var version = _al.GetStateProperty(StateString.Version);
            var vendor = _al.GetStateProperty(StateString.Vendor);
			var renderer = _al.GetStateProperty(StateString.Renderer);

			if (version == null || vendor == null || renderer == null)
            {
                Console.Write("OpenAL Version, vendor or renderer were null. Audio Engine was unable to initialize.");
            }
            else
            {
                Console.WriteLine("Audio from Silk.Net/OpenAL v" + version + " from" + vendor + " rendered with " + renderer);
            }
        }


        internal static bool ALCheckErrors()
        {
            AudioError error = AudioCTX.AL.GetError();

            if (error != AudioError.NoError)
            {
                string message = "***ERROR***\n";

                switch (error)
                {
                    case AudioError.InvalidName:
                        message += "AL_INVALID_NAME: a bad name (ID) was passed to an OpenAL function";
                        break;
                    case AudioError.InvalidEnum:
                        message += "AL_INVALID_ENUM: an invalid enum value was passed to an OpenAL function";
                        break;
                    case AudioError.InvalidValue:
                        message += "AL_INVALID_VALUE: an invalid value was passed to an OpenAL function";
                        break;
                    case AudioError.InvalidOperation:
                        message += "AL_INVALID_OPERATION: the requested operation is not valid";
                        break;
                    case AudioError.OutOfMemory:
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

    }
}
