/***************************************************************************
This script assumes you have already created blank database somewhere. 

To create instance of LocalDB:
1. Open Server Explorer in Visual Studio 2012.
2. Right click and Add Connection.
3. Choose Microsoft SQL Server Database File (SqlClient).
4. Specify file path for mdf file (typically App_Data folder for ASP.Net MVC).
5. Follow rest of the steps as instructed.
6. Replace USE [CCBlog] with USE [C:\MyPath\MyDb.mdf] below.
7. You must run InsertDefaultMetaData.sql after below script.

TIP: To open SQL LocalDB in SQL Server Management Studio, just connect to "(LocalDb)\v11.0".

*****************************************************************************/

USE [CCBlog] /* if you are using SQL LocalDB replace CCBlog with full file path of mdf file */
GO

/****** Object:  Schema [CCBlog]    Script Date: 12/23/2012 12:46:11 AM ******/
CREATE SCHEMA [CCBlog]
GO
/****** Object:  Table [CCBlog].[Post]    Script Date: 12/23/2012 12:46:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [CCBlog].[Post](
	[PostId] [int] IDENTITY(1,1) NOT NULL,
	[UrlFriendlyName] [nvarchar](128) NOT NULL,
	[Title] [nvarchar](4000) NULL,
	[Abstract] [nvarchar](4000) NULL,
	[Body] [nvarchar](max) NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[ModifyDate] [datetimeoffset](7) NOT NULL,
	[AuthorCreateDate] [datetimeoffset](7) NOT NULL,
	[AuthorModifyDate] [datetimeoffset](7) NOT NULL,
	[CreatedByUserId] [int] NOT NULL,
	[ModifiedByUserId] [int] NOT NULL,
	[RevisionCount] [int] NOT NULL,
	[TagIdCsv] [varchar](4000) NULL,
	[SeriesId] [int] NULL,
	[SeriesPart] [int] NULL,
	[DisplayOrder] [int] NULL,
	[PageLayoutName] [nvarchar](255) NULL,
	[StatusId] [int] NOT NULL,
 CONSTRAINT [PK_Post] PRIMARY KEY CLUSTERED 
(
	[PostId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [CCBlog].[PostTag]    Script Date: 12/23/2012 12:46:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CCBlog].[PostTag](
	[PostId] [int] NOT NULL,
	[TagId] [int] NOT NULL,
 CONSTRAINT [PK_PostTag] PRIMARY KEY CLUSTERED 
(
	[PostId] ASC,
	[TagId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [CCBlog].[Role]    Script Date: 12/23/2012 12:46:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CCBlog].[Role](
	[RoleId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK__Role__8AFACE1A84935F27] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [CCBlog].[Series]    Script Date: 12/23/2012 12:46:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CCBlog].[Series](
	[SeriesId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Title] [nvarchar](4000) NULL,
	[Abstract] [nvarchar](max) NULL,
	[StatusId] [int] NOT NULL,
 CONSTRAINT [PK_Series] PRIMARY KEY CLUSTERED 
(
	[SeriesId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [CCBlog].[Status]    Script Date: 12/23/2012 12:46:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CCBlog].[Status](
	[StatusId] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Status] PRIMARY KEY CLUSTERED 
(
	[StatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [CCBlog].[Tag]    Script Date: 12/23/2012 12:46:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CCBlog].[Tag](
	[TagId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Title] [nvarchar](4000) NULL,
	[HelpText] [nvarchar](4000) NULL,
	[IsVisible] [bit] NOT NULL,
	[IsNavigation] [bit] NOT NULL,
	[DisplayOrder] [int] NULL,
 CONSTRAINT [PK_Tag] PRIMARY KEY CLUSTERED 
(
	[TagId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [CCBlog].[User]    Script Date: 12/23/2012 12:46:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CCBlog].[User](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[ClaimedIdentifier] [nvarchar](1024) NOT NULL,
	[FullName] [nvarchar](1024) NULL,
	[Email] [nvarchar](1024) NULL,
	[Nickname] [nvarchar](1024) NULL,
	[IsAuthor] [bit] NOT NULL,
	[RoleId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Index [IX_PostDisplaySort]    Script Date: 12/23/2012 12:46:11 AM ******/
CREATE NONCLUSTERED INDEX [IX_PostDisplaySort] ON [CCBlog].[Post]
(
	[DisplayOrder] ASC,
	[AuthorCreateDate] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UX_Tag]    Script Date: 12/23/2012 12:46:11 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UX_Tag] ON [CCBlog].[Tag]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [CCBlog].[Post] ADD  CONSTRAINT [DF_Post_CreateDate]  DEFAULT (sysdatetimeoffset()) FOR [CreateDate]
GO
ALTER TABLE [CCBlog].[Post] ADD  CONSTRAINT [DF_Post_ModifyDate]  DEFAULT (sysdatetimeoffset()) FOR [ModifyDate]
GO
ALTER TABLE [CCBlog].[Post] ADD  CONSTRAINT [DF_Table_1_CreateDate1]  DEFAULT (sysdatetimeoffset()) FOR [AuthorCreateDate]
GO
ALTER TABLE [CCBlog].[Post] ADD  CONSTRAINT [DF_Table_1_ModifyDate1]  DEFAULT (sysdatetimeoffset()) FOR [AuthorModifyDate]
GO
ALTER TABLE [CCBlog].[Post]  WITH CHECK ADD  CONSTRAINT [FK_Post_Series] FOREIGN KEY([SeriesId])
REFERENCES [CCBlog].[Series] ([SeriesId])
GO
ALTER TABLE [CCBlog].[Post] CHECK CONSTRAINT [FK_Post_Series]
GO
ALTER TABLE [CCBlog].[Post]  WITH CHECK ADD  CONSTRAINT [FK_Post_Status] FOREIGN KEY([StatusId])
REFERENCES [CCBlog].[Status] ([StatusId])
GO
ALTER TABLE [CCBlog].[Post] CHECK CONSTRAINT [FK_Post_Status]
GO
ALTER TABLE [CCBlog].[Post]  WITH CHECK ADD  CONSTRAINT [FK_Post_User_CreatedBy] FOREIGN KEY([CreatedByUserId])
REFERENCES [CCBlog].[User] ([UserId])
GO
ALTER TABLE [CCBlog].[Post] CHECK CONSTRAINT [FK_Post_User_CreatedBy]
GO
ALTER TABLE [CCBlog].[Post]  WITH CHECK ADD  CONSTRAINT [FK_Post_User_ModifiedBy] FOREIGN KEY([ModifiedByUserId])
REFERENCES [CCBlog].[User] ([UserId])
GO
ALTER TABLE [CCBlog].[Post] CHECK CONSTRAINT [FK_Post_User_ModifiedBy]
GO
ALTER TABLE [CCBlog].[PostTag]  WITH CHECK ADD  CONSTRAINT [FK_PostTag_Post] FOREIGN KEY([PostId])
REFERENCES [CCBlog].[Post] ([PostId])
GO
ALTER TABLE [CCBlog].[PostTag] CHECK CONSTRAINT [FK_PostTag_Post]
GO
ALTER TABLE [CCBlog].[PostTag]  WITH CHECK ADD  CONSTRAINT [FK_PostTag_Tag] FOREIGN KEY([TagId])
REFERENCES [CCBlog].[Tag] ([TagId])
GO
ALTER TABLE [CCBlog].[PostTag] CHECK CONSTRAINT [FK_PostTag_Tag]
GO
ALTER TABLE [CCBlog].[Series]  WITH CHECK ADD  CONSTRAINT [FK_Series_Status] FOREIGN KEY([StatusId])
REFERENCES [CCBlog].[Status] ([StatusId])
GO
ALTER TABLE [CCBlog].[Series] CHECK CONSTRAINT [FK_Series_Status]
GO
ALTER TABLE [CCBlog].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Role] FOREIGN KEY([RoleId])
REFERENCES [CCBlog].[Role] ([RoleId])
GO
ALTER TABLE [CCBlog].[User] CHECK CONSTRAINT [FK_User_Role]
GO
