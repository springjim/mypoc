using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace POCControlCenter
{
    public static class DESEncrypt
    {


        #region ========加密========

        private static string strKey = "Jim@2*Ru";
        private static string strIV = "LiuJineagelJim@1p=";

        /// <summary>
        /// 加密数据
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string Encrypt(string _strQ)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(_strQ);
            MemoryStream ms = new MemoryStream();
            DESCryptoServiceProvider tdes = new DESCryptoServiceProvider();
            CryptoStream encStream = new CryptoStream(ms, tdes.CreateEncryptor(Encoding.UTF8.GetBytes(strKey), Encoding.UTF8.GetBytes(strIV)), CryptoStreamMode.Write);
            encStream.Write(buffer, 0, buffer.Length);
            encStream.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray()).Replace("+", "%");

        }

        #endregion

        #region ========解密========


        /// <summary>
        /// 解密数据
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string Decrypt(string _strQ)
        {
            _strQ = _strQ.Replace("%", "+");
            byte[] buffer = Convert.FromBase64String(_strQ);
            MemoryStream ms = new MemoryStream();
            DESCryptoServiceProvider tdes = new DESCryptoServiceProvider();
            CryptoStream encStream = new CryptoStream(ms, tdes.CreateDecryptor(Encoding.UTF8.GetBytes(strKey), Encoding.UTF8.GetBytes(strIV)), CryptoStreamMode.Write);
            encStream.Write(buffer, 0, buffer.Length);
            encStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(ms.ToArray());

        }

        #endregion       

    }
}
