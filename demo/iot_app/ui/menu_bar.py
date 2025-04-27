# ui/menubar.py
import lvgl as lv
import osTimer

try:
    from common import EventMap, AbstractLoad,PrintLog
    from ui.styles import CommonStyle, FontStyle
except:
    from usr.common import EventMap, AbstractLoad,PrintLog
    from usr.ui.styles import CommonStyle, FontStyle


class MenuBar(AbstractLoad):
    NAME = "MenuBar"

    def __init__(self):       
        self.menu_bar = None
        self.base_timer = osTimer()
        self.get_battery_timer = osTimer() #电池计时器
        self.get_signal_timer = osTimer() #信号计时器

    def instance_after(self):
        EventMap.bind("menubar__show", self.__show)
        EventMap.bind("menubar__flush", self.__flush)
        EventMap.bind("menubar__close", self.__close)
        EventMap.bind("menubar__update_poc_status", self.__update_poc_status) #更新poc状态图标

    def __close(self, event=None, msg=None):
        if self.menu_bar is not None:
            self.menu_bar.delete()
            self.menu_bar = None

    def __show(self, event, meta):
        
        self.__close()
        
        self.menu_bar = lv.obj(meta)  #先建个矩形框容器
        self.menu_bar.set_pos(0, 0)
        self.menu_bar.set_size(240, 40)
        self.menu_bar.add_style(CommonStyle.container_bge1e1e1, lv.PART.MAIN | lv.STATE.DEFAULT)
        self.menu_bar.set_style_pad_left(0, 0)
        self.menu_bar.set_style_pad_right(0, 0)
        self.menu_bar.set_style_pad_top(0, 0)
        
        #信号图标
        self.img_signal = lv.img(self.menu_bar)
        self.img_signal.set_src("U:/img/signal_0.png") #先默认0格信号
        self.img_signal.set_size(20, 20)
        self.img_signal.set_pivot(0, 0)
        self.img_signal.set_angle(0)
        self.img_signal.add_style(CommonStyle.img_style, lv.PART.MAIN | lv.STATE.DEFAULT)
        self.img_signal.align(lv.ALIGN.LEFT_MID, 5, 0)  #与父左中对齐
        
        #信号类型：4G,5G
        self.lab_signal = lv.label(self.menu_bar)
        self.lab_signal.set_text("x")
        self.lab_signal.align(lv.ALIGN.LEFT_MID, 28, 0) #与父左中对齐
        self.lab_signal.add_style(FontStyle.consolas_12_txt000000_bg2195f6, lv.PART.MAIN | lv.STATE.DEFAULT)
        net_status = EventMap.send("netservice__get_net_generation") #获取网络状态：如4G
        if net_status:
            self.img_signal.set_src("U:/img/signal_5.png")
            self.lab_signal.set_text(net_status)
        
        #POC图标
        self.img_poc = lv.img(self.menu_bar)
        self.img_poc.set_size(20, 20)
        self.img_poc.set_pivot(0, 0)
        self.img_poc.set_angle(0)
        self.img_poc.add_style(CommonStyle.img_style, lv.PART.MAIN | lv.STATE.DEFAULT)
        self.img_poc.align(lv.ALIGN.LEFT_MID, 58, 0)  #与父左中对齐
        
        #GPS图标
        self.img_gps = lv.img(self.menu_bar)
        self.img_gps.set_size(20, 20)
        self.img_gps.set_pivot(0, 0)
        self.img_gps.set_angle(0)
        self.img_gps.add_style(CommonStyle.img_style, lv.PART.MAIN | lv.STATE.DEFAULT)
        self.img_gps.align(lv.ALIGN.RIGHT_MID, -65, 0)  #与父右中对齐
        
        # 电池图标
        self.img_battery = lv.img(self.menu_bar)
        self.img_battery.set_src("U:/img/charge_battery.png")
        self.img_battery.set_size(20, 20)
        self.img_battery.set_pivot(0, 0)
        self.img_battery.set_angle(0)
        self.img_battery.add_style(CommonStyle.img_style, lv.PART.MAIN | lv.STATE.DEFAULT)
        self.img_battery.align(lv.ALIGN.RIGHT_MID, -40, 1)  #与父右中对齐
        
        #电量标签
        self.lab_battery = lv.label(self.menu_bar)
        self.lab_battery.set_text("100%")
        self.lab_battery.align(lv.ALIGN.RIGHT_MID, -2, 0)
        self.lab_battery.add_style(FontStyle.consolas_12_txt000000_bg2195f6, lv.PART.MAIN | lv.STATE.DEFAULT)
        
        #系统时间标签
        self.lab_time = lv.label(self.menu_bar)
        self.lab_time.set_text("")
        self.lab_time.align(lv.ALIGN.CENTER, 0, 0)   #与父水平且垂直都置中对齐
        self.lab_time.add_style(FontStyle.consolas_12_txt000000_bg2195f6, lv.PART.MAIN | lv.STATE.DEFAULT)

        self.__update_time() #获取当前时间
        self.__update_battery()  #获取当前电量
        self.__update_signal()  #获取当前信号强度
        
        self.base_timer.start(500, 1, self.__update_time)
        self.get_battery_timer.start(10000, 1, self.__update_battery)
        self.get_signal_timer.start(2000, 1, self.__update_signal)

        EventMap.bind("menubar__update_gps_status", self.__update_gps_status)
        
    def __flush(self, event, msg):
        pass

    def __update_time(self, arg=None):
        time = EventMap.send("devinfoservice__get_time")
        if time:
            self.lab_time.set_text(time[1])

    def __update_battery(self, arg=None):
        battery = EventMap.send("screen_get_battery")
        if battery:
            self.img_battery.set_src(battery)

    def __update_signal(self, arg=None):
        sig = EventMap.send("screen_get_signal")
        if 0 < sig <= 31:
            self.img_signal.set_src('U:/img/signal_' + str(int(sig * 5 / 31)) + '.png')
            self.lab_signal.set_text("4G")
        else:
            self.img_signal.set_src("U:/img/signal_0.png")
            self.lab_signal.set_text("x")
            
    def __update_poc_status(self, event, msg):
        """
        0 停止  1 对讲  2 播放
        """
        PrintLog.log(MenuBar.NAME, "poc status: {}".format(msg))
        if 0 == msg:
            self.img_poc.add_flag(lv.obj.FLAG.HIDDEN)  #隐藏图标
        elif 1 == msg:
            self.img_poc.clear_flag(lv.obj.FLAG.HIDDEN)
            self.img_poc.set_src("U:/img/poc_speaking.png") #显示对讲，主讲
        elif 2 == msg:
            self.img_poc.clear_flag(lv.obj.FLAG.HIDDEN)
            self.img_poc.set_src("U:/img/poc_play.png") #显示播放，收听

    def __update_gps_status(self, event, msg):
        print("__update_gps_status -----------------  {}".format(msg))
        if not msg:
            self.img_gps.add_flag(lv.obj.FLAG.HIDDEN)
        else:
            self.img_gps.clear_flag(lv.obj.FLAG.HIDDEN)
            self.img_gps.set_src("U:/img/gps.png")