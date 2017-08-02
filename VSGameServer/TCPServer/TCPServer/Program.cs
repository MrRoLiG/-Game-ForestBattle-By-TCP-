using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Asynchronous();
            Console.ReadKey();
        }
        static byte[] recData = new byte[1024];
        /// <summary>
        /// 异步
        /// </summary>
        static void Asynchronous()
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            EndPoint localPoint = new IPEndPoint(IPAddress.Parse("172.17.111.5"), 88);
            serverSocket.Bind(localPoint);

            serverSocket.Listen(10);

            //Socket clientSocket = serverSocket.Accept();
            //上面语句只接收一个客户端，下面使用异步方式处理接收多个客户端的消息
            serverSocket.BeginAccept(AcceptCallBack, serverSocket);
        }

        static Message message = new Message();
        //调用一个Message

        /// <summary>
        /// 消息接收回调函数
        /// </summary>
        /// <param name="ar"></param>
        static void ReceiveCallBack(IAsyncResult ar)
        {
            Socket clientSocket = null;
            try
            {
                clientSocket = ar.AsyncState as Socket;
                int length = clientSocket.EndReceive(ar);
                if (length == 0)
                {
                    clientSocket.Close();
                    return;
                }
                //处理客户端输入c关闭后，服务器端一直输入空的问题

                message.AddCount(length);

                //string recMessage = Encoding.UTF8.GetString(recData, 0, length);
                //Console.WriteLine("receive message from clients:" + recMessage);

                message.ReadMessage();

                clientSocket.BeginReceive(message.Data, message.StartIndex, message.RemainSize, SocketFlags.None, ReceiveCallBack, clientSocket);
                //循环回调
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                if (clientSocket != null)
                {
                    clientSocket.Close();
                }
            }
            //此处添加异常处理是为了处理异常关闭，即客户端的突然关闭
        }
        /// <summary>
        /// 客户端接收回调函数
        /// </summary>
        /// <param name="ar"></param>
        static void AcceptCallBack(IAsyncResult ar)
        {
            Socket serverSocket = ar.AsyncState as Socket;
            Socket clientSocket = serverSocket.EndAccept(ar);

            string sendMessage = "hello clients";
            byte[] sendData = Encoding.UTF8.GetBytes(sendMessage);
            clientSocket.Send(sendData);

            clientSocket.BeginReceive(message.Data, message.StartIndex, message.RemainSize, SocketFlags.None, ReceiveCallBack, clientSocket);
            //开始接收从客户端接收到的消息
            serverSocket.BeginAccept(AcceptCallBack, serverSocket);
            //循环回调
        }
        /// <summary>
        /// 同步
        /// </summary>
        static void Synchronization()
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            EndPoint localPoint = new IPEndPoint(IPAddress.Parse("172.17.111.5"), 88);
            serverSocket.Bind(localPoint);

            serverSocket.Listen(100);

            Socket clientSocket = serverSocket.Accept();

            string sendMessage = "hello clients";
            byte[] sendData = Encoding.UTF8.GetBytes(sendMessage);
            clientSocket.Send(sendData);

            byte[] recData = new byte[1024];
            int length = clientSocket.Receive(recData);
            string recMessage = Encoding.UTF8.GetString(recData, 0, length);
            Console.WriteLine("receive the message from client:\n" + recMessage);

        }
    }
}
