//用户注册界面

using System;
using System.Data.SqlClient;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Windows;

namespace User
{
    public partial class RegisteWindow : Window
    {
        string cmd;//SQL语句
        SqlDataReader sqlDataReader;
        (SqlConnection, SqlCommand) t = (null, null);//t.Item1为SqlConnection，t.Item2为SqlCommand

        public RegisteWindow()
        {
            InitializeComponent();
            CountUser();
            GainLableID();
            BalanceTextBox.Text = 0.ToString();
            BalanceTextBox.IsReadOnly = true;//余额初始为0，注册完成后方可充值
        }

        //获取RFID标签号
        private void GainLableID()
        {
            SerialPort sp = new SerialPort("COM5");
            sp.Open();
            string rs = @"\d{10}";
            Match match = Regex.Match(sp.ReadLine(), rs);
            sp.Close();
            if (match.Success)
            {
                LableIDTextBox.Text = match.Value;
                LableIDTextBox.IsReadOnly = true;
            }
            else
                GainLableID();//获取失败，重新获取
        }

        //查询用户总人数，为新注册用户设置账号
        private void CountUser()
        {
            cmd = @"select count(*) from UserInfoTable";//查询所有用户数
            try
            {
                t = (new OperateDatabase(cmd)).OperateData();
                sqlDataReader = t.Item2.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    IDTextBox.Text = ((int)sqlDataReader[0] + 1).ToString();//用户数加1
                    IDTextBox.IsReadOnly = true;
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show("错误提示：" + exp.Message);
            }
            finally
            {
                (t.Item1).Close();
                (t.Item1).Dispose();
            }
        }

        //取消，返回主界面
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            (new MainWindow()).Show();
            this.Close();
        }

        //确认按钮，注册
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            cmd = "insert into UserInfoTable(UserID,UserName,UserPhone,UserPassword,CarID,LableID,Balance)" +
                                        "values('" + IDTextBox.Text + "','" + NameTextBox.Text + "','" + PhoneTextBox.Text + "'," +
                                        "'" + PassWordTextBox.Text + "','" + CarIDTextBox.Text + "','" + LableIDTextBox.Text + "'," +
                                        "'" + BalanceTextBox.Text + "')";//插入记录

            try
            {
                t = (new OperateDatabase(cmd)).OperateData();
                if (t.Item2.ExecuteNonQuery() == 1)//注册成功
                {
                    MessageBox.Show("注册成功");
                    (new InfoViewWindow(IDTextBox.Text)).Show();
                    this.Close();
                }
                else
                    return;
            }
            catch (Exception exp)
            {
                MessageBox.Show("错误提示：" + exp.Message);
            }
            finally
            {
                (t.Item1).Close();
                (t.Item1).Dispose();
            }
        }
    }
}