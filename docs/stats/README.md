# Estatísticas da coleção

Esta pasta contém uma página estática gerada a partir do arquivo `GametimeSummary.csv`, localizado na raiz do projeto. Este arquivo foi gerado a partir da cópia manual dos dados à partir da funcionalidade de "game collection" dos sites trueachiements de XBOX, Playstation e Steam.

## Arquivos

- `index.html`: estrutura da página estática.
- `styles.css`: estilos visuais da página.
- `app.js`: dados embutidos do CSV e lógica da tabela/gráfico.

## Como regenerar

Quando `GametimeSummary.csv` for atualizado, regenere `docs/stats/app.js` a partir dele mantendo estas regras:

1. Ler o CSV usando `;` como separador.
2. Ignorar linhas repetidas de cabeçalho no meio do arquivo, por exemplo linhas em que `Title` seja `Title` ou `Platform` seja `Platform`.
3. Converter `Time played` para minutos numéricos em uma propriedade `minutes`.
   - Exemplos: `342 hrs 42 mins` vira `20562`; `35 mins` vira `35`; vazio vira `0`.
4. Manter o texto original de `Time played` para exibição, mas ordenar a coluna de tempo usando `minutes`.
5. Converter `%age` para número decimal na propriedade `percent`; valores inválidos ou vazios devem virar `null`.
6. Quando `Achievements` estiver no formato `x / y`, preencher `unlocked` e `totalAchievements`; outros formatos ficam como `null` nesses campos.
7. No gráfico Top 10, agrupar jogos por título normalizado e somar o tempo entre plataformas.
   - Remover sufixos de plataforma no fim do título, como `(PS4)`, `(PS3)`, `(PS3/Vita)`, `(Xbox)`, `(PC)` e `(Steam)`.
   - Remover símbolos como `™`, `®` e `©` antes da comparação.
8. O painel de gráfico deve ter duas abas no mesmo container:
   - `Mais tempo jogado`: Top 10 geral, agrupado por jogo.
   - `Mais tempo jogado (sem infantis)`: Top 10 com a mesma regra de agrupamento, mas ignorando estes jogos: `Minecraft`, `Minecraft Dungeons`, `Roblox`, `Jurassic World Evolution`, `Jurassic World Evolution 2` e `Jurassic World Evolution 3`.
9. O gráfico deve continuar mostrando a divisão por plataforma na própria barra empilhada e na legenda.
10. A tabela deve permitir ordenação por todas as colunas, busca textual e filtro por plataforma.
11. Se os arquivos forem usados pelo PWA, atualizar também o `CACHE_NAME` em `docs/service-worker.js`.

## Data exibida

A página exibe manualmente a mensagem `Atualizada em 07 de setembro de 2026`. Ao regenerar com um CSV novo, atualizar esse texto em `index.html` para a data correspondente.

