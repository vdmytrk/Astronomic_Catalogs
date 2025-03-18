using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services;
using Astronomic_Catalogs.Services.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Astronomic_Catalogs.Data;

public class DatabaseInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseInitializer> _logger;
    private readonly RoleControllerService _roleService;
    private readonly RoleManager<AspNetRole> _roleManager;
    private readonly UserManager<AspNetUser> _userManager;
    private readonly string _scriptsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database Scripts");

    public DatabaseInitializer(
        ApplicationDbContext context,
        ILogger<DatabaseInitializer> logger,
        RoleControllerService roleService,
        RoleManager<AspNetRole> roleManager,
        UserManager<AspNetUser> userManager

        )
    {
        _context = context;
        _logger = logger;
        _roleService = roleService;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task InitializeDatabaseAsync()
    {
        try
        {
            if (!await _context.Database.CanConnectAsync())
            {
                _logger.LogWarning("Database not found. Creating...");
                if (await CreateDatabaseAsync())
                {
                    await ExecuteAllSqlScriptsAsync();
                    await ExecuteStoredProcedureAsync("MigrateNGCICOStoNGCICO_W");
                    await EnsureIdentityDataAsync();
                }
            }
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
                var newRole = new AspNetRole() { Name = role };
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
            user = new AspNetUser
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

    private async Task ExecuteStoredProcedureAsync(string storedProcedure)
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

    private async Task ExecuteSqlScriptAsync(string scriptName)
    {
        string scriptPath = $"{_scriptsDirectory}/{scriptName}"; 

        if (!File.Exists(scriptPath))
        {
            _logger.LogError($"Script file {scriptPath} not found. Skipping.");
            return;
        }

        int? defaultTimeout = null;
        try
        {
            var sql = await File.ReadAllTextAsync(scriptPath);
            if (!string.IsNullOrWhiteSpace(sql))
            {
                _logger.LogDebug($"Executing script: {scriptPath}");

                if (scriptPath.EndsWith("NGC2000_UKTemporarily.sql"))
                {
                    defaultTimeout = _context.Database.GetCommandTimeout();
                    _context.Database.SetCommandTimeout(600);
                }
                await _context.Database.ExecuteSqlRawAsync(sql);
            }
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
    #endregion

    #region SQL
    private async Task ExecuteAllSqlScriptsAsync()
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
