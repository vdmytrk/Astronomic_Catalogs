--=======================================================================================================================================--
--==========================================================  STORED PRCEDURE  ==========================================================--
--=======================================================================================================================================--
-------------------------------------------------------------------------------------------------------------------------------------------
--==================================================  GET DATA FROM CollinderCatalog   ==================================================--
-------------------------------------------------------------------------------------------------------------------------------------------

CREATE OR ALTER PROC GetFilteredCollinderData
    @NameOtherCat NVARCHAR(100) = NULL,
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
    @PageNumber INT = 1,
    @RowOnPage INT = 50
AS
BEGIN		
	DECLARE @TotalCount INT = 0, @PageCountInResult INT = 0, @Offset INT = 0;

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
		-- Default value blok
		SET @Dec_From_Pole = ISNULL(@Dec_From_Pole, '-'); 
		SET @Dec_To_Pole = ISNULL(@Dec_To_Pole, '+');
		SET @NameOtherCat = NULLIF(@NameOtherCat, '');
		SET @Ang_Diameter_min = ISNULL(@Ang_Diameter_min, 0);
		SET @Ang_Diameter_max = 
			CASE 
				WHEN @Ang_Diameter_max IS NULL OR @Ang_Diameter_max >= 350 THEN 1000
				ELSE @Ang_Diameter_max
			END;
		SET @PageNumber = ISNULL(@PageNumber, 1);
		SET @RowOnPage = ISNULL(@RowOnPage, 50);
		

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



		SELECT * INTO #TemporaryTable
		FROM CollinderCatalog
		WHERE
			(@NameOtherCat IS NULL OR [NameOtherCat] LIKE '%' + @NameOtherCat + '%')
			AND (
				@Constellations = '[]' OR @Constellations IS NULL OR EXISTS (
					SELECT 1 
					FROM OPENJSON(@Constellations) AS j
					WHERE Constellation LIKE j.[value] + '%'
				)
			)
			AND (
				[Ang_Diameter_Max] >= CAST(@Ang_Diameter_min AS FLOAT) 
				OR (@Ang_Diameter_min = 0 AND [Ang_Diameter_Max] IS NULL)
			)
			AND (
				[Ang_Diameter_Max] <= CAST(@Ang_Diameter_max AS FLOAT)
				OR (@Ang_Diameter_max = 1000 AND [Ang_Diameter_Max] IS NULL)
			)
			AND (
				((Right_ascension_H * 3600 + Right_ascension_M * 60 + Right_ascension_S) BETWEEN @RA_From AND @RA_To)
				OR ([Right_ascension] = '')
			)
			AND (
				(((Declination_D * 3600 + Declination_M * 60 + Declination_S) * CASE WHEN NS = '-' THEN -1 ELSE 1 END)
				BETWEEN @Dec_From AND @Dec_To)
				OR [Declination] = ''
			)
			AND (
				@ObjectTypes IS NULL OR EXISTS (
					SELECT 1 
					FROM OPENJSON(@ObjectTypes) AS j
					WHERE [Class] LIKE j.[value]
				)
			);

		
		SELECT @TotalCount = COUNT(*) FROM #TemporaryTable;
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
			Id, [Namber_name], NameOtherCat, Constellation
			, Right_ascension, Right_ascension_H, Right_ascension_M, Right_ascension_S
			, Declination, NS, Declination_D, Declination_M, Declination_S
			, App_Mag, App_Mag_Flag
			, CountStars, CountStars_ToFinding
			, [Ang_Diameter], Ang_Diameter_Max
			, Class, Comment
			, COUNT(*) OVER() AS [PageCount]
			, @PageNumber AS PageNumber -- Using the PageNumber field of the database table to pass a value of @PageCountInResult.
		FROM #TemporaryTable
		ORDER BY Id
		OFFSET @Offset ROWS FETCH NEXT @RowOnPage ROWS ONLY;
		
		DROP TABLE #TemporaryTable;

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


