using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Audio_Offset_Analyzer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            chart1.ChartAreas[0].AxisY.Minimum = Int16.MinValue;
            chart1.ChartAreas[0].AxisY.Maximum = Int16.MaxValue;

            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Maximum = 2352;

            int offset = (int)numericUpDown1.Value * 176400 + (int)numericUpDown4.Value * 2352;

            textBox3.Text = offset.ToString();
        }

        private void textBox1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            string source1 = ((string[])e.Data.GetData(DataFormats.FileDrop)).First();

            numericUpDown1.Value = 0;
            numericUpDown4.Value = 0;

            int second = (int)numericUpDown1.Value * 75 * 2352;

            textBox1.Text = source1;

            AudioSource1.Clear();
            DecodeAudio(source1, AudioSource1);

            
            numericUpDown1.Maximum = AudioSource1.Count / (75 * 2352);

            chart1.Series[0].Points.Clear();

            for (int i = 0; i < 2352; i++)
            {
                Int16 sample = (Int16)AudioSource1[second + i * 4];
                sample += (Int16)(AudioSource1[second + i * 4 + 1] << 8);

                chart1.Series[0].Points.Add(sample);
            }
        }

        List<byte> AudioSource1 = new List<byte>();
        List<byte> AudioSource2 = new List<byte>();

        private void DecodeAudio(string source, List<byte> AudioSource)
        {
            using (Process ffmpeg = new Process())
            {
                ffmpeg.StartInfo.FileName = "ffmpeg.exe";
                ffmpeg.StartInfo.Arguments = string.Format("-i \"{0}\" -bitexact -map_metadata -1 -f s16le pipe:1", source);
                ffmpeg.StartInfo.CreateNoWindow = true;
                ffmpeg.StartInfo.UseShellExecute = false;
                ffmpeg.StartInfo.RedirectStandardOutput = true;

                ffmpeg.Start();

                //using (BinaryWriter bw = new BinaryWriter(new FileStream(audio_file + ".wav", FileMode.Create)))
                //{
                using (BinaryReader bsr = new BinaryReader(ffmpeg.StandardOutput.BaseStream))
                {
                    //bsr.ReadBytes(44);

                    byte[] _temp;

                    do
                    {
                        _temp = bsr.ReadBytes(2352);
                        AudioSource.AddRange(_temp);
                    }
                    while (_temp.Length == 2352);
                }
                //}
                ffmpeg.WaitForExit();
            }
        }

        private void textBox2_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void textBox2_DragDrop(object sender, DragEventArgs e)
        {
            string source2 = ((string[])e.Data.GetData(DataFormats.FileDrop)).First();

            textBox2.Text = source2;

            int second = (int)numericUpDown1.Value * 75 * 2352;

            AudioSource2.Clear();
            DecodeAudio(source2, AudioSource2);

            chart1.Series[1].Points.Clear();

            for (int i = 0; i < 2352; i++)
            {
                Int16 sample = (Int16)AudioSource2[second + i * 4];
                sample += (Int16)(AudioSource2[second + i * 4 + 1] << 8);

                chart1.Series[1].Points.Add(sample);
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            int second = (int)numericUpDown1.Value * 75 * 2352;
            int offset = (int)numericUpDown4.Value * 2352;
            int offset1 = (int)numericUpDown2.Value * 4;
            int offset2 = (int)numericUpDown3.Value * 4;

            textBox3.Text = ((int)numericUpDown1.Value * 352800 + (int)numericUpDown4.Value * 2352).ToString();

            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();

            for (int i = 0; i < 2352; i++)
            {
                Int16 sample = (Int16)AudioSource1[offset1 + second + offset + i * 4];
                sample += (Int16)(AudioSource1[offset1 + second + offset + i * 4 + 1] << 8);

                chart1.Series[0].Points.Add(sample);

                sample = (Int16)AudioSource2[offset2 + second + offset + i * 4];
                sample += (Int16)(AudioSource2[offset2 + second + offset + i * 4 + 1] << 8);

                chart1.Series[1].Points.Add(sample);
            }
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            int second = (int)numericUpDown1.Value * 75 * 2352;
            int offset = (int)numericUpDown4.Value * 2352;
            int offset2 = (int)numericUpDown3.Value * 4;

            //chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();

            for (int i = 0; i < 2352; i++)
            {
                //Int16 sample = (Int16)AudioSource1[second + i * 2];
                //sample += (Int16)(AudioSource1[second + i * 2 + 1] << 8);
                //
                //chart1.Series[0].Points.Add(sample);

                Int16 sample = (Int16)AudioSource2[offset2 + second+ offset + i * 4];
                sample += (Int16)(AudioSource2[offset2 + second+ offset + i * 4 + 1] << 8);

                chart1.Series[1].Points.Add(sample);
            }
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            int second = (int)numericUpDown1.Value * 75 * 2352;
            int offset = (int)numericUpDown4.Value * 2352;
            int offset1 = (int)numericUpDown2.Value * 4;

            chart1.Series[0].Points.Clear();
            //chart1.Series[1].Points.Clear();

            for (int i = 0; i < 2352; i++)
            {
                Int16 sample = (Int16)AudioSource1[offset1 + second + offset + i * 4];
                sample += (Int16)(AudioSource1[offset1 + second + offset + i * 4 + 1] << 8);

                chart1.Series[0].Points.Add(sample);

                //Int16 sample = (Int16)AudioSource2[offset + second + i * 2];
                //sample += (Int16)(AudioSource2[offset + second + i * 2 + 1] << 8);
                //
                //chart1.Series[1].Points.Add(sample);
            }
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            int second = (int)numericUpDown1.Value * 75 * 2352;
            int offset = ((int)numericUpDown4.Value * 2352);
            int offset1 = (int)numericUpDown2.Value * 4;
            int offset2 = (int)numericUpDown3.Value * 4;

            textBox3.Text = ((int)numericUpDown1.Value * 176400 + (int)numericUpDown4.Value * 2352).ToString();

            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();

            for (int i = 0; i < 2352; i++)
            {
                Int16 sample = (Int16)AudioSource1[offset1 + second + offset + i * 4];
                sample += (Int16)(AudioSource1[offset1 + second + offset + i * 4 + 1] << 8);

                chart1.Series[0].Points.Add(sample);

                sample = (Int16)AudioSource2[offset2 + second + offset + i * 4];
                sample += (Int16)(AudioSource2[offset2 + second + offset + i * 4 + 1] << 8);

                chart1.Series[1].Points.Add(sample);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int samples = (int)numericUpDown2.Value;
            int offset = int.Parse(textBox3.Text);
            int bytes = Math.Abs(samples) * 4;



            if(samples < 0)
            {
                AudioSource1.RemoveRange(AudioSource1.Count - bytes, bytes);
                AudioSource1.InsertRange(offset, new byte[bytes]);
            }
            else
            {
                AudioSource1.RemoveRange(offset, bytes);
                AudioSource1.AddRange(new byte[bytes]);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string file = textBox1.Text + ".fixed.wav";

            byte[] wavheader = { 0x52, 0x49, 0x46, 0x46, 0xff, 0xff, 0xff, 0xff, 0x57, 0x41, 0x56, 0x45, 0x66, 0x6D, 0x74, 0x20, 0x10, 0x00, 0x00, 0x00, 0x01, 0x00, 0x02, 0x00, 0x44, 0xAC, 0x00, 0x00, 0x10, 0xB1, 0x02, 0x00, 0x04, 0x00, 0x10, 0x00, 0x64, 0x61, 0x74, 0x61, 0xff, 0xff, 0xff, 0xff };

            using (BinaryWriter bw = new BinaryWriter(new FileStream(file, FileMode.Create)))
            {
                bw.Write(wavheader);
                bw.Write(AudioSource1.ToArray());
            }
            
        }
    }
}
