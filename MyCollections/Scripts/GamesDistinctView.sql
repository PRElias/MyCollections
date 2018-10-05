ALTER VIEW GamesDistinct AS
SELECT
DISTINCT
COALESCE(T1.[FriendlyName], T1.[Name]) AS Name
,(SELECT TOP 1 T2.[Cover] FROM [dbo].[Game] T2 WHERE T2.[FriendlyName] = T1.[FriendlyName] AND T2.[Cover] IS NOT NULL) AS Cover
,(SELECT COUNT(T2.[GameID]) FROM [dbo].[Game] T2 WHERE T2.[FriendlyName] = T1.[FriendlyName] AND T2.[Active] = 1) AS Quantity
,TU.[Email]
FROM [dbo].[Game] T1
INNER JOIN [dbo].[AspNetUsers] TU on T1.[UserId] = TU.[Id]
WHERE T1.[Active] = 1