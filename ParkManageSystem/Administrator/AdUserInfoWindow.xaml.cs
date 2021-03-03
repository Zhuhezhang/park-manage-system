//用户信息管理界面

using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Administrator
{
    public partial class AdUserInfoWindow : Window
    {
        string cn, cmd, editUserID;//数据库连接字符串、sql语句、正在编辑的行的ID
        SqlConnection sqlConnection = null;

        public AdUserInfoWindow()
        {
            InitializeComponent();
            cn = "Data Source =.;Initial Catalog=ParkManageSystem;Integrated Security=True";//数据库实例连接字符串
            UserInfoDataGrid.Visibility = Visibility.Hidden;
            UserInfoDataGrid.PreparingCellForEdit += new EventHandler<DataGridPreparingCellForEditEventArgs>(GetEditUserID);
        }                                   //用于获取正在编辑的数据行的ID

        //获取正在编辑行的UserID
        private void GetEditUserID(object sender, EventArgs e)
        {
            DataRowView drv = UserInfoDataGrid.SelectedItem as DataRowView;
            editUserID = drv.Row[0].ToString();//获取选中的数据的ID
        }

        //退出则返回登录界面
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            (new MainWindow()).Show();
            this.Close();
        }

        //跳转到车辆进入信息管理界面
        private void CarEnterInfoButton_Click(object sender, RoutedEventArgs e)
        {
            (new AdCarEnterInfoWindow()).Show();
            this.Close();
        }

        //跳转到车辆离开信息管理界面
        private void CarLeaveInfoButton_Click(object sender, RoutedEventArgs e)
        {
            (new AdCarLeaveInfoWindow()).Show();
            this.Close();
        }

        //跳转到管理员信息管理界面
        private void AdInfoButton_Click(object sender, RoutedEventArgs e)
        {
            (new AdAdInfoWindow()).Show();
            this.Close();
        }

        //跳转到停车场信息管理界面
        private void ParkInfoButton_Click(object sender, RoutedEventArgs e)
        {
            (new AdParkInfoWindow()).Show();
            this.Close();
        }

        //保存修改的数据
        private void ModifyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (UserInfoDataGrid.Visibility == Visibility.Hidden || UserInfoDataGrid.SelectedItem == null)
                    return;
                DataRowView drv = UserInfoDataGrid.SelectedItem as DataRowView;
                cmd = "update UserInfoTable set UserID='" + drv.Row[0].ToString() + "'," +
                      "UserName='" + drv.Row[1].ToString() + "',UserPhone='" + drv.Row[2].ToString() + "'," +
                      "UserPassWord='" + drv.Row[3].ToString() + "',CarID='" + drv.Row[4].ToString() + "'," +
                      "LableID='" + drv.Row[5].ToString() + "',Balance='" + drv.Row[6].ToString() + "' where UserID='" + editUserID + "'";

                (new OperateDatabase(cmd)).OperateData();
                MessageBox.Show("数据修改成功");
            }
            catch (Exception exp)
            {
                MessageBox.Show("数据修改失败：" + exp.Message);
            }
        }

        //删除选中的行
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (UserInfoDataGrid.Visibility == Visibility.Hidden || UserInfoDataGrid.SelectedItem == null)
                    return;
                DataRowView drv = UserInfoDataGrid.SelectedItem as DataRowView;
                string userid = drv.Row[0].ToString();//获取选中的行的第一列的数据
                drv.Delete();//删除选定的行
                cmd = "delete from UserInfoTable where UserID='" + userid + "'";

                (new OperateDatabase(cmd)).OperateData();
                MessageBox.Show("数据删除成功");
            }
            catch (Exception exp)
            {
                MessageBox.Show("数据删除失败：" + exp.Message);
            }
        }

        //保存插入数据
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (UserInfoDataGrid.Visibility == Visibility.Hidden || UserInfoDataGrid.SelectedItem == null)
                    return;
                DataRowView drv = UserInfoDataGrid.SelectedItem as DataRowView;
                cmd = "insert into UserInfoTable(UserID,UserName,UserPhone,UserPassWord,CarID,LableID,Balance) values('" + drv.Row[0].ToString() + "'," +
                      "'" + drv.Row[1].ToString() + "','" + drv.Row[2].ToString() + "','" + drv.Row[3].ToString() + "'," +
                      "'" + drv.Row[4].ToString() + "','" + drv.Row[5].ToString() + "','" + drv.Row[6].ToString() + "')";

                (new OperateDatabase(cmd)).OperateData();
                MessageBox.Show("数据插入成功");
            }
            catch (Exception exp)
            {
                MessageBox.Show("数据插入失败：" + exp.Message);
            }
        }

        //根据条件查询
        private void QueryButton_Click(object sender, RoutedEventArgs e)
        {
            if (QueryComboBox.Text.Equals("账号"))
                cmd = "select * from UserInfoTable where UserID='" + QueryTextBox.Text + "'";
            else if (QueryComboBox.Text.Equals("名字"))
                cmd = "select * from UserInfoTable where UserName='" + QueryTextBox.Text + "'";
            else if (QueryComboBox.Text.Equals("电话"))
                cmd = "select * from UserInfoTable where UserPhone='" + QueryTextBox.Text + "'";
            else if (QueryComboBox.Text.Equals("密码"))
                cmd = "select * from UserInfoTable where UserPassWord='" + QueryTextBox.Text + "'";
            else if (QueryComboBox.Text.Equals("车牌号"))
                cmd = "select * from UserInfoTable where CarID='" + QueryTextBox.Text + "'";
            else if (QueryComboBox.Text.Equals("标签号"))
                cmd = "select * from UserInfoTable where LableID='" + QueryTextBox.Text + "'";
            else if (QueryComboBox.Text.Equals("余额"))
                cmd = "select * from UserInfoTable where Balance='" + QueryTextBox.Text + "'";
            else if (QueryComboBox.Text.Equals(""))
                cmd = "select * from UserInfoTable";
            else
            {
                MessageBox.Show("请输入正确的查询条件");
            }

            try
            {
                sqlConnection = new SqlConnection(cn);
                sqlConnection.Open();
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd, cn);//创建SqlDataAdapter实例da，并指定SQL查询string和SqlConnection
                da.Fill(ds, "UserInfoTable");//从数据库中读取数据，并填充ds
                DataView dv = new DataView(ds.Tables["UserInfoTable"]);//创建DataView实例dv，并指定其DataTable
                UserInfoDataGrid.ItemsSource = dv;//设置DataGrid的ItemsSource属性
                UserInfoDataGrid.Columns[0].Header = "帐号";//设置表格列标题名
                UserInfoDataGrid.Columns[1].Header = "名字";
                UserInfoDataGrid.Columns[2].Header = "电话";
                UserInfoDataGrid.Columns[3].Header = "密码";
                UserInfoDataGrid.Columns[4].Header = "车牌号";
                UserInfoDataGrid.Columns[5].Header = "标签号";
                UserInfoDataGrid.Columns[6].Header = "余额";
                UserInfoDataGrid.Visibility = Visibility.Visible;
            }
            catch (Exception exp)
            {
                MessageBox.Show("查询出错：" + exp.Message);
            }
            finally
            {
                sqlConnection.Close();
                sqlConnection.Dispose();
            }
        }
    }
}