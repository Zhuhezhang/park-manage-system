//充值界面

using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;

namespace User
{
    public partial class RechargeWindow : Window
    {
        string id, balance;//用户账号、余额
        public RechargeWindow(string id, string balance)
        {
            InitializeComponent();
            this.id = id;
            this.balance = balance;
        }

        //取消充值，返回信息查看
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            (new InfoViewWindow(id)).Show();
            this.Close();
        }

        //确认充值并返回信息查看
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            string rs = @"^[0-9]+$";//判断输入数值是否合法
            Match match = Regex.Match(RechargeTextBox.Text, rs);
            if (!match.Success)
            {
                MessageBox.Show("请输入合法数值");
                (new InfoViewWindow (id)).Show();
                this.Close();
                return;
            }

            string ba = (int.Parse(RechargeTextBox.Text) + int.Parse(balance)).ToString();
            string cmd = "update UserInfoTable set Balance='" + ba + "' where UserID='" + id + "'";
            (SqlConnection, SqlCommand) t = (null, null);//t.Item1为SqlConnection，t.Item2为SqlCommand

            try
            {
                t = (new OperateDatabase(cmd)).OperateData();
                (t.Item2).ExecuteReader();
                MessageBox.Show("充值成功");
                (new InfoViewWindow(id)).Show();
                this.Close();
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