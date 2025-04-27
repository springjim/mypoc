document.addEventListener('DOMContentLoaded', () => {

  //第一步必须连接本地应用

  document.getElementById('connectNativeApp').addEventListener('click', () => {
    // 发送消息到内容脚本 content.js 中，window.postMessage一般用于向Chrome 扩展的内容脚本发送消息
	  

    const messageObject = {  
        messageType : "CONNECT_NATIVE_APP"        
     };   

    window.postMessage(
      { 
		type: "CONNECT_NATIVE_APP", 	 
		message: JSON.stringify(messageObject)		 
	  },
      "*"   //*表示不限窗口
    );
  });


//调度台登录
  document.getElementById('sendMessageLoginPlatform').addEventListener('click', () => {
    // 发送消息到内容脚本 content.js 中，window.postMessage一般用于向Chrome 扩展的内容脚本发送消息
	    
    const userId = document.getElementById('userId').value.trim();
	// 检查是否为空或不满6位
     if (userId === '' || userId.length !== 6 || isNaN(userId)) {
       // 显示错误提示
	    alert('请输入6位的对讲帐号');
		return;
      }

	 

    const messageObject = {  
        messageType : "TYPE_LOGIN_PLATFORM",
        messageId : "abc1232afdsfsdfd",
		messageDesc: "上报调度台登录",
		userId: userId
     };   

    window.postMessage(
      { 
		type: "SEND_TO_NATIVE_APP", 	 
		message: JSON.stringify(messageObject)		 
	  },
      "*"   //*表示不限窗口
    );
  });

  
  //上报工作对讲组
  document.getElementById('sendMessageWorkGroupId').addEventListener('click', () => {
    // 发送消息到内容脚本 content.js 中，window.postMessage一般用于向Chrome 扩展的内容脚本发送消息

	 
    const groupId = document.getElementById('groupId').value.trim();
	// 检查是否为空或不满6位
     if (groupId === '' || isNaN(groupId)) {
       // 显示错误提示
	    alert('请输入正确的对讲组ID号');
		return;
      }


    const userId = document.getElementById('userId').value.trim();
	// 检查是否为空或不满6位
     if (userId === '' || userId.length !== 6 || isNaN(userId)) {
       // 显示错误提示
	    alert('请输入6位的对讲帐号');
		return;
      }

    const messageObject = {  
        messageType : "TCP_REPORT",
		messageId : "abc1232afdsfsdfd",
		messageDesc: "上报工作组信令",
		groupId: groupId,
		userId: userId 
     };   

    window.postMessage(
      { 
		type: "SEND_TO_NATIVE_APP", 	 
		message: JSON.stringify(messageObject)		 
	  },
      "*"   //*表示不限窗口
    );
  });


//抢麦
  document.getElementById('sendMessageApplyMic').addEventListener('click', () => {
    // 发送消息到内容脚本 content.js 中，window.postMessage一般用于向Chrome 扩展的内容脚本发送消息	 
    const messageObject = {  
        messageType : "TCP_APPLY_MIC",
        messageId : "abc1232afdsfsdfd",
		messageDesc: "下达抢麦信令"		
     };

    window.postMessage(
      { 
		type: "SEND_TO_NATIVE_APP", 		 
		message: JSON.stringify(messageObject)	
	    },
      "*"   //*表示不限窗口
    );
  });


//释放麦
  document.getElementById('sendMessageReleasMic').addEventListener('click', () => {
    // 发送消息到内容脚本 content.js 中，window.postMessage一般用于向Chrome 扩展的内容脚本发送消息	 
    const messageObject = {  
        messageType : "TCP_RELEASE_MIC",
        messageId : "abc1232afdsfsdfd",
		messageDesc: "下达释放麦信令"		
     };

    window.postMessage(
      { type: "SEND_TO_NATIVE_APP", 		 
		message: JSON.stringify(messageObject)		
	    },
      "*"   //*表示不限窗口
    );
  });


//调度台登出
  document.getElementById('sendMessageLogout').addEventListener('click', () => {
    // 发送消息到内容脚本 content.js 中，window.postMessage一般用于向Chrome 扩展的内容脚本发送消息	
	
	const groupId = document.getElementById('groupId').value.trim();
	// 检查是否为空或不满6位
     if (groupId === '' || isNaN(groupId)) {
       // 显示错误提示
	    alert('请输入正确的对讲组ID号');
		return;
      }


    const userId = document.getElementById('userId').value.trim();
	// 检查是否为空或不满6位
     if (userId === '' || userId.length !== 6 || isNaN(userId)) {
       // 显示错误提示
	    alert('请输入6位的对讲帐号');
		return;
      }


    const messageObject = {  
        messageType : "TYPE_LOGOUT",
        messageId : "abc1232afdsfsdfd",
		messageDesc: "下达调度台登出信令",
		groupId: groupId,
		userId: userId 
			
     };

    window.postMessage(
      { type: "SEND_TO_NATIVE_APP", 		 
		message: JSON.stringify(messageObject)		
	    },
      "*"   //*表示不限窗口
    );
  });


  // 监听来自内容脚本的消息
  window.addEventListener("message", (event) => {
    if (event.data.type === "FROM_NATIVE_APP") {
      console.log(event.data);
      document.getElementById('response').innerText = JSON.stringify(event.data.message);
    }
  });


});