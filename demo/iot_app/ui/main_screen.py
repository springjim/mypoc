# screens/main_screen.py
import lvgl as lv

try:
    from ui.styles import CommonStyle, MainScreenStyle, LVGLColor
    from common import EventMap
    from ui.screen import Screen
except:
    from usr.ui.styles import CommonStyle, MainScreenStyle, LVGLColor
    from usr.common import EventMap
    from usr.ui.screen import Screen

class MainScreen(Screen):
    NAME = "MainScreen"

    def __init__(self):
        
        self.meta = lv.obj()    # lvgl meta object
        self.btn_list = []  #按钮列表: 元素为元组
        #self.btn_list_name = ["群组管理", "成员列表", "设备信息", "天气", "设置"]
        self.btn_list_name = ["强哥管理来", "成员列表", "设备信息", "天气", "设置"]
        self.curr_idx = 0
        self.count = len(self.btn_list_name)

    def load(self):
        self.meta.add_style(CommonStyle.default, lv.PART.MAIN | lv.STATE.DEFAULT)
        # 列表------------------------------------------------------------------------------------------
        self.list_menu = lv.list(self.meta)
        self.list_menu.set_pos(0, 40)
        self.list_menu.set_size(240, 200)
        self.list_menu.set_style_pad_left(0, 0)
        self.list_menu.set_style_pad_right(0, 0)
        self.list_menu.set_style_pad_top(0, 0)
        self.list_menu.set_style_pad_row(1, 0)
        #列表初始化样式：设置背景和默认状态下样式
        self.list_menu.add_style(CommonStyle.container_bgffffff, lv.PART.MAIN | lv.STATE.DEFAULT)
        #列表初始化样式：设置滚动条和默认状态下样式
        self.list_menu.add_style(MainScreenStyle.list_scrollbar, lv.PART.SCROLLBAR | lv.STATE.DEFAULT)
        #列表初始化样式：设置滚动条和正在滚动下样式
        self.list_menu.add_style(MainScreenStyle.list_scrollbar, lv.PART.SCROLLBAR | lv.STATE.SCROLLED)
        
        # 添加管理列表------------------------------------------------------------------------------------------
        self.btn_list = []
        for idx, item in enumerate(self.btn_list_name):
            btn = lv.btn(self.list_menu)
            btn.set_pos(20, 0)
            btn.set_size(240, 47)
            btn.add_style(MainScreenStyle.btn_group, lv.PART.MAIN | lv.STATE.DEFAULT)
            img = lv.img(btn)
            img.align(lv.ALIGN.LEFT_MID, 10, 0)  #图像在按钮的左侧中间
            img.set_size(32, 32)
            img.set_src('U:/img/main_list_{}.png'.format(idx + 1))
            lab = lv.label(btn)
            lab.align(lv.ALIGN.LEFT_MID, 50, 13)
            lab.set_size(210, 40)
            lab.set_text(item)
            self.btn_list.append((btn, img, lab))
        self.add_state()
        
    def load_after(self):
        pass    

    def add_state(self):
        currBtn = self.list_menu.get_child(self.curr_idx)
        currBtn.set_style_bg_color(lv.color_make(0xe6, 0x94, 0x10), lv.PART.MAIN | lv.STATE.DEFAULT)
        currBtn.set_style_bg_grad_color(lv.color_make(0xe6, 0x94, 0x10), lv.PART.MAIN | lv.STATE.DEFAULT)
        #如果标签文本过长，超出标签宽度，文本会自动循环滚动显示
        self.btn_list[self.curr_idx][2].set_long_mode(lv.label.LONG.SCROLL_CIRCULAR)
        #当前按钮滚动到可见区域，禁用滚动动画，直接滚动到目标位置
        currBtn.scroll_to_view(lv.ANIM.OFF)
        
    def clear_state(self):
        currBtn = self.list_menu.get_child(self.curr_idx)
        currBtn.set_style_bg_color(LVGLColor.BASE_COLOR_WHITE, lv.PART.MAIN | lv.STATE.DEFAULT)
        currBtn.set_style_bg_grad_color(LVGLColor.BASE_COLOR_WHITE, lv.PART.MAIN | lv.STATE.DEFAULT)
        self.btn_list[self.curr_idx][2].set_long_mode(lv.label.LONG.SCROLL_CIRCULAR)
        currBtn.scroll_to_view(lv.ANIM.OFF)

    def key2_once_click(self):
        """
        作为滚动按键
        """
        self.clear_state()
        self.curr_idx = self.next_idx(self.curr_idx, self.count)
        self.add_state()

    def key2_double_click(self):
        if self.curr_idx == 0:
            screen = "GroupScreen"
        elif self.curr_idx == 1:
            screen = "MemberScreen"
        elif self.curr_idx == 2:
            screen = "DeviceScreen"
        elif self.curr_idx == 3:
            screen = "WeatherScreen"
        elif self.curr_idx == 4:
            screen = "SettingScreen"
        else:
            return
        EventMap.send("load_screen",{"screen": screen})