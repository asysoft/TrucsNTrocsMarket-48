/****** Script de la commande SelectTopNRows à partir de SSMS  ******/
use [TnTMarketDB]

--------------------
SELECT *  FROM [dbo].[ListingTypes]
--SELECT *  FROM [dbo].[Listings]

SELECT *  FROM [dbo].[Categories]
SELECT *  FROM [dbo].[CategoryListingTypes]

---------------------------------------- CREER ds SETTINGS
/*
USE [shak29_tntmarket_testdb]
GO

INSERT INTO [dbo].[Settings]
           ([ID]
           ,[Name]
           ,[Description]
           ,[Slogan]
           ,[SearchPlaceHolder]
           ,[EmailContact]
           ,[Version]
           ,[Currency]
           ,[TransactionFeePercent]
           ,[TransactionMinimumSize]
           ,[TransactionMinimumFee]
           ,[SmtpHost]
           ,[SmtpPort]
           ,[SmtpUserName]
           ,[SmtpPassword]
           ,[SmtpSSL]
           ,[EmailAddress]
           ,[EmailDisplayName]
           ,[AgreementRequired]
           ,[AgreementLabel]
           ,[AgreementText]
           ,[SignupText]
           ,[EmailConfirmedRequired]
           ,[Theme]
           ,[DateFormat]
           ,[TimeFormat]
           ,[ListingReviewEnabled]
           ,[ListingReviewMaxPerDay]
           ,[Created]
           ,[LastUpdated])
     VALUES
           (1	
		   Trucs N Trocs	Le SIte des achats et ventes de biens et services à l'Ile Maurice	Trucs N Trocs : Le Marché	Que cherchez vous?	asysoft@yahoo.com	1.0	Rps	10	20	30	NULL	NULL	NULL	NULL	0	NULL	NULL	1	Accepter les termes et conditions ....	Accepter les termes et conditions du site pour l achat la vente et l utilisation de ....	NULL	1	Default	dd/mm/yyyy	hh:mm:ss	1	2	2018-09-28 00:00:00.000	2018-10-03 10:06:39.027
		   )
GO
*/



----------   RESET TOUT

--
delete [dbo].[CategoryListingTypes]
DBCC CHECKIDENT ('[dbo].[CategoryListingTypes]', RESEED, 0)
GO

--
delete [dbo].[Categories]
DBCC CHECKIDENT ('[dbo].[Categories]', RESEED, 0)
GO

--
delete [dbo].[ListingTypes]
DBCC CHECKIDENT ('[dbo].[ListingTypes]', RESEED, 0)
GO

--------------------------------------  INIT DES TYPES D ANNONCES
-- Var Global des compteur et IDs
Declare @idOffre int
Declare @idDemande int

Declare @idCateg int 
Declare @idParent int 
Declare @numOrd int 

INSERT INTO [dbo].[ListingTypes]
([Name] ,[ButtonLabel] ,[PriceEnabled] ,[PriceUnitLabel] ,[OrderTypeID] ,[OrderTypeLabel] ,[PaymentEnabled]  ,[ShippingEnabled])
VALUES  ('Offre','Offre Btn',1,'par jour',0,'OrderTypeLabel',1,1)
SELECT @idOffre = @@IDENTITY

INSERT INTO [dbo].[ListingTypes]
([Name] ,[ButtonLabel] ,[PriceEnabled] ,[PriceUnitLabel] ,[OrderTypeID] ,[OrderTypeLabel] ,[PaymentEnabled]  ,[ShippingEnabled])
VALUES  ('Demande','Demande Btn',1,'par jour',0,'OrderTypeLabel',1,1)
SELECT @idDemande = @@IDENTITY

----------------------  INIT DES CATEGORIES ET DE LEUR SOUS CATEGORIES

---- EMPLOI
--------------- Créee CATEGORIE Parent
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('EMPLOI','Offres d''emploi', 0, 1, 0)
SELECT @idParent = @@IDENTITY

-- Atache Parent a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idParent , @idOffre)
--INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idParent , @idDemande)

SET @numOrd = 0
-- Crée les fils (Premier)
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Offres d''emploi','Offres d''emploi', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
--INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

-------------------------------------
----        VEHICULES
-------------------------------------
--------------- Créee CATEGORIE Parent
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('VEHICULES','Achats-Ventes de Vehicules', 0, 1, 0)
SELECT @idParent = @@IDENTITY

-- Atache Parent a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idParent , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idParent , @idDemande)

SET @numOrd = 0
--** Crée les fils ( Premier)
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Voitures','Achats-Ventes de Voitures', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Motos','Achats-Ventes de Motos', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Caravanes','Achats-Ventes de Caravanes', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Utilitaires','Achats-Ventes de Utilitaires', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Equipements Auto','Achats-Ventes Equipements Auto', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Equipements Moto','Achats-Ventes Equipements Moto', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Equipements Caravanes','Achats-Ventes Equipements Caravanes', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Nautismes','Achats-Ventes de Nautismes', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Equipements Nautismes','Achats-Ventes Equipements Nautismes', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)


-------------------------------------
----        IMMOBILIER
-------------------------------------
--------------- Créee CATEGORIE Parent
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('IMMOBILIER','Ventes immobilieres', 0, 1, 0)
SELECT @idParent = @@IDENTITY

-- Atache Parent a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idParent , @idOffre)
--INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idParent , @idDemande)

SET @numOrd = 0
--** Crée les fils ( Premier)
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Ventes immobilieres','Ventes immobilieres', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
--INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Locations','Locations', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
--INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Colocations','Colocations', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
--INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Bureaux & Commerces','Bureaux & Commerces', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
--INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

-------------------------------------
----        VACANCES
-------------------------------------
--------------- Créee CATEGORIE Parent
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('VACANCES','VACANCES', 0, 1, 0)
SELECT @idParent = @@IDENTITY

-- Attache Parent a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idParent , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idParent , @idDemande)

SET @numOrd = 0
--** Crée les fils ( Premier)
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Locations & Gites','Locations & Gites', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Chambres d''hotes','Chambres d''hotes', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Campings','Campings', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Hotels','Hotels', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Hebergements insolites','Hebergements insolites', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

-------------------------------------
----        MULTIMEDIA
-------------------------------------
--------------- Créee CATEGORIE Parent
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('MULTIMEDIA','MULTIMEDIA', 0, 1, 0)
SELECT @idParent = @@IDENTITY

-- Attache Parent a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idParent , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idParent , @idDemande)

SET @numOrd = 0
--** Crée les fils ( Premier)
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Informatique','Informatique', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Consoles & Jeux video','Consoles & Jeux video', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Image & Son','Image & Son', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Telephonie','Telephonie', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

-------------------------------------
----        MAISON
-------------------------------------
--------------- Créee CATEGORIE Parent
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('MAISON','MAISON', 0, 1, 0)
SELECT @idParent = @@IDENTITY

-- Attache Parent a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idParent , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idParent , @idDemande)

SET @numOrd = 0
--** Crée les fils ( Premier)
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Ameublement','Ameublement', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Electromenager','Electromenager', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Arts de la table','Arts de la table', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Decoration','Decoration', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Arts de la table','Arts de la table', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Decoration','Decoration', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Linge de maison','Linge de maison', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Bricolage','Bricolage', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Jardinage','Jardinage', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Vetements','Vetements', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Chaussures','Chaussures', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Accessoires & Bagagerie','Accessoires & Bagagerie', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Montres & Bijoux','Montres & Bijoux', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Equipement bebe','Equipement bebe', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Vetements bebe','Vetements bebe', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)


-------------------------------------
----        LOISIRS
-------------------------------------
--------------- Créee CATEGORIE Parent
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('LOISIRS','LOISIRS', 0, 1, 0)
SELECT @idParent = @@IDENTITY

-- Attache Parent a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idParent , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idParent , @idDemande)

SET @numOrd = 0
--** Crée les fils ( Premier)
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('DVD / Films','DVD / Films', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('CD / Musique','CD / Musique', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Livres','Livres', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Animaux','Animaux', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Velos','Velos', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Sports & Hobbies','Sports & Hobbies', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Instruments de musique','Instruments de musique', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Collection','Collection', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Jeux & Jouets','Jeux & Jouets', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Vins & Gastronomie','Vins & Gastronomie', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)


-------------------------------------
----        MATERIEL PROFESSIONNEL
-------------------------------------
--------------- Créee CATEGORIE Parent
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('MATERIEL PROFESSIONNEL','MATERIEL PROFESSIONNEL', 0, 1, 0)
SELECT @idParent = @@IDENTITY

-- Attache Parent a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idParent , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idParent , @idDemande)

SET @numOrd = 0
--** Crée les fils ( Premier)
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Materiel Agricole','Materiel Agricole', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Transport & Manutention','Transport & Manutention', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('BTP - Chantier Gros-oeuvre','BTP - Chantier Gros-oeuvre', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Outillage - Materiaux 2nd-oeuvre','Outillage - Materiaux 2nd-oeuvre', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Equipements Industriels','Equipements Industriels', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Restauration - Hotellerie','Restauration - Hotellerie', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Fournitures de Bureau','Fournitures de Bureau', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Commerces & Marches','Commerces & Marches', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Materiel Medical','Materiel Medical', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)



-------------------------------------
----        SERVICES
-------------------------------------
--------------- Créee CATEGORIE Parent
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('SERVICES','SERVICES', 0, 1, 0)
SELECT @idParent = @@IDENTITY

-- Attache Parent a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idParent , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idParent , @idDemande)

SET @numOrd = 0
--** Crée les fils ( Premier)
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Prestations de services','Prestations de services', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Billetterie','Billetterie', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Evenements','Evenements', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Cours particuliers','Cours particuliers', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)

--** Crée les fils ( Les autres)
SET @numOrd = @numOrd + 1
INSERT INTO [dbo].[Categories] ([Name] ,[Description] ,[Parent] ,[Enabled] ,[Ordering]) VALUES ('Covoiturage','Covoiturage', @idParent, 1, @numOrd)
SELECT @idCateg = @@IDENTITY
-- Atache Fils a Offre ou demande
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idOffre)
INSERT INTO [dbo].[CategoryListingTypes]  ([CategoryID] , [ListingTypeID])VALUES (@idCateg , @idDemande)


GO
--------------- VERIFIER
SELECT *  FROM [dbo].[ListingTypes]
--SELECT *  FROM [dbo].[Listings]

SELECT *  FROM [dbo].[Categories]
SELECT *  FROM [dbo].[CategoryListingTypes]
GO



