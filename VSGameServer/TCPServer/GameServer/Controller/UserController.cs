using Common;
using GameServer.DAO;
using GameServer.Model;
using GameServer.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Controller
{
    class UserController:BaseController
    {

        private UserDAO userDAO = new UserDAO();
        private ResultDAO resultDAO = new ResultDAO();

        /// <summary>
        /// 构造函数
        /// </summary>
        public UserController()
        {
            requestCode = RequestCode.User;
        }

        /// <summary>
        /// 服务器端处理登陆请求并返回状态,以及取得当前登录成功用户相关信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="client"></param>
        /// <param name="server"></param>
        public string Login(string data, Clients client,Server server)
        {
            string[] strs = data.Split(',');
            User user = userDAO.VerifyUser(client.ClientConn, strs[0], strs[1]);

            if (user == null)
            {
                return ((int)ReturnCode.Fail).ToString();
                //经对应状态的值返回，节省内存
            }
            else
            {
                Result result = resultDAO.GetResultByUserid(client.ClientConn, user.Id);

                client.SetUserResult(user, result);
                //在登陆的时候保存当前登陆用户以及用户战绩信息

                return string.Format("{0},{1},{2},{3}", ((int)ReturnCode.Success).ToString(), user.Username, result.TotalCount, result.WinCount);

            }
        }

        /// <summary>
        /// 服务器端处理注册请求
        /// </summary>
        /// <param name="data"></param>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        public string Register(string data,Clients client,Server server)
        {
            string[] strs = data.Split(',');
            string username = strs[0];
            string password = strs[1];

            bool res = userDAO.GetUserByUsername(client.ClientConn, username);
            if (res)
            {
                return ((int)ReturnCode.Fail).ToString();
            }

            userDAO.AddUser(client.ClientConn, username, password);
            return ((int)ReturnCode.Success).ToString();
        }
    }
}
