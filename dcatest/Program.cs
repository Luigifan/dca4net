using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using dca4net;
using System.Collections.Generic;

namespace dcatest
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Console.WriteLine("dca4net test application");

            Console.WriteLine("e for encode; d for decode");
            string input = Console.ReadLine();
            if (input.Trim().ToLower() == "e")
                Encode();
            else if (input.Trim().ToLower() == "d")
                Decode();

            Console.ReadLine();
        }

        static void Decode()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select DCA file to decode.";
            ofd.Filter = "DCA Files (*.dca)|*.dca";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Stopwatch t = new Stopwatch();
                DcaDecoder decoder = new DcaDecoder(ofd.FileName);
                t.Start();
                ReadData pcmData = new ReadData();
                pcmData.offset = 0;
                pcmData.data = new byte[1];
                List<byte[]> bytes = new List<byte[]>();
                while ((pcmData = decoder.Decode(pcmData.offset)).data != null)
                {
                    bytes.Add(pcmData.data);
                }

                byte[] FINAL;
                using (BinaryWriter bw = new BinaryWriter(new MemoryStream()))
                {
                    bytes.ForEach(x => bw.Write(x));
                    MemoryStream baseStream = bw.BaseStream as MemoryStream;
                    FINAL = baseStream.ToArray();
                }
                t.Stop();

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Wave Files (*.wav)|*.wav";
                if(sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(sfd.FileName, FINAL);
                }

                Console.WriteLine("\ndone in " + t.Elapsed.Seconds + "s" + " (" + t.ElapsedTicks + " ticks)");
                decoder.Dispose();
            }
        }

        static void Encode()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select MP3 file to encode.";
            ofd.Filter = "MP3 Files (*.mp3)|*.mp3";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Stopwatch t = new Stopwatch();
                DcaEncoder encoder = new DcaEncoder(ofd.FileName);
                t.Start();
                byte[] encodedBytes = encoder.Encode();
                t.Stop();
                if (encodedBytes != null) //encoded ok
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Title = "Save .dca File.";
                    sfd.Filter = "Discord Audio Files (*.dca)|*.dca";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllBytes(sfd.FileName, encodedBytes);
                    }
                }
                Console.WriteLine("\ndone in " + t.Elapsed.Seconds + "s" + " (" + t.ElapsedTicks + " ticks)");
                encoder.Dispose();
            }
        }
    }
}
