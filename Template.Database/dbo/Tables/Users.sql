CREATE TABLE [dbo].[Users] (
    [Id]         UNIQUEIDENTIFIER CONSTRAINT [DF_User_Id] DEFAULT (newid()) NOT NULL,
    [Email]      NVARCHAR (50)    NOT NULL,
    [FirstName]  NVARCHAR (50)    NOT NULL,
    [MiddleName] NVARCHAR (50)    NULL,
    [LastName]   NVARCHAR (50)    NOT NULL,
    [IsActive]   BIT              NOT NULL,
    [CreatedOn]  DATETIME         NOT NULL,
    [CreatedBy]  UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn] DATETIME         NULL,
    [ModifiedBy] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC)
);

