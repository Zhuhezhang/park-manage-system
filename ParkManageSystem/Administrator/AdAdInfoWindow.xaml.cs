//管理员信息管理

using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Administrator
{
    public partial class AdAdInfoWindow : Window
    {
        string cn, cmd, editAdID;//数据库连接字符串、sql语句、正在编辑的行的ID
        SqlConnection sqlConnection = null;

        public AdAdInfoWindow()
        {
            InitializeComponent();
            cn = "Data Source =.;Initial Catalog=ParkManageSystem;Integrated Security=True";//数据库实例连接字符串
            AdInfoDataGrid.Visibility = Visibility.Hidden;
            AdInfoDataGrid.PreparingCellForEdit += new EventHandler<DataGridPreparingCellForEditEventArgs>(GetEditAdID);
        }                                   //用于获取正在编辑的数据行的ID

        //获取正在编辑行的AdID
        private void GetEditAdID(object sender, EventArgs e)
        {
            DataRowView drv = AdInfoDataGrid.SelectedItem as DataRowView;
            editAdID = drv.Row[0].ToString();//获取选中的数据的ID
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

        //跳转到停车场信息管理界面
        private void ParkInfoButton_Click(object sender, RoutedEventArgs e)
        {
            (new AdParkInfoWindow()).Show();
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
                if (AdInfoDataGrid.Visibility == Visibility.Hidden || AdInfoDataGrid.SelectedItem == null)
                    return;
                DataRowView drv = AdInfoDataGrid.SelectedItem as DataRowView;
                cmd = "update AdInfoTable set AdID='" + drv.Row[0].ToString() + "',AdName='" + drv.Row[1].ToString() + "'," +
                      "AdPhone='" + drv.Row[2].ToString() + "',AdPassword='" + drv.Row[3].ToString() + "' where AdID='" + editAdID + "'";

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
                if (AdInfoDataGrid.Visibility == Visibility.Hidden || AdInfoDataGrid.SelectedItem == null)
                    return;
                DataRowView drv = AdInfoDataGrid.SelectedItem as DataRowView;
                string adid = drv.Row[0].ToString();//获取选中的行的第一列的数据
                drv.Delete();//删除选定的行
                cmd = "delete from AdInfoTable where AdID='" + adid + "'";

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
                if (AdInfoDataGrid.Visibility == Visibility.Hidden || AdInfoDataGrid.SelectedItem == null)
                    return;
                DataRowView drv = AdInfoDataGrid.SelectedItem as DataRowView;
                cmd = "insert into AdInfoTable(AdID,AdName,AdPhone,AdPassword) values('" + drv.Row[0].ToString() + "'," +
                      "'" + drv.Row[1].ToString() + "','" + drv.Row[2].ToString() + "','" + drv.Row[3].ToString() + "')";

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
                cmd = "select * from AdInfoTable where AdID='" + QueryTextBox.Text + "'";
            else if (QueryComboBox.Text.Equals("姓名"))
                cmd = "select * from AdInfoTable where AdName='" + QueryTextBox.Text + "'";
            else if (QueryComboBox.Text.Equals("电话"))
                cmd = "select * from AdInfoTable where AdPhone='" + QueryTextBox.Text + "'";
            else if (QueryComboBox.Text.Equals("密码"))
                cmd = "select * from AdInfoTable where AdPassword='" + QueryTextBox.Text + "'";
            else if (QueryComboBox.Text.Equals(""))
                cmd = "select * from AdInfoTable";
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
                da.Fill(ds, "AdInfoTable");//从数据库中读取数据，并填充ds
                DataView dv = new DataView(ds.Tables["AdInfoTable"]);//创建DataView实例dv，并指定其DataTable
                AdInfoDataGrid.ItemsSource = dv;//设置DataGrid的ItemsSource属性
                AdInfoDataGrid.Columns[0].Header = "账号";//设置表格列标题名
                AdInfoDataGrid.Columns[1].Header = "姓名";
                AdInfoDataGrid.Columns[2].Header = "电话";
                AdInfoDataGrid.Columns[3].Header = "密码";
                AdInfoDataGrid.Visibility = Visibility.Visible;
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