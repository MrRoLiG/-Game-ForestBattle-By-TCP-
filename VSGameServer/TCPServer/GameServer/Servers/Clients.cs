using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Common;
using MySql.Data.MySqlClient;
using GameServer.Tool_ConnHelper_;
using GameServer.Model;
using GameServer.DAO;

namespace GameServer.Servers
{
    class Clients
    {
        private Socket clientSocket;
        private MySqlConnection clientConn;
        private ResultDAO resultDAO = new ResultDAO();

        public MySqlConnection ClientConn
        {
            get { return clientConn; }
        }

        private User user;
        private Result result;

        private Room room;

        public Room Room
        {
            set { room = value; }
            get { return room; }
        }
        
        public int HP
        {
            get;
            set;
        }
        public bool TakeDamage(int damage)
        {
            HP -= damage;
            HP = Math.Max(HP, 0);
            if (HP <= 0) return true;
            else return false;
        }

        public void SetUserResult(User user,Result result)
        {
            this.user = user;
            this.result = result;
        }
        public string GetUserResult()
        {
            return user.Id+","+ user.Username + "," + result.TotalCount + "," + result.WinCount;
        }

        private Server server;
        
        //持有对Server类的引用
        private Message message = new Message();

        //默认构造函数
        public Clients() { }
        
        //有参构造函数
        public Clients(Socket clientSocket,Server server)
        {
            this.clientSocket = clientSocket;
            this.server = server;

            clientConn = ConnHelper.Connect();
            //开始与数据库的连接
            //Console.WriteLine("Clients.cs:客户端与数据库进行连接...");
        }
        
        /// <summary>
        /// 开启接收消息
        /// </summary>
        public void Start()
        {
            if (clientSocket == null || clientSocket.Connected == false) return;
            clientSocket.BeginReceive(message.Data, message.StartIndex, message.RemainSize(), SocketFlags.None, ReceiveCallBack,null);
            //Console.WriteLine("Clients.cs:客户端开始接收消息...");
        }
        
        /// <summary>
        /// 消息接收回调函数
        /// </summary>
        /// <param name="ar"></param>
        public void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                int count = clientSocket.EndReceive(ar);
                if (count == 0)
                {
                    Close();
                    //Console.WriteLine("Clients.cs:客户端接收完消息之后断开连接...");
                }
                //断开连接

                message.AddCount(count);
                //更新startIndex
                //Console.WriteLine("服务器端接收到了客户端的请求命令，并开始对其解析...");
                message.ReadMessage(OnProcessMessage);
                //解析消息,并且回调OnProcessMessage方法，处理解析后的到的消息
                Start();
                //clientSocket.BeginReceive(null, 0, 0, SocketFlags.None, ReceiveCallBack, null);
                //循环回调
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Close();
            }
        }
        
        /// <summary>
        /// 回调函数，用来处理解析之后得到的消息
        /// </summary>
        /// <param name="requestCode">解析后得到的requestCode</param>
        /// <param name="actionCode">解析后得到的actionCode</param>
        /// <param name="data">解析后得到的message</param>
        public void OnProcessMessage(RequestCode requestCode,ActionCode actionCode,string data)
        {
            server.HandleRequest(requestCode, actionCode, data, this);
            //通过server对象，将此步骤在server中实现
        }
        
        /// <summary>
        /// 通过在此Clients类中处理服务器端把消息给客户端打包发送
        /// 消息格式：长度+actionCode+数据
        /// </summary>
        /// <param name="actionCode">requstCode</param>
        /// <param name="data">需要处理的消息数据</param>
        public void Send(ActionCode actionCode, string data)
        {
            try
            {
                byte[] bytes = Message.PackData(actionCode, data);
                clientSocket.Send(bytes);
            }
            catch(Exception ex)
            {
                Console.WriteLine("无法发送消息" + ex);
            }
            //Console.WriteLine("已经向客户端发送完针对客户端请求的服务器端的响应命令...");
        }
       
        /// <summary>
        /// 断开连接
        /// </summary>
        public void Close()
        {
            ConnHelper.CloseConnection(clientConn);
            //关闭与数据库的连接
            if (clientSocket != null)
            {
                clientSocket.Close();
            }
            if (room != null)
            {
                room.CloseClientRoom(this);
            }
            server.RemoveClient(this);
        }

        /// <summary>
        /// 获得user的id即房主的Id
        /// </summary>
        /// <returns></returns>
        public int GetUserId()
        {
            return user.Id;
        }

        /// <summary>
        /// 判断该客户端自身是否是房主
        /// </summary>
        /// <returns></returns>
        public bool IsHost()
        {
            return room.IsHost(this);
        }

        /// <summary>
        /// 判断当前客户端中的玩家是否已经死亡
        /// </summary>
        /// <returns></returns>
        public bool IsDead()
        {
            return HP <= 0;
        }

        /// <summary>
        /// 根据玩家是否胜利来更新数据库中的战绩记录
        /// </summary>
        /// <param name="isVictory"></param>
        public void UpdateResult(bool isVictory)
        {
            UpdateResultToDB(isVictory);
            UpdateResultToClient();
        }

        /// <summary>
        /// 将战绩信息更新到数据库
        /// </summary>
        /// <param name="isVictory"></param>
        public void UpdateResultToDB(bool isVictory)
        {
            result.TotalCount++;
            if (isVictory)
            {
                result.WinCount++;
            }
            resultDAO.UpdateOrAddResult(clientConn, result);
        }

        /// <summary>
        /// 将战绩信息更新到客户端
        /// </summary>
        public void UpdateResultToClient()
        {
            string data = string.Format("{0},{1}", result.TotalCount, result.WinCount);
            Send(ActionCode.UpdateResult, data);
        }
    }
}
