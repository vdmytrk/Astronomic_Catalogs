using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services;
using Astronomic_Catalogs.Services.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Security.Claims;

namespace Astronomic_Catalogs.Data;

public class DatabaseInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseInitializer> _logger;
    private readonly RoleControllerService _roleService;
    private readonly RoleManager<Models.AspNetRole> _roleManager;
    private readonly UserManager<Models.AspNetUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;
    private readonly string _scriptsDirectory;
    private readonly bool _restoreDatabase;

    public DatabaseInitializer(
        ApplicationDbContext context,
        ILogger<DatabaseInitializer> logger,
        RoleControllerService roleService,
        RoleManager<Models.AspNetRole> roleManager,
        UserManager<Models.AspNetUser> userManager,
        IConfiguration configuration,
        IWebHostEnvironment env
        )
    {
        _context = context;
        _logger = logger;
        _roleService = roleService;
        _roleManager = roleManager;
        _userManager = userManager;
        _env = env;
        _configuration = configuration;
        _scriptsDirectory = Path.Combine(env.ContentRootPath, "Database Scripts");
        _restoreDatabase = _configuration.GetValue("InitializationDb:DropAzureTable", false);
    }

    public async Task InitializeDatabaseAsync()
    {
        try
        {
            bool isConnectDb = await _context.Database.CanConnectAsync();

            await (_env.EnvironmentName switch
            {
                "Development" => InitializeDevelopmentAsync(isConnectDb),
                "AzureDevelopment" => InitializeAzureDevelopmentAsync(isConnectDb),
                _ => throw new InvalidOperationException($"Unknown environment: {_env.EnvironmentName}")
            });

            if (isConnectDb)
                await EnsureIdentityDataAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database initialization failed.");
            throw;
        }
    }

    #region Identity
    private async Task EnsureIdentityDataAsync()
    {
        string adminEmail = "TestAdmin@example.com";
        string adminPassword = "Password123!";
        int adminYearOfBirth = 2000;

        await AddRoles();
        await AddAdmin(adminEmail, adminPassword, adminYearOfBirth);
        await AddUserClaims(adminEmail, adminYearOfBirth);
        await AddRoleClaims();
    }

    public async Task AddRoles()
    {
        string[] roles = RoleNames.AllRoles;
        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                var newRole = new Models.AspNetRole() { Name = role };
                _roleService.SetData(newRole);
                var roleResult = await _roleManager.CreateAsync(newRole);
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    var errorMessage = $"Failed to create role '{role}'. Errors: {errors}";
                    throw new InvalidOperationException(errorMessage);
                }

                _logger.LogDebug($"Role '{role}' created successfully.");
            }
        }
    }

    public async Task AddAdmin(string adminEmail, string adminPassword, int adminYearOfBirth)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);
        if (user == null)
        {
            user = new Models.AspNetUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                YearOfBirth = adminYearOfBirth
            };

            string role = RoleNames.Admin.ToString();
            var existingRole = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == role)
                ?? throw new Exception($"The {role} role does not exist and couldn't be added to the TestAdmin user!");

            var result = await _userManager.CreateAsync(user, adminPassword);
            if (result.Succeeded)
            {
                _logger.LogDebug($"Admin user '{adminEmail}' created successfully.");
                await _userManager.AddToRoleAsync(user, existingRole.Name!);
            }
            else
            {
                _logger.LogError($"Failed to create user '{adminEmail}'. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }

    private async Task AddUserClaims(string adminEmail, int adminYearOfBirth)
    {
        var existUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);
        if (existUser != null)
        {
            var existingClaims = await _userManager.GetClaimsAsync(existUser);
            var claimsToAdd = new List<Claim>
            {
                new Claim("Department", "HQ"), // HeadQuarters 
                new Claim("CanUsersAccess", "true"),
                new Claim(ClaimTypes.DateOfBirth, adminYearOfBirth.ToString())
            };

            var newClaims = claimsToAdd.Where(c => !existingClaims.Any(ec => ec.Type == c.Type)).ToList();
            if (newClaims.Any())
                await _userManager.AddClaimsAsync(existUser, newClaims);
        }
    }

    public async Task AddRoleClaims()
    {
        string[] roles = RoleNames.AllRoles;
        var claimsToAdd = new List<Claim>
        {
            new Claim("CanRoleAccess", "true"),
            new Claim("CanRolelModify", "true"),
            new Claim("CanRoleWatch", "true"),
        };

        foreach (var roleName in roles)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role is null)
                throw new Exception($"Adding a claim to the roleName is not possible because the {roleName} role does not exist.");

            var existingClaims = await _roleManager.GetClaimsAsync(role);
            var newClaims = claimsToAdd.Where(c => !existingClaims.Any(ec => ec.Type == c.Type)).ToList();
            foreach (var claim in newClaims)
            {
                await _roleManager.AddClaimAsync(role, claim);
            }
        }
    }
    #endregion

    #region Create and fill database
    private async Task InitializeDevelopmentAsync(bool isConnectDb)
    {
        if (isConnectDb)
        {
            await _context.Database.EnsureDeletedAsync();
        }

        if (await CreateDatabaseAsync())
        {
            await ExecuteAllSqlScriptsAsync();
        }

    }

    private async Task InitializeAzureDevelopmentAsync(bool isConnectDb)
    {
        if (isConnectDb)
        {
            _logger.LogWarning("Database is found. Applying migrations...");
            bool success = _restoreDatabase ? await DropTables() : await TrancateTables();
            await ApplyMigrationsAsync();
            await ExecuteAllSqlScriptsAsync();
        }
        else
        {
            _logger.LogWarning("Database does not exist in Azure.");
        }
    }

    /// <summary>
    /// Truncate tables which are not created by script.
    /// </summary>
    public async Task<bool> TrancateTables()
    {
        bool success = true;
        string[] tabels = [
                "CollinderCatalog",
                "Constellation",
                "NameObject",
                "NGCICOpendatasoft",
                "NGCICOpendatasoft_Extension",
                "SourceType",
                "DatabaseInitialization"
            ];

        var existingTables = await GetExistingTablesAsync(tabels);

        if (!existingTables.Any())
        {
            _logger.LogWarning("No tables found to truncate.");
            return success;
        }

        List<string> failedTables = new();
        foreach (string table in existingTables)
        {
            try
            {
                string sql = $"TRUNCATE TABLE {table}";
                await _context.Database.ExecuteSqlRawAsync(sql);
            }
            catch (Exception ex)
            {
                failedTables.Add(table);
                _logger.LogError(ex, $"The table {table} was not truncated with exception: {ex.Message}.");
            }
        }

        if (failedTables.Any())
        {
            success = false;
            throw new Exception($"Failed to delete the following tabels: {string.Join(", ", failedTables)}");
        }

        return success;
    }

    /// <summary>
    /// Drop tables which are not created by script.
    /// </summary>
    public async Task<bool> DropTables()
    {
        bool success = true;

        try
        {
            string sql = $"DELETE FROM __EFMigrationsHistory";
            await _context.Database.ExecuteSqlRawAsync(sql);
            _logger.LogInformation("Table '__EFMigrationsHistory' was cleaned up.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to clean '__EFMigrationsHistory'. EF will skip migrations.");
        }

        string[] dropTables = [
            "AspNetRoleClaims",
            "AspNetRoles",
            "AspNetUserClaims",
            "AspNetUserLogins",
            "AspNetUserRoles",
            "AspNetUsers",
            "AspNetUserTokens",
            "CollinderCatalog",
            "CollinderCatalog_Temporarily",
            "Constellation",
            "DatabaseInitialization",
            "DateTable",
            "LogProcFunc",
            "NameObject",
            "NASAExoplanetCatalog",
            "NASAExoplanetCatalogLastUpdate",
            "NASAExoplanetCatalogUniquePlanets",
            "NASAPlanetarySystemsPlanets",
            "NASAPlanetarySystemsStars",
            "NGC2000_UKTemporarily",
            "NGC2000_UKTemporarilySource",
            "NGCICOpendatasoft",
            "NGCICOpendatasoft_Extension",
            "NGCICOpendatasoft_Source",
            "NGCWikipedia_ExtensionTemporarilySource",
            "NGCWikipedia_TemporarilySource",
            "NLogApplicationCode",
            "PlanetarySystemsCatalog",
            "RequestLogs", 
            "SourceType",
            "TestConnectionForNLog",
            "UserLogs"
        ];

        // Disabling all constraints to delete dropTables.
        string disableConstraintsSql = @"
            DECLARE @sql NVARCHAR(MAX) = N'';

            SELECT @sql += '
            ALTER TABLE [' + TABLE_SCHEMA + '].[' + TABLE_NAME + '] NOCHECK CONSTRAINT ALL;'
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_TYPE = 'BASE TABLE';

            EXEC sp_executesql @sql;
            ";

        await _context.Database.ExecuteSqlRawAsync(disableConstraintsSql);
                
        string dropFKsSql = $@"
            DECLARE @sql NVARCHAR(MAX) = N'';

            SELECT @sql += '
            ALTER TABLE [' + sch.name + '].[' + t.name + '] DROP CONSTRAINT [' + fk.name + '];'
            FROM sys.foreign_keys fk
            INNER JOIN sys.tables t ON fk.parent_object_id = t.object_id
            INNER JOIN sys.schemas sch ON t.schema_id = sch.schema_id
            WHERE t.name IN ({string.Join(",", dropTables.Select(t => $"'{t}'"))});

            EXEC sp_executesql @sql;
        ";

        await _context.Database.ExecuteSqlRawAsync(dropFKsSql);

        var existingTables = await GetExistingTablesAsync(dropTables);

        if (!existingTables.Any())
        {
            _logger.LogWarning("No dropTables found to drop.");
            return success;
        }

        foreach (string table in existingTables)
        {
            try
            {
                string sql = $"DROP TABLE [{table}]";
                await _context.Database.ExecuteSqlRawAsync(sql);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"The table {table} was not dropped: {ex.Message}.");
            }
        }

        return success;
    }

    private async Task<List<string>> GetExistingTablesAsync(string[] tabels)
    {
        List<string> existingTables = new();

        using var connection = new SqlConnection(_context.Database.GetConnectionString()); // Використовуємо нове з'єднання
        await connection.OpenAsync();

        try
        {
            string joinedTableNames = string.Join(", ", tabels.Select(t => $"'{t}'"));
            string query = $@"
            SELECT TABLE_NAME 
            FROM INFORMATION_SCHEMA.TABLES 
            WHERE TABLE_NAME IN ({joinedTableNames})";

            using var command = connection.CreateCommand();
            command.CommandText = query;

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                existingTables.Add(reader.GetString(0));
            }
        }
        finally
        {
            await connection.CloseAsync();
        }

        return existingTables;
    }

    private async Task<bool> CreateDatabaseAsync()
    {
        try
        {
            await _context.Database.EnsureCreatedAsync();
            _logger.LogDebug("Database created successfully.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create database.");
            return false;
        }
    }

    private async Task ApplyMigrationsAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
            _logger.LogInformation("Database migrations applied successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply database migrations.");
            throw;
        }
    }

    public async Task ExecuteStoredProcedureAsync(string storedProcedure)
    {
        try
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC {0}", storedProcedure);
            _logger.LogDebug($"Stored procedure {storedProcedure} executed successfully.");
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, $"Database update error while executing stored procedure {storedProcedure}.");
            throw;
        }
        catch (SqlException sqlEx)
        {
            _logger.LogError(sqlEx, $"SQL error while executing stored procedure {storedProcedure}: {sqlEx.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected error while executing stored procedure {storedProcedure}.");
            throw;
        }
    }

    public async Task<bool> ExecuteSqlScriptAsync(string scriptName)
    {
        string scriptPath = $"{_scriptsDirectory}/{scriptName}";

        if (!File.Exists(scriptPath))
        {
            _logger.LogWarning($"Script file {scriptPath} not found. Skipping.");
            return false;
        }

        int? defaultTimeout = null;
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                var sql = await File.ReadAllTextAsync(scriptPath);
                if (string.IsNullOrWhiteSpace(sql))
                {
                    _logger.LogWarning($"Script {scriptPath} is empty. Skipping.");
                    return false;
                }

                _logger.LogDebug($"Executing script: {scriptPath}");

                if (scriptPath.EndsWith("Data/NGC2000_UKTemporarily.sql"))
                {
                    defaultTimeout = _context.Database.GetCommandTimeout();
                    _context.Database.SetCommandTimeout(600);
                }

                await _context.Database.ExecuteSqlRawAsync(sql);
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error executing script {scriptPath}. Rolling back transaction.");
                return false;
            }
            finally
            {
                if (defaultTimeout.HasValue)
                {
                    _context.Database.SetCommandTimeout(defaultTimeout.Value);
                }
            }
        }
    }

    private async Task ExecuteSqlScriptIfNeededAsync(string scriptPath, string propertyName, DatabaseInitialization? initStatus)
    {
        PropertyInfo? prop = null;

        if (initStatus is not null)
        {
            prop = typeof(DatabaseInitialization).GetProperty(propertyName);
            if (prop == null)
            {
                _logger.LogError($"Property {propertyName} not found in DatabaseInitialization model.");
                return;
            }

            bool isExecuted = (bool)(prop.GetValue(initStatus) ?? false);
            if (isExecuted)
            {
                _logger.LogInformation($"Skipping execution of {scriptPath} because it's already executed.");
                return;
            }
        }

        bool success = await ExecuteSqlScriptAsync(scriptPath);
        if (success && prop is not null)
        {
            prop.SetValue(initStatus, true);
            await _context.SaveChangesAsync();
        }
    }
    #endregion

    #region SQL
    private async Task ExecuteAllSqlScriptsAsync()
    {
        await ExecuteCreateTableSqlScriptsAsync();
        await ExecuteInsertDataSqlScriptsAsync();
        await ExecuteCreateProcedureFunctionSqlScriptsAsync();
        await ExecuteInsertDataSqlByStoredProcedureAsync();
    }

    /// <summary>
    /// Create tabels that do not have a model.
    /// </summary>
    private async Task ExecuteCreateTableSqlScriptsAsync()
    {
        await ExecuteSqlScriptAsync($"Tables/NGC2000_UKTemporarilySource.sql");
        await ExecuteSqlScriptAsync($"Tables/NGC2000_UKTemporarily.sql");
        await ExecuteSqlScriptAsync($"Tables/CollinderCatalog_Temporarily.sql");
        await ExecuteSqlScriptAsync($"Tables/NGCWikipedia_TemporarilySource.sql");
        await ExecuteSqlScriptAsync($"Tables/NGCWikipedia_ExtensionTemporarilySource.sql");
        await ExecuteSqlScriptAsync($"Tables/NGCICOpendatasoft_Source.sql");
        await ExecuteSqlScriptAsync($"Tables/LogProcFunc.sql");
        await ExecuteSqlScriptAsync($"Tables/RequestLog.sql");
        await ExecuteSqlScriptAsync($"Tables/UsertLog.sql");
        await ExecuteSqlScriptAsync($"Tables/NASAExoplanetCatalogLastUpdate.sql");
        await ExecuteSqlScriptAsync($"Tables/NASAExoplanetCatalogUniquePlanets.sql");
    }

    private async Task ExecuteInsertDataSqlScriptsAsync()
    {
        var initStatus = await GetOrCreateInitStatusAsync();

        await ExecuteSqlScriptIfNeededAsync("Data/SourceType.sql", nameof(DatabaseInitialization.Is_SourceType_Executed), initStatus);
        await ExecuteSqlScriptIfNeededAsync("Data/NGC2000_UKTemporarilySource.sql", nameof(DatabaseInitialization.Is_NGC2000_UKTemporarilySource_Executed), initStatus);
        await ExecuteSqlScriptIfNeededAsync("Data/NameObject.sql", nameof(DatabaseInitialization.Is_NameObject_Executed), initStatus);
        await ExecuteSqlScriptIfNeededAsync("Data/Constellation.sql", nameof(DatabaseInitialization.Is_Constellation_Executed), initStatus);
        await ExecuteSqlScriptIfNeededAsync("Data/NGC2000_UKTemporarily.sql", nameof(DatabaseInitialization.Is_NGC2000_UKTemporarily_Executed), initStatus);
        await ExecuteSqlScriptIfNeededAsync("Data/CollinderCatalog_Temporarily.sql", nameof(DatabaseInitialization.Is_CollinderCatalog_Temporarily_Executed), initStatus);
        await ExecuteSqlScriptIfNeededAsync("Data/NGCWikipedia_TemporarilySource.sql", nameof(DatabaseInitialization.Is_NGCWikipedia_TemporarilySource_Executed), initStatus);
        await ExecuteSqlScriptIfNeededAsync("Data/NGCWikipedia_ExtensionTemporarilySource.sql", nameof(DatabaseInitialization.Is_NGCWikipedia_ExtensionTemporarilySource_Executed), initStatus);
        await ExecuteSqlScriptIfNeededAsync("Data/NGCICOpendatasoft_Source.sql", nameof(DatabaseInitialization.Is_NGCICOpendatasoft_Source_Executed), initStatus);
    }

    private async Task ExecuteCreateProcedureFunctionSqlScriptsAsync()
    {
        await ExecuteSqlScriptAsync($"Functions and procedures/CalculationPlanetarySystemData.sql");
        await ExecuteSqlScriptAsync($"Functions and procedures/CreateNewDate.sql");
        await ExecuteSqlScriptAsync($"Functions and procedures/FillNASAExoplanetCatalogLastUpdate.sql");
        await ExecuteSqlScriptAsync($"Functions and procedures/FillNASAExoplanetCatalogUniquePlanets.sql");
        await ExecuteSqlScriptAsync($"Functions and procedures/GetActualDate.sql");
        await ExecuteSqlScriptAsync($"Functions and procedures/GetFilteredCollinderData.sql");
        await ExecuteSqlScriptAsync($"Functions and procedures/GetFilteredNGCICData.sql");
        await ExecuteSqlScriptAsync($"Functions and procedures/GetFilteredPlanetarySystemsData.sql");
        await ExecuteSqlScriptAsync($"Functions and procedures/GetFilteredPlanetsData.sql");
        await ExecuteSqlScriptAsync($"Functions and procedures/InsertCollinderCatalog.sql");
        await ExecuteSqlScriptAsync($"Functions and procedures/InsertNGCICOpendatasoft.sql");
        await ExecuteSqlScriptAsync($"Functions and procedures/InsertNGCICOpendatasoft_Extension.sql");
        await ExecuteSqlScriptAsync($"Functions and procedures/Log_NLogAddLogApplicationCode.sql");
        await ExecuteSqlScriptAsync($"Functions and procedures/MigrateNGCICOStoNGCICO_Cursor.sql");
        await ExecuteSqlScriptAsync($"Functions and procedures/MigrateNGCICOStoNGCICO_While.sql");
        await ExecuteSqlScriptAsync($"Functions and procedures/Recreate_NASAPlanetarySystems_Tables.sql");
        await ExecuteSqlScriptAsync($"Functions and procedures/DataBugFixes.sql");
    }

    private async Task ExecuteInsertDataSqlByStoredProcedureAsync()
    {
        var initStatus = await GetOrCreateInitStatusAsync();

        await ExecuteStoredProcedureAsync("InsertCollinderCatalog");
        await ExecuteStoredProcedureAsync("MigrateNGCICOStoNGCICO_W");
    }

    private async Task<DatabaseInitialization?> GetOrCreateInitStatusAsync()
    {
        var existingDbInit = await GetExistingTablesAsync(["DatabaseInitialization"]);

        if (!_restoreDatabase || existingDbInit.Any())
        {
            var initStatus = await _context.DatabaseInitialization.FirstOrDefaultAsync(x => x.Id == 1);
            if (initStatus == null)
            {
                initStatus = new DatabaseInitialization();
                _context.DatabaseInitialization.Add(initStatus);
                await _context.SaveChangesAsync();
            }
            return initStatus;
        }

        return null;
    }
    #endregion

}
