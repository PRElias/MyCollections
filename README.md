# MyCollections

Este projeto refere-se a um sistema para controle de coleções. Inicialmente apenas para jogos, com planos de incluir vídeos e filmes em versões futuras.

Para criar um usuário e montar sua coleção, visite:

[My Collections](http://mycollections.paulorobertoelias.com.br)

## O projeto

A solução consiste em uma sistema em .Net Core 2.1, com suporte a base de dados, através do Entity Framework e um API para criação de telas para Web a aplicações móveis.

### Integração com o Steam

O principal intuito da solução é prover a consolidação da coleção proveniente de diversas lojas. Infelizmente apenas o Steam oferece uma API atualmente, para recuperar sua biblioteca de jogos. Para que a integração com o Steam funcione, é necessário possuir uma 'key' e um 'steamid' do jogador, que você pode inputar na tela de parâmetros.

## Executando e atualizando jogos

Para iniciar o site localmente, apenas inicie o debug no VSCode

## Atualizando pacotes

A forma mais fácil de atualizar é usando a ferramenta automatizada [dotnet outdated](https://github.com/dotnet-outdated/dotnet-outdated)
Depois de instalado, é só executar o comando abaixo:
`dotnet list package --outdated`

## Extensões úteis do VSCode

Beautify JSON. Apenas abra o arquivo docs/games/games.json and acione Ctrl + Alt + M para ajustar ou Ctrl + Alt para minificar

[JSON Tools](https://marketplace.visualstudio.com/items?itemName=eriklynd.json-tools)

## App Cordova

Há também a opção de se gerar um app Cordova usando os mesmos arquivos do PWA/Website. Execute a batch `CopyFilesToCordovaApp.bat` e a cópia de todos os arquivo necessários será executada automaticamente

# Autor

[Site pessoal](http://paulorobertoelias.com.br)

