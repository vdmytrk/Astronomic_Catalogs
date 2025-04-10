
--=======================================================================================================================================--
--==========================================================  STORED PRCEDURE  ==========================================================--
--=======================================================================================================================================--


CREATE OR ALTER PROC MigrateNGCICOStoNGCICO_W
AS
BEGIN
    DECLARE @Id INT,
            @NGC_IC VARCHAR(13),
            @Name INT,
            @SubObject VARCHAR(15),
            @Messier VARCHAR(15),
            @NGC VARCHAR(14),
            @IC VARCHAR(23),
            @ObjectTypeAbrev VARCHAR(21),
            @ObjectType VARCHAR(26),
            @RA VARCHAR(30),
            @DEC VARCHAR(31),
            @Constellation VARCHAR(21),
            @MajorAxis FLOAT,
            @MinorAxis FLOAT,
            @PositionAngle INT,
            @b_mag FLOAT,
            @v_mag FLOAT,
            @j_mag FLOAT,
            @h_mag FLOAT,
            @k_mag FLOAT,
            @Surface_Brigthness FLOAT,
            @Hubble_OnlyGalaxies VARCHAR(14),
            @Cstar_UMag FLOAT,
            @Cstar_BMag FLOAT,
            @Cstar_VMag FLOAT,
            @Cstar_Names VARCHAR(21),
            @CommonNames VARCHAR(110),
            @NedNotes VARCHAR(110),
            @OpenngcNotes VARCHAR(330),
            @Image VARCHAR(MAX);

	DECLARE @COUNTER INT,
			@SUM_ROWS INT;

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

    SET NOCOUNT ON; 

	--The object which do not have base object
	--Get Id which must to be dublocated other data exept NGC_IC, Name fields.
    IF OBJECT_ID('tempdb..##IDTODUBLICATE', 'U') IS NOT NULL
		BEGIN
			DROP TABLE ##IDTODUBLICATE;
		END;

    SELECT * INTO ##IDTODUBLICATE FROM (
        SELECT 
            DENSE_RANK() OVER(ORDER BY NGC_IC DESC, Name) AS DR,
            ROW_NUMBER() OVER(PARTITION BY NGC_IC + ' ' + CAST(Name AS VARCHAR) ORDER BY NGC_IC + ' ' + CAST(Name AS VARCHAR), SubObject) AS RN, 
            ID,
            NGC_IC,
            Name,
            SubObject
        FROM NGCICOpendatasoft_Source 
        WHERE NGC_IC + ' ' + CAST(Name AS VARCHAR) IN (
            SELECT NGC_IC + ' ' + CAST(Name AS VARCHAR) 
            FROM NGCICOpendatasoft_Source 
            WHERE SubObject <> ''
        )
    ) AS ST
    WHERE ST.RN = 1 AND ST.SubObject <> '';		

    TRUNCATE TABLE NGCICOpendatasoft;
    TRUNCATE TABLE NGCICOpendatasoft_Extension;


    BEGIN TRY
        IF @trancount = 0
			BEGIN	            
				BEGIN TRANSACTION NGC_IC_Opendatasoft_W_Tran;
			END;
        ELSE
			BEGIN
				COMMIT TRANSACTION NGC_IC_Opendatasoft_W_Tran;
				THROW 50001, 'There was an error becose there is more then one open transaction.', 1;
			END;
		
		SET @COUNTER = 1; 
		SET @SUM_ROWS = (SELECT COUNT(Id) FROM NGCICOpendatasoft_Source);

        WHILE @COUNTER <= @SUM_ROWS
        BEGIN
            SELECT
                @Id = ID,
                @NGC_IC = NGC_IC,
                @Name = Name,
                @SubObject = SubObject,
                @Messier = Messier,
                @NGC = NGC,
                @IC = IC,
                @ObjectTypeAbrev = ObjectTypeAbrev,
                @ObjectType = ObjectType,
                @RA = RA,
                @DEC = DEC,
                @Constellation = Constellation,
                @MajorAxis = MajorAxis,
                @MinorAxis = MinorAxis,
                @PositionAngle = PositionAngle,
                @b_mag = b_mag,
                @v_mag = v_mag,
                @j_mag = j_mag,
                @h_mag = h_mag,
                @k_mag = k_mag,
                @Surface_Brigthness = Surface_Brigthness,
                @Hubble_OnlyGalaxies = Hubble_OnlyGalaxies,
                @Cstar_UMag = Cstar_UMag,
                @Cstar_BMag = Cstar_BMag,
                @Cstar_VMag = Cstar_VMag,
                @Cstar_Names = Cstar_Names,
                @CommonNames = CommonNames,
                @NedNotes = NedNotes,
                @OpenngcNotes = OpenngcNotes,
                @Image = Image
            FROM NGCICOpendatasoft_Source
			WHERE ID = @COUNTER;

			SET @COUNTER += 1;
		
            IF EXISTS (SELECT 1 FROM ##IDTODUBLICATE
					   WHERE (@SubObject <> '' AND @SubObject = 'A')
					     AND NGC_IC + CAST([Name] AS VARCHAR) = @NGC_IC + CAST(@Name AS VARCHAR) )
				BEGIN
					INSERT INTO NGCICOpendatasoft (NGC_IC, [Name], SubObject) VALUES (@NGC_IC, @Name, 'DUPLICATE');
				
					EXEC [dbo].[InsertNGCICOpendatasoft] 
					'NGCICOpendatasoft_Extension',
					@Id, @NGC_IC, @Name, @SubObject, @Messier, @NGC, @IC, @ObjectTypeAbrev, @ObjectType, @RA, @DEC, @Constellation, 
					@MajorAxis, @MinorAxis, @PositionAngle, @b_mag, @v_mag, @j_mag, @h_mag, @k_mag, @Surface_Brigthness, 
					@Hubble_OnlyGalaxies, @Cstar_UMag, @Cstar_BMag, @Cstar_VMag, @Cstar_Names, @CommonNames, @NedNotes, @OpenngcNotes,
					@Image;
				END;
            ELSE IF @SubObject IN ('A', 'B', 'C', 'D', 'E', 'F', 'N', 'NW', 'S', 'SE') OR @SubObject LIKE 'NED0%'
				BEGIN
					EXEC [dbo].[InsertNGCICOpendatasoft] 
					'NGCICOpendatasoft_Extension',
					@Id, @NGC_IC, @Name, @SubObject, @Messier, @NGC, @IC, @ObjectTypeAbrev, @ObjectType, @RA, @DEC, @Constellation, 
					@MajorAxis, @MinorAxis, @PositionAngle, @b_mag, @v_mag, @j_mag, @h_mag, @k_mag, @Surface_Brigthness, 
					@Hubble_OnlyGalaxies, @Cstar_UMag, @Cstar_BMag, @Cstar_VMag, @Cstar_Names, @CommonNames, @NedNotes, @OpenngcNotes,
					@Image;
				END;
			ELSE
				BEGIN
					EXEC [dbo].[InsertNGCICOpendatasoft] 
					'NGCICOpendatasoft',
					@Id, @NGC_IC, @Name, @SubObject, @Messier, @NGC, @IC, @ObjectTypeAbrev, @ObjectType, @RA, @DEC, @Constellation, 
					@MajorAxis, @MinorAxis, @PositionAngle, @b_mag, @v_mag, @j_mag, @h_mag, @k_mag, @Surface_Brigthness, 
					@Hubble_OnlyGalaxies, @Cstar_UMag, @Cstar_BMag, @Cstar_VMag, @Cstar_Names, @CommonNames, @NedNotes, @OpenngcNotes,
					@Image;
				END;
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

		COMMIT TRANSACTION NGC_IC_Opendatasoft_W_Tran;

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
					ROLLBACK TRANSACTION NGC_IC_Opendatasoft_W_Tran;
				END;
				   
			SET @FuncProc = ERROR_PROCEDURE();
			SET @Line = ERROR_LINE();
			SET @ErrorNumber = ERROR_NUMBER();
			SET @ErrorSeverity = ERROR_SEVERITY();
			SET @ErrorState = ERROR_STATE();
			SET @ErrorMessage = ERROR_MESSAGE();

			INSERT INTO LogProcFunc (FuncProc, Line, ErrorNumber, ErrorSeverity, ErrorState, ErrorMessage) 
			VALUES (@FuncProc, @Line, @ErrorNumber, @ErrorSeverity, @ErrorState, @ErrorMessage);
			
			SET @FullEerrorMessage = N'An error occurred in MigrateNGCICOStoNGCICO_W: ' +
				' Error_number: ' + CAST(@ErrorNumber AS VARCHAR(10)) + 
				' Error_message: ' + CAST(@ErrorMessage AS NVARCHAR(MAX)) + 
				' Error_severity: ' + CAST(@ErrorSeverity AS VARCHAR(2)) +
				' Error_state: ' +  CAST(@ErrorState AS VARCHAR(3)) + 
				' Error_line: ' + CAST(@Line AS VARCHAR(10));

			THROW 50004, @FullEerrorMessage, 4;

		END TRY
		BEGIN CATCH
			PRINT 'An error occurred during handling error from NGC_IC_Opendatasoft_W_Tran transaction: ' + @ErrorMessage;
		END CATCH
	END CATCH;
END;