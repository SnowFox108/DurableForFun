using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PdfGenerator.Infrastructure;

public interface ITaskHubEventTrackingQuery
{
    Task<IEnumerable<TaskHubPdfXpertTracking>> GetTaskByOrchestrationId(string orchestrationId);
}

public class TaskHubEventTrackingQuery
{
    private readonly IConnectionFactory _connectionFactory;

    public TaskHubEventTrackingQuery(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<TaskHubPdfXpertTracking>> GetTaskByOrchestrationId(ILogger log, string orchestrationId)
    {

        const string sql = @"SELECT * FROM [TaskHub].[PdfXpertTrackings]
                    WHERE OrchestrationId = @OrchestrationId AND EventStatus IN @EventStatus";

        await using var connection = _connectionFactory.OpenOptionConnection(log);
        return await connection.QueryAsync<TaskHubPdfXpertTracking>(sql, new
        {
            OrchestrationId = orchestrationId,
            EventStatus = new[] { PdfXpertEventStatus.New, PdfXpertEventStatus.Processing },
        });
    }
}