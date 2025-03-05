using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace POCControlCenter
{
    public class VlcPlayer
    {
       
        //
        public IntPtr libvlc_instance_;
        public IntPtr libvlc_media_player_;
        private double duration_;
        private static string pluginPath = System.Environment.CurrentDirectory + "\\plugins\\";
        public VlcPlayer()
        { 
            string plugin_arg = "--plugin-path=" + pluginPath;
            string[] arguments = { "--avcodec-threads=124", "--rtsp-frame-buffer-size=1000000", "--network-caching=1200", "--rtsp-tcp", plugin_arg };
            //string[] arguments = { "-I", "dummy", "--ignore-config", "--no-video-title", plugin_arg };
            libvlc_instance_ = LibVlcAPI.libvlc_new(arguments);

            libvlc_media_player_ = LibVlcAPI.libvlc_media_player_new(libvlc_instance_);
        }
       


        public void SetRenderWindow(int wndHandle)
        {
            if (libvlc_instance_ != IntPtr.Zero && wndHandle != 0)
            {
                LibVlcAPI.libvlc_media_player_set_hwnd(libvlc_media_player_, wndHandle);
            }
        }

        public void PlayURL(string URL)
        {
            IntPtr libvlc_media = LibVlcAPI.libvlc_media_new_location(libvlc_instance_, URL);
            if (libvlc_media != IntPtr.Zero)
            {
                LibVlcAPI.libvlc_media_parse(libvlc_media);
                duration_ = LibVlcAPI.libvlc_media_get_duration(libvlc_media) / 1000.0;
                LibVlcAPI.libvlc_media_player_set_media(libvlc_media_player_, libvlc_media);
                LibVlcAPI.libvlc_media_release(libvlc_media);
                LibVlcAPI.libvlc_media_player_play(libvlc_media_player_);
            }
        }

        public bool IsPlaying()
        {
            try
            {
                if (libvlc_media_player_ == IntPtr.Zero ||
                    libvlc_media_player_ == null)
                {
                    return false;
                }

                return LibVlcAPI.libvlc_media_player_is_playing(libvlc_media_player_);
            }
            catch (Exception e2)
            {
                MessageBox.Show("MediaPlayer_IsPlaying" + e2.Message);
                return false;
            }
        }

        public void SetVideoAspectRatio_Origin()
        {
            try
            {
                if (libvlc_media_player_ == IntPtr.Zero ||
                    libvlc_media_player_ == null)
                {
                    return;
                }

                LibVlcAPI.libvlc_video_set_aspect_ratio(libvlc_media_player_, null);
            }
            catch (Exception e2)
            {
                MessageBox.Show("SetVideoAspectRatio:" + e2.Message);

                return;
            }
        }

        public  void SetVideoAspectRatio(string aspect_ratio)
        {
            try
            {
                if (libvlc_media_player_ == IntPtr.Zero ||
                    libvlc_media_player_ == null)
                {
                    return;
                }

                LibVlcAPI.libvlc_video_set_aspect_ratio(libvlc_media_player_, aspect_ratio.ToCharArray());
            }
            catch (Exception e2)
            {
                MessageBox.Show("SetVideoAspectRatio:"+e2.Message);

                return;
            }
        }


        public void PlayFile(string filePath)
        {
            IntPtr libvlc_media = LibVlcAPI.libvlc_media_new_path(libvlc_instance_, filePath);
            if (libvlc_media != IntPtr.Zero)
            {
                LibVlcAPI.libvlc_media_parse(libvlc_media);
                duration_ = LibVlcAPI.libvlc_media_get_duration(libvlc_media) / 1000.0;
                LibVlcAPI.libvlc_media_player_set_media(libvlc_media_player_, libvlc_media);
                LibVlcAPI.libvlc_media_release(libvlc_media);
                LibVlcAPI.libvlc_media_player_play(libvlc_media_player_);
            }
        }

        public void Pause()
        {
            if (libvlc_media_player_ != IntPtr.Zero)
            {
                LibVlcAPI.libvlc_media_player_pause(libvlc_media_player_);
            }
        }
        public void Play()
        {
            if (libvlc_media_player_ != IntPtr.Zero)
            {
                LibVlcAPI.libvlc_media_player_play(libvlc_media_player_);
                //  LibVlcAPI.libvlc_media_player_pause(libvlc_media_player_);
            }
        }
        public void Stop()
        {
            //根据经验，以下是要求放到异步中去执行

            Task.Run(() =>
            {
                if (libvlc_media_player_ != IntPtr.Zero)
                {
                    //SetRenderWindow(IntPtr.Zero.ToInt32());
                    LibVlcAPI.libvlc_media_player_stop(libvlc_media_player_);
                }
            });                      

           

        }
        //  public void FastForward()
        // {
        //    if (libvlc_media_player_ != IntPtr.Zero)
        //   {
        //      LibVlcAPI.libvlc_media_player_fastforward(libvlc_media_player_);
        // }
        // }
        public double GetPlayTime()
        {
            return LibVlcAPI.libvlc_media_player_get_time(libvlc_media_player_) / 1000.0;
        }
        public void SetPlayTime(double seekTime)
        {
            LibVlcAPI.libvlc_media_player_set_time(libvlc_media_player_, (Int64)(seekTime * 1000));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f_pos">0.0~1.0之间</param>
        public void SetPlayPos(float f_pos)
        {
            LibVlcAPI.libvlc_media_player_set_position(libvlc_media_player_, f_pos);
        }

        public int GetVolume()
        {
            return LibVlcAPI.libvlc_audio_get_volume(libvlc_media_player_);
        }
        public void SetVolume(int volume)
        {
            LibVlcAPI.libvlc_audio_set_volume(libvlc_media_player_, volume);
        }
        public void SetFullScreen(bool istrue)
        {
            LibVlcAPI.libvlc_set_fullscreen(libvlc_media_player_, istrue ? 1 : 0);
        }
        public double Duration()
        {
            return duration_;
        }
        public string Version()
        {
            return LibVlcAPI.libvlc_get_version();
        }

        /// <summary>
        /// 录制快照
        /// </summary>
        /// <param name="libvlc_media_player">VLC MediaPlayer变量</param>
        /// <param name="path">快照要存放的路径</param>
        /// <param name="name">快照保存的文件名称</param>
        /// <returns></returns>
        public  bool TakeSnapShot(string path, string name)
        {
            try
            {
                string snap_shot_path = null;

                if (libvlc_media_player_ == IntPtr.Zero ||
                    libvlc_media_player_ == null)
                {
                    return false;
                }

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                snap_shot_path = path + "\\" + name;

                if (0 == LibVlcAPI.libvlc_video_take_snapshot(libvlc_media_player_, 0, snap_shot_path.ToCharArray(), 0, 0))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }

    }
    internal static class LibVlcAPI
    {
        internal struct PointerToArrayOfPointerHelper
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
            public IntPtr[] pointers;
        }
        public static IntPtr libvlc_new(string[] arguments)
        {
            PointerToArrayOfPointerHelper argv = new PointerToArrayOfPointerHelper();
            argv.pointers = new IntPtr[11];
            for (int i = 0; i < arguments.Length; i++)
            {
                argv.pointers[i] = Marshal.StringToHGlobalAnsi(arguments[i]);
            }
            IntPtr argvPtr = IntPtr.Zero;
            try
            {
                int size = Marshal.SizeOf(typeof(PointerToArrayOfPointerHelper));
                argvPtr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(argv, argvPtr, false);
                return libvlc_new(arguments.Length, argvPtr);
            }
            finally
            {
                for (int i = 0; i < arguments.Length + 1; i++)
                {
                    if (argv.pointers[i] != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(argv.pointers[i]);
                    }
                }
                if (argvPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(argvPtr);
                }
            }
        }
        public static IntPtr libvlc_media_new_path(IntPtr libvlc_instance, string path)
        {
            IntPtr pMrl = IntPtr.Zero;
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(path);
                pMrl = Marshal.AllocHGlobal(bytes.Length + 1);
                Marshal.Copy(bytes, 0, pMrl, bytes.Length);
                Marshal.WriteByte(pMrl, bytes.Length, 0);
                return libvlc_media_new_path(libvlc_instance, pMrl);
            }
            finally
            {
                if (pMrl != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pMrl);
                }
            }
        }       


        public static IntPtr libvlc_media_new_location(IntPtr libvlc_instance, string path)
        {
            IntPtr pMrl = IntPtr.Zero;
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(path);
                pMrl = Marshal.AllocHGlobal(bytes.Length + 1);
                Marshal.Copy(bytes, 0, pMrl, bytes.Length);
                Marshal.WriteByte(pMrl, bytes.Length, 0);
                //return libvlc_media_new_path(libvlc_instance, pMrl);
                return libvlc_media_new_location(libvlc_instance, pMrl);
            }
            finally
            {
                if (pMrl != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pMrl);
                }
            }
        }

        // ----------------------------------------------------------------------------------------
        // 以下是libvlc.dll导出函数

        // 创建一个libvlc实例，它是引用计数的
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        private static extern IntPtr libvlc_new(int argc, IntPtr argv);

        // 释放libvlc实例
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_release(IntPtr libvlc_instance);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern String libvlc_get_version();

        // 从视频来源(例如Url)构建一个libvlc_meida
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        private static extern IntPtr libvlc_media_new_location(IntPtr libvlc_instance, IntPtr path);

        // 从本地文件路径构建一个libvlc_media
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        private static extern IntPtr libvlc_media_new_path(IntPtr libvlc_instance, IntPtr path);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_release(IntPtr libvlc_media_inst);

        // 创建libvlc_media_player(播放核心)
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern IntPtr libvlc_media_player_new(IntPtr libvlc_instance);

        // 将视频(libvlc_media)绑定到播放器上
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_player_set_media(IntPtr libvlc_media_player, IntPtr libvlc_media);

        // 设置图像输出的窗口
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_player_set_hwnd(IntPtr libvlc_mediaplayer, Int32 drawable);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_player_play(IntPtr libvlc_mediaplayer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="libvlc_mediaplayer"></param>
        //[DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        //[SuppressUnmanagedCodeSecurity]
        // public static extern void libvlc_media_player_fastforward(IntPtr libvlc_mediaplayer);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_player_pause(IntPtr libvlc_mediaplayer);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_player_stop(IntPtr libvlc_mediaplayer);

        // 解析视频资源的媒体信息(如时长等)
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_parse(IntPtr libvlc_media);

        // 返回视频的时长(必须先调用libvlc_media_parse之后，该函数才会生效)
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern Int64 libvlc_media_get_duration(IntPtr libvlc_media);

        // 当前播放的时间
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern Int64 libvlc_media_player_get_time(IntPtr libvlc_mediaplayer);

        // 设置播放位置(拖动)
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_player_set_time(IntPtr libvlc_mediaplayer, Int64 time);

        // 设置播放位置
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_player_set_position(IntPtr libvlc_mediaplayer, float f_pos);


        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_player_release(IntPtr libvlc_mediaplayer);

        // 获取和设置音量
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int libvlc_audio_get_volume(IntPtr libvlc_media_player);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_audio_set_volume(IntPtr libvlc_media_player, int volume);

        // 设置全屏
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_set_fullscreen(IntPtr libvlc_media_player, int isFullScreen);

        //长宽比例
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_video_set_aspect_ratio(IntPtr libvlc_media_player, char[] aspect_ratio );

        //判断播放时是否在播放
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern bool libvlc_media_player_is_playing(IntPtr libvlc_media_player);


        //进行快照
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int libvlc_video_take_snapshot(IntPtr libvlc_media_player, int num, char[] filepath, int i_width, int i_height);

    }
}
