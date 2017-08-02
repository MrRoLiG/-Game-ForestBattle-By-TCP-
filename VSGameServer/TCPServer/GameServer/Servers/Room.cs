using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.Servers
{
    enum RoomState
    {
        WaitingJoin,
        WaitingBattle,
        Battle,
        End
    }

    class Room
    {
        private List<Clients> clientRoom = new List<Clients>();

        private RoomState state = RoomState.WaitingJoin;

        private const int MAX_HP = 200; 

        private Server server; 
        public Room (Server server)
        {
            this.server = server;
        }

        /// <summary>
        /// 添加房间
        /// </summary>
        /// <param name="client">创建房间的客户端</param>
        public void AddClient(Clients client)
        {
            client.HP = MAX_HP;
            clientRoom.Add(client);
            client.Room = this;
            if (clientRoom.Count >= 2)
            {
                state = RoomState.WaitingBattle;
            }
        }

        /// <summary>
        /// 移除房间
        /// </summary>
        /// <param name="client"></param>
        public void RemoveClient(Clients client)
        {
            client.Room = null;
            clientRoom.Remove(client);
            if (clientRoom.Count >= 2)
            {
                state = RoomState.WaitingBattle;
            }
            else
            {
                state = RoomState.WaitingJoin;
            }
        }

        /// <summary>
        /// 取得房主信息
        /// </summary>
        public string GetHostData()
        {
            return clientRoom[0].GetUserResult();
        }

        /// <summary>
        /// 取得当前房间是否处于等待加入连接的状态
        /// </summary>
        /// <returns></returns>
        public bool IsWaitingJoin()
        {
            return state == RoomState.WaitingJoin;
        }

        /// <summary>
        /// 关闭client的房间
        /// </summary>
        /// <param name="client"></param>
        public void CloseClientRoom(Clients client)
        {
            if (client == clientRoom[0])
            {
                Close();
            }
            else
            {
                clientRoom.Remove(client);
            }
        }

        /// <summary>
        /// 关闭房主房间
        /// </summary>
        public void Close()
        {
            foreach(Clients client in clientRoom)
            {
                client.Room = null;
            }
            server.RemoveRoom(this);
        }

        /// <summary>
        /// 获得房间Id
        /// </summary>
        public int GetId()
        {
            if (clientRoom.Count > 0)
            {
                return clientRoom[0].GetUserId();
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 得到房间所有客户端的信息
        /// </summary>
        /// <returns></returns>
        public string GetRoomUserData()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach(Clients client in clientRoom)
            {
                stringBuilder.Append(client.GetUserResult() + "|");
            }
            if (stringBuilder.Length > 0)
            {
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 向其他客户端广播消息
        /// </summary>
        /// <param name="excludeClient"></param>
        /// <param name="actionCode"></param>
        /// <param name="data"></param>
        public void BroadcastMessage(Clients excludeClient,ActionCode actionCode,string data)
        {
            foreach(Clients client in clientRoom)
            {
                if(client != excludeClient)
                {
                    server.SendResponse(client, actionCode, data);
                }
            }
        }

        /// <summary>
        /// 判断是否是房主
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool IsHost(Clients client)
        {
            return client == clientRoom[0];
            //如果相等的话就是房主，clientRoom[0]代表房主
        }

        /// <summary>
        /// 开始计时,使用线程
        /// </summary>
        public void StartTimer()
        {
            new Thread(RunTimer).Start();
        }

        /// <summary>
        /// 运行计时器
        /// </summary>
        private void RunTimer()
        {
            Thread.Sleep(1000);
            for(int i = 3; i > 0; i--)
            {
                BroadcastMessage(null, ActionCode.ShowTimer, i.ToString());
                Thread.Sleep(1000);
            }
            //计时结束之后，开始游戏
            BroadcastMessage(null, ActionCode.StartPlay, "r");
        }

        /// <summary>
        /// 判断房间内角色的血量值，根据不同情况做出处理
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="excludeClient"></param>
        public void TakeDamage(int damage,Clients excludeClient)
        {
            bool isDead = false;
            foreach(Clients client in clientRoom)
            {
                if (client != excludeClient)
                {
                    if (client.TakeDamage(damage))
                    {
                        isDead = true;
                    }
                }
            }
            if(isDead == false)
            {
                return;
            }
            else
            {
                //如果其中一个游戏角色死亡，就要结束游戏
                foreach(Clients client in clientRoom)
                {
                    if (client.IsDead())
                    {
                        client.UpdateResult(false);
                        client.Send(ActionCode.GameOver, ((int)ReturnCode.Fail).ToString());
                    }
                    else
                    {
                        client.UpdateResult(true);
                        client.Send(ActionCode.GameOver, ((int)ReturnCode.Success).ToString());
                    }
                }
                Close();
            }
        }
    }
}
