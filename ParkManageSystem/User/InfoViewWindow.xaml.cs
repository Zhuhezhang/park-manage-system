//用户信息查看

using System;
using System.Data.SqlClient;
using System.Windows;

namespace User
{
    public partial class InfoViewWindow : Window
    {
        public InfoViewWindow(string id)
        {
            InitializeComponent();
            IDTextBox.Text = id;
            IDTextBox.IsReadOnly = true;
            GetUserInfo();
        }

        //跳转到充值界面
        private void RechargeButton_Click(object sender, RoutedEventArgs e)
        {
            (new RechargeWindow(IDTextBox.Text, BalanceTextBox.Text)).Show();
            this.Close();
        }

        //获取用户信息
        private void GetUserInfo()
        {
            string cmd = "select * from UserInfoTable where UserID='" + IDTextBox.Text + "'";
            SqlDataReader sqlDataReader;
            (SqlConnection, SqlCommand) t = (null, null);//t.Item1为SqlConnection，t.Item2为SqlCommand

            try
            {
                t = (new OperateDatabase(cmd)).OperateData();
                sqlDataReader = t.Item2.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    NameTextBox.Text = sqlDataReader[1].ToString();
                    NameTextBox.IsReadOnly = true;
                    PhoneTextBox.Text = sqlDataReader[2].ToString();
                    PhoneTextBox.IsReadOnly = true;
                    PassWordTextBox.Text = sqlDataReader[3].ToString();
                    PassWordTextBox.IsReadOnly = true;
                    CarIDTextBox.Text = sqlDataReader[4].ToString();
                    CarIDTextBox.IsReadOnly = true;
                    LableIDTextBox.Text = sqlDataReader[5].ToString();
                    LableIDTextBox.IsReadOnly = true;
                    BalanceTextBox.Text = sqlDataReader[6].ToString();
                    BalanceTextBox.IsReadOnly = true;
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

        //跳转到信息修改
        private void ModifyButton_Click(object sender, RoutedEventArgs e)
        {
            (new InfoModifyWindow(IDTextBox.Text, NameTextBox.Text, PhoneTextBox.Text,
                PassWordTextBox.Text, CarIDTextBox.Text, LableIDTextBox.Text, BalanceTextBox.Text)).Show();
            this.Close();
        }

        //返回主界面
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            (new MainWindow()).Show();
            this.Close();
        }
    }
}