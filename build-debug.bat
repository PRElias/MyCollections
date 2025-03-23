del /f /q .\platforms\android\app\build\outputs\bundle\release\*
cd AppCordova
wsl.exe docker run -v .:/opt/src --rm androidbuild cordova build android --debug 
ren .\platforms\android\app\build\outputs\bundle\release\app-debug.apk mycollections.apk
cmd.exe