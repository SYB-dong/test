﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Cient : Form
    {
        TcpClient client;
        private const int bufferSize = 8000;
        NetworkStream sendStream;

        public Cient()
        {
            InitializeComponent();
        }

        // 发送数据
        private void button2_Click(object sender, EventArgs e)
        {
            // 获取下拉框中的选择
            // 以相应的形式去发送
            if (client != null)
            {
                //要发送的信息
                if (richtext_send.Text.Trim() == string.Empty) {
                    MessageBox.Show(" 数据为空，请输入数据 ");
                    return;
                }
                string msg1 = richtext_send.Text.Trim();
                Int64 num = 0;
                try {
                    num =  System.Convert.ToInt64(msg1, 16);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(" 转换失败 " + ex.ToString());
                    return;
                }

                Int64 x = IPAddress.HostToNetworkOrder(num);
                // sendStream.WriteByte(x.)
                //将信息存入缓存中
                string hexOutput = String.Format("{0:X}", x);
                //byte[] buffer = System.Convert.ToByte(x);
                byte[] buffer = Encoding.Default.GetBytes(hexOutput);
                try {
                    sendStream.Write(buffer, 0, buffer.Length);
                    MessageBox.Show("数据发送成功");
                }
                catch (Exception ex){
                    MessageBox.Show("数据发送失败：  " + ex.ToString());
                }
               
            }
        }

        private void ListenerServer()
        {
            do
            {
                byte[] buffer = new byte[bufferSize];                        //  前面已定义buffersize=8000,类型为int
                int msgsize1;
                try

                {
                    lock (sendStream)
                    {
                        msgsize1 = sendStream.Read(buffer, 0, bufferSize);
                    }
                    string RecvDataText = null;
                    RecvDataText = Encoding.Default.GetString(buffer);       //转换成string 并储存在 RecvDataText

                    // this.Invoke就是跨线程访问ui的方法
                    Invoke((EventHandler)(delegate
                    {
                        StringBuilder recBuffer16 = new StringBuilder();//定义16进制接收缓存  
                        for (int i = 0; i < msgsize1; i++)
                        {
                            recBuffer16.AppendFormat("{0:X2}" + " ", buffer[i]);//X2表示十六进制格式（大写），域宽2位，不足的左边填0。  
                        }
                        rich_text_recv.Text += recBuffer16.ToString();//加显到接收区  
                    }));
                }
                catch
                {
                    MessageBox.Show(" 出现异常：连接已关闭....\n ");    
                    break;
                }
            } while (true);
        }

        private void connect_Click(object sender, EventArgs e)
        {
            if (text_ip.Text.Trim() == string.Empty)
            {
                MessageBox.Show("ip addr is empty ! ");
                return;
            }

            if (text_port.Text.Trim() == string.Empty)
            {
                MessageBox.Show("port is empty ! ");
                return;
            }
            IPAddress ip = IPAddress.Parse(text_ip.Text);
            client = new TcpClient();
            try
            {
                client.Connect(ip, int.Parse(text_port.Text));
                MessageBox.Show("连接服务端成功....\n ");
            }
            catch (Exception ex) {
                MessageBox.Show("连接服务端失败.... :  " + ex.ToString());
                
                return;
            }
            //获取用于发送数据的传输流
            sendStream = client.GetStream();
            System.Threading.Thread thread = new Thread(ListenerServer);
            thread.Start();
        }
    }
}
