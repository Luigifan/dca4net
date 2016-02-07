using System;
using dca4net;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace dcatest
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Console.WriteLine("dca4net test application");

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select MP3 file to encode.";
            ofd.Filter = "MP3 Files (*.mp3)|*.mp3";
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                Stopwatch t = new Stopwatch();
                DcaEncoder encoder = new DcaEncoder(ofd.FileName);
                t.Start();
                byte[] encodedBytes = encoder.Encode();
                t.Stop();
                if(encodedBytes != null) //encoded ok
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Title = "Save .dca File.";
                    sfd.Filter = "Discord Audio Files (*.dca)|*.dca";
                    if(sfd.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllBytes(sfd.FileName, encodedBytes);
                    }
                }
                Console.WriteLine("\ndone in " + t.Elapsed.Seconds + "s" + " (" + t.ElapsedTicks + " ticks)");
                encoder.Dispose();
            }

            
            Console.ReadLine();
        }
    }
}
