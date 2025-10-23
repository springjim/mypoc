package com.mypoc.pttlibrary.internal.audio;

import android.content.Context;
import android.media.AudioFormat;
import android.media.AudioManager;
import android.media.AudioTrack;
import android.media.audiofx.Equalizer;
import android.os.Process;
import android.util.Log;

import com.example.soundtouchdemo.JNISoundTouch;
import com.mypoc.pttlibrary.internal.PTTSessionManager;
import com.webrtc.audioprocessing.Apm;
import com.webrtc.audioprocessing.ApmViewModel;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;

/**
 * AudioPlayer 是喇叭出声
 */
public class AudioPlayer {
    public final String TAG = "AudioPlayer";
    private Context mContext;
    public boolean playing = false;
    public int bufferSize;
    private ApmViewModel vm;
    private static final int AEC_LOOP_COUNT = 1;
    private Apm _apm;  //webrtc的 核心类，处理: 回音，降噪，自动增益，静音检测
    private AudioTrack audioTrack2 = null;
    private JNISoundTouch soundtouch = new JNISoundTouch();
    private Equalizer equalizer = null;

    class PlayedSamples {

        public void setPlayedSamples(int samples) {
            long current = getUnsignedInt(samples);
            if (current < _low) {
                _high += 0x00000000ffffffffL;
            }
            _low = current;
        }

        public long getPlayedSamples() {
            return _high + _low;
        }

        private long getUnsignedInt(int data) {
            return data & 0x00000000ffffffffL;
        }

        private long _low = 0;
        private long _high = 0;
    }

    public AudioPlayer(Context mContext) {

        Log.d(TAG, "AudioPlayer");
        this.mContext = mContext;

        bufferSize = AudioTrack.getMinBufferSize(Config.PLAY_SAMPLE_RATE,
                Config.PLAY_CHANNELS,
                AudioFormat.ENCODING_PCM_16BIT);
        if (bufferSize < 0) {
            return;
        }
        try {
            int ret = -1;
            vm = new ApmViewModel(); //很多是默认设置
            vm.setDelayAgnostic(true);

            _apm = new Apm(vm.getAecExtendFilter(), vm.getSpeechIntelligibilityEnhance(), vm.getDelayAgnostic(), vm.getBeamForming(),
                    vm.getNextGenerationAEC(), vm.getExperimentalNS(), vm.getExperimentalAGC());
            ret = _apm.HighPassFilter(vm.getHighPassFilter());
            ret = _apm.AECMSetSuppressionLevel(Apm.AECM_RoutingMode.values()[3]);  //3: Speakerphone 4: LoudSpeakerphone
            ret = _apm.AECM(true);

            ret = _apm.NSSetLevel(Apm.NS_Level.values()[2]); //2:High 3:VeryHigh
            ret = _apm.NS(vm.getNs());

            ret = _apm.VAD(vm.getVad());
            ret = _apm.AGCSetAnalogLevelLimits(0, 255);   // 原来 0, 255
            ret = _apm.AGCSetMode(Apm.AGC_Mode.AdaptiveDigital); // 0:AdaptiveAnalog, 1:AdaptiveDigital, 2:FixedDigital
            ret = _apm.AGCSetTargetLevelDbfs(vm.getAgcTargetLevelInt());
            ret = _apm.AGCSetcompressionGainDb(vm.getAgcCompressionGainInt());
            ret = _apm.AGCEnableLimiter(true);
            ret = _apm.AGC(true);

            //AudioManager audioManager = (AudioManager) context.getSystemService(Context.AUDIO_SERVICE);
            //audioManager.setMode(AudioManager.MODE_IN_COMMUNICATION);
            //audioManager.setSpeakerphoneOn(on); //on=true, 表示扬声器出声， false 表示听筒出声

        } catch (Exception e) {
            e.printStackTrace();
        }

        AudioManager audioManager = (AudioManager) mContext.getSystemService(Context.AUDIO_SERVICE);
        int maxVol = audioManager.getStreamMaxVolume(AudioManager.STREAM_MUSIC);

        Log.e(TAG, "maxVol=" + maxVol);

        //播放时，音量设最大值
        //2025.10 不自动改音量到最大，由客户去调
        //audioManager.setStreamVolume(AudioManager.STREAM_MUSIC, maxVol, AudioManager.FLAG_PLAY_SOUND);

        audioTrack2 = new AudioTrack(AudioManager.STREAM_MUSIC,//AudioManager.STREAM_SYSTEM, AudioManager.STREAM_MUSIC
                Config.PLAY_SAMPLE_RATE, Config.PLAY_CHANNELS,
                Config.PLAY_AUDIO_ENCODING, bufferSize*2, AudioTrack.MODE_STREAM);

    }

    public void writeAudio() {
        Log.d(TAG, "writeAudio");
        new Thread(new Runnable() {
            @Override
            public void run() {
                try {
                    Log.d(TAG, "writeAudio2 run start");
                    soundtouch.setSampleRate(8000);
                    soundtouch.setChannels(1);
                    //以下设置大了，会造成破音了
                    soundtouch.setPitchSemiTones(0);  //音量加大8个半音， 最大值是12，不能取12，会造成声音失真
                    Process.setThreadPriority(Process.THREAD_PRIORITY_URGENT_AUDIO);

                    audioTrack2.play();

                    // 初始化 Equalizer
                    int audioSessionId = audioTrack2.getAudioSessionId();
                    //Android 提供了 Equalizer 和 BassBoost 等音频效果器，可以通过调整音频的频率响应来增强音量感知
                    equalizer = new Equalizer(0, audioSessionId);
                    equalizer.setEnabled(true); // 启用均衡器

                    AudioRecorder.setDecoder(Config.PLAY_SAMPLE_RATE);
                    AudioRecorder.startDecoder();

                    long writtenSamples = 0;
                    PlayedSamples playedSamples = new PlayedSamples();
                    short[] tmpIn = new short[320 / 2];
                    while (playing) {
                        try {

                            Thread.sleep(1);
                            ByteBuffer sb = PTTSessionManager.Consumer_Get(500L);
                            if (PTTSessionManager.getInstance().getmMEDIA_EX_FILE_FRAME() == 1) {
                                //文件语音帧
                                if (sb != null) {
                                    if (audioTrack2 != null) //写入播放器
                                        audioTrack2.write(sb.array(), 0, sb.array().length);
                                }
                            } else {
                                //麦克风语音
                                if (sb != null) {
                                    byte[] tmp = AudioRecorder.playData(sb); //要先解码
                                    if (tmp==null) continue;

                                    ByteBuffer.wrap(tmp).order(ByteOrder.LITTLE_ENDIAN).asShortBuffer().get(tmpIn);
                                    if (tmpIn != null) {
                                        for (int i = 0; i < AEC_LOOP_COUNT; ++i) {
                                            int bufferOffSet = i * tmpIn.length / AEC_LOOP_COUNT;
                                            if (!vm.getAecNone()) {
                                                _apm.ProcessRenderStream(tmpIn, bufferOffSet);
                                            }
                                        }
                                        int size = PTTSessionManager.UsedSize();
                                        if (size >= 5) {
                                            soundtouch.setTempoChange(20);  //这个声音加速的
                                            soundtouch.putSamples(tmpIn, tmpIn.length);
                                            short[] data;
                                            do {
                                                data = soundtouch.receiveSamples();
                                                if (data.length <= 0) break;
                                                playedSamples.setPlayedSamples(audioTrack2.getPlaybackHeadPosition());
                                                int samplesWritten = audioTrack2.write(data, 0, data.length);
                                                if (samplesWritten != data.length) {
                                                    Log.e(TAG, "writeAudio2 异常");
                                                    break;
                                                }
                                                writtenSamples += samplesWritten;

                                            } while (true);
                                        } else {

                                            playedSamples.setPlayedSamples(audioTrack2.getPlaybackHeadPosition());
                                            int samplesWritten = audioTrack2.write(tmpIn, 0, tmpIn.length);
                                            if (samplesWritten != tmpIn.length) {
                                                Log.e(TAG, "writeAudio2 异常");

                                            } else {
                                                writtenSamples += samplesWritten;
                                            }

                                        }
                                        PTTSessionManager.Consumer_Put(tmpIn);

                                    }
                                    //aec处理 end

                                }

                            }

                        } catch (Exception e) {
                            Log.e(TAG, "writeAudio2:\n" + Log.getStackTraceString(e));
                        }
                    }

                    Log.d(TAG, "writeAudio2 run stop");

                    AudioRecorder.stopDecoder();
                    AudioRecorder.releaseDecoder();

                    if (audioTrack2 != null &&
                            audioTrack2.getPlayState() == AudioTrack.PLAYSTATE_PLAYING)
                        audioTrack2.stop();

                    Log.d(TAG, "writeAudio2 run end");
                } catch (Exception ex) {
                    Log.e(TAG, "writeAudio2:\n" + Log.getStackTraceString(ex));
                }

            }
        }).start();
    }


    public void startPlay() {
        Log.d(TAG, "startPlay");
        playing = true;
    }

    public void stopPlay() {
        Log.d(TAG, "stopPlay" );
        boolean preplaying = playing;
        playing = false;

        try
        {
            if( preplaying )
                Thread.sleep( 5 );
        }
        catch ( Exception ex )
        {
            Log.e(TAG, "stop AudioPlayer thread:\n" + Log.getStackTraceString(ex) );
        }

        Log.d(TAG, "stopPlay thread......decodedQueue.put");


        ByteBuffer b1 = ByteBuffer.allocate( 13 );
        ByteBuffer b2 = ByteBuffer.allocate( 13 );
        for(int i=0; i<13; ++i)
        {
            b1.put( (byte)0xFF );
            b2.put( (byte)0xFF );
        }
        try {
            PTTSessionManager.usedDecodedQueue2.put( b1 );
            PTTSessionManager.usedDecodedQueue2.put( b2 );
        } catch (InterruptedException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        }
        Log.d(TAG, "stopPlay  MyApplication.decodedQueue2.put" );

    }

    public void release() {

        Log.d(TAG, "release" );

        if( audioTrack2!=null )
        {
            if (audioTrack2.getPlayState() == AudioTrack.PLAYSTATE_PLAYING)
            {
                audioTrack2.stop();
            }
            audioTrack2.release();
            audioTrack2 = null;
        }

        if (equalizer!=null)
            equalizer.release();

        if (_apm != null) {
            _apm.close();
        }
    }
}
