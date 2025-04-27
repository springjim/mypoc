import checkNet
import osTimer
import dataCall
import sim
try:
    from common import AbstractLoad, EventMap, PrintLog
except:
    from usr.common import AbstractLoad, EventMap, PrintLog

class NetService(AbstractLoad):
    """
    网络服务
    """
    THRESHOLD = 10

    def __init__(self):
        self.__check_net = checkNet.CheckNetwork("QuecPython_EC600M_CN", "Poc_Demo_v1.0")
        self.__check_net_timer = osTimer()  #用于定期检查网络状态
        self.__check_net_timeout = 60 * 1000
        self.__check_net_error_count = 0
        self.__net_generation = "4G"

    def instance_after(self, *args, **kwargs): 
        EventMap.bind("netservice__set_net_keepalive", self.__set_net_keepalive)
        EventMap.bind("netservice__set_net_generation", self.__set_net_generation)
        EventMap.bind("netservice__get_net_generation", self.__get_net_generation)
        
        # 当网络状态发生变化，比如断线、上线时，调用回调
        dataCall.setCallback(self.__datacall_callback)

    def load(self, *args, **kwargs):
        status = 1
        if sim.getStatus():
            #返回值为1，说明已经检测到SIM卡
            self.__check_net.poweron_print_once() #网络初始化信息。
            #等待网络准备就绪，超时时间为 30 秒
            stagecode, subcode = checkNet.waitNetworkReady(30)
            # 正常情况下，返回 (3,1) 表示准备就绪
            if stagecode == 1 and subcode != 1:
                status = 1
            elif stagecode == 3 and subcode == 1:
                status = 2  #正常的
            elif stagecode == 2:
                status = 3
            else:
                status = 3
            self.__do_net_check()
            self.__set_net_keepalive(event=None, msg=self.__check_net_timeout)  # 手动开启心跳检测
        EventMap.send('welcomescreen__net_status', status)

    def __datacall_callback(self, args):
        # pdp = args[0]  #蜂窝无线网卡编号，表示当前是哪一路无线网卡的网络连接状态发生了变化。
        # 网络状态，0表示网络连接断开，1表示网络连接成功。
        nw_sta = args[1]
        if nw_sta == 1: # 1 网络已连接
            EventMap.send("mediaservice__tts_play", ("网络已连接", 0))
            EventMap.send('welcomescreen__net_status', 2)
            PrintLog.log("NetService", "Network connected.")
            self.__net_generation = "4G"
            EventMap.send("menubar__update_net_status", self.__net_generation)
        else:   # 0 网络已断开
            EventMap.send("mediaservice__tts_play", ("网络已断开", 0))
            PrintLog.log("NetService", "Network disconnected.")
            EventMap.send('welcomescreen__net_status', 3)
            self.__net_generation = None
            EventMap.send("menubar__update_net_status", self.__net_generation)

    def __set_net_generation(self, event, msg):
        self.__net_generation = msg

    def __get_net_generation(self, event, msg):
        return self.__net_generation

    def __set_net_keepalive(self, event, msg):
        self.__check_net_timer.stop()
        self.__check_net_timeout = msg
        self.__check_net_timer.start(self.__check_net_timeout, 1, lambda arg: self.__do_net_check()) # 心跳检测

    def __do_net_check(self):
        status = 3
        #默认开机对profileID为1的那一路进行自动激活和使能重连，ipType =0 表示是IPv4类型地址
        if dataCall.getInfo(1, 0) != -1:
            status = 2 if dataCall.getInfo(1, 0)[2][0] else 3  # 三元表达式
        EventMap.send('welcomescreen__net_status', status)
