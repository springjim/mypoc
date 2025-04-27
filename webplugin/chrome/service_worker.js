let port = null;
let pendingMessage = null;

function connectToNativeApp() {

  console.log("connetiong native app");
  port = chrome.runtime.connectNative('com.mypoc.myhost');

  if (chrome.runtime.lastError)
  {
     console.error(chrome.runtime.lastError.message);
  }

  console.log("port=",port);
  
  port.onMessage.addListener((response) => {

    console.log("Received from native app:", response);

    chrome.tabs.query({active: true, currentWindow: true}, function(tabs) {
	  if (tabs && tabs.length > 0)
	  {
		  chrome.tabs.sendMessage(tabs[0].id, {type: "FROM_NATIVE_APP", message: response});
	  }	else {
		  console.log("No active tab found. Storing message for later.");
          pendingMessage = response;
	  }
	  });      

  });


  chrome.tabs.onUpdated.addListener((tabId, changeInfo, tab) => {
  if (changeInfo.status === 'complete' && pendingMessage) {
    chrome.tabs.sendMessage(tabId, {type: "FROM_NATIVE_APP", message: pendingMessage});
    pendingMessage = null; // 清空暂存的消息
  }
  });


  port.onDisconnect.addListener(() => {
    console.log("Disconnected from native app");
    port = null;
  });
}

// donot set auto connect
//chrome.runtime.onInstalled.addListener(() => {
//  connectToNativeApp();
//});

function disconnectFromNativeApp() {
  if (port) {
    port.disconnect();
    port = null;
    console.log("Disconnected from native app.");
  } else {
    console.log("No active connection to disconnect.");
  }
}


chrome.runtime.onMessage.addListener((request, sender, sendResponse) => {
  if (request.type === "CONNECT_TO_NATIVE_APP")
  {
     connectToNativeApp();
	 sendResponse({ status: "connected" });  //sender to content.js
  } 
  else if (request.type === "DISCONNECT_FROM_NATIVE_APP")
  {
      disconnectFromNativeApp();
      sendResponse({ status: "disconnected" });  //sender to content.js
  }
  else if (request.type === "SEND_TO_NATIVE_APP") {
    if (port) {
      port.postMessage(request.message);
      console.log("Sent to native app:", request.message);
    } else {
      console.log("Not connected to native app");
    }
  }



});


