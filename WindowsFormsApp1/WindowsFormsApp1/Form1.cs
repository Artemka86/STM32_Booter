using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Collections;
using System.Threading;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        delegate void labelTextDelegate(string text);

        static int ID_num=0;
        int[] ID_base = new int[3000];

        static byte rx_ok=0;
        static byte port_open=0, can_open=0;



        public Form1()
        {
            InitializeComponent();
            string[] ports = SerialPort.GetPortNames();

            //comboBox2.SelectedIndex = 0;
     
            for(int i = 0; i < ports.Length; i++)
            {
                comboBox1.Items.Add(ports[i].ToString());

            }
            comboBox1.SelectedIndex = 0;

        }


        char inthex(int a)
        {

            switch (a)
            {
                case 0:
                    return '0';
                    break;
                case 1:
                    return '1';
                    break;
                case 2:
                    return '2';
                    break;
                case 3:
                    return '3';
                    break;
                case 4:
                    return '4';
                    break;
                case 5:
                    return '5';
                    break;
                case 6:
                    return '6';
                    break;
                case 7:
                    return '7';
                    break;
                case 8:
                    return '8';
                    break;
                case 9:
                    return '9';
                    break;
                case 0x0A:
                    return 'A';
                    break;
                case 0x0b:
                    return 'B';
                    break;
                case 0x0C:
                    return 'C';
                    break;
                case 0x0D:
                    return 'D';
                    break;
                case 0x0E:
                    return 'E';
                    break;
                case 0x0F:
                    return 'F';
                    break;


            }
            return '0';

        }



        int hexint(char input_symbol)
        {
            switch (input_symbol)
            {
                case '0':
                    return 0;
                    break;
                case '1':
                    return 1;
                    break;
                case '2':
                    return 2;
                    break;
                case '3':
                    return 3;
                    break;
                case '4':
                    return 4;
                    break;
                case '5':
                    return 5;
                    break;
                case '6':
                    return 6;
                    break;
                case '7':
                    return 7;
                    break;
                case '8':
                    return 8;
                    break;
                case '9':
                    return 9;
                    break;
                case 'A':
                    return 10;
                    break;
                case 'B':
                    return 11;
                    break;
                case 'C':
                    return 12;
                    break;
                case 'D':
                    return 13;
                    break;
                case 'E':
                    return 14;
                    break;
                case 'F':
                    return 15;
                    break;
            }

            return 0;
        }



        
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            

          // LabelText(serialPort1.ReadExisting());
           // if (serialPort1.ReadExisting()[0] == 'Y') rx_ok = 1;
        }


        private void LabelText(string text)
        {
            string show_string="t12381122334455667788";
            char[] a=new char[40];
            char[] ID_str = new char[8];

            int stdID,extID,ID_position,set_ok=0;


            const string fileName = "from.bin";
            int data_byte_int_1, data_byte_int_2, data_byte_int_3, data_byte_int_4, adr;

            UInt16 coding;



            if (InvokeRequired)
            {
                BeginInvoke(new labelTextDelegate(LabelText), new object[] { text });
                return;
            }
            else
            {


                richTextBox1.AppendText(text);


                if (text.Length == 0) return;


                if (text[0] == 'Y') rx_ok = 1;

                    if (text[0] == 'Q')
                {
                    data_byte_int_1 = (hexint(text[1])) * 0x10 + (hexint(text[2]));
                    data_byte_int_2 = (hexint(text[3])) * 0x10 + (hexint(text[4]));
                    data_byte_int_3 = (hexint(text[5])) * 0x10 + (hexint(text[6]));
                    data_byte_int_4 = (hexint(text[7])) * 0x10 + (hexint(text[8]));
                    adr = (hexint(text[10])) * 0x100 + (hexint(text[11]) * 0x10 + hexint(text[12]));


                    if (File.Exists(saveFileDialog1.FileName) == false)
                    {

                        FileStream new_file = File.Create(saveFileDialog1.FileName);
                        new_file.Close();
                        //FileMode.

                    }


                    using (BinaryWriter writer = new BinaryWriter(File.Open(saveFileDialog1.FileName, FileMode.Open)))
                    {


                       progressBar1.Value = adr;

                        writer.Seek(adr, SeekOrigin.Begin);
                        writer.Write(data_byte_int_1);
                        adr++;
                        writer.Seek(adr, SeekOrigin.Begin);
                        writer.Write(data_byte_int_2);
                        adr++;
                        writer.Seek(adr, SeekOrigin.Begin);
                        writer.Write(data_byte_int_3);
                        adr++;
                        writer.Seek(adr, SeekOrigin.Begin);
                        writer.Write(data_byte_int_4);

                       
                        if (adr == 0x7FF) progressBar1.Value = 0;

                    }




                }

            }



            richTextBox1.ScrollToCaret();

        }



        private void Form1_Load(object sender, EventArgs e)
        {

        }

        


        




        

       

        private void button5_Click(object sender, EventArgs e)
        {

           if (serialPort1.IsOpen==false)
           {
                serialPort1.PortName = comboBox1.SelectedItem.ToString();
                serialPort1.DtrEnable = true;
                serialPort1.Open();
               

                serialPort1.WriteLine("V\r\n");
                string rx_version = serialPort1.ReadExisting();
                if (rx_version.Length < 1) return;
              richTextBox1.AppendText(rx_version);
               
                button5.Text = "Disconnect";
                button3.Enabled = true;
                button6.Enabled = true;
                port_open = 1;
                return ;
           }

           if (serialPort1.IsOpen==true)
           {



            serialPort1.Close();
               button5.Text = "Connect";
               button3.Enabled = false;
               button6.Enabled = false;
               port_open = 0;


           }





        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

       


        



        private void button3_Click_1(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //progressBar1.Maximum = 0x7ff;
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                progressBar1.Maximum = 0x7ff;

     
                
                


            }
        }
       
        private void button2_Click(object sender, EventArgs e)
        {





        }

        private void button4_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("t1\n");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // const string fileName = "1.bin";
            int adrL, adrH, adr, i;

            byte[] data_packet = new byte[9];
            byte[] dump = new byte[0x7FF];

            char[] string_send = new char[23];

            //System.IO.StreamReader(openFileDialog1.FileName);

            adr = 0;

            


            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                using (BinaryReader reader = new BinaryReader(File.Open(openFileDialog1.FileName, FileMode.Open)))
                {



                    string_send[0] = 'E';
                  

                    progressBar1.Maximum = 0x800;

                    for (adr = 0; adr < 0x800; adr = adr + 8)
                    {
                        progressBar1.Value = adr;
                        if (adr == 0x7F8) progressBar1.Value = 0;

                        data_packet[0] = reader.ReadByte();

                        string_send[2] = inthex(data_packet[0] / 0x10);
                        string_send[3] = inthex(data_packet[0] % 0x10);

                        data_packet[1] = reader.ReadByte();

                        string_send[4] = inthex(data_packet[1] / 0x10);
                        string_send[5] = inthex(data_packet[1] % 0x10);

                        data_packet[2] = reader.ReadByte();

                        string_send[6] = inthex(data_packet[2] / 0x10);
                        string_send[7] = inthex(data_packet[2] % 0x10);

                        data_packet[3] = reader.ReadByte();

                        string_send[8] = inthex(data_packet[3] / 0x10);
                        string_send[9] = inthex(data_packet[3] % 0x10);

                        data_packet[4] = reader.ReadByte();

                        string_send[10] = inthex(data_packet[4] / 0x10);
                        string_send[11] = inthex(data_packet[4] % 0x10);

                        data_packet[5] = reader.ReadByte();

                        string_send[12] = inthex(data_packet[5] / 0x10);
                        string_send[13] = inthex(data_packet[5] % 0x10);

                        data_packet[6] = reader.ReadByte();

                        string_send[14] = inthex(data_packet[6] / 0x10);
                        string_send[15] = inthex(data_packet[6] % 0x10);

                        data_packet[7] = reader.ReadByte();

                        string_send[16] = inthex(data_packet[7] / 0x10);
                        string_send[17] = inthex(data_packet[7] % 0x10);



                        adrH = adr / 0x100;
                        adrL = adr - adrH * 0x100;


                        string_send[18] = inthex(adrH / 0x10);
                        string_send[19] = inthex(adrH % 0x10);

                        string_send[20] = inthex(adrL / 0x10);
                        string_send[21] = inthex(adrL % 0x10);


                        string complete_command = string.Concat<char>(string_send);

                       

                        serialPort1.Write(complete_command);


                       
                        while ((serialPort1.ReadChar() != 'O'))
                        
                        {
                           
                        }
                     
                        rx_ok = 0;

                    }


                }

            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("S" + maskedTextBox3.Text + "\n");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            int i = 0;
            byte[] data_packet = new byte[9];
            char[] string_send = new char[4];


            progressBar1.Maximum = 2048;
            progressBar1.Value = 0;

            string_send[0] = 'B';
            string_send[3] = '\n';
            byte j=0;
            for (i = 0; i <= 2048; i++)
            {

                data_packet[0] = 0x77;// j;
                j++;
                string_send[1] = inthex(data_packet[0] / 0x10);
                string_send[2] = inthex(data_packet[0] % 0x10);
                string complete_command = string.Concat<char>(string_send);
                serialPort1.Write(complete_command);


                //serialPort1.WriteLine("X58\n");
                progressBar1.Value = i;
                while ((serialPort1.ReadChar() != 'Y'))

                {

                }
              // Thread.Sleep(100);
            }
            progressBar1.Value = 0;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {

           
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                long size;
                long page;
                int adr;
                byte a;
                using (FileStream fs = File.Open(openFileDialog1.FileName,FileMode.Open,FileAccess.Read,FileShare.None)) {

                    size = fs.Length;
                    label1.Text = size.ToString();
                    page = (size / 2048);
                    label2.Text = page.ToString();

                    long ost = size - page * 2048;


                    if (ost != 0)
                    {

                        page++;
                        long new_size = 2048 * page;
                        ost = new_size - size;
                    }

                    label5.Text = ost.ToString();
                    fs.Close();

                    
                    using (BinaryWriter writer = new BinaryWriter(File.Open(openFileDialog1.FileName, FileMode.Open))) {

                        adr = (int)size;
                        label6.Text = adr.ToString();
                        //adr++;

                        a = 0x55;
                        for (int i = 0; i <= ost-1; i++)
                        {
                            writer.Seek(adr+i, SeekOrigin.Begin);
                            writer.Write(a);
                        }

                        writer.Close();
                    }

                   
                };



                //using (BinaryReader reader = new BinaryReader(File.Open(openFileDialog1.FileName, FileMode.Open)))
                // {

                    //label1.Text = openFileDialog1.FileName.Length.ToString();


               // }


            }


        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            long size;
            long page;
            int adr = 0;
            byte a;


            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {


                using (FileStream fs2 = File.Open(openFileDialog1.FileName, FileMode.Open, FileAccess.Read, FileShare.None))
                {

                    size = fs2.Length;
                    label1.Text = size.ToString();
                    page = (size / 2048);
                    label2.Text = page.ToString();

                    fs2.Close();
                    progressBar1.Maximum = (int)size;
                    progressBar2.Maximum = (int)page;

                }


                using (BinaryReader reader = new BinaryReader(File.Open(openFileDialog1.FileName, FileMode.Open)))
                {


                    byte[] data_packet = new byte[9];
                    char[] string_send = new char[4];


                    for (int page_count = 1; page_count <= page; page_count++)
                   {
                        label5.Text = page_count.ToString();
                        serialPort1.WriteLine("S"+(page_count+9).ToString()+"\n");
                        // Thread.Sleep(3000);
                        while ((serialPort1.ReadChar() != 'Y')) { }

                        rx_ok = 0;
                    string_send[0] = 'B';
                        string_send[3] = '\n';
                    progressBar1.Maximum = 2048;
                        for (int adr_count = 0; adr_count <= 2047; adr_count++)
                        {
                            data_packet[0] = reader.ReadByte();

                            string_send[1] = inthex(data_packet[0] / 0x10);
                            string_send[2] = inthex(data_packet[0] % 0x10);
                            string complete_command = string.Concat<char>(string_send);
                            serialPort1.Write(complete_command);

                            progressBar1.Value = adr_count;
                           // Thread.Sleep(1);
                            while (serialPort1.ReadChar() != 'Y') { }

                        }

                        progressBar2.Value = page_count;
                        progressBar1.Value = 0;
                        serialPort1.WriteLine("W" + (page_count+9).ToString() + "\n");

                        while ((serialPort1.ReadChar() != 'Y')) { }

                    }


                }
                progressBar2.Value = 0;

            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("R\n");
            serialPort1.Close();
            Thread.Sleep(5000);
            //serialPort1.Close();

            serialPort1.Open();
            serialPort1.WriteLine("V\n");
            string rx_version = serialPort1.ReadExisting();
            richTextBox1.AppendText(rx_version);


        }

        private void button11_Click(object sender, EventArgs e)
        {

            //char[] string_send = new char[4];
            UInt32 a=0, b=0, kw;
            UInt32[] key = new UInt32[4];
            long r;
            UInt32 data_a = 0,data_b=0;

            kw = 4;
            r = 10;
            int size = 0,i=0;

            key[0] = 0xA321456C;
            key[1] = 0x67AFCDB0;
            key[2] = 0xF9A8C5E6;
            key[3] = 0xC13EA371;
            // key[4] = 0x12345678;


            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (FileStream fs2 = File.Open(openFileDialog1.FileName, FileMode.Open, FileAccess.Read, FileShare.None))
                {

                    size = (int)fs2.Length;
                    fs2.Close();


                }




                size = size / 8;
                using (BinaryReader reader = new BinaryReader(File.Open(openFileDialog1.FileName, FileMode.Open)))
                {
                    using (BinaryWriter writer = new BinaryWriter(File.Open("crypt.bin", FileMode.OpenOrCreate)))
                    {


                        for (i = 0; i < size; i++) { 
                        data_a = reader.ReadUInt32();
                        data_b = reader.ReadUInt32();

                        a = data_a; b = data_b;
                            for (r = 0; r < kw * 4 + 32; r++)
                            {
                                a += (uint)(b + ((b << 6) ^ (b >> 8)) + key[r % kw] + r);
                                r++;
                                b += (uint)(a + ((a << 6) ^ (a >> 8)) + key[r % kw] + r);
                            }
                        writer.Write(a);
                        writer.Write(b);
                    }

                    }
                }

            }


        }

        private void button12_Click(object sender, EventArgs e)
        {
            //char[] string_send = new char[4];
            UInt32 a = 0, b = 0, kw;
            UInt32[] key = new UInt32[4];
            long r;
            UInt32 data_a = 0, data_b = 0;

            kw = 4;

            key[0] = 0xA321456C;
            key[1] = 0x67AFCDB0;
            key[2] = 0xF9A8C5E6;
            key[3] = 0xC13EA371;
           // key[4] = 0x12345678;

            r = 1;
            int size = 0, i = 0;

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (FileStream fs2 = File.Open(openFileDialog1.FileName, FileMode.Open, FileAccess.Read, FileShare.None))
                {

                    size = (int)fs2.Length;
                    fs2.Close();


                }




                size = size / 8;
                using (BinaryReader reader = new BinaryReader(File.Open(openFileDialog1.FileName, FileMode.Open)))
                {
                    using (BinaryWriter writer = new BinaryWriter(File.Open("decrypt.bin", FileMode.OpenOrCreate)))
                    {


                        for (i = 0; i < size; i++)
                        {
                            data_a = reader.ReadUInt32();
                            data_b = reader.ReadUInt32();

                            a = data_a; b = data_b;

                            for (r = kw * 4 + 31; r != -1; r--)
                            {
                                b -=(uint)( a + ((a << 6) ^ (a >> 8)) + key[r % kw] + r);
                                r--;
                                a -= (uint)(b + ((b << 6) ^ (b >> 8)) + key[r % kw] + r);
                            }

                            writer.Write(a);
                            writer.Write(b);
                        }

                    }
                }

            }
        }

        private void button6_Click_1(object sender, EventArgs e)
        {

            serialPort1.WriteLine("P" + maskedTextBox2.Text + "\n");


        }

        

        





        }


    }

