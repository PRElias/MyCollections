del /f /q .\platforms\android\app\build\outputs\bundle\release\*
wsl.exe docker run -v .:/opt/src --rm androidbuild cordova build android --release  --keystore=.\gatec.keystore --storePassword=gatec2013 --alias=gatec --password=gatec2013
java -jar .\bundletool-all-1.16.0.jar build-apks --mode=universal --bundle=.\platforms\android\app\build\outputs\bundle\release\app-release.aab --output=.\platforms\android\app\build\outputs\bundle\release\app-release.apks --ks=.\gatec.keystore --ks-pass=pass:gatec2013 --ks-key-alias=gatec
ren .\platforms\android\app\build\outputs\bundle\release\app-release.apks app-release.zip
tar -xvf ".\platforms\android\app\build\outputs\bundle\release\app-release.zip" -C ".\platforms\android\app\build\outputs\bundle\release"
for /f "delims=" %%a in (version.txt) do set "versao=%%a" 
ren .\platforms\android\app\build\outputs\bundle\release\universal.apk GATEC_AQP_%versao%.apk
cmd.exe