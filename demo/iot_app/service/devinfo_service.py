import utime
import modem
import sim
import net
from misc import ADC, USB, Power
try:
    from common import AbstractLoad, EventMap, PrintLog
except:
    from usr.common import AbstractLoad, EventMap, PrintLog

class DevInfoService(AbstractLoad):
    def __init__(self):
        self.usb = USB() #用于检测usb
        self.week_list = ["一","二","三","四","五","六","日"]

    def instance_after(self):
        EventMap.bind("devinfoservice__get_time", self.__get_time)
        EventMap.bind("screen_get_battery", self.__get_battery)
        EventMap.bind("screen_get_signal", self.__get_signal)
        EventMap.bind("devinfoservice__get_firmware", self.__get_firmware)
        EventMap.bind("devinfoservice__get_usb_status", self.__get_usb_status)
        EventMap.bind("devinfoservice__get_iccid", self.__get_iccid)
        EventMap.bind("devinfoservice__get_imei", self.__get_imei)
        EventMap.bind("devinfoservice__get_device_operator", self.__get_device_operator)

    def __get_time(self, event=None, msg=None):
        local_time = utime.localtime()
        date = "{:04}-{:02}-{:02}".format(local_time[0], local_time[1], local_time[2])
        time = "{:02}:{:02}:{:02}".format(local_time[3], local_time[4], local_time[5])
        result = [date, time, self.week_list[local_time[6]]]
        return result

    def __get_firmware(self, *args):
        '''
        获取设备固件版本号
        '''
        #fw_version = modem.getDevFwVersion().replace('_OCPU_', '_')
        #fw_version = fw_version.replace('TEST', 'BETA')
        fw_version = modem.getDevFwVersion()  #获取失败会返回整型值 -1
        if isinstance(fw_version, str):
            return fw_version
        return "--"

    def __get_usb_status(self, event=None, msg=None):
        '''
        usb插拔检测
        '''
        state = self.usb.getStatus()
        if state == -1: state = 0
        return state
    
    def __get_iccid(self, event, msg):
        '''
        获取sim卡的ICCID
        '''
        iccid = sim.getIccid()
        if -1 == iccid:
            iccid = None
        else:
            dev_ope = self.__get_device_operator()
            if'中国电信' == dev_ope or '中国联通' == dev_ope:
                iccid = iccid[0:19]
        return iccid
    
    def __get_imei(self, event, msg):
        imei = modem.getDevImei()       
        if -1 == imei: imei = None
        return imei

    def __get_device_operator(self, event=None, msg=None):
        """
        获取设备运营商
        """
        net_ope_map = {
            "46001": "中国联通", "46006": "中国联通", "46009": "中国联通", "46010": "中国联通",
            "46000": "中国移动", "46002": "中国移动", "46004": "中国移动", 
            "46007": "中国移动", "46020": "中国移动", "46008": "中国移动", "46013": "中国移动",
            "46003": "中国电信", "46005": "中国电信", "46011": "中国电信", "46012": "中国电信"
        }
        """获取运营商"""
        try:
            _imsi = sim.getImsi()[0:5]  # 获取当前网络的运营商信息简称
        except Exception:
            return "无"
        return net_ope_map.get(_imsi, None)
    
    def get_device_fw_version(self, *args):
        '''
        获取设备固件版本号
        '''
        fw_version = modem.getDevFwVersion()
        if isinstance(fw_version, str):
            return fw_version
        return "--"  

    def __get_battery(self, event=None, msg=None):
        """
        获取电池事件,暂时没支持
        :param event:
        :param msg:
        :return:
        """
        value = EventMap.send("get_battery")
        if value < 3380:
            level = 0
        elif value < 3630:
            level = 1
        elif value < 3830:
            level = 2
        elif value < 4023:
            level = 3
        else:
            level = 4
        if self.get_usb_state():
            img_path = 'U:/img/charge_battery.png'
        else:
            img_path = 'U:/img/battery_' + str(level) + '.png'
        return img_path
    
    def __get_signal(self, event=None, msg=None):
        '''
        获取csq信号强度
        失败返回整型值 -1 ，返回值为 99 表示异常
        返回值：信号强度值范围0 ~ 31，值越大表示信号强度越好。
        '''
        return net.csqQueryPoll()
    
    def get_usb_state(self, event=None, msg=None):
        state = self.usb.getStatus()
        if state == -1:
            state = 0
        return state
