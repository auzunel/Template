CREATE TABLE [dbo].[UserMembership] (
    [Id]           INT              IDENTITY (1, 1) NOT NULL,
    [UserId]       UNIQUEIDENTIFIER NOT NULL,
    [PasswordHash] NVARCHAR (90)    NOT NULL,
    [PasswordSalt] NVARCHAR (50)    NOT NULL,
    [IsLocked]     BIT              NOT NULL,
    [WrongAttempt] INT              DEFAULT ((0)) NOT NULL,
    [CreatedOn]    DATETIME         NOT NULL,
    [CreatedBy]    UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn]   DATETIME         NULL,
    [ModifiedBy]   UNIQUEIDENTIFIER NULL,
    [Timestamp]    ROWVERSION       NOT NULL,
    CONSTRAINT [PK_UserManagement] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserManagement_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);

