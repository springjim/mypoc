// listen message from service_worker.js 
chrome.runtime.onMessage.addListener((request, sender, sendResponse) => {
  if (request.type === "FROM_NATIVE_APP") {
    //to user front JS
    window.postMessage({type: "FROM_NATIVE_APP", message: request.message}, "*");
  }
});


window.addEventListener("message", (event) => {

  if (event.data.type === "CONNECT_NATIVE_APP"){
    //send service_worker.js
	chrome.runtime.sendMessage(
      {type: "CONNECT_TO_NATIVE_APP", message: event.data.message},
      (response) => {
        if (chrome.runtime.lastError) {
          console.log("Error:chrome.runtime.lastError");
        } else {
          console.log("Received response:", response);
        }
      }
    );

	
  } else if (event.data.type === "SEND_TO_NATIVE_APP") {
    //send service_worker.js
    chrome.runtime.sendMessage(
      {type: "SEND_TO_NATIVE_APP", message: event.data.message},
      (response) => {
        if (chrome.runtime.lastError) {
          console.log("Error:chrome.runtime.lastError");
        } else {
          console.log("Received response:", response);
        }
      }
    );
  }
});