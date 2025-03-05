using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.CompilerServices;
using System.Diagnostics;


namespace POCClientNetLibrary
{
    public static class CLog
    {
        #region Server
        public const string ServerStarted = "Server started";
        public const string ServerStopped = "Server stopped";
        #endregion

        #region Client
        public const string ClientConnected    = "Client conencted";
        public const string ClientDisconnected = "Client disconnected";
        public const string TcpClientUnexpected = "TCP Connection closed unexpectedly";
        public const string UdpClientUnexpected = "UDP Connection closed unexpectedly";
        #endregion

        #region Common
        public const string ApplicationName = "POCControlCenter";
        public const string Application     = "Application";
        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="msg">Custom message</param>
        /// <param name="e">Event</param>
        /// <returns></returns>
        public static string Message(string entity, string msg, string e)
        {
            return string.Format("{0} {1} {2}", entity, msg, e);
        }

        /// <summary>
        /// 字节数组转16进制字符串
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static string ByteArrayToStr(byte[] data, int pos)
        {
            StringBuilder sBuilder = new StringBuilder();
            // Loop through each byte of the hashed data   
            // and format each one as a hexadecimal string.   
            for ( int i = pos; i < data.Length; i++ )
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.   
            return sBuilder.ToString();
        }


        #endregion
    }

}
