/****** Script de la commande SelectTopNRows à partir de SSMS  ******/

--use [TnTMarketDB]
--use Shak29_tntmarket_testdb

SELECT *  FROM [dbo].[ListingTypes]
--SELECT *  FROM [dbo].[Listings]


SELECT *  FROM [dbo].[Categories]
where ID in (23,52)

SELECT *  FROM [dbo].[CategoryListingTypes]

SELECT *  FROM [dbo].[Settings]

select loc.ID as locID, loc.Name as locName, loc.Description as locDesc, cat.ID as CatID, cat.Name as catName, cat.Description as catDesc, lis.* from [dbo].[Listings] lis
inner join [dbo].[LocationsRef] loc on lis.LocationRefID = loc.id 
inner join [dbo].[Categories] cat   on lis.CategoryID = cat.ID

where ID in (167,312,13)


----------
