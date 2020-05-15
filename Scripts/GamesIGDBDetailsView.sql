CREATE VIEW GamesIGDBDetails AS
select 
IGDBId,
SteamApID,
Name,
JSON_VALUE(GameDetails, '$[0].summary') AS Summary,
JSON_VALUE(GameDetails, '$[0].rating') AS Rating,
JSON_VALUE(GameDetails, '$[0].release_dates[0].human') AS ReleaseDate,
JSON_VALUE(GameDetails, '$[0].cover.url') AS Cover,
JSON_VALUE(GameDetails, '$[0].screenshots[0].url') AS Screenshot1,
JSON_VALUE(GameDetails, '$[0].screenshots[1].url') AS Screenshot2,
JSON_VALUE(GameDetails, '$[0].screenshots[2].url') AS Screenshot3
from Game where IGDBId is not null