using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Net.Sockets;

namespace SocketServerMeter
{
    public partial class Form1 : Form
    {
        String hostName;
        IPHostEntry myself;
        IPAddress address;
        Thread myListener;
        Socket handler;
        int port2 = 6666;
        string host2 = "192.168.3.2";
        bool clientConnected = false;
        IPAddress ip2;
        IPEndPoint ipe2;
        Socket listener;
        byte[] GetData = new byte[9];
        byte[] PutDataTemp1 = {0x01,0x03,0x01,0x08,0x00,0x02,0x44,0x35};
        byte[] PutDataPress1 = {0x01,0x03, 0x01,0x06,0x00,0x02,0x25,0xF6};
        byte[] PutDataTemp2 = { 0x02, 0x02, 0x01, 0x08, 0x02, 0x8B, 0xB9 };
        byte[] PutDataPress2 = { 0x02, 0x02, 0x01, 0x0c,0x02, 0x89, 0x79 };

        byte[] floatData = new byte[4];

        Thread threadCircleRequest;

        public Form1()
        {
           InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
              
            hostName = Dns.GetHostName();
            myself = Dns.GetHostEntry(hostName);
            address = myself.AddressList[0];
            ip2 = IPAddress.Parse(host2);
            ipe2 = new IPEndPoint(ip2, port2);
         
        

        }

        void startConnect()
        {
            handler = listener.Accept();
            while (true)
            {
            Action<string> actionShowConnectMessage = (x) => { this.textBox2.Text = x; };
            textBox2.Invoke(actionShowConnectMessage, handler.RemoteEndPoint.ToString() + "已经连接...");
            clientConnected = true;
            handler.Receive(GetData);

            floatData[0] = GetData[6];
            floatData[1] = GetData[5];
            floatData[2] = GetData[4];
            floatData[3] = GetData[3];


            Action<string> actionShowRecievedData = (x) => { this.textBox1.Text = x; };
            textBox1.Invoke(actionShowRecievedData, (BitConverter.ToSingle(floatData, 0)).ToString());

            Action<string> actionShowRecievedRawData = (x) => { this.textBox3.Text = x; };
            textBox3.Invoke(actionShowRecievedRawData, BitConverter.ToString(GetData, 0));

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                textBox2.Text = "正在侦听...";
                listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                myListener = new Thread(startConnect);
                button2.Enabled = false;
                listener.Bind(ipe2);
                listener.Listen(10);
                myListener.IsBackground = true;
                myListener.Start();
                button7.Enabled = true;

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message); 
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (clientConnected)
            {
                handler.Send(PutDataTemp1);
            }
            else
            {
                MessageBox.Show("客户端未连接！");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (clientConnected)
            {
                handler.Send(PutDataPress1);
            }
            else
            {
                MessageBox.Show("客户端未连接！");
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (clientConnected)
            {
                handler.Send(PutDataTemp2);
            }
            else
            {
                MessageBox.Show("客户端未连接！");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (clientConnected)
            {
                handler.Send(PutDataPress2);
            }
            else
            {
                MessageBox.Show("客户端未连接！");
            }
        }

        void circleRequest()
        {
            while (true)
            {
                if (this.clientConnected)
                {
                    handler.Send(PutDataPress1);
                    Thread.Sleep(1000);
                    handler.Send(PutDataTemp1);
                    Thread.Sleep(1000);
                    handler.Send(PutDataPress2);
                    Thread.Sleep(1000);
                    handler.Send(PutDataTemp2);
                    Thread.Sleep(1000);
                }
            }
        }

        private void buttonCircleRequest_Click(object sender, EventArgs e)
        {

            buttonCircleRequest.Enabled = false;
            buttonStopCircle.Enabled = true;
            threadCircleRequest = new Thread(circleRequest);
            threadCircleRequest.Start();

        }

        private void buttonStopCircle_Click(object sender, EventArgs e)
        {
            threadCircleRequest.Abort();
            buttonCircleRequest.Enabled = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            listener.Close();
            myListener.Abort();
            button2.Enabled = true;
            button7.Enabled = false;
            textBox2.Text = "停止侦听";
        }
    }
}
