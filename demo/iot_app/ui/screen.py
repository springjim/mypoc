# screen.py
try:
    from common import AbstractLoad
except:
    from usr.common import AbstractLoad

class Screen(AbstractLoad):
    class Type():
        Init = "init"
        Normal = "normal"   # 默认
        MenuBar = "menubar"
        ToolBar = "toolbar"
        StatusBar = "statusbar"

    def __init__(self):
        self.meta = None    # lvgl meta object
        self.meta_info = {}
        self.last_screen = None

    def set_last_screen(self, name):
        self.last_screen = name

    def load_before(self):
        pass

    def load(self):
        pass

    def load_after(self):
        pass

    def instance_after(self):
        pass

    def deactivate(self):
        pass

    def key2_once_click(self):
        pass

    def key2_double_click(self):
        pass

    def key2_long_press(self):
        pass   

    def prev_idx(self, now_idx, count):
        cur_idx = now_idx - 1
        if cur_idx < 0:
            cur_idx = count - 1
        return cur_idx
    def next_idx(self, now_idx, count):
        cur_idx = now_idx + 1
        if cur_idx > count - 1:
            cur_idx = 0
        return cur_idx

