using POCControlCenter.DataEntity;
using POCControlCenter.Service.Entity;
using POCControlCenter.Service.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter
{
    /// <summary>
    /// Summary description for StrHelper.
    /// Named abbreviation  :
    /// Str: unicode string
    /// Arr: unicode array
    /// Hex: Binary data
    /// Hexbin:  Binary data in ASCII characters    Cases of characters  "1" Hex is  0x31 It is represented as hexbin   "3""1"
    /// Asc: ASCII
    /// Uni: UNICODE
    /// </summary>
    public sealed class StrHelper
    {
        #region Hex And the conversion of Hexbin
        public static void Hexbin2Hex(byte[] bHexbin, byte[] bHex, int nLen)
        {
            for (int i = 0; i < nLen / 2; i++)
            {
                if (bHexbin[2 * i] < 0x41)
                {
                    bHex[i] = Convert.ToByte(((bHexbin[2 * i] - 0x30) << 4) & 0xf0);
                }
                else
                {
                    bHex[i] = Convert.ToByte(((bHexbin[2 * i] - 0x37) << 4) & 0xf0);
                }
                if (bHexbin[2 * i + 1] < 0x41)
                {
                    bHex[i] |= Convert.ToByte((bHexbin[2 * i + 1] - 0x30) & 0x0f);
                }
                else
                {
                    bHex[i] |= Convert.ToByte((bHexbin[2 * i + 1] - 0x37) & 0x0f);
                }
            }
        }
        public static byte[] Hexbin2Hex(byte[] bHexbin, int nLen)
        {
            if (nLen % 2 != 0)
                return null;

            byte[] bHex = new byte[nLen / 2];
            Hexbin2Hex(bHexbin, bHex, nLen);

            return bHex;
        }
        public static void Hex2Hexbin(byte[] bHex, byte[] bHexbin, int nLen)
        {
            byte c;
            for (int i = 0; i < nLen; i++)
            {
                c = Convert.ToByte((bHex[i] >> 4) & 0x0f);
                if (c < 0x0a)
                {
                    bHexbin[2 * i] = Convert.ToByte(c + 0x30);
                }
                else
                {
                    bHexbin[2 * i] = Convert.ToByte(c + 0x37);
                }
                c = Convert.ToByte(bHex[i] & 0x0f);
                if (c < 0x0a)
                {
                    bHexbin[2 * i + 1] = Convert.ToByte(c + 0x30);
                }
                else
                {
                    bHexbin[2 * i + 1] = Convert.ToByte(c + 0x37);
                }
            }
        }
        public static byte[] Hex2Hexbin(byte[] bHex, int nLen)
        {
            byte[] bHexbin = new byte[nLen * 2];
            Hex2Hexbin(bHex, bHexbin, nLen);
            return bHexbin;
        }
        #endregion


        #region  Array and string conversion between

        public static byte[] Str2Arr(String s)
        {
            return (new UnicodeEncoding()).GetBytes(s);
        }
        public static string Arr2Str(byte[] buffer)
        {
            return (new UnicodeEncoding()).GetString(buffer, 0, buffer.Length);
        }

        public static byte[] Str2AscArr(String s)
        {
            return System.Text.UnicodeEncoding.Convert(
                System.Text.Encoding.Unicode,
                System.Text.Encoding.ASCII,
                Str2Arr(s)
                );
        }
        public static string AscArr2Str(byte[] b)
        {
            return System.Text.UnicodeEncoding.Unicode.GetString(
            System.Text.ASCIIEncoding.Convert(System.Text.Encoding.ASCII,System.Text.Encoding.Unicode,b)
            );
        }

        public static byte[] Str2HexAscArr(String s)
        {
            byte[] hex    = Str2AscArr(s);
            byte[] hexbin = Hex2Hexbin(hex, hex.Length);
            return hexbin;
        }
        public static string HexAscArr2Str(byte[] buffer)
        {
            byte[] b = Hex2Hexbin(buffer, buffer.Length);
            return AscArr2Str(b);
        }
        #endregion
    }

    public class Utils
    {

        public static DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// 将新接口的所有groupDto转成兼容旧的group类
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static List<Group> ConvertNewGroupList(GroupListResponse response)
        {
            List<Group> retList = new List<Group>();
            if (response.code==0 && response.data!=null)
            {
                foreach(GroupDto groupDto in response.data)
                {
                    Group group = new Group();
                    group.group_id = groupDto.groupId;
                    group.group_name = groupDto.groupName;
                    if (groupDto.aclass.HasValue)
                        group.aclass = groupDto.aclass.Value;
                    if (groupDto.ownerId.HasValue)
                        group.owner_id = groupDto.ownerId.Value;
                    retList.Add(group);
                }
            }

            return retList;
        }

        /// <summary>
        /// 将新接口的所有groupTempDto转成兼容旧的group类
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static List<Group> ConvertNewGroupTempList(GroupTempListResponse response)
        {
            List<Group> retList = new List<Group>();
            if (response.code == 0 && response.data != null)
            {
                foreach (GroupTempDto groupDto in response.data)
                {
                    Group group = new Group();
                    group.group_id = groupDto.groupId;
                    group.group_name = groupDto.groupName;
                    if (groupDto.aclass.HasValue)
                        group.aclass = groupDto.aclass.Value;
                    if (groupDto.ownerId.HasValue)
                        group.owner_id = groupDto.ownerId.Value;
                    group.user_ids = groupDto.userIds;
                    retList.Add(group);
                }
            }

            return retList;
        }

        ///   <summary>  
        ///   给一个字符串进行MD5加密  
        ///   </summary>  
        ///   <param   name="strText">待加密字符串</param>  
        ///   <returns>加密后的字符串</returns>  
        public static string MD5Encrypt(string strText)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            // byte[] result = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(strText));
            byte[] result = md5.ComputeHash(Encoding.GetEncoding("UTF-8").GetBytes(strText));
            // byte[] data = md5Hasher.ComputeHash(Encoding.GetEncoding("UTF-8").GetBytes(strText));
            // return System.Text.Encoding.Default.GetString(result);
            return Encoding.GetEncoding("UTF-8").GetString(result);
        }


        /// <summary>   
        /// MD5加密   
        /// </summary>   
        /// <param name="str"></param>   
        /// <returns></returns>   
        public static string MD5(string str)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.  
            MD5 md5Hasher = new MD5CryptoServiceProvider();
            // Convert the input string to a byte array and compute the hash.   
            byte[] data = md5Hasher.ComputeHash(Encoding.GetEncoding("UTF-8").GetBytes(str));
            // return BitConverter.ToString(data);//可以直接使用这个方法   

            //  Create a new Stringbuilder to collect the bytes   
            //  and create a string.   
            StringBuilder sBuilder = new StringBuilder();
            // Loop through each byte of the hashed data   
            // and format each one as a hexadecimal string.   
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            // Return the hexadecimal string.   
            return sBuilder.ToString();
        }

        /// <summary>  
        /// 获取文件的MD5码  
        /// </summary>  
        /// <param name="fileName">传入的文件名（含路径及后缀名）</param>  
        /// <returns></returns>  
        public  static string GetMD5HashFromFile(string fileName)
        {
            FileStream file = null;
            try
            {
                MD5 md5 = new MD5CryptoServiceProvider();

                file = new FileStream(fileName, System.IO.FileMode.Open, FileAccess.Read);
                byte[] retVal = md5.ComputeHash(file);                

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                    file.Dispose();
                }
            }
        }

        /// <summary>
        /// 字符串转16进制字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private static byte[] strToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");

            if ((hexString.Length % 2) != 0)
                return null; // hexString += " "; 

            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte( hexString.Substring(i * 2, 2), 16 );

            return returnBytes;
        }


        /// <summary>
        /// 字节数组转16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";

            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }

            return returnStr;
        }

        // 将 Stream 写入文件
        public static void StreamToFile(Stream stream, string fileName)
        {
            // 把 Stream 转换成 byte[]
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);
            // 把 byte[] 写入文件
            FileStream fs = new FileStream(fileName, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(bytes);
            bw.Close();
            fs.Close();
        }

        // 从文件读取 Stream
        public static Stream FileToStream(string fileName)
        {
            // 打开文件
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            // 读取文件的 byte[]
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);
            fileStream.Close();
            // 把 byte[] 转换成 Stream
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        /// 将 Stream 转成 byte[]
        public static byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        /// 将 byte[] 转成 Stream
        public static Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }


        //{
        // 二进制转换成图片
        //MemoryStream ms = new MemoryStream(bytes);
        //ms.Position = 0;
        //Image img = Image.FromStream(ms);
        //ms.Close();
        //this.pictureBox1.Image=img

        /// <summary>
        /// 
        /// </summary>

        //Bitmap 转化为 Byte[]
        //Bitmap BitReturn = new Bitmap();
        //byte[] bReturn = null;
        //MemoryStream ms = new MemoryStream();
        //BitReturn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        //bReturn = ms.GetBuffer();
        //}


        /// <summary>
        /// 判断文本框混合输入长度
        /// </summary>
        /// <param name="str">要判断的字符串</param>
        /// <param name="i">长度</param>
        /// <returns></returns>
        public static  bool ContentLengthNotExceed(string str, int i)
        {
            byte[] b = Encoding.Default.GetBytes(str);
            int m = b.Length;
            if (m < i)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static object getResources( String ResourcesName, String ItemName )
        {
            //
            // Project->Add Existing Item->All files->资源文件名.resources
            //
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager( 
                ResourcesName,
                System.Reflection.Assembly.GetExecutingAssembly() );
            return rm.GetObject(ItemName);
        }

        public static DateTime ConvertToDateTime(string datestr, string formatstr)
        {
            DateTimeFormatInfo dtFormat = new System.Globalization.DateTimeFormatInfo();
            dtFormat.FullDateTimePattern = formatstr;
            DateTime dt = Convert.ToDateTime(datestr, dtFormat);

            return dt;
        }

       

        //据当前时间返回毫秒数
        public static double getCurrentTimeMillis()
        {
            //DateTime.Now 已经是本地时间,不能再加 toLocalTime()了
            return DateTime.UtcNow.Subtract(startTime).TotalMilliseconds;
        }

        //工具方法,将日期转为秒数
        public static int getTotalSecondsByDate(int year, int month, int day)
        {
            // DateTime date = new DateTime(year, month, day,0,0,0,DateTimeKind.Utc);

            DateTime date = new DateTime(year, month, day, 0, 0, 0, 0).AddHours(-8);  //他妈的，还要再减去8小时才对
            return Convert.ToInt32(date.Subtract(startTime).TotalMilliseconds / 1000);

        }

        /// <summary>
        /// 标准方法,默认就用格式化: yyyy-MM-dd hh:mm:ss
        /// </summary>
        /// <param name="datestr"></param>       
        /// <returns></returns>
        public static DateTime ConvertToDateTime_Stand(string datestr)
        {            
            return Convert.ToDateTime(datestr);
        }

        //将list转到 DataTable
        public static  DataTable ToDataTable(ArrayList list)
        {
            DataTable result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    result.Columns.Add(pi.Name, pi.PropertyType);
                }

                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        object obj = pi.GetValue(list[i], null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }

    }
}
