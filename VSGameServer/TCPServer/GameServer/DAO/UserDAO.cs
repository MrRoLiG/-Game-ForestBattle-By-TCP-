using GameServer.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.DAO
{
    class UserDAO
    {
        /// <summary>
        /// 验证登陆信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public User VerifyUser(MySqlConnection conn,string username,string password)
        {
            MySqlDataReader reader=null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from user where username=@username and password=@password", conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    int id = reader.GetInt32("id");

                    User user = new User(id, username, password);
                    return user;

                    //Console.WriteLine("登陆时在数据库中找到对应信息...");
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("在VerifyUser的时候出现异常" + ex);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
            return null;
        }

        /// <summary>
        /// 在注册的时候根据用户名来查找数据
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool GetUserByUsername(MySqlConnection conn, string username)
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from user where username=@username", conn);
                cmd.Parameters.AddWithValue("@username", username);

                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return true;
                    //Console.WriteLine("登陆时在数据库中找到对应信息...");
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("在GetUserByUsername的时候出现异常" + ex);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
            return false;
        }

        /// <summary>
        /// 在注册的时候如果确定用户名未被注册，添加注册用户信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void AddUser(MySqlConnection conn, string username, string password)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("insert into user set username=@username,password=@password", conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("在AddUser的时候出现异常" + ex);
            }
        }
    }
}
