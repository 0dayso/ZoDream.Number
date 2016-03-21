using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using ZoDream.Number.Model;

namespace ZoDream.Number.Helper
{
    public class DatabaseHelper
    {
        public static string ConnectionString = "Database = 'zodream'; Data Source = '127.0.0.1'; Port = '3306'; User Id = 'root'; Password = ''; charset = 'utf8'; pooling = true; Allow Zero Datetime = True";

        // INSERT IGNORE INTO `table_name` (`email`, `phone`, `user_id`) VALUES ('test9@163.com', '99999', '9999'); 避免重复


        public static MobileItem GetMobile(string number)
        {
            var result = new MobileItem();
            var section = NumberHelper.GetSectionNumber(number);
            if (string.IsNullOrWhiteSpace(section))
            {
                return result;
            }
            try
            {
                var reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT `number`,`city`,`type`,`city_code`,`postcode` FROM `zd_mobile` WHERE `number` = ?section LIMIT 1", new MySqlParameter("?section", section));
                
                if (reader.HasRows)
                {
                    reader.Read();
                    result.Number = reader.GetString(0);
                    result.City = reader.GetString(1);
                    result.Type = reader.GetString(2);
                    result.CityCode = reader.GetString(3);
                    result.PostCode = reader.GetString(4);
                }
                reader.Close();
            }
            catch (Exception)
            {
                
            }
            finally
            {
                
            }
            return result;
        }

        public static void InsertMobile(string number, string city, string kind, string citycode, string postcode)
        {
            MySqlHelper.ExecuteNonQuery(ConnectionString, 
                "INSERT INTO `zd_mobile`(`number`, `city`, `type`, `city_code`, `postcode`) VALUES (?number}, ?city, ?kind, ?citycode, ?postcode) ON DUPLICATE KEY UPDATE `city`=?city,`type`=?kind,`city_code`=?citycode,`postcode`=?postcode;", 
                new MySqlParameter("?number", number),
                new MySqlParameter("?city", number),
                new MySqlParameter("?kind", number),
                new MySqlParameter("?citycode", number),
                new MySqlParameter("?postcode", number));
        }

        public static bool InsertNumber(MySqlConnection conn,string number)
        {
            var section = NumberHelper.GetSectionNumber(number);
            if (string.IsNullOrWhiteSpace(section))
            {
                return false;
            }
            try
            {
                var id = Convert.ToInt32(MySqlHelper.ExecuteScalar(conn,
                "SELECT `id` FROM `zd_mobile` WHERE `number` = ?section LIMIT 1", new MySqlParameter("?section", section)));
                if (id < 1)
                {
                    return false;
                }
                MySqlHelper.ExecuteNonQuery(conn,
                    "INSERT IGNORE INTO `zd_number` (`number`, `mobile_id`) VALUES (?number, ?id);", new MySqlParameter("?number", number), new MySqlParameter("?id", id));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            

        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>sql2000数据库
        /// <param name="SQLStringList">多条SQL语句</param>
        public static void ExecuteSqlTran(List<string> SQLStringList)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand {Connection = conn};
                var tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    for (var n = 0; n < SQLStringList.Count; n++)
                    {
                        var strsql = SQLStringList[n];
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            cmd.ExecuteNonQuery();
                        }
                        if (n <= 0 || (n%500 != 0 && n != SQLStringList.Count - 1)) continue;
                        tx.Commit();
                        tx = conn.BeginTransaction();
                        //原本是直接下面统一提交，听从sp1234的意见，就在这里重启事务，不知道这样写会不会/好，不过我这些写运行起来好像没问题。
                    }
                    //tx.Commit();
                }
                catch (System.Data.SqlClient.SqlException E)
                {
                    tx.Rollback();
                    throw new Exception(E.Message);
                }
            }
        }
    }
}
