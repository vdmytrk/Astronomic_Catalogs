
--=======================================================================================================================================--
--==========================================================  STORED PRCEDURE  ==========================================================--
--=======================================================================================================================================--
-------------------------------------------------------------------------------------------------------------------------------------------
--================================================  INSET DATA INTO NGCICOpendatasoft  ================================================--
-------------------------------------------------------------------------------------------------------------------------------------------
CREATE OR ALTER PROC InsertNGCICOpendatasoft
    @TableName NVARCHAR(100), 
    @Id INT,
    @NGC_IC NVARCHAR(13),
    @Name INT,
    @SubObject NVARCHAR(15),
    @Messier NVARCHAR(15),
    @NGC NVARCHAR(15),
    @IC NVARCHAR(23),
    @ObjectTypeAbrev NVARCHAR(21),
    @ObjectType NVARCHAR(26),
    @RA NVARCHAR(30),
    @DEC NVARCHAR(31),
    @Constellation NVARCHAR(21),
    @MajorAxis FLOAT,
    @MinorAxis FLOAT,
    @PositionAngle INT,
    @b_mag FLOAT,
    @v_mag FLOAT,
    @j_mag FLOAT,
    @h_mag FLOAT,
    @k_mag FLOAT,
    @Surface_Brigthness FLOAT,
    @Hubble_OnlyGalaxies NVARCHAR(14),
    @Cstar_UMag FLOAT,
    @Cstar_BMag FLOAT,
    @Cstar_VMag FLOAT,
    @Cstar_Names NVARCHAR(255),
    @CommonNames NVARCHAR(255),
    @NedNotes NVARCHAR(255),
    @OpenngcNotes NVARCHAR(330),
    @Image NVARCHAR(MAX)
AS
BEGIN	
	SET NOCOUNT ON; 

    DECLARE @SQL NVARCHAR(MAX);

	-- For error hendling
	DECLARE @FuncProc AS VARCHAR(50), 
			@Line AS INT, 
			@ErrorNumber AS INT, 
			@ErrorMessage NVARCHAR(MAX), 
			@ErrorSeverity INT, 
			@ErrorState INT;

	BEGIN TRY
		SET @SQL = 'INSERT INTO ' + QUOTENAME(@TableName) + ' (
			NGC_IC, [Name], SubObject, Messier, NGC, IC, ObjectTypeAbrev, ObjectType, 
			RA, Right_ascension_H, Right_ascension_M, Right_ascension_S,
			[DEC], NS, Declination_D, Declination_M, Declination_S,
			Constellation, MajorAxis, MinorAxis, PositionAngle, b_mag, v_mag, j_mag, h_mag, k_mag, Surface_Brigthness,
			Hubble_OnlyGalaxies, Cstar_UMag, Cstar_BMag, Cstar_VMag, Cstar_Names, CommonNames, NedNotes, OpenngcNotes, [Image]
		)
		VALUES (
			@NGC_IC, @Name, @SubObject, @Messier, @NGC, @IC, @ObjectTypeAbrev, @ObjectType, 
			@RA, 
			REPLACE((SELECT RA_H
					FROM (SELECT ID, VALUE AS RA_H, ROW_NUMBER() OVER (PARTITION BY ID ORDER BY ID) AS RN 
						  FROM NGCICOpendatasoft_Source
						  CROSS APPLY STRING_SPLIT(@RA, '':'') AS VALUE
						  WHERE VALUE <> '''') AS T2
					WHERE T2.ID = @Id AND RN = 1), ''00'', 0), -- as Right_ascension_H, 
			REPLACE((SELECT RA_M
					FROM (SELECT ID, VALUE AS RA_M, ROW_NUMBER() OVER (PARTITION BY ID ORDER BY ID) AS RN 
						  FROM NGCICOpendatasoft_Source
						  CROSS APPLY STRING_SPLIT(@RA, '':'') AS VALUE
						  WHERE VALUE <> '''') AS T2
					WHERE T2.ID = @Id AND RN = 2), ''00'', 0), -- as Right_ascension_M,
			(SELECT RA_S
				FROM (SELECT ID, VALUE AS RA_S, ROW_NUMBER() OVER (PARTITION BY ID ORDER BY ID) AS RN 
					  FROM NGCICOpendatasoft_Source
					  CROSS APPLY STRING_SPLIT(@RA, '':'') AS VALUE
					  WHERE VALUE <> '''') AS T2
				WHERE T2.ID = @Id AND RN = 3), -- as Right_ascension_S,
			@DEC, 
			LEFT(REPLACE(@DEC, CHAR(9), ''''), 1), --  AS NS,
			REPLACE(REPLACE(REPLACE((SELECT D_H
									FROM (SELECT ID, VALUE AS D_H, ROW_NUMBER() OVER (PARTITION BY ID ORDER BY ID) AS RN 
										  FROM NGCICOpendatasoft_Source
										  CROSS APPLY STRING_SPLIT(@DEC, '':'') AS VALUE
										  WHERE VALUE <> '''') AS T2
									WHERE T2.ID = @Id AND RN = 1), ''+'', ''''), ''-'', ''''), ''00'', 0), -- as Declination_D, 
			REPLACE((SELECT D_M
					FROM (SELECT ID, VALUE AS D_M, ROW_NUMBER() OVER (PARTITION BY ID ORDER BY ID) AS RN 
						  FROM NGCICOpendatasoft_Source
						  CROSS APPLY STRING_SPLIT(@DEC, '':'') AS VALUE
						  WHERE VALUE <> '''') AS T2
					WHERE T2.ID = @Id AND RN = 2), ''00'', 0), -- as Declination_M,
			(SELECT D_S
				FROM (SELECT ID, VALUE AS D_S, ROW_NUMBER() OVER (PARTITION BY ID ORDER BY ID) AS RN 
					  FROM NGCICOpendatasoft_Source
					  CROSS APPLY STRING_SPLIT(@DEC, '':'') AS VALUE
					  WHERE VALUE <> '''') AS T2
				WHERE T2.ID = @Id AND RN = 3), -- as Declination_S, 
			@Constellation, @MajorAxis, @MinorAxis, @PositionAngle, @b_mag, @v_mag, @j_mag, @h_mag, @k_mag, @Surface_Brigthness,
			@Hubble_OnlyGalaxies, @Cstar_UMag, @Cstar_BMag, @Cstar_VMag, @Cstar_Names, @CommonNames, @NedNotes, @OpenngcNotes, @Image
		);';
		
		EXEC sp_executesql @SQL, 
			N'@Id INT, @NGC_IC NVARCHAR(13), @Name INT, @SubObject NVARCHAR(15), @Messier NVARCHAR(15), @NGC NVARCHAR(15), @IC NVARCHAR(23),
			@ObjectTypeAbrev NVARCHAR(21), @ObjectType NVARCHAR(26), @RA NVARCHAR(30), @DEC NVARCHAR(31), @Constellation NVARCHAR(21), 
			@MajorAxis FLOAT, @MinorAxis FLOAT, @PositionAngle INT, @b_mag FLOAT, @v_mag FLOAT, @j_mag FLOAT, @h_mag FLOAT, 
			@k_mag FLOAT, @Surface_Brigthness FLOAT, @Hubble_OnlyGalaxies NVARCHAR(14), @Cstar_UMag FLOAT, @Cstar_BMag FLOAT, 
			@Cstar_VMag FLOAT, @Cstar_Names NVARCHAR(255), @CommonNames NVARCHAR(255), @NedNotes NVARCHAR(255), 
			@OpenngcNotes NVARCHAR(330), @Image NVARCHAR(MAX)',
			@Id, @NGC_IC, @Name, @SubObject, @Messier, @NGC, @IC, 
			@ObjectTypeAbrev, @ObjectType, @RA, @DEC, @Constellation, 
			@MajorAxis, @MinorAxis, @PositionAngle, @b_mag, @v_mag, @j_mag, @h_mag, 
			@k_mag, @Surface_Brigthness, @Hubble_OnlyGalaxies, @Cstar_UMag, @Cstar_BMag, 
			@Cstar_VMag, @Cstar_Names, @CommonNames, @NedNotes, 
			@OpenngcNotes, @Image;		
	END TRY
	BEGIN CATCH						
			SET @ErrorMessage = 'An error occurred during handling error from OuterTransaction transaction INTO INNER STORES PROCEDURE: ' +
				' Error_number: ' + CAST(ERROR_NUMBER() AS VARCHAR(10)) + 
				' Error_message: ' + CAST(ERROR_MESSAGE() AS NVARCHAR(MAX)) +
				' Error_severity: ' + CAST(ERROR_SEVERITY() AS VARCHAR(2)) +
				' Error_state: ' +  CAST(ERROR_STATE() AS VARCHAR(3)) + 
				' Error_line: ' + CAST(ERROR_LINE() AS VARCHAR(10));
		THROW 50003, @ErrorMessage, 3;
	END CATCH
END;