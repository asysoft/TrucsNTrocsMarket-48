
  update [TnTMarketDB].[dbo].[Listings]
  set	--IsOccasion = 1 ,
		--StockDispo = 1 ,
		[ListingRef] =  convert(varchar(4), year(getdate())) 
						+ convert(char(2),getdate(),1)
						+ FORMAT(ID, '100000')

