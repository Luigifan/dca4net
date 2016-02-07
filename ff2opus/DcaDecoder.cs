using Discord.Audio.Opus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dca4net
{
    public class DcaDecoder : IDisposable
    {
        private bool disposed = false;
        private OpusDecoder decoder;
        private string __filename;
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

        public DcaDecoder()
        {
            decoder = new OpusDecoder(48000, 2, 20);
        }
        public DcaDecoder(string file)
        {
            __filename = file;
            decoder = new OpusDecoder(48000, 2, 20);
        }

        /// <summary>
        /// Decodes a dca file to PCM data
        /// </summary>
        /// <returns>byte array with PCM data</returns>
        public byte[] Decode(int offset)
        {
            int channels = 2;
            int ms = 20;

            int blockSize = 48 * 2 * channels * ms;

            using(FileStream fs = new FileStream(__filename, FileMode.Open))
            using (BinaryReader br = new BinaryReader(fs))
            {
                ushort bytesToRead = 0;
                br.BaseStream.Position = offset;
                
                while ((bytesToRead = br.ReadUInt16()) > 0)
                {
                    byte[] buffer = new byte[bytesToRead];
                    buffer = br.ReadBytes(bytesToRead);
                    fs.Position = bytesToRead + 2;
                    byte[] pcmBuffer = new byte[blockSize];
                    int bytesDecoded = decoder.DecodeFrame(buffer, 0, bytesToRead, pcmBuffer);
                    return pcmBuffer;
                }
            }
            return null;
        }
        
        private byte[] makeGiantByteArray(List<byte[]> buffers)
        {
            long size = 0;
            buffers.ForEach(x => size += x.Length);
            byte[] giantBuffer = new byte[size];

            long offset = 0;
            foreach(var byteBuffer in buffers)
            {
                Buffer.BlockCopy(byteBuffer, 0, giantBuffer, (int)offset, byteBuffer.Length);
                offset += byteBuffer.Length;
            }

            return giantBuffer;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                __filename = null;
                decoder.Dispose();
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
