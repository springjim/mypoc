# 导入usocket模块
# -*- coding: UTF-8 -*-
import usocket
import log
import utime
import checkNet
import osTimer
import _thread
import ustruct
from queue import Queue
import audio
from machine import Pin

try:
    from service.messagebuilder import MessageBuilder
    from service.messagetype import MessageType
    from common import  EventMap, PrintLog
except:
    from usr.service.messagebuilder import MessageBuilder
    from usr.service.messagetype import MessageType
    from usr.common import  EventMap, PrintLog    


#采用单例模式
class SingletonSocket:
    NAME = "SingletonSocket"
    _instance = None
    _initialized = False

    # 4. 处理特殊消息类型(length=0)
    SPECIAL_MESSAGE_TYPES  =frozenset([
                MessageType.MESSAGE_MEDIA_EX,
                MessageType.MESSAGE_MEDIA_EX_FILE_FRAME,
                MessageType.MESSAGE_SOS_MEDIA_EX,

                MessageType.MESSAGE_PTT2POC_START_EX,
                MessageType.MESSAGE_PTT2POC_END_EX,

                MessageType.MESSAGE_P2G_CHATMESSAGES,
                MessageType.MESSAGE_P2G_FILEMESSAGES,
                MessageType.MESSAGE_P2P_CHATMESSAGES,
                MessageType.MESSAGE_P2P_FILEMESSAGES,

                MessageType.MESSAGE_SOS_LOCATION,
                MessageType.MESSAGE_OFFLINE_USER_RECORD,
                MessageType.MESSAGE_PTT_POC_START,
                MessageType.MESSAGE_PTT_POC_STOP,
                MessageType.MESSAGE_CREATE_GROUP,

                MessageType.MESSAGE_GPS_COMMAND,
                MessageType.MESSAGE_SHARE_VIDEOLIVE,
                MessageType.MESSAGE_RELAY_USER_MESSAGE,
                MessageType.MESSAGE_VIDEO_MESSAGE,

                MessageType.MESSAGE_GROUP_USER_CHANGE,
                MessageType.MESSAGE_AV_CHAT_NEW,
                MessageType.MESSAGE_AV_REMOTE_MONI,
                MessageType.MESSAGE_USER_RTSTATE_UPDATE,
                MessageType.MESSAGE_MEET_CHAT,
                MessageType.MESSAGE_MEET_SCREEN_SHARE,
                MessageType.MESSAGE_SOS_SESSION
            ])
    
    def __new__(cls, *args, **kwargs):
        #确保单例对象
        if not cls._instance:
            cls._instance = super(SingletonSocket, cls).__new__(cls)
        return cls._instance
    
    def __init__(self):
         if not self._initialized:
            # mypoc的socket服务配置，将来以下配置用固件烧录写入
            self.SERVER_IP = '159.75.230.229'
            self.SERVER_PORT = 17001
            
            # 全局变量，用于控制接收线程的运行
            self.receive_thread_running = True
            # 全局变量，用于控制子线程的运行
            self.timer_thread_running = True
            
            self.audio_queue= Queue(100)  #队列最大长度, int类型, 默认长度是100
            self.audio_thread_running = True
            self.audio_playing = False
            
            # 启动音频播放线程
            _thread.start_new_thread(self.audio_play_thread, ())
            
            # 定义消息头长度
            self.HEADER_LENGTH = 3
            
            # 初始化状态标志完成
            self._initialized = True
            self.is_connected = False
            self.sock = None
            self.buffer = bytearray()  # 动态字节数组
            
    def audio_play_thread(self):
        """音频播放线程，从队列中取出数据并播放"""
        
        aud = audio.Audio(0)
        print(aud.setVolume(11))
        print("audio初始化状:"+str(aud.getState()))
        
        # 设置pa 功放
        p1 = Pin(Pin.GPIO1, Pin.OUT, Pin.PULL_DISABLE, 0)
        aud.set_pa(Pin.GPIO1,4)
        
        #初始化 GPIO38 引脚，用于控制噪声消除功能的开关
        noise_reduction_switch = Pin(Pin.GPIO38, Pin.OUT, Pin.PULL_DISABLE, 0)
        noise_reduction_switch.write(1) #设置当前PIN脚输出高电平,打开噪声消除
        # 存储累积的音频数据
        accumulated_data = bytearray()
        
        while self.audio_thread_running:
            try:
                # 从队列获取音频数据
                audio_data = self.audio_queue.get()  # 阻塞获取
                
                # 播放音频流 (AMR-NB格式通常使用模式4)
                if audio_data and len(audio_data) > 0:
                    # 累积数据
                    accumulated_data += audio_data
                    # 如果累积了20条消息，播放效果刚刚好
                    if len(accumulated_data) >= 20 * len(audio_data):  # 假设每条消息长度相近
                        #print("播放累积的音频数据（长度：{}）".format(len(accumulated_data)))
                        ret = aud.playStream(4, accumulated_data)
                        #print("播放结果:", ret)  #如果返回 -1表示播放失败
                        
                        # 清空累积的数据
                        accumulated_data = bytearray()
                   
            
            except Exception as e:
                PrintLog.log(SingletonSocket.NAME, "Audio play error: {}".format(e))
                break
    
        PrintLog.log(SingletonSocket.NAME, "Audio play thread exited")
            
       
    def connect(self):
        """连接tcp服务器 """
        if self.is_connected:
            PrintLog.log(SingletonSocket.NAME, "Already connected to server")            
            return True
        
        try:
            #创建一个socket实例
            self.sock = usocket.socket(usocket.AF_INET, usocket.SOCK_STREAM)
            self.sock.connect((self.SERVER_IP, self.SERVER_PORT))
            PrintLog.log(SingletonSocket.NAME,"Connected to server {}:{}".format(self.SERVER_IP, self.SERVER_PORT))
            
            #启动接收数据的线程
            _thread.start_new_thread(self.receive_data_thread, (self.sock, self.buffer))
            
            # 启动心跳线程
            _thread.start_new_thread(self.timer_callback, (20000, self.send_heartbeat, self.sock))
            self.is_connected = True
            return True
        except Exception as e:
            PrintLog.log(SingletonSocket.NAME,"Connection error: {}".format(e))
            self.close()
            return False
        
    def send_applymic(self):
        """
        申请麦权信令
        """
        if not self.is_connected:
            PrintLog.log(SingletonSocket.NAME,"Socket is not connected")
            return False

        try:
            message = MessageBuilder.build_applymic_message()
            self.sock.send(message)
            PrintLog.log(SingletonSocket.NAME,"Sent applymic message: {}".format(' '.join(['%02x' % b for b in message])))
            return True
        except Exception as e:
            PrintLog.log(SingletonSocket.NAME,"Error sending applymic: {}".format(e))
            return False
        
    
    def send_releasemic(self):
        """
        释放麦权信令
        """
        if not self.is_connected:
            PrintLog.log(SingletonSocket.NAME,"Socket is not connected")
            return False

        try:
            message = MessageBuilder.build_releasemic_message()
            self.sock.send(message)
            PrintLog.log(SingletonSocket.NAME,"Sent releasemic message: {}".format(' '.join(['%02x' % b for b in message])))
            return True
        except Exception as e:
            PrintLog.log(SingletonSocket.NAME,"Error sending releasemic: {}".format(e))
            return False

        

        
    def send_report(self, group_id, user_id):
        """发送工作组消息"""
        if not self.is_connected:
            PrintLog.log(SingletonSocket.NAME,"Socket is not connected")
            return False
        
        PrintLog.log(SingletonSocket.NAME,"group_id={}".format(group_id))
            
        try:
            message = MessageBuilder.build_report_message(group_id, user_id)
            self.sock.send(message)
            PrintLog.log(SingletonSocket.NAME,"Sent report message: {}".format(' '.join(['%02x' % b for b in message])))
            return True
        except Exception as e:
            PrintLog.log(SingletonSocket.NAME,"Error sending report: {}".format(e))
            return False
    
    def close(self):
        """关闭tcp连接"""
        self.receive_thread_running = False
        self.timer_thread_running = False
        self.audio_thread_running = False
        
        # 清空音频队列
        while not self.audio_queue.empty():
            try:
                self.audio_queue.get()
            except:
                break
        
        if self.sock:
            try:
                self.sock.close()
            except:
                pass
            self.sock = None
            
        self.is_connected = False
        PrintLog.log(SingletonSocket.NAME,"Socket connection closed")
    
    
    def timer_callback(self, interval, callback, sock):
        """定时器回调"""
        while self.timer_thread_running:
            try:
                # 调用回调函数
                callback(sock)
            except Exception as e:
                PrintLog.log(SingletonSocket.NAME,"Timer callback error: {}".format(e))
            utime.sleep_ms(interval)
            
    def send_message(self,message):
        if not self.is_connected:
            PrintLog.log(SingletonSocket.NAME,"Socket is not connected")
            return False

        try:             
            self.sock.send(message)
            #PrintLog.log(SingletonSocket.NAME,"Sent message: {}".format(' '.join(['%02x' % b for b in message])))
            return True
        except Exception as e:
            PrintLog.log(SingletonSocket.NAME,"Error sending message: {}".format(e))
            return False
        
            
    def send_heartbeat(self, sock):
        """发送心跳"""
        try:
            PrintLog.log(SingletonSocket.NAME,"Sending heartbeat")
            message = MessageBuilder.build_hearbeat_message()
            sock.send(message)
            PrintLog.log(SingletonSocket.NAME,"Sent heartbeat: {}".format(' '.join(['%02x' % b for b in message])))
        except Exception as e:
            PrintLog.log(SingletonSocket.NAME,"Error sending heartbeat: {}".format(e))
            self.close()
            
            
    def parse_message(self, data):
        """
        解析消息
        :param data: 完整的消息数据（包括头部和负载）
        :return: messageid, zero, msglen, payload
        """
        try:
            messageid = ustruct.unpack('>H', data[0:2])[0]  # 前2个字节是messageid，按大端序解析
            zero = data[2]  # 第3个字节是0
            msglen = ustruct.unpack('>H', data[3:5])[0]  # 第4、5个字节是msglen，按大端序解析
            payload = data[5:5 + msglen]  # 从第6个字节开始是负载数据
            return messageid, zero, msglen, payload
        except Exception as e:
            PrintLog.log(SingletonSocket.NAME, "Error parsing message: {}".format(e))
            return None
        
        
    def receive_data_thread(self, sock, buffer):
        """
        从缓冲区中读取并解析一个完整的消息
        :param sock: socket 对象
        :param buffer: 缓冲区，存储未处理的数据
        :return: 如果解析成功，返回 (messageid, zero, msglen, payload)，并更新缓冲区；否则返回 None
        """
        MIN_LENGTH = 3  # 最小消息头长度
        AUDIO_MESSAGE_ID = 99  # 语音消息的ID
        MAX_BUFFER_SIZE = 2048  # 8KB缓冲区上限
        RECV_CHUNK_SIZE = 1024     # 单次recv最大字节数
        
        while self.receive_thread_running:

            try:

                buffer.extend(sock.recv(RECV_CHUNK_SIZE))

                while(self.receive_thread_running):

                    # 检查缓冲区中是否有足够的数据解析消息头
                    #print("重头分析...len={}".format(len(buffer)))
                    if len(buffer) < MIN_LENGTH:
                        chunk = sock.recv(min(RECV_CHUNK_SIZE, MAX_BUFFER_SIZE - len(buffer)))
                        if not chunk :
                            PrintLog.log(SingletonSocket.NAME,"Connection closed by server")
                            self.close()
                            break
                        buffer.extend(chunk)
                        continue

                    # 2. 解析消息头（直接通过 bytes 索引访问)                
                    message_id = (buffer[0] << 8) | buffer[1]  # 比ustruct.unpack更快
                    length= buffer[2]  #第3个字节
                    print("message_id=%d,length=%d" % (message_id,length))
                
                    # 3. 计算当前消息完整长度
                    if message_id in self.SPECIAL_MESSAGE_TYPES:
                        if len(buffer) < 5:
                            print("不够5个")
                            buffer.extend(sock.recv(min(RECV_CHUNK_SIZE, MAX_BUFFER_SIZE - len(buffer))))
                            continue
                        payload_len = (buffer[3] << 8) | buffer[4]
                        total_len = 3 + payload_len  #注意, payload_len本身包含了msglen长度的2个字节
                        #print("SPECIAL_MESSAGE_TYPES total_len={}".format(total_len))
                    else:
                        payload_len = length
                        total_len = 3 + length
                        #print("non SPECIAL_MESSAGE_TYPES total_len={}".format(total_len))
                    
                    # 4. 检查是否有完整包
                    if len(buffer) < total_len:
                        print("len(buffer)={},total_len={}".format(len(buffer),total_len))
                        print("长度不够")
                        #need_bytes = total_len - len(buffer)
                        buffer.extend(sock.recv(min(RECV_CHUNK_SIZE, MAX_BUFFER_SIZE - len(buffer))))
                        continue
                
                    # 5. 处理单个消息包
                    single_message = bytes(buffer[:total_len])
                    buffer = buffer[total_len:]  # 原子化移除已处理数据
                    if message_id== AUDIO_MESSAGE_ID:
                        #restbuffer = "empty" if not buffer else "".join("%02x " % b for b in buffer).strip()
                        #print("发现语音包 ID=99, 长度=%d" % len(single_message[5:]))
                        self.audio_queue.put(single_message[5:])
                        #print("余下包:%s"%(restbuffer,))
                        self.handle_message(message_id, length, payload_len, single_message[5:])
                    
                    else:
                        if message_id in self.SPECIAL_MESSAGE_TYPES:
                            self.handle_message(message_id, length, payload_len, single_message[5:])
                        else:
                            self.handle_message(message_id, length, length, single_message[3:])
                        
                    # 5. 缓冲区维护（三重保护机制）
                    if len(buffer) > MAX_BUFFER_SIZE:
                        PrintLog.log(SingletonSocket.NAME, "剩余内存太多,清空了") 
                        buffer = bytearray()
                        continue
                    else:
                        #print("余下长度：{}, 字节={}".format(len(buffer), buffer))
                        utime.sleep_ms(1)
                        continue
                                                                  
                
                
            except Exception as e:
                PrintLog.log(SingletonSocket.NAME,"Error in receive thread: {}".format(e))
                self.close()
                break
        
        PrintLog.log(SingletonSocket.NAME,"Receive thread exited")
        
        
    def handle_message(self, messageid, zero, msglen, payload):
        """处理接收到的消息"""

        #处理各种收到消息
        if messageid==MessageType.MESSAGE_APPLY_MIC_SUCCESS:
            #抢麦成功
            print("抢麦成功")
            EventMap.send("pocservice_rob_applymic_state",True)
            
        elif messageid==MessageType.MESSAGE_APPLY_MIC_FAILED:
            #抢麦失败
            print("抢麦失败")
            EventMap.send("pocservice_rob_applymic_state",False)
        
        elif messageid==MessageType.MESSAGE_MEDIA_EX:
            #语音包不用打印，会影响播放效率，延时很大
            pass            
        else:
            payload_str = "empty" if not payload else "".join("%02x " % b for b in payload).strip()
            log_msg = "Received message - ID: %d, Zero: %d, Length: %d, Payload: %s" % \
              (messageid, zero, msglen, payload_str)
            PrintLog.log(SingletonSocket.NAME, log_msg )
            # 在这里添加你的消息处理逻辑

    
        
        
    