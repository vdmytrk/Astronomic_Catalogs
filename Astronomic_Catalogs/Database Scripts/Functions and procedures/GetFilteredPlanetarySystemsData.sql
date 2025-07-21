--=======================================================================================================================================--
--==========================================================  STORED PRCEDURE  ==========================================================--
--=======================================================================================================================================--
-------------------------------------------------------------------------------------------------------------------------------------------
--================================================  GET DATA FROM NASAExoplanetCatalog   ================================================--
-------------------------------------------------------------------------------------------------------------------------------------------

CREATE OR ALTER PROC GetFilteredPlanetarySystemsData
	@PlanetWithSize BIT = 0,
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


			IF @PlanetWithSize NOT IN (0, 1)
                OR @HabitableZone IS NOT NULL AND @HabitableZone NOT IN (0, 1)
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
        SET @PlanetWithSize = ISNULL(@PlanetWithSize, 0);
		SET @PageNumber = ISNULL(@PageNumber, 1);
		SET @RowOnPage = ISNULL(@RowOnPage, 10);
		SET @OrderBy = ISNULL(@OrderBy, 0);
		

		
		DECLARE @PlanetTypes TABLE (TypeId INT);
		IF ISJSON(@PlanetType) = 1
		BEGIN
			INSERT INTO @PlanetTypes (TypeId)
			SELECT TRY_CAST([value] AS INT)
			FROM OPENJSON(@PlanetType);
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
			COUNT(*) OVER (PARTITION BY S.Hostname) AS SystemPlanetCount
		INTO #PlanetWithCount
		FROM dbo.NASAPlanetarySystemsPlanets AS P
		INNER JOIN dbo.NASAPlanetarySystemsStars AS S ON P.StarId = S.Id;
		
		CREATE NONCLUSTERED INDEX IX_PlanetWithCounts_Hostname ON #PlanetWithCount(Hostname);


		
		SELECT DISTINCT pwc.Hostname
		INTO #ValidHostname
		FROM #PlanetWithCount pwc
		OUTER APPLY (
			SELECT 
				MAX(CASE WHEN p.Pl_orbsmax IS NOT NULL AND p.Pl_orbsmax > 0 THEN 1 ELSE 0 END) AS HasOrbit,
				MAX(CASE WHEN p.Pl_rade IS NOT NULL AND p.Pl_rade > 0 THEN 1 ELSE 0 END) AS HasRade,
				MAX(CASE WHEN p.InHabitablZone = 1 THEN 1 ELSE 0 END) AS HasHabitable,
				MAX(CASE WHEN p.InHabitablZone = 1 AND p.Pl_rade BETWEEN 0.27 AND 1.2 THEN 1 ELSE 0 END) AS HasTerrestrialHZ,
				MAX(CASE WHEN @SyDistFrom IS NOT NULL AND p.Sy_dist >= @SyDistFrom THEN 1 ELSE 0 END) AS HasSyDistFrom,
				MAX(CASE WHEN @SyDistTo IS NULL THEN 1 WHEN p.Sy_dist <= @SyDistTo THEN 1 ELSE 0 END) AS HasSyDistTo
			FROM #PlanetWithCount p
			WHERE p.Hostname = pwc.Hostname
		) oa
		WHERE
			(@PlanetWithSize = 0 OR oa.HasRade = 1)
			AND (@OrderBy NOT IN (1,2,3,4) OR oa.HasOrbit = 1)
			AND (@OrderBy NOT IN (5,6) OR oa.HasRade = 1)
			AND (
				@PlanetWithSize = 0 
				OR (
					@PlanetType IS NULL OR @PlanetType = '[]'
					OR EXISTS (
						SELECT 1 FROM @PlanetTypes pt
						WHERE 
							(pt.TypeId = 1 AND pwc.Pl_rade BETWEEN 0.04 AND 0.27) OR
							(pt.TypeId = 2 AND pwc.Pl_rade >  0.27 AND pwc.Pl_rade <= 0.7) OR
							(pt.TypeId = 3 AND pwc.Pl_rade >  0.7  AND pwc.Pl_rade <= 1.2) OR
							(pt.TypeId = 4 AND pwc.Pl_rade >  1.2  AND pwc.Pl_rade <= 1.9) OR
							(pt.TypeId = 5 AND pwc.Pl_rade >  1.9  AND pwc.Pl_rade <= 3.1) OR
							(pt.TypeId = 6 AND pwc.Pl_rade >  3.1  AND pwc.Pl_rade <= 5.1) OR
							(pt.TypeId = 7 AND pwc.Pl_rade >  5.1  AND pwc.Pl_rade <= 8.3) OR
							(pt.TypeId = 8 AND pwc.Pl_rade >  8.3  AND pwc.Pl_rade <= 13.7) OR
							(pt.TypeId = 9 AND pwc.Pl_rade > 13.7  AND pwc.Pl_rade <= 50)
					)
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
				@PlenetsCountFrom IS NULL OR pwc.SystemPlanetCount >= @PlenetsCountFrom
			)
			AND (
				@PlenetsCountTo IS NULL OR pwc.SystemPlanetCount <= @PlenetsCountTo
			)
			AND (
				@HabitableZone IS NULL OR @HabitableZone = 0 OR oa.HasHabitable = 1
			)
			AND (
				@TerrestrialHabitableZone IS NULL OR @TerrestrialHabitableZone = 0 OR oa.HasTerrestrialHZ = 1
			)
			AND (
				(@SyDistFrom IS NULL OR oa.HasSyDistFrom = 1) 
				AND	(@SyDistTo IS NULL OR oa.HasSyDistTo = 1)
		);

		CREATE NONCLUSTERED INDEX IX_ValidHostnames_Hostname ON #ValidHostname(Hostname);
		

		-- Aggregated values for sorting
		SELECT
			Hostname,
			MIN(NULLIF(Pl_orbsmax, 0)) AS MinPlOrbsmax,
			MAX(NULLIF(Pl_orbsmax, 0)) AS MaxPlOrbsmax,
			MIN(NULLIF(Pl_rade, 0)) AS MinPlRade,
			MAX(NULLIF(Pl_rade, 0)) AS MaxPlRade
		INTO #SystemAggregates
		FROM #PlanetWithCount
		GROUP BY Hostname;



		SELECT DISTINCT
			vh.Hostname,
			CASE @OrderBy
				WHEN 1 THEN ISNULL(sa.MinPlOrbsmax, 9999999)
				WHEN 2 THEN ISNULL(-1 * sa.MinPlOrbsmax, -9999999)
				WHEN 3 THEN ISNULL(sa.MaxPlOrbsmax, 9999999)
				WHEN 4 THEN ISNULL(-1 * sa.MaxPlOrbsmax, -9999999)
				WHEN 5 THEN ISNULL(sa.MinPlRade, 9999999)
				WHEN 6 THEN ISNULL(sa.MaxPlRade, 9999999)
				ELSE NULL
			END AS SortKey
		INTO #SortKeyTable
		FROM #ValidHostname vh
		JOIN #SystemAggregates sa ON vh.Hostname = sa.Hostname;


		SELECT *, ROW_NUMBER() OVER (ORDER BY SortKey, Hostname) AS SystemNumber
		INTO #HostnameOrdered
		FROM #SortKeyTable;
		


		-- Paggination		
		SET @TotalCount = (SELECT TOP 1 SystemNumber FROM #HostnameOrdered ORDER BY SystemNumber DESC); 
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
			SystemNumber,
			ho.Hostname, 
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
			SystemPlanetCount,
			RowOnPage = @TotalCount -- Using the RowOnPage field of the database table to pass a value of @TotalCount.
			-- Because EF Core does not support reading OUTPUT parameters directly in FromSql*-methods. Dapper is required.		
		FROM #HostnameOrdered ho
		LEFT JOIN #PlanetWithCount pwc ON ho.Hostname = pwc.Hostname
		WHERE ho.SystemNumber BETWEEN @Offset + 1 AND (@Offset + @RowOnPage);

		DROP TABLE #PlanetWithCount;
		DROP TABLE #ValidHostname;
		DROP TABLE #SystemAggregates;
		DROP TABLE #SortKeyTable;
		DROP TABLE #HostnameOrdered;


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
