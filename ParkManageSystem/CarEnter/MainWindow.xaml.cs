//车辆进入停车场处理窗体---读取标签并查询数据库显示车牌号以及余额

using System;
using System.Data.SqlClient;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;

namespace CarEnter
{
    public partial class MainWindow : Window
    {
        string userid, carid, lableid, vacancyNum, cmd;//用户账号、车牌号、标签号、停车场空位数、连接数据库字符串、sql语句
        SqlDataReader sqlDataReader;
        SerialPort mySerialPort = new SerialPort("COM7");//创建COM3串口实例
        (SqlConnection, SqlCommand) t = (null, null);//t.Item1为SqlConnection，t.Item2为SqlCommand

        public MainWindow()
        {
            InitializeComponent();
            mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);//监视串口是否有数据
            mySerialPort.Open();//打开串口
            this.WindowState = WindowState.Minimized;//窗体最小化
        }

        //串口有数据就读出
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort sp = (SerialPort)sender;
                string indata = sp.ReadExisting();
                string rs = @"\d{10}";
                Match match = Regex.Match(indata, rs);
                if (match.Success)
                {
                    lableid = match.Value;
                    if (GetParkVacancyNum().Equals("0"))//停车场无空位/出错
                    {
                        MessageBox.Show("停车场无空位");
                        return;
                    }
                    GetInfoThrouLableID();//通过标签号获取信息
                    RecordCarEnterIInfo();//记录用户进入停车场信息
                    ReduceParkVacancyNum();//停车场空位减一
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        //查询停车场空位数
        private string GetParkVacancyNum()
        {
            cmd = "select VacancyNum from ParkInfoTable where ParkID = 1";//查询停车场空位数
            try
            {
                t = (new OperateDatabase(cmd)).OperateData();
                sqlDataReader = t.Item2.ExecuteReader();
                if (sqlDataReader.Read())//读到数据
                {
                    vacancyNum = sqlDataReader[0].ToString();
                    if (vacancyNum.Equals("0"))//无空位
                        return "0";
                    else
                        return "1";
                }
                else
                    return "0";
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                (t.Item1).Close();
                (t.Item1).Dispose();
            }
        }

        //通过标签号获取信息
        private void GetInfoThrouLableID()
        {
            cmd = "select UserID,CarID,Balance from UserInfoTable where LableID='" + lableid + "'";//查询数据获取用户信息并显示在屏幕上
            try
            {
                t = (new OperateDatabase(cmd)).OperateData();
                sqlDataReader = t.Item2.ExecuteReader();
                if(sqlDataReader.Read())//获得数据
                {
                    CarEnterWindow.Dispatcher.Invoke//窗体界面和串口读取不在一个线程，所有需要该方法访问
                        (new Action
                            (delegate
                                {
                                    userid = sqlDataReader[0].ToString();
                                    CarIDTextBox.Text = carid = sqlDataReader[1].ToString();
                                    CarIDTextBox.IsReadOnly = true;
                                    BalanceTextBox.Text = sqlDataReader[2].ToString();
                                    BalanceTextBox.IsReadOnly = true;
                                }
                            )
                        );
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                (t.Item1).Close();
                (t.Item1).Dispose();
            }
        }

        //记录用户进入停车场信息
        private void RecordCarEnterIInfo()
        {
            cmd = "insert into CarEnterInfoTable(UserID,CarID,EnterTime) values('" + userid + "','" + carid + "','" + DateTime.Now + "')";
            try                                      //记录用户进入停车场信息
            {
                t = (new OperateDatabase(cmd)).OperateData();
                t.Item2.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                (t.Item1).Close();
                (t.Item1).Dispose();
            }
        }

        //停车场空位减一
        private void ReduceParkVacancyNum()
        {
            cmd = "update ParkInfoTable set VacancyNum = '" + (int.Parse(vacancyNum) - 1).ToString() + "' where ParkID = 1";//查询停车场空位数
            try
            {
                t = (new OperateDatabase(cmd)).OperateData();
                if (t.Item2.ExecuteNonQuery() == 1)//受影响的行数
                {
                    CarEnterWindow.Dispatcher.Invoke
                    (new Action
                        (delegate
                            {
                                CarEnterWindow.WindowState = WindowState.Normal;//窗体还原
                                Thread.Sleep(3000);//睡眠3秒
                                CarEnterWindow.WindowState = WindowState.Minimized;//窗体最小化
                            }
                        )
                    );
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                (t.Item1).Close();
                (t.Item1).Dispose();
            }
        }
    }
}