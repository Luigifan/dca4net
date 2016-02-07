using Discord.Audio.Opus;
using NAudio.Wave;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace dca4net
{
    public class DcaEncoder : IDisposable
    {
        private string __filename;
        private bool disposed = false;
        private OpusEncoder encoder;

        public string Filename
        {
            get
            {
                return __filename;
            }
            set
            {
                __filename = value;
            }
        }

        public DcaEncoder()
        {
            encoder = new OpusEncoder(48000, 2, 20, null, OpusApplication.MusicOrMixed);
        }

        public DcaEncoder(string filename)
        {
            __filename = filename;
            encoder = new OpusEncoder(48000, 2, 20, null, OpusApplication.MusicOrMixed);
        }

        public byte[] Encode()
        {
            int ms = 20;
            int channels = 2;
            int sampleRate = 48000;

            int blockSize = 48 * 2 * channels * ms; //the size per each frame to encode
            byte[] buffer = new byte[blockSize]; //a nicely sized pcm buffer to work with.
            var outFormat = new WaveFormat(sampleRate, 16, channels);
            
            if(__filename.EndsWith(".mp3"))
            {
                using (var mp3Reader = new Mp3FileReader(__filename))
                {
                    using (var resampler = new WaveFormatConversionStream(outFormat, mp3Reader))
                    {
                        int byteCount;
                        using (BinaryWriter bw = new BinaryWriter(new MemoryStream()))
                        {
                            while ((byteCount = resampler.Read(buffer, 0, blockSize)) > 0)
                            {
                                //now to encode
                                byte[] opusOutput = new byte[buffer.Length]; //extra bytes but that's okay
                                int opusEncoded = encoder.EncodeFrame(buffer, 0, opusOutput);
                                bw.Write((ushort)opusEncoded);
                                bw.Write(opusOutput, 0, opusEncoded);
                            }
                            MemoryStream baseStream = bw.BaseStream as MemoryStream;
                            return baseStream.ToArray();
                        }
                    }
                }
            }
            return null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if(disposing)
            {
                __filename = null;
                encoder.Dispose();
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
