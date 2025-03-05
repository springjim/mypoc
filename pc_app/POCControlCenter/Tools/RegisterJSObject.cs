using POCControlCenter.DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace POCControlCenter
{
    public class RegisterJSObject
    {

        private ControlMainForm myMainForm;
       

        public RegisterJSObject(ControlMainForm mainForm)
        {
            myMainForm = mainForm;
        }

        public void JSCallFunction_Fence(string fenceId)
        {
            if (myMainForm != null)
            {
                myMainForm.BeginInvoke(
                  new Action(() =>
                  {

                      myMainForm.JSCallFunction_Fence(fenceId);

                  })

              );
            }


        }

        public void JSCallFunction_Map(string useridlist)
        {
            if (myMainForm != null)
            {
                myMainForm.BeginInvoke(
                  new Action(() =>
                  {

                      myMainForm.JSCallFunction_Map(useridlist);

                  })

              );
            }


        }

        public void JSCallFunction_PersonTrackPlayBack(string userid, string username)
        {
            if (myMainForm != null)
            {
                myMainForm.BeginInvoke(
                  new Action(() =>
                  {

                      myMainForm.JSCallFunction_PersonTrackPlayBack(userid, username);

                  })

              );
            }
               

        }

        public void JSCallFunction_PersonAudioCall(string userid, string username)
        {
            if (myMainForm != null)
            {
                myMainForm.BeginInvoke( 
                    new Action(()=>
                    {
                        
                            myMainForm.JSCallFunction_PersonAudioCall(userid, username);
                        
                    })

                );
            }
                
        }

        public void JSCallFunction_PersonVideoCall(string userid, string username)
        {
            if (myMainForm != null)
            {
                myMainForm.BeginInvoke(
                  new Action(() =>
                  {

                      myMainForm.JSCallFunction_PersonVideoCall(userid, username);

                  })

              );
            }
                
        }


    }
}
