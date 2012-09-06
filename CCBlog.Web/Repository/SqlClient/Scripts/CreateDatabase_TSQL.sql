USE [master]
GO
/****** Object:  Database [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF]    Script Date: 9/3/2012 3:31:39 AM ******/
CREATE DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'CCBlogData', FILENAME = N'C:\GitHubSrc\CCBlog\CCBlog.Web\App_Data\CCBlogData.mdf' , SIZE = 3136KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'CCBlogData_log', FILENAME = N'C:\GitHubSrc\CCBlog\CCBlog.Web\App_Data\CCBlogData_log.ldf' , SIZE = 832KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET ARITHABORT OFF 
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET AUTO_SHRINK ON 
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET  ENABLE_BROKER 
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET  MULTI_USER 
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET DB_CHAINING OFF 
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF]
GO
/****** Object:  Schema [ccb]    Script Date: 9/3/2012 3:31:39 AM ******/
CREATE SCHEMA [ccb]
GO
/****** Object:  StoredProcedure [ccb].[AddRole]    Script Date: 9/3/2012 3:31:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Name
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [ccb].[AddRole] 
		   @Name nvarchar(256),
           @IsAdmin bit,
           @Comment nvarchar(4000),
		   @RoleId int = NULL OUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO [ccb].[Role]
           (
		   [Name],
           [IsAdmin],
           [Comment]
           --[CreateDate],	--Use defaults
           --[CreatedBy],
           --[ModifiedDate],
           --[ModifiedBy],
		   )
     VALUES
           (
		   @Name,
           @IsAdmin,
           @Comment
		   )

	SELECT @RoleId = SCOPE_IDENTITY()
END

GO
/****** Object:  StoredProcedure [ccb].[AddUser]    Script Date: 9/3/2012 3:31:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Name
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [ccb].[AddUser] 
		   @ClaimedIdentifier nvarchar(1024),
           @FullName nvarchar(1024),
           @Email nvarchar(1024),
           @IsLoggedIn bit,
           @Comment nvarchar(4000),
		   @RoleID int = NULL,
		   @UserId int OUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO [ccb].[User]
           (
		   [ClaimedIdentifier],
           [FullName],
           [Email],
           --[CreateDate],	--Use defaults
           --[CreatedBy],
           --[ModifiedDate],
           --[ModifiedBy],
           [LastLoginDate],
           [Comment]
		   )
     VALUES
           (
		   @ClaimedIdentifier,
           @FullName,
           @Email,
           IIF(@IsLoggedIn = 0, NULL, getutcdate()),
           @Comment
		   )

	SELECT @UserId = SCOPE_IDENTITY()

	IF @RoleID IS NOT NULL
	BEGIN
		EXEC ccb.AddUserRole @UserId, @RoleID
	END
END

GO
/****** Object:  StoredProcedure [ccb].[AddUserRole]    Script Date: 9/3/2012 3:31:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Name
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [ccb].[AddUserRole] 
		   @UserId int,
           @RoleId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO [ccb].[UserRole]
           (
		   [UserId],
           [RoleId]
           --[CreateDate],	--Use defaults
           --[CreatedBy],
           --[ModifiedDate],
           --[ModifiedBy],
		   )
     VALUES
           (
		   @UserId,
           @RoleId
		   )
END


GO
/****** Object:  StoredProcedure [ccb].[LoginUser]    Script Date: 9/3/2012 3:31:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Name
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [ccb].[LoginUser] 
		   @ClaimedIdentifier nvarchar(1024),
		   @CreateUserIfNotExists bit = 1,
           @FullName nvarchar(1024) = NULL,
           @Email nvarchar(1024) = NULL,
		   @UserId int OUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT @UserID = Id
	From ccb.[User]
	WHERE ClaimedIdentifier = @ClaimedIdentifier

	IF @UserId IS NULL
	BEGIN
		IF @CreateUserIfNotExists = 1
			EXECUTE [ccb].[AddUser] 
			   @ClaimedIdentifier = @ClaimedIdentifier
			  ,@FullName = @FullName
			  ,@Email = @Email
			  ,@IsLoggedIn = 1
			  ,@Comment = 'Autocreated'
			  ,@UserId = @UserId OUTPUT
		ELSE
			RAISERROR(N'Cannot login user with ClaimedIdentifier "%s" because this user was not found', 16, 127, @ClaimedIdentifier)
	END
	ELSE
	BEGIN
		UPDATE u
		SET
			   [FullName] = ISNULL(@FullName, u.FullName),
			   [Email] = ISNULL(@Email, u.Email),
			   [ModifiedDate] = IIF(@FullName IS NULL AND @Email IS NULL, u.ModifiedDate, GETUTCDATE()),
			   [ModifiedBy] = ORIGINAL_LOGIN(),
			   [LastLoginDate] = GETUTCDATE()
		FROM [ccb].[User] AS u
		WHERE u.Id = @UserID;
	END
END


GO
/****** Object:  Table [ccb].[Role]    Script Date: 9/3/2012 3:31:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ccb].[Role](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[IsAdmin] [bit] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[CreatedBy] [nvarchar](256) NOT NULL,
	[ModifiedDate] [datetimeoffset](7) NOT NULL,
	[ModifiedBy] [nvarchar](256) NOT NULL,
	[Comment] [nvarchar](4000) NULL,
 CONSTRAINT [PK__Role__3214EC078F3C7A46] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [ccb].[User]    Script Date: 9/3/2012 3:31:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ccb].[User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClaimedIdentifier] [nvarchar](1024) NOT NULL,
	[FullName] [nvarchar](1024) NULL,
	[Email] [nvarchar](1024) NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[CreatedBy] [nvarchar](256) NOT NULL,
	[ModifiedDate] [datetimeoffset](7) NOT NULL,
	[ModifiedBy] [nvarchar](256) NOT NULL,
	[LastLoginDate] [datetimeoffset](7) NULL,
	[Comment] [nvarchar](4000) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserRole]    Script Date: 9/3/2012 3:31:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRole](
	[UserId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[CreatedBy] [nvarchar](256) NOT NULL,
	[ModifiedDate] [datetimeoffset](7) NOT NULL,
	[ModifiedBy] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET IDENTITY_INSERT [ccb].[Role] ON 

INSERT [ccb].[Role] ([Id], [Name], [IsAdmin], [CreateDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [Comment]) VALUES (1, N'Administrator', 1, CAST(0x07D0518786571C360B0000 AS DateTimeOffset), N'REDMOND\shitals', CAST(0x07D0518786571C360B0000 AS DateTimeOffset), N'REDMOND\shitals', N'Created by seed data generation')
SET IDENTITY_INSERT [ccb].[Role] OFF
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ__User__5A4AFD15629890D5]    Script Date: 9/3/2012 3:31:39 AM ******/
ALTER TABLE [ccb].[User] ADD UNIQUE NONCLUSTERED 
(
	[ClaimedIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [ccb].[Role] ADD  CONSTRAINT [DF__Role__CreateDate__173876EA]  DEFAULT (getutcdate()) FOR [CreateDate]
GO
ALTER TABLE [ccb].[Role] ADD  CONSTRAINT [DF__Role__CreatedBy__182C9B23]  DEFAULT (original_login()) FOR [CreatedBy]
GO
ALTER TABLE [ccb].[Role] ADD  CONSTRAINT [DF__Role__ModifiedDa__1920BF5C]  DEFAULT (getutcdate()) FOR [ModifiedDate]
GO
ALTER TABLE [ccb].[Role] ADD  CONSTRAINT [DF__Role__ModifiedBy__1A14E395]  DEFAULT (original_login()) FOR [ModifiedBy]
GO
ALTER TABLE [ccb].[User] ADD  DEFAULT (getutcdate()) FOR [CreateDate]
GO
ALTER TABLE [ccb].[User] ADD  DEFAULT (original_login()) FOR [CreatedBy]
GO
ALTER TABLE [ccb].[User] ADD  DEFAULT (getutcdate()) FOR [ModifiedDate]
GO
ALTER TABLE [ccb].[User] ADD  DEFAULT (original_login()) FOR [ModifiedBy]
GO
ALTER TABLE [dbo].[UserRole] ADD  DEFAULT (getutcdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[UserRole] ADD  DEFAULT (original_login()) FOR [CreatedBy]
GO
ALTER TABLE [dbo].[UserRole] ADD  DEFAULT (getutcdate()) FOR [ModifiedDate]
GO
ALTER TABLE [dbo].[UserRole] ADD  DEFAULT (original_login()) FOR [ModifiedBy]
GO
ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD  CONSTRAINT [FK_UserRole_Role] FOREIGN KEY([RoleId])
REFERENCES [ccb].[Role] ([Id])
GO
ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_UserRole_Role]
GO
ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD  CONSTRAINT [FK_UserRole_User] FOREIGN KEY([UserId])
REFERENCES [ccb].[User] ([Id])
GO
ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_UserRole_User]
GO
USE [master]
GO
ALTER DATABASE [C:\GITHUBSRC\CCBLOG\CCBLOG.WEB\APP_DATA\CCBLOGDATA.MDF] SET  READ_WRITE 
GO
