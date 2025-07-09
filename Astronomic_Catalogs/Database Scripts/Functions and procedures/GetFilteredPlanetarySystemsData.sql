--=======================================================================================================================================--
--==========================================================  STORED PRCEDURE  ==========================================================--
--=======================================================================================================================================--
-------------------------------------------------------------------------------------------------------------------------------------------
--================================================  GET DATA FROM NASAExoplanetCatalog   ================================================--
-------------------------------------------------------------------------------------------------------------------------------------------

CREATE OR ALTER PROC GetFilteredPlanetarySystemsData
	@PlanetType NVARCHAR(MAX) = NULL, 
	@Name NVARCHAR(MAX) = NULL,
	@PlenetsCountFrom INT = NULL,
	@PlenetsCountTo INT = NULL, 
	@OrderBy INT = NULL, 
	@HabitableZone BIT = NULL,
	@TerrestrialHabitableZone BIT = NULL,
	@SyDistFrom FLOAT = NULL,
	@SyDistTo FLOAT = NULL,
	@PageNumber INT = 1,
    @RowOnPage INT = 10
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


			IF @OrderBy IS NOT NULL AND @OrderBy NOT IN (0, 1, 2, 3, 4, 5, 6)
			BEGIN
				RAISERROR('Invalid value for @OrderBy. Allowed values: 0 - 6.', 16, 1);
				RETURN;
			END


			IF @PlenetsCountFrom IS NOT NULL AND @PlenetsCountFrom < 0
			BEGIN
				RAISERROR('@PlenetsCountFrom must be >= 0.', 16, 1);
				RETURN;
			END


			IF @PlenetsCountTo IS NOT NULL AND @PlenetsCountTo < 0
			BEGIN
				RAISERROR('@PlenetsCountTo must be >= 0.', 16, 1);
				RETURN;
			END


			IF @PlenetsCountFrom IS NOT NULL AND @PlenetsCountTo IS NOT NULL AND @PlenetsCountFrom > @PlenetsCountTo
			BEGIN
				RAISERROR('@PlenetsCountFrom cannot be greater than @PlenetsCountTo.', 16, 1);
				RETURN;
			END


			IF @SyDistFrom IS NOT NULL AND @SyDistTo IS NOT NULL AND @SyDistFrom > @SyDistTo
			BEGIN
				RAISERROR('@SyDistFrom cannot be greater than @SyDistTo.', 16, 1);
				RETURN;
			END
						

			IF @HabitableZone IS NOT NULL AND @HabitableZone NOT IN (0, 1)
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
		SET @PageNumber = ISNULL(@PageNumber, 1);
		SET @RowOnPage = ISNULL(@RowOnPage, 10);
		

		
		-- Parsing the input JSON parameters into a tables.
		DECLARE @PlanetTypes TABLE (TypeId INT);
		IF ISJSON(@PlanetType) = 1
		BEGIN
			INSERT INTO @PlanetTypes (TypeId)
			SELECT TRY_CAST([value] AS INT)
			FROM OPENJSON(@PlanetType)
			WHERE TRY_CAST([value] AS INT) BETWEEN 1 AND 9;
		END;

		SELECT 
			S.Hostname,
			S.Hd_name,
			S.Hip_name,
			S.Tic_id,
			S.Gaia_id,
			S.St_spectype,
			S.St_teff,
			S.St_rad,
			S.St_mass,
			S.St_met,
			S.St_metratio,
			S.St_lum,
			S.St_age,
			S.Sy_dist,
			S.St_lum_Sun_Absol,
			S.HabitablZone,

			P.Pl_name,
			P.StarId,
			P.Pl_letter,
			P.Pl_orbsmax,
			P.Pl_rade,
			P.Pl_radj,
			P.Pl_masse,
			P.Pl_massj,
			P.LatestDate,
			P.InHabitablZone,
			COUNT(*) OVER (PARTITION BY S.Hostname) AS PlanetsCount
		INTO #PlanetWithCounts
		FROM dbo.NASAPlanetarySystemsPlanets AS P
		INNER JOIN dbo.NASAPlanetarySystemsStars AS S ON P.StarId = S.Id;
		
		CREATE NONCLUSTERED INDEX IX_PlanetWithCounts_Hostname ON #PlanetWithCounts(Hostname);



		SELECT DISTINCT pwc.Hostname
		INTO #ValidHostnames 
		FROM #PlanetWithCounts pwc
		OUTER APPLY (
			SELECT 
				MAX(CASE WHEN ISNULL(p.Pl_orbsmax, 0) <> 0 THEN 1 END) AS HasOrbit,
				MAX(CASE WHEN ISNULL(p.Pl_rade, 0) <> 0 THEN 1 END) AS HasRade,
				MAX(CASE WHEN p.InHabitablZone = 1 THEN 1 END) AS HasHabitable,
				MAX(CASE WHEN p.InHabitablZone = 1 AND p.Pl_rade BETWEEN 0.27 AND 1.2 THEN 1 END) AS HasTerrestrialHZ,
				MAX(CASE WHEN p.Sy_dist >= ISNULL(@SyDistFrom, -9999) THEN 1 END) AS HasSyDistFrom,
				MAX(CASE WHEN p.Sy_dist <= ISNULL(@SyDistTo, 9999999) THEN 1 END) AS HasSyDistTo,
				MAX(CASE WHEN Pl_rade BETWEEN 0.04 AND 50 THEN Pl_rade END) AS SampleRade,
				COUNT(*) AS SystemPlanetCount
			FROM #PlanetWithCounts p
			WHERE p.Hostname = pwc.Hostname
		) ap
		WHERE 
			(@PlanetType IS NULL OR @PlanetType = '[]' OR ap.HasRade = 1)
			AND (@OrderBy NOT IN (1,2,3,4) OR ap.HasOrbit = 1)
			AND (@OrderBy NOT IN (5,6) OR ap.HasRade = 1)
			AND (
				@PlanetType IS NULL OR @PlanetType = '[]' OR NOT EXISTS (SELECT 1 FROM @PlanetTypes)
				OR EXISTS (
					SELECT 1 
					FROM @PlanetTypes pt
					WHERE
						(pt.TypeId = 1 AND ap.SampleRade BETWEEN 0.04 AND 0.27) OR
						(pt.TypeId = 2 AND ap.SampleRade > 0.27 AND ap.SampleRade <= 0.7) OR
						(pt.TypeId = 3 AND ap.SampleRade > 0.7  AND ap.SampleRade <= 1.2) OR
						(pt.TypeId = 4 AND ap.SampleRade > 1.2  AND ap.SampleRade <= 1.9) OR
						(pt.TypeId = 5 AND ap.SampleRade > 1.9  AND ap.SampleRade <= 3.1) OR
						(pt.TypeId = 6 AND ap.SampleRade > 3.1  AND ap.SampleRade <= 5.1) OR
						(pt.TypeId = 7 AND ap.SampleRade > 5.1  AND ap.SampleRade <= 8.3) OR
						(pt.TypeId = 8 AND ap.SampleRade > 8.3  AND ap.SampleRade <= 13.7) OR
						(pt.TypeId = 9 AND ap.SampleRade > 13.7 AND ap.SampleRade <= 50)
				)
			)
			AND (
				@Name IS NULL OR
				pwc.Hostname LIKE '%' + @Name + '%' OR
				pwc.Hd_name LIKE '%' + @Name + '%' OR
				pwc.Hip_name LIKE '%' + @Name + '%' OR
				pwc.Tic_id LIKE '%' + @Name + '%' OR
				pwc.Gaia_id LIKE '%' + @Name + '%'
			)
			AND (
				@PlenetsCountFrom IS NULL OR ap.SystemPlanetCount >= @PlenetsCountFrom
			)
			AND (
				@PlenetsCountTo IS NULL OR ap.SystemPlanetCount <= @PlenetsCountTo
			)
			AND (
				@HabitableZone IS NULL OR @HabitableZone = 0 OR ap.HasHabitable = 1
			)
			AND (
				@TerrestrialHabitableZone IS NULL OR @TerrestrialHabitableZone = 0 OR ap.HasTerrestrialHZ = 1
			)
			AND (
				(@SyDistFrom IS NULL OR ap.HasSyDistFrom = 1) 
				AND	(@SyDistTo IS NULL OR ap.HasSyDistTo = 1)
			);

		CREATE NONCLUSTERED INDEX IX_ValidHostnames_Hostname ON #ValidHostnames(Hostname);



		-- Aggregated values for sorting (excluding nulls)
		SELECT
			Hostname,
			MIN(NULLIF(Pl_orbsmax, 0)) AS MinPlOrbsmax,
			MAX(NULLIF(Pl_orbsmax, 0)) AS MaxPlOrbsmax,
			MIN(NULLIF(Pl_rade, 0)) AS MinPlRade,
			MAX(NULLIF(Pl_rade, 0)) AS MaxPlRade
		INTO #SystemAggregates
		FROM #PlanetWithCounts
		GROUP BY Hostname;
		

		SELECT 
			pwc.*, 
			DENSE_RANK() OVER (ORDER BY pwc.Hostname) AS SystemCount,
			CASE @OrderBy
				WHEN 1 THEN ISNULL(sa.MinPlOrbsmax, 9999999)
				WHEN 2 THEN ISNULL(sa.MaxPlOrbsmax, 9999999)
				WHEN 3 THEN ISNULL(-1 * sa.MinPlOrbsmax, -9999999)
				WHEN 4 THEN ISNULL(-1 * sa.MaxPlOrbsmax, -9999999)
				WHEN 5 THEN ISNULL(sa.MinPlRade, 9999999)
				WHEN 6 THEN ISNULL(sa.MaxPlRade, 9999999)
				ELSE NULL
			END AS SortKey
		INTO #SummaryTable
		FROM #PlanetWithCounts pwc
		INNER JOIN #SystemAggregates sa ON pwc.Hostname = sa.Hostname
		INNER JOIN #ValidHostnames vh ON pwc.Hostname = vh.Hostname;

		
		-- Paggination		
		SET @TotalCount = (SELECT TOP 1 SystemCount FROM #SummaryTable ORDER BY SystemCount DESC);
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
			SystemCount,
			Hostname, 
			Hd_name as HdName, 
			Hip_name as HipName, 
			Tic_id as TicId, 
			Gaia_id as GaiaId, 
			St_spectype as StSpectype, 
			St_teff as StTeff, 
			St_rad as StRad, 
			St_mass as StMass, 
			St_met as StMet, 
			St_metratio as StMetratio, 
			St_lum as StLum, 
			St_age as StAge, 
			Sy_dist as SyDist, 
			St_lum_Sun_Absol as StLumSunAbsol, 
			HabitablZone as HabitablZone, 
			Pl_letter as PlLetter, 
			Pl_orbsmax as PlOrbsmax, 
			Pl_rade as PlRade, 
			Pl_radj as PlRadJ, 
			Pl_masse as PlMasse, 
			Pl_massj as PlMassJ, 
			RowOnPage = @TotalCount 
		FROM #SummaryTable
		WHERE SystemCount BETWEEN @Offset + 1 AND (@Offset + @RowOnPage) 
		ORDER BY SortKey, Hostname, Pl_name;
		

		DROP TABLE #PlanetWithCounts;
		DROP TABLE #ValidHostnames;
		DROP TABLE #SystemAggregates;
		DROP TABLE #SummaryTable;


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
