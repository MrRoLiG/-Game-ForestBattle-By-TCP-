using GameServer.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.DAO
{
    class ResultDAO
    {
        public Result GetResultByUserid(MySqlConnection conn,int userId)
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from result where userid=@userid ", conn);
                cmd.Parameters.AddWithValue("@userid", userId);

                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    int id = reader.GetInt32("id");
                    int totalCount = reader.GetInt32("totalcount");
                    int winCount = reader.GetInt32("wincount");

                    Result result = new Result(id, userId, totalCount,winCount);
                    
                    //Console.WriteLine("登陆时在数据库中找到对应信息...");
                    return result;
                }
                else
                {
                    Result result = new Result(-1,userId,0,0);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("在GetResultByUserid的时候出现异常" + ex);
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


        public void UpdateOrAddResult(MySqlConnection conn,Result result)
        {
            try
            {
                MySqlCommand cmd = null;

                if (result.Id <= -1)
                {
                    cmd = new MySqlCommand("insert into result set totalcount=@totalcount,wincount=@wincount,userid=@userid", conn);
                }
                else
                {
                    cmd = new MySqlCommand("update result set totalcount=@totalcount,wincount=@wincount where userid=@userid", conn);
                }
                cmd.Parameters.AddWithValue("@totalcount", result.TotalCount);
                cmd.Parameters.AddWithValue("@wincount", result.WinCount);
                cmd.Parameters.AddWithValue("@userid", result.UserId);

                cmd.ExecuteNonQuery();

                if (result.Id <= -1)
                {
                    Result tempResult = GetResultByUserid(conn, result.UserId);
                    result.Id = tempResult.Id;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("在UpdateOrAddResult的时候出现异常" + ex);
            }
        }
    }
}
