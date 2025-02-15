using System.Data;
using System.Data.SqlClient;
using NLog;

public class DatabaseService
{
    //private static readonly NLog.ILogger Logger = LogManager.GetCurrentClassLogger();
    //private readonly string _connectionString;

    //public DatabaseService(string connectionString)
    //{
    //    _connectionString = connectionString;
    //}

    //public void ExecuteStoredProcedure(string procedureName, SqlParameter[] parameters)
    //{
    //    Logger.Info($"Calling stored procedure: {procedureName}");

    //    using var connection = new SqlConnection(_connectionString);
    //    using var command = new SqlCommand(procedureName, connection)
    //    {
    //        CommandType = CommandType.StoredProcedure
    //    };

    //    if (parameters != null)
    //    {
    //        command.Parameters.AddRange(parameters);
    //    }

    //    try
    //    {
    //        connection.Open();
    //        command.ExecuteNonQuery();
    //        Logger.Info($"Stored procedure {procedureName} executed successfully");
    //    }
    //    catch (Exception ex)
    //    {
    //        Logger.Error(ex, $"Error executing stored procedure {procedureName}");
    //        throw;
    //    }
    //}
}