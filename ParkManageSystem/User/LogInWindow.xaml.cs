//停车场管理系统---用户登录/注册界面

using System;
using System.Data.SqlClient;
using System.Windows;

namespace User
{
    public partial class LogInWindow : Window
    {
        public LogInWindow()
        {
            InitializeComponent();
        }

        //点击注册按钮
        private void RegisteButton_Click(object sender, RoutedEventArgs e)
        {
            (new RegisteWindow()).Show();
            this.Close();
        }

        //点击登录按钮
        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            string id = IDTextBox.Text;
            string passWord = PassWordTextBox.Text;
            string cmd = "select UserID,UserPassword from UserInfoTable where UserID='" + id + "'and UserPassword ='" + passWord + "'";//SQL语句实现表数据的读取
            (SqlConnection, SqlCommand) t = (null, null);//t.Item1为SqlConnection，t.Item2为SqlCommand
            SqlDataReader sqlDataReader;

            try
            {
                t = (new OperateDatabase(cmd)).OperateData();
                sqlDataReader = (t.Item2).ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    (new InfoViewWindow(id)).Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("用户名或密码错误，请重新输入！");
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show("错误：" + exp.Message);
            }
            finally
            {
                (t.Item1).Close();
                (t.Item1).Dispose();
            }
        }
    }
}