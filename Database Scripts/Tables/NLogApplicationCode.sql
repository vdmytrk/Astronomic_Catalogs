
--=========================================================
-- Table for logging application code via NLog.
--=========================================================





IF OBJECT_ID('dbo.NLogApplicationCode', 'U') IS NOT NULL
BEGIN
	TRUNCATE TABLE NLogApplicationCode;
END
ELSE 
BEGIN
	CREATE TABLE NLogApplicationCode(
		[Id] int IDENTITY(1,1) NOT NULL,
		[CreatedOn] datetime2(7) NOT NULL, -- Paired with the [logged] field for understanding the logging process.
		[Logged] datetime2(7) NOT NULL, 
		[Level] varchar(10) NULL,
		[Ip] varchar(50) NULL,
		[MachineName] nvarchar(255) NULL,
		[SessionId] nvarchar(255) NULL,
		[Logger] varchar(300) NULL,
		[Controller] varchar(100) NULL,
		[Action] varchar(50) NULL,
		[Method] varchar(300) NULL,
		[Exception] nvarchar(max) NULL,
		[Message] nvarchar(max) NULL,
		[ActivityId] nvarchar(50) NULL,
		[Scope] varchar(max) NULL,
		CONSTRAINT [PK_NLog] PRIMARY KEY CLUSTERED ([id]) WITH 
		(
			IGNORE_DUP_KEY = OFF,
			ALLOW_PAGE_LOCKS = OFF
		) 
	);

	ALTER TABLE [NLogApplicationCode] 
		ADD CONSTRAINT [DF_NLog_createdOn]  DEFAULT (getutcdate()) FOR [createdOn];
END
GO