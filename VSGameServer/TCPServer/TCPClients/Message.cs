using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPClients
{
    class Message
    {
        public static byte[] GetBytes(string sendData)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(sendData);
            //先将需要发送的字符转换成byte数组
            int dataLength = dataBytes.Length;
            //取得其长度
            byte[] length = BitConverter.GetBytes(dataLength);
            //将该字符的byte数组的长度转换成4个固定字节长度
            byte[] newBytes = length.Concat(dataBytes).ToArray();
            //将字符的byte数组长度的字节数组和要发送的字符的byte数组拼接起来

            return newBytes;
            //得到的byte数组=发送字符的byte数组的长度的4字节byte形式byte数组+所要发送字符的byte数组
        }
    }
}
