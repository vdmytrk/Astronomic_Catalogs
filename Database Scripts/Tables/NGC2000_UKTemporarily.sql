
--=========================================================================================================================================
-----------------------------------------------------     NGC2000_UKTemporarily      -----------------------------------------------------
--=========================================================================================================================================





IF OBJECT_ID('dbo.NGC2000_UKTemporarily', 'U') IS NOT NULL
BEGIN
	TRUNCATE TABLE NGC2000_UKTemporarily;
END
ELSE 
BEGIN
	create table NGC2000_UKTemporarily (
		Id int primary key identity, 
		Catalog varchar(3) not null,
		Namber_name int default 0, 
		Name varchar(50) default null, 
		Comment varchar(50) default null, 
		Source_Type varchar(5) default null, 
		Right_ascension AS 
			ISNULL(CAST([Right_ascension_H] AS varchar(10)), '') + 'h ' + 
			ISNULL(CAST([Right_ascension_M] AS varchar(10)), '') + 'm ' + 
			ISNULL(CAST([Right_ascension_S] AS varchar(10)), '') + 's' PERSISTED,
		Right_ascension_H int default 00, 
		Right_ascension_M float default 0, 
		Right_ascension_S float default 0.0, 
		Declination AS 
			ISNULL(CAST([Declination_D] AS varchar(10)), '') + '° ' + 
			ISNULL(CAST([Declination_M] AS varchar(10)), '') + ''' ' + 
			ISNULL(CAST([Declination_S] AS varchar(10)), '') + '"' PERSISTED,
		Declination_D int default 00, 
		Declination_M int default 0,
		Declination_S float default 0.0,
		LII int default null,
		BII int default null,
		Ref_Revision varchar(2) default null,
		Constellation varchar(3) default null,
		Limit_Ang_Diameter varchar(1) default null,
		Ang_Diameter float default null,
		App_Mag float default null,
		App_Mag_Flag varchar(1) default null,
		Description varchar(100) default null,
		Class varchar(10) default null, 
		Note varchar(max)
	);
END


