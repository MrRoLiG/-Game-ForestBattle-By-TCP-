using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPClients
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            clientSocket.Connect(new IPEndPoint(IPAddress.Parse("172.17.111.5"),88));

            byte[] recData = new byte[1024];
            int length = clientSocket.Receive(recData);
            string recMessage = Encoding.UTF8.GetString(recData, 0, length);
            Console.WriteLine("recevie message from the server:\n" + recMessage);

            //while (true)
            //{
            //    string sendMessage = Console.ReadLine();
            //    if (sendMessage == "c")
            //    {
            //        clientSocket.Close();
            //        return;
            //    }
            //    //此处处理正常关闭，即输入c后关闭客户端
            //    byte[] sendData = Encoding.UTF8.GetBytes(sendMessage);
            //    clientSocket.Send(sendData);
            //}

            for(int i = 0; i < 100; i++)
            {
                clientSocket.Send(Message.GetBytes(i.ToString()));
            }
            //测试：消除粘包和分包现象，通过Message类来处理
            
            Console.ReadKey();
            clientSocket.Close();
        }
        
    }
}
