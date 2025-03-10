
--=========================================================================================================================================
--------------------------------------------------     CollinderCatalog_Temporarily      --------------------------------------------------
--=========================================================================================================================================





IF OBJECT_ID('dbo.CollinderCatalog_Temporarily', 'U') IS NOT NULL
BEGIN
	TRUNCATE TABLE CollinderCatalog_Temporarily;
END
ELSE 
BEGIN
	CREATE TABLE CollinderCatalog_Temporarily (
		[Id] int NOT NULL IDENTITY,
		[Namber_name] nvarchar(10) NOT NULL,
		[NameOtherCat] nvarchar(40) NULL,
		[Constellation] nvarchar(5) NULL,
		[Right_ascension] nvarchar(20) NULL,
		[Declination] nvarchar(20) NULL,
		[App_Mag] nvarchar(10) NULL,
		[CountStars] nvarchar(10) NULL,
		[Ang_Diameter] nvarchar(10) NULL,
		[Class] nvarchar(10) NULL,
		[Comment] nvarchar(max) NULL,
		CONSTRAINT [PK_CollinderCatalog_temporarily] PRIMARY KEY ([Id])
	);
END



