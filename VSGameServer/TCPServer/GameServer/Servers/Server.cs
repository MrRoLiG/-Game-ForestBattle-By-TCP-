using Common;
using GameServer.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Servers
{
    class Server
    {
        private IPEndPoint ipEndPoint;
        private Socket serverSocket;

        private ControllerManager controllerManager ;

        //引用一个Controllermanager在Server端管理所有controller操作

        private List<Clients> clientList = new List<Clients>();
        //定义一个clientList来统一管理所有连接的客户端
        private List<Room> roomList = new List<Room>();
        //定义一个roomList来统一管理所有的房间

        //默认构造函数
        public Server() { }
        //有参构造函数
        public Server(string ip,int port)
        {
            controllerManager = new ControllerManager(this);
            SetIPAndPort(ip,port);
            Console.WriteLine("服务器已经绑定ip和port...");
        }
        /// <summary>
        /// 设置ip和端口号
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口号</param>
        public void SetIPAndPort(string ip,int port)
        {
            ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        }
        /// <summary>
        /// 启动服务器端
        /// </summary>
        public void Start()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            Console.WriteLine("服务器端开启...");
            serverSocket.Bind(ipEndPoint);
            serverSocket.Listen(0);
            Console.WriteLine("开始监听客户端连接...");

            serverSocket.BeginAccept(AcceptCallBack, null);
        }
        /// <summary>
        /// 服务器端接收回调函数
        /// </summary>
        /// <param name="ar"></param>
        public void AcceptCallBack(IAsyncResult ar)
        {
            Socket clientSocket = serverSocket.EndAccept(ar);
            Clients client = new Clients(clientSocket,this);
            client.Start();
            clientList.Add(client);
            //客户端连接时，加入客户端列表
            Console.WriteLine("有客户端加入连接...");
            serverSocket.BeginAccept(AcceptCallBack, null);
            //循环回调
        }
        /// <summary>
        /// 在客户端断开连接的时候，移除Client
        /// </summary>
        public void RemoveClient(Clients client)
        {
            lock (clientList)
            {
                clientList.Remove(client);
                //Console.WriteLine("客户端断开连接...");
            }
            //这里添加一个lock以防多个客户端关闭时出现异常
        }
        /// <summary>
        /// 给client客户端发起响应，通过client来处理
        /// </summary>
        /// <param name="client">要响应的客户端</param>
        /// <param name="requestCode">响应的request</param>
        /// <param name="data">响应后发送的数据</param>
        public void SendResponse(Clients client,ActionCode actionCode,string data)
        {
            //Console.WriteLine("服务器向客户端发起请求响应...");
            client.Send(actionCode, data);
        }
        /// <summary>
        /// 处理请求
        /// 处理从客户端发送过来的消息，该消息已经经过解析，得到了requestCode、actionCode、data
        /// </summary>
        /// <param name="requestCode">解析之后得到的requestCode</param>
        /// <param name="actionCode">解析之后得到的actionCode</param>
        /// <param name="data">解析之后得到的message</param>
        /// <param name="client">发送消息的client</param>
        public void HandleRequest(RequestCode requestCode, ActionCode actionCode, string data, Clients client)
        {
            controllerManager.HandleRequest(requestCode, actionCode, data, client);
            //在此服务器端又通过controllerManager对象对解析后消息进行处理
            //这里通过client->server->controllerManager来连续调用的目的在于，降低整个工程的耦合度
        }

        /// <summary>
        /// 在服务器端创建房间
        /// </summary>
        /// <param name="client">创建房间的客户端</param>
        public void CreateRoom(Clients client)
        {
            Room room = new Room(this);
            room.AddClient(client);

            roomList.Add(room);
        }

        /// <summary>
        /// 取得当前的房间列表
        /// </summary>
        /// <returns></returns>
        public List<Room> ListRoom()
        {
            return roomList;
        }

        /// <summary>
        /// 通过Id得到房间
        /// </summary>
        /// <param name="id"></param>
        public Room GetRoomById(int id)
        {
            foreach(Room roomTemp in roomList)
            {
                if (roomTemp.GetId() == id) return roomTemp;
            }
            return null;
        }

        /// <summary>
        /// 移除房间
        /// </summary>
        /// <param name="room"></param>
        public void RemoveRoom(Room room)
        {
            if (roomList != null&&room!=null)
            {
                roomList.Remove(room);
            }
        }
    }
}
