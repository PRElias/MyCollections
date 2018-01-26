# MyCollections

Este projeto refere-se a um sistema para controle de coleções. Inicialmente apenas para jogos, com planos de incluir vídeos e filmes em versões futuras.

## O projeto

A solução consiste em uma API em .Net Core 2.0, com suporte a base de dados, através do Entity Framework e não apresenta nesta versão uma interface visual para manutenção dos dados, sendo necessário manipular diretamente as tabelas.

### Integração com o Steam

O principal intuito da solução é prover a consolidação da coleção proveniente de diversas lojas. Infelizmente apenas o Steam oferece uma API atualmente, para recuperar sua biblioteca de jogos. Para que a integração com o Steam funcione, é necessário possuir uma 'key' e um 'steamid', que você pode preencer no banco através dos scripts abaixo:

<code>update [dbo].[Param] set [value] = SUACHAVEAQUI where [key] = 'steam-key'</code>

<code>update [dbo].[Param] set [value] = SUACHAVEAQUI where [key] = 'steam-steamid'</code>

### PWA - Aplicativo móvel

Ainda em fase inicial, esse Website, por seguir muitos dos padrões propostos no PWA (Progressive Web Apps) pode ser "instalado" em dispositivos mobile e se conecta a API para expor os resultados

# Autor

[Site pessoal](http://paulorobertoelias.com.br)
