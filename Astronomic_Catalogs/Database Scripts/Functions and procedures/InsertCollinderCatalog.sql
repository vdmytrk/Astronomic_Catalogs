--=======================================================================================================================================--
--==========================================================  STORED PRCEDURE  ==========================================================--
--=======================================================================================================================================--
-------------------------------------------------------------------------------------------------------------------------------------------
--================================================   INSERT DATA INTO CollinderCatalog   ================================================--
-------------------------------------------------------------------------------------------------------------------------------------------

CREATE OR ALTER PROC InsertCollinderCatalog
AS
BEGIN
    -- For error hendling
	DECLARE @FuncProc AS VARCHAR(50), 
			@Line AS INT, 
			@ErrorNumber AS INT, 
			@ErrorMessage NVARCHAR(MAX), 
			@FullEerrorMessage NVARCHAR(MAX),
			@ErrorSeverity INT, 
			@ErrorState INT;
	
    SET NOCOUNT ON; 
	
	BEGIN TRY
		INSERT CollinderCatalog (
			[Namber_name], 
			[NameOtherCat], 
			[Constellation], 

			[Right_ascension], 
			Right_ascension_H,
			Right_ascension_M,
			Right_ascension_S,

			[Declination], 
			NS,
			Declination_D,
			Declination_M,
			Declination_S,

			[App_Mag], 
			App_Mag_Flag,
			[CountStars], 
			CountStars_ToFinding,
			[Ang_Diameter],
			[Ang_Diameter_Max], 
			[Class],
			[Comment]) 
		SELECT
			[Namber_name],
			[NameOtherCat],
			[Constellation],
			[Right_ascension],
			REPLACE((SELECT RA_H
					FROM (SELECT ID, VALUE AS RA_H, ROW_NUMBER() OVER (PARTITION BY ID ORDER BY ID) AS RN 
						  FROM CollinderCatalog_Temporarily
							  CROSS APPLY STRING_SPLIT(Right_ascension, ' ') AS VALUE
						  WHERE VALUE <> '') AS T2
					WHERE T2.ID = T1.ID AND RN = 1), 'h', '') as Right_ascension_H, 
			REPLACE((SELECT RA_M
					FROM (SELECT ID, VALUE AS RA_M, ROW_NUMBER() OVER (PARTITION BY ID ORDER BY ID) AS RN 
						  FROM CollinderCatalog_Temporarily
							 CROSS APPLY STRING_SPLIT(Right_ascension, ' ') AS VALUE
						  WHERE VALUE <> '') AS T2
					WHERE T2.ID = T1.ID AND RN = 2), 'm', '') as Right_ascension_M,
			REPLACE((SELECT RA_S
					FROM (SELECT ID, VALUE AS RA_S, ROW_NUMBER() OVER (PARTITION BY ID ORDER BY ID) AS RN 
						  FROM CollinderCatalog_Temporarily
							 CROSS APPLY STRING_SPLIT(Right_ascension, ' ') AS VALUE
						  WHERE VALUE <> '') AS T2
					WHERE T2.ID = T1.ID AND RN = 3), 's', '') as Right_ascension_S,
			REPLACE(Declination, '?', '°') AS [Declination],
			LEFT(LTRIM(Declination), 1) AS NS,
			REPLACE(REPLACE(REPLACE(REPLACE((SELECT D_H
					FROM (SELECT ID, VALUE AS D_H, ROW_NUMBER() OVER (PARTITION BY ID ORDER BY ID) AS RN 
						  FROM CollinderCatalog_Temporarily
							  CROSS APPLY STRING_SPLIT([Declination], ' ') AS VALUE
						  WHERE VALUE <> '') AS T2
					WHERE T2.ID = T1.ID AND RN = 1), '?', ''), '°', ''), '+', ''), '-', '') as Declination_D, 
			REPLACE((SELECT D_M
					FROM (SELECT ID, VALUE AS D_M, ROW_NUMBER() OVER (PARTITION BY ID ORDER BY ID) AS RN 
						  FROM CollinderCatalog_Temporarily
							 CROSS APPLY STRING_SPLIT([Declination], ' ') AS VALUE
						  WHERE VALUE <> '') AS T2
					WHERE T2.ID = T1.ID AND RN = 2), '''', '') as Declination_M,
			REPLACE((SELECT D_S
					FROM (SELECT ID, VALUE AS D_S, ROW_NUMBER() OVER (PARTITION BY ID ORDER BY ID) AS RN 
						  FROM CollinderCatalog_Temporarily
							 CROSS APPLY STRING_SPLIT([Declination], ' ') AS VALUE
						  WHERE VALUE <> '') AS T2
					WHERE T2.ID = T1.ID AND RN = 3), '"', '') as Declination_S, 
			CASE 
				WHEN CHARINDEX('v', RIGHT(LTRIM(RTRIM(App_Mag)), 1)) = 1 OR CHARINDEX('p', RIGHT(App_Mag, 1)) = 1			
					THEN SUBSTRING(LTRIM(RTRIM(App_Mag)), 1, LEN(LTRIM(RTRIM(App_Mag)))-1)
				WHEN TRY_CAST(LTRIM(RTRIM(App_Mag)) AS FLOAT) IS NOT NULL OR TRY_CAST(LTRIM(RTRIM(App_Mag)) AS INT) IS NOT NULL
					THEN LTRIM(RTRIM(App_Mag))
				ELSE NULL
			END AS App_Mag,
			CASE
				WHEN TRY_CAST(RIGHT(LTRIM(RTRIM(App_Mag)), 1) AS INT) IS NULL AND
						(CHARINDEX('v', RIGHT(LTRIM(RTRIM(App_Mag)), 1)) = 1 OR CHARINDEX('p', RIGHT(LTRIM(RTRIM(App_Mag)), 1)) = 1)
					THEN RIGHT(LTRIM(RTRIM(App_Mag)), 1) 
				WHEN LTRIM(RTRIM(App_Mag)) = 'n/a'
					THEN 'n/a'
				WHEN LTRIM(RTRIM(App_Mag)) = 'nl'
					THEN 'nl'
				ELSE NULL
			END AS App_Mag_Flag, 
			[CountStars],
			CASE 
				WHEN LTRIM(RTRIM(CountStars)) = 'nl'
					THEN NULL
				WHEN TRY_CAST(LTRIM(RTRIM(CountStars)) AS INT) IS NOT NULL 
					THEN LTRIM(RTRIM(CountStars))
				WHEN TRY_CAST(LTRIM(RTRIM(CountStars)) AS INT) IS NULL 
					THEN (SELECT STRING_AGG(Value, '')
						  FROM (SELECT Value = SUBSTRING(LTRIM(RTRIM(CountStars)), Number, 1)
								FROM (SELECT LTRIM(RTRIM(CountStars)) AS FirstColumn) AS Source
									 CROSS APPLY (VALUES (1), (2), (3), (4), (5), (6), (7), (8), (9), (0)) AS T(Number)
								WHERE SUBSTRING(LTRIM(RTRIM(CountStars)), Number, 1) LIKE '[0-9.]') AS Filtered
						 )
				ELSE 0 
			END AS CountStars_ToFinding,
			[Ang_Diameter],
			CASE
				WHEN CHARINDEX('x', LTRIM(RTRIM([Ang_Diameter]))) > 0
					THEN LEFT(LTRIM(RTRIM([Ang_Diameter])), CHARINDEX('x', LTRIM(RTRIM([Ang_Diameter]))) - 1)
				WHEN LTRIM(RTRIM(T1.[Ang_Diameter])) = 'n/a' OR LTRIM(RTRIM(T1.[Ang_Diameter])) = 'nl'
					THEN NULL
				WHEN TRY_CAST(LTRIM(RTRIM(T1.[Ang_Diameter])) AS FLOAT) IS NOT NULL
					THEN LTRIM(RTRIM(T1.[Ang_Diameter]))
				ELSE NULL 
			END AS [Ang_Diameter_Max], 
			[Class],
			[Comment]
		FROM CollinderCatalog_Temporarily AS T1;

	END TRY
	BEGIN CATCH
		BEGIN TRY
			PRINT 'BEGIN CATCH';
			SET @FuncProc = ERROR_PROCEDURE();
			SET @Line = ERROR_LINE();
			SET @ErrorNumber = ERROR_NUMBER();
			SET @ErrorSeverity = ERROR_SEVERITY();
			SET @ErrorState = ERROR_STATE();
			SET @ErrorMessage = ERROR_MESSAGE();

			INSERT INTO LogProcFunc (FuncProc, Line, ErrorNumber, ErrorSeverity, ErrorState, ErrorMessage) 
			VALUES (@FuncProc, @Line, @ErrorNumber, @ErrorSeverity, @ErrorState, @ErrorMessage);
			
			SET @FullEerrorMessage = 'An error occurred in GetFilteredCollinderData: ' +
				' Error_number: ' + CAST(@ErrorNumber AS VARCHAR(10)) + 
				' Error_message: ' + CAST(@ErrorMessage AS NVARCHAR(MAX)) +
				' Error_severity: ' + CAST(@ErrorSeverity AS VARCHAR(2)) +
				' Error_state: ' +  CAST(@ErrorState AS VARCHAR(3)) + 
				' Error_line: ' + CAST(@Line AS VARCHAR(10));
			
			THROW 50004, @FullEerrorMessage, 4;
		END TRY
		BEGIN CATCH
			PRINT 'An error occurred during handling error in GetFilteredCollinderData stored procedure: ' + @ErrorMessage;
		END CATCH
	END CATCH
END

