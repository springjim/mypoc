import osTimer
from machine import ExtInt

try:
    from common import EventMap
except:
    from usr.common import EventMap


class KeyManger(object):

    def __init__(self):
        
        # key1作为PPT按键
        self.__key1_long_timer = osTimer()
        self.__key1_long_timer_flag = False

        self.__key2_count = 0
        self.__key2_long_timer = osTimer()
        self.__key2_double_timer = osTimer()
        self.__key2_double_timer_flag = False
        self.__key2_long_timer_flag = False
        
        #外部中断, 要查具体开发板上的引脚
        self.key1 = ExtInt(ExtInt.GPIO13, ExtInt.IRQ_RISING_FALLING, ExtInt.PULL_PU, self.__key1_event_handler)
        self.key1.enable()   # key1

        self.key2 = ExtInt(ExtInt.GPIO12, ExtInt.IRQ_RISING_FALLING, ExtInt.PULL_PU, self.__key2_event_handler)
        self.key2.enable()   # key2

    def __key1_event_handler(self, event):
        if event[1] == 1:
            self.__key1_press_handle()
        else:
            self.__key1_up_handle()

    def __key1_press_handle(self):
        self.__key1_long_timer.start(500, 0, self.__key1_long_handle)

    def __key1_long_handle(self, arg):
        self.__key1_long_timer_flag = True  # key1键 长按标志
        EventMap.send("ppt_press")

    def __key1_up_handle(self):
        self.__key1_long_timer.stop()

        if self.__key1_long_timer_flag:
            self.__key1_long_timer_flag = False
            EventMap.send("ppt_release")
            return
        # EventMap.send("key1_once_click")

    def __key2_event_handler(self, event):
        if event[1] == 1:
            self.__key2_press_handle()
        else:
            self.__key2_up_handle()

    def __key2_press_handle(self):
        self.__key2_long_timer.start(1500, 0, self.__key2_long_handle)

    def __key2_long_handle(self, arg):
        self.__key2_long_timer_flag = True  # key2键 长按标志
        EventMap.send("key2_long_press")

    def __key2_up_handle(self):
        """key2键 抬起"""
        self.__key2_long_timer.stop()

        if self.__key2_long_timer_flag:
            self.__key2_long_timer_flag = False
            # EventMap.send("key2_long_press_release")
            return
        self.__key2_count += 1

        # 判断是否准备双击
        if not self.__key2_double_timer_flag:   
            self.__key2_double_timer_flag = True
            self.__key2_double_timer.start(300, 0, self.__key2_up_timer)

    def __key2_up_timer(self, args):
        if 2 <= self.__key2_count:
            EventMap.send("key2_double_click")
        else:
            EventMap.send("key2_once_click")
        self.__key2_count = 0
        self.__key2_double_timer_flag = False
    

