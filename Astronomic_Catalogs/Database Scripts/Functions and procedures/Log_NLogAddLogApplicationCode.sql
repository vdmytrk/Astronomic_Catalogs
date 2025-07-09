--=======================================================================================================================================--
--==========================================================  STORED PRCEDURE  ==========================================================--
--=======================================================================================================================================--
-------------------------------------------------------------------------------------------------------------------------------------------
--===============================================  LOGGING DATA INTO NLogApplicationCode  ===============================================--
-------------------------------------------------------------------------------------------------------------------------------------------


--=========================================================
-- The stored procedure for loging application code by NLog.
--=========================================================
CREATE OR ALTER PROC NLogAddLogApplicationCode
    @logged nvarchar(max) = NULL,
    @level nvarchar(max) = NULL,
    @ip nvarchar(max) = NULL,
    @machineName nvarchar(max) = NULL,
    @sessionId nvarchar(max) = NULL,
    @logger nvarchar(max) = NULL,
    @controller nvarchar(max) = NULL,
    @action nvarchar(max) = NULL,
    @method nvarchar(max) = NULL,
    @exception nvarchar(max) = NULL,
    @message nvarchar(max) = NULL,
    @activityId nvarchar(max) = NULL,
    @scope nvarchar(max) = NULL
AS
BEGIN
    SET NOCOUNT ON; 

    BEGIN TRY 
        INSERT INTO NLogApplicationCode 
            (Logged, Level, Ip, Machinename, Sessionid, Logger, Controller, Action, Method, Exception, Message, Activityid, Scope)
        VALUES
            (@logged, @level, @ip, @machineName, @sessionId, @logger, @controller, @action, @method, @exception, @message, @activityId, @scope);
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

