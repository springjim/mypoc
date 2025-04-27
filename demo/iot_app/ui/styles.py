

import lvgl as lv


LCD_SIZE_WIDTH  = 240
LCD_SIZE_HEIGHT = 240


class LVGLColor(object):
    """
    颜色集
    """
    # lv.color_make(0xFF, 0xFF, 0xFF)
    # lv.color_hex(0xFF) 
    # lv.color_white()
    BASE_COLOR_BLACK        = lv.color_hex(0x000000)
    BASE_COLOR_WHITE        = lv.color_hex(0xFFFFFF)
    BASE_COLOR_GRAY         = lv.color_hex(0x808080)
    BASE_COLOR_RED          = lv.color_hex(0xFF0000)
    BASE_COLOR_PURPLE       = lv.color_hex(0x800080)
    BASE_COLOR_GREEN        = lv.color_hex(0x008000)
    BASE_COLOR_YELLOW       = lv.color_hex(0xFFFF00)
    BASE_COLOR_BLUE         = lv.color_hex(0x0000FF)



class FontStyle(object):
    montserrat14_txt000000_bg2195f6 = lv.style_t()
    consolas_12_txt000000_bg2195f6 = lv.style_t()

    @classmethod
    def init_style(cls):
        cls.montserrat14_txt000000_bg2195f6.init()
        cls.montserrat14_txt000000_bg2195f6.set_radius(0)
        cls.montserrat14_txt000000_bg2195f6.set_bg_color(lv.color_make(0x21, 0x95, 0xf6))
        cls.montserrat14_txt000000_bg2195f6.set_bg_grad_color(lv.color_make(0x21, 0x95, 0xf6))
        cls.montserrat14_txt000000_bg2195f6.set_bg_grad_dir(lv.GRAD_DIR.VER)
        cls.montserrat14_txt000000_bg2195f6.set_bg_opa(0)
        cls.montserrat14_txt000000_bg2195f6.set_text_color(lv.color_make(0x00, 0x00, 0x00))
        cls.montserrat14_txt000000_bg2195f6.set_text_font(lv.font_montserrat_14)
        cls.montserrat14_txt000000_bg2195f6.set_text_letter_space(0)
        cls.montserrat14_txt000000_bg2195f6.set_pad_left(0)
        cls.montserrat14_txt000000_bg2195f6.set_pad_right(0)
        cls.montserrat14_txt000000_bg2195f6.set_pad_top(0)
        cls.montserrat14_txt000000_bg2195f6.set_pad_bottom(0)

        
        cls.consolas_12_txt000000_bg2195f6.init()
        cls.consolas_12_txt000000_bg2195f6.set_radius(0)
        cls.consolas_12_txt000000_bg2195f6.set_bg_color(lv.color_make(0x21, 0x95, 0xf6))
        cls.consolas_12_txt000000_bg2195f6.set_bg_grad_color(lv.color_make(0x21, 0x95, 0xf6))
        cls.consolas_12_txt000000_bg2195f6.set_bg_grad_dir(lv.GRAD_DIR.VER)
        cls.consolas_12_txt000000_bg2195f6.set_bg_opa(0)
        cls.consolas_12_txt000000_bg2195f6.set_text_color(lv.color_make(0x00, 0x00, 0x00))
        #cls.consolas_12_txt000000_bg2195f6.set_text_font(lv.font_consolas_12)
        cls.consolas_12_txt000000_bg2195f6.set_text_font(lv.font_montserrat_14)
        cls.consolas_12_txt000000_bg2195f6.set_text_letter_space(0)
        cls.consolas_12_txt000000_bg2195f6.set_pad_left(0)
        cls.consolas_12_txt000000_bg2195f6.set_pad_right(0)
        cls.consolas_12_txt000000_bg2195f6.set_pad_top(0)
        cls.consolas_12_txt000000_bg2195f6.set_pad_bottom(0)

class CommonStyle(object):
    default = lv.style_t()
    container_bgffffff = lv.style_t()
    container_bg000000 = lv.style_t()
    container_bge1e1e1 = lv.style_t()

    img_style = lv.style_t()

    @classmethod
    def init_style(cls):
        cls.default.init()
        cls.default.set_bg_color(LVGLColor.BASE_COLOR_WHITE)
        cls.default.set_radius(0)
        cls.default.set_img_recolor_opa(0)
        cls.default.set_bg_opa(255)
        cls.default.set_text_font(lv.font_montserrat_14)

        cls.container_bgffffff.init()
        cls.container_bgffffff.set_radius(0)
        cls.container_bgffffff.set_bg_color(LVGLColor.BASE_COLOR_WHITE)
        cls.container_bgffffff.set_bg_grad_color(LVGLColor.BASE_COLOR_WHITE)
        cls.container_bgffffff.set_anim_speed(10)
        cls.container_bgffffff.set_bg_grad_dir(lv.GRAD_DIR.VER)
        cls.container_bgffffff.set_bg_opa(255)
        cls.container_bgffffff.set_border_width(0)
        cls.container_bgffffff.set_pad_left(0)
        cls.container_bgffffff.set_pad_right(0)
        cls.container_bgffffff.set_pad_top(0)
        cls.container_bgffffff.set_pad_bottom(0)

        cls.container_bg000000.init()
        cls.container_bg000000.set_radius(0)
        cls.container_bg000000.set_bg_color(LVGLColor.BASE_COLOR_BLACK)
        cls.container_bg000000.set_bg_grad_color(LVGLColor.BASE_COLOR_BLACK)
        cls.container_bg000000.set_anim_speed(10)
        cls.container_bg000000.set_bg_grad_dir(lv.GRAD_DIR.VER)
        cls.container_bg000000.set_bg_opa(255)
        cls.container_bg000000.set_border_width(0)
        cls.container_bg000000.set_pad_left(0)
        cls.container_bg000000.set_pad_right(0)
        cls.container_bg000000.set_pad_top(0)
        cls.container_bg000000.set_pad_bottom(0)

        cls.container_bge1e1e1.init()
        cls.container_bge1e1e1.set_radius(0)
        cls.container_bge1e1e1.set_bg_color(lv.color_hex(0xE1E1E1))
        cls.container_bge1e1e1.set_bg_grad_color(lv.color_hex(0xE1E1E1))
        cls.container_bge1e1e1.set_anim_speed(10)
        cls.container_bge1e1e1.set_bg_grad_dir(lv.GRAD_DIR.VER)
        cls.container_bge1e1e1.set_bg_opa(255)
        cls.container_bge1e1e1.set_border_width(0)
        cls.container_bge1e1e1.set_pad_left(0)
        cls.container_bge1e1e1.set_pad_right(0)
        cls.container_bge1e1e1.set_pad_top(0)
        cls.container_bge1e1e1.set_pad_bottom(0)

        cls.img_style.init()
        cls.img_style.set_img_recolor(LVGLColor.BASE_COLOR_WHITE)
        cls.img_style.set_img_recolor_opa(0)
        cls.img_style.set_img_opa(255)


class MainScreenStyle(object):
    list_scrollbar = lv.style_t()
    btn_group = lv.style_t()

    @classmethod
    def init_style(cls):
        cls.list_scrollbar.init()
        cls.list_scrollbar.set_radius(3)
        cls.list_scrollbar.set_bg_color(LVGLColor.BASE_COLOR_WHITE)
        cls.list_scrollbar.set_bg_grad_color(LVGLColor.BASE_COLOR_WHITE)
        #垂直渐变
        cls.list_scrollbar.set_bg_grad_dir(lv.GRAD_DIR.VER)
        cls.list_scrollbar.set_bg_opa(0)

        cls.btn_group.init()
        cls.btn_group.set_radius(0)
        cls.btn_group.set_bg_color(LVGLColor.BASE_COLOR_WHITE)
        cls.btn_group.set_bg_grad_color(LVGLColor.BASE_COLOR_WHITE)
        cls.btn_group.set_anim_speed(10)
        cls.btn_group.set_bg_grad_dir(lv.GRAD_DIR.VER)
        cls.btn_group.set_border_width(0)
        cls.btn_group.set_bg_opa(255)
        cls.btn_group.set_pad_left(0)
        cls.btn_group.set_pad_right(0)
        cls.btn_group.set_pad_top(0)
        cls.btn_group.set_pad_bottom(0)
        cls.btn_group.set_text_color(lv.color_make(0x00, 0x00, 0x00))
        #cls.btn_group.set_text_font(lv.font_consolas_12)
        cls.btn_group.set_text_font(lv.font_montserrat_14)


class DevInfoScreenStyle(object):

    @classmethod
    def init_style(cls):
        pass



FontStyle.init_style()
CommonStyle.init_style()
MainScreenStyle.init_style()
DevInfoScreenStyle.init_style()



