package com.mypoc.ptt.activity.backgroud;

import com.pedro.rtmp.utils.ConnectCheckerRtmp;
import com.pedro.rtsp.utils.ConnectCheckerRtsp;

public interface ConnectCheckerRtp extends ConnectCheckerRtmp, ConnectCheckerRtsp {
    /**
     * Commons
     */
    void onConnectionStartedRtp(String rtpUrl);

    void onConnectionSuccessRtp();

    void onConnectionFailedRtp(String reason);

    void onNewBitrateRtp(long bitrate);

    void onDisconnectRtp();

    void onAuthErrorRtp();

    void onAuthSuccessRtp();

    /**
     * RTMP
     */
    @Override
    default void onConnectionStartedRtmp(String rtmpUrl) {
        onConnectionStartedRtp(rtmpUrl);
    }

    @Override
    default void onConnectionSuccessRtmp() {
        onConnectionSuccessRtp();
    }

    @Override
    default void onConnectionFailedRtmp(String reason) {
        onConnectionFailedRtp(reason);
    }

    @Override
    default void onNewBitrateRtmp(long bitrate) {
        onNewBitrateRtp(bitrate);
    }

    @Override
    default void onDisconnectRtmp() {
        onDisconnectRtp();
    }

    @Override
    default void onAuthErrorRtmp() {
        onAuthErrorRtp();
    }

    @Override
    default void onAuthSuccessRtmp() {
        onAuthSuccessRtp();
    }

    /**
     * RTSP
     */
    @Override
    default void onConnectionStartedRtsp(String rtspUrl) {
        onConnectionStartedRtp(rtspUrl);
    }

    @Override
    default void onConnectionSuccessRtsp() {
        onConnectionSuccessRtp();
    }

    @Override
    default void onConnectionFailedRtsp(String reason) {
        onConnectionFailedRtp(reason);
    }

    @Override
    default void onNewBitrateRtsp(long bitrate) {
        onNewBitrateRtp(bitrate);
    }

    @Override
    default void onDisconnectRtsp() {
        onDisconnectRtp();
    }

    @Override
    default void onAuthErrorRtsp() {
        onAuthErrorRtp();
    }

    @Override
    default void onAuthSuccessRtsp() {
        onAuthSuccessRtp();
    }
}
