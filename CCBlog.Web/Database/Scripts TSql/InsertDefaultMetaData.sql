--Valid status values
INSERT CCBlog.[Status] (StatusId, [Name]) VALUES (0, 'Inactive / Private')
INSERT CCBlog.[Status] (StatusId, [Name]) VALUES (1, 'Active / Published')
INSERT CCBlog.[Status] (StatusId, [Name]) VALUES (2, 'In-works / Draft')


--Create roles
SET IDENTITY_INSERT IdentityTable ON

INSERT CCBlog.Role(RoleId, [Name]) VALUES (1, 'Administrator')
INSERT CCBlog.Role(RoleId, [Name]) VALUES (2, 'Author')

SET IDENTITY_INSERT IdentityTable OFF