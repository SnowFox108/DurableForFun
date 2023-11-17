using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;

namespace PdfGenerator.Infrastructure;

public class ConnectionFactory : IConnectionFactory
{
    public string ConnectionString { get; private set; }
    public SqlConnection OpenOptionConnection(ILogger log)
    {
        ConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings_Options");

        log.LogInformation($"Connecting to Database: {ConnectionString}");

        var connection = new SqlConnection(ConnectionString);
        connection.Open();
        return connection;
    }
}