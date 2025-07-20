--=======================================================================================================================================--
--==========================================================  STORED PRCEDURE  ==========================================================--
--=======================================================================================================================================--
-------------------------------------------------------------------------------------------------------------------------------------------
--================================================  GET DATA FROM NASAExoplanetCatalog   ================================================--
-------------------------------------------------------------------------------------------------------------------------------------------

CREATE OR ALTER PROC GetFilteredPlanetsData
	@PlanetWithSize BIT = 0, 
	@PlanetType NVARCHAR(MAX) = NULL, 
	@Telescope NVARCHAR(MAX) = NULL,
	@PlanetName NVARCHAR(MAX) = NULL,
	@Name NVARCHAR(MAX) = NULL,
	@DiscoveryMethod NVARCHAR(MAX) = NULL,	
	@DateFrom DATETIME = '1992-01-01',
	@DateTo DATETIME = GETDATE,
	@HabitableZone BIT = NULL,
	@TerrestrialHabitableZone BIT = NULL,
	@DistanceToStarFrom FLOAT = NULL,
	@DistanceToStarTo FLOAT = NULL,
	@PageNumber INT = 1,
    @RowOnPage INT = 50
AS
BEGIN	

	DECLARE @TotalCount INT = 0, @PageCountInResult INT = 0, @Offset INT = 0;
	
    SET NOCOUNT ON;

	BEGIN TRY
		-- Parameter validation		
		DECLARE @InvalidPattern  NVARCHAR(100) = '%[^A-Za-z0-9 \-_,.''`]%';

		BEGIN
			IF @PlanetType IS NOT NULL AND @PlanetType != '[]'
			BEGIN
				IF EXISTS (
					SELECT [value]
					FROM OPENJSON(@PlanetType)
					WHERE TRY_CAST([value] AS INT) IS NULL OR TRY_CAST([value] AS INT) NOT BETWEEN 1 AND 9
				)
				BEGIN
					RAISERROR('Invalid value in @PlanetType. Allowed values are integers from 1 to 9 (as a JSON array).', 16, 1);
					RETURN;
				END
			END
			

			IF @Telescope IS NOT NULL AND @Telescope != '[]'
			BEGIN
				IF EXISTS (
					SELECT [value]
					FROM OPENJSON(@Telescope)
					WHERE 
						ISJSON([value]) = 1
						OR TRY_CAST([value] AS NVARCHAR(100)) IS NULL
						OR [value] LIKE @InvalidPattern ESCAPE '\'
				)
				BEGIN
					RAISERROR('Invalid value in @Telescope. All elements must be valid strings in a JSON array.', 16, 1);
					RETURN;
				END
			END
			

			IF @DiscoveryMethod IS NOT NULL AND @DiscoveryMethod != '[]'
			BEGIN
				IF EXISTS (
					SELECT [value]
					FROM OPENJSON(@DiscoveryMethod)
					WHERE 
						ISJSON([value]) = 1
						OR TRY_CAST([value] AS NVARCHAR(100)) IS NULL
						OR [value] LIKE @InvalidPattern ESCAPE '\'
				)
				BEGIN
					RAISERROR('Invalid value in @DiscoveryMethod. All elements must be valid strings in a JSON array.', 16, 1);
					RETURN;
				END
			END
			

			IF @PlanetName IS NOT NULL AND @PlanetName != '[]'
			BEGIN
				IF EXISTS (
					SELECT [value]
					FROM OPENJSON(@PlanetName)
					WHERE 
						ISJSON([value]) = 1
						OR TRY_CAST([value] AS NVARCHAR(100)) IS NULL
			            OR LEN([value]) != 1
						OR [value] NOT LIKE '[a-z]' 
				)
				BEGIN
					RAISERROR('Invalid value in @PlanetName. All elements must be sinlge lowercase letter in a JSON array.', 16, 1);
					RETURN;
				END
			END


			IF @DateFrom IS NOT NULL AND @DateTo IS NOT NULL AND @DateFrom > @DateTo
			BEGIN
				RAISERROR('Invalid date range.', 16, 1);
				RETURN;
			END
			

			IF (@DistanceToStarFrom IS NOT NULL AND @DistanceToStarFrom < 0)
			   OR (@DistanceToStarTo IS NOT NULL AND @DistanceToStarTo < 0)
			   OR (@DistanceToStarFrom IS NOT NULL AND @DistanceToStarTo IS NOT NULL AND @DistanceToStarFrom > @DistanceToStarTo)
			BEGIN
				RAISERROR('Invalid distance range to star.', 16, 1);
				RETURN;
			END


			IF @PlanetWithSize NOT IN (0, 1)
				OR (@HabitableZone IS NOT NULL AND @HabitableZone NOT IN (0, 1))
				OR (@TerrestrialHabitableZone IS NOT NULL AND @TerrestrialHabitableZone NOT IN (0, 1))
			BEGIN
				RAISERROR('Invalid value for one of the BIT parameters.', 16, 1);
				RETURN;
			END


			IF @Name IS NOT NULL AND @Name LIKE @InvalidPattern  ESCAPE '\'
			BEGIN
				RAISERROR('Invalid characters in @Name.', 16, 1);
				RETURN;
			END

		END


		-- Default value blok
		SET @PlanetWithSize = ISNULL(@PlanetWithSize, 0)		
		SET @DateFrom = ISNULL(@DateFrom, '1992-01-01'); 
		SET @DateTo = ISNULL(@DateTo, GETDATE()); 
		SET @PageNumber = ISNULL(@PageNumber, 1);
		SET @RowOnPage = ISNULL(@RowOnPage, 50);


		-- Parsing the input JSON parameters into a tables.
		DECLARE @PlanetTypes TABLE (TypeId INT);
		IF ISJSON(@PlanetType) = 1
		BEGIN
			INSERT INTO @PlanetTypes (TypeId)
			SELECT TRY_CAST([value] AS INT)
			FROM OPENJSON(@PlanetType)
			WHERE TRY_CAST([value] AS INT) BETWEEN 1 AND 9;
		END;	
		

		DECLARE @Telescopes TABLE (Name NVARCHAR(100));
		IF ISJSON(@Telescope) = 1
		BEGIN
			INSERT INTO @Telescopes (Name)
			SELECT [value]
			FROM OPENJSON(@Telescope)
			WHERE ISJSON([value]) = 0
				  AND TRY_CAST([value] AS NVARCHAR(100)) IS NOT NULL;
		END;
		

		DECLARE @PlanetNames TABLE (Name NVARCHAR(1));
		IF ISJSON(@PlanetName) = 1
		BEGIN
			INSERT INTO @PlanetNames (Name)
			SELECT [value]
			FROM OPENJSON(@PlanetName)
			WHERE ISJSON([value]) = 0
				  AND TRY_CAST([value] AS NVARCHAR(100)) IS NOT NULL;
		END;	
		


		DECLARE @DiscoveryMethods TABLE (Name NVARCHAR(100));
		IF ISJSON(@DiscoveryMethod) = 1
		BEGIN
			INSERT INTO @DiscoveryMethods (Name)
			SELECT [value]
			FROM OPENJSON(@DiscoveryMethod)
			WHERE ISJSON([value]) = 0 
				  AND TRY_CAST([value] AS NVARCHAR(100)) IS NOT NULL;
		END;	
		

		-- =====================================================================================

		SELECT 
			EC.*,			
			S.HabitablZone,
			P.LatestDate,
			P.InHabitablZone,
			COUNT(*) OVER (PARTITION BY EC.Pl_name) AS PlanetCount
		INTO #SummaryTable
		FROM dbo.NASAExoplanetCatalog AS EC
		LEFT JOIN dbo.NASAPlanetarySystemsPlanets AS P ON EC.Pl_name = P.Pl_name
		LEFT JOIN dbo.NASAPlanetarySystemsStars AS S ON P.StarId = S.Id 
		WHERE 
			(
				@PlanetWithSize IS NULL OR @PlanetWithSize = 0
				OR (
					(
						(ISNULL(P.Pl_rade, 0) > 0 OR ISNULL(P.Pl_radj, 0) > 0)
					)
					AND (
						@PlanetType IS NULL OR @PlanetType = '[]'
						OR EXISTS (
							SELECT 1 FROM @PlanetTypes pt WHERE
							(pt.TypeId = 1 AND P.Pl_rade BETWEEN 0.04 AND 0.27) OR
							(pt.TypeId = 2 AND P.Pl_rade >  0.27 AND P.Pl_rade <= 0.7) OR
							(pt.TypeId = 3 AND P.Pl_rade >  0.7  AND P.Pl_rade <= 1.2) OR
							(pt.TypeId = 4 AND P.Pl_rade >  1.2  AND P.Pl_rade <= 1.9) OR
							(pt.TypeId = 5 AND P.Pl_rade >  1.9  AND P.Pl_rade <= 3.1) OR
							(pt.TypeId = 6 AND P.Pl_rade >  3.1  AND P.Pl_rade <= 5.1) OR
							(pt.TypeId = 7 AND P.Pl_rade >  5.1  AND P.Pl_rade <= 8.3) OR
							(pt.TypeId = 8 AND P.Pl_rade >  8.3  AND P.Pl_rade <= 13.7) OR
							(pt.TypeId = 9 AND P.Pl_rade > 13.7  AND P.Pl_rade <= 50)
						)
					)
				)
			)
			AND 
			(
				@Telescope IS NULL 
				OR EXISTS (
					SELECT 1 FROM @Telescopes T	WHERE T.Name = EC.Disc_telescope
				)
			)
			AND (
				@PlanetName IS NULL
				OR EXISTS (
					SELECT 1 FROM @PlanetNames T WHERE T.Name = EC.Pl_letter
				)
			)
			AND (
				@Name IS NULL
				OR EC.Hostname LIKE '%' + @Name + '%'
				OR EC.Hd_name LIKE '%' + @Name + '%'
				OR EC.Hip_name LIKE '%' + @Name + '%'
				OR EC.Tic_id LIKE '%' + @Name + '%'
				OR EC.Gaia_id LIKE '%' + @Name + '%'
			)
			AND (
				@DiscoveryMethod IS NULL
				OR EXISTS (
					SELECT 1 FROM @DiscoveryMethods T WHERE T.Name = EC.Discoverymethod
				)
			)
			AND (@DateFrom IS NULL OR EC.Disc_pubdate >= @DateFrom)
			AND (@DateTo IS NULL OR EC.Disc_pubdate <= @DateTo)
			AND (
				@HabitableZone IS NULL OR @HabitableZone = 0
				OR P.InHabitablZone = @HabitableZone
			)
			AND (
				@TerrestrialHabitableZone IS NULL OR @TerrestrialHabitableZone = 0
				OR (
					P.InHabitablZone = 1 AND P.Pl_rade BETWEEN 0.27 AND 1.2
				)
			)
			AND (@DistanceToStarFrom IS NULL OR EC.Pl_orbsmax >= @DistanceToStarFrom)
			AND (@DistanceToStarTo IS NULL OR EC.Pl_orbsmax <= @DistanceToStarTo);
					   		

		
		-- Paggination
		SELECT @TotalCount = COUNT(*) FROM #SummaryTable;
		SET @PageCountInResult = CEILING(1.0 * @TotalCount / @RowOnPage);

		IF @PageCountInResult = 0
			BEGIN
				SET @Offset = 0;
				SET @PageNumber = 0;
			END
		ELSE IF @PageNumber <= @PageCountInResult
			BEGIN
				SET @Offset = (@PageNumber - 1) * @RowOnPage;
			END
		ELSE 
			BEGIN -- To return the last available page.
				SET @Offset = (@PageCountInResult - 1) * @RowOnPage; 
				SET @PageNumber = @PageCountInResult;
			END


			
		SELECT 
			RowId, Pl_name, Hostname, Pl_letter, Hd_name, Hip_name, Tic_id, Gaia_id, Default_flag, Sy_snum, Sy_pnum, Sy_mnum, Cb_flag, 
			Discoverymethod, Disc_year, Disc_refname, Disc_pubdate, Disc_locale, Disc_facility, Disc_telescope, 
			Disc_instrument, Rv_flag, Pul_flag, Ptv_flag, Tran_flag, Ast_flag, Obm_flag, Micro_flag, Etv_flag, Ima_flag, Dkin_flag, Soltype, 
			Pl_controv_flag, Pl_refname, Pl_orbper, Pl_orbpererr1, Pl_orbpererr2, Pl_orbperlim, Pl_orbsmax, 
			Pl_orbsmaxerr1, Pl_orbsmaxerr2, Pl_orbsmaxlim, Pl_rade, Pl_radeerr1, Pl_radeerr2, Pl_radelim, Pl_radj, Pl_radjerr1, Pl_radjerr2, Pl_radjlim, 
			Pl_masse, Pl_masseerr1, Pl_masseerr2, Pl_masselim, Pl_massj, Pl_massjerr1, Pl_massjerr2, Pl_massjlim, Pl_msinie, 
			Pl_msinieerr1, Pl_msinieerr2, Pl_msinielim, Pl_msinij, Pl_msinijerr1, Pl_msinijerr2, Pl_msinijlim, Pl_cmasse, 
			Pl_cmasseerr1, Pl_cmasseerr2, Pl_cmasselim, Pl_cmassj, Pl_cmassjerr1, Pl_cmassjerr2, Pl_cmassjlim, Pl_bmasse, 
			Pl_bmasseerr1, Pl_bmasseerr2, Pl_bmasselim, Pl_bmassj, Pl_bmassjerr1, Pl_bmassjerr2, Pl_bmassjlim, Pl_bmassprov, 
			Pl_dens, Pl_denserr1, Pl_denserr2, Pl_denslim, Pl_orbeccen, Pl_orbeccenerr1, Pl_orbeccenerr2, Pl_orbeccenlim, Pl_insol, Pl_insolerr1, 
			Pl_insolerr2, Pl_insollim, Pl_eqt, Pl_eqterr1, Pl_eqterr2, Pl_eqtlim, Pl_orbincl, Pl_orbinclerr1, Pl_orbinclerr2, 
			Pl_orbincllim, Pl_tranmid, Pl_tranmiderr1, Pl_tranmiderr2, Pl_tranmidlim, Pl_tsystemref, Ttv_flag, Pl_imppar, 
			Pl_impparerr1, Pl_impparerr2, Pl_impparlim, Pl_trandep, Pl_trandeperr1, Pl_trandeperr2, Pl_trandeplim, Pl_trandur, 
			Pl_trandurerr1, Pl_trandurerr2, Pl_trandurlim, Pl_ratdor, Pl_ratdorerr1, Pl_ratdorerr2, Pl_ratdorlim, Pl_ratror, 
			Pl_ratrorerr1, Pl_ratrorerr2, Pl_ratrorlim, Pl_occdep, Pl_occdeperr1, Pl_occdeperr2, Pl_occdeplim, Pl_orbtper, 
			Pl_orbtpererr1, Pl_orbtpererr2, Pl_orbtperlim, Pl_orblper, Pl_orblpererr1, Pl_orblpererr2, Pl_orblperlim, Pl_rvamp, 
			Pl_rvamperr1, Pl_rvamperr2, Pl_rvamplim, Pl_projobliq, Pl_projobliqerr1, Pl_projobliqerr2, Pl_projobliqlim, Pl_trueobliq,
			Pl_trueobliqerr1, Pl_trueobliqerr2, Pl_trueobliqlim, St_refname, St_spectype, St_teff, St_tefferr1, St_tefferr2, St_tefflim, 
			St_rad, St_raderr1, St_raderr2, St_radlim, St_mass, St_masserr1, St_masserr2, St_masslim, St_met, St_meterr1, St_meterr2, 
			St_metlim, St_metratio, St_lum, St_lumerr1, St_lumerr2, St_lumlim, St_logg, St_loggerr1, St_loggerr2, St_logglim, St_age, 
			St_ageerr1, St_ageerr2, St_agelim, St_dens, St_denserr1, St_denserr2, St_denslim, St_vsin, St_vsinerr1, St_vsinerr2, St_vsinlim, 
			St_rotp, St_rotperr1, St_rotperr2, St_rotplim, St_radv, St_radverr1, St_radverr2, St_radvlim, Sy_refname, Rastr, Ra, Decstr, Dec, 
			Glat, Glon, Elat, Elon, Sy_pm, Sy_pmerr1, Sy_pmerr2, Sy_pmra, Sy_pmraerr1, Sy_pmraerr2, Sy_pmdec, Sy_pmdecerr1, 
			Sy_pmdecerr2, Sy_dist, Sy_disterr1, Sy_disterr2, Sy_plx, Sy_plxerr1, Sy_plxerr2, Sy_bmag, Sy_bmagerr1, Sy_bmagerr2, Sy_vmag, 
			Sy_vmagerr1, Sy_vmagerr2, Sy_jmag, Sy_jmagerr1, Sy_jmagerr2, Sy_hmag, Sy_hmagerr1, Sy_hmagerr2, Sy_kmag, Sy_kmagerr1, Sy_kmagerr2, 
			Sy_umag, Sy_umagerr1, Sy_umagerr2, Sy_gmag, Sy_gmagerr1, Sy_gmagerr2, Sy_rmag, Sy_rmagerr1, Sy_rmagerr2, Sy_imag, Sy_imagerr1, 
			Sy_imagerr2, Sy_zmag, Sy_zmagerr1, Sy_zmagerr2, Sy_w1mag, Sy_w1magerr1, Sy_w1magerr2, Sy_w2mag, Sy_w2magerr1, Sy_w2magerr2, 
			Sy_w3mag, Sy_w3magerr1, Sy_w3magerr2, Sy_w4mag, Sy_w4magerr1, Sy_w4magerr2, Sy_gaiamag, Sy_gaiamagerr1, 
			Sy_gaiamagerr2, Sy_icmag, Sy_icmagerr1, Sy_icmagerr2, Sy_tmag, Sy_tmagerr1, Sy_tmagerr2, Sy_kepmag, Sy_kepmagerr1, 
			Sy_kepmagerr2, Rowupdate, Pl_pubdate, Releasedate, Pl_nnotes, St_nphot, St_nrvc, St_nspec, Pl_nespec, Pl_ntranspec, Pl_ndispec, 
			HabitablZone, LatestDate, InHabitablZone, PlanetCount, 
			RowOnPage = @TotalCount 
		FROM #SummaryTable AS TT
		ORDER BY Pl_name 
		OFFSET @Offset ROWS FETCH NEXT @RowOnPage ROWS ONLY;

		DROP TABLE IF EXISTS #SummaryTable;

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