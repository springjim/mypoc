
import sys


try:
    from common import AbstractLoad, PrintLog
    from dev.key import KeyManger
    #from services import *
    from service.battery import BatteryManager,Battery
    
    from service.devinfo_service import DevInfoService
    from service.media_service import MediaService
    from service.net_service  import NetService
    
    from service.poc_service import PocService
    
    from ui.poc_ui import PocUI
    from ui.main_screen import MainScreen
    from ui.welcome_screen import WelcomeScreen
    from ui.prompt_box import PromptBox
    from ui.menu_bar import MenuBar
    
    #from ui.lcd_init import init_lcd
    
                      
except:
    from usr.common import AbstractLoad, PrintLog
    from usr.dev.key import KeyManger
    #from usr.services import *
    
    from usr.service.battery import BatteryManager,Battery
    
    from usr.service.devinfo_service import DevInfoService
    from usr.service.media_service import MediaService
    from usr.service.net_service  import NetService
    
    from usr.service.poc_service import PocService
    
    from usr.ui.poc_ui import PocUI
    from usr.ui.main_screen import MainScreen
    from usr.ui.welcome_screen import WelcomeScreen
    from usr.ui.prompt_box import PromptBox
    from usr.ui.menu_bar import MenuBar
    
    #from usr.ui.lcd_init import init_lcd


class App(object):
    __service_list = []
    __ui = None
    __key = None

    @classmethod
    def set_ui(cls, ui):
        cls.__ui = ui

    @classmethod
    def add_key(cls, key):
        cls.__key = key

    @classmethod
    def add_bar(cls, bar:AbstractLoad):
        """
        这里只负责向UI添加屏幕栏, 屏幕栏由UI进行管理
        """
        try:
            if isinstance(bar, AbstractLoad):
                cls.__ui.add_bar(bar)     
        except Exception as e:
            raise Exception("[App](abort) add_bar error: ", e)
        return cls

    @classmethod
    def add_msgbox(cls, msgbox:AbstractLoad):
        """
        这里只负责向UI添加消息框, 消息框由UI进行管理
        """
        try:
            if isinstance(msgbox, AbstractLoad):
                cls.__ui.add_msgbox(msgbox)     
        except Exception as e:
            raise Exception("[App](abort) add_msgbox error: ", e)
        return cls

    @classmethod
    def add_screen(cls, screen:AbstractLoad):
        """
        这里只负责向UI添加屏幕, 屏幕由UI进行管理
        """
        if None == cls.__ui:
            raise Exception("UI is None.")
        try:
            if isinstance(screen, AbstractLoad):
                cls.__ui.add_screen(screen)    
        except Exception as e:
            raise Exception("[App](abort) add_screen error: ", e)
        return cls
        
    @classmethod
    def add_service(cls, service:AbstractLoad):
        """
        添加服务
        """
        try:
            if isinstance(service, AbstractLoad):
                service.instance_after()   # 初始化服务
                cls.__service_list.append(service)
        except Exception as e:
            raise Exception("[App](abort) add_service error: ", e)
        return cls

    @classmethod
    def exec(cls):
        """
        启动App
        """
        if None == cls.__ui:
            raise Exception("[App](abort) exec interrupt, UI is null.")
        try:
            # start ui
            cls.__ui.start()
            
            import lvgl as lv
            lv.task_handler()
            
            # start services
            for service in App.__service_list:
                service.load_before()
                service.load()
                service.load_after()
        except Exception as e:            
            sys.print_exception(e)
            print("[App] exec error:\n ", e)



if __name__ == '__main__':
    
    #=== 1.Add key ===
    App.add_key(KeyManger())   
 
    #=== 2.add main UI ===
    App.set_ui(PocUI())

    #=== 3.Add screen bar ===
    App.add_bar(MenuBar())

    #=== 4.Add message box ===
    App.add_msgbox(PromptBox())

    #=== 5.Add UI screen ===
    App.add_screen( MenuBar()) \
        .add_screen( MainScreen()) \
        .add_screen( WelcomeScreen() ) \
        .add_screen( PromptBox() )
        
    
    #=== 6.Add Service ===
    App.add_service( NetService()) \
        .add_service( PocService()) \
        .add_service( MediaService()) \
        .add_service( DevInfoService() ) \
        .add_service( BatteryManager() )

    #=== 7.Run the app ===
    App.exec()



                      