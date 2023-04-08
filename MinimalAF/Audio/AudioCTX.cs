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
        struct ALSourceAudioSourcePair {
            public OpenALSource AlSource;
            public AudioSource AudioSource;
        }

        private static ALDevice s_device;
        private static ALContext s_context;
        private static ALSourceAudioSourcePair[] s_allAvailableOpenALSources = new ALSourceAudioSourcePair[MAX_AUDIO_SOURCES];
        private static AudioListener s_currentListener;
        public static AudioListener CurrentListener => s_currentListener;
        public const int MAX_AUDIO_SOURCES = 256;

        public static void SetCurrentListener(AudioListener instance) {
            s_currentListener = instance;

            if (s_currentListener != null) {
                s_currentListener.Update();
            }
        }

        public static unsafe void Init() {
            // init OpenAL
            {
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
        }

        static int NextAvailableALSource() {
            for(int i = 0; i < s_allAvailableOpenALSources.Length; i++) {
                if (s_allAvailableOpenALSources[i].AudioSource == null) {
                    // lazy creation of al sources.
                    if (s_allAvailableOpenALSources[i].AlSource == null) {
                        s_allAvailableOpenALSources[i].AlSource = OpenALSource.CreateOpenALSource();
                    }

                    return i;
                }
            }

            return -1;
        }

        internal static int GetIndexForIAudioSourceInput(IAudioSourceInput input) {
            for(int i = 0; i < s_allAvailableOpenALSources.Length; i++) {
                if (s_allAvailableOpenALSources[i].AudioSource == null) continue;

                if (s_allAvailableOpenALSources[i].AudioSource.CurrentInput == input) {
                    return i;
                }
            }

            return -1;
        }


        /// This method finds an OpenAL audio source we can use for our virtual audio source.
        internal static OpenALSource GetALSourceForAudioSource(AudioSource audioSource) {
            // If the audio source is already playing, we would want it to get
            // the same OpenAL audio source again.
            if (audioSource.Index != -1) {
                return s_allAvailableOpenALSources[audioSource.Index].AlSource;
            }

            // The audio source isn't playing, we can just get the next one, if it is available.
            int nextAvailable = NextAvailableALSource();
            if (nextAvailable != -1) {
                s_allAvailableOpenALSources[nextAvailable].AudioSource = audioSource;
                audioSource.Index = nextAvailable;
                return s_allAvailableOpenALSources[nextAvailable].AlSource;
            }


            // This code is highly suspect. If we have a low priority streamed thing,
            // and we pull out the audio source from under it, then a lot of things will break
            //// But what if there isn't another audio source available? 
            //// We want to find the lowest priority source, and if it is lower priority than
            //// audioSource, we will re-assign it's OpenAL audio source to audioSource.
            //{
            //    AudioSource lowestPrioritySource = null;
            //    int lowestPriorityIndex = 0;
            //    for(int i = 0; i < s_allAvailableOpenALSources.Length; i++) {
            //        var virtualAudioSource = s_allAvailableOpenALSources[i].AudioSource;

            //        if (lowestPrioritySource == null || 
            //            virtualAudioSource.Priority < lowestPrioritySource.Priority) {
            //            lowestPrioritySource = virtualAudioSource;
            //            lowestPriorityIndex = i;
            //        }
            //    }

            //    if (lowestPrioritySource != null && lowestPrioritySource.Priority < audioSource.Priority) {
            //        // kick the old audio source, and put the new one in it's place
            //        lowestPrioritySource.Index = -1;
            //        s_allAvailableOpenALSources[lowestPriorityIndex].AudioSource = audioSource;
            //        audioSource.Index = lowestPriorityIndex;
            //    }
            //}

            return null;
        }


        public static void Update() {
            // Update listener
            {
                if (s_currentListener != null) {
                    s_currentListener.Update();
                }
            }

            // update sources
            {
                for(int i = 0; i < s_allAvailableOpenALSources.Length; i++) {
                    var audioSource = s_allAvailableOpenALSources[i].AudioSource;
                    if (audioSource == null) continue;

                    audioSource.Update();

                    var alSource = s_allAvailableOpenALSources[i].AlSource;
                    alSource.PullDataFrom(audioSource);

                    var alSourceState = alSource.GetSourceState();
                    if (
                        alSourceState != ALSourceState.Playing && 
                        alSourceState != ALSourceState.Paused
                    ) {
                        audioSource.Index = -1;
                        s_allAvailableOpenALSources[i].AudioSource = null;
                    }
                }
            }
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


            // cleanup the audio pool
            {
                for(int i = 0; i < s_allAvailableOpenALSources.Length; i++) {
                    var audioSource = s_allAvailableOpenALSources[i].AudioSource;
                    if (audioSource != null) {
                        audioSource.Index = -1;
                        s_allAvailableOpenALSources[i].AudioSource = null;
                    }

                    var alSource = s_allAvailableOpenALSources[i].AlSource;
                    if (alSource != null) {
                        alSource.Dispose();
                    }
                }
            }

            SetCurrentListener(null);
        }

        internal static string ALCheckErrors() {
            ALError error = AL.GetError();

            if (error != ALError.NoError) {
                

                switch (error) {
                    case ALError.InvalidName:
                        return "AL_INVALID_NAME: a bad name (ID) was passed to an OpenAL function";
                    case ALError.InvalidEnum:
                        return "AL_INVALID_ENUM: an invalid enum value was passed to an OpenAL function";
                    case ALError.InvalidValue:
                        return "AL_INVALID_VALUE: an invalid value was passed to an OpenAL function";
                    case ALError.InvalidOperation:
                        return "AL_INVALID_OPERATION: the requested operation is not valid";
                    case ALError.OutOfMemory:
                        return "AL_OUT_OF_MEMORY: the requested operation resulted in OpenAL running out of memory";
                    default:
                        return "UNKNOWN AL ERROR: " + error.ToString();
                }
            }

            return "";
        }

        internal static bool ALCall(Action method) {
            string err = ALCheckErrors();
            if (err != "") {
#if RELEASE
                Console.WriteLine("Some other AL call has an un-thrown error");
#else
                throw new Exception("Some other AL call has an un-thrown error");
#endif
            }

            method();

            err = ALCheckErrors();
            if (err != "") {
#if RELEASE
                Console.WriteLine("ERROR: " + err);
#else
                throw new Exception("ERROR: " + err);
#endif
            }

            return err == "";
        }

        internal static bool ALCall<T>(out T res, Func<T> method) {
            string err = ALCheckErrors();
            if (err != "") {
#if RELEASE
                Console.WriteLine("Some other AL call has an un-thrown error");
#else
                throw new Exception("Some other AL call has an un-thrown error");
#endif
            }
            res = method();

            err = ALCheckErrors();
            if (err != "") {
#if RELEASE
                Console.WriteLine("ERROR: " + err);
#else
                throw new Exception("ERROR: " + err);
#endif
            }

            return err == "";
        }

        private static IEnumerable<string> ALGetDevices() {
            if (!ALCall(out IEnumerable<string> devices, () => { 
                return ALC.GetStringList(GetEnumerationStringList.CaptureDeviceSpecifier); 
            })) {
                return null;
            }

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