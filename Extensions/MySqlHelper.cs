using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace DataGet.Extensions
{
    public abstract class MySqlHelper
    {
        //数据库连接字符串
        public static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();

        /// <summary>
        /// 使用ADO连接数据库执行增、删、改
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>返回受影响行数</returns>
        public static int SqlByAdoExecuteNonQuery(string sql)
        {
            MySqlConnection conn = null;
            int result = 0;
            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = sql;
                result = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return result;
        }

        /// <summary>
        /// 使用ADO连接数据库执行增、删、改
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>返回object</returns>
        public static object SqlByAdoExecuteScalar(string sql)
        {
            MySqlConnection conn = null;
            object result = 0;
            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = sql;
                result = command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return result;
        }
        /// <summary>
        /// 使用ADO连接数据库公用 返回DataSet
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataSet SqlByAdoExecuteQuery(string sql)
        {
            MySqlConnection conn = null;
            DataSet ds = null;
            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = sql;
                MySqlDataAdapter adapter = new MySqlDataAdapter();
                adapter.SelectCommand = command;
                ds = new DataSet();
                adapter.Fill(ds);
            }
            catch (Exception ex)
            {
                Console.WriteLine("mysql-SqlByAdoExecuteQuery:" + ex.Message + ",连接字符串：" + connectionString);
                //throw ex;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return ds;
        }


    }
}
