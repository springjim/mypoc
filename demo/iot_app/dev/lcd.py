from machine import LCD, Pin

white = 0xFFFF
black = 0x0000

XSTART_H = 0xf0
XSTART_L = 0xf1
YSTART_H = 0xf2
YSTART_L = 0xf3
XEND_H   = 0xE0
XEND_L   = 0xE1
YEND_H   = 0xE2
YEND_L   = 0xE3

XSTART   = 0xD0
XEND     = 0xD1
YSTART   = 0xD2
YEND     = 0xD3

class ST7789():
    def __init__(self, Interface, SPICS, SPIRST, SPIDC, SPIPort=1, SPIMode=0, InitData=None, width=240, height=240, clk=26000):
        self._LcdInit(Interface, SPICS, SPIRST, SPIDC, SPIPort, SPIMode, InitData, width, height, clk)
        self.clear(white)
        
    def _LcdInit(self, Interface, SPICS, SPIRST, SPIDC, SPIPort, SPIMode, InitData, width, height, clk):
        self._lcd_w = width
        self._lcd_h = height
        self._interface = Interface
        self._spiport = SPIPort
        self._spimode = SPIMode
        self._spics = SPICS
        self._spidc = SPIDC
        self._spirst = SPIRST
        self._clk = clk
        self.gpio = Pin(Pin.GPIO15, Pin.OUT, Pin.PULL_DISABLE, 1)
        
        L2R_U2D = 0
        L2R_D2U = 1
        R2L_U2D = 2
        R2L_D2U = 3

        U2D_L2R = 4
        U2D_R2L = 5
        D2U_L2R = 6
        D2U_R2L = 7

        regval = 0
        if(dir == L2R_U2D):
            regval |= (0 << 7) | (0 << 6) | (0 << 5)
        elif dir == L2R_D2U:
            regval |= (1 << 7) | (0 << 6) | (0 << 5)
        elif dir == R2L_U2D:
            regval |= (0 << 7) | (1 << 6) | (0 << 5)
        elif dir == R2L_D2U:
            regval |= (1 << 7) | (1 << 6) | (0 << 5)
        elif dir == U2D_L2R:
            regval |= (0 << 7) | (0 << 6) | (1 << 5)
        elif dir == U2D_R2L:
            regval |= (0 << 7) | (1 << 6) | (1 << 5)
        elif dir == D2U_L2R:
            regval |= (1 << 7) | (0 << 6) | (1 << 5)
        elif dir == D2U_R2L:
            regval |= (1 << 7) | (1 << 6) | (1 << 5)
        else:
            regval |= (0 << 7) | (0 << 6) | (0 << 5)
        if(regval & 0X20):
            if(width < height):
                self._lcd_w = height
                self._lcd_h = width
        else:
            if(width > height):
                self._lcd_w = height
                self._lcd_h = width

        init_data = (
            2, 0, 120,
            0, 0, 0x11,
            0, 1, 0x36,
            1, 1, regval,
            0, 1, 0x3A,
            1, 1, 0x05,
            0, 0, 0x21,
            0, 5, 0xB2,
            1, 1, 0x05,
            1, 1, 0x05,
            1, 1, 0x00,
            1, 1, 0x33,
            1, 1, 0x33,
            0, 1, 0xB7,
            1, 1, 0x23,
            0, 1, 0xBB,
            1, 1, 0x22,
            0, 1, 0xC0,
            1, 1, 0x2C,
            0, 1, 0xC2,
            1, 1, 0x01,
            0, 1, 0xC3,
            1, 1, 0x13,
            0, 1, 0xC4,
            1, 1, 0x20,
            0, 1, 0xC6,
            1, 1, 0x0F,
            0, 2, 0xD0,
            1, 1, 0xA4,
            1, 1, 0xA1,
            0, 1, 0xD6,
            1, 1, 0xA1,
            0, 14, 0xE0,
            1, 1, 0x70,
            1, 1, 0x06,
            1, 1, 0x0C,
            1, 1, 0x08,
            1, 1, 0x09,
            1, 1, 0x27,
            1, 1, 0x2E,
            1, 1, 0x34,
            1, 1, 0x46,
            1, 1, 0x37,
            1, 1, 0x13,
            1, 1, 0x13,
            1, 1, 0x25,
            1, 1, 0x2A,
            0, 14, 0xE1,
            1, 1, 0x70,
            1, 1, 0x04,
            1, 1, 0x08,
            1, 1, 0x09,
            1, 1, 0x07,
            1, 1, 0x03,
            1, 1, 0x2C,
            1, 1, 0x42,
            1, 1, 0x42,
            1, 1, 0x38,
            1, 1, 0x14,
            1, 1, 0x14,
            1, 1, 0x27,
            1, 1, 0x2C,
            0, 0, 0x29,
            0, 1, 0x36,
            1, 1, regval,
            0, 4, 0x2a,
            1, 1, 0x00,
            1, 1, 0x00,
            1, 1, 0x00,
            1, 1, 0xef,
            0, 4, 0x2b,
            1, 1, 0x00,
            1, 1, 0x00,
            1, 1, 0x00,
            1, 1, 0xef,
            0, 0, 0x2c,
        )
        lcd_set_display_area = (
            0, 4, 0x2a,             # 0x2a 设置列
            1, 1, XSTART_H,
            1, 1, XSTART_L,
            1, 1, XEND_H,
            1, 1, XEND_L,
            0, 4, 0x2b,             # 0x2b 设置行
            1, 1, YSTART_H,
            1, 1, YSTART_L,
            1, 1, YEND_H,
            1, 1, YEND_L,
            0, 0, 0x2c,
        )
        lcd_display_on = (
            0, 0, 0x11,      # 0x11 唤醒
            2, 0, 20,
            0, 0, 0x29,      # 0x29 显示使能
        )
        lcd_display_off = (
            0, 0, 0x28,      # 0x28 显示关闭
            2, 0, 120,
            0, 0, 0x10,      # 0x10 睡眠
        )
        if InitData is None:
            self._initData = bytearray(init_data)
        else:
            self._initData = InitData

        self._invalidData = bytearray(lcd_set_display_area)
        self._displayOn = bytearray(lcd_display_on)
        self._displayOff = bytearray(lcd_display_off)

        self._lcd = LCD()
        self._lcd.lcd_init(
            self._initData,
            self._lcd_w,
            self._lcd_h,
            self._clk,
            1,
            4,
            0,
            self._invalidData,
            self._displayOn,
            self._displayOff,
            None)
        
    def clear(self, color=0x000000):     # 清屏
        self._lcd.lcd_clear(color)
        
    def display_on(self):
        self._lcd.lcd_display_on()

    def display_off(self):
        self._lcd.lcd_display_off()

