//Electron 主进程
const { app, BrowserWindow, ipcMain } = require('electron');
const { spawn } = require('child_process');
const path = require('path');

let mainWindow;
let localAppProcess;

// 启动本地应用
function startLocalApp() {
    try {

        console.log(`开始调用本地应用...`);

        localAppProcess = spawn(path.join(__dirname, 'localapp', 'ConsoleApp.exe'), [],  {
        stdio: ['pipe', 'pipe', 'inherit']
      });
  
      localAppProcess.on('exit', (code) => {
        console.log(`本地应用已退出，代码 ${code}`);
      });
  
      localAppProcess.on('error', (err) => {
        console.error('启动本地应用失败:', err);
      });

      // 返回成功状态
      return { success: true, message: '本地应用已启动' };

    } catch (err) {
        console.error('启动本地应用时发生错误:', err);
        return { success: false, message: err.message };
    }
  }

// 处理与本地应用的消息交互
function sendMessageToLocalApp(message, callback) {
  if (!localAppProcess) {
    callback({ error: '本地应用未启动' });
    return;
  }

  const jsonStr = JSON.stringify(message);
  const jsonBuffer = Buffer.from(jsonStr, 'utf8');
  const lengthBuffer = Buffer.alloc(4);
  lengthBuffer.writeUInt32LE(jsonBuffer.length, 0);

  // 发送消息：4字节长度 + JSON内容
  localAppProcess.stdin.write(Buffer.concat([lengthBuffer, jsonBuffer]));

  // 接收响应
  localAppProcess.stdout.once('data', (data) => {
    const responseLength = data.readUInt32LE(0);
    const responseJson = data.slice(4, 4 + responseLength).toString('utf8');
    callback(JSON.parse(responseJson));
  });
}

// 创建窗口
function createWindow() {
  mainWindow = new BrowserWindow({
    width: 800,
    height: 600,
    webPreferences: {
      preload: path.join(__dirname, 'preload.js'),
      contextIsolation: true,
      enableRemoteModule: false
    }
  });

  mainWindow.loadFile('test.html');
}

// 启动应用
app.whenReady().then(() => {
  //startLocalApp();  //放到test.html 中按钮调用
  createWindow();

  app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) createWindow();
  });
});

// 退出应用
app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') app.quit();
});

// 监听渲染进程的 IPC 消息
ipcMain.handle('connect-native-app', async () => {
    return startLocalApp(); // 调用 startLocalApp 并返回结果
  });

// 处理前端请求
ipcMain.handle('send-message', async (event, message) => {
  return new Promise((resolve) => {
    console.log("发送消息:"+ JSON.stringify(message));
    sendMessageToLocalApp(message, resolve);
  });
});