--=======================================================================================================================================--
--==========================================================  STORED PRCEDURE  ==========================================================--
--=======================================================================================================================================--
-------------------------------------------------------------------------------------------------------------------------------------------
--============================================  FILL DATA IN NASAExoplanetCatalogLastUpdate  ============================================--
-------------------------------------------------------------------------------------------------------------------------------------------

CREATE OR ALTER PROC FillNASAExoplanetCatalogLastUpdate  	
AS
BEGIN
    SET NOCOUNT ON; 

    BEGIN TRY 

		IF OBJECT_ID('dbo.NASAExoplanetCatalogLastUpdate', 'U') IS NOT NULL
		BEGIN
			TRUNCATE TABLE NASAExoplanetCatalogLastUpdate;
		END
		 
		
		INSERT INTO NASAExoplanetCatalogLastUpdate (
			Rowid,
			Pl_name,
			Hostname,
			Pl_letter,
			Hd_name,
			Hip_name,
			Tic_id,
			Gaia_id,
			Pl_orbsmax,
			Pl_rade,
			Pl_radj,
			Pl_masse,
			Pl_massj,
			St_spectype,
			St_teff,
			St_rad,
			St_mass,
			St_met,
			St_metratio,
			St_lum,
			St_age,
			Sy_dist,
			LatestDate
		)
		SELECT 
			Rowid,
			Pl_name,
			Hostname,
			Pl_letter,
			Hd_name,
			Hip_name,
			Tic_id,
			Gaia_id,
			Pl_orbsmax,
			Pl_rade,
			Pl_radj,
			Pl_masse,
			Pl_massj,
			St_spectype,
			St_teff,
			St_rad,
			St_mass,
			St_met,
			St_metratio,
			St_lum,
			St_age,
			Sy_dist,
			ld.LatestDate AS LatestDate
		FROM NASAExoplanetCatalog AS NEC
		CROSS APPLY (
			SELECT MAX(dt) AS LatestDate
			FROM (VALUES  (NEC.Disc_pubdate), (NEC.Rowupdate), (NEC.Pl_pubdate), (NEC.Releasedate)) AS AllDates(dt) 
			WHERE dt IS NOT NULL
		) AS ld		

	END TRY
    BEGIN CATCH
        BEGIN TRY
            DECLARE @FuncProcErr AS NVARCHAR(MAX), 
                @Line AS INT, 
                @ErrorNumber AS INT, 
                @ErrorMessage NVARCHAR(MAX),
                @FullEerrorMessage NVARCHAR(MAX),
                @ErrorSeverity INT,
                @ErrorState INT,
                @xstate INT;

            SET @FuncProcErr = ISNULL(ERROR_PROCEDURE(), N'UnknownProcedure');
            SET @Line = ERROR_LINE();
            SET @ErrorNumber = ERROR_NUMBER();
            SET @ErrorSeverity = ERROR_SEVERITY();
            SET @ErrorState = ERROR_STATE();
            SET @ErrorMessage = ERROR_MESSAGE();            
    

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
            DECLARE @SecondErrorMessage NVARCHAR(MAX);
            IF @FullEerrorMessage IS NULL SET @FullEerrorMessage = 'Unknown error occurred and logging also failed.';
            SET @SecondErrorMessage = 
                'An error occurred during handling error in ' + @FuncProcErr + ' stored procedure: ' + @FullEerrorMessage;
            
            THROW 52000, @SecondErrorMessage, 0;
        END CATCH
    END CATCH
END
