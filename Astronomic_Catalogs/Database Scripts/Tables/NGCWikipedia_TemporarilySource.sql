
--=========================================================================================================================================
------------------------------------------------     NGCWikipedia_TemporarilySource      ------------------------------------------------
--=========================================================================================================================================





IF OBJECT_ID('dbo.NGCWikipedia_TemporarilySource', 'U') IS NOT NULL
BEGIN
	IF OBJECT_ID('dbo.NGCWikipedia_ExtensionTemporarilySource', 'U') IS NOT NULL
	BEGIN
		DROP TABLE NGCWikipedia_ExtensionTemporarilySource;
	END

	TRUNCATE TABLE NGCWikipedia_TemporarilySource;
END
ELSE 
BEGIN
	CREATE TABLE NGCWikipedia_TemporarilySource (
		NGC_number int Primary Key not null,
		Ohter_names varchar(400) default null,
		Object_type varchar(60) default null,
		Cnstellation varchar(20) not null,
		Right_ascension_J2000 varchar(14), 
		Declination_J2000 varchar(15),
		Apparent_magnitude float default 0
	);
END


