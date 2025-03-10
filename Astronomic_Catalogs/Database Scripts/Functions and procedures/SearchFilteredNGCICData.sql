
--=======================================================================================================================================--
--==========================================================  STORED PRCEDURE  ==========================================================--
--=======================================================================================================================================--
-------------------------------------------------------------------------------------------------------------------------------------------
--=============================================  GET FROM NGCICOpendatasoft WITH FILTERS  =============================================--
-------------------------------------------------------------------------------------------------------------------------------------------

CREATE OR ALTER PROC SearchFilteredNGCICData
	@ChbOnePage AS BIT,  
    @TextNambername AS VARCHAR(MAX),
    @TextRefRevision AS VARCHAR(MAX),
    @TextConstellation AS VARCHAR(MAX),
    @TextDescription AS VARCHAR(MAX),
    @TextComment AS VARCHAR(MAX),
    @TextClass AS VARCHAR(MAX),
    @TextName AS VARCHAR(MAX),
    @TextRightascensionmin AS VARCHAR(MAX),
    @TextRightascensionmax AS VARCHAR(MAX),
    @TextDeclinationmin AS VARCHAR(MAX),
    @TextDeclinationmax AS VARCHAR(MAX),
    @TextLIImin AS VARCHAR(MAX),
    @TextLIImax AS VARCHAR(MAX),
    @TextBIImin AS VARCHAR(MAX),
    @TextBIImax AS VARCHAR(MAX),
    @TextAngDiametermin AS VARCHAR(MAX),
    @TextAngDiametermax AS VARCHAR(MAX),
    @TextAppMagmin AS VARCHAR(MAX),
    @TextAppMagmax AS VARCHAR(MAX),
    @ChbAppMagFlag AS BIT,
    @ChbNGC AS BIT, 
    @ChbI AS BIT,
    @SLRowOnPage AS int,
    @TypeGx AS BIT,
    @TypeOC AS BIT,
    @TypeGb AS BIT,
    @TypeNb AS BIT,
    @TypePl AS BIT,
    @TypeCN AS BIT,
    @TypeAst AS BIT,
    @TypeKt AS BIT,
    @TypeTS AS BIT,
    @TypeDS AS BIT,
    @TypeSS AS BIT,
    @TypeN AS BIT,
    @TypeU AS BIT,
    @TypeS AS BIT,
    @TypePD AS BIT,
    @PAGEnUMBER AS int 
AS 
BEGIN TRY
	DECLARE @ChbOnePageI AS BIT;  
    DECLARE @TextNamberNameI AS VARCHAR(MAX) = ''; 
    DECLARE @TextRefRevisionI AS VARCHAR(MAX) = ''; 
    DECLARE @TextConstellationI AS VARCHAR(MAX) = ''; 
    DECLARE @TextDescriptionI AS VARCHAR(MAX) = ''; 
    DECLARE @TextCommentI AS VARCHAR(MAX) = ''; 
    DECLARE @TextClassI AS VARCHAR(MAX) = ''; 
    DECLARE @TextNameI AS VARCHAR(MAX) = ''; 
    DECLARE @TextRightAscensionMinI AS VARCHAR(MAX) = ''; 
    DECLARE @TextRightAscensionMaxI AS VARCHAR(MAX) = ''; 
    DECLARE @TextDeclinationminI AS VARCHAR(MAX) = ''; 
    DECLARE @TextDeclinationmaxI AS VARCHAR(MAX) = ''; 
    DECLARE @TextLII_MinI AS VARCHAR(MAX) = ''; 
    DECLARE @TextLII_MaxI AS VARCHAR(MAX) = ''; 
    DECLARE @TextBII_MinI AS VARCHAR(MAX) = ''; 
    DECLARE @TextBIImaxI AS VARCHAR(MAX) = ''; 
    DECLARE @TextAngDiameterminI AS VARCHAR(MAX) = ''; 
    DECLARE @TextAngDiametermaxI AS VARCHAR(MAX) = ''; 
    DECLARE @TextAppMagminI AS VARCHAR(MAX) = ''; 
    DECLARE @TextAppMagmaxI AS VARCHAR(MAX) = ''; 
    DECLARE @ChbAppMagFlagI AS VARCHAR(MAX) = ''; 
    DECLARE @Catalogue AS VARCHAR(MAX) = ''; 
    DECLARE @TypeGxI AS VARCHAR(MAX) = ''; 
    DECLARE @TypeOCI AS VARCHAR(MAX) = ''; 
    DECLARE @TypeGbI AS VARCHAR(MAX) = ''; 
    DECLARE @TypeNbI AS VARCHAR(MAX) = ''; 
    DECLARE @TypePlI AS VARCHAR(MAX) = ''; 
    DECLARE @TypeCNI AS VARCHAR(MAX) = ''; 
    DECLARE @TypeAstI AS VARCHAR(MAX) = '';
    DECLARE @TypeKtI AS VARCHAR(MAX) = '';
    DECLARE @TypeTSI AS VARCHAR(MAX) = '';
    DECLARE @TypeDSI AS VARCHAR(MAX) = '';
    DECLARE @TypeSSI AS VARCHAR(MAX) = '';
    DECLARE @TypeNI AS VARCHAR(MAX) = '';
    DECLARE @TypeUI AS VARCHAR(MAX) = ''; 
    DECLARE @TypeSI AS VARCHAR(MAX) = ''; 
    DECLARE @TypePDI AS VARCHAR(MAX) = '';
	DECLARE @AllFiltersSource_Type AS VARCHAR(MAX);
    DECLARE @PAGEnUMBERI AS int; 

	DECLARE @QUERY AS NVARCHAR(MAX);

	IF @ChbOnePage IS NOT NULL AND (@ChbOnePage = 1 OR @ChbOnePage = 0) 
		SET @ChbOnePageI = @ChbOnePage;
	ELSE 
		BEGIN
			DECLARE @ErrorMessege0 NVARCHAR(MAX), @ErrorSeverity0 INT, @ErrorState0 INT;
			SET @ChbOnePageI = 1;
			SET @ErrorMessege0 = N'ÇÍÀ×ÅÍÍß ÏÀÐÀÌÅÒÐÓ @ChbOnePage: ' + CAST (@ChbOnePage AS NVARCHAR) + 
				'. ÇÍÀ×ÅÍÍß ÇÌ²ÍÍÎ¯ @ChbOnePageI: ' + CAST (@ChbOnePageI AS NVARCHAR);
			SET @ErrorSeverity0 = 11; 
			SET @ErrorState0 = ERROR_STATE();
			
			-- https://docs.microsoft.com/en-us/sql/t-sql/language-elements/raiserror-transact-sql?view=sql-server-ver16
			-- Use RAISERROR inside the CATCH block to return error
			-- information about the original error that caused
			-- execution to jump to the CATCH block.
			RAISERROR (@ErrorMessege0, 
					   @ErrorSeverity0, 
					   @ErrorState0 
					   );	
		END

	IF @TextNambername != '%'
		SET @TextNamberNameI = ' AND Namber_name LIKE ''' + '%' + @TextNambername + '%' + ''' ';
	IF @TextRefRevision != '%'
		SET @TextRefRevisionI = ' AND Ref_Revision LIKE ''' + '%' + @TextRefRevision + '%' + ''' ';
	IF @TextConstellation != '%'
		SET @TextConstellationI = ' AND Constellation LIKE ''' + '%' + @TextConstellation + '%' + ''' ';
	IF @TextDescription != '%'
		SET @TextDescriptionI = ' AND Description LIKE ''' + '%' + @TextDescription + '%' + ''' ';
	IF @TextComment != '%'
		SET @TextCommentI = ' AND Comment LIKE ''' + '%' + @TextComment + '%' + ''' ';
	IF @TextClass != '%'
		SET @TextClassI = ' AND Class LIKE ''' + '%' + @TextClass + '%' + ''' ';
	IF @TextName != '%'
		SET @TextNameI = ' AND Name LIKE ''' + '%' + @TextName + '%' + ''' ';
	IF @TextRightascensionmin != '0'
		SET @TextRightAscensionMinI = ' AND Right_ascension >= ' + CONVERT (VARCHAR, @TextRightascensionmin) + ' ';
	IF @TextRightascensionmax != '23'
		SET @TextRightAscensionMaxI = ' AND Right_ascension <= ' + CONVERT (VARCHAR, @TextRightascensionmax) + ' ';
	IF @TextDeclinationmin != '0'
		SET @TextDeclinationminI = ' AND Declination >= ' + CONVERT (VARCHAR, @TextDeclinationmin) + ' ';
	IF @TextDeclinationmax != '59.9'
		SET @TextDeclinationmaxI = ' AND Declination <= ' + CONVERT (VARCHAR, @TextDeclinationmax) + ' ';
	IF @TextLIImin != '-89'
		SET @TextLII_MinI = ' AND LII >= ' + CONVERT (VARCHAR, @TextLIImin) + ' ';
	IF @TextLIImax != '89'
		SET @TextLII_MaxI = ' AND LII <= ' + CONVERT (VARCHAR, @TextLIImax) + ' ';
	IF @TextBIImin != '0'
		SET @TextBII_MinI = ' AND BII >= ' + CONVERT (VARCHAR, @TextBIImin) + ' ';
	IF @TextBIImax != '59'
		SET @TextBIImaxI = ' AND BII <= ' + CONVERT (VARCHAR, @TextBIImax) + ' ';
	IF @TextAngDiametermin != '0'
		SET @TextAngDiameterminI = ' AND Ang_Diameter >= ' + CONVERT (VARCHAR, @TextAngDiametermin) + ' ';
	IF @TextAngDiametermax != '240'
		SET @TextAngDiametermaxI = ' AND Ang_Diameter <= ' + CONVERT (VARCHAR, @TextAngDiametermax) + ' ';
	IF @TextAppMagmin != '0'
		SET @TextAppMagminI = ' AND App_Mag >= ' + CONVERT (VARCHAR, @TextAppMagmin) + ' ';
	IF @TextAppMagmax != '8'
		SET @TextAppMagmaxI = ' AND App_Mag <= ' + CONVERT (VARCHAR, @TextAppMagmax) + ' ';
	IF @ChbAppMagFlag IS NOT NULL AND (@ChbAppMagFlag = 1 OR @ChbAppMagFlag = 0) 
		IF (@ChbAppMagFlag = 1) 
			SET @ChbAppMagFlagI = ' AND App_Mag_Flag LIKE ''%p%'' ';
	IF (@ChbNGC IS NOT NULL AND (@ChbNGC = 1 OR @ChbNGC = 0)) AND 
	   (@ChbI IS NOT NULL AND (@ChbI = 1 OR @ChbI = 0))  
		IF (@ChbNGC = 0 AND @ChbI = 1) 
			SET @Catalogue = ' AND Namber_name LIKE ''I%'' ';
		ELSE IF (@ChbNGC = 1 AND @ChbI = 0) 
			SET @Catalogue = ' AND 1 = CASE WHEN ISNUMERIC (Namber_name) = 0 THEN 0 ELSE 1 END ';
		ELSE IF (@ChbNGC = 0 AND @ChbI = 0) 
			SET @Catalogue = ' AND Namber_name = ''X'' ';
	IF @TypeGx IS NOT NULL AND (@TypeGx = 1 OR @TypeOC = 1 OR @TypeGb= 1 OR @TypeNb = 1 OR @TypePl = 1 OR 
	                            @TypeCN = 1 OR @TypeAst = 1 OR @TypeKt = 1 OR @TypeTS = 1 OR @TypeDS = 1 OR  
								@TypeSS = 1 OR @TypeN = 1 OR @TypeU = 1 OR @TypeS = 1 OR @TypePD = 1)
		SET @AllFiltersSource_Type = ' AND ( ';
	ELSE 
		SET @AllFiltersSource_Type = ' AND ( Source_Type != ''1'' OR ';
	IF @TypeGx IS NOT NULL AND @TypeGx = 1
		SET  @AllFiltersSource_Type = @AllFiltersSource_Type + ' Source_Type = ''Gx'' OR ';
	IF @TypeOC IS NOT NULL AND @TypeOC = 1
		SET  @AllFiltersSource_Type = @AllFiltersSource_Type + ' Source_Type = ''OC'' OR ';
	IF @TypeGb IS NOT NULL AND @TypeGb = 1
		SET  @AllFiltersSource_Type = @AllFiltersSource_Type + ' Source_Type = ''Gb'' OR ';
	IF @TypeNb IS NOT NULL AND @TypeNb = 1
		SET  @AllFiltersSource_Type = @AllFiltersSource_Type + ' Source_Type = ''Nb'' OR ';
	IF @TypePl IS NOT NULL AND @TypePl = 1
		SET  @AllFiltersSource_Type = @AllFiltersSource_Type + ' Source_Type = ''Pl'' OR ';
	IF @TypeCN IS NOT NULL AND @TypeCN = 1
		SET  @AllFiltersSource_Type = @AllFiltersSource_Type + ' Source_Type = ''C+N'' OR ';
	IF @TypeAst IS NOT NULL AND @TypeAst = 1
		SET  @AllFiltersSource_Type = @AllFiltersSource_Type + ' Source_Type = ''Ast'' OR ';
	IF @TypeKt IS NOT NULL AND @TypeKt = 1
		SET  @AllFiltersSource_Type = @AllFiltersSource_Type + ' Source_Type = ''Kt'' OR ';
	IF @TypeTS IS NOT NULL AND @TypeTS = 1
		SET  @AllFiltersSource_Type = @AllFiltersSource_Type + ' Source_Type = ''TS'' OR ';
	IF @TypeDS IS NOT NULL AND @TypeDS = 1
		SET  @AllFiltersSource_Type = @AllFiltersSource_Type + ' Source_Type = ''DS'' OR ';
	IF @TypeSS IS NOT NULL AND @TypeSS = 1
		SET  @AllFiltersSource_Type = @AllFiltersSource_Type + ' Source_Type = ''SS'' OR ';
	IF @TypeN IS NOT NULL AND @TypeN = 1
		SET  @AllFiltersSource_Type = @AllFiltersSource_Type + ' Source_Type = ''?'' OR ';
	IF @TypeU IS NOT NULL AND @TypeU = 1
		SET  @AllFiltersSource_Type = @AllFiltersSource_Type + ' Source_Type = ''U'' OR ';
	IF @TypeS IS NOT NULL AND @TypeS = 1
		SET  @AllFiltersSource_Type = @AllFiltersSource_Type + ' Source_Type = ''-'' OR ';
	IF @TypePD IS NOT NULL AND @TypePD = 1
		SET  @AllFiltersSource_Type = @AllFiltersSource_Type + ' Source_Type = ''PD'' OR ';
	SET @PAGEnUMBERI = @PAGEnUMBER;
	--  select * from SourceType;

	SET @QUERY = 
	N'SELECT RN, Id, Namber_name, Name, Comment, Source_Type, Right_ascension, Declination, LII, BII, Ref_Revision, Constellation, 
		Limit_Ang_Diameter, Ang_Diameter, App_Mag, App_Mag_Flag, Description, Class, 
		(COUNT(*) OVER() /  + ' + CAST (@SLRowOnPage AS VARCHAR) + ' + 1) AS PageCount, 
		PageNumber 
	FROM (
		SELECT ROW_NUMBER() OVER (ORDER BY ID) AS RN, * 
		FROM NGCICOpendatasoft		
		WHERE 1 = 1 ' + @TextNamberNameI + @TextRefRevisionI + @TextConstellationI + @TextDescriptionI + @TextCommentI + @TextClassI +
		@TextNameI + @TextRightAscensionMinI + @TextRightAscensionMaxI + @TextDeclinationminI + @TextDeclinationmaxI + 
		@TextLII_MinI + @TextLII_MaxI + @TextBII_MinI + @TextBIImaxI + @TextAngDiameterminI + @TextAngDiametermaxI + 
		@TextAppMagminI + @TextAppMagmaxI + @ChbAppMagFlagI + @Catalogue + @AllFiltersSource_Type + 'Source_Type = ''1'' ) ' +
		') as ST
	ORDER BY Namber_name
	OFFSET (' + CAST (@PAGEnUMBERI AS VARCHAR) + ' - 1) * ' + CAST (@SLRowOnPage AS VARCHAR) + 
	' ROWS FETCH NEXT ' + CAST (@SLRowOnPage AS VARCHAR) + ' ROWS ONLY';
	INSERT INTO LOG_TABLE (ErrorMessage) VALUES ('@PAGEnUMBERI: ' + CAST (@PAGEnUMBERI AS VARCHAR) +
													', @SLRowOnPage: ' + CAST (@SLRowOnPage AS VARCHAR) +
													' | @TextNamberNameI: ' + @TextNamberNameI +
													', @TextRefRevisionI: ' + @TextRefRevisionI + 
													', @TextConstellationI: ' + @TextConstellationI + 
													', @TextDescriptionI: ' + @TextDescriptionI +
													', @TextCommentI: ' + @TextCommentI +
													', @TextClassI: ' + @TextClassI +
													', @TextNameI: ' + @TextNameI +
													', @TextRightAscensionMinI: ' + @TextRightAscensionMinI + ', @TextRightAscensionMaxI: ' + @TextRightAscensionMaxI +
													', @TextDeclinationminI: ' + @TextDeclinationminI + ', @TextDeclinationmaxI: ' + @TextDeclinationmaxI +
													', @TextLII_MinI: ' + @TextLII_MinI +	', @TextLII_MaxI: ' + @TextLII_MaxI +
													', @TextBII_MinI: ' + @TextBII_MinI +	', @TextBIImaxI: ' + @TextBIImaxI +
													', @TextAngDiameterminI: ' + @TextAngDiameterminI +	', @TextAngDiametermaxI: ' + @TextAngDiametermaxI +
													', @TextAppMagminI: ' + @TextAppMagminI + ', @TextAppMagmaxI: ' + @TextAppMagmaxI +
													', @ChbAppMagFlagI: ' + @ChbAppMagFlagI +
													', @Catalogue: ' + @Catalogue +
													', @TypeGxI: ' + @TypeGxI + ', @TypeOCI: ' + @TypeOCI + ', @TypeGbI: ' + @TypeGbI +
													', @TypeNbI: ' + @TypeNbI + ', @TypePlI: ' + @TypePlI + ', @TypeCNI: ' + @TypeCNI +
													', @TypeAstI: ' + @TypeAstI + ', @TypeKtI: ' + @TypeKtI + ', @TypeTSI: ' + @TypeTSI +
													', @TypeDSI: ' + @TypeDSI +	', @TypeSSI: ' + @TypeSSI + ', @TypeNI: ' + @TypeNI +
													', @TypeUI: ' + @TypeUI + ', @TypeSI: ' + @TypeSI +	', @TypePDI: ' + @TypePDI +
													', @AllFiltersSource_Type: ' + @AllFiltersSource_Type
													);
	INSERT INTO LOG_TABLE (ErrorMessage) VALUES ('@QUERY: ' + @QUERY);
	EXECUTE sp_executesql @QUERY, 
		N'@PAGEnUMBERI INT, @SLRowOnPage INT', 
		@PAGEnUMBERI = @PAGEnUMBERI, @SLRowOnPage = @SLRowOnPage;
END TRY
BEGIN  CATCH
	DECLARE @FuncProc AS VARCHAR(50), @Line AS INT, @ErrorNumber AS INT, @ErrorMessage VARCHAR(MAX), @ErrorSeverity INT, @ErrorState INT;

	SET @FuncProc = ERROR_PROCEDURE();
	SET @Line = ERROR_LINE();
	SET @ErrorNumber = ERROR_NUMBER();
	SET @ErrorSeverity = ERROR_SEVERITY();
	SET @ErrorState = ERROR_STATE();
	SET @ErrorMessage = ERROR_MESSAGE();

	INSERT INTO LOG_TABLE (FuncProc, Line, ErrorNumber, ErrorSeverity, ErrorState, ErrorMessage) 
	VALUES (@FuncProc, @Line, @ErrorNumber, @ErrorSeverity, @ErrorState, @ErrorMessage);
END CATCH


