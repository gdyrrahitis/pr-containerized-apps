DECLARE @dbname nvarchar(128)
SET @dbname = N'CatalogDb'

IF (NOT EXISTS (SELECT name 
FROM master.dbo.sysdatabases 
WHERE ('[' + name + ']' = @dbname 
OR name = @dbname)))
BEGIN
	CREATE DATABASE [CatalogDb]
END