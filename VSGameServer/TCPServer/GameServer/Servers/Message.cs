using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Servers
{
    //为了解决粘包和分包问题，使用message来处理消息的解析
    class Message
    {
        private byte[] recData = new byte[1024];
        //存储接收到的数据
        private int startIndex = 0;
        //表示在recData中存储的字节数

        public byte[] Data { get { return recData; } }
        public int StartIndex { get { return startIndex; } }

        /// <summary>
        /// 数组中能够接着存储的最大字节数
        /// </summary>
        /// <returns></returns>
        public int RemainSize() { return recData.Length - startIndex; }

        /// <summary>
        /// 当接收到消息的时候更新startIndex
        /// </summary>
        /// <param name="count"></param>
        public void AddCount(int count)
        {
            startIndex += count;
        }

        /// <summary>
        /// 处理消息的解析
        /// 解析的消息的格式：长度+requestCode+actionCode+数据
        /// </summary>
        /// <param name="processCallBack">对解析之后得到的requestCode, actionCode, message进行后续操作的回调函数</param>
        public void ReadMessage(Action<RequestCode,ActionCode,string> processCallBack)
        {
            while (true)
            {
                if (startIndex <= 4) return;
                int count = BitConverter.ToInt32(recData, 0);
                //数据长度

                if (startIndex - 4 >= count)
                {
                    //string message = Encoding.UTF8.GetString(recData, 4, count);
                    //Console.WriteLine("接收到信息：" + message);

                    RequestCode requestCode = (RequestCode)BitConverter.ToInt32(recData, 4);
                    ActionCode actionCode = (ActionCode)BitConverter.ToInt32(recData, 8);
                    //从客户端发送过来的数据中解析出requestCode和actionCode

                    string message = Encoding.UTF8.GetString(recData, 12, count-8);
                    processCallBack(requestCode, actionCode, message);
                    //回调处理message的函数，这里只进行消息的解析

                    Array.Copy(recData, count + 4, recData, 0, startIndex - count - 4);
                    startIndex -= count + 4;
                }
                else { return; }
            }
        }
        /// <summary>
        /// 将服务器响应所需要发送的消息进行打包
        /// 消息格式：长度+actionCode+数据
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="data"></param>
        public static byte[] PackData(ActionCode actionCode,string data)
        {
            byte[] actionCodeBytes = BitConverter.GetBytes((int)actionCode);
            //2
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            //3
            int dataAmount = actionCodeBytes.Length + dataBytes.Length;
            byte[] dataAmountBytes = BitConverter.GetBytes(dataAmount);
            //1(顺序)

            //Console.WriteLine("将服务器响应客户端所需要发送的消息命令进行打包...");

            byte[] newBytes=dataAmountBytes.Concat(actionCodeBytes).ToArray<byte>();
            return newBytes.Concat(dataBytes).ToArray<byte>();
        }
    }
}
