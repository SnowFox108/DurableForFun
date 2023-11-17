using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace PdfGenerator.Infrastructure;

public interface IConnectionFactory
{
    string ConnectionString { get; }
    SqlConnection OpenOptionConnection(ILogger log);

}