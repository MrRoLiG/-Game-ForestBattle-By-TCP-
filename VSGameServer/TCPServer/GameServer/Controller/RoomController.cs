using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Servers;

namespace GameServer.Controller
{
    class RoomController:BaseController
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public RoomController()
        {
            requestCode = RequestCode.Room;
        }

        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="data"></param>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        public string CreateRoom(string data,Clients client,Server server)
        {
            server.CreateRoom(client);
            return ((int)ReturnCode.Success).ToString()+","+ ((int)RoleType.Blue).ToString();
        }

        /// <summary>
        /// 取得房间列表
        /// </summary>
        /// <param name="data"></param>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        public string ListRoom(string data, Clients client, Server server)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach(Room room in server.ListRoom())
            {
                if (room.IsWaitingJoin())
                {
                    stringBuilder.Append(room.GetHostData()+"|");
                }
            }
            //如果房间列表为空，再在客户端做处理不显示房间列表
            if (stringBuilder.Length == 0)
            {
                stringBuilder.Append("0");
            }
            //否则去掉所要传输的字符串的最后一个的'|'字符
            else
            {
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }
            //返回该字符串给客户端
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 加入房间
        /// </summary>
        /// <param name="data"></param>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        public string JoinRoom(string data, Clients client, Server server)
        {
            int id = int.Parse(data);
            Room room = server.GetRoomById(id);
            if (room == null)
            {
                //没有找到
                return ((int)ReturnCode.NotFound).ToString();
            }
            else if (room.IsWaitingJoin() == false)
            {
                //没有找到
                return ((int)ReturnCode.Fail).ToString();
            }
            else
            {
                room.AddClient(client);
                string roomData = room.GetRoomUserData();
                //"returncode,roleType-id,username,totalcount,wincount|id,username,totalcount,wincount"
                room.BroadcastMessage(client, ActionCode.UpdateRoom, roomData);
                return ((int)ReturnCode.Success).ToString()+ "," + ((int)RoleType.Red).ToString()+"-" +roomData;
            }
        }

        /// <summary>
        /// 退出房间
        /// </summary>
        /// <param name="data"></param>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        public string QuitRoom(string data,Clients client,Server server) 
        {
            Room room = client.Room;
            if (client.IsHost())
            {
                //是房主
                //先广播到其他的客户端
                room.BroadcastMessage(client, ActionCode.QuitRoom, ((int)ReturnCode.Success).ToString());
                //再销毁自身房间
                room.Close();
                return ((int)ReturnCode.Success).ToString();
            }
            else
            {
                //不是房主
                //先移除自己的房间
                client.Room.RemoveClient(client);
                //然后通知其他客户端自己已经退出房间
                room.BroadcastMessage(client, ActionCode.UpdateRoom, room.GetRoomUserData());
                return ((int)ReturnCode.Success).ToString();
            }
        }
    }
}
