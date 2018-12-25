select * from [dbo].[AspNetUsers]
--where id = '3ba1513a-2f5c-4789-a2b5-739fd4adfd8d'
where usertype = 2

select * from [dbo].[UserCategories]
where AspNetUserId = '3ba1513a-2f5c-4789-a2b5-739fd4adfd8d'

----------------------------------
select * from [dbo].[UserImgFiles]

select * from [dbo].[UserCategories]

select * from [dbo].[UsersAddInfos]


-------------------
delete [dbo].[AspNetUserCategories]
where AspNetUserId in ( '79e8c384-4d2a-4bb2-bffb-e2194cc8f024', 'a63e3d56-777f-4c3d-9d71-7fa7ebb66818' )

delete [dbo].[AspNetUsers]
--where id in ( '79e8c384-4d2a-4bb2-bffb-e2194cc8f024' , 'a63e3d56-777f-4c3d-9d71-7fa7ebb66818' )
where usertype = 2
-------------------------------
select r.Name, u.UserName from [dbo].[AspNetUserRoles] ur
inner join  [dbo].[AspNetRoles] r on ur.RoleId = r.Id
inner join [dbo].[AspNetUsers] u on u.Id = ur.UserId 

----------------
select * from [dbo].[Listings]