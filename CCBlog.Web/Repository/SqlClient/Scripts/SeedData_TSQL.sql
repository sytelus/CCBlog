IF NOT EXISTS (SELECT TOP 1 1 FROM [ccb].[Role])
BEGIN
	EXECUTE [ccb].[AddRole] 
	   @Name = "Administrator"
	  ,@IsAdmin = 1
	  ,@Comment = 'Created by seed data generation'
END