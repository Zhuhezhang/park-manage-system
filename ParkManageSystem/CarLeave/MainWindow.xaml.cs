//车辆离开停车场处理窗体---读取标签并查询数据库显示车牌号、余额、扣费金额

using System;
using System.Data.SqlClient;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;

namespace CarLeave
{
    public partial class MainWindow : Window
    {
        //用户账号、标签号、车牌号、余额、车辆进入时间、车辆离开时间、扣费金额、连接数据库字符串、sql语句
        string userid, lableid, carid, balance, enterTime, leaveTime, feeDeduction, cmd;
        SqlDataReader sqlDataReader;
        SerialPort mySerialPort = new SerialPort("COM6");
        (SqlConnection, SqlCommand) t = (null, null);//t.Item1为SqlConnection，t.Item2为SqlCommand

        public MainWindow()
        {
            InitializeComponent();
            mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);//监视串口是否有数据
            mySerialPort.Open();
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
                    GetUserInfoThrouLableID();//通过标签号获取用户信息
                    GetCarEnterInfoThrouLanbleID();//通过用户帐号获取车辆进入的信息
                    if (!Charge())//根据停车时间收费
                        return;
                    InsertInfoInCarLeaveTable();//将记录插入数据库的车辆离开表
                    DeleteCarEnterInfoThrouLanbleID();//通过标签号删除车辆进入的信息
                    AddParkVacancyNum();//停车场空余位加1
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        //通过标签号获取用户信息
        private void GetUserInfoThrouLableID()
        {
            cmd = "select CarID,Balance,UserID from UserInfoTable where LableID='" + lableid + "'";
            try
            {
                t = (new OperateDatabase(cmd)).OperateData();
                sqlDataReader = t.Item2.ExecuteReader();
                if (sqlDataReader.Read())
                {
                    carid = sqlDataReader[0].ToString();
                    balance = sqlDataReader[1].ToString();
                    userid = sqlDataReader[2].ToString();
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

        //通过用户帐号获取车辆进入的信息
        private void GetCarEnterInfoThrouLanbleID()
        {
            cmd = "select EnterTime from CarEnterInfoTable where UserID='" + userid + "'";
            try
            {
                t = (new OperateDatabase(cmd)).OperateData();
                sqlDataReader = t.Item2.ExecuteReader();
                if (sqlDataReader.Read())
                    enterTime = sqlDataReader[0].ToString();
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

        //根据停车时间收费
        private bool Charge()
        {
            DateTime time1 = DateTime.Now;//获取当前时间
            leaveTime = time1.ToString();
            DateTime time2 = Convert.ToDateTime(enterTime);//将字符串转换成DateTime类型
            ulong totalseconds = (ulong)((time1 - time2).TotalSeconds);//将TimeSpan类型转化为秒
            if (ulong.Parse(balance) < totalseconds)//如果余额不足（基于现场演示，设每秒一块钱）
            {
                MessageBox.Show("余额不足，请充值！\n需要收费：" + totalseconds + "\n余额：" + balance);
                return false;
            }
            else
            {
                feeDeduction = totalseconds.ToString();
                cmd = "update UserInfoTable set Balance = '" + (ulong.Parse(balance) - totalseconds).ToString() + "'";
                try
                {
                    t = (new OperateDatabase(cmd)).OperateData();
                    t.Item2.ExecuteNonQuery();
                    return true;
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

        //将记录插入数据库的车辆离开表
        private void InsertInfoInCarLeaveTable()
        {
            cmd = "insert into CarLeaveInfoTable(UserID,CarID,EnterTime,LeaveTime,FeeDeduction) " +
                  "values('"+userid+ "','" + carid + "','" + enterTime + "','" + leaveTime + "','" + feeDeduction + "')";
            try
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

        //通过标签号删除车辆进入的信息
        private void DeleteCarEnterInfoThrouLanbleID()
        {
            cmd = "delete from CarEnterInfoTable where UserID='" + userid + "'";
            try
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

        //停车场空余位加1
        private void AddParkVacancyNum()
        {
            cmd = "update ParkInfoTable set VacancyNum = VacancyNum + 1 where ParkID = 1";
            try
            {
                t = (new OperateDatabase(cmd)).OperateData();
                if (t.Item2.ExecuteNonQuery() == 1)
                {
                    CarLeaveWindow.Dispatcher.Invoke//窗体界面和串口读取不在一个线程，所有需要该方法访问
                        (new Action
                            (delegate
                                {
                                    CarIDTextBox.Text = carid;
                                    BalanceTextBox.Text = (int.Parse(balance) - int.Parse(feeDeduction)).ToString();
                                    FeeDeTextBox.Text = feeDeduction;
                                    CarLeaveWindow.WindowState = WindowState.Normal;//窗体还原
                                    Thread.Sleep(3000);//睡眠3秒
                                    CarLeaveWindow.WindowState = WindowState.Minimized;//窗体最小化
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