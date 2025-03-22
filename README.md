# MyCollections

Este projeto refere-se a um sistema para controle de coleções. Inicialmente apenas para jogos, com planos de incluir vídeos e filmes em versões futuras.

Ele foi concebido em uma era pré-inteligência artificial, então toda concepção da arquitetura e o código foram escritos manualmente (ao menos até a presente data em marcço de 2025) como uma forma de eu estudar as tecnologias envolvidas.

Minha ideia para criá-lo surgiu da minha dificuldade em saber quais jogos eu tenho, tamanha a quantidade. Infelizmente apenas a Steam fornece uma API para que eu possa recuperar os jogos de forma automática, então eu construí como parte integrante do projeto, um site para que eu possa incluir jogos manualmente também.

![png](https://github.com/PRElias/MyCollections/blob/master/readme-images-01.png)

# Índice
* [O Projeto](#O-projeto)
* [Funcionamento Básico](#Funcionamento-básico)
* [App Cordova](#app-cordova)
* [Integração com o Steam](#Integração-com-o-Steam)
* [Informações diversas](#Informações-diversas)

## O projeto

A solução consiste em uma sistema em .Net 9.0 para o site e manutenção dos registros em si, porém, o site da exibição da coleção é a parte e consiste apenas em HTML, CSS e Javascript e foi concebido desta forma para poder ser servido a partir do Github Pages (você pode conferir o conteúdo clicando [aqui](https://mycollections.paulorobertoelias.com.br/)), como um PWA ou mesmo ter todos os arquivos copiados para dentro de um app Cordova, também disponível.

## Funcionamento básico

Ao executar a aplicação .Net um website local pode ser acessado onde você pode fazer a busca automática do Steam e também adicionar ou editar jogos. Todas essas informações são salvas em um arquivo JSON e respectivas imagens diretamente no diretório local do projeto, que são então salvos no repositório do GitHub e posteriormente copiados para dentro do app Cordova.

## App Cordova

Há também a opção de se gerar um app Cordova usando os mesmos arquivos do PWA/Website. Execute a batch `CopyFilesToCordovaApp.bat` e a cópia de todos os arquivo necessários será executada automaticamente. Depois, é só compilar o app usando a batch `build-debug.bat` e copiar o APK para o celular.

O APK não é assinado com nenhum certificado e não foi pensado para ser disponibilizado na loja do Google, já que ele roda offline, sem nenhuma integração, sendo todos os dados gerados e manipulados de forma desconectada usando a aplicação .Net

## Integração com o Steam

O principal intuito da solução é prover a consolidação da coleção proveniente de diversas lojas. Infelizmente apenas o Steam oferece uma API atualmente, para recuperar sua biblioteca de jogos. Para que a integração com o Steam funcione, é necessário possuir uma 'key' e um 'steamid' do jogador, que você pode inputar na tela de parâmetros.

## Informações diversas

### Atualizando pacotes .Net

Quando o projeto surgiu, ele utilizada .Net Core 2.1, mas agora o mesmo foi atualizado para a última versão disponível, .Net 9.

A forma mais fácil de atualizar é usando a ferramenta automatizada [dotnet outdated](https://github.com/dotnet-outdated/dotnet-outdated)
Depois de instalado, é só executar o comando abaixo:
`dotnet list package --outdated`

### Extensões úteis do VSCode

Beautify JSON. Apenas abra o arquivo docs/games/games.json and acione Ctrl + Alt + M para ajustar ou Ctrl + Alt para minificar

[JSON Tools](https://marketplace.visualstudio.com/items?itemName=eriklynd.json-tools)

# Autor

[Site pessoal](http://paulorobertoelias.com.br)

