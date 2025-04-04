del /f /q .\AppCordova\platforms\android\app\build\outputs\apk\debug\*
cd AppCordova
wsl.exe docker run -v .:/opt/src --rm androidbuild cordova build android --debug 
ren .\AppCordova\platforms\android\app\build\outputs\apk\debug\app-debug.apk mycollections.apk
cmd.exe