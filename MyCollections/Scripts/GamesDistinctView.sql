ALTER VIEW GamesDistinct AS
SELECT
DISTINCT
T1.[Name]
,(SELECT TOP 1 T2.[Cover] FROM [DB_A0A084_mycollection].[dbo].[Game] T2 WHERE T2.[Name] = T1.[Name] AND T2.[Cover] IS NOT NULL) AS Cover
,(SELECT COUNT(T2.[GameID]) FROM [DB_A0A084_mycollection].[dbo].[Game] T2 WHERE T2.[Name] = T1.[Name] AND T2.[Active] = 1) AS Quantity
FROM [DB_A0A084_mycollection].[dbo].[Game] T1
WHERE [Active] = 1