

/****** Object:  Table [dbo].[UserCategories]    Script Date: 27/12/2018 15:07:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserCategories](
	[AspNetUserId] [nvarchar](128) NOT NULL,
	[CategoryID] [int] NOT NULL,
 CONSTRAINT [PK_dbo.UserCategories] PRIMARY KEY CLUSTERED 
(
	[AspNetUserId] ASC,
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[UserCategories]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserCategories_dbo.AspNetUsers_AspNetUserId] FOREIGN KEY([AspNetUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO

ALTER TABLE [dbo].[UserCategories] CHECK CONSTRAINT [FK_dbo.AspNetUserCategories_dbo.AspNetUsers_AspNetUserId]
GO

ALTER TABLE [dbo].[UserCategories]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserCategories_dbo.Categories_CategoryID] FOREIGN KEY([CategoryID])
REFERENCES [dbo].[Categories] ([ID])
GO

ALTER TABLE [dbo].[UserCategories] CHECK CONSTRAINT [FK_dbo.AspNetUserCategories_dbo.Categories_CategoryID]
GO

--------------------------------------------------------

CREATE TABLE [dbo].[UserImgFiles](
	[AspNetUserId] [nvarchar](128) NOT NULL,
	[PictureID] [int] NOT NULL,
	[Ordering] [int] NOT NULL,
 CONSTRAINT [PK_dbo.UserImgFiles] PRIMARY KEY CLUSTERED 
(
	[AspNetUserId] ASC,
	[PictureID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[UserImgFiles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserImgFileS_dbo.AspNetUsers_AspNetUserId] FOREIGN KEY([AspNetUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[UserImgFiles] CHECK CONSTRAINT [FK_dbo.AspNetUserImgFileS_dbo.AspNetUsers_AspNetUserId]
GO

ALTER TABLE [dbo].[UserImgFiles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserImgFileS_dbo.Pictures_PictureID] FOREIGN KEY([PictureID])
REFERENCES [dbo].[Pictures] ([ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[UserImgFiles] CHECK CONSTRAINT [FK_dbo.AspNetUserImgFileS_dbo.Pictures_PictureID]
GO

-------------------------------------


CREATE TABLE [dbo].[UsersAddInfos](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [nvarchar](128) NOT NULL,
	[ProCompany] [nvarchar](max) NULL,
	[ProSiret] [nvarchar](max) NULL,
	[ProAdress] [nvarchar](max) NULL,
	[ProTownZip] [nvarchar](max) NULL,
	[ProPhone] [nvarchar](max) NULL,
	[ProSiteWeb] [nvarchar](max) NULL,
	[ProEmail] [nvarchar](max) NULL,
	[LocationRefID] [int] NOT NULL,
	[ProLongitude] [float] NULL,
	[ProLatitude] [float] NULL,
 CONSTRAINT [PK_dbo.UsersAddInfos] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[UsersAddInfos]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UsersAddInfos_dbo.AspNetUsers_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[UsersAddInfos] CHECK CONSTRAINT [FK_dbo.UsersAddInfos_dbo.AspNetUsers_UserID]
GO

ALTER TABLE [dbo].[UsersAddInfos]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UsersAddInfos_dbo.LocationsRef_LocationRefID] FOREIGN KEY([LocationRefID])
REFERENCES [dbo].[LocationsRef] ([ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[UsersAddInfos] CHECK CONSTRAINT [FK_dbo.UsersAddInfos_dbo.LocationsRef_LocationRefID]
GO

-----------------------------------------------------

ALTER TABLE [dbo].[Listings] ADD [OwnerUserType]  int NOT NULL 

ALTER TABLE [dbo].[Listings] ADD  DEFAULT ((0)) FOR [OwnerUserType]
GO


--------------------------
ALTER TABLE [dbo].[AspNetUsers]  ADD [UserType] int NOT NULL

ALTER TABLE [dbo].[AspNetUsers] ADD  DEFAULT ((0)) FOR [UserType]
GO

-------------------------------
select * from sys.servers
-----------
EXEC sp_addlinkedserver [database.windows.net];
GO
USE tempdb;
GO
CREATE SYNONYM [MokaTest_MigrationHistory] FOR 
    [database.windows.net].[shak29_tntmarket_testdb].dbo.[__MigrationHistory];
GO

insert into [MokaTest_MigrationHistory] 
select * from  TnTMarketDB.[dbo].[__MigrationHistory]

select * from  [MokaTest_MigrationHistory]
----------------

sp_configure 'show advanced options', 1;  
RECONFIGURE;
GO 
sp_configure 'Ad Hoc Distributed Queries', 1;  
RECONFIGURE;  
GO  

insert into OPENDATASOURCE (
        'SQLNCLI'
        ,'Data Source=198.38.83.33;Initial Catalog=Shak29_tntmarket_testdb;User ID=shak29_tnttest2018;Password=TnTtest2018'
        ).Shak29_tntmarket_testdb.dbo.[__MigrationHistory]
select * from  TnTMarketDB.[dbo].[__MigrationHistory]
