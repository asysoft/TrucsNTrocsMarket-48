USE [TnTMarketDB]
GO

INSERT INTO [dbo].[PubLocations]
   ([PageName] ,[TopFileName] ,[TopIsFree] ,[TopFileNum] ,[LeftFileName] ,[LeftIsFree] ,[LeftFileNum] ,[RightFileName] ,[RightIsFree]  ,[RightFileNum] ,[LeftAspNetUser_Id]  ,[RightAspNetUser_Id] ,[TopAspNetUser_Id])
VALUES
    ('Index- Page accueil','pub_Index_Top.png',1,1,'pub_Index_Left.png',1,1,'pub_Index_Right.png',1,1,null,null,null)
GO


