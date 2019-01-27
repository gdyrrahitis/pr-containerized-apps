-- Drop Primary key
IF OBJECT_ID('[dbo].[PK_Catalog]', 'PK') IS NOT NULL 
BEGIN
	ALTER TABLE [dbo].[Catalog] DROP CONSTRAINT PK_Catalog
END
GO

-- Drop Unique key constraint 
IF OBJECT_ID('dbo.[CK_ConstraintName]', 'UQ') IS NOT NULL
BEGIN
	ALTER TABLE [dbo].[Catalog] DROP CONSTRAINT CIX_Catalog
END
GO

-- Drop table
IF EXISTS (
	(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='Catalog')
)
BEGIN
	DROP TABLE [dbo].[Catalog];
END
GO

CREATE TABLE [dbo].[Catalog] (
	Id int identity(1,1) not null,
	CatalogItemId uniqueidentifier not null,
	Name varchar(100) not null,
	Price money not null
);
GO

ALTER TABLE [dbo].[Catalog]
ADD CONSTRAINT PK_Catalog
PRIMARY KEY NONCLUSTERED (CatalogItemId)
CREATE UNIQUE CLUSTERED INDEX CIX_Catalog ON [dbo].[Catalog](Id)
GO