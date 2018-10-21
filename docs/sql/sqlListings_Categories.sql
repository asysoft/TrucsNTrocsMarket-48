/****** Script de la commande SelectTopNRows à partir de SSMS  ******/

--use [TnTMarketDB]
--use Shak29_tntmarket_testdb

SELECT *  FROM [dbo].[ListingTypes]
--SELECT *  FROM [dbo].[Listings]


SELECT *  FROM [dbo].[Categories]
SELECT *  FROM [dbo].[CategoryListingTypes]

SELECT *  FROM [dbo].[Settings]

select * from [dbo].[LocationsRef]
where ID = 43


----------
