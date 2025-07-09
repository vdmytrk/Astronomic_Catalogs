--=======================================================================================================================================--
--==========================================================  STORED PRCEDURE  ==========================================================--
--=======================================================================================================================================--
-------------------------------------------------------------------------------------------------------------------------------------------
--===========================  FILL DATA IN NASAPlanetarySystemsStars AND NASAPlanetarySystemsPlanets TABLES  ===========================--
-------------------------------------------------------------------------------------------------------------------------------------------

CREATE OR ALTER PROC CalculationPlanetarySystemData 
AS
BEGIN		
    DECLARE @FuncProc AS NVARCHAR(100) = 'CalculationPlanetarySystemData'; 
    DECLARE @StartedTran BIT = 0;

    SET NOCOUNT ON; 

    -- 1. Clearing target tables
	IF OBJECT_ID('dbo.NASAPlanetarySystemsPlanets', 'U') IS NOT NULL
		OR OBJECT_ID('dbo.NASAPlanetarySystemsStars', 'U') IS NOT NULL
	BEGIN
		EXEC Recreate_NASAPlanetarySystems_Tables;
	END;

	BEGIN TRY
	    IF @@TRANCOUNT = 0
        BEGIN                
            BEGIN TRANSACTION; 
            SET @StartedTran = 1;
        END;
        ELSE IF @@TRANCOUNT > 0 
        BEGIN
            DECLARE @ErrorMsg NVARCHAR(200) = 'Procedure ' + @FuncProc + ' cannot be executed inside another open transaction.';        
            THROW 50001, @ErrorMsg, 1;
        END;
		
		-- 2. Inserting unique stars into the NASAPlanetarySystemsStars table
		INSERT INTO NASAPlanetarySystemsStars (
			Hostname,
			Hd_name,
			Hip_name,
			Tic_id,
			Gaia_id,
			St_spectype,
			St_teff,
			St_rad,
			St_mass,
			St_met,
			St_metratio,
			St_lum,
			St_age,
			Sy_dist,
			St_lum_Sun_Absol,
			HabitablZone
		)
		SELECT
			Hostname,
			MAX(Hd_name),
			MAX(Hip_name),
			MAX(Tic_id),
			MAX(Gaia_id),
			MAX(St_spectype),
			MAX(St_teff),
			MAX(St_rad),
			MAX(St_mass),
			MAX(St_met),
			MAX(St_metratio),
			MAX(St_lum),
			MAX(St_age),
			MAX(Sy_dist),
			CAST(POWER(CAST(10.0 AS decimal(18,10)), CAST(MAX(St_lum) AS decimal(18,10))) AS decimal(18,10)) AS St_lum_Sun_Absol, 
			CAST(POWER(CAST(10.0 AS decimal(18,10)), CAST(MAX(St_lum) AS decimal(18,10)) / 2.0) * 1.26 AS decimal(18,10)) AS HabitablZone 
		FROM NASAExoplanetCatalogUniquePlanets
		WHERE Hostname IN (
			SELECT DISTINCT Hostname
			FROM NASAExoplanetCatalogUniquePlanets
			WHERE Pl_rade IS NOT NULL OR Pl_radj IS NOT NULL
		)
		GROUP BY Hostname;

		-- 3. Inserting planets into the NASAPlanetarySystemsPlanets table
		INSERT INTO NASAPlanetarySystemsPlanets (
			Pl_name,
			StarId,
			Pl_letter,
			Pl_orbsmax,
			Pl_rade,
			Pl_radj,
			Pl_masse,
			Pl_massj,
			LatestDate,
			InHabitablZone
		)
		SELECT
			P.Pl_name,
			S.Id AS StarId,
			P.Pl_letter,
			P.Pl_orbsmax,
			P.Pl_rade,
			P.Pl_radj,
			P.Pl_masse,
			P.Pl_massj,
			P.LatestDate,
			CASE  
				WHEN S.St_lum != 0 
						AND P.Pl_orbsmax BETWEEN HZ.HZ_Inner AND HZ.HZ_Outer					 
				THEN 1
				ELSE 0
			END AS InHabitablZone
		FROM NASAExoplanetCatalogUniquePlanets P
		INNER JOIN NASAPlanetarySystemsStars S ON P.Hostname = S.Hostname
		CROSS APPLY (
			SELECT
				CAST(POWER(CAST(10.0 AS decimal(18,10)), CAST(S.St_lum AS decimal(18,10)) / 2.0) * 0.75 AS decimal(18,10)) AS HZ_Inner,
				CAST(POWER(CAST(10.0 AS decimal(18,10)), CAST(S.St_lum AS decimal(18,10)) / 2.0) * 1.77 AS decimal(18,10)) AS HZ_Outer
		) AS HZ
		WHERE EXISTS (
			SELECT 1
			FROM NASAExoplanetCatalogUniquePlanets X
			WHERE X.Hostname = P.Hostname
				AND (X.Pl_rade IS NOT NULL OR X.Pl_radj IS NOT NULL)
		);	
		
        IF @StartedTran = 1 AND XACT_STATE() = 1
        BEGIN
			COMMIT TRANSACTION;
        END;

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
        
                ROLLBACK TRANSACTION;
            END;
            IF @xstate = 1 AND @StartedTran = 1
            BEGIN
                SET @RollebackMassage = N'ROLLBACK TRANSACTION STATE: @xstate = ' + CAST(@xstate AS VARCHAR) + 
				        N' AND @StartedTran = ' + CAST(@StartedTran AS VARCHAR);
        
                INSERT INTO LogProcFunc (FuncProc, ErrorSeverity, ErrorState, ErrorMessage) 
                VALUES (@FuncProc, 0, 0, @RollebackMassage);

                ROLLBACK TRANSACTION;
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
            'An error occurred during handling error in ' + @FuncProc + ' stored procedure: ' + @FullEerrorMessage;

            THROW 52000, @FullEerrorMessage, 0;
        END CATCH
	END CATCH
END


