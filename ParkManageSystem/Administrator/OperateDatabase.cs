//管理者应用的数据库通用操作类

using System;
using System.Data.SqlClient;

namespace Administrator
{
    class OperateDatabase
    {
        SqlConnection sqlConnection = null;
        SqlCommand sqlCommand = null;
        string cmd, cn;//sql语句、数据路连接字符串

        public OperateDatabase(string cmd)
        {
            this.cmd = cmd;
            cn = "Data Source =.;Initial Catalog=ParkManageSystem;Integrated Security=True";
        }

        //通过sql语句操作数据库数据
        internal void OperateData()
        {
            try
            {
                sqlConnection = new SqlConnection(cn);
                sqlConnection.Open();
                sqlCommand = new SqlCommand(cmd, sqlConnection);
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                throw exp;//抛出异常，由调用者处理
            }
            finally
            {
                sqlConnection.Close();
                sqlConnection.Dispose();
            }
        }
    }
}