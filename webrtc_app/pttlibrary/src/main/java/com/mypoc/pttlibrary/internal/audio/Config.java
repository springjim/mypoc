package com.mypoc.pttlibrary.internal.audio;

import android.media.AudioFormat;

public class Config
{
    
    // 30ms * 8000= 240, 240*2=480
    public static final int FRAME_SIZE = 480;  //RECORD_SAMPLE_RATE = 8000;
    //public static final int FRAME_SIZE = 480*2;   //RECORD_SAMPLE_RATE = 16000;    
    
    public static final int RECORD_SAMPLE_RATE  = 8000;//8000
    //public static final int RECORD_SAMPLE_RATE = 16000; 
    //public static final int RECORD_SAMPLE_RATE = 32000;       
    //public static final int RECORD_SAMPLE_RATE = 44100;      
    
    public static final int RECORD_CHANNELS  = AudioFormat.CHANNEL_IN_MONO;  //指单通道IN，在所有安卓机上都能用

       
    public static final int RECORD_AUDIO_ENCODING = AudioFormat.ENCODING_PCM_16BIT; //AudioFormat.ENCODING_PCM_16BIT
    
    public static final int PLAY_SAMPLE_RATE  = 8000;//8000
    //public static final int PLAY_SAMPLE_RATE = 16000;
    //public static final int PLAY_SAMPLE_RATE = 32000;
    //public static final int PLAY_SAMPLE_RATE = 44100;    
    
    public static final int PLAY_CHANNELS  = AudioFormat.CHANNEL_OUT_MONO;

    
    public static final int PLAY_AUDIO_ENCODING = AudioFormat.ENCODING_PCM_16BIT;
    
    public static final int MSG_HEADER_LEN = 3; //two bytes MSG ID + one byte length
    
    public static final short MSG_MEDIA = 0; // media message type
    
    public static final int RCV_BUF_LEN = 1024; //tcp receive side buffer len
}
