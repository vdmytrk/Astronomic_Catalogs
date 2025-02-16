
--=======================================================================================================================================--
--===========================================  LOGGING THE REQUESTS TO FUNCTION AND PRCEDURE  ===========================================--
--=======================================================================================================================================--




IF OBJECT_ID('dbo.LogProcFunc', 'U') IS NOT NULL
BEGIN
	TRUNCATE TABLE LogProcFunc;
END
ELSE 
BEGIN
	CREATE TABLE LogProcFunc (
		[Id] INT NOT NULL IDENTITY,
		[Time] DATETIME DEFAULT GETUTCDATE(),
		[FuncProc] VARCHAR(100) DEFAULT NULL, 
		[Line] INT DEFAULT 0,  
		[ErrorNumber] INT DEFAULT 0, 
		[ErrorSeverity] INT, 
		[ErrorState] INT,
		[ErrorMessage] NVARCHAR(MAX) DEFAULT ''
	);
END


