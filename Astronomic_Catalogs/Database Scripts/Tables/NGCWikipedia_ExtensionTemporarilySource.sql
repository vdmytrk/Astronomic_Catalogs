
--=========================================================================================================================================
-------------------------------------------     NGCWikipedia_ExtensionTemporarilySource      -------------------------------------------
--=========================================================================================================================================





IF OBJECT_ID('dbo.NGCWikipedia_ExtensionTemporarilySource', 'U') IS NOT NULL
BEGIN
	TRUNCATE TABLE NGCWikipedia_ExtensionTemporarilySource;
END
ELSE 
BEGIN
	CREATE TABLE NGCWikipedia_ExtensionTemporarilySource  (
		NGC_number int not null FOREIGN KEY REFERENCES NGCWikipedia_TemporarilySource(NGC_number),
		Subtype varchar(2) not null,
		Ohter_names varchar(400) default null,
		Object_type varchar(60) default null,
		Cnstellation varchar(20) not null,
		Right_ascension_J2000 varchar(14), 
		Declination_J2000 varchar(15),
		Apparent_magnitude float default 0
	);
END



