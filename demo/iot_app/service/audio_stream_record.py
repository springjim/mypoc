# -*- coding: UTF-8 -*-
import utime
import audio
from machine import Pin
import _thread
from queue import Queue

try:
    from service.messagebuilder import MessageBuilder
    from service.messagetype import MessageType
    from common import AbstractLoad, EventMap, PrintLog
    from service.socket_client import SingletonSocket
    
    
except:
    from usr.service.messagebuilder import MessageBuilder
    from usr.service.messagetype import MessageType
    from usr.common import AbstractLoad, EventMap, PrintLog 
    from usr.service.socket_client import SingletonSocket
   


class AudioStreamRecorder:
    NAME="AudioStreamRecorder"
    _instance = None  # 单例实例
    _clslock = _thread.allocate_lock()  # 线程锁，防止多线程竞争
    BUFFER_SIZE = 1024  # 缓冲区大小，单位字节

    def __new__(cls):
        """单例模式实现"""
        if cls._instance is None:
            with cls._clslock:
                if cls._instance is None:
                    cls._instance = super().__new__(cls)
                    cls._instance._initialized = False
        return cls._instance
    
    def __init__(self):
        """初始化录音设备"""
        if not self._initialized:
            self.socket_client = SingletonSocket()  #创建socket对象
            self.sample_rate = 8000
            self.recorder = audio.Record()
            self.is_recording = False  # 控制录音开关
            self._initialized = True
            self._lock = _thread.allocate_lock()  # 线程锁，防止多线程竞争
            self.read_buf = bytearray(AudioStreamRecorder.BUFFER_SIZE)

            self.audio_queue = Queue(100)  # 缓存100条音频数据
            # 启动数据处理线程
            _thread.start_new_thread(self.audio_record_thread, ())


    def audio_record_thread(self):
        """数据处理线程（将数据切割成13字节/段并发送）"""
        # 切割成13字节/段（模拟协议分包）
        chunk_size = 13
        while True:
            try:
                audio_data = self.audio_queue.get()
                if audio_data and len(audio_data) > 0:
                     for i in range(0, len(audio_data), chunk_size):
                        chunk = audio_data[i:i+chunk_size]
                        # TCP发送
                        self.socket_client.send_message(MessageBuilder.build_audiomedia_message(chunk))
                        #print("发送语音包")  #不能打印，影响发送延迟
            except Exception as e:
                PrintLog.log(AudioStreamRecorder.NAME, "Audio play error: {}".format(e))
                break                    
            
            #utime.sleep_ms(2)  # 避免CPU跑满


    def stop_streaming(self):
        """停止录音（线程安全 + 硬件中断）"""  
        try:
            self.is_recording = False
            #self.recorder.stop()  # 确保硬件停止
        except Exception as e:
            print("stop_streaming异常:", e)
                     
 

    def start_streaming(self):
        """
        启动语音流采集(AMR-NB格式,8000Hz)
        """       
        
        self.is_recording = True
        
        try:
            print("启动语音采集")         
            self.recorder.stream_start( self.recorder.AMRNB,  8000,  30 ) #每次采集2秒                
        
            # 读取音频数据（每秒约13*50=650字节，AMR-NB每20ms帧约32字节）
             
            while self.is_recording:
                audio_data_len = self.recorder.stream_read(self.read_buf, AudioStreamRecorder.BUFFER_SIZE)
                if audio_data_len>0:
                    self.audio_queue.put(self.read_buf[:audio_data_len])  # 存入队列
                    print("采集到的语音包长度:%d" % audio_data_len)  #都是20ms一帧, 13个字节                                   

            
            print("采集停止")
            self.recorder.stop()                 

        except Exception as e:
            print("采集异常:", e)
            self.recorder.stop()                

        
        



