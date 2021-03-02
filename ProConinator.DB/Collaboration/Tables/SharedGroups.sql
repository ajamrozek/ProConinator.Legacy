CREATE TABLE [dbo].[SharedGroups]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [OwnerId] NVARCHAR(128) NOT NULL, 
    [GroupId] UNIQUEIDENTIFIER NOT NULL, 
    [CollaboratorId] NVARCHAR(128) NOT NULL, 
    CONSTRAINT [FK_SharedGroups_ToAspNetUsers_Owner] FOREIGN KEY (OwnerId) REFERENCES dbo.AspNetUsers(Id),
	CONSTRAINT [FK_SharedGroups_ToAspNetUsers_Collaborator] FOREIGN KEY (CollaboratorId) REFERENCES dbo.AspNetUsers(Id)
)
