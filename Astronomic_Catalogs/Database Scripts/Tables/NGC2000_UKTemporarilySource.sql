
--=========================================================================================================================================
---------------------------------------------------    NGC2000_UKTemporarilySource    ---------------------------------------------------
--=========================================================================================================================================





IF OBJECT_ID('dbo.NGC2000_UKTemporarilySource', 'U') IS NOT NULL
BEGIN
	TRUNCATE TABLE NGC2000_UKTemporarilySource;
END
ELSE 
BEGIN
	CREATE TABLE NGC2000_UKTemporarilySource (
		Id int primary key identity,
		Namber_name varchar(6),
		Source_Type varchar(5) default null,
		Right_ascension varchar(100) default null,
		Ref_Revision varchar(2) default null,
		Constellation varchar(3) default null,
		Limit_Ang_Diameter varchar(1) default null,
		Ang_Diameter float default 0,
		App_Mag float default 0,
		App_Mag_Flag varchar(1) default null,
		Description varchar(100) default null,
		Class varchar(10) default null 
	);	
END



