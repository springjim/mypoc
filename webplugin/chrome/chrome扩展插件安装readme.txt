1、将上面的nginx下的文件放到nginx服务器的html目录下，用 http://localhost:8000/test.html 来调用
2、本地应用的部署
     第1步，将你的consoleApp目录的路径，修改到 writereg.reg 文件中，并保存
     第2步，双击执行writereg.reg
     第3步，修改consoleApp里的com.mypoc.myhost-manifest.json的 path值，指向你的电脑中的 ConsoleApp.exe 绝对路径

3、在google浏览器里，打开开发者模式，加载我的插件demo

4、在电脑上安装一个nginx服务软件，设置好 nginx.conf 文件，将其中port设为8000

5、将nginx目录下的2个文件，拷到 nginx服务器目录下html 目录下

6、浏览器里输入 http://localhost:8000/test.html , 开始测试
