CREATE TABLE [Users] (
    [UserId] int NOT NULL IDENTITY,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [IsAdmin] bit NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([UserId])
);
GO


CREATE TABLE [Reports] (
    [ReportId] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [ReportType] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [ReportDate] datetime2 NOT NULL,
    [Location] nvarchar(max) NOT NULL,
    [Status] int NOT NULL,
    [FilePath] nvarchar(max) NULL,
    [AdminNotes] nvarchar(max) NULL,
    [LastUpdated] datetime2 NULL,
    [AssignedAdminId] int NULL,
    CONSTRAINT [PK_Reports] PRIMARY KEY ([ReportId]),
    CONSTRAINT [FK_Reports_Users_AssignedAdminId] FOREIGN KEY ([AssignedAdminId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Reports_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
);
GO


CREATE INDEX [IX_Reports_AssignedAdminId] ON [Reports] ([AssignedAdminId]);
GO


CREATE INDEX [IX_Reports_UserId] ON [Reports] ([UserId]);
GO


