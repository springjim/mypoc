import audio
import queue
from machine import Pin, ExtInt
try:
    from common import AbstractLoad, EventMap, PrintLog
except:
    from usr.common import AbstractLoad, EventMap, PrintLog

class MediaService(AbstractLoad):
    """
    媒体服务
    """
    def __init__(self):
        self.aud = audio.Audio(0)  # 0听筒 1耳机 2喇叭, 当前开发板只支持听筒，请查找模组型号，寻找支持喇叭的
        self.tts = audio.TTS(0) # 初始化 tts ,0表示听筒，1表示耳机，2表示喇叭, 当前开发板只支持听筒，请查找模组型号，寻找支持喇叭的
        self.tts.setVolume(9) #tts播放音量大小，音量值应在区间[0 ~ 9]，0表示静音
        self.q = queue.Queue()
        
        #初始化外部中断，用于检测麦克风的状态
        #GPIO14 是麦克风检测的引脚,self.__mic 是中断回调函数
        self.mic_det = ExtInt(ExtInt.GPIO14, ExtInt.IRQ_RISING_FALLING, ExtInt.PULL_PU, self.__mic)
        #启用麦克风检测中断
        self.mic_det.enable()
        #初始化 GPIO1 引脚，用于控制音频功放 (PA, Power Amplifier)
        self.p1 = Pin(Pin.GPIO1, Pin.OUT, Pin.PULL_DISABLE, 0)
        self.aud.set_pa(Pin.GPIO1, 4) #作用是将指定的 GPIO 引脚与音频功放关联，并设置功放的增益（放大倍数）
        self.mute_value = 0
        self.mute = 0
        #初始化 GPIO38 引脚，用于控制噪声消除功能的开关
        self.noise_reduction_switch = Pin(Pin.GPIO38, Pin.OUT, Pin.PULL_DISABLE, 0)

    def __mic(self, args):
        # 角发沿，1：下降沿
        if args[1]:
            EventMap.send("update_ej_img", 0)
        else:
            EventMap.send("update_ej_img", 1)

    def instance_after(self):
        EventMap.bind("mediaservice__noise_reduction_enable", self.__noise_reduction_enable)
        EventMap.bind("mediaservice__get_mic_det_state", self.__get_mic_det_state)
        EventMap.bind("mediaservice__audio_tone", self.__audio_tone)
        EventMap.bind("mediaservice__beep_tone", self.__beep_tone)
        EventMap.bind("mediaservice__tts_play", self.__tts_play)
        EventMap.bind("mediaservice__tts_stop", self.__tts_stop)
        #poc.set_vol(1, 8)  # 设置 tts的音量为8
        #poc.set_vol(2, 1)  # 设置tone的音量为1

    def __noise_reduction_enable(self, event, msg):
        if msg:
            self.noise_reduction_switch.write(1) #设置当前PIN脚输出高电平,打开噪声消除
        else:
            self.noise_reduction_switch.write(0) #设置当前PIN脚输出低电平,关闭噪声消除

    def __beep_tone(self, event, msg):
        if self.q.empty():
            self.q.put(None)

    def __audio_tone(self, event, msg):
        # 16是拨号音, 播放100ms
        self.aud.aud_tone_play(16, 100)

    def __get_mic_det_state(self, event=None, msg=None):
        #读取当前引脚电平
        return self.mic_det.read_level() 

    def __tts_play(self, event=None, msg=None):
        if msg[0][0] == '0':
            self.tts.play(4, msg[1], 2, '[n1]' + msg[0])
        else:
            pass
            #poc.play_tts(msg[0], msg[1])   #poc服务的tts播放

    def __tts_stop(self, event, msg):
        self.tts.stop()     
