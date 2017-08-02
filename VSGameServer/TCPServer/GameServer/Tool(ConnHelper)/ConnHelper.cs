using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Tool_ConnHelper_
{
    class ConnHelper
    {
        public const string CONNECTSTRING = "datasource=127.0.0.1;user=root;password=root;database=forestbattle";

        /// <summary>
        /// 建立连接
        /// </summary>
        /// <returns></returns>
        public static MySqlConnection Connect()
        {
            MySqlConnection conn = new MySqlConnection(CONNECTSTRING);
            try
            {
                conn.Open();
                return conn;
            }
            catch(Exception ex)
            {
                Console.WriteLine("连接数据库的时候出现异常" + ex);
                return null;
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="conn">需要关闭的MySqlConnection</param>
        public static void CloseConnection(MySqlConnection conn)
        {
            if (conn != null)
            {
                conn.Close();
                Console.WriteLine("与数据库断开连接...");
            }
            else
            {
                Console.WriteLine("MySqlConnection不能为空！");
            }
        }

    }
}
