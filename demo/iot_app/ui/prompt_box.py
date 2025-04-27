# ui/promptbox.py
import lvgl as lv
try:
    from ui.styles import FontStyle
    from common import EventMap, AbstractLoad
except:
    from usr.ui.styles import FontStyle
    from usr.common import EventMap, AbstractLoad

#-------------------------------------------------------------------------------
# 消息框
class PromptBox(AbstractLoad):
    NAME = "PromptBox"

    def __init__(self):        
        self.prompt_box = None
        self.prompt_label = None

    def instance_after(self):
        EventMap.bind("promptbox__show", self.__show)
        EventMap.bind("promptbox__close", self.__close)

    def __close(self, event=None, msg=None):
        if self.prompt_box is not None:
            self.prompt_box.delete()
            self.prompt_box = None

    def __show(self, event, msg):
        if self.prompt_box is not None:
            self.prompt_box.delete()
            self.prompt_box = None

        meta = msg.get("meta")  #获取lv.obj meta界面对象
        show_msg = msg.get("msg")

        self.prompt_box = lv.msgbox(meta, "PromptBox", "", [], False)
        self.prompt_box.set_size(180, 90)
        self.prompt_box.align(lv.ALIGN.CENTER, 0, 0)
        self.prompt_label = lv.label(self.prompt_box)
        self.prompt_label.set_pos(0, 0)
        self.prompt_label.set_size(140, 50)
        self.prompt_label.add_style(FontStyle.consolas_12_txt000000_bg2195f6, lv.PART.MAIN | lv.STATE.DEFAULT)
        self.prompt_label.set_text(show_msg)
        self.prompt_label.set_long_mode(lv.label.LONG.WRAP)
        self.prompt_label.set_style_text_align(lv.TEXT_ALIGN.CENTER, 0)