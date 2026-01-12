# MyPOC 一站式开箱即用的公网对讲和适配终端到GB28181视频监控平台，适配Android公网对讲机（含大屏幕触屏、小屏幕按键机型）、安卓平板、嵌入式公网对讲机，全面支持信创产业要求

## 一、介绍
一款轻量级的对讲平台，可用于公网、企业局域网，能基本满足大多数常用需求；在公安、消防、安保、物业、电梯、机场、物流、铁路、企业园区、智慧工地等行业有广泛应用
终端poc app已适配GB28181, 可以注册到标准的国标视频监控平台;
提供对讲平台与GB28181视频监控平台完整搭建

国内同步仓库(优先更新内容)，[请参考](https://gitee.com/zhousiraaa/mypoc)

### 1、软件架构

![](https://github.com/springjim/mypoc/blob/main/image/architecture.png)

平台是典型云服务器+微服务+C/S+B/S的架构，平台采用多租户模型，可以布署于公网，也可以灵活布署于局域网运行。

#### 1.1对讲平台
包含语音对讲、定位轨迹、电子围栏及音视频调度

#### 1.2视频监控平台
集成了强大的开源流媒体服务[ZLMediaKit](https://github.com/ZLMediaKit/ZLMediaKit)和 SIP服务 [Wvp-GB28181-pro](https://github.com/648540858/wvp-GB28181-pro/tree/master)框架，并在此基础上扩展了公网对讲终端的GB28181协议接入，实现注册、注销、终端音视频实时点播、录像、下载等功能。

1. 目前支持android或单兵终端的H264编码，打包成 PS流上传到GB28181平台；
2. 自动判断硬件是否支持 H265硬编码，如果支持自动H265硬编码并打包成PS流上传到GB28181平台；
3. 支持终端前摄、后摄、USB(OTG)摄像头的全部 H264或 H265上传PS流；
4. 打包的PS流与海康、大华、宇视等厂家兼容，自动对VPS、PPS、SPS及 I、P、B等帧的封包处理。
5. 支持AAC音频采集打包成 PS流上传到GB28181平台；

#### 1.3视频会议平台(集成webrtc sfu)
-  提供端到端加密(e2ee)实现音视频传输  
-  提供企业内部代替“腾讯会议”的方案  
-  支持会议中调度台分享、APP分享桌面  
-  支持会议录制，录制加水印  
-  支持会议中途邀请  
-  支持调度台之间级联会议  
-  支持会议中回音消除、适配弱网流畅优先  
-  支持全内网部署，内网千兆带宽下支持近百人开语音或视频会议  
![输入图片说明](image/meet01.png)
![输入图片说明](image/meet02.png)
![输入图片说明](image/meet03.png)

### 2、二次开发与代码集成
本平台提供了API与协议对接，同时提供绝大多数客户端源代码开放，所有的第三方开发者或企业可以在此基础上定制和二次开发

| 客户端源代码      | 适配硬件终端                                | 源代码相对位置目录 | 集成和源代码说明 |
|-------------|---------------------------------------|-----------|----------|
| Android app | 兼容安卓手机、国内绝大多数厂商安卓公网对讲机或平板终端、单兵终端等            | demo\android_app         | [集成说明](https://gitee.com/zhousiraaa/mypoc/wikis/android%E5%AE%A2%E6%88%B7%E7%AB%AF%E6%BA%90%E7%A0%81%E9%9B%86%E6%88%90%E8%AF%B4%E6%98%8E?sort_id=13973379)        |
| iot嵌入式app   | 兼容移远模组EC600x/EC800x系列开发板              | demo\iot_app         | [集成说明](https://gitee.com/zhousiraaa/mypoc/wikis/iot%E5%B5%8C%E5%85%A5%E5%BC%8F%E5%AE%A2%E6%88%B7%E7%AB%AF%E6%BA%90%E7%A0%81%E9%9B%86%E6%88%90%E8%AF%B4%E6%98%8E)        |
| PC桌面端应用     | Windows10及11                          | pc_app         | 待补充...        |
| Web平台应用     | 为Edge、Chrome浏览器提供插件方案，可以将对讲快速集成到web应用 | webplugin\chrome         | [集成说明](https://gitee.com/zhousiraaa/mypoc/wikis/%E6%B5%8F%E8%A7%88%E5%99%A8(Edge%E5%92%8CChrome)%E5%BF%AB%E9%80%9F%E9%9B%86%E6%88%90%E5%AF%B9%E8%AE%B2%E8%AF%B4%E6%98%8E)        |
| Electron应用  | 提供eclectron对讲插件,快速集成                  | webplugin\electron         | [集成说明](https://gitee.com/zhousiraaa/mypoc/wikis/Electron%20%E5%BA%94%E7%94%A8%E5%BF%AB%E9%80%9F%E9%9B%86%E6%88%90%E5%AF%B9%E8%AE%B2%E8%AF%B4%E6%98%8E)        |

## 二、信创产业适配

### 1、服务端操作系统支持统信，中标麒麟，银河麒麟，欧拉
### 2、数据库支持人大金仓
### 3、Web版调度台 
  浏览器支持：360安全浏览器、微软Edge、Google Chrome浏览器，全部原生支持无需插件安装
  web版主要模块有：群组对讲、定位轨迹、电子围栏、离线地图、视频/语音通话、视频图传与分享、视频会议及录音录像等等
  ![输入图片说明](image/xinchuang/xc-login.png)
  ![输入图片说明](image/xinchuang/xc-offlinemap.png)
  ![输入图片说明](image/xinchuang/xc-stream.png)
  ![输入图片说明](image/xinchuang/xc-meeting.png)
  ![输入图片说明](image/xinchuang/xc-record.png)

## 三、平台介绍教材
- [下载最新版部署说明](https://gitee.com/zhousiraaa/mypoc/blob/master/doc/MyPOC%E5%B9%B3%E5%8F%B0%E9%83%A8%E7%BD%B2%E6%89%8B%E5%86%8C-2025.10%E7%89%88.pdf)
- [下载最新版教材说明](https://gitee.com/zhousiraaa/mypoc/blob/master/doc/MyPOC%E5%B9%B3%E5%8F%B0%E4%BB%8B%E7%BB%8D-2025.10%E7%89%88.pdf)

![](image/mypoc_intro.png)

## 四、各端功能介绍
### 1、服务端
服务端技术栈有：JDK8.0+、Mina框架 (TCP Socket)、SpringBoot、SpringCloud、MybatisPlus等, 服务端业务主要分两大部分： 
1. 实时数据/信令包，采用TCP Socket协议，主要是自定义信令包（心跳、抢麦、释放麦、上下线状态、系统状态、音视频邀请等）、媒体包（如语音编码包）等，基于Mina框架进行二进制编解码、可以满足高并发的要求。
2. 业务接口：主要实现登录、认证、鉴权、及各业务模块的接口功能.


### 2、PC端
![](images/pcoverview.png)
基于Visual Studio 2017、.net framework 4.5.2+开发，主要功能有:  
* 成员在线、离线状态同步
* 语音呼叫
* 对讲抢话
* 对讲频道切换
* 对讲禁言管理
* 对讲组自建和临时管理
* 地图框选建对讲组
* 语音同步录音、查询、下载；
* GPS数据记录、轨迹查询及回放；
* 遥毙、激活；
* 电子围栏与告警
* 视频监控与录像
* 音视频通话
* 音视频会议
* PC屏幕分享
* 集群广播
### 3、安卓端
![](image/android_overview.png)
因为对讲采用实时采集麦克风，所以要解决业界的回音啸叫、自动增益、降噪、静音检测等问题，本端主要参考了 [Android-Audio-Processing-Using-WebRTC](https://github.com/mail2chromium/Android-Audio-Processing-Using-WebRTC)，
> 在如下方面进行语音的3A处理
* Acoustic Echo Cancellation (AEC)
* Automatic Gain Controller (AGC)
* Noise Suppression (NS)
* Voice Activity Detection (VAD)
  
> 主要功能有：
* 对讲
* 对讲组自建（管理员分配权限）
* 对讲组切换
* NFC巡更签到
* 二维码巡更签到
* 直播图传
* 音视频通话
* 水印打卡
* 定位上报
* SOS呼叫与定位上报
* 支持OTG USB2/USB3 外接UVC直播图传

### 4、Web端
![](image/web_manager.png)
采用Vue2框架开发，主要功能有：
* 系统管理（角色、权限与人员分配）
* 巡更管理
* 基础管理
* 系统记录
* 任务管理

### 5、Android接入GB28181说明

第一步：app中注册28181平台

![](image/android28181reg.png)


第二步：在国标视频监控平台实时点播

![](image/platform28181.png)

目前终端已支持：前摄像头、后摄像头、UVC(USB OTG)摄像头接入28181

![](image/android_gb28181.jpg)

## 五、服务端接口说明

### 1、[MyPOC-RestAPI说明](doc/MyPOC-RestAPI.pdf)
### 2、[MyPOC-Socket说明](doc/MyPOC-Socket.pdf)

## 六、硬件适配系列

### 1、三防公网对讲机（触摸屏）
提供PTT按键、SOS按键及其它定义键的键值或广播值，几乎全部能适配

### 2、公网对讲机（按键机型）
![](image/hardware/binqi01.jpg)
![](image/hardware/binqi02.jpg)

#### 功能
提供GPS定位、语音单呼，组呼，群组切换，电子围栏报警等

### 3、ZF记录仪
![](image/hardware/zfy01.jpg)
![](image/hardware/zfy02.jpg)
![](image/hardware/zfy03.jpg)

提供全部功能等

### 4、嵌入式公网对讲机
基于移远的EC600x, EC800x 等系列适配
![](image/hardware/iot01.jpg)
![](image/hardware/iot02.png)

#### 功能
提供GPS定位、语音单呼，组呼，群组切换，电子围栏报警等

## 七、演示链接

[PC端应用下载](https://saas-park.oss-cn-shenzhen.aliyuncs.com/app/release/POCPlatformApp_Setup.exe)

[Android端应用下载](https://saas-park.oss-cn-shenzhen.aliyuncs.com/app/release/middle_PocApp_V8.0.6_202406041849.apk)

测试帐号获取，请发邮件联系，参考联系方式

## 八、国内仓库同步（最新）

https://gitee.com/zhousiraaa/mypoc  

## 九、合作说明

1. 公网帐号和环境可免费申请和测试
2. 服务端代码或接口二次开发的对接会收取点费用，视具体工作量而定
3. 如果需要整个平台源代码，请用以下联系方式与我私聊

## 十、联系方式

邮箱  
springyxlyxl@163.com

微信

![](image/wx.png)


