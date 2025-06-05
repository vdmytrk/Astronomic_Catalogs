--=======================================================================================================================================--
--==========================================================  STORED PRCEDURE  ==========================================================--
--=======================================================================================================================================--
-------------------------------------------------------------------------------------------------------------------------------------------
--================================================  MIGRATE DATA INTO NGCICOpendatasoft  ================================================--
-------------------------------------------------------------------------------------------------------------------------------------------

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
			@Limit_Ang_Diameter NVARCHAR(1),
			@Ang_Diameter FLOAT,
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
            @NedNotes NVARCHAR(MAX),
            @OpenngcNotes NVARCHAR(MAX),
            @Image VARCHAR(MAX);

	DECLARE @COUNTER INT,
			@SUM_ROWS INT;

	DECLARE @FuncProc AS NVARCHAR(100) = 'MigrateNGCICOStoNGCICO_C';
		
    DECLARE @StartedTran BIT = 0;

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
	

	IF @@TRANCOUNT = 0
    BEGIN                
        BEGIN TRANSACTION MigrateNGCICOStoNGCICO_W_Tran;
        SET @StartedTran = 1;
    END;    
    ELSE IF @@TRANCOUNT > 0 
    BEGIN
        DECLARE @ErrorMsg NVARCHAR(200) = 'Procedure ' + @FuncProc + ' cannot be executed inside another open transaction.';    
        THROW 50001, @ErrorMsg, 1;
    END;

    TRUNCATE TABLE NGCICOpendatasoft;
    TRUNCATE TABLE NGCICOpendatasoft_Extension;


    BEGIN TRY		
		SET @COUNTER = 1; 
		SET @SUM_ROWS = (SELECT COUNT(Id) FROM NGCICOpendatasoft_Source);

        WHILE @COUNTER <= @SUM_ROWS
        BEGIN
            SELECT 
                @Id = O.ID,
                @NGC_IC = O.NGC_IC,
                @Name = O.Name,
                @SubObject = O.SubObject,
                @Messier = O.Messier,
                @NGC = O.NGC,
                @IC = O.IC,
                @Limit_Ang_Diameter = T.Limit_Ang_Diameter,
                @Ang_Diameter = T.Ang_Diameter,
                @ObjectTypeAbrev = O.ObjectTypeAbrev,
                @ObjectType = O.ObjectType,
                @RA = O.RA,
                @DEC = O.DEC,
                @Constellation = O.Constellation,
                @MajorAxis = O.MajorAxis,
                @MinorAxis = O.MinorAxis,
                @PositionAngle = O.PositionAngle,
                @b_mag = O.b_mag,
                @v_mag = O.v_mag,
                @j_mag = O.j_mag,
                @h_mag = O.h_mag,
                @k_mag = O.k_mag,
                @Surface_Brigthness = O.Surface_Brigthness,
                @Hubble_OnlyGalaxies = O.Hubble_OnlyGalaxies,
                @Cstar_UMag = O.Cstar_UMag,
                @Cstar_BMag = O.Cstar_BMag,
                @Cstar_VMag = O.Cstar_VMag,
                @Cstar_Names = O.Cstar_Names,
                @CommonNames = O.CommonNames,
                @NedNotes = O.NedNotes,
                @OpenngcNotes = O.OpenngcNotes,
                @Image = O.Image
            FROM NGCICOpendatasoft_Source AS O
			LEFT JOIN NGC2000_UKTemporarily AS T -- Don’t use 'UPDATE NGCICOpendatasoft' as used below, since it updates both tables: NGCICOpendatasoft and NGCICOpendatasoft_Extension!
				ON O.NGC_IC + ' ' + CAST(O.Name AS varchar) = RTRIM(T.Catalog) + ' ' + CAST(T.Namber_name AS varchar)
			WHERE O.ID = @COUNTER;

			SET @COUNTER += 1;
		
            IF EXISTS (SELECT 1 FROM ##IDTODUBLICATE
					   WHERE (@SubObject <> '' AND @SubObject = 'A')
					     AND NGC_IC + CAST([Name] AS VARCHAR) = @NGC_IC + CAST(@Name AS VARCHAR) )
				BEGIN
					INSERT INTO NGCICOpendatasoft (NGC_IC, [Name], SubObject, SourceTable) VALUES (@NGC_IC, @Name, 'DUPLICATE', 'NGCICOpendatasoft');
				
					EXEC [dbo].[InsertNGCICOpendatasoft_Extension] 
					--'NGCICOpendatasoft_Extension',
					@Id, @NGC_IC, @Name, @SubObject, @Messier, @NGC, @IC, @Limit_Ang_Diameter, @Ang_Diameter, @ObjectTypeAbrev, 
					@ObjectType, @RA, @DEC, @Constellation, @MajorAxis, @MinorAxis, @PositionAngle, @b_mag, @v_mag, @j_mag, @h_mag, @k_mag, 
					@Surface_Brigthness, @Hubble_OnlyGalaxies, @Cstar_UMag, @Cstar_BMag, @Cstar_VMag, @Cstar_Names, @CommonNames, 
					@NedNotes, @OpenngcNotes, @Image;
				END;
            ELSE IF @SubObject IN ('A', 'B', 'C', 'D', 'E', 'F', 'N', 'NW', 'S', 'SE') OR @SubObject LIKE 'NED0%'
				BEGIN
					EXEC [dbo].[InsertNGCICOpendatasoft_Extension] 
					--'NGCICOpendatasoft_Extension',
					@Id, @NGC_IC, @Name, @SubObject, @Messier, @NGC, @IC, @Limit_Ang_Diameter, @Ang_Diameter, @ObjectTypeAbrev, 
					@ObjectType, @RA, @DEC, @Constellation, @MajorAxis, @MinorAxis, @PositionAngle, @b_mag, @v_mag, @j_mag, @h_mag, @k_mag, 
					@Surface_Brigthness, @Hubble_OnlyGalaxies, @Cstar_UMag, @Cstar_BMag, @Cstar_VMag, @Cstar_Names, @CommonNames, 
					@NedNotes, @OpenngcNotes, @Image;
				END;
			ELSE
				BEGIN
					EXEC [dbo].[InsertNGCICOpendatasoft] 
					--'NGCICOpendatasoft',
					@Id, @NGC_IC, @Name, @SubObject, @Messier, @NGC, @IC, @Limit_Ang_Diameter, @Ang_Diameter, @ObjectTypeAbrev, 
					@ObjectType, @RA, @DEC, @Constellation, @MajorAxis, @MinorAxis, @PositionAngle, @b_mag, @v_mag, @j_mag, @h_mag, @k_mag, 
					@Surface_Brigthness, @Hubble_OnlyGalaxies, @Cstar_UMag, @Cstar_BMag, @Cstar_VMag, @Cstar_Names, @CommonNames, 
					@NedNotes, @OpenngcNotes, @Image;
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
		
		IF @StartedTran = 1 AND XACT_STATE() = 1
            BEGIN
			COMMIT TRANSACTION MigrateNGCICOStoNGCICO_W_Tran; 
        END

		DROP TABLE ##IDTODUBLICATE;

    END TRY
    BEGIN CATCH
        BEGIN TRY
            DECLARE @FuncProcErr AS NVARCHAR(MAX), 
                    @Line AS INT, 
                    @ErrorNumber AS INT, 
                    @ErrorMessage NVARCHAR(MAX),  
                    @FullEerrorMessage NVARCHAR(MAX),
                    @ErrorSeverity INT, 
                    @ErrorState INT;
        
            DECLARE @RollebackMassage NVARCHAR(MAX);
         
            SET @FuncProcErr = ISNULL(ERROR_PROCEDURE(), N'UnknownProcedure');
            SET @Line = ERROR_LINE();
            SET @ErrorNumber = ERROR_NUMBER();
            SET @ErrorSeverity = ERROR_SEVERITY();
            SET @ErrorState = ERROR_STATE();
            SET @ErrorMessage = ERROR_MESSAGE();

            DECLARE @xstate INT = XACT_STATE();
         
            IF @xstate = -1
            BEGIN
                SET @RollebackMassage = N'ROLLBACK TRANSACTION STATE: @xstate = ' + CAST(@xstate AS VARCHAR);
                
                INSERT INTO LogProcFunc (FuncProc, ErrorSeverity, ErrorState, ErrorMessage) 
                VALUES (@FuncProc, 0, 0, @RollebackMassage);
        
                ROLLBACK TRANSACTION MigrateNGCICOStoNGCICO_W_Tran; 
            END;
            IF @xstate = 1 AND @StartedTran = 1 
            BEGIN
                SET @RollebackMassage = N'ROLLBACK TRANSACTION STATE: @xstate = ' + CAST(@xstate AS VARCHAR) + 
				        N' AND @StartedTran = ' + CAST(@StartedTran AS VARCHAR);
                
                INSERT INTO LogProcFunc (FuncProc, ErrorSeverity, ErrorState, ErrorMessage) 
                VALUES (@FuncProc, 0, 0, @RollebackMassage);

                ROLLBACK TRANSACTION MigrateNGCICOStoNGCICO_W_Tran; 
            END;
               
         
            SET @FullEerrorMessage = 
               N'An error occurred in ' + @FuncProcErr + N': ' +
               N' Error_number: ' + CAST(@ErrorNumber AS NVARCHAR) + 
               N' Error_message: ' + ISNULL(@ErrorMessage, N'N/A') + 
               N' Error_severity: ' + CAST(@ErrorSeverity AS NVARCHAR) +
               N' Error_state: ' + CAST(@ErrorState AS NVARCHAR) + 
               N' Error_line: ' + CAST(@Line AS NVARCHAR);

            INSERT INTO LogProcFunc (FuncProc, Line, ErrorNumber, ErrorSeverity, ErrorState, ErrorMessage) 
            VALUES (@FuncProcErr, @Line, @ErrorNumber, @ErrorSeverity, @ErrorState, @ErrorMessage);
            
            THROW 51000, @FullEerrorMessage, 0;
        END TRY        
        BEGIN CATCH        
            IF @FullEerrorMessage IS NULL 
                SET @FullEerrorMessage = 'Unknown error occurred and logging also failed.';
                    
            DECLARE @SecondErrorMessage NVARCHAR(MAX);
            SET @SecondErrorMessage = 
            'An error occurred during handling error in ' + @FuncProcErr + ' stored procedure: ' + @FullEerrorMessage;
        
            THROW 52000, @FullEerrorMessage, 0;
        END CATCH
    END CATCH;
END;