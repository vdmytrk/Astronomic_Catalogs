--=======================================================================================================================================--
--==========================================================  STORED PRCEDURE  ==========================================================--
--=======================================================================================================================================--
-------------------------------------------------------------------------------------------------------------------------------------------
--==================================================  GET DATA FROM NGCICOpendatasoft  ==================================================--
-------------------------------------------------------------------------------------------------------------------------------------------

CREATE OR ALTER PROC GetFilteredNGCICData
    @Name NVARCHAR(100) = NULL,
    @Constellations NVARCHAR(MAX) = NULL, 
    @Ang_Diameter_min FLOAT = NULL,
    @Ang_Diameter_max FLOAT = NULL,
    @RA_From_Hours INT = NULL, 
    @RA_From_Minutes INT = NULL,
    @RA_From_Seconds FLOAT = NULL,
    @RA_To_Hours INT = NULL,
    @RA_To_Minutes INT = NULL,
    @RA_To_Seconds FLOAT = NULL,
    @Dec_From_Pole CHAR(1) = NULL,
    @Dec_From_Degrees INT = NULL,
    @Dec_From_Minutes INT = NULL,
    @Dec_From_Seconds FLOAT = NULL,
    @Dec_To_Pole CHAR(1) = NULL,
    @Dec_To_Degrees INT = NULL,
    @Dec_To_Minutes INT = NULL,
    @Dec_To_Seconds FLOAT = NULL,
    @ObjectTypes NVARCHAR(MAX) = NULL,
    @IncludeNGC BIT = 0,
    @IncludeIC BIT = 0,
    @IncludeMessier BIT = 0,
    @PageNumber INT = 1,
    @RowOnPage INT = 50
AS
BEGIN
	DECLARE @TotalCount INT = 0, @PageCountInResult INT = 0, @Offset INT = 0;

	SET NOCOUNT ON; 

    BEGIN TRY	
		-- Default value blok
		SET @Dec_From_Pole = ISNULL(@Dec_From_Pole, '-'); 
		SET @Dec_To_Pole = ISNULL(@Dec_To_Pole, '+');
		SET @Name = NULLIF(@Name, '');
		SET @Ang_Diameter_min = ISNULL(@Ang_Diameter_min, 0);
		SET @Ang_Diameter_max = ISNULL(@Ang_Diameter_max, 300);
		SET @PageNumber = ISNULL(@PageNumber, 1);
		SET @RowOnPage =  ISNULL(@RowOnPage, 50);


		-- Parameter calculation block
		DECLARE @RA_From FLOAT = ISNULL(@RA_From_Hours, 0) * 3600 + ISNULL(@RA_From_Minutes, 0) * 60 + ISNULL(@RA_From_Seconds, 0);
		DECLARE @RA_To FLOAT = ISNULL(@RA_To_Hours, 24) * 3600 + ISNULL(@RA_To_Minutes, 60) * 60 + ISNULL(@RA_To_Seconds, 60); 

		DECLARE @Dec_From FLOAT = CASE WHEN @Dec_From_Pole = '-' THEN
				(ISNULL(@Dec_From_Degrees, 90) * 3600 + ISNULL(@Dec_From_Minutes, 60) * 60 + ISNULL(@Dec_From_Seconds, 60)) * -1
			ELSE
				(ISNULL(@Dec_From_Degrees, 0) * 3600 + ISNULL(@Dec_From_Minutes, 0) * 60 + ISNULL(@Dec_From_Seconds, 0))
			END;
		
		DECLARE @Dec_To FLOAT = CASE WHEN @Dec_To_Pole = '-' THEN
				(ISNULL(@Dec_To_Degrees, 0) * 3600 + ISNULL(@Dec_To_Minutes, 0) * 60 + ISNULL(@Dec_To_Seconds, 0)) * -1
			ELSE
				(ISNULL(@Dec_To_Degrees, 90) * 3600 + ISNULL(@Dec_To_Minutes, 60) * 60 + ISNULL(@Dec_To_Seconds, 60))
			END;


		SELECT * INTO #SummaryTable
		FROM (
			SELECT Id, NGC_IC, [Name], SubObject, Messier, Name_UK, Comment, Other_names, NGC, IC, ObjectTypeAbrev, ObjectType, Object_type,
				Source_Type, RA, Right_ascension, Right_ascension_H, Right_ascension_M, Right_ascension_S, DEC, Declination, NS, Declination_D,
				Declination_M, Declination_S, Constellation, MajorAxis, MinorAxis, PositionAngle, App_Mag, App_Mag_Flag, b_mag, v_mag, j_mag, h_mag,
				k_mag, Surface_Brigthness, Hubble_OnlyGalaxies, Cstar_UMag, Cstar_BMag, Cstar_VMag, Cstar_Names, CommonNames, NedNotes, OpenngcNotes,
				[Image], Ang_Diameter, Limit_Ang_Diameter, SourceTable, RowOnPage
			FROM NGCICOpendatasoft
			WHERE
				(@Name IS NULL OR [Other_names] LIKE '%' + @Name + '%')
				AND (
					@Constellations = '[]' OR @Constellations IS NULL OR EXISTS (
						SELECT 1 
						FROM OPENJSON(@Constellations) AS j
						WHERE Constellation LIKE j.[value] + '%'
					)
				)
				AND (
					Ang_Diameter >= CAST(@Ang_Diameter_min AS FLOAT) 
					OR (@Ang_Diameter_min = 0 AND Ang_Diameter IS NULL)
				)
				AND (
					Ang_Diameter <= CAST(@Ang_Diameter_max AS FLOAT)
					OR (@Ang_Diameter_max = 300 AND Ang_Diameter IS NULL)
				)
				AND (
					((Right_ascension_H * 3600 + Right_ascension_M * 60 + Right_ascension_S) BETWEEN @RA_From AND @RA_To)
					OR (RA = '')
				)
				AND (
					(((Declination_D * 3600 + Declination_M * 60 + Declination_S) * CASE WHEN NS = '-' THEN -1 ELSE 1 END)
					BETWEEN @Dec_From AND @Dec_To)
					OR [DEC] = ''
				)
				AND (
					@ObjectTypes IS NULL OR EXISTS (
						SELECT 1 
						FROM OPENJSON(@ObjectTypes) AS j
						WHERE ObjectTypeAbrev LIKE j.[value] 
					)
				)
				AND (	
					(@IncludeNGC = @IncludeIC AND @IncludeIC = @IncludeMessier) 
					OR 
					(@IncludeNGC = 1 AND @IncludeIC = 1 AND @IncludeMessier = 0) 
					OR (
						(@IncludeNGC = 1 AND NGC_IC LIKE 'NGC%')
						OR (@IncludeIC = 1 AND NGC_IC LIKE 'IC%')
						OR (@IncludeMessier = 1 AND Messier IS NOT NULL AND Messier <> '')
					)
				)

			UNION ALL

			SELECT Id, NGC_IC, [Name], SubObject, Messier, Name_UK, Comment, Other_names, NGC, IC, ObjectTypeAbrev, ObjectType, Object_type,
				Source_Type, RA, Right_ascension, Right_ascension_H, Right_ascension_M, Right_ascension_S, DEC, Declination, NS, Declination_D,
				Declination_M, Declination_S, Constellation, MajorAxis, MinorAxis, PositionAngle, App_Mag, App_Mag_Flag, b_mag, v_mag, j_mag, h_mag,
				k_mag, Surface_Brigthness, Hubble_OnlyGalaxies, Cstar_UMag, Cstar_BMag, Cstar_VMag, Cstar_Names, CommonNames, NedNotes, OpenngcNotes,
				[Image], Ang_Diameter, Limit_Ang_Diameter, SourceTable, RowOnPage
			FROM NGCICOpendatasoft_Extension
			WHERE
				(@Name IS NULL OR [Other_names] LIKE '%' + @Name + '%')
				AND (
					@Constellations = '[]' OR @Constellations IS NULL OR EXISTS (
						SELECT 1 
						FROM OPENJSON(@Constellations) AS j
						WHERE Constellation LIKE j.[value] + '%'
					)
				)
				AND (
					Ang_Diameter >= CAST(@Ang_Diameter_min AS FLOAT) 
					OR (@Ang_Diameter_min = 0 AND Ang_Diameter IS NULL)
				)
				AND (
					Ang_Diameter <= CAST(@Ang_Diameter_max AS FLOAT)
					OR (@Ang_Diameter_max = 300 AND Ang_Diameter IS NULL)
				)
				AND (
					((Right_ascension_H * 3600 + Right_ascension_M * 60 + Right_ascension_S) BETWEEN @RA_From AND @RA_To)
					OR RA = ''
				)
				AND (
					(((Declination_D * 3600 + Declination_M * 60 + Declination_S) * CASE WHEN NS = '-' THEN -1 ELSE 1 END)
					BETWEEN @Dec_From AND @Dec_To)
					OR [DEC] = ''
				)
				AND (
					@ObjectTypes IS NULL OR EXISTS (
						SELECT 1 
						FROM OPENJSON(@ObjectTypes) AS j
						WHERE ObjectTypeAbrev LIKE j.[value] 
					)
				)
				AND (	
					(@IncludeNGC = @IncludeIC AND @IncludeIC = @IncludeMessier) 
					OR 
					(@IncludeNGC = 1 AND @IncludeIC = 1 AND @IncludeMessier = 0) 
					OR (
						(@IncludeNGC = 1 AND NGC_IC LIKE 'NGC%')
						OR (@IncludeIC = 1 AND NGC_IC LIKE 'IC%')
						OR (@IncludeMessier = 1 AND Messier IS NOT NULL AND Messier <> '')
					)
				)
		) AS SelectedData;

				
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
			END 



		-- Not a CASE because of different field types: Messier is INT, while NGC_IC and [Name] are not.
		DECLARE @MessierOnly BIT = IIF(@IncludeNGC = 0 AND @IncludeIC = 0 AND @IncludeMessier = 1, 1, 0);
		DECLARE @NGCOrICOnly BIT = IIF((@IncludeNGC = 1 OR @IncludeIC = 1) AND @IncludeMessier = 0, 1, 0);

		IF @MessierOnly = 1 
		BEGIN
			SELECT 
				Id, NGC_IC, [Name], SubObject, Messier,
				Name_UK, Comment, Other_names, NGC, IC,
				Limit_Ang_Diameter, Ang_Diameter,
				ObjectTypeAbrev, ObjectType, Object_type, Source_Type,
				RA, Right_ascension, Right_ascension_H, Right_ascension_M, Right_ascension_S,
				[DEC], Declination, NS, Declination_D, Declination_M, Declination_S,
				Constellation, MajorAxis, MinorAxis, PositionAngle,
				App_Mag, App_Mag_Flag,
				b_mag, v_mag, j_mag, h_mag, k_mag,
				Surface_Brigthness, Hubble_OnlyGalaxies,
				Cstar_UMag, Cstar_BMag, Cstar_VMag, Cstar_Names,
				CommonNames, NedNotes, OpenngcNotes,
				[Image] AS Image,
				SourceTable,
				COUNT(*) OVER() AS RowOnPage 
			FROM #SummaryTable
			ORDER BY CAST(Messier AS INT)
			OFFSET @Offset ROWS FETCH NEXT @RowOnPage ROWS ONLY;
		END
		ELSE IF @NGCOrICOnly = 1 
		BEGIN			
			SELECT 
				Id, NGC_IC, [Name], SubObject, Messier,
				Name_UK, Comment, Other_names, NGC, IC,
				Limit_Ang_Diameter, Ang_Diameter,
				ObjectTypeAbrev, ObjectType, Object_type, Source_Type,
				RA, Right_ascension, Right_ascension_H, Right_ascension_M, Right_ascension_S,
				[DEC], Declination, NS, Declination_D, Declination_M, Declination_S,
				Constellation, MajorAxis, MinorAxis, PositionAngle,
				App_Mag, App_Mag_Flag,
				b_mag, v_mag, j_mag, h_mag, k_mag,
				Surface_Brigthness, Hubble_OnlyGalaxies,
				Cstar_UMag, Cstar_BMag, Cstar_VMag, Cstar_Names,
				CommonNames, NedNotes, OpenngcNotes,
				[Image] AS Image,
				SourceTable,
				COUNT(*) OVER() AS RowOnPage
			FROM #SummaryTable
			ORDER BY NGC_IC
			OFFSET @Offset ROWS FETCH NEXT @RowOnPage ROWS ONLY;
		END
		ELSE 
		BEGIN			
			SELECT 
				Id, NGC_IC, [Name], SubObject, Messier,
				Name_UK, Comment, Other_names, NGC, IC,
				Limit_Ang_Diameter, Ang_Diameter,
				ObjectTypeAbrev, ObjectType, Object_type, Source_Type,
				RA, Right_ascension, Right_ascension_H, Right_ascension_M, Right_ascension_S,
				[DEC], Declination, NS, Declination_D, Declination_M, Declination_S,
				Constellation, MajorAxis, MinorAxis, PositionAngle,
				App_Mag, App_Mag_Flag,
				b_mag, v_mag, j_mag, h_mag, k_mag,
				Surface_Brigthness, Hubble_OnlyGalaxies,
				Cstar_UMag, Cstar_BMag, Cstar_VMag, Cstar_Names,
				CommonNames, NedNotes, OpenngcNotes,
				[Image] AS Image,
				SourceTable,
				COUNT(*) OVER() AS RowOnPage
			FROM #SummaryTable
			ORDER BY [Name]
			OFFSET @Offset ROWS FETCH NEXT @RowOnPage ROWS ONLY;
		END

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
