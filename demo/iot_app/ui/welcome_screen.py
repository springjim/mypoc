import lvgl as lv
import osTimer
import utime
import sim
try:
    from ui.styles import CommonStyle, MainScreenStyle, LVGLColor,FontStyle
    from common import EventMap
    from ui.screen import Screen
    from ui.main_screen import MainScreen
except:
    from usr.ui.styles import CommonStyle, MainScreenStyle, LVGLColor,FontStyle
    from usr.common import EventMap
    from usr.ui.screen import Screen
    from usr.ui.main_screen import MainScreen

class WelcomeScreen(Screen):
    NAME = "WelcomeScreen"

    def __init__(self):
        self.meta = lv.obj()    # lvgl meta object
        
        self.main_screen_timer = osTimer()
        self.check_net_timer = osTimer()
        self.check_xin_timer = osTimer()
        self.net_status = 0     
        self.cloud_status = 0
        self.connect_field_count = 0
        self.connect_switch = False
        self.error_reason = None

    def load(self):
        self.msgbox_tip = lv.msgbox(self.meta, "Tip:", "", [], False)
        self.msgbox_tip.set_size(180, 90)
        self.msgbox_tip.align(lv.ALIGN.CENTER, 0, 0)
        self.lab_msgboxtip = lv.label(self.msgbox_tip)
        self.lab_msgboxtip.set_size(140, 50)
        self.lab_msgboxtip.set_text("初始化中...")
        self.lab_msgboxtip.set_long_mode(lv.label.LONG.SCROLL_CIRCULAR)
        self.lab_msgboxtip.add_style(FontStyle.consolas_12_txt000000_bg2195f6, lv.PART.MAIN | lv.STATE.DEFAULT)
        self.lab_msgboxtip.set_style_text_align(lv.TEXT_ALIGN.LEFT, 0)
        self.lab_msgboxtip.align(lv.ALIGN.CENTER, 0, 0)

        EventMap.bind("welcomescreen__net_status", self.__net_status)
        EventMap.bind("welcomescreen__get_net_status", self.__get_net_status)
        EventMap.bind("welcomescreen__check_cloud_status", self.__check_cloud_status)
        EventMap.bind("welcomescreen__check_error_reason", self.__check_error_reason)
        count = 0
        while True:
            if 1 == sim.getStatus():
                break
            utime.sleep(1)
            count += 1
            if count > 6:
                break
        if 0 == sim.getStatus():
            self.net_status = 1   # 未插卡标志 
            self.lab_msgboxtip.set_text("未检测到sim卡")
            EventMap.send("mediaservice__tts_play", ("未检测到sim卡", 0))
        else:
            self.lab_msgboxtip.set_text("网络注册中...")
            self.check_net_timer.start(40 * 1000, 0, lambda arg: self.__check_net_status())
            self.check_xin_timer.start(20 * 1000, 0, lambda arg: self.__check_xin_result())

    def __check_xin_result(self):
        EventMap.send("pocservice__check_xin_platform")
        if self.error_reason == "已被芯平台绑定":
            self.check_net_timer.stop()
            self.lab_msgboxtip.set_text("已被芯平台绑定")
            EventMap.send("mediaservice__tts_play", (self.error_reason, 0))

    def __net_status(self, event, msg):
        self.net_status = msg

    def __get_net_status(self, event, msg):
        return self.net_status

    def __check_cloud_status(self, event, msg):
        self.cloud_status = msg

        if self.cloud_status == 2:
            self.__check_net_status()
        if self.cloud_status == 1:
            self.check_xin_timer.stop()
            self.check_net_timer.stop() # /
            self.error_reason = None
            # 3s 之后跳转主界面
            self.main_screen_timer.start(3*1000, 0, lambda arg: EventMap.send("load_screen", {"screen": MainScreen.NAME}) )

    def __check_error_reason(self, event, msg):
        self.error_reason = msg

    def __check_net_status(self):
        if self.cloud_status == 1:
            return
        if self.net_status == 2 and self.cloud_status == 1:
            return
        if self.error_reason is not None:
            self.check_xin_timer.stop()
            reason = self.error_reason  
        elif self.net_status == 3:
            self.check_xin_timer.stop()
            reason = "网络异常"
        elif self.net_status == 2 and self.cloud_status != 1:
            reason = "账号登录中..."
        else:
            reason = "网络搜索中..."
        if "帐号不存在" == reason:
            self.lab_msgboxtip.set_text("账号不存在")
            EventMap.send("mediaservice__tts_play", (reason, 0))
        else:
            self.lab_msgboxtip.set_text(reason)
