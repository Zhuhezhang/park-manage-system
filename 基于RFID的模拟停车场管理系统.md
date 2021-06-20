在Windows的Visual Stdio中利用WPF C#开发一个基于RFID的模拟停车场管理系统，实现刷卡进入/离开停车场并根据停车时长收费，管理员可以对数据库任何数据进行插入、修改、查询和删除的功能。有任何问题可在评论区询问。
@[TOC](目录)
# 1.程序说明
- 该程序需要利用到RFID设备，包括读卡器以及卡片，读卡器类似于如下图，而卡片读出的是十位数。
![RFID读卡器](https://img-blog.csdnimg.cn/20210303230619528.jpg?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3FxXzQzNzk0NjMz,size_16,color_FFFFFF,t_70#pic_center)
- 使用串口读取数据，因此程序中的串口号和您的可能不同，需要进行修改，否则无法正确运行程序。
- 初次使用没有管理者数据时需要在数据库中添加管理者信息
- 源码的中的ParkManageSystem文件夹为Visual Stdio中的源码，直接使用后者打开此文件夹的.sln文件即可打开项目。而后缀为.mdf和.ldf的文件为SQL Server的数据库文件，在使用前需要使用这两个文件建立数据库。
- 里面有四个文件夹对应四个独立的程序，分别是管理界面、车辆离开、车辆进入、用户界面。代码有注释，很清楚了，具体的自己看吧。每个程序的入口是mainwindow.cs，没有rfid设备运行不了。
- 也可以根据如下的数据库逻辑结构建立数据库，但是程序连接数据库部分的代码也需要修改。

用户信息表(UserInfo)

| 字段名 | 数据类型 | 主键 | 是否可空 | 描述   |
|:--------:|:-------------:|:-------------:|:-------------:|:-------------:|
|UserID	|Nvarchar(10)|	是|	否|	用户ID|
|UserName|	Nvarchar(50)	|否|	否|	用户名字|
|UserPhone|	Nvarchar(20)	|否|	否|	用户电话|
|UserPassword|	Nvarchar(10)|	否	|否|	用户密码|
|CarID|	Nvarchar(10)	|否|	否|	车牌号|
|LableID|	Nvarchar(10)|	否|	否|	标签号|
|Balance|	Int	|否|	否|	余额|
车辆进出记录表(CarAccessInfo)

| 字段名 | 数据类型 | 主键 | 是否可空 | 描述   |
|:--------:|:-------------:|:-------------:|:-------------:|:-------------:|
|UserID	|Nvarchar(10)|	是	|否	|用户ID|
CarID	|Nvarchar(10)|	否|	否	|车牌号|
|EnterTime	|Datetime|	是	|否	|进入停车场时间|
|LeaveTime|	Datetime|	否|	是	|离开停车场时间|

管理员信息表(AdInfo)
| 字段名 | 数据类型 | 主键 | 是否可空 | 描述   |
|:--------:|:-------------:|:-------------:|:-------------:|:-------------:|
|AdID|	Nvarchar(10)	|是	|否	|管理员ID|
|AdName|	Nvarchar(50)|	否|	否|	管理员名字|
|AdPhone|	Nvarchar(20)	|否	|否|	管理员电话|
|AdPassword|	Nvarchar(10)	|否	|否	|管理员密码|

停车场信息表(ParkInfo)
| 字段名 | 数据类型 | 主键 | 是否可空 | 描述   |
|:--------:|:-------------:|:-------------:|:-------------:|:-------------:|
|ParkID|	Nvarchar(10)|	是	|否	|停车场ID|
|ParkName|	Nvarchar(10)|	否|	否	|停车场名字|
|VacancyNum	|Int	|否|	否|	空位数|
- 该项目包含四个独立的程序，也就是对应VS源码文件夹中的四个，分别是管理者界面、车辆进入（响应车辆进入刷卡的界面）、车辆离开、用户（用于用户注册充值等操作）。由于启动用户界面时，车辆进入/离开的的程序也会跟着启动（这两个只有在刷卡时才会弹出，没有在隐藏起来），所以不需要独立启动这两个程序，但在修改这两个程序中的代码时，需要重新编译运行后关闭。
- 以下为程序流程图，源码的查看可根据这进行查看，源码里也有更为详细的说明。
![用户流程图](https://img-blog.csdnimg.cn/20210303234325224.jpg?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3FxXzQzNzk0NjMz,size_16,color_FFFFFF,t_70#pic_center)
用户流程图
![管理者流程图](https://img-blog.csdnimg.cn/20210303234354475.jpg?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3FxXzQzNzk0NjMz,size_16,color_FFFFFF,t_70#pic_center)
管理者流程图
- E-R图
![E-R图](https://img-blog.csdnimg.cn/20210303234651913.jpg?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3FxXzQzNzk0NjMz,size_16,color_FFFFFF,t_70#pic_center)
# 2.程序演示
（说明：因之前演示的时候忘记截图，后来RFID设备被收走后就没有机会截图运行结果了，所以这里只截取了在VS中的页面。）
管理员登录界面：管理员通过账号密码登录，登录之后可以进行之后的增删查改等操作。
![在这里插入图片描述](https://img-blog.csdnimg.cn/20210303235429150.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3FxXzQzNzk0NjMz,size_16,color_FFFFFF,t_70#pic_center)
管理员操作界面：管理员拥有车辆进入信息管理、车辆离开信息管理、停车场信息管理、用户信息管理的权限，能对这些信息进行插入、修改、删除和查询。应该先查询才可以使用添加/删除/修改的操作。直接点击查询可以查询该表的所有信息，下拉列表可以选择查询条件，输入框中可输入要查询的。之后点击空白行输入信息后点击插入可插入信息，点击单元格修改后点击修改可修改信息，点击一行选择删除可删除信息。
 ![在这里插入图片描述](https://img-blog.csdnimg.cn/20210303235458687.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3FxXzQzNzk0NjMz,size_16,color_FFFFFF,t_70#pic_center)
车辆进入停车场提示：当车辆刷卡进入停车场时，会在屏幕上显示如下图所示的提示信息，提示消息会在几秒之后消失。
 ![在这里插入图片描述](https://img-blog.csdnimg.cn/20210303235948909.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3FxXzQzNzk0NjMz,size_16,color_FFFFFF,t_70#pic_center)
车辆离开停车场提示：当车辆离开停车场时会弹出如下图所示的信息提示。
![在这里插入图片描述](https://img-blog.csdnimg.cn/20210304000003712.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3FxXzQzNzk0NjMz,size_16,color_FFFFFF,t_70#pic_center)
用户注册：当用户首次进入该停车场时，需要进行用户注册，填写自己的信息，如帐号、密码、车牌号、标签号等，如图5所示，还有图6所示的欢迎界面。点击欢迎您的界面即出现界面选择注册或登录。
![在这里插入图片描述](https://img-blog.csdnimg.cn/20210304000100331.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3FxXzQzNzk0NjMz,size_16,color_FFFFFF,t_70#pic_center)
![在这里插入图片描述](https://img-blog.csdnimg.cn/20210304000042335.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3FxXzQzNzk0NjMz,size_16,color_FFFFFF,t_70#pic_center)
用户登录：输入注册的帐号和密码就可以登录，登录后可以对自己的信息进行查看和修改。分别如图7、8、9所示。
 ![在这里插入图片描述](https://img-blog.csdnimg.cn/20210304000223774.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3FxXzQzNzk0NjMz,size_16,color_FFFFFF,t_70#pic_center)
![在这里插入图片描述](https://img-blog.csdnimg.cn/20210304000232436.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3FxXzQzNzk0NjMz,size_16,color_FFFFFF,t_70#pic_center)
![在这里插入图片描述](https://img-blog.csdnimg.cn/20210304000240109.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3FxXzQzNzk0NjMz,size_16,color_FFFFFF,t_70#pic_center)
余额充值：当余额不足时，可以进行余额充值。
 ![在这里插入图片描述](https://img-blog.csdnimg.cn/20210304000252504.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3FxXzQzNzk0NjMz,size_16,color_FFFFFF,t_70#pic_center)
# 3.源码地址
[https://github.com/zhz000/park-manage-system](https://github.com/zhz000/park-manage-system)
[https://gitee.com/zhz000/park-manage-system](https://gitee.com/zhz000/park-manage-system)

