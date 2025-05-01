
--=======================================================================================================================================--
--==========================================================  STORED PRCEDURE  ==========================================================--
--=======================================================================================================================================--



CREATE OR ALTER PROC MigrateNGCICOStoNGCICO_C
AS 
BEGIN 
    DECLARE @Id int;
	DECLARE @NGC_IC varchar(13);
    DECLARE @Name int;
    DECLARE @SubObject varchar(15);
	DECLARE @Messier varchar(15);
    DECLARE @NGC varchar(14);
    DECLARE @IC varchar(23);
	DECLARE @Limit_Ang_Diameter NVARCHAR(1);
	DECLARE @Ang_Diameter FLOAT;
    DECLARE @ObjectTypeAbrev varchar(21);
    DECLARE @ObjectType	varchar(26);
    DECLARE @RA varchar(30);
    DECLARE @DEC varchar(31);
    DECLARE @Constellation varchar(21); 
    DECLARE @MajorAxis float;
    DECLARE @MinorAxis float;
    DECLARE @PositionAngle int;
    DECLARE @b_mag float;
    DECLARE @v_mag float;
    DECLARE @j_mag float;
    DECLARE @h_mag float;
    DECLARE @k_mag float;
    DECLARE @Surface_Brigthness float;
    DECLARE @Hubble_OnlyGalaxies varchar(14);
    DECLARE @Cstar_UMag float;
    DECLARE @Cstar_BMag float;
    DECLARE @Cstar_VMag float;
    DECLARE @Cstar_Names varchar(21);
    DECLARE @CommonNames varchar(110);
    DECLARE @NedNotes varchar(max);
    DECLARE @OpenngcNotes varchar(max);
    DECLARE @Image varchar(max);
	
	-- For error hendling
	DECLARE @FuncProc AS VARCHAR(50), 
			@Line AS INT, 
			@ErrorNumber AS INT, 
			@ErrorMessage NVARCHAR(MAX), 
			@FullEerrorMessage NVARCHAR(MAX),
			@ErrorSeverity INT, 
			@ErrorState INT;

	DECLARE @trancount INT;
	SET @trancount = @@TRANCOUNT; 


	SET @Limit_Ang_Diameter = NULL;
	SET @Ang_Diameter = 0;

	SET NOCOUNT ON;	  

	--The object which do not have base object
	--Get Id which must to be dublocated other data exept NGC_IC, Name fields.
	IF OBJECT_ID('tempdb..##IDTODUBLICATE', 'U') IS NOT NULL
	BEGIN
		DROP TABLE ##IDTODUBLICATE;
	END;

	SELECT * INTO ##IDTODUBLICATE FROM (
		SELECT 
			ROW_NUMBER() OVER(PARTITION BY NGC_IC + ' ' + CAST(Name AS VARCHAR) ORDER BY NGC_IC + ' ' + CAST(Name AS VARCHAR), SubObject) AS RN, 
			ID,
			NGC_IC,
			Name,
			SubObject
		FROM NGCICOpendatasoft_Source 
		WHERE NGC_IC + ' ' + CAST(Name AS VARCHAR) IN (
			SELECT NGC_IC + ' ' + CAST(Name AS VARCHAR) 
			FROM NGCICOpendatasoft_Source 
			WHERE SubObject <> '')  
		) AS ST
	WHERE ST.RN = 1 AND ST.SubObject <> '';

	TRUNCATE TABLE NGCICOpendatasoft;
	TRUNCATE TABLE NGCICOpendatasoft_Extension;		
	
	BEGIN TRY
        IF @trancount = 0
			BEGIN
				BEGIN TRANSACTION NGC_IC_Opendatasoft_C_Tran;
			END;
        ELSE
			BEGIN
				COMMIT TRANSACTION NGC_IC_Opendatasoft_C_Tran;
				THROW 50001, 'There was an error becose there is more then one open transaction.', 1; 
			END;

		IF CURSOR_STATUS('local', 'NGCICOpendatasoft_Source_Cursor') >= -1
		BEGIN
			CLOSE NGCICOpendatasoft_Source_Cursor;
			DEALLOCATE NGCICOpendatasoft_Source_Cursor;
		END;

		DECLARE NGCICOpendatasoft_Source_Cursor CURSOR LOCAL FOR
			SELECT DISTINCT 
				O.ID, 
				O.NGC_IC, 
				O.Name, 
				O.SubObject, 
				O.Messier, 
				O.NGC, 
				O.IC,
				T.Limit_Ang_Diameter, 
				T.Ang_Diameter,
                O.ObjectTypeAbrev, 
				O.ObjectType, 
				O.RA, 
				O.DEC, 
				O.Constellation, 
				O.MajorAxis, 
				O.MinorAxis, 
				O.PositionAngle,
                O.b_mag, 
				O.v_mag, 
				O.j_mag, 
				O.h_mag, 
				O.k_mag, 
				O.Surface_Brigthness, 
				O.Hubble_OnlyGalaxies, 
				O.Cstar_UMag,
                O.Cstar_BMag, 
				O.Cstar_VMag, 
				O.Cstar_Names, 
				O.CommonNames, 
				O.NedNotes, 
				O.OpenngcNotes, 
				O.Image
			FROM NGCICOpendatasoft_Source AS O
			LEFT JOIN NGC2000_UKTemporarily AS T -- Don’t use 'UPDATE NGCICOpendatasoft' as used below, since it updates both tables: NGCICOpendatasoft and NGCICOpendatasoft_Extension!
				ON O.NGC_IC + ' ' + CAST(O.Name AS varchar) = RTRIM(T.Catalog) + ' ' + CAST(T.Namber_name AS varchar);

		OPEN NGCICOpendatasoft_Source_Cursor;
		
		FETCH NEXT FROM NGCICOpendatasoft_Source_Cursor INTO
			@Id, @NGC_IC, @Name, @SubObject, @Messier, @NGC, @IC, @Limit_Ang_Diameter, @Ang_Diameter, 
			@ObjectTypeAbrev, @ObjectType, @RA, @DEC, @Constellation, @MajorAxis, @MinorAxis, @PositionAngle, 
			@b_mag, @v_mag, @j_mag, @h_mag, @k_mag, @Surface_Brigthness, @Hubble_OnlyGalaxies, @Cstar_UMag,
			@Cstar_BMag, @Cstar_VMag, @Cstar_Names, @CommonNames, @NedNotes, @OpenngcNotes, @Image;
		
		WHILE @@FETCH_STATUS = 0
		BEGIN	
			IF EXISTS (SELECT 1 FROM ##IDTODUBLICATE 
					   WHERE (@SubObject <> '' AND @SubObject = 'A')
					     AND NGC_IC + CAST([Name] AS VARCHAR) = @NGC_IC + CAST(@Name AS VARCHAR) ) 
				BEGIN					
					INSERT INTO NGCICOpendatasoft (NGC_IC, [Name], SubObject) VALUES (@NGC_IC, @Name, 'DUPLICATE');
						
					EXEC [dbo].[InsertNGCICOpendatasoft] 
					'NGCICOpendatasoft_Extension',
					@Id, @NGC_IC, @Name, @SubObject, @Messier, @NGC, @IC, @Limit_Ang_Diameter, @Ang_Diameter, @ObjectTypeAbrev, 
					@ObjectType, @RA, @DEC, @Constellation, @MajorAxis, @MinorAxis, @PositionAngle, @b_mag, @v_mag, @j_mag, @h_mag, @k_mag, 
					@Surface_Brigthness, @Hubble_OnlyGalaxies, @Cstar_UMag, @Cstar_BMag, @Cstar_VMag, @Cstar_Names, @CommonNames, 
					@NedNotes, @OpenngcNotes, @Image;
				END;
			ELSE IF (@SubObject IN ('A', 'B', 'C', 'D', 'E', 'F', 'N', 'NW', 'S', 'SE') OR @SubObject LIKE 'NED0%')
				BEGIN
					EXEC [dbo].[InsertNGCICOpendatasoft] 
					'NGCICOpendatasoft_Extension',
					@Id, @NGC_IC, @Name, @SubObject, @Messier, @NGC, @IC, @Limit_Ang_Diameter, @Ang_Diameter, @ObjectTypeAbrev, 
					@ObjectType, @RA, @DEC, @Constellation, @MajorAxis, @MinorAxis, @PositionAngle, @b_mag, @v_mag, @j_mag, @h_mag, @k_mag, 
					@Surface_Brigthness, @Hubble_OnlyGalaxies, @Cstar_UMag, @Cstar_BMag, @Cstar_VMag, @Cstar_Names, @CommonNames, 
					@NedNotes, @OpenngcNotes, @Image;
				END;
			ELSE
				BEGIN
					EXEC [dbo].[InsertNGCICOpendatasoft] 
					'NGCICOpendatasoft',
					@Id, @NGC_IC, @Name, @SubObject, @Messier, @NGC, @IC, @Limit_Ang_Diameter, @Ang_Diameter, @ObjectTypeAbrev, 
					@ObjectType, @RA, @DEC, @Constellation, @MajorAxis, @MinorAxis, @PositionAngle, @b_mag, @v_mag, @j_mag, @h_mag, @k_mag, 
					@Surface_Brigthness, @Hubble_OnlyGalaxies, @Cstar_UMag, @Cstar_BMag, @Cstar_VMag, @Cstar_Names, @CommonNames, 
					@NedNotes, @OpenngcNotes, @Image;
				END;
   	  
			FETCH NEXT FROM NGCICOpendatasoft_Source_Cursor INTO
			@Id, @NGC_IC, @Name, @SubObject, @Messier, @NGC, @IC, @Limit_Ang_Diameter, @Ang_Diameter, @ObjectTypeAbrev, @ObjectType, @RA, @DEC, 
			@Constellation, @MajorAxis, @MinorAxis, @PositionAngle, @b_mag, @v_mag, @j_mag, @h_mag, @k_mag, @Surface_Brigthness, 
			@Hubble_OnlyGalaxies, @Cstar_UMag, @Cstar_BMag, @Cstar_VMag, @Cstar_Names, @CommonNames, @NedNotes, @OpenngcNotes, @Image;

		END;

		IF CURSOR_STATUS('local', 'NGCICOpendatasoft_Source_Cursor') >= -1
			BEGIN
				CLOSE NGCICOpendatasoft_Source_Cursor;
				DEALLOCATE NGCICOpendatasoft_Source_Cursor;
			END;

		UPDATE NGCICOpendatasoft
		SET Name_UK = T2.Name,
			Comment = T2.Comment,
			Source_Type = T2.Source_Type,
			App_Mag = T2.App_Mag,
			App_Mag_Flag = T2.App_Mag_Flag
		FROM NGCICOpendatasoft AS T1
		INNER JOIN NGC2000_UKTemporarily AS T2
		ON T1.NGC_IC + CAST(T1.[Name] AS VARCHAR) = RTRIM(T2.Catalog) + CAST(LTRIM(RTRIM(T2.Namber_name)) AS VARCHAR);
			   		 
		UPDATE NGCICOpendatasoft
		SET Other_names = T3.Other_names,
			Object_type = T3.Object_type
		FROM NGCICOpendatasoft AS T1
		JOIN NGCWikipedia_TemporarilySource AS T3
		ON T1.NGC_IC + CAST(T1.[Name] AS VARCHAR) = 'NGC' + CAST(T3.NGC_number AS VARCHAR);

		COMMIT TRANSACTION NGC_IC_Opendatasoft_C_Tran;
		
		DROP TABLE ##IDTODUBLICATE;

	END TRY
	BEGIN CATCH
		BEGIN TRY
			DECLARE @xstate INT;
			SET @xstate = XACT_STATE();

			IF @xstate = -1
				BEGIN
					PRINT 'EXCEPTION STATE: @xstate = ' + CAST(@xstate AS VARCHAR);
					ROLLBACK;
				END;
			IF @xstate = 1 and @trancount = 0
				BEGIN
					PRINT 'EXCEPTION STATE: @xstate = ' + CAST(@xstate AS VARCHAR) + ', @trancount = ' + CAST(@trancount AS VARCHAR);
					ROLLBACK;
				END;
			IF @xstate = 1 and @trancount > 0				
				BEGIN
					PRINT 'EXCEPTION STATE: @xstate = ' + CAST(@xstate AS VARCHAR) + ', @trancount = ' + CAST(@trancount AS VARCHAR);
					ROLLBACK TRANSACTION NGC_IC_Opendatasoft_C_Tran;
				END;
				   
			SET @FuncProc = ERROR_PROCEDURE();
			SET @Line = ERROR_LINE();
			SET @ErrorNumber = ERROR_NUMBER();
			SET @ErrorSeverity = ERROR_SEVERITY();
			SET @ErrorState = ERROR_STATE();
			SET @ErrorMessage = ERROR_MESSAGE();

			INSERT INTO LogProcFunc (FuncProc, Line, ErrorNumber, ErrorSeverity, ErrorState, ErrorMessage) 
			VALUES (@FuncProc, @Line, @ErrorNumber, @ErrorSeverity, @ErrorState, @ErrorMessage);
			
			SET @FullEerrorMessage = 'An error occurred in MigrateNGCICOStoNGCICO_C: ' +
				' Error_number: ' + CAST(@ErrorNumber AS VARCHAR(10)) + 
				' Error_message: ' + CAST(@ErrorMessage AS NVARCHAR(MAX)) + 
				' Error_severity: ' + CAST(@ErrorSeverity AS VARCHAR(2)) +
				' Error_state: ' +  CAST(@ErrorState AS VARCHAR(3)) + 
				' Error_line: ' + CAST(@Line AS VARCHAR(10));

			THROW 50004, @FullEerrorMessage, 4;

		END TRY
		BEGIN CATCH
			PRINT N'An error occurred during handling error from NGC_IC_Opendatasoft_C_Tran transaction: ' + @ErrorMessage;
		END CATCH
	END CATCH
END;