//Electron 预加载脚本
const { contextBridge, ipcRenderer } = require('electron');

contextBridge.exposeInMainWorld('electronAPI', {

  sendMessage: (message) => ipcRenderer.invoke('send-message', message),
  connectNativeApp: () => ipcRenderer.invoke('connect-native-app')
  
});