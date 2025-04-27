import ustruct

class MessageBuilder:
    
    @staticmethod
    def build_hearbeat_message():
        """
        构建一个 heartbeat 消息       
        :return: bytes，符合格式的消息
        """
        
        # 消息格式：
        # - 前2字节：messageID (大端序，值为4)
        # - 第3字节：值为0
        
        # 使用 ustruct.pack 打包数据
        message = ustruct.pack(
            '>HB',  # 格式字符串：< 表示大端序，H=2字节无符号，B=1字节无符号，I=4字节无符号，I=4字节无符号
            4,       # messageID (2字节，值为4)
            0       # 第3字节 (1字节，值为0)           
        )        

        return message
    
    @staticmethod
    def build_applymic_message():
        """
        构建一个抢麦apply_mic消息
        """
        # 使用 ustruct.pack 打包数据
        message = ustruct.pack(
            '>HB',  # 格式字符串：> 表示大端序，H=2字节无符号，B=1字节无符号，I=4字节无符号，I=4字节无符号
            10,       # messageID (2字节，值为1)
            0       # 第3字节 (1字节，值为0)            
        )
        

        return message
    
    @staticmethod
    def build_releasemic_message():
        """
        构建一个释放麦release_mic消息
        """
        # 使用 ustruct.pack 打包数据
        message = ustruct.pack(
            '>HB',  # 格式字符串：> 表示大端序，H=2字节无符号，B=1字节无符号，I=4字节无符号，I=4字节无符号
            13,       # messageID (2字节，值为1)
            0       # 第3字节 (1字节，值为0)            
        )
        

        return message
    
    @staticmethod
    def build_audiomedia_message(framedata):
        """
        构建一个 语音帧 消息
        :param framedata: 字节数组，13个字节(按8000，amrnb) 
        :return: bytes，符合格式的消息
        """
        # 使用 ustruct.pack 打包数据
        message = ustruct.pack(
            '>HBH13s',  # 格式字符串：< 表示小端序，H=2字节无符号，B=1字节无符号，I=4字节无符号，I=4字节无符号
            99,       # messageID (2字节，值为1)
            0,       # 第3字节 (1字节，值为0)
            15, 
            framedata  
        )        

        return message

    
    @staticmethod
    def build_report_message(group_id, user_id):
        """
        构建一个 report 消息
        :param group_id: 整型，groupID 字段
        :param user_id: 整型，userID 字段
        :return: bytes，符合格式的消息
        """
        
        # 消息格式：
        # - 前2字节：messageID (大端序，值为1)
        # - 第3字节：值为8
        # - 接下来4字节：groupID (大端序)
        # - 最后4字节：userID (大端序)

        # 使用 ustruct.pack 打包数据
        message = ustruct.pack(
            '>HBII',  # 格式字符串：< 表示小端序，H=2字节无符号，B=1字节无符号，I=4字节无符号，I=4字节无符号
            1,       # messageID (2字节，值为1)
            8,       # 第3字节 (1字节，值为8)
            group_id, # groupID (4字节)
            user_id   # userID (4字节)
        )
        

        return message