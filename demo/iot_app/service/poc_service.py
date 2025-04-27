import osTimer
import sim
import modem

try:
    from common import AbstractLoad, EventMap, PrintLog
    
    from service.http_client import http_client_singleton
    from service.socket_client import SingletonSocket
    from service.audio_stream_record import AudioStreamRecorder

except:
    from usr.common import AbstractLoad, EventMap, PrintLog
    
    from usr.service.http_client import http_client_singleton    
    from usr.service.socket_client import SingletonSocket  
    from usr.service.audio_stream_record import AudioStreamRecorder
    
class PocService(AbstractLoad):
    """
    Poc服务
    """
    BAND_CALL = 1
    BND_LISTEN_START = 4
    BND_LISTEN_STOP = 6
    BND_SPEAK_STOP = 3
    PTT_ON = 1
    PTT_OFF = 0

    class CALL_STATE(object):
        IN_CALL = 1     # 主动呼叫
        ON_CALL = 2     # 被呼叫
        CALL_END = 0    # 呼叫结束
        ROB_CALL = 3    # 由于优先级被抢呼
        
    def __init__(self):
        
        self.socket_client = SingletonSocket()  #创建socket对象
        self.audio_stream_record= AudioStreamRecorder() #创建语音采集对象
        #http_client_singleton 不用创建，因为它是模块级的单例对象
        
        self.__speaker_status = 1     # 1：主叫 0：不允许打断 3：收听语音 2：？
        self.__call_time_status = False
        self.__call_member_timer = osTimer()
        self.__call_quit_time = 30

        self.__cloud_check_timer = osTimer()
        
        
        
        self.net_error = False
        self.error_msg = ""
        
        self.last_audio = None
        self.__session_info = None
        self.tts_play_enable = True
        self.speak_close_first = False
        self.__rocker_arm = 1
        self.main_call_end_state = False
        self.__ptt_hint_tone = 1
        self.__login_status = False
        
        self.__last_join_group = None
        self.__user_info = None
        self._user = None
        
        self.__user_id = None  #当前登录用户ID
        self.__group_name = None  #当前组名称
        self.__group_id = None  #当前组ID
        
        self.__group_name_default = "current no group"

        self.__cell_timer = osTimer()
        
    def instance_after(self, *args, **kwargs):
        
        EventMap.bind("pocservice__call_member_status", self.__call_member_status) #检查设备是否单呼
        EventMap.bind("pocservice__speaker_enable", self.__speaker_enable) # 开启/关闭Poc对讲
        
        EventMap.bind("pocservice_rob_applymic_state", self.__rob_applymic_state) # 


        '''
        EventMap.bind("pocservice__speaker_enable", self.__speaker_enable)
        EventMap.bind("pocservice__close_speaker", self.__close_speaker)
        EventMap.bind("pocservice__set_ptt_hint_tone", self.__set_ptt_hint_tone)
        EventMap.bind("pocservice__join_group", self.__join_group)
        EventMap.bind("pocservice__leave_group", self.__leave_group)
        EventMap.bind("member_speakbtn_click", self.__call_member)
        EventMap.bind("pocservice__call_member_status", self.__call_member_status)
        EventMap.bind("pocservice__call_member_exit", self.__call_member_exit)
        EventMap.bind("pocservice__get_platform", self.__get_platform)
        EventMap.bind("pocservice__set_platform", self.__set_platform) 
        EventMap.bind("pocservice__get_speaker_status", self.__get_speaker_status)
        EventMap.bind("pocservice__get_rocker_arm", self.__get_rocker_arm)
        EventMap.bind("pocservice__get_login_status", self.__get_login_status)
        EventMap.bind("pocservice__set_account", self.__set_account)
        EventMap.bind("pocservice__set_password", self.__set_password)
        EventMap.bind("pocservice__get_user_type", self.__get_user_type)
        EventMap.bind("pocservice__check_xin_platform", self.__check_xin_platform)
        EventMap.bind("pocservice__get_audio_status", self.__get_audio_status)
        EventMap.bind("request_weather_info", self.request_weather_info)
        EventMap.bind("get_gps_img_state", self.get_gps_img_state)
        EventMap.bind("request_lbs_info", self.request_lbs_info)

        EventMap.bind("get_group_name", self.__get_group_name)
        EventMap.bind("about_get_user", self.__about_user)
        EventMap.bind("group_get_list", self.__get_group_list)
        EventMap.bind("member_get_list", self.__get_member_list)
        EventMap.bind("group_count", self.__get_group_count)
        EventMap.bind("member_count", self.__get_member_count)
        '''
    
    def load(self, *args, **kwargs):
        
        PrintLog.log("PocService", "poc init with sim status = {}".format(sim.getStatus()))        
            
        #登录
        login_res = http_client_singleton.login()
        if login_res.is_success():
            print("登录成功！Token:", http_client_singleton.token)
            respdict= login_res.to_dict()
            self.__user_id=respdict["data"]["userId"]
            self.__poc_login_cb(1)
            
        else:
            print("登录失败:", login_res.errorMsg)
            self.__poc_login_cb(0)
        
        # 获取固定组    
        fixgroup_res = http_client_singleton.get_fixgroup_info()
        if fixgroup_res.is_success():
            print("获取固定组成功: ", fixgroup_res.data)
            #取第一个组的ID和名称，后续从SecureData 读
            respdict= fixgroup_res.to_dict()
            if len(respdict["data"])>0:
                self.__group_id= respdict["data"][0]["groupId"]
                self.__group_name= respdict["data"][0]["groupName"]
                print("获取固定组！组id={},组名称={}".format(self.__group_id,self.__group_name))
            
        else:
            print("获取固定组失败:", fixgroup_res.errorMsg)
        
        # 测试获取临时组    
        tempgroup_res = http_client_singleton.get_tempgroup_info()
        if tempgroup_res.is_success():
            print("获取临时组成功！Token:", tempgroup_res.data)
            respdict= tempgroup_res.to_dict()
            if (not self.__group_id) and (len(respdict["data"])>0):
                #没取到固定组，且有临时组
                self.__group_id= respdict[0]["groupId"]
                self.__group_name= respdict[0]["groupName"]
                print("获取固定组！组id={},组名称={}".format(self.__group_id,self.__group_name))
                
            
        else:
            print("获取临时组失败:", tempgroup_res.errorMsg)    
            
        # 初始化并连接socket
        if not self.socket_client.connect():
            print("Failed to connect socket")
            return
            
        # 检查socket是否已连接
        if not self.socket_client.is_connected:
            print("Socket not connected")
            return
        
        #查询SecureData, 看上次退出时的群组ID
        #发送工作报文
        if  self.__group_id and self.__user_id:
            print("Socket 发送工作报文")
            self.socket_client.send_report(self.__group_id,self.__user_id)
        
         
       
    #登录成功后，告诉欢迎页面   
    def __poc_login_cb(self, param):
        EventMap.send("welcomescreen__check_cloud_status", param) # 登录成功首页显示已登录，且去查询组群信息
        # 已登录
        if param == 1:
            self.net_error = False
            #保存是哪个平台，写到SecureData.Store, todo...
            
        # 未登录
        else:
            #每隔5秒检查有无登录
            self.__cloud_check_timer.start(5*1000, 1, lambda arg: self.__check_cloud_connect())  
            self.net_error = True
    
    
    def __check_cloud_connect(self):  
        login_res = http_client_singleton.login()
        if login_res.is_success():
            PrintLog.log("PocService", "cloud connect status success")
            self.__cloud_check_timer.stop()
            self.__poc_login_cb(1)
        else:
            EventMap.send("welcomescreen__check_cloud_status", 2)  

    def __speaker_enable(self, event, msg=None):
        #开启与关闭对讲
        PrintLog.log("PocService", "speaker enable: {}".format(msg))
        if msg:
            #开启对讲
            EventMap.send("poc_play_status", True)  # 唤醒LCD
            #发送抢麦信令
            self.socket_client.send_applymic()

        else:
            # 关闭Poc对讲
            # 发送释放麦
            self.socket_client.send_releasemic()
            self.audio_stream_record.stop_streaming()
            # 关闭讲话的30秒的计时器
            pass

    def __rob_applymic_state(self,event,msg=False):
        """
        抢麦成功与失败处理
        """
        if msg:
            #抢麦成功
            self.audio_stream_record.start_streaming()
            #后面加计时30秒，自动释放麦
            pass
        else:
            #抢麦失败
            pass 



    def __call_member_status(self, event, mode):
        '''
        检查设备是否在单呼状态

        mode: 1-PTT按下, 0-PTT抬起, 2-退出单呼返回群组
        '''
        """ if not self.__call_time_status:
            return
        if mode == PocService.PTT_ON:
            #按下ptt键
            self.__call_member_timer.stop()
        elif mode == PocService.PTT_OFF:
            #松开ptt键
            self.__call_member_timer.start(self.__call_quit_time * 1000, 0, lambda arg: self.__call_member_exit())
        else:
            self.__call_time_status = False
            #离开单呼群组
            self.__call_member_timer.stop() """
        
         
            
        
            
        
        