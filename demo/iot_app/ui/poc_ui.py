
import lvgl as lv
import osTimer

try:    
    from common import EventMap, AbstractLoad, PrintLog
    from ui.welcome_screen import WelcomeScreen
    from ui.main_screen import MainScreen
    from ui.prompt_box import PromptBox
    from dev.lcd import ST7789
except:    
    from usr.common import AbstractLoad, EventMap, PrintLog
    from usr.ui.welcome_screen import WelcomeScreen
    from usr.ui.main_screen import MainScreen
    from usr.ui.prompt_box import PromptBox
    from usr.dev.lcd import ST7789


LCD_SIZE_WIDTH  = 240
LCD_SIZE_HEIGHT = 240

g_lcd = ST7789(Interface=1, SPICS=35, SPIRST=34, SPIDC=16)

def init_lvgl():
    lv.init()   # 初始化lvgl

    # Register SDL display driver.
    disp_buf1 = lv.disp_draw_buf_t()
    buf1_1 = bytearray(LCD_SIZE_WIDTH * LCD_SIZE_HEIGHT * 2)
    disp_buf1.init(buf1_1, None, len(buf1_1))
    disp_drv = lv.disp_drv_t()
    disp_drv.init()
    disp_drv.draw_buf = disp_buf1
    disp_drv.flush_cb = g_lcd._lcd.lcd_write
    disp_drv.hor_res = LCD_SIZE_WIDTH
    disp_drv.ver_res = LCD_SIZE_HEIGHT
    disp_drv.register()
    lv.tick_inc(5)  # 启动lvgv线程
    PrintLog.log("init_lvgl", "init_lvgl OK.")

init_lvgl() 
      

class PocUI(AbstractLoad):

    def __init__(self):
        self.bar_list = []
        self.msgbox_list = []
        self.screen_list = []
        self.curr_screen = None
        
        if g_lcd is None:
            PrintLog.log("PocUI", "g_lcd IS NONE.")
        
        self.lcd = g_lcd

        self.__poc_speak_status = False     # 默认不处于对讲中
        self.__poc_play_status = False      # 默认不处于播放中
        self.lcd_sleep_time = 15 # 息屏时间(s)
        self.lcd_sleep_timer = osTimer()

    def start(self):
        EventMap.bind("load_screen", self.load_screen)      # 加载屏幕
        EventMap.bind("load_msgbox", self.load_msgbox)      # 加载消息框
        EventMap.bind("close_msgbox", self.close_msgbox)    # 关闭消息框
        EventMap.bind("poc_play_status", self.poc_play_status) 
        EventMap.bind("ppt_press", self.ppt_press)
        EventMap.bind("ppt_release", self.ppt_release)
        EventMap.bind("key1_once_click", self.key1_once_click)
        EventMap.bind("key2_once_click", self.key2_once_click)
        EventMap.bind("key2_double_click", self.key2_double_click)
        EventMap.bind("key2_long_press", self.key2_long_press)
        EventMap.bind("lcd_state_manage", self.lcd_sleep_enable)

        for bar in self.bar_list: bar.instance_after()
        for box in self.msgbox_list: box.instance_after()
        for src in self.screen_list: src.instance_after()

        EventMap.send("load_screen", {"screen": WelcomeScreen.NAME})
        #EventMap.send("load_screen", {"screen": MainScreen.NAME})
        
        PrintLog.log("PocUI", "UI load finished.")
        self.lcd_sleep_enable()
        
    def load_msgbox(self, event, msg):
        """
        加载消息框, 注意msg的格式:
        
        {
            "type": "promptbox", # 默认提示框
            "title": "[promptbox]"
            "msg": "hello world",
            "mode": 0
        }
        """
        if isinstance(msg, dict):
            _type = msg.get("type", PromptBox.NAME) # 默认提示框
            _type = "{}__show".format(type.lower())
            _msg = {
                "meta":self.curr_screen.meta,
                "msg": msg.get("msg", "[promptbox]"),
                "mode": msg.get("mode", 0)
            }
            EventMap.send(_type, _msg)
        else:
            _msg = {
                "meta":self.curr_screen.meta,
                "title": "[promptbox]",
                "msg": msg,
                "mode": 0
            }
            EventMap.send("promptbox__show", _msg)

    def close_msgbox(self, event, msg):
        """
        在这里对所有消息框发送关闭消息
        """
        EventMap.send("promptbox__close")

    def load_screen(self, event, msg):
        """
        加载UI屏幕
        """
        for scr in self.screen_list:
            if scr.NAME != msg["screen"]:
                continue
            if self.curr_screen:
                if scr.NAME != self.curr_screen.NAME:
                    scr.set_last_screen(self.curr_screen.NAME)
                self.curr_screen.deactivate()  #清理操作
            self.curr_screen = scr
            
            PrintLog.log("PocUI", "load screen:{}".format(scr.NAME))

            # 加载屏幕之前先加载屏幕栏
            if self.curr_screen.NAME != "WelcomeScreen":
                EventMap.send("menubar__show", self.curr_screen.meta)

            scr.load_before()
            scr.load()
            scr.load_after()
            lv.img.cache_invalidate_src(None)
            lv.img.cache_set_size(8)
            lv.scr_load(self.curr_screen.meta) # load lvgl meta object

    def add_bar(self, bar):
        self.bar_list.append(bar)
        return self
    def add_msgbox(self, msgbox):
        self.msgbox_list.append(msgbox)
        return self

    def add_screen(self, screen):
        self.screen_list.append(screen)
        return self

    def poc_play_status(self, event, play_staus):
        """
        poc播放状态
        """
        self.__poc_play_status = play_staus
        self.lcd_sleep_timer.stop()
        g_lcd.display_on()

        # 恢复自动息屏
        if not self.__poc_play_status: 
            self.lcd_sleep_enable()

    def lcd_sleep_enable(self, bol=True):
        """
        LCD睡眠使能

        :param bol: 是否允许自动息屏
        """
        self.lcd_sleep_timer.stop()
        if bol:
            self.lcd.display_on()
            PrintLog.log("PocUI", "lcd exit sleep.")
            self.lcd_sleep_timer.start(self.lcd_sleep_time * 1000, 1, lambda arg: self.lcd_sleep_enable(0))   # 10s之后自动息屏
        else:
            if self.__poc_speak_status or self.__poc_play_status:
                PrintLog.log("PocUI", " poc speaking... can't sleep!")
                return
            self.lcd.display_off()
            PrintLog.log("PocUI", "lcd enter sleep.")

    def ppt_press(self, event, msg):
        """
        ptt 长按抢麦
        """
        self.lcd_sleep_enable()  # 不允许黑屏       
        EventMap.send("pocservice__call_member_status", 1)
        self.__poc_speak_status = EventMap.send("pocservice__speaker_enable", 1)  # 开启对讲

    def ppt_release(self, event, msg):
        """
        ptt 抬起
        """        
        EventMap.send("pocservice__speaker_enable", 0)
        EventMap.send("pocservice__call_member_status", 0)


    def key1_once_click(self, event, msg):
        self.lcd_sleep_enable()
        self.curr_screen.key1_once_click()

    def key2_once_click(self, event, msg):
        self.lcd_sleep_enable()
        self.curr_screen.key2_once_click()

    def key2_double_click(self, event, msg):
        self.lcd_sleep_enable()
        self.curr_screen.key2_double_click()

    def key2_long_press(self, event, msg):
        # from misc import Power
        # Power.powerDown()
        self.lcd_sleep_enable()
        self.curr_screen.key2_long_press()

