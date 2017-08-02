using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer
{
    class Message
    {
        private byte[] recData = new byte[1024];
        //接收到的数据
        private int startIndex = 0;
        //表示已经在数组中存取的数据的字节数

        public byte[] Data { get { return recData; } }
        public int StartIndex { get { return startIndex; } }
        
        /// <summary>
        /// 数组中能够接着存储的最大字节数 
        /// </summary>
        public int RemainSize { get { return recData.Length - startIndex; } }
        /// <summary>
        /// 当接收到消息的时候需要更新startIndex
        /// </summary>
        /// <param name="count"></param>
        public void AddCount(int count)
        {
            startIndex += count;
        }
        /// <summary>
        /// 重新解析数据或者读取数据
        /// </summary>
        public void ReadMessage()
        {
            while (true)
            {
                if (startIndex <= 4)
                    return;
                int length = BitConverter.ToInt32(recData, 0);
                Console.WriteLine(length);
                Console.WriteLine("startIndex:" + startIndex);
                if (startIndex - 4 >= length)
                {
                    string message = Encoding.UTF8.GetString(recData, 4, length);
                    Console.WriteLine("解析出来数据" + message);
                    Array.Copy(recData, length + 4, recData, 0, startIndex - length - 4);
                    startIndex -= length + 4;
                }
                else
                {
                    return;
                }
            }
        }
    }
}
