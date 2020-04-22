using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace TCP_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            StartSeverAsync();
            Console.ReadKey();
        }

        static void StartSeverAsync()
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPAddress ipAddress = IPAddress.Parse("192.168.0.108");
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 88);

            serverSocket.Bind(ipEndPoint);
            serverSocket.Listen(0);

            serverSocket.BeginAccept(AcceptCallBack, serverSocket);
        }

        static void AcceptCallBack(IAsyncResult ar)
        {
            Socket serverSocket = ar.AsyncState as Socket;
            Socket clientSocket =  serverSocket.EndAccept(ar);

            string msg = "Hello Client! 你好";
            byte[] data = System.Text.Encoding.UTF8.GetBytes(msg);
            clientSocket.Send(data);

            clientSocket.BeginReceive(dataBuffer, 0, 1024, SocketFlags.None, ReceiveCallBack, clientSocket);
            serverSocket.BeginAccept(AcceptCallBack, serverSocket);
        }

        static byte[] dataBuffer = new byte[1024];
        static void ReceiveCallBack(IAsyncResult ar)
        {

            Socket clientSocket = null;
            try
            {
                clientSocket = ar.AsyncState as Socket;
                int count = clientSocket.EndReceive(ar);
                if(0 == count)
                {
                    clientSocket.Close();
                    Console.WriteLine("退出");
                    return;
                }
                string msg = Encoding.UTF8.GetString(dataBuffer, 0, count);
                Console.WriteLine("接收到的数据为：" + msg);
                clientSocket.BeginReceive(dataBuffer, 0, 1024, SocketFlags.None, ReceiveCallBack, clientSocket);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                if (clientSocket != null)
                {
                    clientSocket.Close();
                }
            }
        }
    }
}
