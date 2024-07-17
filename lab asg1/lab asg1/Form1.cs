using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;

namespace lab_asg1
{
    public partial class Form1 : Form
    {
        Bitmap off = new Bitmap(2000, 1000);
        float w = 50;
        float h = 50;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Paint(object? sender, PaintEventArgs e)
        {
            ClientC d = new ClientC();
            off = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            DrawDubb(e.Graphics, d);
        }
        void DrawScene(Graphics g, ClientC d)
        {
            g.Clear(Color.White);
            SolidBrush brsh = new SolidBrush(Color.Black);
            g.FillEllipse(brsh, d.x, d.y, w, h);
        }
        void DrawDubb(Graphics g, ClientC d)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2, d);
            g.DrawImage(off, 0, 0);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Paint += Form1_Paint;
            this.WindowState = FormWindowState.Maximized;
            ClientC c = new ClientC();
            c.connectToSocket("localhost", 6000);
            while (true)
            {
                bool resp = c.recieveMessage();
                if (!resp)
                {
                    break;
                }
                DrawDubb(this.CreateGraphics(), c);
            }
            c.closeConnection();
        }
        class ClientC
        {
            public int x = 50;
            public int y = 50;
            NetworkStream stream;
            TcpClient tcpClient;
            public bool connectToSocket(String host, int portNumber)
            {
                try
                {
                    tcpClient = new TcpClient(host, portNumber);
                    stream = tcpClient.GetStream();
                    Console.WriteLine("Connection Made ! with " + host);
                    return true;
                }
                catch (System.Net.Sockets.SocketException e)
                {
                    Console.WriteLine("Connection Failed: " + e);
                    return false;
                }
            }
            public bool recieveMessage()
            {
                try
                {
                    byte[] receiveBuffer = new byte[1024];
                    int bytesReceived = stream.Read(receiveBuffer);
                    string data = Encoding.UTF8.GetString(receiveBuffer.AsSpan(0, bytesReceived));
                    if (data == "exit")
                    {
                        return false;
                    }
                    string[] points = data.Split(",");
                    if (points.Length > 1)
                    {
                        for (int i = 0; i < points.Length; i++)
                        {
                            if (i == 0)
                            {
                                x = int.Parse(points[0]);
                                Console.WriteLine(points[0]);
                                y = int.Parse(points[1]);
                                Console.WriteLine(points[1]);
                                break;
                            }
                        }
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Connection not initialized : " + e);
                    return false;
                }
            }
            public bool closeConnection()
            {
                stream.Close();
                tcpClient.Close();
                Console.WriteLine("Connection terminated ");
                return true;
            }
        }
        
    }
}
