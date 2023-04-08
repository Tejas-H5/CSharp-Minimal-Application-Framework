using NAudio.Wave;
using OpenTK.Audio.OpenAL;
using System;

namespace MinimalAF.Audio {
    public class AudioDataImportSettings {
        public bool ForceMono { get; set; } = false;
    }

    /// <summary>
    /// Represents audio as an array of shorts.
    /// 
    /// Overrides GetHashCode to be a constant ID, so this can be used as a HashMap/Dictionary key
    /// </summary>
    public class AudioRawData {
        short[] _samples;
        private int _sampleRate;
        private int _channels;
        private ALFormat _format;
        
        /// <summary>
        /// Samples from each channel are interleaved
        /// </summary>
        public short[] Samples => _samples;
        public ALFormat Format => _format;
        public long Length => _samples.LongLength;
        public int SampleRate => _sampleRate;
        public int Channels => _channels;
        public double Duration => SampleToSeconds(Length);

        public double SampleToSeconds(long sample) {
            return (sample / _channels) / (double)SampleRate;
        }

        public long ToSample(double seconds) {
            return ((long)(seconds * SampleRate)) * _channels;
        }

        public AudioRawData(short[] rawInterleavedData, int sampleRate, int numChannels) {
            this._sampleRate = sampleRate;
            _channels = numChannels;
            _samples = rawInterleavedData;
            
            // want to keep it simple for now, so yeah
            if (_channels == 2) {
                _format = ALFormat.Stereo16;
            } else {
                _format = ALFormat.Stereo8;
            }
        }

        public static AudioRawData FromFile(string filepath, AudioDataImportSettings importSettings = null) {
            if (importSettings == null) {
                importSettings = new AudioDataImportSettings {
                    ForceMono = false
                };
            }

            try {
                return LoadAudioClip(filepath, importSettings);
            } catch (Exception e) {
                Console.WriteLine("Error:\n " + e);
                return null;
            }
        }

        //Code partially taken from https://stackoverflow.com/questions/42483778/how-to-get-float-array-of-samples-from-audio-file
        private static AudioRawData LoadAudioClip(string filepath, AudioDataImportSettings importSettings) {
            using MediaFoundationReader media = new MediaFoundationReader(filepath);
            WaveFormat metadata = media.WaveFormat;
            int sampleRate = metadata.SampleRate;
            int channels = metadata.Channels;

            ISampleProvider isp = media.ToSampleProvider();

            // Should use ceiling ?
            long numSamples = (long)(media.TotalTime.TotalSeconds * sampleRate * channels);

            float[] rawData = new float[numSamples];
            isp.Read(rawData, 0, rawData.Length);

            short[] rawData16bit = new short[rawData.LongLength];
            for (long i = 0; i < rawData.LongLength; i++) {
                rawData16bit[i] = (short)(rawData[i] * short.MaxValue);
            }

            if (importSettings.ForceMono) {
                if (channels > 1) {
                    short[] rawData16BitMono = new short[rawData16bit.Length / channels];

                    for (int i = 0; i < rawData16BitMono.Length; i++) {
                        short data = 0;
                        for (int j = 0; j < channels; j++) {
                            data += (short)(rawData16bit[i * channels + j] / channels);
                        }

                        rawData16BitMono[i] = data;
                    }

                    rawData16bit = rawData16BitMono;
                    channels = 1;
                }
            }

            return new AudioRawData(rawData16bit, sampleRate, channels);
        }
    }
}
