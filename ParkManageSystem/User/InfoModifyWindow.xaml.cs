//修改信息

using System;
using System.Data.SqlClient;
using System.Windows;

namespace User
{
    public partial class InfoModifyWindow : Window
    {
        public InfoModifyWindow(string id, string name, string phone, string passWord, string carID, string lableID, string balance)
        {
            InitializeComponent();
            IDTextBox.Text = id;
            IDTextBox.IsReadOnly = true;
            NameTextBox.Text = name;
            PhoneTextBox.Text = phone;
            PassWordTextBox.Text = passWord;
            CarIDTextBox.Text = carID;
            LableIDTextBox.Text = lableID;
            LableIDTextBox.IsReadOnly = true;
            BalanceTextBox.Text = balance;
            BalanceTextBox.IsReadOnly = true;
        }

        //确认按钮，提交修改后的信息
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            string cmd = "update UserInfoTable set UserName='" + NameTextBox.Text + "',UserPhone='" + PhoneTextBox.Text + "'," +
                         "UserPassWord='" + PassWordTextBox.Text + "',CarID='" + CarIDTextBox.Text + "' where UserID='" + IDTextBox.Text + "'";
            (SqlConnection, SqlCommand) t = (null, null);//t.Item1为SqlConnection，t.Item2为SqlCommand

            try
            {
                t = (new OperateDatabase(cmd)).OperateData();
                t.Item2.ExecuteReader();
                MessageBox.Show("信息修改成功");
                (new InfoViewWindow(IDTextBox.Text)).Show();
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

        //取消，返回信息查看界面
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            (new InfoViewWindow(IDTextBox.Text)).Show();
            this.Close();
        }
    }
}