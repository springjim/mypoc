package com.mypoc.pttlibrary.internal.audio;

import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothProfile;
import android.content.Context;
import android.media.AudioFormat;
import android.media.AudioManager;
import android.media.AudioRecord;
import android.media.MediaCodec;
import android.media.MediaCodecInfo;
import android.media.MediaFormat;
import android.media.MediaRecorder;
import android.media.audiofx.AcousticEchoCanceler;
import android.media.audiofx.AutomaticGainControl;
import android.media.audiofx.NoiseSuppressor;
import android.os.Process;
import android.util.Log;

import com.mypoc.pttlibrary.internal.PTTService;
import com.mypoc.pttlibrary.internal.PTTSessionManager;
import com.mypoc.pttlibrary.utils.ByteTool;
import com.webrtc.audioprocessing.Apm;
import com.webrtc.audioprocessing.ApmViewModel;

import java.io.IOException;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;

public class AudioRecorder {

    private final static String TAG = "AudioRecorder";
    private static int IntDecoderCount = 1;

    private static final int TIMEOUT_USEC = 20;
    /**
     * packsize = 320 bytes
     */
    public static int PACKSIZE = TIMEOUT_USEC * (Config.RECORD_SAMPLE_RATE * (16 / 8)) / 1000;
    private static MediaCodec decoder = null;
    private static MediaFormat formatdecoder = null;
    //编码方式  "audio/amr";  // "audio/mp4a-latm"; //"audio/3gpp";  //"audio/amr-wb" //audio/amr
    private static final String AudioType = MediaFormat.MIMETYPE_AUDIO_AMR_NB;
    private static final int SAMPLE_RATE = Config.RECORD_SAMPLE_RATE;
    private static final int KEY_BIT_RATE = 4750;//4750

    private static ByteBuffer[] inputBuffers;
    private static ByteBuffer[] outputBuffers;
    private static ByteBuffer inputBuffer;
    private static ByteBuffer outputBuffer;
    private static MediaCodec.BufferInfo bufferInfo;
    public static boolean resetDecoderFlag = false;
    public static AudioManager mAudioManager = null;
    public Context context;
    private PTTService pttService;

    private AudioRecord audioRecord = null;
    AcousticEchoCanceler acousticEchoCanceler = null;
    NoiseSuppressor noiseSuppressor = null;

    //使有webrtc apm处理语音：回音，降噪，自动增益，静音检测
    private ApmViewModel vm;
    private static final int AEC_LOOP_COUNT = 1;
    private Apm _apm;
    public static byte[] NullSoundData = new byte[PACKSIZE];

    //编码
    private static MediaCodec encoder = null;
    private static MediaFormat formatencoder = null;
    private static ByteBuffer[] ecoderinputBuffers = null;
    private static ByteBuffer[] ecoderoutputBuffers = null;
    private static ByteBuffer ecoderinputBuffer = null;
    private static ByteBuffer ecoderoutputBuffer = null;
    private static MediaCodec.BufferInfo ecoderbufferInfo;

    public AudioRecorder(Context context, PTTService pttService,AudioManager audioManager) {
        this.context = context;
        this.pttService= pttService;
        this.mAudioManager= audioManager;
        try {

            try {
                int ret = -1;
                vm = new ApmViewModel(); //很多是默认设置
                vm.setDelayAgnostic(true);

                //////以下 2024.10.28 经过了调参，回音最少了
                _apm = new Apm(vm.getAecExtendFilter(), vm.getSpeechIntelligibilityEnhance(), vm.getDelayAgnostic(), vm.getBeamForming(),
                        vm.getNextGenerationAEC(), vm.getExperimentalNS(), vm.getExperimentalAGC());
                ret = _apm.HighPassFilter(false);
                ret = _apm.AECMSetSuppressionLevel(Apm.AECM_RoutingMode.Speakerphone);  //3: Speakerphone 4: LoudSpeakerphone
                ret = _apm.AECM(true);
                ret = _apm.NSSetLevel(Apm.NS_Level.High); //2:High 3:VeryHigh
                ret = _apm.NS(true);

                ret = _apm.VAD(true);
                ret = _apm.AGCSetAnalogLevelLimits(0, 255);   //原来 0,255
                //手机移动端一般用 AdaptiveDigital ， pc端用AdaptiveAnalog
                ret = _apm.AGCSetMode(Apm.AGC_Mode.AdaptiveDigital); // 0:AdaptiveAnalog, 1:AdaptiveDigital, 2:FixedDigital
                ret = _apm.AGCSetTargetLevelDbfs(0);  // [0~31]  值越小，音越大
                ret = _apm.AGCSetcompressionGainDb(12);  //[0~90] 值越大，回音也会增大， 经过调试值为12，声音最大
                ret = _apm.AGCEnableLimiter(true);   //不限制会破音
                ret = _apm.AGC(true);

                //AudioManager audioManager = (AudioManager) context.getSystemService(Context.AUDIO_SERVICE);
                //audioManager.setMode(AudioManager.MODE_IN_COMMUNICATION);
                //audioManager.setSpeakerphoneOn(on); //on=true, 表示扬声器出声， false 表示听筒出声

                Log.d(TAG, "VM:" + vm.toString());

            } catch (Exception e) {
                e.printStackTrace();
            }

            final int minBufferSize = AudioRecord.getMinBufferSize(
                    Config.RECORD_SAMPLE_RATE,
                    Config.RECORD_CHANNELS,
                    Config.RECORD_AUDIO_ENCODING);

            audioRecord = new AudioRecord(MediaRecorder.AudioSource.MIC, // 指定音频来源，这里为麦克风
                    Config.RECORD_SAMPLE_RATE, // 采样频率
                    Config.RECORD_CHANNELS, // 录制通道
                    Config.RECORD_AUDIO_ENCODING, // 录制编码格式
                    minBufferSize * 2); // 录制缓冲区大小



            if (audioRecord.getState()!=AudioRecord.STATE_INITIALIZED){
              //麦克风被其它app抢占了

            };

            if (AutomaticGainControl.isAvailable()) {
                //关闭掉系统原生支持的AGC
                AutomaticGainControl agc = AutomaticGainControl.create(
                        audioRecord.getAudioSessionId()
                );
                agc.setEnabled(false);
            }

            for (int m = 0; m < 20; ++m) {
                short[] a = new short[160];
                try {
                    PTTSessionManager.Consumer_Put(a);
                } catch (InterruptedException e) {
                }
            }

        } catch (Exception e) {
            e.printStackTrace();
        }

    }

    private static boolean setEncoder(int rate) {
        try {
            final int minBufferSize = AudioRecord.getMinBufferSize(
                    Config.RECORD_SAMPLE_RATE,
                    Config.RECORD_CHANNELS,
                    Config.RECORD_AUDIO_ENCODING);

            encoder = MediaCodec.createEncoderByType(AudioType);

            Log.d(TAG, "setEncoder");
            Log.d(TAG, "encoder " + minBufferSize + " bytes AudioRecord.getMinBufferSize");

            formatencoder = new MediaFormat();
            formatencoder.setString(MediaFormat.KEY_MIME, AudioType);
            formatencoder.setInteger(MediaFormat.KEY_CHANNEL_COUNT, 1);
            formatencoder.setInteger(MediaFormat.KEY_AAC_PROFILE, MediaCodecInfo.CodecProfileLevel.AACObjectELD); // AACObjectELD
            formatencoder.setInteger(MediaFormat.KEY_SAMPLE_RATE, rate);//Config.RECORD_SAMPLE_RATE
            formatencoder.setInteger(MediaFormat.KEY_BIT_RATE, KEY_BIT_RATE);    //AAC-LC 8kbps
            formatencoder.setInteger(MediaFormat.KEY_MAX_INPUT_SIZE, minBufferSize * 2);

            encoder.configure(formatencoder, null, null, MediaCodec.CONFIGURE_FLAG_ENCODE);

        } catch (Exception e) {
            Log.e(TAG, "encoder " + Log.getStackTraceString(e));
            e.printStackTrace();
        }
        return true;
    }

    public static void startEcoder() {
        try {
            Log.e(TAG, "startEcoder");
            setEncoder(Config.PLAY_SAMPLE_RATE);
            if (encoder == null) return;
            encoder.start();
            Log.e(TAG, "encoder.start");
            //------------------------
            ecoderinputBuffers = encoder.getInputBuffers();
            ecoderoutputBuffers = encoder.getOutputBuffers();
            ecoderbufferInfo = new MediaCodec.BufferInfo();

        } catch (Exception e) {
            Log.e(TAG, "startEcoder " + Log.getStackTraceString(e));
        }
    }

    private static void resetEncoder() {
        Log.e(TAG, "==================== resetEcoder Start ==========================");
        try {
            Log.e(TAG, "==================== stopEcoder ==========================");
            stopEcoder();
            Log.e(TAG, "==================== startEcoder ==========================");
            startEcoder();
            Log.e(TAG, "==================== resetEecoder End ==========================");
        } catch (Exception ex) {
            Log.e(TAG, "==================== resetEecoder Exception:");
            Log.e(TAG, Log.getStackTraceString(ex));
        }
    }

    public static byte[] getEncoderData(byte[] data) {
        if (encoder == null) {
            Log.d(TAG, "==================== getEcoderData: encoder==null");
            resetEncoder();
            return null;
        }

        //一帧最少 PACKSIZE
        if (data == null || data.length < PACKSIZE) {
            return null;
        }

        int inputBufferIndex = -1;
        int outputBufferIndex = -1;
        byte[] outData = null;

        try {
            inputBufferIndex = encoder.dequeueInputBuffer(0); // TIMEOUT_USEC
            Log.d(TAG, "encoder.dequeueInputBuffer :" + inputBufferIndex);

            if (inputBufferIndex >= 0) {
                ecoderinputBuffers[inputBufferIndex].clear();
                ecoderinputBuffer = ecoderinputBuffers[inputBufferIndex];

                ecoderinputBuffer.put(data, 0, data.length);
                encoder.queueInputBuffer(inputBufferIndex, 0, data.length, System.nanoTime() / 1000, 0);
            } else if (inputBufferIndex == MediaCodec.INFO_TRY_AGAIN_LATER) {
                Log.d(TAG, "encoder dequeueInputBuffer : No buffer available...");
            }

            outputBufferIndex = encoder.dequeueOutputBuffer(ecoderbufferInfo, 10);
            Log.d(TAG, "encoder dequeueOutputBuffer outputBufferIndex=" + outputBufferIndex);

            if (outputBufferIndex >= 0) {
                ecoderoutputBuffer = ecoderoutputBuffers[outputBufferIndex];

                ecoderoutputBuffer.position(ecoderbufferInfo.offset);
                ecoderoutputBuffer.limit(ecoderbufferInfo.offset + ecoderbufferInfo.size);
                ecoderoutputBuffer.position(ecoderbufferInfo.offset);

                outData = new byte[ecoderbufferInfo.size];
                ecoderoutputBuffer.get(outData, 0, ecoderbufferInfo.size);  //取到编码后的数据

                encoder.releaseOutputBuffer(outputBufferIndex, false);
                ecoderoutputBuffer = null;

                return outData;

            } else if (outputBufferIndex == MediaCodec.INFO_OUTPUT_BUFFERS_CHANGED) {
                ecoderoutputBuffers = encoder.getOutputBuffers();
                Log.d(TAG, "encoder INFO_OUTPUT_BUFFERS_CHANGED outputBufferIndex: " + outputBufferIndex);
            } else if (outputBufferIndex == MediaCodec.INFO_OUTPUT_FORMAT_CHANGED) {
                MediaFormat mediaformat = encoder.getOutputFormat();
                Log.d(TAG, "encoder INFO_OUTPUT_FORMAT_CHANGED mediaformat:" + mediaformat.toString());
            } else if (outputBufferIndex == MediaCodec.INFO_TRY_AGAIN_LATER) {
                Log.d(TAG, "encoder dequeueOutputBuffer: No buffer available...");
            } else {
                Log.d(TAG, "encoder Message outputBufferIndex: " + outputBufferIndex);
            }

            return null;
        } catch (Exception ex) {
            Log.e(TAG, "getEcoderData Exception #################################");
            Log.e(TAG, "encoder " + Log.getStackTraceString(ex));
        }

        return null;
    }

    public static void stopEcoder() {
        try {
            Log.d(TAG, "stopEcoder");
            if (encoder != null)
                encoder.stop();

            if (encoder != null)
                encoder.release();

            encoder = null;
            Log.e(TAG, "encoder.stop");

        } catch (Exception e) {
            Log.e(TAG, "stopEcoder " + Log.getStackTraceString(e));
        }
    }


    public static boolean setDecoder(int rate) {
        int minBufferSize = AudioRecord.getMinBufferSize(
                Config.RECORD_SAMPLE_RATE,
                Config.RECORD_CHANNELS,
                Config.RECORD_AUDIO_ENCODING);

        IntDecoderCount = (int) minBufferSize / PACKSIZE;

        Log.d(TAG, "Decoder " + minBufferSize + " bytes AudioRecord.getMinBufferSize");

        try {
            decoder = MediaCodec.createDecoderByType(AudioType);
            //decoder = MediaCodec.createByCodecName("OMX.qcom.audio.decoder.aac");

        } catch (IOException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        }
        Log.d(TAG, "setDecoder");
        //语音编码
        formatdecoder = new MediaFormat();
        formatdecoder.setString(MediaFormat.KEY_MIME, AudioType);
        formatdecoder.setInteger(MediaFormat.KEY_CHANNEL_COUNT, 1);
        formatdecoder.setInteger(MediaFormat.KEY_AAC_PROFILE, MediaCodecInfo.CodecProfileLevel.AACObjectELD);
        formatdecoder.setInteger(MediaFormat.KEY_SAMPLE_RATE, rate); //Config.RECORD_SAMPLE_RATE
        formatdecoder.setInteger(MediaFormat.KEY_BIT_RATE, KEY_BIT_RATE);//AAC-HE 8kbps
        formatdecoder.setInteger(MediaFormat.KEY_MAX_INPUT_SIZE, minBufferSize * 4);

        decoder.configure(formatdecoder, null, null, 0);

        return true;
    }

    public static void startDecoder() {
        try {
            Log.d(TAG, ">>>>>>>>>>=========================== startDecoder");
            if (decoder == null) return;
            decoder.start();
            inputBuffers = decoder.getInputBuffers();
            outputBuffers = decoder.getOutputBuffers();
            bufferInfo = new MediaCodec.BufferInfo();

        } catch (Exception e) {
            Log.e(TAG, Log.getStackTraceString(e));
        }
    }

    private void handleHeadsetStateChange(int i) {
        try {
            BluetoothAdapter adapter = BluetoothAdapter.getDefaultAdapter();

            if (mAudioManager == null) {
                mAudioManager = (AudioManager) context.getSystemService(Context.AUDIO_SERVICE);
            }
            if (mAudioManager == null) {
                return;
            }
            mAudioManager.setMode(AudioManager.MODE_IN_COMMUNICATION);  //原来的 AudioManager.MODE_IN_COMMUNICATION
            mAudioManager.setSpeakerphoneOn(true);

            if (BluetoothProfile.STATE_CONNECTED == adapter.getProfileConnectionState(BluetoothProfile.HEADSET)) {
                //手机连接上蓝牙耳机
                // Toast.makeText(mContext,"连接上蓝牙耳机", Toast.LENGTH_SHORT).show();
                Log.i(TAG, "connect to blooth er");
                if (i == 1) {
                    mAudioManager.startBluetoothSco();
                    mAudioManager.setBluetoothScoOn(true); // 打开SCO
                }
                if (i == 0) {
                    mAudioManager.setBluetoothScoOn(false);
                    mAudioManager.stopBluetoothSco();
                }

                mAudioManager.adjustVolume(AudioManager.ADJUST_RAISE, 0);
            } else if (BluetoothProfile.STATE_DISCONNECTED == adapter.getProfileConnectionState(BluetoothProfile.HEADSET)) {
                //未连接
                // Toast.makeText(mContext, "蓝牙设备链接已断开!", Toast.LENGTH_SHORT).show();
                Log.i(TAG, "disconnect to blooth er");
                if (mAudioManager.isBluetoothScoOn()) {
                    mAudioManager.setBluetoothScoOn(false);
                    mAudioManager.stopBluetoothSco();
                }
            }
        } catch (Exception e) {
            Log.e(TAG, Log.getStackTraceString(e));
        }
    }


    // 音频焦点监听器
    private AudioManager.OnAudioFocusChangeListener audioFocusListener =
            new AudioManager.OnAudioFocusChangeListener() {
                @Override
                public void onAudioFocusChange(int focusChange) {
                    switch (focusChange) {
                        case AudioManager.AUDIOFOCUS_LOSS:
                            // 被永久抢占（如来电），停止对讲
                            if (pttService!=null && pttService.getEventListener()!=null)
                                pttService.getEventListener().onMicOccupied();
                            break;
                        case AudioManager.AUDIOFOCUS_LOSS_TRANSIENT:
                            // 被临时抢占（如语音助手），暂停对讲
                            if (pttService!=null && pttService.getEventListener()!=null)
                                pttService.getEventListener().onMicOccupied();
                            break;
                        case AudioManager.AUDIOFOCUS_GAIN:
                            // 重新获取焦点，恢复对讲

                            break;
                    }
                }
            };


    //检查音频焦点
    private boolean checkAudioFocus(){
        int result = mAudioManager.requestAudioFocus(
                audioFocusListener,
                AudioManager.STREAM_VOICE_CALL,  // 使用语音通话流（高优先级）
                AudioManager.AUDIOFOCUS_GAIN_TRANSIENT_EXCLUSIVE     // 临时独占
        );
        if (result != AudioManager.AUDIOFOCUS_REQUEST_GRANTED) {
            Log.e(TAG, "获取音频焦点失败，无法对讲");
            if (pttService!=null && pttService.getEventListener()!=null)
                pttService.getEventListener().onMicOccupied();
            return false;
        }
        return true;
    }

    /**
     * 实际采集语音
     */
    public void readAudio() {

        //采集前先获取下音频焦点
        /*if (!checkAudioFocus())
            return;*/

        //更高优先级的音频线程，适用于超低延迟需求（如实时通话、专业音频处理）
        Process.setThreadPriority(Process.THREAD_PRIORITY_URGENT_AUDIO);
        //Process.setThreadPriority(Process.THREAD_PRIORITY_AUDIO);

        int minBufferSize = AudioRecord.getMinBufferSize(
                Config.RECORD_SAMPLE_RATE, Config.RECORD_CHANNELS,
                AudioFormat.ENCODING_PCM_16BIT);

        Log.d(TAG, "readAudio " + minBufferSize + " bytes AudioRecord.getMinBufferSize");

        int bufferSize = PACKSIZE;  //PACKSIZE= 320, 节320个字节，160个short
        int read = 0;
        int result = 0;

        byte[] outdata = null;
        byte[] buffer1 = new byte[bufferSize];
        short[] buffer1Short = new short[160];
        ByteBuffer buffer = ByteBuffer.allocate(4 * bufferSize);

        try {

            int c = -1;
            int i = -1;
            PTTSessionManager.recordedQueue2.clear();

            int count_start_recoding = 0;
            while (++c < PACKSIZE)
                NullSoundData[c] = (byte) 0xff;


            audioRecord.startRecording();

            if (audioRecord.getRecordingState() != AudioRecord.RECORDSTATE_RECORDING) {
                if (pttService!=null && pttService.getEventListener()!=null){
                    pttService.getEventListener().onMicOccupied(); //发出占用事件
                }
                return;
            }

            int out_analog_level = 200;

            short[] tmpIn = new short[PACKSIZE / 2];
            byte[] aecBuf = new byte[PACKSIZE];

            short[] aecTmpIn = new short[PACKSIZE / 2];
            short[] aecTmpOut_agc = new short[PACKSIZE / 2];

            while (PTTSessionManager.recording) {

                try {
                    Log.d(TAG, "recording");
                    result = read = audioRecord.read(buffer1, 0, bufferSize);

                    if (result == AudioRecord.ERROR_INVALID_OPERATION ||
                            result == AudioRecord.ERROR_BAD_VALUE) {
                        Log.e(TAG, "readAudio2  An error occured with the AudioRecord API !");
                    }

                    if (read > 0) {

                        //默认使用webrtc的aecm
                        Log.d(TAG,"读取到pcm数据,长度:"+ read);

                        ByteBuffer.wrap(buffer1).order(ByteOrder.LITTLE_ENDIAN).asShortBuffer().get(tmpIn);

                        if (read / 2 == tmpIn.length) {

                            for (int j = 0; j < AEC_LOOP_COUNT; ++j) {
                                int processBufferOffSet = j * tmpIn.length / AEC_LOOP_COUNT;

                                _apm.SetStreamDelay(vm.getAceBufferDelayMs());
                                if (vm.getAgc()) {
                                    _apm.AGCSetStreamAnalogLevel(out_analog_level);
                                }

                                _apm.ProcessCaptureStream(tmpIn, processBufferOffSet);

                                if (vm.getAgc()) {
                                    out_analog_level = _apm.AGCStreamAnalogLevel();
                                    Log.i(TAG, "readAudio2 out_analog_level=" + out_analog_level);

                                }

                                if (vm.getVad()) {
                                    if (!_apm.VADHasVoice()) continue;
                                }
                            }

                            //
                            ByteBuffer.wrap(aecBuf).order(ByteOrder.LITTLE_ENDIAN).asShortBuffer().put(tmpIn);
                            buffer.put(aecBuf, 0, read);

                            buffer.flip();
                            Log.i(TAG, "1  buffer   position:" + buffer.position() + " limit:" + buffer.limit() + " remaining:" + buffer.remaining());
                            outdata = new byte[PACKSIZE];
                            buffer.get(outdata, 0, PACKSIZE);

                            //Log.i(TAG,"采集pcm语音包:"+ ByteTool.bytesToHex(outdata));

                            Log.i(TAG, "2  buffer   position:" + buffer.position() + " limit:" + buffer.limit() + " remaining:" + buffer.remaining());


                            Log.i(TAG, "readAudio2 =========================================recordedQueue");
                            PTTSessionManager.recordedQueue2.put(ByteBuffer.wrap(outdata, 0, PACKSIZE));    //写入列队

                            buffer.clear();

                        }


                    }
                } catch (IllegalStateException e) {

                    if (pttService!=null && pttService.getEventListener()!=null){
                        pttService.getEventListener().onMicOccupied(); //发出占用事件
                    }

                } catch  (Exception ex) {
                    Log.e(TAG,
                            "readAudio2 Exception #################################");
                    Log.e(TAG, "encoder " + Log.getStackTraceString(ex));
                }
            }
        } catch (Exception e) {
            Log.e(TAG, "readAudio2 Exception EEEEEEEEEEEEEEEEEEEEEEEEEEE");
            Log.e(TAG, "readAudio2 " + Log.getStackTraceString(e));
        } finally {
            audioRecord.stop();
        }

    }


    /**
     * 设置开始采集语音的开关及准备工作
     */
    public void startRecord() {

        Log.d(TAG, "audioRecord startRecord ......");

        try {

            PTTSessionManager.recording = true;
            //android 自带的AEC不行
            //以下是对蓝牙耳机的处理
            //handleHeadsetStateChange(1);//打开sco
            //OnStartRecord();  //这个一般用于开始本地写录音文件的回调
        } catch (Exception e) {
            Log.e(TAG, Log.getStackTraceString(e));
            Log.e(TAG, "start record is exception");
        }
    }

    public void stopRecord() {

        Log.d(TAG, "audioRecord stopRecord ......");
        try {

            boolean preRecording = PTTSessionManager.recording;
            PTTSessionManager.recording = false;
            //handleHeadsetStateChange(0);//关闭sco
            //OnStopRecord();  //TODO 本地的录音存储

            try {
                if (preRecording)
                    Thread.sleep(1);
            } catch (Exception ex) {
                Log.e(TAG, "stopRecord:\n" + Log.getStackTraceString(ex));
            }
        } catch (Exception e) {
            Log.e(TAG, Log.getStackTraceString(e));
        }
    }

    public static byte[] playData(ByteBuffer sb) {
        // recv net data ==> Play ==> DecoderData;
        try {
            //获取播放列表
            if (sb == null) {
                Log.e(TAG, "playData sb is null");
                return null;
            }

            byte[] tmp = getDecoderData(sb.array());

            if (tmp == null) {
                Log.e(TAG, "playData getDecoderData is null ");
                return null;
            }
            /*if (tmp.length < PACKSIZE) {
                Log.e(TAG, "playData getDecoderData length:" + tmp.length);
                return null;
            }*/

            return tmp;
        } catch (Exception e) {
            Log.e(TAG, "playData InterruptedException :" + e.getMessage());
        }

        Log.d(TAG, "==================== playData end");
        return null;

    }

    public static void stopDecoder() {
        try {
            Log.d(TAG, "stopDecoder ===========================<<<<<<<<<<<<<");
            if (decoder == null) return;
            decoder.stop();
        } catch (Exception e) {
            Log.e(TAG, Log.getStackTraceString(e));
        }
    }

    public static void releaseDecoder() {
        try {
            Log.d(TAG, "releaseDecoder ===========================<<<<<<<<<<<<<");
            if (decoder == null) return;
            decoder.release();
            decoder = null;
        } catch (Exception e) {
            Log.e(TAG, Log.getStackTraceString(e));
        }
    }

    private static void resetDecoder() {
        Log.e(TAG, "==================== resetDecoder Start ==========================");
        try {
            Log.e(TAG, "==================== stopDecoder ==========================");
            stopDecoder();
            Log.e(TAG, "==================== releaseDecoder ==========================");
            releaseDecoder();

            Log.e(TAG, "==================== setDecoder ==========================");
            setDecoder(Config.PLAY_SAMPLE_RATE);

            Log.e(TAG, "==================== startDecoder ==========================");
            startDecoder();

            Log.e(TAG, "==================== resetDecoder End ==========================");
        } catch (Exception ex) {
            Log.e(TAG, "==================== resetDecoder Exception:");
            Log.e(TAG, Log.getStackTraceString(ex));
        }
    }

    private static byte[] getDecoderData(byte[] data) {

        int inputBufferIndex = 0;
        int outputBufferIndex = 0;

        byte[] outData = null;
        ByteBuffer outBuf = null;

        Log.d(TAG, "==================== getDecoderData start");

        if (decoder == null) {
            Log.d(TAG, "==================== getDecoderData: decoder==null");
            resetDecoder();
            return null;
        }

        //amrnb的一帧数据至少13个字节
        if (data == null || data.length < 13) {
            Log.d(TAG, "==================== getDecoderData: data==null || data.length < 13 ");
            return null;
        }

        try {

            inputBufferIndex = decoder.dequeueInputBuffer(TIMEOUT_USEC); // TIMEOUT_USEC
            Log.d(TAG, "decoder.dequeueInputBuffer inputBufferIndex:" + inputBufferIndex);

            if (inputBufferIndex >= 0) {
                inputBuffer = inputBuffers[inputBufferIndex];
                inputBuffer.clear();
                inputBuffer.put(data);
                decoder.queueInputBuffer(inputBufferIndex, 0, data.length, System.nanoTime() / 1000, 0);
            } else if (inputBufferIndex == MediaCodec.INFO_TRY_AGAIN_LATER) {
                Log.d(TAG, "dequeueInputBuffer : No buffer available...");
            }

            //解码后
            outputBufferIndex = decoder.dequeueOutputBuffer(bufferInfo, 10); //
            Log.d(TAG, "decoder.dequeueOutputBuffer outputBufferIndex:" + outputBufferIndex);

            if (outputBufferIndex >= 0) {
                outputBuffer = outputBuffers[outputBufferIndex];
                if (outputBuffer == null) return null;

                outputBuffer.position(bufferInfo.offset);
                outputBuffer.limit(bufferInfo.offset + bufferInfo.size);
                outputBuffer.position(bufferInfo.offset);

                outData = new byte[bufferInfo.size];
                outputBuffer.get(outData, 0, bufferInfo.size);

                Log.i(TAG, "解码前 msgLen:" + data.length + " 解码后 len:" + bufferInfo.size);

                decoder.releaseOutputBuffer(outputBufferIndex, false);
                return outData;

            } else if (outputBufferIndex == MediaCodec.INFO_OUTPUT_BUFFERS_CHANGED) {
                outputBuffers = decoder.getOutputBuffers();
                Log.d(TAG, "OUTPUT_BUFFERS_CHANGED...");

            } else if (outputBufferIndex == MediaCodec.INFO_OUTPUT_FORMAT_CHANGED) {
                MediaFormat mediaformat = decoder.getOutputFormat();
                Log.d(TAG, "OUTPUT_FORMAT_CHANGED:" + mediaformat.toString());

            } else if (outputBufferIndex == MediaCodec.INFO_TRY_AGAIN_LATER) {
                Log.d(TAG, "No buffer available...");

            } else {
                Log.d(TAG, "Message outputBufferIndex:" + outputBufferIndex);

            }

        } catch (Exception e) {
            Log.e(TAG, "getDecoderData Exception");
            Log.e(TAG, Log.getStackTraceString(e));
        }
        Log.d(TAG, "==================== getDecoderData end");
        return null;
    }


    public static void decoderData(byte[] data) {
        //recv net data ==> Play ==> DecoderData;
        recvNetData(data);
    }

    /**
     * @param data
     */
    //todo 收到网络包
    private static void recvNetData(byte[] data) {

        try {

            Log.i(TAG, "data长度" + data.length);

            //if (data == null || data.length < 13) return;
            if (PTTSessionManager.getInstance().isPlayAudio()) {
                Log.i(TAG, "=================decodedQueue");

                short[] buffer = PTTSessionManager.Producer_Get(500L);
                if (buffer != null) {
                    PTTSessionManager.Producer_Put(ByteBuffer.wrap(data, 0, data.length));
                }

            } else {
                //aec期间，发到另外一个链表存储
                //丢掉
            }

        } catch (InterruptedException e) {
            Log.e(TAG, "recvNetData InterruptedException :" + e.getMessage());
        }
    }

    /**
     * 收到文件的语音帧
     *
     * @param data
     */
    public static void recvFileAudioFrame(byte[] data) {

        try {

            Log.i(TAG, "FileAudioFrame data长度" + data.length);

            Log.i(TAG, "=================decodedQueue");
            //webrtc aec框架
            PTTSessionManager.Producer_Put(ByteBuffer.wrap(data, 0, data.length));


        } catch (InterruptedException e) {
            Log.e(TAG, "recvNetData InterruptedException :" + e.getMessage());
        }
    }

    public void release() {

        Log.d(TAG, "audioRecord release......");

        if (audioRecord != null)
            audioRecord.release();

        if (acousticEchoCanceler != null)
            acousticEchoCanceler.setEnabled(false);

        if (noiseSuppressor != null)
            noiseSuppressor.setEnabled(false);

        if (_apm != null) {
            _apm.close();
        }

        audioRecord = null;

    }

}
