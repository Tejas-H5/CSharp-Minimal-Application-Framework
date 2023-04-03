﻿using NAudio.Wave;
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
    public class AudioData : IDisposable {
        private static int nextAudioDataID = 1;
        public int ID;
        public override int GetHashCode() {
            return ID;
        }


        short[] rawData;
        public short[] RawData {
            get {
                return rawData;
            }
        }

        private int sampleRate;
        private int channels;
        private int len;
        private ALFormat format;

        public ALFormat Format {
            get {
                return format;
            }
        }


        public int Length => len;

        public int SampleRate {
            get => sampleRate;
        }

        public int Channels {
            get => channels;
        }

        public double Duration {
            get => SampleToSeconds(Length);
        }

        public int DurationSamples {
            get => rawData.Length / Channels;
        }

        public double SampleToSeconds(int sample) {
            return sample / (double)SampleRate;
        }

        public int ToSample(double seconds) {
            return (int)(seconds * SampleRate);
        }

        public float GetSample(int sample, int channel) {
            return rawData[channel % Channels + sample * Channels];
        }

        public AudioData(short[] rawInterleavedData, int sampleRate, int numChannels) {
            ID = nextAudioDataID;
            nextAudioDataID++;

            this.sampleRate = sampleRate;
            channels = numChannels;
            rawData = rawInterleavedData;
            len = rawData.Length / numChannels;

            if (channels == 2) {
                format = ALFormat.Stereo16;
            } else {
                format = ALFormat.Stereo8;
            }
        }

        public static AudioData FromFile(string filepath, AudioDataImportSettings importSettings = null) {
            if (importSettings == null) {
                importSettings = new AudioDataImportSettings() {
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
        private static AudioData LoadAudioClip(string filepath, AudioDataImportSettings importSettings) {
            using (MediaFoundationReader media = new MediaFoundationReader(filepath)) {
                WaveFormat metadata = media.WaveFormat;
                int sampleRate = metadata.SampleRate;
                int channels = metadata.Channels;

                ISampleProvider isp = media.ToSampleProvider();

                int numSamples = (int)(media.TotalTime.TotalSeconds * sampleRate * channels);

                //TODO: consider using a less memory intensive storage method for audio
                float[] rawData = new float[numSamples];
                isp.Read(rawData, 0, rawData.Length);

                short[] rawData16bit = new short[rawData.Length];
                for (int i = 0; i < rawData.Length; i++) {
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

                Console.WriteLine("Opened [" + filepath + "]");
                Console.WriteLine("Sample rate: [" + sampleRate + "]");
                Console.WriteLine("Duration: [" + media.TotalTime.TotalSeconds + "]");

                return new AudioData(rawData16bit, sampleRate, channels);
            }
        }

        public void Dispose() {
        }
    }
}
