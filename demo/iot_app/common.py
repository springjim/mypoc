import _thread
import utime
import usys


class EventMap(object):
    """===example===

    import EventMap

    def time_out(event=None, msg=None):
        pass
        
    EventMap.bind("time_out", time_out)

    EventMap.send("time_out")
    """
    __event_map = dict()
    __event_log = None

    MODE_SYNC = 0
    MODE_ASYNC = 1

    def __init__(self):
        pass

    @classmethod
    def bind(cls, event, callback):
        """
        :param event: event name
        :param callback: event callback
        """
        if None == event or "" == event:
            return
        cls.__event_map[event] = callback
    
    @classmethod
    def unbind(cls, event):
        """
        :param event: event name
        """
        if None == event or "" == event:
            return
        cls.__event_map.pop(event, None)

    @classmethod
    def send(cls, event, msg=None, mode=MODE_SYNC):
        """
        :param event: event name
        :param msg: event message
        :param mode: send mode, sync or async
        """
        if event not in cls.__event_map:
            return

        if cls.MODE_SYNC == mode:
            res = None
            try:
                if event in cls.__event_map:
                    res = cls.__event_map[event](event, msg)
            except Exception as e:
                if cls.__event_log:
                    cls.__event_log.info("ERROR executed (event) -> {} (params) -> {} (result) -> {}".format(event, msg, res))
                usys.print_exception(e)
            if cls.__event_log:
                cls.__event_log.info("SYNC executed (event) -> {} (params) -> {} (result) -> {}".format(event, msg, res))
            return res
        
        elif cls.MODE_ASYNC == mode:
            try:
                _thread.start_new_thread(cls.__event_map[event], (event, msg))
            except Exception as e:
                if cls.__event_log:
                    cls.__event_log.info("ERROR executed (event) -> {} (params) -> {} (result) -> {}".format(event, msg, res))
                usys.print_exception(e)
            if cls.__event_log:
                cls.__event_log.info("ASYNC executed (event) -> {} (params) -> {} (result) -> {}".format(event, msg, None))

    @classmethod
    def event_map(cls):
        """
        print event map
        """
        print("---------event_map---------")
        for event in cls.__event_map.keys():
            print("event:", event)
        print("---------------------------")

    @classmethod
    def set_log(cls, log_adapter):
        cls.__event_log = log_adapter


class Lock(object):
    """互斥锁"""
    def __init__(self):
        self.lock = _thread.allocate_lock()         # 创建

    def __enter__(self, *args, **kwargs):           # 获取
        self.lock.acquire()
        
    def __exit__(self, *args, **kwargs):            # 释放
        self.lock.release()


class AbstractLoad(object):
    def load_before(self, *args, **kwargs):
        """加载前调用"""
        pass

    def load(self, *args, **kwargs):
        """加载时调用"""
        pass

    def load_after(self, *args, **kwargs):
        """加载后调用"""
        pass

    def instance_after(self, *args, **kwargs):
        """实例化后调用"""
        pass

    def deactivate(self, *args, **kwargs):
        """失效"""
        pass


class PrintLog(object):
    """
    打印形式的log, 可以通过enable控制是否打印
    """    
    enable = True

    @classmethod
    def log(cls, tag, msg):
        """
        :param tag: tag
        :param msg: msg
        """
        if not cls.enable:
            return

        local_time = utime.localtime()
        date = "{:04}-{:02}-{:02}".format(local_time[0], local_time[1], local_time[2])
        time = "{:02}:{:02}:{:02}".format(local_time[3], local_time[4], local_time[5])

        _msg = "[{} {}]({})------ {}".format(date, time, tag, msg)
        print( _msg ) 


