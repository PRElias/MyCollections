@echo off
setlocal

:: Define os caminhos relativos
set "ORIGEM=docs"
set "DESTINO=AppCordova\www"

:: Cria a pasta de destino se não existir
if not exist "%DESTINO%" mkdir "%DESTINO%"

:: Copia todos os arquivos e subpastas, exceto os arquivos especificados
xcopy "%ORIGEM%\*" "%DESTINO%\" /E /I /Y /EXCLUDE:excecoes-copia.txt

echo Arquivos copiados com sucesso!
endlocal
