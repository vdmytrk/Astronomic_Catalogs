
--=========================================================================================================================================
---------------------------------------------------     NGCICOpendatasoft_Source      ---------------------------------------------------
--=========================================================================================================================================





IF OBJECT_ID('dbo.NGCICOpendatasoft_Source', 'U') IS NOT NULL
BEGIN
	TRUNCATE TABLE NGCICOpendatasoft_Source;
END
ELSE 
BEGIN
	CREATE TABLE NGCICOpendatasoft_Source (
		Id int Primary Key identity not null,
		NGC_IC varchar(13) default null,
		[Name] int default null,
		SubObject varchar(15) default null,
		Messier	varchar(15) default null,
		NGC	varchar(14) default null,
		IC varchar(23) default null,
		ObjectTypeAbrev	varchar(21) default null,
		ObjectType varchar(26) default null,
		RA varchar(30) default null,
		[DEC]	varchar(31) default null,
		Constellation varchar(21) default null,
		MajorAxis float default 0,
		MinorAxis float default 0,
		PositionAngle int default 0,
		b_mag float default 0,
		v_mag float default 0,
		j_mag float default 0,
		h_mag float default 0,
		k_mag float default 0,
		Surface_Brigthness float default 0,
		Hubble_OnlyGalaxies	varchar(14) default null,
		Cstar_UMag float default 0,
		Cstar_BMag float default 0,
		Cstar_VMag float default 0,
		Cstar_Names varchar(21) default null,
		CommonNames varchar(110) default null,
		NedNotes nvarchar(max) default null,
		OpenngcNotes nvarchar(max) default null,
		[Image] varchar(max) default null
	);
END


