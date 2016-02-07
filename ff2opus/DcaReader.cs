using System;
using System.Collections.Generic;
using System.IO;

namespace dca4net
{
    /// <summary>
    /// Class containing various functions for operating on DCA files. (Discord Audio)
    /// 
    /// DCA files are binary formats containing multiple packets for easy transfer over Discord's voice chat service.
    /// DCA packets are opus encoded bytes beginning with a size to read for each packet.
    /// </summary>
    public class DcaReader : IDisposable
    {
        private bool disposed;

        private string filename;

        private List<byte[]> Packets = new List<byte[]>();

        public List<byte[]> GetPackets
        {
            get
            {
                return Packets;
            }
        }

        public DcaReader(string file)
        {
            filename = file;
        }

        public void ReadAllBytes()
        {
            byte[] initialBuffer = File.ReadAllBytes(filename);
            using (BinaryReader br = new BinaryReader(new MemoryStream(initialBuffer)))
            {
                UInt16 bytesToRead = 0;
                while((bytesToRead = br.ReadUInt16()) > 0)
                {
                    byte[] packet = new byte[bytesToRead];
                    packet = br.ReadBytes(bytesToRead);
                    Packets.Add(packet);
                    br.BaseStream.Position = bytesToRead + 1;
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if(disposing)
            {
                Packets.Clear();
                Packets = null;
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
