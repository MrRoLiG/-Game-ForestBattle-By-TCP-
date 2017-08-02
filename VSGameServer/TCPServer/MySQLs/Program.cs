using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLs
{
    class Program
    {
        static void Main(string[] args)
        {
            string connStr = "DataSource=localhost;port=3306;user=root;password=root;Database=forestbattle";
            MySqlConnection connect = new MySqlConnection(connStr);
            try
            {
                connect.Open();

                #region 查询
                //                string cmdStr = "select * from forestbattle.user";
                //                MySqlCommand cmd = new MySqlCommand(cmdStr, connect);

                //                MySqlDataReader reader = cmd.ExecuteReader();
                //                while (reader.Read())
                //                {
                //                    string username = reader.GetString("username");
                //                    string password = reader.GetString("password");
                //                    Console.WriteLine(username + ":" + password);
                //                }
                //                reader.Close();
                #endregion

                #region 插入
                //string addUsername = "123";
                //string addPassword = "123";
                //string cmdStr = "insert into user(username,password) values(@username,@password)";

                //MySqlCommand cmd = new MySqlCommand(cmdStr, connect);
                //cmd.Parameters.AddWithValue("@username", addUsername);
                //cmd.Parameters.AddWithValue("@password", addPassword);
                //cmd.ExecuteNonQuery();
                #endregion

                #region 删除
                //string cmdStr = "delete from user where username=@deletename";
                //MySqlCommand cmd = new MySqlCommand(cmdStr, connect);
                //cmd.Parameters.AddWithValue("@deletename", "123");
                //cmd.ExecuteNonQuery();
                #endregion

                #region 更新
                //string cmdStr = "update user set password=@password where username='luoling'";
                //MySqlCommand cmd = new MySqlCommand(cmdStr, connect);
                //cmd.Parameters.AddWithValue("@password", "LUOLING");
                //cmd.ExecuteNonQuery();
                #endregion

                string cmdCheckStr = "select * from forestbattle.user";
                MySqlCommand cmdCheck = new MySqlCommand(cmdCheckStr, connect);
               
                MySqlDataReader reader = cmdCheck.ExecuteReader();
                while (reader.Read())
                {
                    string username = reader.GetString("username");
                    string password = reader.GetString("password");
                    Console.WriteLine(username + ":" + password);
                }
                reader.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            connect.Close();
            Console.ReadKey();
        }
    }
}
