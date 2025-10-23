package com.mypoc.pttlibrary.api;

import android.media.AudioFormat;

/**
 * 配置类
 */
public class PTTConfig {
    /**
     * webapi 服务地址
     */
    private final String restServerUrl;

    /**
     * socket服务地址
     */
    private final String tcpServerHost;
    /**
     * socket服务port
      */
    private final int tcpServerPort;

    private final int audioSampleRate;
    private final int audioChannelConfig;
    private final int audioFormat;
    /**
     * 心跳间隔(单位秒),默认30秒
     */
    private final int heartbeatIntervalSec;

    /**
     * ptt键值,如 131, 这个因厂家而异，要咨询各厂家的技术人员
     */
    private final int pttKeyVal;

    /**
     * ptt键按下的广播字符串,如 qmstar.keyflag.ptt.down, 这个因厂家而异，要咨询各厂家的技术人员
     */
    private final String pttDownBroadCastVal;

    /**
     * ptt键抬起的广播字符串,如 qmstar.keyflag.ptt.up, 这个因厂家而异，要咨询各厂家的技术人员
     */
    private final String pttUpBroadCastVal;

    /**
     * ptt键用键值监听，还是用广播方式监听， true: 用广播， false: 用键值
     */
    private final boolean pttUseBroadCastMode;

    /**
     *
     * 获取麦权的超时时间 （单位: 秒）
     * 默认60秒
     * 即讲话人获得了麦权，能最大允许多长时间讲话，一般这个要小于服务端设定的时间
     * 如服务端是1分钟，这里要小于它，
     */
    private final int micOwnerTimeoutSec;

    private PTTConfig(Builder builder) {
        this.restServerUrl = builder.restServerUrl;
        this.tcpServerHost = builder.tcpServerHost;
        this.tcpServerPort = builder.tcpServerPort;
        this.audioSampleRate = builder.audioSampleRate;
        this.audioChannelConfig = builder.audioChannelConfig;
        this.audioFormat = builder.audioFormat;
        this.heartbeatIntervalSec = builder.heartbeatIntervalSec;
        this.micOwnerTimeoutSec= builder.micOwnerTimeoutSec;

        this.pttKeyVal=builder.pttKeyVal;
        this.pttDownBroadCastVal= builder.pttDownBroadCastVal;
        this.pttUpBroadCastVal= builder.pttUpBroadCastVal;
        this.pttUseBroadCastMode= builder.pttUseBroadCastMode;


    }

    public static class Builder {

        private String restServerUrl;
        private String tcpServerHost;
        private int tcpServerPort;
        private int audioSampleRate = 8000;
        private int audioChannelConfig = AudioFormat.CHANNEL_IN_MONO;
        private int audioFormat = AudioFormat.ENCODING_PCM_16BIT;
        private int heartbeatIntervalSec= 30;  //30秒间隔
        private int micOwnerTimeoutSec=60;     //60秒

        private int pttKeyVal=131;
        private String pttDownBroadCastVal="qmstar.keyflag.ptt.down";
        private String pttUpBroadCastVal="qmstar.keyflag.ptt.up";
        private boolean pttUseBroadCastMode=true;


        public Builder setRestServerUrl(String url) {
            this.restServerUrl = url;
            return this;
        }

        public Builder setTcpServerHost(String host) {
            this.tcpServerHost = host;
            return this;
        }

        public Builder setTcpServerPort(int port) {
            this.tcpServerPort = port;
            return this;
        }

        public Builder setAudioSampleRate(int rate) {
            this.audioSampleRate = rate;
            return this;
        }

        public Builder setAudioChannelConfig(int config) {
            this.audioChannelConfig = config;
            return this;
        }

        public Builder setAudioFormat(int format) {
            this.audioFormat = format;
            return this;
        }

        public Builder setHeartbeatIntervalSec(int intervalSec) {
            this.heartbeatIntervalSec = intervalSec;
            return this;
        }

        public Builder setMicOwnerTimeoutSec(int micOwnerTimeoutSec) {
            this.micOwnerTimeoutSec = micOwnerTimeoutSec;
            return this;
        }

        public Builder setPttKeyVal(int pttKeyVal) {
            this.pttKeyVal = pttKeyVal;
            return this;
        }

        public Builder setPttDownBroadCastVal(String pttDownBroadCastVal) {
            this.pttDownBroadCastVal = pttDownBroadCastVal;
            return this;
        }

        public Builder setPttUpBroadCastVal(String pttUpBroadCastVal) {
            this.pttUpBroadCastVal = pttUpBroadCastVal;
            return this;
        }

        public Builder setPttUseBroadCastMode(boolean pttUseBroadCastMode) {
            this.pttUseBroadCastMode = pttUseBroadCastMode;
            return this;
        }

        /////////////////////////////
        public PTTConfig build() {
            return new PTTConfig(this);
        }

    }

    public int getMicOwnerTimeoutSec() {
        return micOwnerTimeoutSec;
    }

    public String getRestServerUrl() {
        return restServerUrl;
    }

    public String getTcpServerHost() {
        return tcpServerHost;
    }

    public int getTcpServerPort() {
        return tcpServerPort;
    }

    public int getAudioSampleRate() {
        return audioSampleRate;
    }

    public int getAudioChannelConfig() {
        return audioChannelConfig;
    }

    public int getAudioFormat() {
        return audioFormat;
    }

    public int getHeartbeatIntervalSec() {
        return heartbeatIntervalSec;
    }

    public int getPttKeyVal() {
        return pttKeyVal;
    }

    public String getPttDownBroadCastVal() {
        return pttDownBroadCastVal;
    }

    public String getPttUpBroadCastVal() {
        return pttUpBroadCastVal;
    }

    public boolean isPttUseBroadCastMode() {
        return pttUseBroadCastMode;
    }
}
