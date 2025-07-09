--=======================================================================================================================================--
--==========================================================  STORED PRCEDURE  ==========================================================--
--=======================================================================================================================================--
-------------------------------------------------------------------------------------------------------------------------------------------
--=============================  RECREATE NASAPlanetarySystemsStars AND NASAPlanetarySystemsPlanets TABLES  =============================--
-------------------------------------------------------------------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE Recreate_NASAPlanetarySystems_Tables
AS
BEGIN
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

		IF OBJECT_ID('dbo.NASAPlanetarySystemsPlanets', 'U') IS NOT NULL
		BEGIN

			IF EXISTS (
				SELECT * 
				FROM sys.foreign_keys 
				WHERE [name] = 'FK_NASAPlanetarySystemsPlanets_StarId'
			)
			BEGIN
				ALTER TABLE dbo.NASAPlanetarySystemsPlanets
				DROP CONSTRAINT FK_NASAPlanetarySystemsPlanets_StarId;
			END;


			DROP TABLE dbo.NASAPlanetarySystemsPlanets;
		END;

		IF OBJECT_ID('dbo.NASAPlanetarySystemsStars', 'U') IS NOT NULL
		BEGIN
			DROP TABLE dbo.NASAPlanetarySystemsStars;
		END;
		


		
		CREATE TABLE NASAPlanetarySystemsStars (
			[Id] INT IDENTITY PRIMARY KEY,
			[Hostname] NVARCHAR(255) UNIQUE, 

			[Hd_name] NVARCHAR(MAX),
			[Hip_name] NVARCHAR(MAX),
			[Tic_id] NVARCHAR(MAX),
			[Gaia_id] NVARCHAR(MAX),

			[St_spectype] NVARCHAR(MAX),
			[St_teff] DECIMAL(18, 6),
			[St_rad] DECIMAL(18, 6),
			[St_mass] DECIMAL(18, 6),
			[St_met] DECIMAL(18, 6),
			[St_metratio] NVARCHAR(MAX),
			[St_lum] DECIMAL(18, 6),
			[St_age] DECIMAL(18, 6),
			[Sy_dist] DECIMAL(18, 6),

			[St_lum_Sun_Absol] DECIMAL(18, 6),
			[HabitablZone] DECIMAL(18, 6)
		);

		CREATE TABLE dbo.NASAPlanetarySystemsPlanets (
			[Pl_name] NVARCHAR(255) UNIQUE,
			[StarId] INT NOT NULL,

			[Pl_letter] NVARCHAR(1),

			[Pl_orbsmax] DECIMAL(18, 6),
			[Pl_rade] DECIMAL(18, 6),
			[Pl_radj] DECIMAL(18, 6),
			[Pl_masse] DECIMAL(18, 6),
			[Pl_massj] DECIMAL(18, 6),

			[LatestDate] DATE,
			[InHabitablZone] BIT
		);
			   		 
		ALTER TABLE NASAPlanetarySystemsPlanets
		ADD CONSTRAINT FK_NASAPlanetarySystemsPlanets_StarId 
				FOREIGN KEY ([StarId]) 
				REFERENCES dbo.NASAPlanetarySystemsStars([Id]) 
				ON DELETE CASCADE

	END TRY
	BEGIN CATCH 				
			SET @ErrorMessage = 'An error occurred during handling error from OuterTransaction transaction INTO INNER STORES PROCEDURE: ' +
				' Error_number: ' + CAST(ERROR_NUMBER() AS VARCHAR(10)) + 
				' Error_message: ' + CAST(ERROR_MESSAGE() AS NVARCHAR(MAX)) +
				' Error_severity: ' + CAST(ERROR_SEVERITY() AS VARCHAR(2)) +
				' Error_state: ' +  CAST(ERROR_STATE() AS VARCHAR(3)) + 
				' Error_line: ' + CAST(ERROR_LINE() AS VARCHAR(10));
		THROW 50003, @ErrorMessage, 3;
	END CATCH
END