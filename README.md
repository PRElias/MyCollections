# MyCollections

Este projeto refere-se a um sistema para controle de coleções. Inicialmente apenas para jogos, com planos de incluir vídeos e filmes em versões futuras.

Minha ideia para criá-lo surgiu da minha dificuldade em saber quais jogos eu tenho, tamanha a quantidade. Infelizmente apenas a Steam fornece uma API para que eu possa recuperar os jogos de forma automática, então eu constui como parte integrante do projeto, um site para que eu possa incluir jogos manualmente também.

## O projeto

A solução consiste em uma sistema em .Net 9.0 para o site e manutenção dos registros em si, porém, o site da exibição da coleção é a parte e consiste apenas em HTML, CSS e Javascript e foi concebido desta forma para poder ser servido a partir do Github Pages, como um PWA ou mesmo ter todos os arquivos copiados para dentro de um app Cordova, também disponível

### Integração com o Steam

O principal intuito da solução é prover a consolidação da coleção proveniente de diversas lojas. Infelizmente apenas o Steam oferece uma API atualmente, para recuperar sua biblioteca de jogos. Para que a integração com o Steam funcione, é necessário possuir uma 'key' e um 'steamid' do jogador, que você pode inputar na tela de parâmetros.

## Executando e atualizando jogos

Para iniciar o site localmente, apenas inicie o debug no VSCode ou no Visual Studio.

## Atualizando pacotes .Net

A forma mais fácil de atualizar é usando a ferramenta automatizada [dotnet outdated](https://github.com/dotnet-outdated/dotnet-outdated)
Depois de instalado, é só executar o comando abaixo:
`dotnet list package --outdated`

## Extensões úteis do VSCode

Beautify JSON. Apenas abra o arquivo docs/games/games.json and acione Ctrl + Alt + M para ajustar ou Ctrl + Alt para minificar

[JSON Tools](https://marketplace.visualstudio.com/items?itemName=eriklynd.json-tools)

## App Cordova

Há também a opção de se gerar um app Cordova usando os mesmos arquivos do PWA/Website. Execute a batch `CopyFilesToCordovaApp.bat` e a cópia de todos os arquivo necessários será executada automaticamente. Depois, é só compilar o app usando a batch `build-debug.bat` e copiar o APK para o celular.

O APK não é assinado com nenhum certificado e não foi pensado para ser disponibilizado na loja do Google, já que ele roda offline, sem nenhuma integração, sendo todos os dados gerados e manipulados de forma desconectada usando a aplicação .Net

# Autor

[Site pessoal](http://paulorobertoelias.com.br)

