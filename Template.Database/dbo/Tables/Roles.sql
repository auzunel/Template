CREATE TABLE [dbo].[Roles] (
    [Id]         INT              IDENTITY (1, 1) NOT NULL,
    [RoleName]   NVARCHAR (50)    NOT NULL,
    [CreatedOn]  DATETIME         NOT NULL,
    [CreatedBy]  UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn] DATETIME         NULL,
    [ModifiedBy] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([Id] ASC)
);

