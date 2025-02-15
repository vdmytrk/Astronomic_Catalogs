using Astronomic_Catalogs.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Astronomic_Catalogs.Data;

public class DatabaseInitializer_WithComments
{
    private readonly ApplicationDbContext _context;
    private readonly ConnectionStringProvider _connectionStringProvider;
    private readonly ILogger<DatabaseInitializer_WithComments> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _scriptsDirectory;

    public DatabaseInitializer_WithComments(
        ApplicationDbContext context, 
        ConnectionStringProvider connectionStringProvider, 
        ILogger<DatabaseInitializer_WithComments> logger, 
        IConfiguration configuration)
    {
        _context = context;
        _connectionStringProvider = connectionStringProvider;
        _logger = logger;
#if !DEBUG
        _scriptsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database Scripts");
#else
        _scriptsDirectory = "Database Scripts";
        _configuration = configuration;
#endif
    }



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
        _logger.LogInformation("Checking if database exists...");

        bool canConnect = await _context.Database.CanConnectAsync(); // Перевірка існування БД.
        try
        {
            if (!canConnect) // 2)	БД НЕ ІСНУЄ:
            {
                _logger.LogWarning("Database does not exist or is not accessible. Creating database...");
                await ExecuteMigrationScenarioAsync(canConnect); // FALSE
                return;
            }

            // 1)	БД ІСНУЄ:
            // Щоб реалізувати цей сценарій потрібно додати в кожен скрип по вставці даних перевірку чи не існують
            //   дані які він намагатиметься вставити. 
            //   Якщо для одиничного INSERT це не проблема, то для великих таблиць потрібно додавати чи існує відповідна 
            //   к-сть рядків вставлених попередніми INSERT-ами.
            // А також потрібно додати аналогічну перевірку перед запуском збережених процедур.

            //_logger.LogInformation("Database exists and is accessible.");
            //var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync();

            //if (!appliedMigrations.Any()) // ii)	НЕМАЄ РАНІШЕ ПРОВЕДЕНИХ МІГРАЦІЙ (проект запускається вперше) - ПОТРІБНО створити базові таблиці:
            //{
            //    _logger.LogInformation("No migrations applied. Applying all migrations...");
            //    await ExecuteMigrationScenarioAsync(canConnect); // TRUE
            //}
            //else
            //{
            //    await ApplyMigrationsAsync(); // i)	Є РАНІШЕ ПРОВЕДЕНІ МІГРАЦІЇ - НЕПОТРІБНО створювати базові таблиці:
            //}

            await ApplyMigrationsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during database initialization.");
            throw; 
        }
    }

    private async Task ExecuteMigrationScenarioAsync(bool canConnect)
    {
        try
        {
            await ApplyMigrationsAsync();
            return;
        }
        //catch (Exception ex) when ((ex is SqlException sqlEx && sqlEx.Number == 4060) || // 4060: Cannot open database   
        //                            ex.Message.Contains("PendingModelChangesWarning"))
        catch (SqlException sqlEx) when (sqlEx.Number == 4060)
        {
            _logger.LogError(sqlEx, "Cannot connect to database. Attempting to create a new database.");

            //await DeleteDbAsync();
        }
        catch (Exception ex) when (ex.Message.Contains("PendingModelChangesWarning"))
        {
            _logger.LogWarning(ex, "Model changes detected, but no migration was added. Applying SQL scripts manually.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
            throw;
        }

        await ExecuteManuallyDbInitalization(canConnect);
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

            //await _context.Database.ExecuteSqlAsync($"EXEC {storedProcedure}");
            //await _context.Database.ExecuteSqlRawAsync("EXEC MigrateNGCICOStoNGCICO_W");
            //var result = 
                await _context.Database.ExecuteSqlRawAsync("EXEC {0}", storedProcedure); // Безпечно до ін'єкцій!
            
            //if (result = 1) - ПОГАНА ІДЕЯ ТОМУ, ЩО:
            // ДЛЯ ВІДЛОВУ ПОМИЛКИ В C#, ТОЧНІШЕ ВІДЛОВУ ПОВЕРНУТОГО ЗНАЧЕННЯ -1 І ТОДІ ПОВІДОМИТИ, ЩО ПРОЦЕДУРА ВІДПРАЦЮВАЛА З ПОМИЛКОЮ.
			// Я К Щ О   В С Т А Н О В Л Е Н О   'SET NOCOUNT ON' - З А В Ж Д И   П О В Е Р Т А Т И М Е Т Ь С Я   -1!!!
            //  АБО ЯКЩО НЕМАЄ КОМАНД INSERT, UPDATE або DELETE, І ЯКЩО ВОНИ НЕ ЗМІНОИЛИ ЖОДНОГО РЯДКА КОДУ!!!

            _logger.LogInformation($"Stored procedure {storedProcedure} executed successfully.");
        }
        catch (DbUpdateException dbEx) // Якщо помилка пов'язана з оновленням БД
        {
            _logger.LogError(dbEx, $"Database update error while executing stored procedure {storedProcedure}.");
            throw;
        }
        catch (SqlException sqlEx) // Якщо це помилка SQL (збережена процедура кинула THROW)
        {
            _logger.LogError(sqlEx, $"SQL error while executing stored procedure {storedProcedure}: {sqlEx.Message}");
            throw;
        }
        catch (Exception ex) // Інші непередбачені помилки
        {
            _logger.LogError(ex, $"Unexpected error while executing stored procedure {storedProcedure}.");
            throw;
        }
    }

    private async Task ExecuteSqlScriptAsync(string scriptName)
    {
        // _scriptsDirectory = "D:\DEVELOPMENT\ПРОЕКТИ ДЛЯ ОСОБИСТИХ ЦІЛЕЙ\Astronomic Project\Astronomic Catalogs\bin\Debug\net9.0\Database Scripts"
        // D:\DEVELOPMENT\ПРОЕКТИ ДЛЯ ОСОБИСТИХ ЦІЛЕЙ\Astronomic Project\Astronomic Catalogs\bin\Debug\net9.0\Database Scripts/$"{_scriptsDirectory}CreateDatabase.sql
        string scriptPath = $"{_scriptsDirectory}/{scriptName}"; // П Р И С Т О С У Й   Д О   В И К О Н А Н Н Я   В   Azure!!!

        if (!File.Exists(scriptPath))
        {
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
                    scriptPath == "Database Scripts/Data/NGC2000_UKTemporarily.sql"
                   ) // П Р И С Т О С У Й   Д О   В И К О Н А Н Н Я   В   Azure!!!
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

    private async Task DeleteDbAsync()
    {
        await using var connection = new SqlConnection(_connectionStringProvider.ConnectionString);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = @"
            ALTER DATABASE [AstroCatalogsDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
            DROP DATABASE [AstroCatalogsDB]";
        await command.ExecuteNonQueryAsync();
    }
    private async Task CreateDatabaseAsync()
    {
        string scriptPath = $"{_scriptsDirectory}/CreateDatabase.sql";

        try
        {
            var script = await File.ReadAllTextAsync(scriptPath);
            var serverConnection = _configuration["ServerConnection"];
            using (var connection = new SqlConnection(serverConnection))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(script, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating database: {ex.Message}");
        }
    }





    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
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
