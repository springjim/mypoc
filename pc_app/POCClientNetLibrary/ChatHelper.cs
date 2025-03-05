using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace POCClientNetLibrary
{
    public static class ChatHelper
    {
        #region Titles

        public const string CONVERSATION = "Conversation";
        public const string INCOMING_CALL = "Incoming call from";
        public const string OUTCOMING_CALL = "Calling to";
        public const string FILE_TRANSFER = "Recieve file from {0}?";
        public const string TRANSFER_CANCELED = "File Transfer canceled";
        public const string TRANSFERED = "File {0} successfully transfered";
        public const string GLOBAL = "Global";
        public const string SETTINGS = "Settings";
        public const string CONNECTED = "connected";
        public const string DISCONNECTED = "disconnected";
        public const string LOCAL = "127.0.0.1";
        public const string VERSION = "1.0";
        public const string APP_NAME = "POCControlCenter";
        public const string SOFTWARE = "Software";
        public const string NO_USERS_ONLINE = "no users online";
        public const string PROFILE = "Profile";
        public const string FILE_FILTER_ALL = "All files (*.*)|*.*";
        #endregion

        #region Registry Keys

        public const string LAUNCH_ON_STARTUP    = "LaunchOnStartup";
        public const string DOUBLE_CLICK_TO_CALL = "DoubleClickToCall";
        public const string SCHEME               = "Scheme";
        public const string DARK                 = "Dark";
        public const string LIGHT                = "Light";

        public const string SERVER_IP            = "ServerIP";
        public const string SERVER_PORT          = "ServerPort";
        public const string USER_NAME            = "UserName";
        public const string PASSWORD             = "Password";

        #endregion

        #region Errors
        public const string PORT_ERROR = "Port number should be between 0 and 65535";
        #endregion

        #region Messages 
        public static string WelcomeMessage = string.Format("{0}: ** Welcome to main chat room, Click on any user to start chat**\n", DateTime.Now.ToString("HH:mm:ss"));
        #endregion

        public class StateObject
        {
            // Client  socket.
            public Socket WorkSocket = null;

            // Size of receive buffer.
            public const int BUFFER_SIZE = 128*1024;
            // Receive buffer.
            public byte[] Buffer   = new byte[BUFFER_SIZE];

            public int currentPOS  = 0;
            public int recieveLEN  = 0;

            // Received data string.
            public StringBuilder Sb = new StringBuilder();
        }

        public static void WriteToEventLog(string message, EventLogEntryType type)
        {
            EventLog.WriteEntry( CLog.ApplicationName, message, type );
        }

        public static string ChatWith(string name)
        {
            return string.Format("** Conversation with {0} **\n", name);
        }
  
        public static string ByteArrayToStr(byte[] data)
        {
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

        public static GPSCommandMessage parseGPSCommandMsgData(byte[] msgBytes, int next, int lastLen, out int parselen)
        {
            GPSCommandMessage chatmsg = new GPSCommandMessage();
            int index = next;
            parselen = 0;
            try
            {
                short messageId = BitConverter.ToInt16(msgBytes, index);
                messageId = IPAddress.NetworkToHostOrder(messageId);

                byte messageLen = msgBytes[index + 2];  //为0                

                short length;       //消息长度 
                
                Debug.WriteLine("parseGPSCommandMsgData next:" + next + " cmd:" + messageId + " len:" + messageLen + " las:" + lastLen);

                switch (messageId)
                {
                    case MyType.TYPE_GPS_COMMAND:
                        length = BitConverter.ToInt16(msgBytes, index + 3);
                        length = IPAddress.NetworkToHostOrder(length);
                        chatmsg.setMsglen(length);

                        if (3 + length > lastLen)
                            return chatmsg;

                        chatmsg.setRequestType(msgBytes[index + 5]);
                        chatmsg.setLocationMode(msgBytes[index + 6]);
                        int dispatcherId = 0;

                        dispatcherId = BitConverter.ToInt32(msgBytes, index + 7);
                        dispatcherId = IPAddress.NetworkToHostOrder(dispatcherId);
                        chatmsg.setDispatcherId(dispatcherId);

                        dispatcherId = BitConverter.ToInt32(msgBytes, index + 11);
                        dispatcherId = IPAddress.NetworkToHostOrder(dispatcherId);
                        chatmsg.setGroupId(dispatcherId);

                        dispatcherId = BitConverter.ToInt32(msgBytes, index + 15);
                        dispatcherId = IPAddress.NetworkToHostOrder(dispatcherId);
                        chatmsg.setUserId(dispatcherId);

                        chatmsg.setErrorCode(msgBytes[index + 19]);                                                                  
                        

                        //******************************
                        //返回已分析的长度

                        parselen = 3 + length;
                        break;

                    default:
                        return null;
                }
            }
            catch (ArgumentOutOfRangeException e1)
            {
                Debug.WriteLine(e1.Message);
            }
            catch (ArgumentNullException e2)
            {
                Debug.WriteLine(e2.Message);
            }
            catch (DecoderFallbackException e3)
            {
                Debug.WriteLine(e3.Message);
            }
            catch (ArgumentException e4)
            {
                Debug.WriteLine(e4.Message);
            }

            return chatmsg;
        }

        public static AVChatNewMessage parseAVChatNewMsgData(byte[] msgBytes, int next, int lastLen, out int parselen)
        {
            AVChatNewMessage chatmsg = new AVChatNewMessage();
            int index = next;
            parselen = 0;
            try
            {
                short messageId = BitConverter.ToInt16(msgBytes, index);
                messageId = IPAddress.NetworkToHostOrder(messageId);

                byte messageLen = msgBytes[index + 2];  //为0                

                short length;       //消息长度 
                short video_type;
                short video_command;
                int fromUserId;
                int toUserId;
                String fromUserName="";
                String toUserName="";
                String desc="";

                Debug.WriteLine("parseAVChatNewMsgData next:" + next + " cmd:" + messageId + " len:" + messageLen + " las:" + lastLen);

                switch (messageId)
                {
                    case MyType.TYPE_AV_CHAT_NEW:
                        length = BitConverter.ToInt16(msgBytes, index + 3);
                        length = IPAddress.NetworkToHostOrder(length);
                        chatmsg.msgLength=length;

                        if (3 + length > lastLen)
                            return chatmsg;

                        video_type = BitConverter.ToInt16(msgBytes, index + 5);
                        video_type = IPAddress.NetworkToHostOrder(video_type);
                        chatmsg.video_type=video_type;

                        video_command = BitConverter.ToInt16(msgBytes, index + 7);
                        video_command = IPAddress.NetworkToHostOrder(video_command);
                        chatmsg.video_command=video_command;

                        fromUserId = BitConverter.ToInt32(msgBytes, index + 9);
                        fromUserId = IPAddress.NetworkToHostOrder(fromUserId);
                        chatmsg.fromUserId= fromUserId;

                        toUserId = BitConverter.ToInt32(msgBytes, index + 13);
                        toUserId = IPAddress.NetworkToHostOrder(toUserId);
                        chatmsg.toUserId = toUserId;

                        int pos = index + 3 + 2 + 2 + 2 + 8;
                        short len = 0;
                        //取fromUserName长度
                        len = BitConverter.ToInt16(msgBytes, index + pos);
                        len = IPAddress.NetworkToHostOrder(len);
                        if (len > 0)
                        {
                            fromUserName = Encoding.GetEncoding("UTF-8").GetString(msgBytes, pos + 2, len);
                            pos += 2;
                            pos += len;
                        }
                        else
                        {
                            pos += 2;
                        }

                        //取toUserName字段
                        len = 0;
                        len = BitConverter.ToInt16(msgBytes, pos);
                        len = IPAddress.NetworkToHostOrder(len);
                        if (len > 0)
                        {
                            toUserName = Encoding.GetEncoding("UTF-8").GetString(msgBytes, pos + 2, len);
                            pos += 2;
                            pos += len;

                        }
                        else
                        {
                            pos += 2;
                        }

                        //desc
                        len = 0;
                        len = BitConverter.ToInt16(msgBytes, pos);
                        len = IPAddress.NetworkToHostOrder(len);
                        if (len > 0)
                        {
                            desc = Encoding.GetEncoding("UTF-8").GetString(msgBytes, pos + 2, len);
                            pos += 2;
                            pos += len;

                        }
                        else
                        {
                            pos += 2;
                        }

                        //set值
                        chatmsg.fromUserName = fromUserName;
                        chatmsg.toUserName = toUserName;
                        chatmsg.desc = desc;

                        //******************************
                        //返回已分析的长度

                        parselen = 3 + length;
                        break;

                    default:
                        return null;
                }
            }
            catch (ArgumentOutOfRangeException e1)
            {
                Debug.WriteLine(e1.Message);
            }
            catch (ArgumentNullException e2)
            {
                Debug.WriteLine(e2.Message);
            }
            catch (DecoderFallbackException e3)
            {
                Debug.WriteLine(e3.Message);
            }
            catch (ArgumentException e4)
            {
                Debug.WriteLine(e4.Message);
            }

            return chatmsg;
        }


        public static VideoMessage parseVideoMsgData(byte[] msgBytes, int next, int lastLen, out int parselen)
        {
            VideoMessage chatmsg = new VideoMessage();
            int index = next;
            parselen = 0;
            try
            {
                short messageId = BitConverter.ToInt16(msgBytes, index);
                messageId = IPAddress.NetworkToHostOrder(messageId);

                byte messageLen = msgBytes[index + 2];  //为0                
                
                short length;       //消息长度 
                short video_type;
                short video_command;
                int roomid;
                string video_join_type = "";
                string app = "";
                string stream = "";
                string streamurl = "";
                string roomname = "";
                string roomdesc = "";
                string ext1 = "";
                string ext2= "";
                string ext3 = "";
                string ext4 = "";
                string ext5 = "";                

                Debug.WriteLine("parseVideoMsgData next:" + next + " cmd:" + messageId + " len:" + messageLen + " las:" + lastLen);

                switch (messageId)
                {
                    case MyType.VIDEO_MESSAGE:
                        length = BitConverter.ToInt16(msgBytes, index + 3);
                        length = IPAddress.NetworkToHostOrder(length);
                        chatmsg.setMsglen(length);

                        if (3 + length > lastLen)
                            return chatmsg;

                        video_type = BitConverter.ToInt16(msgBytes, index + 5);
                        video_type = IPAddress.NetworkToHostOrder(video_type);
                        chatmsg.setVideo_type(video_type);

                        video_command = BitConverter.ToInt16(msgBytes, index + 7);
                        video_command = IPAddress.NetworkToHostOrder(video_command);
                        chatmsg.setVideo_command(video_command);

                        roomid = BitConverter.ToInt32(msgBytes, index + 9);
                        roomid = IPAddress.NetworkToHostOrder(roomid);
                        chatmsg.setRoomid(roomid);

                        int pos = index + 3 + 2 + 2 + 2 + 4;
                        short len = 0;
                        //取video_join_type字段
                        len = BitConverter.ToInt16(msgBytes, index + pos);
                        len = IPAddress.NetworkToHostOrder(len);
                        if (len > 0)
                        {
                            video_join_type= Encoding.GetEncoding("UTF-8").GetString(msgBytes, pos + 2, len);
                            pos += 2;
                            pos += len;
                        } else
                        {
                            pos += 2;
                        }

                        //取app字段
                        len = 0;
                        len = BitConverter.ToInt16(msgBytes, pos);
                        len = IPAddress.NetworkToHostOrder(len);
                        if (len > 0)
                        {
                            app = Encoding.GetEncoding("UTF-8").GetString(msgBytes, pos + 2, len);
                            pos += 2;
                            pos += len;

                        } else
                        {
                            pos += 2;
                        }

                        //stream
                        len = 0;
                        len = BitConverter.ToInt16(msgBytes, pos);
                        len = IPAddress.NetworkToHostOrder(len);
                        if (len > 0)
                        {
                            stream = Encoding.GetEncoding("UTF-8").GetString(msgBytes, pos + 2, len);
                            pos += 2;
                            pos += len;

                        }
                        else
                        {
                            pos += 2;
                        }
                        //streamurl
                        len = 0;
                        len = BitConverter.ToInt16(msgBytes, pos);
                        len = IPAddress.NetworkToHostOrder(len);
                        if (len > 0)
                        {
                            streamurl = Encoding.GetEncoding("UTF-8").GetString(msgBytes, pos + 2, len);
                            pos += 2;
                            pos += len;

                        }
                        else
                        {
                            pos += 2;
                        }

                        //roomname
                        len = 0;
                        len = BitConverter.ToInt16(msgBytes, pos);
                        len = IPAddress.NetworkToHostOrder(len);
                        if (len > 0)
                        {
                            roomname = Encoding.GetEncoding("UTF-8").GetString(msgBytes, pos + 2, len);
                            pos += 2;
                            pos += len;

                        }
                        else
                        {
                            pos += 2;
                        }

                        //roomdesc
                        len = 0;
                        len = BitConverter.ToInt16(msgBytes, pos);
                        len = IPAddress.NetworkToHostOrder(len);
                        if (len > 0)
                        {
                            roomdesc = Encoding.GetEncoding("UTF-8").GetString(msgBytes, pos + 2, len);
                            pos += 2;
                            pos += len;

                        }
                        else
                        {
                            pos += 2;
                        }
                        //ext1
                        len = 0;
                        len = BitConverter.ToInt16(msgBytes, pos);
                        len = IPAddress.NetworkToHostOrder(len);
                        if (len > 0)
                        {
                            ext1 = Encoding.GetEncoding("UTF-8").GetString(msgBytes, pos + 2, len);
                            pos += 2;
                            pos += len;

                        }
                        else
                        {
                            pos += 2;
                        }
                        //ext2
                        len = 0;
                        len = BitConverter.ToInt16(msgBytes, pos);
                        len = IPAddress.NetworkToHostOrder(len);
                        if (len > 0)
                        {
                            ext2 = Encoding.GetEncoding("UTF-8").GetString(msgBytes, pos + 2, len);
                            pos += 2;
                            pos += len;

                        }
                        else
                        {
                            pos += 2;
                        }
                        //ext3
                        len = 0;
                        len = BitConverter.ToInt16(msgBytes, pos);
                        len = IPAddress.NetworkToHostOrder(len);
                        if (len > 0)
                        {
                            ext3 = Encoding.GetEncoding("UTF-8").GetString(msgBytes, pos + 2, len);
                            pos += 2;
                            pos += len;

                        }
                        else
                        {
                            pos += 2;
                        }
                        //ext4
                        len = 0;
                        len = BitConverter.ToInt16(msgBytes, pos);
                        len = IPAddress.NetworkToHostOrder(len);
                        if (len > 0)
                        {
                            ext4 = Encoding.GetEncoding("UTF-8").GetString(msgBytes, pos + 2, len);
                            pos += 2;
                            pos += len;

                        }
                        else
                        {
                            pos += 2;
                        }
                        //ext5                                             
                        len = 0;
                        len = BitConverter.ToInt16(msgBytes, pos);
                        len = IPAddress.NetworkToHostOrder(len);
                        if (len > 0)
                        {
                            ext5 = Encoding.GetEncoding("UTF-8").GetString(msgBytes, pos + 2, len);
                            pos += 2;
                            pos += len;

                        }
                        else
                        {
                            pos += 2;
                        }

                        //set值
                        chatmsg.setVideo_type(video_type);
                        chatmsg.setVideo_command(video_command);
                        chatmsg.setRoomid(roomid);
                        chatmsg.setVideo_join_type(video_join_type);
                        chatmsg.setApp(app);
                        chatmsg.setStream(stream);
                        chatmsg.setStreamurl(streamurl);
                        chatmsg.setRoomname(roomname);
                        chatmsg.setRoomdesc(roomdesc);
                        chatmsg.setExt1(ext1);
                        chatmsg.setExt2(ext2);
                        chatmsg.setExt3(ext3);
                        chatmsg.setExt4(ext4);
                        chatmsg.setExt5(ext5);

                        //******************************
                        //返回已分析的长度

                        parselen = 3 + length;
                        break;

                    default:
                        return null;
                }
            }
            catch (ArgumentOutOfRangeException e1)
            {
                Debug.WriteLine(e1.Message);
            }
            catch (ArgumentNullException e2)
            {
                Debug.WriteLine(e2.Message);
            }
            catch (DecoderFallbackException e3)
            {
                Debug.WriteLine(e3.Message);
            }
            catch (ArgumentException e4)
            {
                Debug.WriteLine(e4.Message);
            }

            return chatmsg;
        }

        public static ChatMessage parseChatMsgData( byte[] msgBytes, int next, int lastLen, out int parselen)
        {
            ChatMessage chatmsg = new ChatMessage();
            int index = next;
            parselen = 0;

            try
            {

            short  messageId  = BitConverter.ToInt16(msgBytes, index);
            messageId         = IPAddress.NetworkToHostOrder(messageId);
            byte   messageLen = msgBytes[index+2];

            String msgContent = "";

            int    groupId = 0;
            int    userId = 0;
            long   datetime;
            byte   charsetnameLength;
            String charsetname = null;
            short  length;
            String formatdate = null;

            

            Debug.WriteLine("parseChatMsgData next:" + next + " cmd:" + messageId + " len:" + messageLen + " las:" + lastLen);

            switch (messageId)
            {
                // P2G
                case MyType.TYPE_P2G_MESSAGES:
                    if ( 3 + messageLen > lastLen )
                        return chatmsg;

                    groupId = BitConverter.ToInt32(msgBytes, index + 3);
                    groupId = IPAddress.NetworkToHostOrder(groupId);
                    userId  = BitConverter.ToInt32(msgBytes, index + 7);
                    userId  = IPAddress.NetworkToHostOrder(userId);

                    messageLen = msgBytes[index + 11];
                    msgContent = Encoding.GetEncoding("UTF-8").GetString(msgBytes, index + 12, messageLen);

                    
                    chatmsg.setMsglength(messageLen + "");
                    chatmsg.setGroupid(groupId + "");
                    chatmsg.setUserid (userId + "");
                    chatmsg.setMsgtype(messageId + "");
                    chatmsg.setMsgcontent(msgContent);

                    parselen = 3 + 4 + 4 + 1 + messageLen;
                    break;

                // P2P
                case MyType.TYPE_P2P_MESSAGES:
                    if ( 3 + messageLen > lastLen )
                        return chatmsg;

                    groupId = BitConverter.ToInt32(msgBytes, index + 3);
                    groupId = IPAddress.NetworkToHostOrder(groupId);
                    userId = BitConverter.ToInt32(msgBytes, index + 7);
                    userId = IPAddress.NetworkToHostOrder(userId);
                    messageLen = msgBytes[index + 11];
                    msgContent = Encoding.GetEncoding("UTF-8").GetString(msgBytes, index + 12, messageLen);
                    
                    chatmsg.setMsglength(messageLen + "");
                    chatmsg.setGroupid(groupId + "");
                    chatmsg.setUserid(userId + "");
                    chatmsg.setMsgtype(messageId + "");
                    chatmsg.setMsgcontent(msgContent);

                    parselen = 3 + 4 + 4 + 1 + messageLen;
                    break;

                case MyType.TYPE_P2G_CHAT_TEXT:
                    length = BitConverter.ToInt16(msgBytes, index + 3);
                    length = IPAddress.NetworkToHostOrder(length);

                    if ( 3 + length > lastLen )
                        return chatmsg;


                    groupId = BitConverter.ToInt32(msgBytes, index + 5);
                    groupId = IPAddress.NetworkToHostOrder(groupId);

                    userId = BitConverter.ToInt32(msgBytes, index + 9);
                    userId = IPAddress.NetworkToHostOrder(userId);

                    datetime = BitConverter.ToInt64(msgBytes, index + 13);
                    datetime = IPAddress.NetworkToHostOrder(datetime);

                    charsetnameLength = msgBytes[index + 21];

                    charsetname = Encoding.GetEncoding("UTF-8").GetString(msgBytes, index + 22, charsetnameLength);
                    msgContent  = Encoding.GetEncoding("UTF-8").GetString(msgBytes,
                                  22 + charsetnameLength, length - (2 + 4 + 4 + 8 + 1 + charsetnameLength));
                    // DateTime.FromFileTimeUtc();

                    
                    chatmsg.setMsglength(length + "");
                    chatmsg.setCharsetname(charsetname);
                    // formatdate= TimeUtil.getFormatTime(datetime, "yy-MM-dd HH:mm:ss");
                    // chatmsg.setDatetime(formatdate);
                    chatmsg.setDatetime( DateTime.FromFileTimeUtc(datetime).ToLongDateString() );
                    chatmsg.setGroupid(groupId + "");
                    chatmsg.setMsgtype(messageId + "");
                    chatmsg.setMsgcontent(msgContent);
                    chatmsg.setUserid(userId + "");

                    parselen = 3 + length;
                    break;

                case MyType.TYPE_P2P_CHAT_TEXT:
                    length  = BitConverter.ToInt16(msgBytes, index + 3);
                    length = IPAddress.NetworkToHostOrder(length);

                    if ( 3 + length > lastLen )
                        return chatmsg;

                    groupId = BitConverter.ToInt32(msgBytes, index + 5);
                    groupId = IPAddress.NetworkToHostOrder(groupId);

                    userId  = BitConverter.ToInt32(msgBytes, index + 9);
                    userId = IPAddress.NetworkToHostOrder(userId);

                    datetime = BitConverter.ToInt64(msgBytes, index + 13);
                    datetime = IPAddress.NetworkToHostOrder(datetime);

                    charsetnameLength = msgBytes[index + 21];

                    charsetname = Encoding.GetEncoding("UTF-8").GetString(msgBytes, index + 22, charsetnameLength);
                    msgContent  = Encoding.GetEncoding("UTF-8").GetString(msgBytes, 
                                  22 + charsetnameLength,length - (2 + 4 + 4 + 8 + 1 + charsetnameLength));

                    
                    chatmsg.setMsglength(length + "");
                    chatmsg.setCharsetname(charsetname);

                    formatdate = DateTime.FromFileTimeUtc(datetime).ToLongDateString();
                    chatmsg.setDatetime(formatdate);
                    chatmsg.setGroupid(groupId + "");
                    chatmsg.setMsgtype(messageId + "");
                    chatmsg.setMsgcontent(msgContent);
                    chatmsg.setUserid(userId + "");

                    parselen = 3 + length;
                    break;

                case MyType.TYPE_P2G_CHAT_FILE:
                    length  = BitConverter.ToInt16(msgBytes, index + 3);
                    length = IPAddress.NetworkToHostOrder(length);

                    if ( 3 + length > lastLen )
                        return chatmsg;

                    groupId = BitConverter.ToInt32(msgBytes, index + 5);
                    groupId = IPAddress.NetworkToHostOrder(groupId);

                    userId  = BitConverter.ToInt32(msgBytes, index + 9);
                    userId  = IPAddress.NetworkToHostOrder(userId);

                    int pos = index + 3 + 2 + 4 + 4;
                    short len = 0;
                    len = BitConverter.ToInt16(msgBytes, index + pos);
                    len = IPAddress.NetworkToHostOrder(len);

                    String file_key = Encoding.GetEncoding("UTF-8").GetString(msgBytes, pos + 2, len);
                    pos += 2;
                    pos += len;

                    len = BitConverter.ToInt16(msgBytes, pos);
                    len = IPAddress.NetworkToHostOrder(len);
                    String file_name = Encoding.GetEncoding("UTF-8").GetString(msgBytes, pos + 2, len);
                    pos += 2;
                    pos += len;

                    len = BitConverter.ToInt16(msgBytes, pos);
                    len = IPAddress.NetworkToHostOrder(len);
                    String file_type = Encoding.GetEncoding("UTF-8").GetString(msgBytes, pos + 2, len);
                    pos += 2;
                    pos += len;

                    len = BitConverter.ToInt16(msgBytes, pos);
                    len = IPAddress.NetworkToHostOrder(len);
                    String file_MD5 = Encoding.GetEncoding("UTF-8").GetString(msgBytes, pos + 2, len);
                    pos += 2;
                    pos += len;

                    len = BitConverter.ToInt16(msgBytes, pos);
                    len = IPAddress.NetworkToHostOrder(len);
                    String filemsgcontent = Encoding.GetEncoding("UTF-8").GetString(msgBytes, pos + 2, len);
                    pos += 2;
                    pos += len;

                    long filesize = BitConverter.ToInt64(msgBytes, pos);
                    filesize = IPAddress.NetworkToHostOrder(filesize);
                    pos += 8;
                    datetime = BitConverter.ToInt64(msgBytes, pos);
                    datetime = IPAddress.NetworkToHostOrder(datetime);

                    
                    chatmsg.setUserid(userId + "");
                    chatmsg.setGroupid(groupId + "");
                    chatmsg.setMsgtype(MyType.TYPE_P2G_CHAT_FILE + "");
                    chatmsg.setFilename(file_name);
                    chatmsg.setFilekey(file_key);
                    chatmsg.setFileMD5(file_MD5);
                    chatmsg.setFiletype(file_type);
                    chatmsg.setFilesize(filesize);
                    chatmsg.setMsgcontent(filemsgcontent);
                    chatmsg.setDatetime(DateTime.FromFileTimeUtc(datetime).ToLongDateString());

                    parselen = 3 + length;
                    break;

                default:
                    return null;
            }
            }
            catch (ArgumentOutOfRangeException e1)
            {
                Debug.WriteLine(e1.Message);
            }
            catch (ArgumentNullException e2)
            {
                Debug.WriteLine(e2.Message);
            }
            catch (DecoderFallbackException e3)
            {
                Debug.WriteLine(e3.Message);
            }
            catch (ArgumentException e4)
            {
                Debug.WriteLine(e4.Message);
            }

            return chatmsg;

        }

        public static ManageDataPacket parseManageData( byte[] msgBytes, int next, int lastLen, out int parseLen )
        {
            ManageDataPacket data = new ManageDataPacket();
            parseLen  = 0;
            int index = next;

            short messageId = BitConverter.ToInt16(msgBytes, index);
            messageId       = IPAddress.NetworkToHostOrder(messageId);
            //byte messageLen = msgBytes[index + 2];
            short  messageLen = (short)msgBytes[index + 2];

            Debug.WriteLine("parseManageData next:" + next + " cmd:" + messageId + " len:" + messageLen + " las:" + lastLen);

            if ( 3 + messageLen > lastLen )
                return data;

            data.setMessageId(messageId);
            //data.setLength(messageLen);
            data.setLength(msgBytes[index + 2]);

            int groupId      = 0;
            int userId       = 0;
            int msgSerialNo  = 0;
            byte userNameLen = 0;
            String userName = "";
            byte groupNameLen;
            String groupName;
            int inviteId;
            int worktype;
            //2017.12.04 为了支持GPSCommand报文
            short msglen;
            byte requestType;
            byte locationMode;
            int dispatcherId;
            byte errorCode;
            //2018.01.10 为了支持SOS键报文
            byte sos_type;
            int sos_datetime;
            long tmplong;
            double longitude;
            double latitude;
            short gps_type_len;
            string gps_type="";           

            long sos_id;  //ptt_soslog 表中的id值

            switch (messageId)
            {
                // 收到服务器心跳检测
                case MyType.TYPE_CHECK_SERVER:
                // 抢麦成功
                case MyType.TYPE_MIC_SUCCESS:
                // 抢麦失败
                case MyType.TYPE_MIC_FAILED:
                // 收到客户端检查服务器心跳检测的响应
                case MyType.TYPE_SERVICE_RESPONSE:
                
                    break;

                // 登录
                case MyType.TYPE_LOGIN:
                // 注销
                case MyType.TYPE_LOGOUT:
                // 收到邀请加入组的回复
                case MyType.TYPE_REJECT_APPLY:
                case MyType.TYPE_REJECT_INVITE:
                //收到被踢出的控制消息
                case MyType.KICK_OFF:
                    if (messageLen >= 8)
                    {
                        groupId = BitConverter.ToInt32(msgBytes, index + 3);
                        groupId = IPAddress.NetworkToHostOrder(groupId);

                        userId  = BitConverter.ToInt32(msgBytes, index + 7);
                        userId  = IPAddress.NetworkToHostOrder(userId);

                        data.setGroupId(groupId);
                        data.setUserId(userId);
                    }

                    if (messageLen >= 12)
                    {
                        userNameLen = msgBytes[index + 11];
                        userName = Encoding.GetEncoding("UTF-8").GetString(msgBytes, index + 12, userNameLen);

                        data.setUserNameLen(userNameLen);
                        data.setUserName(userName);
                    }
                    break;

                //收到视频报文的通知
                case MyType.VIDEO_MESSAGE:


                    break;
                
                // 收到解散组通知
                case MyType.TYPE_DELETE_GROUP:
                    if (messageLen >= 4)
                    {
                        //2017.9.26 加入
                        groupId = BitConverter.ToInt32(msgBytes, index + 3);
                        groupId = IPAddress.NetworkToHostOrder(groupId);                        

                        data.setGroupId(groupId);                       
                    }
                    break;

                // 收到踢除用户通知
                case MyType.TYPE_KICK_USER:
                // 收到用户退出组通知
                case MyType.TYPE_EXIT_GROUP:
                    if (messageLen >= 8)
                    {
                        groupId = BitConverter.ToInt32(msgBytes, index + 3);
                        groupId = IPAddress.NetworkToHostOrder(groupId);

                        userId = BitConverter.ToInt32(msgBytes, index + 7);
                        userId = IPAddress.NetworkToHostOrder(userId);

                        data.setGroupId(groupId);
                        data.setUserId(userId);
                    }
                    break;

                // 收到申请加入组的回复
                case MyType.TYPE_ACCEPT_APPLY:
                case MyType.TYPE_ACCEPT_INVITE:
                    groupId = BitConverter.ToInt32(msgBytes, index + 3);
                    groupId = IPAddress.NetworkToHostOrder(groupId);

                    userId = BitConverter.ToInt32(msgBytes, index + 7);
                    userId = IPAddress.NetworkToHostOrder(userId);

                    userNameLen = msgBytes[index + 11];
                    userName = Encoding.GetEncoding("UTF-8").GetString(msgBytes, index + 12, userNameLen);
                    data.setGroupId(groupId);
                    data.setUserId(userId);
                    data.setUserNameLen(userNameLen);
                    data.setUserName(userName);
                    break;

                // 收到用户加入组的申请
                case MyType.TYPE_RECIVED_APPLY:
                    // 获取值
                    groupId = BitConverter.ToInt32(msgBytes, index + 3);
                    groupId = IPAddress.NetworkToHostOrder(groupId);
                    userId = BitConverter.ToInt32(msgBytes, index + 7);
                    userId = IPAddress.NetworkToHostOrder(userId);
                    userNameLen = msgBytes[11];
                    userName = Encoding.GetEncoding("UTF-8").GetString(msgBytes, index + 12, userNameLen);
                    msgSerialNo = BitConverter.ToInt32(msgBytes, userNameLen + index + 12);
                    msgSerialNo = IPAddress.NetworkToHostOrder(msgSerialNo);
                    // 设置值
                    data.setGroupId(groupId);
                    data.setUserId(userId);
                    data.setUserNameLen(userNameLen);
                    data.setUserName(userName);
                    data.setMsgSerialNo(msgSerialNo);
                    break;

                // 收到加入组的邀请
                case MyType.TYPE_RECIVED_INVITE:
                    groupId = BitConverter.ToInt32(msgBytes, index + 3);
                    groupId = IPAddress.NetworkToHostOrder(groupId);

                    groupNameLen = msgBytes[index + 7];
                    groupName = Encoding.GetEncoding("UTF-8").GetString(msgBytes, index + 8, groupNameLen);

                    data.setGroupId(groupId);
                    data.setUserId(userId);
                    data.setGroupName(groupName);
                    data.setGroupNameLen(groupNameLen);
                    data.setMsgSerialNo(msgSerialNo);
                    break;

                case MyType.TYPE_SYS_MESSAGE:
                    if (messageLen >= 8)
                    {
                        groupId = BitConverter.ToInt32(msgBytes, index + 3);
                        groupId = IPAddress.NetworkToHostOrder(groupId);

                        userId = BitConverter.ToInt32(msgBytes, index + 7);
                        userId = IPAddress.NetworkToHostOrder(userId);

                        int type = BitConverter.ToInt32(msgBytes, index + 11);
                        type = IPAddress.NetworkToHostOrder(type);

                        data.setGroupId(groupId);
                        data.setUserId(userId);
                        data.setType(type);

                        Debug.WriteLine( "parseManageData type:" + type );
                    }
                    break;

                case MyType.TYPE_PERSON_INVITE:
                    groupId = BitConverter.ToInt32(msgBytes, index + 3);
                    groupId = IPAddress.NetworkToHostOrder(groupId);

                    groupNameLen = msgBytes[7];
                    groupName = Encoding.GetEncoding("UTF-8").GetString(msgBytes, index + 8, groupNameLen);

                    userId = BitConverter.ToInt32(msgBytes, index + 8 + groupNameLen);
                    userId = IPAddress.NetworkToHostOrder(userId);

                    inviteId = BitConverter.ToInt32(msgBytes, index + 12 + groupNameLen);
                    inviteId = IPAddress.NetworkToHostOrder(inviteId);

                    worktype = BitConverter.ToInt32(msgBytes, index + 16 + groupNameLen);
                    worktype = IPAddress.NetworkToHostOrder(worktype);

                    data.setGroupId(groupId);
                    data.setUserId(userId);
                    data.setInviteId(inviteId);
                    data.setWorkType(worktype);
                    data.setGroupName(groupName);
                    data.setGroupNameLen(groupNameLen);
                    break;

                case MyType.TYPE_PERSON_INVITE_RELEASE:
                    groupId = BitConverter.ToInt32(msgBytes, index + 3);
                    groupId = IPAddress.NetworkToHostOrder(groupId);

                    groupNameLen = msgBytes[index + 7];
                    groupName = Encoding.GetEncoding("UTF-8").GetString(msgBytes, index + 8, groupNameLen);

                    userId = BitConverter.ToInt32(msgBytes, index + 8 + groupNameLen);
                    userId = IPAddress.NetworkToHostOrder(userId);

                    inviteId = BitConverter.ToInt32(msgBytes, index + 12 + groupNameLen);
                    inviteId = IPAddress.NetworkToHostOrder(inviteId);

                    worktype = BitConverter.ToInt32(msgBytes, index + 16 + groupNameLen);
                    worktype = IPAddress.NetworkToHostOrder(worktype);

                    data.setGroupId(groupId);
                    data.setUserId(userId);
                    data.setInviteId(inviteId);
                    data.setWorkType(worktype);
                    data.setGroupName(groupName);
                    data.setGroupNameLen(groupNameLen);
                    break;
                //接收到SOS键按下
                case MyType.TYPE_SOS_LOCATION:
                                       

                    msglen = BitConverter.ToInt16(msgBytes, index + 3);
                    msglen = IPAddress.NetworkToHostOrder(msglen);
                    //注意，这里的定位的长度不是在 messageLen (即第三个字节)中表示的,
                    //所以要用 msglen更新为parseLen的值
                    messageLen = msglen;

                    groupId = BitConverter.ToInt32(msgBytes, index + 5);
                    groupId = IPAddress.NetworkToHostOrder(groupId);
                    userId = BitConverter.ToInt32(msgBytes, index + 9);
                    userId = IPAddress.NetworkToHostOrder(userId);
                    sos_type = msgBytes[index + 13];

                    sos_datetime= BitConverter.ToInt32(msgBytes, index + 14);
                    sos_datetime = IPAddress.NetworkToHostOrder(sos_datetime);

                    byte[] doublebytes = new byte[8];
                    
                    for(int i = 0; i < 8; i++)
                    {
                        doublebytes[i] = msgBytes[index + 25-i];
                    }
                    
                    longitude= BitConverter.ToDouble(doublebytes,0);
                    
                    for (int i = 0; i < 8; i++)
                    {
                        doublebytes[i] = msgBytes[index + 33 - i];
                    }
                    latitude = BitConverter.ToDouble(doublebytes, 0);
                    

                    gps_type_len = BitConverter.ToInt16(msgBytes, index + 34);
                    gps_type_len = IPAddress.NetworkToHostOrder(gps_type_len);

                    if (gps_type_len > 0)
                    {
                        gps_type = Encoding.GetEncoding("UTF-8").GetString(msgBytes, index + 36, gps_type_len);
                    }
                    //取sosid
                    sos_id = BitConverter.ToInt64(msgBytes, index + 36 + gps_type_len);
                    sos_id = IPAddress.NetworkToHostOrder(sos_id);

                    //再设置data
                    data.setSos_type(sos_type);
                    data.setSos_datetime(sos_datetime);
                    data.setGroupId(groupId);
                    data.setUserId(userId);
                    data.setLongitude(longitude);
                    data.setLatitude(latitude);
                    data.setGps_type(gps_type);
                    data.setSos_id(sos_id);

                    break;
                //接收到SOS键放开
                case MyType.TYPE_SOS_KEY_RELEASE:
                    if (messageLen >= 8)
                    {
                        messageLen = 8;
                        groupId = BitConverter.ToInt32(msgBytes, index + 3);
                        groupId = IPAddress.NetworkToHostOrder(groupId);

                        userId = BitConverter.ToInt32(msgBytes, index + 7);
                        userId = IPAddress.NetworkToHostOrder(userId);

                        data.setGroupId(groupId);
                        data.setUserId(userId);
                    }
                    break;
                //2017.12.04  加入TYPE_GPS_COMMAND 的解析
                case MyType.TYPE_GPS_COMMAND:
                    //                 
                    msglen = BitConverter.ToInt16(msgBytes, index + 3);
                    msglen = IPAddress.NetworkToHostOrder(msglen);
                    //
                    messageLen =msglen;

                    requestType = msgBytes[index + 5];
                    locationMode = msgBytes[index + 6];

                    dispatcherId = BitConverter.ToInt32(msgBytes, index + 7);
                    dispatcherId = IPAddress.NetworkToHostOrder(dispatcherId);

                    groupId = BitConverter.ToInt32(msgBytes, index + 11);
                    groupId = IPAddress.NetworkToHostOrder(groupId);

                    userId = BitConverter.ToInt32(msgBytes, index + 15);
                    userId = IPAddress.NetworkToHostOrder(userId);
                    errorCode = msgBytes[19];
                    
                    data.setRequestType(requestType);
                    data.setLocationMode(locationMode);
                    data.setDispatcherId(dispatcherId);
                    data.setGroupId(groupId);
                    data.setUserId(userId);
                    data.setErrorCode(errorCode);
                    break;
                default:
                    return null;

            }

            parseLen = 3 + messageLen;
            return data;
        }
    }

    /// <summary>
    /// Data structure to interact with server
    /// </summary>
    public class Data
    {
        public short  Command  { get; set; }
        public short  MsgLen   { get; set; }
        public AudioDataPacket media { get; set; }
       
        public short paseLen { get; set; }

        //2017.2.17 增加5个包的收发解析
        public List<AudioDataPacket> media_arr { get; set; }

        //2017.11.21 增加报文中组ID，用户ID的识别
        public int group_id { get; set; }
        public int user_id { get; set; }

        //2018.01.11 增加SOS报文中的transfer_mode, endpoint_type
        /// <summary>
        /// 转发模式，0 表示转发给调度台,如果一个企业号内有多个调度台登入，即都转发， 1 表示转发到当前群组内所有人,除了自己
        /// </summary>
        public byte transfer_mode { get; set; }
        /// <summary>
        /// 表示发起端: 0 表示 是poc端发起来的sos语音包， 1 表示的是调度端发起的sos语音包
        /// </summary>
        public byte endpoint_type { get; set; }

        public Data()
        {
            Command  = MyType.TYPE_MEDIA;
            MsgLen   = 0;
            media    = null;
            media_arr = new List<AudioDataPacket>();
            paseLen  = 0;
        }

        public Data( short command )
        {
            Command = command;
            MsgLen  = 0;
            media   = null;
            media_arr = new List<AudioDataPacket>();
            paseLen = 0;
        }

        //public Data( byte[] data, int pos, int LastLen )
        //{
        //    int next = pos;
        //    media    = null;
        //    media_arr = new List<AudioDataPacket>();
        //    // First four bytes are for the Command
        //    Command = BitConverter.ToInt16(data, next);
        //    Command = IPAddress.NetworkToHostOrder(Command);
        //    next   += sizeof(short);

        //    MsgLen  = data[next];
        //    next   += sizeof( byte );

        //    byte[] body;

        //    switch ( Command )
        //    {
        //        case MyType.TYPE_MEDIA_EX:
        //        case MyType.TYPE_TOPOC_START_EX:
        //        case MyType.TYPE_TOPOC_END_EX:
        //            //2017.5.19 这里有类型溢出情况,要考虑

        //            MsgLen = BitConverter.ToInt16(data, next);
        //            MsgLen = IPAddress.NetworkToHostOrder(MsgLen);

        //            Debug.WriteLine("1. Data pos:" + pos + " cmd:" + Command + " len:" + MsgLen + " las:"+ LastLen);

        //            if ( 3 + MsgLen > LastLen )
        //            {
        //                media   = null;
        //                paseLen = 0;
        //                return;
        //            }
        //            next  += sizeof(short);
        //            //2017.2.17 修改为5包,或动态分析接收数据包的个数
        //            //5包合一: start
        //            int bytenumPerpackage = (MsgLen - 2) / 5;

        //            //第一个包
        //            body = new byte[bytenumPerpackage];
        //            Array.Copy(data, next, body, 0, bytenumPerpackage);
        //            media_arr.Add(new AudioDataPacket(Command, (short)(bytenumPerpackage), body));
        //            //第二个包
        //            if ((MsgLen - 2) > bytenumPerpackage)
        //            {
        //                body = new byte[bytenumPerpackage];
        //                Array.Copy(data, next + bytenumPerpackage, body, 0, bytenumPerpackage);
        //                media_arr.Add(new AudioDataPacket(Command, (short)(bytenumPerpackage), body));
        //            }
        //            //第三个包
        //            if ((MsgLen - 2) > (2 * bytenumPerpackage))
        //            {
        //                body = new byte[bytenumPerpackage];
        //                Array.Copy(data, next + 2 * bytenumPerpackage, body, 0, bytenumPerpackage);
        //                media_arr.Add(new AudioDataPacket(Command, (short)(bytenumPerpackage), body));
        //            }

        //            //第四个包
        //            if ((MsgLen - 2) > (3 * bytenumPerpackage))
        //            {
        //                body = new byte[bytenumPerpackage];
        //                Array.Copy(data, next + 3 * bytenumPerpackage, body, 0, bytenumPerpackage);
        //                media_arr.Add(new AudioDataPacket(Command, (short)(bytenumPerpackage), body));
        //            }

        //            //第五个包
        //            if ((MsgLen - 2) > (4 * bytenumPerpackage))
        //            {
        //                body = new byte[bytenumPerpackage];
        //                Array.Copy(data, next + 4 * bytenumPerpackage, body, 0, bytenumPerpackage);
        //                media_arr.Add(new AudioDataPacket(Command, (short)(bytenumPerpackage), body));
        //            }

        //            paseLen = (short)(3 + MsgLen);
        //            //5包合一: end

        //            //old start: 一包一包收发
        //            //body = new byte[MsgLen - 2];
        //            //Array.Copy(data, next, body, 0, MsgLen - 2);
        //            //media = new AudioDataPacket(Command, (short)(MsgLen - 2), body);
        //            //paseLen = (short)(3 + MsgLen);
        //            //old end: 一包一包收发
        //            break;

        //        case MyType.TYPE_MEDIA:
        //        case MyType.TYPE_TOPOC_START:
        //        case MyType.TYPE_TOPOC_END:

        //            Debug.WriteLine("2. Data pos:" + pos + " cmd:" + Command + " len:" + MsgLen + " las:" + LastLen);

        //            if ( 3 + MsgLen > LastLen )
        //            {
        //                media = null;
        //                paseLen = 0;
        //                return;
        //            }

        //            body  = new byte[MsgLen];
        //            Array.Copy(data, next, body, 0, MsgLen);
        //            media = new AudioDataPacket( Command, MsgLen, body );
        //            paseLen = (short)(3 + MsgLen);
        //            break;

        //        default:
        //            Debug.WriteLine("3. Data pos:" + pos + " cmd:" + Command + " len:" + MsgLen + " las:" + LastLen);

        //            media   = null;
        //            paseLen = 0;
        //            break;
        //    }


        //}

        public Data(byte[] data, int pos, int LastLen)
        {
            int next = pos;
            media = null;
            media_arr = new List<AudioDataPacket>();
            // First four bytes are for the Command
            Command = BitConverter.ToInt16(data, next);
            Command = IPAddress.NetworkToHostOrder(Command);
            next += sizeof(short);

            MsgLen = data[next];
            next += sizeof(byte);

            byte[] body;

            switch (Command)
            {
                case MyType.TYPE_MEDIA_EX:
                case MyType.TYPE_TOPOC_START_EX:
                case MyType.TYPE_TOPOC_END_EX:
                    //2017.5.19 这里有类型溢出情况,要考虑
                    MsgLen = BitConverter.ToInt16(data, next);
                    MsgLen = IPAddress.NetworkToHostOrder(MsgLen);

                    Int32 i32_MsgLen = MsgLen < 0 ? 65535 + MsgLen : MsgLen;

                    Debug.WriteLine("1. Data pos:" + pos + " cmd:" + Command + " len:" + i32_MsgLen + " las:" + LastLen);

                    if ((3 + i32_MsgLen) > LastLen)
                    {
                        media = null;
                        paseLen = 0;
                        return;
                    }
                    next += sizeof(short);
                    //2017.2.17 修改为5包,或动态分析接收数据包的个数
                    //2023.9 又改回为一包一包的接收

                    //5包合一: start
                    //int bytenumPerpackage = (i32_MsgLen - 2) / 5;
                    int bytenumPerpackage = (i32_MsgLen - 2-8) / 1;

                    //第一个包
                    body = new byte[bytenumPerpackage];
                    Array.Copy(data, next, body, 0, bytenumPerpackage);
                    media_arr.Add(new AudioDataPacket(Command, (short)(bytenumPerpackage), body));

                    //改成一包一包接收后，以下不会执行了

                    //第二个包
                    if ((i32_MsgLen - 2-8) > bytenumPerpackage)
                    {
                        body = new byte[bytenumPerpackage];
                        Array.Copy(data, next + bytenumPerpackage, body, 0, bytenumPerpackage);
                        media_arr.Add(new AudioDataPacket(Command, (short)(bytenumPerpackage), body));
                    }
                    //第三个包
                    if ((i32_MsgLen - 2-8) > (2 * bytenumPerpackage))
                    {
                        body = new byte[bytenumPerpackage];
                        Array.Copy(data, next + 2 * bytenumPerpackage, body, 0, bytenumPerpackage);
                        media_arr.Add(new AudioDataPacket(Command, (short)(bytenumPerpackage), body));
                    }

                    //第四个包
                    if ((i32_MsgLen - 2-8) > (3 * bytenumPerpackage))
                    {
                        body = new byte[bytenumPerpackage];
                        Array.Copy(data, next + 3 * bytenumPerpackage, body, 0, bytenumPerpackage);
                        media_arr.Add(new AudioDataPacket(Command, (short)(bytenumPerpackage), body));
                    }

                    //第五个包
                    if ((i32_MsgLen - 2-8) > (4 * bytenumPerpackage))
                    {
                        body = new byte[bytenumPerpackage];
                        Array.Copy(data, next + 4 * bytenumPerpackage, body, 0, bytenumPerpackage);
                        media_arr.Add(new AudioDataPacket(Command, (short)(bytenumPerpackage), body));
                    }

                    paseLen = (short)(3 + i32_MsgLen);  //已分析完的长度
                    //2017.11.21 扩展的
                    group_id = 0;   //原数据包是不含组ID标识的
                    user_id = 0;    //原数据包是不含用户ID标识的

                    //5包合一: end

                    //old start: 一包一包收发
                    //body = new byte[MsgLen - 2];
                    //Array.Copy(data, next, body, 0, MsgLen - 2);
                    //media = new AudioDataPacket(Command, (short)(MsgLen - 2), body);
                    //paseLen = (short)(3 + MsgLen);
                    //old end: 一包一包收发
                    break;
                case MyType.TYPE_SOS_MEDIA_EX:
                    //2018.01.11 对SOS语音包进行解析                    
                    MsgLen = BitConverter.ToInt16(data, next);
                    MsgLen = IPAddress.NetworkToHostOrder(MsgLen);

                    Int32 i32_MsgLen_1 = MsgLen < 0 ? 65535 + MsgLen : MsgLen;

                    Debug.WriteLine("1. Data pos:" + pos + " cmd:" + Command + " len:" + i32_MsgLen_1 + " las:" + LastLen);

                    if ((3 + i32_MsgLen_1) > LastLen)
                    {
                        media = null;
                        paseLen = 0;
                        return;
                    }
                    next += sizeof(short);
                    //2017.2.17 修改为5包,或动态分析接收数据包的个数
                    //2023.9 又改回为一包一包的接收

                    //5包合一: start
                    //int bytenumPerpackage_1 = (i32_MsgLen_1 - 2-8-2) / 5;
                    int bytenumPerpackage_1 = (i32_MsgLen_1 - 2 - 8 - 2) / 1;

                    //第一个包
                    body = new byte[bytenumPerpackage_1];
                    Array.Copy(data, next, body, 0, bytenumPerpackage_1);
                    media_arr.Add(new AudioDataPacket(Command, (short)(bytenumPerpackage_1), body));

                    //改成一包一包接收后，以下不会执行了


                    //第二个包
                    if ((i32_MsgLen_1 - 2 - 8 - 2) > bytenumPerpackage_1)
                    {
                        body = new byte[bytenumPerpackage_1];
                        Array.Copy(data, next + bytenumPerpackage_1, body, 0, bytenumPerpackage_1);
                        media_arr.Add(new AudioDataPacket(Command, (short)(bytenumPerpackage_1), body));
                    }
                    //第三个包
                    if ((i32_MsgLen_1 - 2 - 8 - 2) > (2 * bytenumPerpackage_1))
                    {
                        body = new byte[bytenumPerpackage_1];
                        Array.Copy(data, next + 2 * bytenumPerpackage_1, body, 0, bytenumPerpackage_1);
                        media_arr.Add(new AudioDataPacket(Command, (short)(bytenumPerpackage_1), body));
                    }

                    //第四个包
                    if ((i32_MsgLen_1 - 2 - 8 - 2) > (3 * bytenumPerpackage_1))
                    {
                        body = new byte[bytenumPerpackage_1];
                        Array.Copy(data, next + 3 * bytenumPerpackage_1, body, 0, bytenumPerpackage_1);
                        media_arr.Add(new AudioDataPacket(Command, (short)(bytenumPerpackage_1), body));
                    }

                    //第五个包
                    if ((i32_MsgLen_1 - 2 - 8 - 2) > (4 * bytenumPerpackage_1))
                    {
                        body = new byte[bytenumPerpackage_1];
                        Array.Copy(data, next + 4 * bytenumPerpackage_1, body, 0, bytenumPerpackage_1);
                        media_arr.Add(new AudioDataPacket(Command, (short)(bytenumPerpackage_1), body));
                    }

                    paseLen = (short)(3 + i32_MsgLen_1);  //已分析完的长度
                    //2018.01.11 扩展的
                    next += (i32_MsgLen_1 - 2 - 8 - 2);

                    //groupId = BitConverter.ToInt32(msgBytes, index + 3);
                    //groupId = IPAddress.NetworkToHostOrder(groupId);
                    endpoint_type = data[next];
                    next += 1;
                    transfer_mode = data[next];
                    next += 1;

                    group_id = BitConverter.ToInt32(data, next);
                    group_id = IPAddress.NetworkToHostOrder(group_id);
                    next += 4;

                    user_id = BitConverter.ToInt32(data, next);
                    user_id = IPAddress.NetworkToHostOrder(user_id);

                    //5包合一: end                    

                    break;
                case MyType.MEDIA_EX_TOPLATFORM:
                    //2017.11.21 对转发给调度台的专包进行解析
                    MsgLen = BitConverter.ToInt16(data, next);
                    MsgLen = IPAddress.NetworkToHostOrder(MsgLen);

                    Int32 i32_MsgLen_2 = MsgLen < 0 ? 65535 + MsgLen : MsgLen;

                    Debug.WriteLine("1. Data pos:" + pos + " cmd:" + Command + " len:" + i32_MsgLen_2 + " las:" + LastLen);

                    if ((3 + i32_MsgLen_2+8) > LastLen)
                    {
                        media = null;
                        paseLen = 0;
                        return;
                    }
                    next += sizeof(short);
                    //2017.2.17 修改为5包,或动态分析接收数据包的个数
                    //5包合一: start
                    int bytenumPerpackage_2 = (i32_MsgLen_2 - 2) / 5;

                    //第一个包
                    body = new byte[bytenumPerpackage_2];
                    Array.Copy(data, next, body, 0, bytenumPerpackage_2);
                    media_arr.Add(new AudioDataPacket(Command, (short)(bytenumPerpackage_2), body));
                    //第二个包
                    if ((i32_MsgLen_2 - 2) > bytenumPerpackage_2)
                    {
                        body = new byte[bytenumPerpackage_2];
                        Array.Copy(data, next + bytenumPerpackage_2, body, 0, bytenumPerpackage_2);
                        media_arr.Add(new AudioDataPacket(Command, (short)(bytenumPerpackage_2), body));
                    }
                    //第三个包
                    if ((i32_MsgLen_2 - 2) > (2 * bytenumPerpackage_2))
                    {
                        body = new byte[bytenumPerpackage_2];
                        Array.Copy(data, next + 2 * bytenumPerpackage_2, body, 0, bytenumPerpackage_2);
                        media_arr.Add(new AudioDataPacket(Command, (short)(bytenumPerpackage_2), body));
                    }

                    //第四个包
                    if ((i32_MsgLen_2 - 2) > (3 * bytenumPerpackage_2))
                    {
                        body = new byte[bytenumPerpackage_2];
                        Array.Copy(data, next + 3 * bytenumPerpackage_2, body, 0, bytenumPerpackage_2);
                        media_arr.Add(new AudioDataPacket(Command, (short)(bytenumPerpackage_2), body));
                    }

                    //第五个包
                    if ((i32_MsgLen_2 - 2) > (4 * bytenumPerpackage_2))
                    {
                        body = new byte[bytenumPerpackage_2];
                        Array.Copy(data, next + 4 * bytenumPerpackage_2, body, 0, bytenumPerpackage_2);
                        media_arr.Add(new AudioDataPacket(Command, (short)(bytenumPerpackage_2), body));
                    }

                    paseLen = (short)(3 + i32_MsgLen_2+8);  //已分析完的长度, 多出8是指后面的groupid,userid
                    //2017.11.21 扩展的
                    next += (i32_MsgLen_2-2);

                    //groupId = BitConverter.ToInt32(msgBytes, index + 3);
                    //groupId = IPAddress.NetworkToHostOrder(groupId);

                    group_id = BitConverter.ToInt32(data, next);   
                    group_id= IPAddress.NetworkToHostOrder(group_id);
                    next += 4;

                    user_id = BitConverter.ToInt32(data, next);
                    user_id = IPAddress.NetworkToHostOrder(user_id);

                    break;
                case MyType.TYPE_MEDIA:
                case MyType.TYPE_TOPOC_START:
                case MyType.TYPE_TOPOC_END:

                    Debug.WriteLine("2. Data pos:" + pos + " cmd:" + Command + " len:" + MsgLen + " las:" + LastLen);

                    if (3 + MsgLen > LastLen)
                    {
                        media = null;
                        paseLen = 0;
                        return;
                    }

                    body = new byte[MsgLen];
                    Array.Copy(data, next, body, 0, MsgLen);
                    media = new AudioDataPacket(Command, MsgLen, body);
                    paseLen = (short)(3 + MsgLen);
                    break;

                default:
                    Debug.WriteLine("3. Data pos:" + pos + " cmd:" + Command + " len:" + MsgLen + " las:" + LastLen);

                    media = null;
                    paseLen = 0;
                    break;
            }


        }

        /// <summary>
        /// Encodes data structure
        /// </summary>
        /// <returns></returns>
        public byte[] ToByte()
        {
            //var zeroInt   = BitConverter.GetBytes(0);
            //var zeroShort = BitConverter.GetBytes((short)0);
            var result    = new List<byte>();
            short tmp = IPAddress.HostToNetworkOrder(Command);
            result.AddRange( BitConverter.GetBytes(tmp) );
            result.Add( (byte)0 );
            return result.ToArray();
        }

        //2018.1.17 直接发送SOS结束对讲
        public byte[] SOSReleaseMessageEncode(int groupid, int userid)
        {
            var result = new List<byte>();
            short tmp = IPAddress.HostToNetworkOrder((short)MyType.TYPE_SOS_KEY_RELEASE);
            result.AddRange(BitConverter.GetBytes(tmp));
            result.Add((byte)(8));
            //
            int id = IPAddress.HostToNetworkOrder(groupid);
            result.AddRange(BitConverter.GetBytes(id));
            //userID
            id = IPAddress.HostToNetworkOrder(userid);
            result.AddRange(BitConverter.GetBytes(id));
            return result.ToArray();

        }

        //2018.1.17 直接发送SOS开始对讲
        public byte[] SOSLocationMessageEncode(int groupid, int userid, int sosdatetime)
        {
            var result = new List<byte>();
            short tmp = IPAddress.HostToNetworkOrder((short)MyType.TYPE_SOS_LOCATION);
            result.AddRange(BitConverter.GetBytes(tmp));
            result.Add((byte)(0));

            short itemlen = 0;            
            itemlen = (short)(2 + 13 + 8 + 8 + 2); // GPS_TYPE 不写了为空
            tmp = IPAddress.HostToNetworkOrder(itemlen);
            result.AddRange(BitConverter.GetBytes(tmp));
            //
            //groupID
            int id = IPAddress.HostToNetworkOrder(groupid);
            result.AddRange(BitConverter.GetBytes(id));
            //userID
            id = IPAddress.HostToNetworkOrder(userid);
            result.AddRange(BitConverter.GetBytes(id));
            //sos_type 3 表示调度台发起的
            result.Add((byte)(3));
            id = IPAddress.HostToNetworkOrder(sosdatetime);
            result.AddRange(BitConverter.GetBytes(id));
            //经纬用0
            result.AddRange(BitConverter.GetBytes(0l));
            result.AddRange(BitConverter.GetBytes(0l));
            //gps_type长度为0
            tmp =0;
            tmp = IPAddress.HostToNetworkOrder(tmp);
            result.AddRange(BitConverter.GetBytes(tmp));

            return result.ToArray();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandType">1: 开启监控    2: 结束监控</param>
        /// <param name="fromUserId">发起监人的ID</param>
        /// <param name="jsonStr">目标用户ID，多个用,隔开</param>
        /// <returns></returns>
        public byte[] AVRemoteMoniMesssageEncode(byte commandType, int fromUserId, String jsonStr)
        {
            var result = new List<byte>();
            short tmp = IPAddress.HostToNetworkOrder((short)MyType.TYPE_AV_REMOTE_MONI);
            result.AddRange(BitConverter.GetBytes(tmp));
            result.Add((byte)(0));
            short itemlen = 0;
            itemlen = (2 + 1 +  4);
            short toUserIdsLen= (short)Encoding.GetEncoding("UTF-8").GetBytes(jsonStr).GetLength(0);
            itemlen = (short)(itemlen + toUserIdsLen);

            tmp = IPAddress.HostToNetworkOrder((short)itemlen);
            result.AddRange(BitConverter.GetBytes(tmp));

            result.Add(commandType);

            int id = IPAddress.HostToNetworkOrder((int)fromUserId);
            result.AddRange(BitConverter.GetBytes(id));

            var encoded = Encoding.GetEncoding("UTF-8").GetBytes(jsonStr);
            result.AddRange(encoded);

            return result.ToArray();

        }


        //2017.12.28 直接发报文请求停止终端的直播或监控
        public byte[] StopVideoLiveMoniMessageEncode(short VideoType, short VideoCommand,int userid)
        {
            var result = new List<byte>();
            short tmp = IPAddress.HostToNetworkOrder((short)MyType.RELAY_USER_MESSAGE);
            result.AddRange(BitConverter.GetBytes(tmp));

            result.Add((byte)(0));
            //内含的videomessage报文长度，所有字符串都为空，但每个长度占2
            short itemlen = 0;
            itemlen = (2 + 2 + 2 + 4);
            itemlen =(short)( itemlen + 11 * 2 + 3); //3是固定的报头
            //**********************

            short msgLength = (short)(2 + 4 + 4 + itemlen);
            tmp = IPAddress.HostToNetworkOrder(msgLength);
            result.AddRange(BitConverter.GetBytes(tmp));
            //
            int id = IPAddress.HostToNetworkOrder((int)userid);
            result.AddRange(BitConverter.GetBytes(id));

            //下面是个坑，要写成int
            int tmp_2 = IPAddress.HostToNetworkOrder((int)MyType.RELAY_USER_MESSAGE);
            result.AddRange(BitConverter.GetBytes(tmp_2));
            ////////////////////////////////////////////////////
            ///////写入具体的videomessage
            tmp = IPAddress.HostToNetworkOrder((short)MyType.VIDEO_MESSAGE);
            result.AddRange(BitConverter.GetBytes(tmp));
            result.Add((byte)(0));
            tmp = IPAddress.HostToNetworkOrder((short)(10+11*2));
            result.AddRange(BitConverter.GetBytes(tmp));

            tmp = IPAddress.HostToNetworkOrder((short)VideoType);
            result.AddRange(BitConverter.GetBytes(tmp));

            tmp = IPAddress.HostToNetworkOrder((short)VideoCommand);
            result.AddRange(BitConverter.GetBytes(tmp));
            //roomid
            result.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)0)));
            //11个
            tmp = IPAddress.HostToNetworkOrder((short)0);
            result.AddRange(BitConverter.GetBytes(tmp));
            tmp = IPAddress.HostToNetworkOrder((short)0);
            result.AddRange(BitConverter.GetBytes(tmp));
            tmp = IPAddress.HostToNetworkOrder((short)0);
            result.AddRange(BitConverter.GetBytes(tmp));
            tmp = IPAddress.HostToNetworkOrder((short)0);
            result.AddRange(BitConverter.GetBytes(tmp));
            tmp = IPAddress.HostToNetworkOrder((short)0);
            result.AddRange(BitConverter.GetBytes(tmp));
            tmp = IPAddress.HostToNetworkOrder((short)0);
            result.AddRange(BitConverter.GetBytes(tmp));

            tmp = IPAddress.HostToNetworkOrder((short)0);
            result.AddRange(BitConverter.GetBytes(tmp));
            tmp = IPAddress.HostToNetworkOrder((short)0);
            result.AddRange(BitConverter.GetBytes(tmp));
            tmp = IPAddress.HostToNetworkOrder((short)0);
            result.AddRange(BitConverter.GetBytes(tmp));
            tmp = IPAddress.HostToNetworkOrder((short)0);
            result.AddRange(BitConverter.GetBytes(tmp));
            tmp = IPAddress.HostToNetworkOrder((short)0);
            result.AddRange(BitConverter.GetBytes(tmp));

            return result.ToArray();

        }

        //2017.12.04 加入请求POC终端实时GPS定位的要求
        public byte[] GPSCommandMessageEncode(int groupID, int userID, byte locationMode, int dispatcherId)
        {
            var result = new List<byte>();
                        
            short tmp = IPAddress.HostToNetworkOrder((short)MyType.TYPE_GPS_COMMAND);
            result.AddRange(BitConverter.GetBytes(tmp));

            result.Add((byte)(0));

            tmp = IPAddress.HostToNetworkOrder((short)17);
            result.AddRange(BitConverter.GetBytes(tmp));
            result.Add((byte)(0));  //请求
            result.Add(locationMode); //定位精度要求
            //dispatcherId
            int id = IPAddress.HostToNetworkOrder((int)dispatcherId);
            result.AddRange(BitConverter.GetBytes(id));
            //groupID
            id = IPAddress.HostToNetworkOrder((int)groupID);
            result.AddRange(BitConverter.GetBytes(id));
            //userID
            id = IPAddress.HostToNetworkOrder((int)userID);
            result.AddRange(BitConverter.GetBytes(id));
            //errorCode
            result.Add((byte)(0));

            return result.ToArray();
        }

        public byte[] GroupInviteCommandMessageEncode(int groupId, String groupName, int userId, int inviteId)
        {
            var result = new List<byte>();

            short tmp = IPAddress.HostToNetworkOrder((short)MyType.TYPE_RECIVED_INVITE);
            result.AddRange(BitConverter.GetBytes(tmp));
            var fromUserNameLen = Encoding.GetEncoding("UTF-8").GetBytes(groupName).GetLength(0);
            result.Add((byte)(13 + fromUserNameLen));  //一般组名称不会太长，如果太长，调用端传空字符串也可以

            int itmp = IPAddress.HostToNetworkOrder(groupId);
            result.AddRange(BitConverter.GetBytes(itmp));

            result.Add((byte)(fromUserNameLen));

            var encoded = Encoding.GetEncoding("UTF-8").GetBytes(groupName);
            result.AddRange(encoded);

            itmp = IPAddress.HostToNetworkOrder(userId);
            result.AddRange(BitConverter.GetBytes(itmp));

            itmp = IPAddress.HostToNetworkOrder(inviteId);
            result.AddRange(BitConverter.GetBytes(itmp));

            return result.ToArray();
        }

        /// <summary>
        /// 该命令前提是对方已经有了指定的组，然后才能同步
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="groupName"></param>
        /// <param name="userId"></param>
        /// <param name="inviteId"> 被邀请的同步用户ID</param>
        /// <returns></returns>
        public byte[] GroupSyncCommandMessageEncode(int groupId, String groupName, int userId, int inviteId)
        {
            var result = new List<byte>();

            short tmp = IPAddress.HostToNetworkOrder((short)MyType.TYPE_GROUP_SYNC);
            result.AddRange(BitConverter.GetBytes(tmp));
            var fromUserNameLen = Encoding.GetEncoding("UTF-8").GetBytes(groupName).GetLength(0);
            result.Add((byte)(13+ fromUserNameLen));  //一般组名称不会太长，如果太长，调用端传空字符串也可以

            int itmp = IPAddress.HostToNetworkOrder(groupId);
            result.AddRange(BitConverter.GetBytes(itmp));

            result.Add((byte)(fromUserNameLen));

            var encoded = Encoding.GetEncoding("UTF-8").GetBytes(groupName);
            result.AddRange(encoded);

            itmp = IPAddress.HostToNetworkOrder(userId);
            result.AddRange(BitConverter.GetBytes(itmp));

            itmp = IPAddress.HostToNetworkOrder(inviteId);
            result.AddRange(BitConverter.GetBytes(itmp));

            return result.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meetType"></param>
        /// <param name="meetCommand"></param>
        /// <param name="rommId">房间号，为一个short类型整数，只要唯一</param>
        /// <param name="fromUserId"></param>
        /// <param name="toUserId"></param>
        /// <param name="fromUserName"></param>
        /// <param name="toUserName"></param>
        /// <param name="desc">desc中直接传入 streamBig （高分辨率）, streamSmall （低分辨率）, streamSub (屏幕分享)</param>
        /// <returns></returns>
        public byte[] MeetChatCommandMessageEncode(short meetType, short meetCommand, short rommId,
            int fromUserId, int toUserId, string fromUserName, string toUserName, string desc)
        {
            var result = new List<byte>();

            short tmp = IPAddress.HostToNetworkOrder((short)MyType.TYPE_MEET_CHAT);
            result.AddRange(BitConverter.GetBytes(tmp));

            result.Add((byte)(0));


            var fromUserNameLen = Encoding.GetEncoding("UTF-8").GetBytes(fromUserName).GetLength(0);
            var toUserNameLen = Encoding.GetEncoding("UTF-8").GetBytes(toUserName).GetLength(0);
            var descLen = Encoding.GetEncoding("UTF-8").GetBytes(desc).GetLength(0);
            //计算长度
            int packSize = 22 + fromUserNameLen + toUserNameLen + descLen;

            tmp = IPAddress.HostToNetworkOrder((short)packSize);
            result.AddRange(BitConverter.GetBytes(tmp));

            tmp = IPAddress.HostToNetworkOrder(meetType);
            result.AddRange(BitConverter.GetBytes(tmp));

            tmp = IPAddress.HostToNetworkOrder(meetCommand);
            result.AddRange(BitConverter.GetBytes(tmp));

            tmp = IPAddress.HostToNetworkOrder(rommId);
            result.AddRange(BitConverter.GetBytes(tmp));

            int itmp = IPAddress.HostToNetworkOrder(fromUserId);
            result.AddRange(BitConverter.GetBytes(itmp));

            itmp = IPAddress.HostToNetworkOrder(toUserId);
            result.AddRange(BitConverter.GetBytes(itmp));

            //
            tmp = IPAddress.HostToNetworkOrder((short)fromUserNameLen);
            result.AddRange(BitConverter.GetBytes(tmp));

            var encoded = Encoding.GetEncoding("UTF-8").GetBytes(fromUserName);
            result.AddRange(encoded);
            //

            tmp = IPAddress.HostToNetworkOrder((short)toUserNameLen);
            result.AddRange(BitConverter.GetBytes(tmp));

            encoded = Encoding.GetEncoding("UTF-8").GetBytes(toUserName);
            result.AddRange(encoded);
            //
            tmp = IPAddress.HostToNetworkOrder((short)descLen);
            result.AddRange(BitConverter.GetBytes(tmp));

            encoded = Encoding.GetEncoding("UTF-8").GetBytes(desc);
            result.AddRange(encoded);

            return result.ToArray();
        }

        public byte[] MeetScreenShareCommandMessageEncode(short shareCommand,
            int fromUserId, int toUserId, string fromUserName, string toUserName, string desc)
        {
            var result = new List<byte>();

            short tmp = IPAddress.HostToNetworkOrder((short)MyType.TYPE_MEET_SCREEN_SHARE);
            result.AddRange(BitConverter.GetBytes(tmp));

            result.Add((byte)(0));


            var fromUserNameLen = Encoding.GetEncoding("UTF-8").GetBytes(fromUserName).GetLength(0);
            var toUserNameLen = Encoding.GetEncoding("UTF-8").GetBytes(toUserName).GetLength(0);
            var descLen = Encoding.GetEncoding("UTF-8").GetBytes(desc).GetLength(0);
            //计算长度
            int packSize = 18 + fromUserNameLen + toUserNameLen + descLen;

            tmp = IPAddress.HostToNetworkOrder((short)packSize);
            result.AddRange(BitConverter.GetBytes(tmp));            

            tmp = IPAddress.HostToNetworkOrder(shareCommand);
            result.AddRange(BitConverter.GetBytes(tmp));

            int itmp = IPAddress.HostToNetworkOrder(fromUserId);
            result.AddRange(BitConverter.GetBytes(itmp));

            itmp = IPAddress.HostToNetworkOrder(toUserId);
            result.AddRange(BitConverter.GetBytes(itmp));

            //
            tmp = IPAddress.HostToNetworkOrder((short)fromUserNameLen);
            result.AddRange(BitConverter.GetBytes(tmp));

            var encoded = Encoding.GetEncoding("UTF-8").GetBytes(fromUserName);
            result.AddRange(encoded);
            //

            tmp = IPAddress.HostToNetworkOrder((short)toUserNameLen);
            result.AddRange(BitConverter.GetBytes(tmp));

            encoded = Encoding.GetEncoding("UTF-8").GetBytes(toUserName);
            result.AddRange(encoded);
            //
            tmp = IPAddress.HostToNetworkOrder((short)descLen);
            result.AddRange(BitConverter.GetBytes(tmp));

            encoded = Encoding.GetEncoding("UTF-8").GetBytes(desc);
            result.AddRange(encoded);

            return result.ToArray();
        }

        public byte[] AVChatNewCommandMessageEncode(short videoType, short videoCommand,
            int fromUserId, int toUserId, string fromUserName, string toUserName, string desc )
        {
            var result = new List<byte>();

            short tmp = IPAddress.HostToNetworkOrder((short)MyType.TYPE_AV_CHAT_NEW);
            result.AddRange(BitConverter.GetBytes(tmp));

            result.Add((byte)(0));            

            
            var fromUserNameLen = Encoding.GetEncoding("UTF-8").GetBytes(fromUserName).GetLength(0);
            var toUserNameLen = Encoding.GetEncoding("UTF-8").GetBytes(toUserName).GetLength(0);
            var descLen = Encoding.GetEncoding("UTF-8").GetBytes(desc).GetLength(0);
            //计算长度
            int packSize = 20+ fromUserNameLen+ toUserNameLen+ descLen;         
                                 
            tmp = IPAddress.HostToNetworkOrder((short)packSize);
            result.AddRange(BitConverter.GetBytes(tmp));

            tmp = IPAddress.HostToNetworkOrder(videoType);
            result.AddRange(BitConverter.GetBytes(tmp));

            tmp = IPAddress.HostToNetworkOrder(videoCommand);
            result.AddRange(BitConverter.GetBytes(tmp));

            int itmp = IPAddress.HostToNetworkOrder(fromUserId);
            result.AddRange(BitConverter.GetBytes(itmp));

            itmp = IPAddress.HostToNetworkOrder(toUserId);
            result.AddRange(BitConverter.GetBytes(itmp));

            //
            tmp = IPAddress.HostToNetworkOrder((short)fromUserNameLen);
            result.AddRange(BitConverter.GetBytes(tmp));

            var encoded = Encoding.GetEncoding("UTF-8").GetBytes(fromUserName);
            result.AddRange(encoded);
            //

            tmp = IPAddress.HostToNetworkOrder((short)toUserNameLen);
            result.AddRange(BitConverter.GetBytes(tmp));

            encoded = Encoding.GetEncoding("UTF-8").GetBytes(toUserName);
            result.AddRange(encoded);
            //
            tmp = IPAddress.HostToNetworkOrder((short)descLen);
            result.AddRange(BitConverter.GetBytes(tmp));

            encoded = Encoding.GetEncoding("UTF-8").GetBytes(desc);
            result.AddRange(encoded);
           
            return result.ToArray();

        }

        public  byte[] NoticeMessageEncode(int fromUserID, int toUserID, int noticeId, int createtime )
        {
            var result = new List<byte>();
            //2018.01.30             
            short tmp = IPAddress.HostToNetworkOrder((short)MyType.TYPE_DISPATCHER_SEND_NOTICE);
            result.AddRange(BitConverter.GetBytes(tmp));
            result.Add((byte)(16));
            int id = IPAddress.HostToNetworkOrder(fromUserID);
            result.AddRange(BitConverter.GetBytes(id));

            id = IPAddress.HostToNetworkOrder(toUserID);
            result.AddRange(BitConverter.GetBytes(id));

            id = IPAddress.HostToNetworkOrder(noticeId);
            result.AddRange(BitConverter.GetBytes(id));

            id = IPAddress.HostToNetworkOrder(createtime);
            result.AddRange(BitConverter.GetBytes(id));

            return result.ToArray();
        }

        //2019.02.10 远程切换前后摄像头
        public byte[] CameraSwitchMessageEncode(int groupID, int userID)
        {
            var result = new List<byte>();
            
            short tmp = IPAddress.HostToNetworkOrder((short)MyType.CAMERA_SWITCH);

            result.AddRange(BitConverter.GetBytes(tmp));

            result.Add((byte)(sizeof(int) + sizeof(int)));
            int id = IPAddress.HostToNetworkOrder((int)groupID);
            result.AddRange(BitConverter.GetBytes(id));
            id = IPAddress.HostToNetworkOrder((int)userID);
            result.AddRange(BitConverter.GetBytes(id));
            return result.ToArray();
        }

        public  byte[] ReportMessageEncode(int groupID, int userID)
        {
            var result = new List<byte>();

            //2017.11.16 这里故意改为调度台的上报为TYPE_REPROT_PLATFORM 
            //short tmp = IPAddress.HostToNetworkOrder((short)MyType.TYPE_REPROT);
            short tmp = IPAddress.HostToNetworkOrder((short)MyType.TYPE_REPROT_PLATFORM);

            result.AddRange( BitConverter.GetBytes(tmp) );

            result.Add( (byte)( sizeof(int)+sizeof(int)) );
            int id = IPAddress.HostToNetworkOrder((int)groupID);
            result.AddRange(BitConverter.GetBytes(id));
            id = IPAddress.HostToNetworkOrder((int)userID);
            result.AddRange(BitConverter.GetBytes(id));
            return result.ToArray();
        }

        public  byte[] LogoutMessageEncode(int groupID, int userID)
        {
            var result = new List<byte>();

            short tmp = IPAddress.HostToNetworkOrder((short)MyType.TYPE_LOGOUT);
            result.AddRange( BitConverter.GetBytes(tmp) );

            result.Add( (byte)( sizeof(int)+sizeof(int)) );

            int id = IPAddress.HostToNetworkOrder((int)groupID);
            result.AddRange(BitConverter.GetBytes(id));
            id = IPAddress.HostToNetworkOrder((int)userID);
            result.AddRange(BitConverter.GetBytes(id));

            return result.ToArray();
        }

        public byte[] MediaEncode( byte[] outbuf )
        {
            var result = new List<byte>();

            short tmp = IPAddress.HostToNetworkOrder( (short)MyType.TYPE_MEDIA_EX );
            result.AddRange( BitConverter.GetBytes(tmp) );
            result.Add((byte)0);

            short len = IPAddress.HostToNetworkOrder( (short) (2+outbuf.Length+8) );
            result.AddRange( BitConverter.GetBytes(len) );
            result.AddRange(outbuf);
            //2023.9 增加毫秒时间戳，将来用于AEC
            result.AddRange(BitConverter.GetBytes(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond));

            return result.ToArray();
        }

        /// <summary>
        /// 专用于文件广播帧的数据包
        /// </summary>
        /// <param name="outbuf"></param>
        /// <returns></returns>
        public byte[] MediaEncode_FileFrame(byte[] outbuf)
        {
            var result = new List<byte>();

            short tmp = IPAddress.HostToNetworkOrder((short)MyType.TYPE_MEDIA_EX_FILE_FRAME);
            result.AddRange(BitConverter.GetBytes(tmp));
            result.Add((byte)0);

            short len = IPAddress.HostToNetworkOrder((short)(2 + outbuf.Length + 8));
            result.AddRange(BitConverter.GetBytes(len));
            result.AddRange(outbuf);
            //2023.9 增加毫秒时间戳，将来用于AEC
            result.AddRange(BitConverter.GetBytes(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond));

            return result.ToArray();
        }

        //2018.01.12 增加SOS语音数据包
        public byte[] MediaEncode_SOS(byte[] outbuf,byte endpoint_type,byte transfer_mode,int groupid, int userid )
        {
            var result = new List<byte>();

            short tmp = IPAddress.HostToNetworkOrder((short)MyType.TYPE_SOS_MEDIA_EX);
            Debug.WriteLine("MyType.TYPE_SOS_MEDIA_EX:"+ MyType.TYPE_SOS_MEDIA_EX);
            Debug.WriteLine("MyType.TYPE_SOS_MEDIA_EX groupid:" + groupid);
            Debug.WriteLine("MyType.TYPE_SOS_MEDIA_EX userid:" + userid);
            Debug.WriteLine("MyType.TYPE_SOS_MEDIA_EX endpoint_type:" + endpoint_type);
            result.AddRange(BitConverter.GetBytes(tmp));
            result.Add((byte)0);

            short len = IPAddress.HostToNetworkOrder((short)(2 + outbuf.Length+ 8 + 1 + 1 ));            
            result.AddRange(BitConverter.GetBytes(len));
            result.AddRange(outbuf);
            //
            result.Add(endpoint_type);
            result.Add(transfer_mode);

            groupid = IPAddress.HostToNetworkOrder(groupid);
            result.AddRange(BitConverter.GetBytes(groupid));

            userid = IPAddress.HostToNetworkOrder(userid);
            result.AddRange(BitConverter.GetBytes(userid));

            return result.ToArray();
        }

        /**
         * 发送命令向服务器 Constant.TYPE_P2P_CHAT :发送文本 Constant.TYPE_P2G_CHAT :发送文本
         */
        public byte[] ContentMessageEncode( short messageId, ChatMessage data )
        {
            byte  charsetnamelen = (byte) Encoding.GetEncoding("UTF-8").GetBytes( data.getCharsetname() ).GetLength(0);
            short contentlen = (short)Encoding.GetEncoding("UTF-8").GetBytes( data.getMsgcontent() ).GetLength(0);
            short length = 0;
            short tmp   = 0;
            int   gid   = 0;
            int   uid   = 0;

            var result = new List<byte>();

            switch (messageId)
            {
                case MyType.TYPE_P2G_CHAT_TEXT:
                    length = (short)(2 + 4 + 4 + 1 + charsetnamelen + contentlen);
                    // packet = new byte[3 + length];
                    // 1. messageId
                    tmp = IPAddress.HostToNetworkOrder( (short)messageId );
                    result.AddRange( BitConverter.GetBytes(tmp) );
                    // 2. length
                    result.Add( (byte)0 );
                    // 3. MsgContentLength总长
                    tmp = IPAddress.HostToNetworkOrder((short)length);
                    result.AddRange(BitConverter.GetBytes(tmp));
                    // 4. groupID
                    gid = int.Parse(data.getGroupid()); 
                    gid = IPAddress.HostToNetworkOrder(gid);
                    result.AddRange(BitConverter.GetBytes(gid));
                    // 5. userId
                    uid = int.Parse(data.getUserid());
                    uid = IPAddress.HostToNetworkOrder(uid);
                    result.AddRange(BitConverter.GetBytes(uid));
                    // 6. CharsetNamelenght
                    result.Add((byte)charsetnamelen);
                    // 7. CharsetName
                    result.AddRange(Encoding.GetEncoding("UTF-8").GetBytes(data.getCharsetname()));
                    // 8. MsgContent
                    result.AddRange(Encoding.GetEncoding("UTF-8").GetBytes(data.getMsgcontent()));
                    break;

                case MyType.TYPE_P2P_CHAT_TEXT:
                    length = (short)(2 + 4 + 4 + 1 + charsetnamelen + contentlen);
                    // packet = new byte[3 + length];
                    // 1. messageId
                    tmp = IPAddress.HostToNetworkOrder( (short)messageId );
                    result.AddRange( BitConverter.GetBytes(tmp) );
                    // 2. length
                    result.Add( (byte)0 );
                    // 3. MsgContentLength总长
                    tmp = IPAddress.HostToNetworkOrder((short)length);
                    result.AddRange(BitConverter.GetBytes(tmp));
                    // 4. groupID
                    gid = int.Parse(data.getGroupid()); 
                    gid = IPAddress.HostToNetworkOrder(gid);
                    result.AddRange(BitConverter.GetBytes(gid));
                    // 5. userId
                    uid = int.Parse(data.getUserid());
                    uid = IPAddress.HostToNetworkOrder(uid);
                    result.AddRange(BitConverter.GetBytes(uid));
                    // 6. CharsetNamelenght
                    result.Add((byte)charsetnamelen);
                    // 7. CharsetName
                    result.AddRange(Encoding.GetEncoding("UTF-8").GetBytes(data.getCharsetname()));
                    // 8. MsgContent
                    result.AddRange(Encoding.GetEncoding("UTF-8").GetBytes(data.getMsgcontent()));
                    break;
            }
            return result.ToArray();
        }

        private static void Encode( string str, List<byte> result )
        {
            var encoded = Encoding.GetEncoding("UTF-8").GetBytes(str);
            var len     = Encoding.GetEncoding("UTF-8").GetBytes(str).GetLength(0);

            short tmp = IPAddress.HostToNetworkOrder((short)len);
            result.AddRange( BitConverter.GetBytes(tmp) );
            result.AddRange( encoded );
        }

        private static void Encode( IReadOnlyCollection<byte> data, List<byte> result )
        {
            int tmp = IPAddress.HostToNetworkOrder((int)data.Count);
            result.AddRange( BitConverter.GetBytes(tmp) );
            result.AddRange( data );
        }
    }

    public static class MyType
    {
        // TCP MessageID
        public const short TYPE_MEDIA            = 0; //Media消息
        public const short TYPE_REPROT           = 1; //上报
        public const short TYPE_CHECK_CLINET     = 2; //心跳检查
        public const short TYPE_CLIENT_RESPONSE  = 3;
        public const short TYPE_CHECK_SERVER     = 4; //心跳检查
        public const short TYPE_SERVICE_RESPONSE = 5;
        public const short TYPE_LOGIN            = 6;
        public const short TYPE_LOGOUT           = 7;
        //
        public const short TYPE_REPROT_PLATFORM = 9;  //2017.11.16 给调度台专用

        //抢麦(普通对讲模式的麦)
        public const short TYPE_ROB_MIC      = 10;//groupId ,userId

        //抢麦成功
        public const short TYPE_MIC_SUCCESS  = 11;//groupId ,userId

        //抢麦失败
        public const short TYPE_MIC_FAILED   = 12;//groupId ,userId

        //释放麦
        public const short TYPE_REALASE_MIC  = 13;//groupId ,userId

        public const short TYPE_TOPOC_START_MIC = 16;//当开始要实现PTT转POC
        public const short TYPE_TOPOC_REALASE_MIC = 17;//当结束要实现PTT转POC

        //增加广播模式的抢麦，因为要解决调度台的麦超时不设限制，或者是个很大的值(如 1小时)
        public const short TYPE_ROB_BROAD_MIC = 18;

        //nofication
        public const short TYPE_KICK_USER = 20;//userName ,userId 
        public const short TYPE_EXIT_GROUP = 21;//userName,userId
        /**收到某用户创建的会话邀请*/
        public const short TYPE_RECIVED_INVITE = 22;// invite id,group id  
        //nofication
        public const short TYPE_ACCEPT_INVITE = 23;//
        public const short TYPE_REJECT_INVITE = 24;//
        //申请处理
        public const short TYPE_RECIVED_APPLY = 25;//apply id
        //nofication
        public const short TYPE_ACCEPT_APPLY = 26;//
        public const short TYPE_REJECT_APPLY = 27;//
        //nofication
        public const short TYPE_DELETE_GROUP = 28;//groupName
        public const short TYPE_CREATE_GROUP = 29;//PC端不收该数据

        //消息类型
        public const short TYPE_P2P_CHAT_TEXT = 30;
        public const short TYPE_P2G_CHAT_TEXT = 31;
        public const short TYPE_P2P_CHAT_FILE = 32;
        public const short TYPE_P2G_CHAT_FILE = 33;

        public const short TYPE_TOPOC_START = 34; //当开始要实现PTT转POC 十六进制 22
        public const short TYPE_TOPOC_END = 35;   //当结束要实现PTT转POC 十六进制 23
        //2017.12.04 GPS实时定位
        public const short TYPE_GPS_COMMAND = 36;
        //2018.01.10 SOS定位
        public const short TYPE_SOS_LOCATION = 38;
        //SOS报文的语音数据包, 2018.1.2
        public const short TYPE_SOS_MEDIA_EX = 39;
        //SOS键释放的报文，服务器用来结束录音
        public const short TYPE_SOS_KEY_RELEASE = 62;

        public const short TYPE_P2P_MESSAGES = 40;
        public const short TYPE_P2G_MESSAGES = 41;

        /**系统消息*/
        public const short TYPE_SYS_MESSAGE = 42;
        /**某用户邀请单聊*/
        public const short TYPE_PERSON_INVITE = 43;
        /**某用户释放邀请单聊*/
        public const short TYPE_PERSON_INVITE_RELEASE = 44;

        /// <summary>
        /// 同帐号的互相踢除，后登入的踢出前一个,2019.1.5 add
        /// </summary>
        public const short KICK_OFF = 45;
        /// <summary>
        /// 远程切换前后摄像头
        /// </summary>
        public const short CAMERA_SWITCH = 46;

        public const short RELAY_USER_MESSAGE = 61;

        /**调度台发送到POC的公告通知报文*/
        public const short TYPE_DISPATCHER_SEND_NOTICE = 63;

        /**视频统一消息，这里用于调度台实时收到消息推送成功、停止报文*/
        public const short VIDEO_MESSAGE = 70;    //视频统一消息

        public const short TYPE_GROUP_SYNC = 71;  //强制同步到指定组讲话

        public const short TYPE_AV_CHAT_NEW = 75;  //2022.11.08 新版的音视频通话协议

        public const short TYPE_AV_REMOTE_MONI = 76;  //2023.8  远程监控调取,本质上就是app端直播

        //MediaEX消息
        public const short TYPE_TOPOC_START_EX = 94; //当开始要实现PTT转POC 十六进制 22
        public const short TYPE_TOPOC_END_EX = 95; //当结束要实现PTT转POC 十六进制 23        
        public const short TYPE_MEDIA_EX = 99;
        //MediaEX消息
        //2017.11.21 增加服务端发来的专用于调度台的专包
        public const short MEDIA_EX_TOPLATFORM = 100;

        /// <summary>
        /// 文件广播用此帧来做，因为不考虑 AEC回给自己情况
        /// </summary>
        public const short TYPE_MEDIA_EX_FILE_FRAME = 101;

        //2023.10.8 邀请视频会议,专用于PC端发起的视频会商功能
        public const short TYPE_MEET_CHAT = 102;   

        //2023.10.8 视频会议中的屏幕分享通知(开始,结束)
        public const short TYPE_MEET_SCREEN_SHARE = 103;	 

        //2023.8.16 用于修改群组名称，或将来扩展属性 
        public const short TYPE_MODIFY_GROUP = 110;

    }

    public static class MySysMsgType
    {
        /**某用户开始说话*/
        public  const int SYS_MSSAGE_TALK_START    = 1;
        /**某用户停止说话*/
        public  const int SYS_MSSAGE_TALK_STOP     = 2;
        /**某用户进入某群组*/
        public  const int SYS_MSSAGE_IN_GROUP      = 3;
        /**某用户离开某群组*/
        public  const int SYS_MSSAGE_OUT_GROUP     = 4;
        /**某用户拒绝邀请*/
        public  const int SYS_MSSAGE_REJECT_INVITE = 5;

        // /**某用户邀请单聊*/
        // public final const int SYS_MSSAGE_PERSON_INVITE =6;

        /**某用户同意单聊邀请*/
        public  const int SYS_MSSAGE_ENTER_PRESON = 6;
        /**某用户拒绝单聊邀请*/
        public  const int SYS_MSSAGE_EXIT_PRESON  = 7;

        /**某用户上线*/
        public  const int SYS_MSSAGE_ONLINE_PRESON  = 8;
        /**某用户掉线*/
        public  const int SYS_MSSAGE_OFFLINE_PRESON = 9;

        /**转POC某用户开始说话*/
        public  const int SYS_MSSAGE_TALK_START_TOPOC = 10;
        /**转POC某用户停止说话*/
        public  const int SYS_MSSAGE_TALK_STOP_TOPOC = 11;

        /**呼叫中对方正在通话*/
        public const  int SYS_MSSAGE_TALK_INCALL = 12;

        public const  int TYPE_TOPOC_START_MIC = 16;//申请中继台成功
        public const  int TYPE_TOPOC_FAIL_MIC  = 17; //申请中继台失败或释放中继台

        public const  int TYPE_TOPOC_RELEASE_SUCCESS_MIC = 18;//申请中继台成功
        public const  int TYPE_TOPOC_RELEASE_FAIL_MIC    = 19;//申请中继台失败或释放中继台
    }

    public static class MySysWorkType
    {
        /**某用户开始说话*/
        public  const int SYS_MSSAGE_WORK_NULL   = 0;
        /**某用户开始说话*/
        public  const int SYS_MSSAGE_PRESON_TALK = 1;
        /**某用户开始说话*/
        public  const int SYS_MSSAGE_PRESON_CHAT = 2;
    }

    public class AudioDataPacket
    {
        //2016.3.21
        private short  length;   //1 btye  //2016.3.21 change byte to short
        private short  messageId;//2 btye
        private byte[] body;     //

        public AudioDataPacket()
        {
        }

        public AudioDataPacket(short messageId, short length, byte[] body)
        {
            this.length    = length;
            this.messageId = messageId;
            this.body      = body;
        }

        public short getLength()
        {
            return length;
        }

        public void setLength(short length)
        {
            this.length = length;
        }

        public byte[] getBody()
        {
            return body;
        }

        public void setBody(byte[] body)
        {
            this.body = body;
        }

        public String toString()
        {
            return "AudioDataPacket [messageId=" + messageId + ",audio length="
                    + length + "]";
        }

        public short getMessageId()
        {
            return messageId;
        }

        public void setMessageId(short messageId)
        {
            this.messageId = messageId;
        }
    }


    public class ManageDataPacket
    {
        private short  messageId;     //2 byte
        private byte   length;        //1 byte

        private int    groupId;       //4 byte
        private int    userId;        //4 byte
        private byte   userNameLen;   //1 byte
        private String userName;      //
        private byte   groupNameLen;  //1 byte
        private int    type;          //4 byte
        private int    worktype;      //4 type
        private int    inviteId;      //4 byte
        //2017.12.04 以下用于兼容GPSCommandMessage
        /**
	 * 消息类型（0：请求  1：结果）
	 */
        private byte requestType;
        /**
	 * 定位类型（0：自动 1：一般  2：高精）
	 */
        private byte locationMode;
        /**
	 * 调度员ID
	 */
        private int dispatcherId;
        /**
	 * 返回码（0：成功  1：终端gps权限未打开 2:数据库写入失败）
	 */
        private byte errorCode;

        /**
	 * SOS类型, 报警级别,1表示SOS, 2 表示其它
	 */
        private byte sos_type;

        /**
         * SOS报警时间,秒数
         */
        private int sos_datetime;

        /**
         * 经度
         */
        private double longitude;

        /**
         * 纬度
         */
        private double latitude;

        /**
         * 定位类型: 如 baidu,gps,Google
         */
        private String gps_type;

        /// <summary>
        /// sos 的id字段
        /// </summary>
        private long sos_id;

        public long getSos_id()
        {
            return sos_id;
        }

        public void setSos_id(long sos_id)
        {
            this.sos_id = sos_id;
        }

        public byte getSos_type()
        {
            return sos_type;
        }

        public void setSos_type(byte sos_type)
        {
            this.sos_type = sos_type;
        }

        public int getSos_datetime()
        {
            return sos_datetime;
        }

        public void setSos_datetime(int sos_datetime)
        {
            this.sos_datetime = sos_datetime;
        }

        public double getLatitude()
        {
            return latitude;
        }

        public void setLatitude(double latitude)
        {
            this.latitude = latitude;
        }

        public double getLongitude()
        {
            return longitude;
        }

        public void setLongitude(double longitude)
        {
            this.longitude = longitude;
        }

        public String getGps_type()
        {
            return gps_type;
        }

        public void setGps_type(String gps_type)
        {
            this.gps_type = gps_type;
        }

        //
        public byte getRequestType()
        {
            return requestType;
        }

        public void setRequestType(byte requestType)
        {
            this.requestType = requestType;
        }

        public byte getLocationMode()
        {
            return locationMode;
        }

        public void setLocationMode(byte locationMode)
        {
            this.locationMode = locationMode;
        }

        public int getDispatcherId()
        {
            return dispatcherId;
        }

        public void setDispatcherId(int dispatcherId)
        {
            this.dispatcherId = dispatcherId;
        }

        public byte getErrorCode()
        {
            return errorCode;
        }

        public void setErrorCode(byte errorCode)
        {
            this.errorCode = errorCode;
        }

        public byte getGroupNameLen()
        {
            return groupNameLen;
        }

        public void setGroupNameLen(byte groupNameLen)
        {
            this.groupNameLen = groupNameLen;
        }

        private String groupName;// 1 byte

        private int msgSerialNo;// 4 byte

        public ManageDataPacket()
        {
        }

        public short getMessageId()
        {
            return messageId;
        }

        public void setMessageId(short messageId)
        {
            this.messageId = messageId;
        }

        public byte getLength()
        {
            return length;
        }

        public void setLength(byte length)
        {
            this.length = length;
        }

        public int getGroupId()
        {
            return groupId;
        }

        public void setGroupId(int groupId)
        {
            this.groupId = groupId;
        }

        public int getUserId()
        {
            return userId;
        }

        public void setUserId(int userId)
        {
            this.userId = userId;
        }

        public byte getUserNameLen()
        {
            return userNameLen;
        }

        public void setUserNameLen(byte userNameLen)
        {
            this.userNameLen = userNameLen;
        }

        public String getUserName()
        {
            return userName;
        }

        public void setUserName(String userName)
        {
            this.userName = userName;
        }

        public String getGroupName()
        {
            return groupName;
        }

        public void setGroupName(String groupName)
        {
            this.groupName = groupName;
        }

        public int getType()
        {
            return type;
        }

        public void setType(int type)
        {
            this.type = type;
        }


        public int getWorkType()
        {
            return worktype;
        }
        public void setWorkType(int worktype)
        {
            this.worktype = worktype;
        }
        public int getInviteId()
        {
            return inviteId;
        }
        public void setInviteId(int inviteId)
        {
            this.inviteId = inviteId;
        }

        public int getMsgSerialNo()
        {
            return msgSerialNo;
        }

        public void setMsgSerialNo(int msgSerialNo)
        {
            this.msgSerialNo = msgSerialNo;
        }

        public String toString()
        {
            return "ManageDataPacket [messageId=" + messageId + ", length="
                    + length + ", groupId=" + groupId + ", userId=" + userId
                    + ", userNameLen=" + userNameLen + ", userName=" + userName
                    + ", groupNameLen=" + groupNameLen + ", type=" + type
                    + ", worktype=" + worktype + ", inviteId=" + inviteId
                    + ", groupName=" + groupName + ", msgSerialNo=" + msgSerialNo
                    + "]";
        }

    }

    public class GPSCommandMessage
    {
        private short msglen;
        private byte requestType;
        private byte locationMode;
        private int dispatcherId;
        private int groupId;
        private int userId;
        private byte errorCode;
        public GPSCommandMessage()
        {
            
        }

        public String toString()
        {
            return "GPSCommandMessage [msglen=" + msglen + ", requestType="
                    + requestType + ", locationMode=" + locationMode
                    + ", dispatcherId=" + dispatcherId + ", groupId=" + groupId
                    + ", userId=" + userId + ", errorCode=" + errorCode + "]";
        }

        public short getMsglen()
        {
            return msglen;
        }

        public void setMsglen(short msglen)
        {
            this.msglen = msglen;
        }

        public byte getRequestType()
        {
            return requestType;
        }

        public void setRequestType(byte requestType)
        {
            this.requestType = requestType;
        }

        public byte getLocationMode()
        {
            return locationMode;
        }

        public void setLocationMode(byte locationMode)
        {
            this.locationMode = locationMode;
        }

        public int getDispatcherId()
        {
            return dispatcherId;
        }

        public void setDispatcherId(int dispatcherId)
        {
            this.dispatcherId = dispatcherId;
        }

        public int getGroupId()
        {
            return groupId;
        }

        public void setGroupId(int groupId)
        {
            this.groupId = groupId;
        }

        public int getUserId()
        {
            return userId;
        }

        public void setUserId(int userId)
        {
            this.userId = userId;
        }

        public byte getErrorCode()
        {
            return errorCode;
        }

        public void setErrorCode(byte errorCode)
        {
            this.errorCode = errorCode;
        }

    }


    public class AVChatNewMessage
    {
        /**
     * 整个消息的长度(除固定头3个字节外)
     */
        public short msgLength { get; set; }

        public short video_type { get; set; }   //视频类型: 1、语音通话  2、视频通话
        public short video_command { get; set; } //视频命令， 1: 请求  2: 应答
        public int fromUserId { get; set; }
        public int toUserId { get; set; }

        public String fromUserName { get; set; }


        public String toUserName { get; set; }

        public String desc { get; set; }
        public AVChatNewMessage()
        {

        }

        public String toString()
        {
            return "RequestAVChatNewPacket{" +                     
                    ", msgLength=" + msgLength +
                    ", video_type=" + video_type +
                    ", video_command=" + video_command +
                    ", fromUserId=" + fromUserId +
                    ", toUserId=" + toUserId +
                    ", fromUserName='" + fromUserName + '\'' +
                    ", toUserName='" + toUserName + '\'' +
                    ", desc='" + desc + '\'' +
                    '}';
        }

    }

    public class VideoMessage
    {
        private short msglen;
        private short video_type;    //视频类型: 视频广播、视频监控、视频会议、视频通话
        private short video_command; //视频命令
        private String video_join_type;  //仅用于视频会议类型，表示参考类型: main, join 
        private String app;
        private String stream;
        private String streamurl;
        private String roomname;  //会议室名称
        private int roomid = 0;    //会议室名称
        private String roomdesc;  //会议附加描述
                                  //以下扩展
        private String ext1;  //
        private String ext2;
        private String ext3;
        private String ext4;
        private String ext5;

        public VideoMessage()
        {

        }

        public String toString()
        {
            return "VideoMessage [msglen=" + msglen + ", video_type=" + video_type
                    + ", video_command=" + video_command + ", video_join_type="
                    + video_join_type + ", app=" + app + ", stream=" + stream
                    + ", streamurl=" + streamurl + ", roomname=" + roomname
                    + ", roomid=" + roomid + ", roomdesc=" + roomdesc + ", ext1="
                    + ext1 + ", ext2=" + ext2 + ", ext3=" + ext3 + ", ext4=" + ext4
                    + ", ext5=" + ext5 + "]";
        }

        public short getMsglen()
        {
            return msglen;
        }

        public void setMsglen(short msglen)
        {
            this.msglen = msglen;
        }

        public short getVideo_type()
        {
            return video_type;
        }

        public void setVideo_type(short video_type)
        {
            this.video_type = video_type;
        }

        public short getVideo_command()
        {
            return video_command;
        }

        public void setVideo_command(short video_command)
        {
            this.video_command = video_command;
        }

        public String getVideo_join_type()
        {
            return video_join_type;
        }

        public void setVideo_join_type(String video_join_type)
        {
            this.video_join_type = video_join_type;
        }

        public String getApp()
        {
            return app;
        }

        public void setApp(String app)
        {
            this.app = app;
        }

        public String getStream()
        {
            return stream;
        }

        public void setStream(String stream)
        {
            this.stream = stream;
        }

        public String getStreamurl()
        {
            return streamurl;
        }

        public void setStreamurl(String streamurl)
        {
            this.streamurl = streamurl;
        }

        public String getRoomname()
        {
            return roomname;
        }

        public void setRoomname(String roomname)
        {
            this.roomname = roomname;
        }

        public int getRoomid()
        {
            return roomid;
        }

        public void setRoomid(int roomid)
        {
            this.roomid = roomid;
        }

        public String getRoomdesc()
        {
            return roomdesc;
        }

        public void setRoomdesc(String roomdesc)
        {
            this.roomdesc = roomdesc;
        }

        public String getExt1()
        {
            return ext1;
        }

        public void setExt1(String ext1)
        {
            this.ext1 = ext1;
        }

        public String getExt2()
        {
            return ext2;
        }

        public void setExt2(String ext2)
        {
            this.ext2 = ext2;
        }

        public String getExt3()
        {
            return ext3;
        }

        public void setExt3(String ext3)
        {
            this.ext3 = ext3;
        }

        public String getExt4()
        {
            return ext4;
        }

        public void setExt4(String ext4)
        {
            this.ext4 = ext4;
        }

        public String getExt5()
        {
            return ext5;
        }

        public void setExt5(String ext5)
        {
            this.ext5 = ext5;
        }


    }

    public class ChatMessage
    {
        private int _id;
        private String msgtype;      //消息类型3种 chat 单聊与群聊，语音对讲
        private String msglength;

        private String groupid;
        private String userid;
        private String charsetname;  //编码类型
        private String msgcontent;
        private String datetime;

        private String filetype;
        private String filekey;      //下载时表示下载url，上传时上传
        private String filepath;     // 文件、图片、语音路径
        private String filename;
        private long   filesize = 0;
        private String fileID;
        private String fileMD5;

        private bool isupload = false;     // 是否已经加载成功
        private bool isdownload = false;   // 是否已经加载成功
        private bool isProgressBL = false; // 是否存在进度条
        private int  percent = 0;          // 进度

        public int currentSize = 0;
        public String state = "";             //消息的状态


        public ChatMessage()
        {
        }

        public ChatMessage(int _id, String msgtype, String msglength,
                String groupid, String userid, String charsetname,
                String msgcontent, String datetime, String filetype,
                String filekey, String filepath, String filename, long filesize,
                String fileID, String fileMD5, bool isupload,
                bool isdownload, bool isProgress, int percent,
                int currentSize, String state)
        {
            this._id = _id;
            this.msgtype = msgtype;
            this.msglength = msglength;
            this.groupid = groupid;
            this.userid = userid;
            this.charsetname = charsetname;
            this.msgcontent = msgcontent;
            this.datetime = datetime;
            this.filetype = filetype;
            this.filekey = filekey;
            this.filepath = filepath;
            this.filename = filename;
            this.filesize = filesize;
            this.fileID = fileID;
            this.fileMD5 = fileMD5;
            this.isupload = isupload;
            this.isdownload = isdownload;
            this.isProgressBL = isProgress;
            this.percent = percent;
            this.currentSize = currentSize;
            this.state = state;
        }

        public String getMsglength()
        {
            return msglength;
        }

        public void setMsglength(String msglength)
        {
            this.msglength = msglength;
        }
        public String getFilekey()
        {
            return filekey;
        }

        public void setFilekey(String filekey)
        {
            this.filekey = filekey;
        }
        public bool isIsupload()
        {
            return isupload;
        }
        public void setIsupload(bool isupload)
        {
            this.isupload = isupload;
        }

        public bool isIsdownload()
        {
            return isdownload;
        }
        public void setIsdownload(bool isdownload)
        {
            this.isdownload = isdownload;
        }

        public String getFileID()
        {
            return fileID;
        }
        public void setFileID(String fileID)
        {
            this.fileID = fileID;
        }
        public String getFileMD5()
        {
            return fileMD5;
        }
        public void setFileMD5(String fileMD5)
        {
            this.fileMD5 = fileMD5;
        }
        public String getFiletype()
        {
            return filetype;
        }

        public void setFiletype(String filetype)
        {
            this.filetype = filetype;
        }

        public int getCurrentSize()
        {
            return currentSize;
        }

        public void setCurrentSize(int currentSize)
        {
            this.currentSize = currentSize;
        }

        public String getState()
        {
            return state;
        }

        public void setState(String state)
        {
            this.state = state;
        }

        public int get_id()
        {
            return _id;
        }

        public void set_id(int _id)
        {
            this._id = _id;
        }

        public int getPercent()
        {
            return percent;
        }

        public void setPercent(int percent)
        {
            this.percent = percent;
        }

        public bool isProgress()
        {
            return isProgressBL;
        }

        public void setProgress(bool isProgress)
        {
            this.isProgressBL = isProgress;
        }

        public String getFilepath()
        {
            return filepath;
        }

        public void setFilepath(String filepath)
        {
            this.filepath = filepath;
        }

        public String getFilename()
        {
            return filename;
        }

        public void setFilename(String filename)
        {
            this.filename = filename;
        }

        public long getFilesize()
        {
            return filesize;
        }

        public void setFilesize(long filesize)
        {
            this.filesize = filesize;
        }

        public String getMsgtype()
        {
            return msgtype;
        }

        public void setMsgtype(String msgtype)
        {
            this.msgtype = msgtype;
        }

        public String getGroupid()
        {
            return groupid;
        }

        public void setGroupid(String groupid)
        {
            this.groupid = groupid;
        }

        public String getUserid()
        {
            return userid;
        }

        public void setUserid(String userid)
        {
            this.userid = userid;
        }

        public String getCharsetname()
        {
            return charsetname;
        }

        public void setCharsetname(String charsetname)
        {
            this.charsetname = charsetname;
        }

        public String getMsgcontent()
        {
            return msgcontent;
        }

        public void setMsgcontent(String msgcontent)
        {
            this.msgcontent = msgcontent;
        }

        public String getDatetime()
        {
            return datetime;
        }

        public void setDatetime(String datetime)
        {
            this.datetime = datetime;
        }

        public String toString()
        {
            return "ChatMessage [_id=" + _id + ", msgtype="
                    + msgtype + ", msglength=" + msglength + ", groupid="
                    + groupid + ", userid=" + userid + ", charsetname="
                    + charsetname + ", msgcontent=" + msgcontent + ", datetime="
                    + datetime + ", filetype=" + filetype + ", filekey="
                    + filekey + ", filepath=" + filepath + ", filename="
                    + filename + ", filesize=" + filesize + ", fileID="
                    + fileID + ", fileMD5=" + fileMD5 + ", isupload="
                    + isupload + ", isdownload=" + isdownload + ", isProgress="
                    + isProgressBL + ", percent=" + percent + ", currentSize="
                    + currentSize + ", state=" + state + "]";
        }
    }


}
