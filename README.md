# MyCollections

Este projeto refere-se a um sistema para controle de coleções. Inicialmente apenas para jogos, com planos de incluir vídeos e filmes em versões futuras.

Para criar um usuário e montar sua coleção, visite:

[My Collections](http://mycollections.paulorobertoelias.com.br)

## O projeto

A solução consiste em uma sistema em .Net Core 2.1, com suporte a base de dados, através do Entity Framework e um API para criação de telas para Web a aplicações móveis.

### Integração com o Steam

O principal intuito da solução é prover a consolidação da coleção proveniente de diversas lojas. Infelizmente apenas o Steam oferece uma API atualmente, para recuperar sua biblioteca de jogos. Para que a integração com o Steam funcione, é necessário possuir uma 'key' e um 'steamid' do jogador, que você pode inputar na tela de parâmetros.

### Dicas

Para atualizar o banco de dados após alguma alteração

<code>dotnet ef migrations add Initial</code>

<code>dotnet ef database update</code>

Gerar publicação

<code>dotnet publish --configuration Release</code>

Parar a aplicação no servidor para poder subir versão.

Faça upload do arquivo app_offline.htm para a raíz do servidor

# Autor

[Site pessoal](http://paulorobertoelias.com.br)
