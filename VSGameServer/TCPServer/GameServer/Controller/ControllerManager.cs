using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Reflection;
using GameServer.Servers;

namespace GameServer.Controller
{
    class ControllerManager
    {
        private Server server;

        private Dictionary<RequestCode, BaseController> controllerDict = new Dictionary<RequestCode, BaseController>();
        //使用字典类来管理各种Controller，一个request对应一个controller
        //构造函数
        public ControllerManager(Server server)
        {
            this.server = server;
            InitController();
        }
        /// <summary>
        /// 通过不同的requestCode，将其与其对应的controller添加到字典内
        /// </summary>
        void InitController()
        {
            DefaultController defaultController = new DefaultController();
            controllerDict.Add(defaultController.RequestCode, defaultController);
            controllerDict.Add(RequestCode.User, new UserController());
            controllerDict.Add(RequestCode.Room, new RoomController());
            controllerDict.Add(RequestCode.Game, new GameController()); 
        }

        /// <summary>
        /// 处理请求
        /// 根据相应的requestCode找到controller，然后根据ActionCode找到controller里面的方法进行调用
        /// </summary>
        /// <param name="requestCode">requestCode </param>
        /// <param name="actionCode">actionCode 同时其对应的方法名与其相同</param>
        /// <param name="data">接收到的数据 </param>
        public void HandleRequest(RequestCode requestCode,ActionCode actionCode,string data,Clients client)
        {
            //Console.WriteLine("服务器端正在对客户端发送的解析后的请求命令进行处理...");
            BaseController controller;
            bool isGet = controllerDict.TryGetValue(requestCode, out controller);
            if (isGet == false)
            {
                Console.WriteLine("无法得到" + requestCode + "对应的Controller，无法处理请求");
                return;
            }
            string methodName = Enum.GetName(typeof(ActionCode), actionCode);
            //将一个枚举类型转换成一个字符串，actionCode 对应的就是方法名
            MethodInfo methodInfo = controller.GetType().GetMethod(methodName);
            //根据方法名得到方法信息
            if (methodInfo == null)
            {
                Console.WriteLine("警告：在controller[" + controller.GetType() + "]没有对应的处理方法:["+methodName+"]");
            }
            //安全校验，防止取得的信息为null
            object[] param = new object[] { data, client,server};
            object obj = methodInfo.Invoke(controller,param);
            //通过methodInfo.Invoke来调用与其对应的方法,返回一个object类型
            if(obj==null||string.IsNullOrEmpty(obj as string))
            {
                return;
            }
            //如果obj为null或者空字符串,不做处理
            //相反则返回给客户端
            //Console.WriteLine("处理完成...\n得到服务器端针对客户端请求所做出的响应命令...");
            server.SendResponse(client, actionCode, obj as string);
            //做出响应
            //其过程通过controllerManage->server->client
            //目的是为了模块的单一功能性，从而降低耦合度
        }
    }
}
