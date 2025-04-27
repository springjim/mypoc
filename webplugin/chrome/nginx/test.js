document.addEventListener('DOMContentLoaded', () => {

  //��һ���������ӱ���Ӧ��

  document.getElementById('connectNativeApp').addEventListener('click', () => {
    // ������Ϣ�����ݽű� content.js �У�window.postMessageһ��������Chrome ��չ�����ݽű�������Ϣ
	  

    const messageObject = {  
        messageType : "CONNECT_NATIVE_APP"        
     };   

    window.postMessage(
      { 
		type: "CONNECT_NATIVE_APP", 	 
		message: JSON.stringify(messageObject)		 
	  },
      "*"   //*��ʾ���޴���
    );
  });


//����̨��¼
  document.getElementById('sendMessageLoginPlatform').addEventListener('click', () => {
    // ������Ϣ�����ݽű� content.js �У�window.postMessageһ��������Chrome ��չ�����ݽű�������Ϣ
	    
    const userId = document.getElementById('userId').value.trim();
	// ����Ƿ�Ϊ�ջ���6λ
     if (userId === '' || userId.length !== 6 || isNaN(userId)) {
       // ��ʾ������ʾ
	    alert('������6λ�ĶԽ��ʺ�');
		return;
      }

	 

    const messageObject = {  
        messageType : "TYPE_LOGIN_PLATFORM",
        messageId : "abc1232afdsfsdfd",
		messageDesc: "�ϱ�����̨��¼",
		userId: userId
     };   

    window.postMessage(
      { 
		type: "SEND_TO_NATIVE_APP", 	 
		message: JSON.stringify(messageObject)		 
	  },
      "*"   //*��ʾ���޴���
    );
  });

  
  //�ϱ������Խ���
  document.getElementById('sendMessageWorkGroupId').addEventListener('click', () => {
    // ������Ϣ�����ݽű� content.js �У�window.postMessageһ��������Chrome ��չ�����ݽű�������Ϣ

	 
    const groupId = document.getElementById('groupId').value.trim();
	// ����Ƿ�Ϊ�ջ���6λ
     if (groupId === '' || isNaN(groupId)) {
       // ��ʾ������ʾ
	    alert('��������ȷ�ĶԽ���ID��');
		return;
      }


    const userId = document.getElementById('userId').value.trim();
	// ����Ƿ�Ϊ�ջ���6λ
     if (userId === '' || userId.length !== 6 || isNaN(userId)) {
       // ��ʾ������ʾ
	    alert('������6λ�ĶԽ��ʺ�');
		return;
      }

    const messageObject = {  
        messageType : "TCP_REPORT",
		messageId : "abc1232afdsfsdfd",
		messageDesc: "�ϱ�����������",
		groupId: groupId,
		userId: userId 
     };   

    window.postMessage(
      { 
		type: "SEND_TO_NATIVE_APP", 	 
		message: JSON.stringify(messageObject)		 
	  },
      "*"   //*��ʾ���޴���
    );
  });


//����
  document.getElementById('sendMessageApplyMic').addEventListener('click', () => {
    // ������Ϣ�����ݽű� content.js �У�window.postMessageһ��������Chrome ��չ�����ݽű�������Ϣ	 
    const messageObject = {  
        messageType : "TCP_APPLY_MIC",
        messageId : "abc1232afdsfsdfd",
		messageDesc: "�´���������"		
     };

    window.postMessage(
      { 
		type: "SEND_TO_NATIVE_APP", 		 
		message: JSON.stringify(messageObject)	
	    },
      "*"   //*��ʾ���޴���
    );
  });


//�ͷ���
  document.getElementById('sendMessageReleasMic').addEventListener('click', () => {
    // ������Ϣ�����ݽű� content.js �У�window.postMessageһ��������Chrome ��չ�����ݽű�������Ϣ	 
    const messageObject = {  
        messageType : "TCP_RELEASE_MIC",
        messageId : "abc1232afdsfsdfd",
		messageDesc: "�´��ͷ�������"		
     };

    window.postMessage(
      { type: "SEND_TO_NATIVE_APP", 		 
		message: JSON.stringify(messageObject)		
	    },
      "*"   //*��ʾ���޴���
    );
  });


//����̨�ǳ�
  document.getElementById('sendMessageLogout').addEventListener('click', () => {
    // ������Ϣ�����ݽű� content.js �У�window.postMessageһ��������Chrome ��չ�����ݽű�������Ϣ	
	
	const groupId = document.getElementById('groupId').value.trim();
	// ����Ƿ�Ϊ�ջ���6λ
     if (groupId === '' || isNaN(groupId)) {
       // ��ʾ������ʾ
	    alert('��������ȷ�ĶԽ���ID��');
		return;
      }


    const userId = document.getElementById('userId').value.trim();
	// ����Ƿ�Ϊ�ջ���6λ
     if (userId === '' || userId.length !== 6 || isNaN(userId)) {
       // ��ʾ������ʾ
	    alert('������6λ�ĶԽ��ʺ�');
		return;
      }


    const messageObject = {  
        messageType : "TYPE_LOGOUT",
        messageId : "abc1232afdsfsdfd",
		messageDesc: "�´����̨�ǳ�����",
		groupId: groupId,
		userId: userId 
			
     };

    window.postMessage(
      { type: "SEND_TO_NATIVE_APP", 		 
		message: JSON.stringify(messageObject)		
	    },
      "*"   //*��ʾ���޴���
    );
  });


  // �����������ݽű�����Ϣ
  window.addEventListener("message", (event) => {
    if (event.data.type === "FROM_NATIVE_APP") {
      console.log(event.data);
      document.getElementById('response').innerText = JSON.stringify(event.data.message);
    }
  });


});