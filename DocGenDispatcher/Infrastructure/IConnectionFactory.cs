using System.Data.SqlClient;

namespace PdfGenerator.Infrastructure;

public interface IConnectionFactory
{
    SqlConnection OpenOptionConnection();

}