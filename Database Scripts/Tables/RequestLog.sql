

USE [AstroCatalogsDB]
GO



--=========================================================================================================================================
-----------------------------------------------------     NGC2000_UKTemporarily      -----------------------------------------------------
--=========================================================================================================================================

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


--drop table RequestLogs;
IF OBJECT_ID('dbo.RequestLogs', 'U') IS NOT NULL
BEGIN
	TRUNCATE TABLE RequestLogs;
END
ELSE 
BEGIN
	CREATE TABLE [dbo].[RequestLogs](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[IpAddress] [nvarchar](max) NOT NULL,
		[UserAgent] [nvarchar](max) NOT NULL,
		[RequestTime] [datetime2](7) NOT NULL,
		[UserName] [nvarchar](max) NOT NULL,
		[Path] [nvarchar](max) NOT NULL,
		[Method] [nvarchar](max) NOT NULL,
		[Referer] [nvarchar](max) NOT NULL,
		[StatusCode] [int] NOT NULL,
		[ErrorMessage] [nvarchar](max) NOT NULL,
	 CONSTRAINT [PK_RequestLogs] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


	ALTER TABLE [dbo].[RequestLogs] ADD  DEFAULT (sysdatetime()) FOR [RequestTime]

END





