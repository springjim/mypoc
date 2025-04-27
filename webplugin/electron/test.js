document.getElementById('connectNativeApp').addEventListener('click', async () => {

    try {
        // 调用 Electron 主进程的 connectNativeApp 方法
        const response = await window.electronAPI.connectNativeApp();
    
        // 显示响应结果
        document.getElementById('response').innerText = JSON.stringify(response);
      } catch (err) {
        console.error('连接本地应用失败:', err);
        document.getElementById('response').innerText = `错误: ${err.message}`;
      }

  });

  
  document.getElementById('sendMessageLoginPlatform').addEventListener('click', async () => {
    const userId = document.getElementById('userId').value;
    const response = await window.electronAPI.sendMessage({ messageType: 'TYPE_LOGIN_PLATFORM', 
        messageId: "uuid", messageDesc: "上报调度台登录",
        userId : userId });
    document.getElementById('response').innerText = JSON.stringify(response);
  });

  
  document.getElementById('sendMessageWorkGroupId').addEventListener('click', async () => {

    const userId = document.getElementById('userId').value;
    const groupId = document.getElementById('groupId').value;
    const response = await window.electronAPI.sendMessage({ messageType: 'TCP_REPORT',
        messageId: "uuid", messageDesc: "上报工作组信令", groupId :groupId, userId :userId   });
    document.getElementById('response').innerText = JSON.stringify(response);

  });

  
  document.getElementById('sendMessageApplyMic').addEventListener('click', async () => {

    const response = await window.electronAPI.sendMessage({ messageType: 'TCP_APPLY_MIC',
        messageId: "uuid", messageDesc: "下达抢麦信令"  });
    document.getElementById('response').innerText = JSON.stringify(response);

  });
  
  document.getElementById('sendMessageReleasMic').addEventListener('click', async () => {

    const response = await window.electronAPI.sendMessage({ messageType: 'TCP_RELEASE_MIC',
        messageId: "uuid", messageDesc: "下达释放麦信令"
     });
    document.getElementById('response').innerText = JSON.stringify(response);

  });
  

  document.getElementById('sendMessageLogout').addEventListener('click', async () => {

    const userId = document.getElementById('userId').value;
    const groupId = document.getElementById('groupId').value;

    const response = await window.electronAPI.sendMessage({ messageType: 'TYPE_LOGOUT',
        messageId: "uuid", messageDesc: "下达调度台登出信令", groupId :groupId, userId :userId
     });
    document.getElementById('response').innerText = JSON.stringify(response);

  });