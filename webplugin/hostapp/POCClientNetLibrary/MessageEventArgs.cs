using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCClientNetLibrary
{
    public class ServerEventArgs : EventArgs
    {
        public ManageDataPacket manage { get; set; }
        public ChatMessage      chat   { get; set; }
        public VideoMessage     video { get; set; }

        public MonitorAudioMessage monitorAudio { get; set; }
      

        public AVChatNewMessage avchat_new { get; set; }

        public int calltype  { get; set; }
        public int callstate { get; set; }

        public ServerEventArgs( int itype, int istate )
        {
            calltype  = itype;
            callstate = istate;
        }

        public ServerEventArgs( ManageDataPacket manage)
        {
            this.manage = manage;
        }

        public ServerEventArgs(ChatMessage chat )
        {
            this.chat = chat;
        }

        public ServerEventArgs(MonitorAudioMessage monitorAudio)
        {
            this.monitorAudio = monitorAudio;
        }

        public ServerEventArgs(VideoMessage video)
        {
            this.video = video;
        }

        public ServerEventArgs(AVChatNewMessage chat)
        {
            this.avchat_new = chat;
        }

        public ServerEventArgs()
        {
        }

    }


    public class FileEventArgs : EventArgs
    {
        public string   Key        { get; set; }
        public string   FileName   { get; set; }
        public int      length     { get; set; }
        public Boolean  complete   { get; set; }

        public FileEventArgs( string key, string fileName )
        {
            Key  = key;
            FileName = fileName;
        }
    }


}
