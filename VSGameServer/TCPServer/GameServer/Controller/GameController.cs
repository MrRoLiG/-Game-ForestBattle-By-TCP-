using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Servers;

namespace GameServer.Controller
{
    //使用GameController处理在对战过程中的服务器与客户端的同步
    class GameController : BaseController
    {
        public GameController()
        {
            requestCode = RequestCode.Game;
        }

        /// <summary>
        /// 服务器端转发开始游戏的请求
        /// </summary>
        /// <param name="data"></param>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        public string StartGame(string data, Clients client, Server server)
        {
            if (client.IsHost())
            {
                Room room = client.Room;
                room.BroadcastMessage(client, ActionCode.StartGame, ((int)ReturnCode.Success).ToString());
                room.StartTimer();
                return ((int)ReturnCode.Success).ToString();
            }
            else
            {
                return ((int)ReturnCode.Fail).ToString();
            }
        }

        /// <summary>
        /// 服务器端向其他客户端转发关于同步移动的请求
        /// </summary>
        /// <param name="data"></param>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        public string Move(string data, Clients client, Server server)
        {
            Room room = client.Room;
            if (room != null)
                room.BroadcastMessage(client, ActionCode.Move, data);
            return null;
        }

        /// <summary>
        /// 服务器端向其他客户端转发关于同步箭的请求
        /// </summary>
        /// <param name="data"></param>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        public string Shot(string data, Clients client, Server server)
        {
            Room room = client.Room;
            if (room != null)
                room.BroadcastMessage(client, ActionCode.Shot, data);
            return null;
        }

        /// <summary>
        /// 在服务器端处理来自客户端的伤害处理
        /// </summary>
        /// <param name="data"></param>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        public string Attack(string data, Clients client, Server server)
        {
            int damage = int.Parse(data);
            Room room = client.Room;
            if (room == null)
            {
                return null;
            }
            else
            {
                room.TakeDamage(damage, client);
            }
            return null;
        }

        /// <summary>
        /// 服务器端针对客户端离开战斗的请求处理
        /// </summary>
        /// <param name="data"></param>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        public string QuitBattle(string data, Clients client, Server server)
        {
            Room room = client.Room;
            if (room != null)
            {
                room.BroadcastMessage(null, ActionCode.QuitBattle, "r");
                room.Close();
            }
            return null;
        }
    }
}
