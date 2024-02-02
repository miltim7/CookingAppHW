
using System.Data.SqlClient;
using Dapper;

public class LogService : ILogService
{
    private readonly SqlConnection connection;
    public LogService(SqlConnection connection)
    {
        this.connection = connection;
    }
    public async Task Log(Log log)
    {
        string query = @"insert into Logs (UserId, Url, MethodType, StatusCode, RequestBody, ResponseBody) values (@UserId, @Url, @MethodType, @StatusCode, @RequestBody, @ResponseBody);";

        await connection.ExecuteAsync(query, log);
    }
}