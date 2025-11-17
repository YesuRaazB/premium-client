IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Occupations] (
    [Code] nvarchar(450) NOT NULL,
    [DisplayName] nvarchar(max) NOT NULL,
    [Rating] nvarchar(max) NOT NULL,
    [Factor] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Occupations] PRIMARY KEY ([Code])
);
GO

CREATE TABLE [Members] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [AgeNextBirthday] int NOT NULL,
    [DateOfBirthMMYYYY] nvarchar(max) NOT NULL,
    [OccupationCode] nvarchar(450) NOT NULL,
    [DeathSumInsured] decimal(18,2) NOT NULL,
    [MonthlyPremium] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Members] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Members_Occupations_OccupationCode] FOREIGN KEY ([OccupationCode]) REFERENCES [Occupations] ([Code]) ON DELETE CASCADE
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Code', N'DisplayName', N'Factor', N'Rating') AND [object_id] = OBJECT_ID(N'[Occupations]'))
    SET IDENTITY_INSERT [Occupations] ON;
INSERT INTO [Occupations] ([Code], [DisplayName], [Factor], [Rating])
VALUES (N'Author', N'Author', 2.25, N'White Collar'),
(N'Cleaner', N'Cleaner', 11.5, N'Light Manual'),
(N'Doctor', N'Doctor', 1.5, N'Professional'),
(N'Farmer', N'Farmer', 31.75, N'Heavy Manual'),
(N'Florist', N'Florist', 11.5, N'Light Manual'),
(N'Mechanic', N'Mechanic', 31.75, N'Heavy Manual'),
(N'Other', N'Other', 31.75, N'Heavy Manual');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Code', N'DisplayName', N'Factor', N'Rating') AND [object_id] = OBJECT_ID(N'[Occupations]'))
    SET IDENTITY_INSERT [Occupations] OFF;
GO

CREATE INDEX [IX_Members_OccupationCode] ON [Members] ([OccupationCode]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251115030245_InitialCreate', N'8.0.0');
GO

COMMIT;
GO

