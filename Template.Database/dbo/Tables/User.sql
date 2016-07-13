CREATE TABLE [dbo].[User] (
    [Id]         UNIQUEIDENTIFIER    ROWGUIDCOL   NOT NULL,
    [Email]      NVARCHAR (50) NOT NULL,
    [FirstName]  NVARCHAR (50) NOT NULL,
    [MiddleName] NVARCHAR (50) NULL,
    [LastName]   NVARCHAR (50) NOT NULL,
	[IsActive] NCHAR(10) NULL,
    [CreatedOn]  DATETIME      NOT NULL,
    [CreatedBy]  UNIQUEIDENTIFIER           NOT NULL,
    [ModifiedOn] DATETIME      NULL,
    [ModifiedBy] UNIQUEIDENTIFIER           NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC)
);

