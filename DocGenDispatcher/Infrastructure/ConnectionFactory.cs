using System;
using System.Data.SqlClient;

namespace PdfGenerator.Infrastructure;

public class ConnectionFactory : IConnectionFactory
{
    public SqlConnection OpenOptionConnection()
    {
        var connection = new SqlConnection(Environment.GetEnvironmentVariable("ConnectionStrings:Options"));
        connection.Open();
        return connection;
    }
}