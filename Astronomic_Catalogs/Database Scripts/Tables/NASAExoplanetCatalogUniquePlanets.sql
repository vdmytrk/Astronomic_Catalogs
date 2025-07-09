
--=========================================================================================================================================
------------------------------------------------     NASAExoplanetCatalogUniquePlanets     ------------------------------------------------
--=========================================================================================================================================





IF OBJECT_ID('dbo.NASAExoplanetCatalogUniquePlanets', 'U') IS NOT NULL
BEGIN
	TRUNCATE TABLE NASAExoplanetCatalogUniquePlanets;
END
ELSE 
BEGIN
	CREATE TABLE NASAExoplanetCatalogUniquePlanets (
		[Pl_name] NVARCHAR(255), 
		[Hostname] NVARCHAR(MAX),
		[Pl_letter] NVARCHAR(1),
		[Hd_name] NVARCHAR(MAX),
		[Hip_name] NVARCHAR(MAX),
		[Tic_id] NVARCHAR(MAX),
		[Gaia_id] NVARCHAR(MAX),

		[Pl_orbsmax] DECIMAL(18, 6),
		[Pl_rade] DECIMAL(18, 6),
		[Pl_radj] DECIMAL(18, 6),
		[Pl_masse] DECIMAL(18, 6),
		[Pl_massj] DECIMAL(18, 6),
		[St_spectype] NVARCHAR(MAX),
		[St_teff] DECIMAL(18, 6),
		[St_rad] DECIMAL(18, 6),
		[St_mass] DECIMAL(18, 6),
		[St_met] DECIMAL(18, 6),
		[St_metratio] NVARCHAR(MAX),
		[St_lum] DECIMAL(18, 6),
		[St_age] DECIMAL(18, 6),
		[Sy_dist] DECIMAL(18, 6),

		LatestDate DATE
	);

	
	CREATE CLUSTERED INDEX IX_OrderedByDate_PlName 
    ON NASAExoplanetCatalogUniquePlanets (Pl_name, LatestDate DESC);

END
