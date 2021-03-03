//用户连接数据库通用类

using System;
using System.Data.SqlClient;

namespace User
{
    class OperateDatabase
    {
        string cn, cmd;//数据库连接字符串、sql语句
        SqlConnection sqlConnection;
        SqlCommand sqlCommand;

        internal OperateDatabase(string cmd)
        {
            this.cmd = cmd;
            cn = "Data Source =.;Initial Catalog=ParkManageSystem;Integrated Security=True";
        }
        //通过sql语句操作数据库并返回SqlCommand
        internal (SqlConnection, SqlCommand) OperateData()
        {
            try
            {
                sqlConnection = new SqlConnection(cn);
                sqlConnection.Open();
                sqlCommand = new SqlCommand(cmd, sqlConnection);
                return (sqlConnection, sqlCommand);//返回元组类型
            }
            catch (Exception exp)
            {
                throw exp;//抛出异常，交给调用者处理
            }
        }
    }
}