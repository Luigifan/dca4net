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
            if (!File.Exists("dca.exe"))
                throw new FileNotFoundException("dca.exe not found.");
        }

        public DcaEncoder(string filename)
        {
            if (!File.Exists("dca.exe"))
                throw new FileNotFoundException("dca.exe not found.");

            __filename = filename;
        }

        public byte[] Encode()
        {
            Process p = new Process();
            p.StartInfo.ErrorDialog = true;

            p.StartInfo.FileName = "dca.exe";
            p.StartInfo.Arguments = $" \"{__filename}\"";
            //p.StartInfo.UseShellExecute = false;
           
            p.Start();
            p.WaitForExit();


            return null;
            //Process p = new Process();
            //p.StartInfo.FileName = "dca.exe";
            //p.StartInfo.Arguments = $" \"{__filename}\"";
            //p.StartInfo.RedirectStandardError = true;
            //p.StartInfo.CreateNoWindow = true;
            //p.StartInfo.RedirectStandardOutput = true;
            //p.StartInfo.UseShellExecute = false;
            //p.Start();
            
            //FileStream baseStream = p.StandardOutput.BaseStream as FileStream;
            //byte[] returnedBytes = null;
            //int lastRead = 0;

            //using (MemoryStream ms = new MemoryStream())
            //{
            //    byte[] buffer = new byte[4096];
            //    do
            //    {
            //        lastRead = baseStream.Read(buffer, 0, buffer.Length);
            //        ms.Write(buffer, 0, lastRead);
            //    } while (lastRead > 0);

            //    returnedBytes = ms.ToArray();
            //}

            //return returnedBytes;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if(disposing)
            {
                __filename = null;
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
