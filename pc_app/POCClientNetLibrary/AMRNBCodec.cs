using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POCClientNetLibrary
{
    public class AMRNBCodec : IDisposable
    {
        public AMRNBCodec()
        {
            NativeMethods.AmrDecoder.init();
            NativeMethods.AmrEncoder.init(0);
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the hook
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                NativeMethods.AmrDecoder.exit();
                NativeMethods.AmrEncoder.exit();
            }
        }

        private int Encode(byte[] outputBuffer, short[] inputBuffer, int inputBufferCount)
        {
            return NativeMethods.AmrEncoder.encode(0, inputBuffer, outputBuffer);
        }
        private int Decode(short[] outputBuffer, byte[] inputData, int inputLength)
        {
            NativeMethods.AmrDecoder.decode(inputData, outputBuffer);
            return 0;
        }
        public int Encode(ref byte[] outputBuffer, byte[] inputBuffer, int inputBufferCount)
        {
            outputBuffer   = new byte[13];
            var waveBuffer = new WaveBuffer(inputBuffer);
            return Encode(outputBuffer, waveBuffer.ShortBuffer, inputBufferCount/2 );
        }
        public int Decode(ref byte[] outputBuffer, byte[] inputData, int inputLength)
        {
            outputBuffer   = new byte[320];
            var wareBuffer = new WaveBuffer(outputBuffer);
            Decode(wareBuffer.ShortBuffer, inputData, inputLength);
            return 0;
        }
    }

    public static class NativeMethods
    {
        public enum Mode
        {
            MR475 = 0,/* 4.75 kbps */
            MR515,    /* 5.15 kbps */
            MR59,     /* 5.90 kbps */
            MR67,     /* 6.70 kbps */
            MR74,     /* 7.40 kbps */
            MR795,    /* 7.95 kbps */
            MR102,    /* 10.2 kbps */
            MR122,    /* 12.2 kbps */
            MRDTX,    /* DTX       */
            N_MODES   /* Not Used  */
        }
        private static IntPtr enstate = IntPtr.Zero;
        private static IntPtr destate = IntPtr.Zero;

        [DllImport("libopencore-amrnb-0.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Encoder_Interface_init(int dtx);
        [DllImport("libopencore-amrnb-0.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Encoder_Interface_exit(IntPtr state);
        [DllImport("libopencore-amrnb-0.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Encoder_Interface_Encode(IntPtr state, Mode mode, short[] speech, byte[] outData);
        public static class AmrEncoder
        {
            public static void init(int dtx)
            {
                if(enstate==IntPtr.Zero)
                   enstate = Encoder_Interface_init(dtx);
            }
            public static int encode(int mode, short[] inD, byte[] outD)
            {
                if(enstate==IntPtr.Zero)
                    return 0;
                return Encoder_Interface_Encode(enstate, (Mode)mode, inD, outD);
            }
            public static void exit()
            {
                if( enstate != IntPtr.Zero )
                    Encoder_Interface_exit(enstate);
                enstate = IntPtr.Zero;
            }
        }


        [DllImport("libopencore-amrnb-0.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Decoder_Interface_init();
        [DllImport("libopencore-amrnb-0.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Decoder_Interface_exit(IntPtr state);
        [DllImport("libopencore-amrnb-0.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Decoder_Interface_Decode(IntPtr state, byte[] inData, short[] outData);
        public static class AmrDecoder
        {
            public static void init()
            {
                if (destate == IntPtr.Zero)
                    destate = Decoder_Interface_init();
            }
            public static void exit( )
            {
                if (destate != IntPtr.Zero)
                    Decoder_Interface_exit(destate);
                destate = IntPtr.Zero;
            }
            public static void decode( byte[] inD, short[] outD)
            {
                if (destate != IntPtr.Zero)
                    Decoder_Interface_Decode(destate, inD, outD);
            }
        }
    }


}
