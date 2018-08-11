CREATE VIEW GamesDetails AS
select 
G.FriendlyName AS Game,
G.StoreID,
S.Name AS Store,
S.Logo AS StoreLogo,
G.SystemID,
C.Name As system,
C.Logo As SystemLogo
from Game G
inner join Store S on G.StoreID = S.StoreID
inner join System C on G.SystemID = C.SystemID
and Active = 1