/****** Script de la commande SelectTopNRows à partir de SSMS  ******/
SELECT [NumSerie] as "Numero Serie" 
	  , [Code] 
      ,[NumLot] as "Numero Lot"
      ,[DateGeneration] as "Date Generation"
      ,[DateFinValidite] as "Date Fin Validite"
      --,[IsValid]
      --,[IsActif]
  FROM [TnTMarketDB].[dbo].[PrepaidCards]
  Where NumLot = 2
  order by NumSerie