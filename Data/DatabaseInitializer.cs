using Astronomic_Catalogs.Infrastructure;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services;
using Astronomic_Catalogs.Services.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Astronomic_Catalogs.Data;

public class DatabaseInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseInitializer> _logger;
    private readonly UserControllerService _userService;
    private readonly RoleControllerService _roleService;
    private readonly RoleManager<AspNetRole> _roleManager;
    private readonly UserManager<AspNetUser> _userManager;

    public DatabaseInitializer(
        ApplicationDbContext context,
        ILogger<DatabaseInitializer> logger,
        UserControllerService userService,
        RoleControllerService roleService,
        RoleManager<AspNetRole> roleManager,
        UserManager<AspNetUser> userManager
        )
    {
        _context = context;
        _logger = logger;
        _userService = userService;
        _roleService = roleService;
        _roleManager = roleManager;
        _userManager = userManager;
    }

#if !DEBUG
        private readonly string _scriptsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database Scripts");
#else
    private readonly string _scriptsDirectory = "Database Scripts";
#endif




    /// <summary>
    ///                ATTENTION!!!
    /// 
    /// In Entity Framework 9.0, the method Database.MigrateAsync() has a bug. It does not apply migrations. 
    /// If you encounter a database access issue while executing this method, refer to README.md.
    /// 
    /// As a partial solution to this issue, this method implements multi-level verification and execution of SQL scripts.
    /// </summary>
    public async Task InitializeDatabaseAsync()
    {
        bool canConnect = await _context.Database.CanConnectAsync();
        try
        {
            if (!canConnect)
            {
                _logger.LogWarning("Database does not exist or is not accessible. Creating database...");
                await ExecuteMigrationScenarioAsync(canConnect);
                return;
            }
            // To create the database using migration through the Package Manager Console, refer to the Areas\Admin\Models\README.md file.
            await ApplyMigrationsAsync();

            await EnsureIdentityDataAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during database initialization.");
            throw;
        }
    }

    private async Task EnsureIdentityDataAsync()
    {
        string[] roles = RoleNames.AllRoles;

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                var newRole = new AspNetRole() { Name = role };
                _roleService.SetData(newRole);
                var roleResult = await _roleManager.CreateAsync(newRole);
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    var errorMessage = $"Failed to create role '{role}'. Errors: {errors}";
                    throw new InvalidOperationException(errorMessage);
                }

                _logger.LogInformation($"Role '{role}' created successfully.");
            }
        }

        string adminUserName = "TestAdmin@example.com";
        string adminEmail = "TestAdmin@example.com";
        string adminPassword = "Password123!";

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);
        if (user == null)
        {
            user = new AspNetUser
            {
                UserName = adminUserName,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var existingRole = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == "OwnerAdministratorPower")
                ?? throw new Exception("The OwnerAdministratorPower role does not exist and couldn't be added to the TestAdmin user!");

            var result = await _userManager.CreateAsync(user, adminPassword);
            if (result.Succeeded)
            {
                _logger.LogInformation($"Admin user '{adminUserName}' created successfully.");

                string role = RoleNames.Admin.ToString();
                await _userManager.AddToRoleAsync(user, existingRole.Name!);
                await _userManager.AddClaimsAsync(user, new[]
                {
                    new Claim("Department", "HQ"), // Headquarters 
                    new Claim("CanEditUsers", "true"),
                    new Claim("DateOfBirth", "2000")
                });
            }
            else
            {
                _logger.LogError($"Failed to create user '{adminUserName}'. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }

    private async Task ExecuteMigrationScenarioAsync(bool canConnect)
    {
        try
        {
            await ApplyMigrationsAsync();
            return;
        }
        catch (SqlException sqlEx) when (sqlEx.Number == 4060)
        {
            _logger.LogError(sqlEx, "Cannot connect to database. Attempting to create a new database.");
        }
        catch (Exception ex) when (ex.Message.Contains("PendingModelChangesWarning"))
        {
            _logger.LogWarning(ex, "Model changes detected, but no migration was added. Applying SQL scripts manually.");
            await ExecuteManuallyDbInitalization(canConnect);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
            throw;
        }
    }

    private async Task ApplyMigrationsAsync()
    {
        try
        {
            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                _logger.LogInformation("Applying pending migrations...");
                await _context.Database.MigrateAsync();
                _logger.LogInformation("Migrations applied successfully.");
            }
            else
            {
                _logger.LogInformation("No pending migrations to apply.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during migration.");
            throw;
        }
    }

    private async Task ExecuteManuallyDbInitalization(bool canConnect)
    {
        try
        {
            if (!canConnect)
            {
                await _context.Database.EnsureCreatedAsync();
            }
            await ExecuteSqlScriptsAsync();
            await ExecuteStoredProcedureAsync("MigrateNGCICOStoNGCICO_W");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while executing additional SQL scripts or stored procedures.");
            throw;
        }
    }

    private async Task ExecuteStoredProcedureAsync(string storedProcedure)
    {
        try
        {
            _logger.LogInformation($"Executing stored procedure: {storedProcedure}...");

            await _context.Database.ExecuteSqlRawAsync("EXEC {0}", storedProcedure);

            _logger.LogInformation($"Stored procedure {storedProcedure} executed successfully.");
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

    private async Task ExecuteSqlScriptAsync(string scriptName)
    {
        string scriptPath = $"{_scriptsDirectory}/{scriptName}"; // TODO: DO NOT WORK IN Azure!!!

        if (!File.Exists(scriptPath))
        {
            _logger.LogWarning($"Script file {scriptPath} not found. Skipping.");
            throw new Exception($"Script file {scriptPath} not found. Skipping.");
        }

        var sql = await File.ReadAllTextAsync(scriptPath);
        if (!string.IsNullOrWhiteSpace(sql))
        {
            int? defaultTimeout = null;
            _logger.LogInformation($"Executing script: {scriptPath}");

            try
            {
                if (
                    scriptPath == "Database Scripts/Data/NGC2000_UKTemporarily.sql" // TODO: DO NOT WORK IN Azure!!!
                   )
                {
                    defaultTimeout = _context.Database.GetCommandTimeout();
                    _context.Database.SetCommandTimeout(600);
                }
                await _context.Database.ExecuteSqlRawAsync(sql);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during migration.");
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

    #region SQL
    private async Task ExecuteSqlScriptsAsync()
    {
        await ExecuteCreateTableSqlScriptsAsync();
        await ExecuteCreateProcedureFunctionSqlScriptsAsync();
        await ExecuteInsertDataSqlScriptsAsync();
    }

    /// <summary>
    /// Create tables that do not have a model.
    /// </summary>
    /// <returns></returns>
    private async Task ExecuteCreateTableSqlScriptsAsync()
    {
        await ExecuteSqlScriptAsync($"Tables/NGC2000_UKTemporarilySource.sql");
        await ExecuteSqlScriptAsync($"Tables/NGC2000_UKTemporarily.sql");
        await ExecuteSqlScriptAsync($"Tables/CollinderCatalog_Temporarily.sql");
        await ExecuteSqlScriptAsync($"Tables/NGCWikipedia_TemporarilySource.sql");
        await ExecuteSqlScriptAsync($"Tables/NGCWikipedia_ExtensionTemporarilySource.sql");
        await ExecuteSqlScriptAsync($"Tables/NGCICOpendatasoft_Source.sql");
    }

    private async Task ExecuteCreateProcedureFunctionSqlScriptsAsync()
    {
        await ExecuteSqlScriptAsync($"Functions and procedures/Log_NLogAddLogApplicationCode.sql");
        await ExecuteSqlScriptAsync($"Functions and procedures/InsertNGCICOpendatasoft.sql");
        await ExecuteSqlScriptAsync($"Functions and procedures/MigrateNGCICOStoNGCICO_Cursor.sql");
        await ExecuteSqlScriptAsync($"Functions and procedures/MigrateNGCICOStoNGCICO_While.sql");
        await ExecuteSqlScriptAsync($"Functions and procedures/SearchFilteredNGCICData.sql");
        await ExecuteSqlScriptAsync($"Functions and procedures/GetActualDate.sql");
        await ExecuteSqlScriptAsync($"Functions and procedures/CreateNewDate.sql");
    }

    private async Task ExecuteInsertDataSqlScriptsAsync()
    {
        await ExecuteSqlScriptAsync($"Data/SourceType.sql");
        await ExecuteSqlScriptAsync($"Data/NGC2000_UKTemporarilySource.sql");
        await ExecuteSqlScriptAsync($"Data/NameObject.sql");
        await ExecuteSqlScriptAsync($"Data/Constellation.sql");
        await ExecuteSqlScriptAsync($"Data/NGC2000_UKTemporarily.sql");
        await ExecuteSqlScriptAsync($"Data/CollinderCatalog_Temporarily.sql");
        await ExecuteSqlScriptAsync($"Data/CollinderCatalog.sql");
        await ExecuteSqlScriptAsync($"Data/NGCWikipedia_TemporarilySource.sql"); // !
        await ExecuteSqlScriptAsync($"Data/NGCWikipedia_ExtensionTemporarilySource.sql");
        await ExecuteSqlScriptAsync($"Data/NGCICOpendatasoft_Source.sql");
    }
    #endregion
}
