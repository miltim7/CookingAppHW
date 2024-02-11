using System.Data.SqlClient;
using Dapper;

public class LogRepository : ILogRepository {
    private readonly SqlConnection connection;

    public LogRepository(SqlConnection connection)
    {
        this.connection = connection;
    }

    public async Task InsertAsync(Log log)
    {
        string query = @"insert into Logs (UserId, Url, MethodType, StatusCode, RequestBody, ResponseBody) values (@UserId, @Url, @MethodType, @StatusCode, @RequestBody, @ResponseBody);";

        await connection.ExecuteAsync(query, log);
    }
}