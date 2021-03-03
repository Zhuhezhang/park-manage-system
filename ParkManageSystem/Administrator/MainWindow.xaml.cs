//管理者登录界面

using System;
using System.Data.SqlClient;
using System.Windows;

namespace Administrator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //登录
        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            string cn = "Data Source =.;Initial Catalog=ParkManageSystem;Integrated Security=True";//数据库实例连接字符串
            string cmd = "select AdID,AdPassword from AdInfoTable where AdID='" + IDTextBox.Text + "'and AdPassword ='" + PassWordTextBox.Text + "'";//SQL语句实现表数据的读取
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;
            SqlDataReader sqlDataReader = null;

            try
            {
                sqlConnection = new SqlConnection(cn);//新建数据库连接实例
                sqlConnection.Open();//打开数据库连接
                sqlCommand = new SqlCommand(cmd, sqlConnection);
                sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    (new AdParkInfoWindow()).Show();
                    this.Close();
                }
                else
                { MessageBox.Show("用户名或密码错误，请重新输入！"); }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
            finally
            {
                sqlConnection.Close();
                sqlConnection.Dispose();
            }
        }
    }
}