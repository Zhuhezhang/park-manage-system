//停车场管理系统---用户主页

using System.Diagnostics;
using System.Windows;
using System.Windows.Input;


namespace User
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();//启动这两个程序
            Process.Start(@"C:\大学\RFID原理与应用\停车场管理系统\ParkManageSystem\CarEnter\bin\Debug\CarEnter.exe");
            Process.Start(@"C:\大学\RFID原理与应用\停车场管理系统\ParkManageSystem\CarLeave\bin\Debug\CarLeave.exe");
        }

        //点击窗体显示用户注册/登录窗体UserLogInOrRegisteWindow
        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (new LogInWindow()).Show();
            this.Close();//关闭当前窗体
        }
    }
}