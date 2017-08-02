using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Servers;

namespace GameServer.Controller
{
    abstract class BaseController
    {
        protected RequestCode requestCode = RequestCode.None;
        
        /// <summary>
        /// 当ActionCode没有指定的时候调用
        /// </summary>
        /// <param name="data">客户端发送过来的数据</param>
        /// <returns></returns>
        public virtual string DefaultHandle(string data,Clients client,Server server)
        {
            return null;
        }
        
        //提供得到requestCode的方法
        public RequestCode RequestCode { get { return requestCode; } }
    }
}
