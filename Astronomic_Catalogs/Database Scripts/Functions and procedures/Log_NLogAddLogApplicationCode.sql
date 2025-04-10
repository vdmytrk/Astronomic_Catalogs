
--=======================================================================================================================================--
--==========================================================  STORED PRCEDURE  ==========================================================--
--=======================================================================================================================================--



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
	-- For error hendling
	DECLARE @FuncProc AS VARCHAR(50), @Line AS INT, @ErrorNumber AS INT, @ErrorMessage VARCHAR(MAX), @ErrorSeverity INT, @ErrorState INT;

	DECLARE @insetTime DATETIME = GETUTCDATE();
	DECLARE @trancount INT;
	SET @trancount = @@TRANCOUNT; 

	BEGIN TRY 
		SET NOCOUNT ON; -- To reduce the load on the server
		
		IF @trancount = 0
			BEGIN
				BEGIN TRANSACTION NGCIC_NLogAddLogApplicationCode;
			END;
        ELSE
			BEGIN
				COMMIT TRANSACTION NGCIC_NLogAddLogApplicationCode;
				THROW 50001, 'There was an error becose there is more then one open transaction.', 1; 
			END;

			BEGIN
				INSERT INTO NLogApplicationCode 
				   (logged, level, ip, machineName, sessionId, logger, controller, action, method, exception, message, activityId, scope)
				VALUES
				   (@logged, @level, @ip, @machineName, @sessionId, @logger, @controller, @action, @method, @exception, @message, @activityId, @scope);
			END;
		COMMIT TRANSACTION NGCIC_NLogAddLogApplicationCode;
	END TRY
	BEGIN CATCH
		BEGIN TRY
			DECLARE @xstate INT;
			SET @xstate = XACT_STATE(); 

			IF @xstate = -1
				BEGIN
					PRINT 'EXCEPTION STATE: --> IF(1): @xstate = ' + CAST(@xstate AS VARCHAR);
					ROLLBACK;
				END;
			IF @xstate = 1 and @trancount = 0
				BEGIN
					PRINT 'EXCEPTION STATE: --> IF(2): @xstate = ' + CAST(@xstate AS VARCHAR) + ', @trancount = ' + CAST(@trancount AS VARCHAR);
					ROLLBACK;
				END;
			IF @xstate = 1 and @trancount > 0				
				BEGIN
					PRINT 'EXCEPTION STATE: --> IF(3): @xstate = ' + CAST(@xstate AS VARCHAR) + ', @trancount = ' + CAST(@trancount AS VARCHAR);
					ROLLBACK TRANSACTION NGC_IC_Opendatasoft_C_Tran;
				END;
				   

			PRINT 'BEGIN CATCH';
			SET @FuncProc = ERROR_PROCEDURE();
			SET @Line = ERROR_LINE();
			SET @ErrorNumber = ERROR_NUMBER();
			SET @ErrorSeverity = ERROR_SEVERITY();
			SET @ErrorState = ERROR_STATE();
			SET @ErrorMessage = ERROR_MESSAGE();

			INSERT INTO LogProcFunc (FuncProc, Line, ErrorNumber, ErrorSeverity, ErrorState, ErrorMessage) 
			VALUES (@FuncProc, @Line, @ErrorNumber, @ErrorSeverity, @ErrorState, @ErrorMessage);
			
		END TRY
		BEGIN CATCH
			PRINT 'An error occurred during handling error from NGC_IC_Opendatasoft_C_Tran transaction: ' + ERROR_MESSAGE();
		END CATCH
	END CATCH
END

