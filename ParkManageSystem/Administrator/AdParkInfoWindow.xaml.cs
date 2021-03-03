//停车场信息管理

using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Administrator
{
    public partial class AdParkInfoWindow : Window
    {
        string cn, cmd, editParkID;//数据库连接字符串、sql语句、正在编辑的行的停车场号
        SqlConnection sqlConnection = null;

        public AdParkInfoWindow()
        {
            InitializeComponent();
            cn = "Data Source =.;Initial Catalog=ParkManageSystem;Integrated Security=True";//数据库实例连接字符串
            ParkInfoDataGrid.Visibility = Visibility.Hidden;
            ParkInfoDataGrid.PreparingCellForEdit += new EventHandler<DataGridPreparingCellForEditEventArgs>(GetEditParkID);
        }                                   //用于获取正在编辑的数据行的ID

        //获取正在编辑行的ParkID
        private void GetEditParkID(object sender, EventArgs e)
        {
            DataRowView drv = ParkInfoDataGrid.SelectedItem as DataRowView;
            editParkID = drv.Row[0].ToString();//获取选中的数据的ID
        }

        //退出则返回登录界面
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            (new MainWindow()).Show();
            this.Close ();
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
                if (ParkInfoDataGrid.Visibility == Visibility.Hidden || ParkInfoDataGrid.SelectedItem == null)
                    return;
                DataRowView drv = ParkInfoDataGrid.SelectedItem as DataRowView;
                cmd = "update ParkInfoTable set ParkID='" + drv.Row[0].ToString() + "'," +
                      "ParkName='" + drv.Row[1].ToString() + "',VacancyNum='" + drv.Row[2].ToString() + "' where ParkID='" + editParkID + "'";

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
                if (ParkInfoDataGrid.Visibility == Visibility.Hidden || ParkInfoDataGrid.SelectedItem == null)
                    return;
                DataRowView drv = ParkInfoDataGrid.SelectedItem as DataRowView;
                string parkid = drv.Row[0].ToString();//获取选中的行的第一列的数据
                drv.Delete();//删除选定的行
                cmd = "delete from ParkInfoTable where ParkID='" + parkid + "'";

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
                if (ParkInfoDataGrid.Visibility == Visibility.Hidden || ParkInfoDataGrid.SelectedItem == null)
                    return;
                DataRowView drv = ParkInfoDataGrid.SelectedItem as DataRowView;
                cmd = "insert into ParkInfoTable(ParkID,ParkName,VacancyNum) values('" + drv.Row[0].ToString() + "'," +
                      "'" + drv.Row[1].ToString() + "','" + drv.Row[2].ToString() + "')";

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
            if (QueryComboBox.Text.Equals("停车场号"))
                cmd = "select * from ParkInfoTable where ParkID='" + QueryTextBox.Text + "'";
            else if (QueryComboBox.Text.Equals("停车场名"))
                cmd = "select * from ParkInfoTable where ParkName='" + QueryTextBox.Text + "'";
            else if (QueryComboBox.Text.Equals("空位数"))
                cmd = "select * from ParkInfoTable where VacancyNum='" + QueryTextBox.Text + "'";
            else if (QueryComboBox.Text.Equals(""))
                cmd = "select * from ParkInfoTable";
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
                da.Fill(ds, "ParkInfoTable");//从数据库中读取数据，并填充ds
                DataView dv = new DataView(ds.Tables["ParkInfoTable"]);//创建DataView实例dv，并指定其DataTable
                ParkInfoDataGrid.ItemsSource = dv;//设置DataGrid的ItemsSource属性
                ParkInfoDataGrid.Columns[0].Header = "停车场号";//设置表格列标题名
                ParkInfoDataGrid.Columns[1].Header = "停车场名";
                ParkInfoDataGrid.Columns[2].Header = "空位数";
                ParkInfoDataGrid.Visibility = Visibility.Visible;
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