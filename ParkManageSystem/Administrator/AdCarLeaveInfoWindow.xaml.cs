//车辆离开信息管理

using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Administrator
{
    public partial class AdCarLeaveInfoWindow : Window
    {
        string cn, cmd, editUserID, editEnterTime;//数据库连接字符串、sql语句、正在编辑的行的UserID、进入时间
        SqlConnection sqlConnection = null;

        public AdCarLeaveInfoWindow()
        {
            InitializeComponent();
            cn = "Data Source =.;Initial Catalog=ParkManageSystem;Integrated Security=True";//数据库实例连接字符串
            CarLeaveInfoDataGrid.Visibility = Visibility.Hidden;
            CarLeaveInfoDataGrid.PreparingCellForEdit += new EventHandler<DataGridPreparingCellForEditEventArgs>(GetEditIDAndTime);
        }                                   //用于获取正在编辑的数据行的ID

        //获取正在编辑行的UserID、EnterTime
        private void GetEditIDAndTime(object sender, EventArgs e)
        {
            DataRowView drv = CarLeaveInfoDataGrid.SelectedItem as DataRowView;
            editUserID = drv.Row[0].ToString();//获取选中的数据的ID
            editEnterTime = drv.Row[2].ToString();
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

        //跳转到停车场信息管理界面
        private void ParkInfoButton_Click(object sender, RoutedEventArgs e)
        {
            (new AdParkInfoWindow()).Show();
            this.Close();
        }

        //跳转到管理员信息管理界面
        private void AdInfoButton_Click(object sender, RoutedEventArgs e)
        {
            (new AdAdInfoWindow()).Show();
            this.Close();
        }

        //跳转到用户信息管理界面
        private void UserInfoButton_Click(object sender, RoutedEventArgs e)
        {
            (new AdUserInfoWindow()).Show();
            this.Close();
        }

        //保存修改的数据
        private void ModifyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CarLeaveInfoDataGrid.Visibility == Visibility.Hidden || CarLeaveInfoDataGrid.SelectedItem == null)
                    return;
                DataRowView drv = CarLeaveInfoDataGrid.SelectedItem as DataRowView;
                cmd = "update CarLeaveInfoTable set UserID='" + drv.Row[0].ToString() + "'," +
                      "CarID='" + drv.Row[1].ToString() + "',EnterTime='" + drv.Row[2].ToString() + "'," +
                      "LeaveTime='" + drv.Row[3].ToString() + "',FeeDeduction='" + drv.Row[4].ToString() + "'" +
                      "where UserID='" + editUserID + "' and EnterTime='" + editEnterTime + "'";

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
                if (CarLeaveInfoDataGrid.Visibility == Visibility.Hidden || CarLeaveInfoDataGrid.SelectedItem == null)
                    return;
                DataRowView drv = CarLeaveInfoDataGrid.SelectedItem as DataRowView;
                string userid = drv.Row[0].ToString();//获取选中的行的第一列的数据
                string enterTime = drv.Row[2].ToString();
                drv.Delete();//删除选定的行
                cmd = "delete from CarLeaveInfoTable where UserID='" + userid + "' and EnterTime='" + enterTime + "'";

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
                if (CarLeaveInfoDataGrid.Visibility == Visibility.Hidden || CarLeaveInfoDataGrid.SelectedItem == null)
                    return;
                DataRowView drv = CarLeaveInfoDataGrid.SelectedItem as DataRowView;
                cmd = "insert into CarLeaveInfoTable(UserID,CarID,EnterTime,LeaveTime,FeeDeduction)" +
                      "values('" + drv.Row[0].ToString() + "','" + drv.Row[1].ToString() + "','" + drv.Row[2].ToString() + "'," +
                      "'" + drv.Row[3].ToString() + "','" + drv.Row[4].ToString() + "')";

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
                cmd = "select * from CarLeaveInfoTable where UserID='" + QueryTextBox.Text + "'";
            else if (QueryComboBox.Text.Equals("车牌号"))
                cmd = "select * from CarLeaveInfoTable where CarID='" + QueryTextBox.Text + "'";
            else if (QueryComboBox.Text.Equals("进入时间"))
                cmd = "select * from CarLeaveInfoTable where EnterTime='" + QueryTextBox.Text + "'";
            else if (QueryComboBox.Text.Equals("离开时间"))
                cmd = "select * from CarLeaveInfoTable where LeaveTime='" + QueryTextBox.Text + "'";
            else if (QueryComboBox.Text.Equals("扣费金额"))
                cmd = "select * from CarLeaveInfoTable where FeeDeduction='" + QueryTextBox.Text + "'";
            else if (QueryComboBox.Text.Equals(""))
                cmd = "select * from CarLeaveInfoTable";
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
                da.Fill(ds, "CarLeaveInfoTable");//从数据库中读取数据，并填充ds
                DataView dv = new DataView(ds.Tables["CarLeaveInfoTable"]);//创建DataView实例dv，并指定其DataTable
                CarLeaveInfoDataGrid.ItemsSource = dv;//设置DataGrid的ItemsSource属性
                CarLeaveInfoDataGrid.Columns[0].Header = "用户账号";//设置表格列标题名
                CarLeaveInfoDataGrid.Columns[1].Header = "车牌号";
                CarLeaveInfoDataGrid.Columns[2].Header = "进入时间";
                CarLeaveInfoDataGrid.Columns[3].Header = "离开时间";
                CarLeaveInfoDataGrid.Columns[4].Header = "扣费金额";
                CarLeaveInfoDataGrid.Visibility = Visibility.Visible;
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