--=======================================================================================================================================--
--==========================================================  STORED PRCEDURE  ==========================================================--
--=======================================================================================================================================--
-------------------------------------------------------------------------------------------------------------------------------------------
--==========================================  FILL DATA IN NASAExoplanetCatalogUniquePlanets   ==========================================--
-------------------------------------------------------------------------------------------------------------------------------------------

CREATE OR ALTER PROC FillNASAExoplanetCatalogUniquePlanets 
AS
BEGIN
    DECLARE @FuncProc AS NVARCHAR(100) = 'FillNASAExoplanetCatalogUniquePlanets'; 

    DECLARE @StartedTran BIT = 0;
 
    SET NOCOUNT ON; 

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

 
    BEGIN TRY
        DECLARE @sql NVARCHAR(MAX) = ''
        DECLARE @selectList NVARCHAR(MAX) = ''
        DECLARE @columnList NVARCHAR(MAX) = ''
        DECLARE @applyList NVARCHAR(MAX) = ''
        DECLARE @columnName NVARCHAR(255)
        DECLARE @dataType NVARCHAR(128)
        DECLARE @tableName NVARCHAR(128) = 'NASAExoplanetCatalogLastUpdate'
        DECLARE @defaultValue NVARCHAR(MAX)
        DECLARE @defaultDate DATE = '1900-01-01'

        
        DECLARE col_cursor CURSOR FOR
        SELECT c.name, t.name as data_type
        FROM sys.columns c
        JOIN sys.types t ON c.user_type_id = t.user_type_id
        JOIN sys.objects o ON c.object_id = o.object_id
        WHERE o.name = @tableName
            AND o.type = 'U'
            AND t.name NOT IN ('text', 'image', 'ntext')
            AND c.name NOT IN ('Rowid')

        OPEN col_cursor
        FETCH NEXT FROM col_cursor INTO @columnName, @dataType

        WHILE @@FETCH_STATUS = 0
        BEGIN
            DECLARE @filter NVARCHAR(MAX)

            SET @columnList += ', [' + @columnName + ']'

            -- Condition building for each data type 
            IF @dataType IN ('nvarchar', 'varchar', 'nchar', 'char', 'text')
                BEGIN
                    SET @filter = 'ISNULL(NULLIF(CAST(t.[' + @columnName + '] AS NVARCHAR(MAX)), ''''), NULL) IS NOT NULL'
                    SET @defaultValue = ''''''
                END
            ELSE IF @dataType IN ('int', 'bigint', 'smallint', 'tinyint')
                BEGIN
                    SET @filter = 't.[' + @columnName + '] IS NOT NULL AND t.[' + @columnName + '] <> 0'
                    SET @defaultValue = '0'
                END
            ELSE IF @dataType IN ('float', 'real', 'decimal', 'numeric')
                BEGIN
                    SET @filter = 't.[' + @columnName + '] IS NOT NULL AND t.[' + @columnName + '] <> 0'
                    SET @defaultValue = '0.0'
                END
            ELSE IF @dataType = 'bit'
                BEGIN
                    SET @filter = 't.[' + @columnName + '] IS NOT NULL'
                    SET @defaultValue = '0'
                END
            ELSE IF @dataType IN ('datetime', 'smalldatetime', 'date', 'datetime2')
                BEGIN
                    SET @filter = 't.[' + @columnName + '] IS NOT NULL'
                    SET @defaultValue = '''@defaultDate''' 
                END
            ELSE
                BEGIN
                    SET @filter = 't.[' + @columnName + '] IS NOT NULL'
                    SET @defaultValue = 'NULL'
                END

            -- Correlated subquery generation 
            SET @applyList += '
                OUTER APPLY (
                    SELECT TOP 1 ISNULL(t.[' + @columnName + '], ' + @defaultValue + ') AS [' + @columnName + ']
                    FROM ' + @tableName + ' t
                    WHERE t.Pl_name = src.Pl_name AND ' + @filter + '
                    ORDER BY t.LatestDate DESC
                ) AS [' + @columnName + '_latest]'


            -- Building the list of selected columns
            IF @dataType IN ('nvarchar', 'varchar', 'nchar', 'char', 'text')
                BEGIN
                    SET @selectList += ', ISNULL([' + @columnName + '_latest].[' + @columnName + '], '''')'
                END
            ELSE IF @dataType IN ('datetime', 'smalldatetime', 'date', 'datetime2')
                BEGIN
                    SET @selectList += ', ISNULL([' + @columnName + '_latest].[' + @columnName + '], ''' + CONVERT(NVARCHAR(10), @defaultDate, 120) + ''')'
                END
            ELSE
                BEGIN
                    SET @selectList += ', ISNULL([' + @columnName + '_latest].[' + @columnName + '], ' + @defaultValue + ')'
                END


            FETCH NEXT FROM col_cursor INTO @columnName, @dataType
        END

        CLOSE col_cursor
        DEALLOCATE col_cursor

        -- Building the complete query
        SET @sql = '
            INSERT INTO NASAExoplanetCatalogUniquePlanets (' + STUFF(@columnList, 1, 2, '') + ')
            SELECT ' + STUFF(@selectList, 1, 2, '') + '
            FROM (
                SELECT DISTINCT Pl_name FROM ' + @tableName + '
            ) AS src
            ' + @applyList + '
            OPTION (RECOMPILE);'


        TRUNCATE TABLE NASAExoplanetCatalogUniquePlanets
        EXEC sp_executesql @sql

        
        
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
            'An error occurred during handling error in ' + @FuncProcErr + ' stored procedure: ' + @FullEerrorMessage;
        
            THROW 52000, @FullEerrorMessage, 0;
        END CATCH
    END CATCH
END
