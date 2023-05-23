using System;

namespace PdfGenerator.Infrastructure;

public class TaskHubPdfXpertTracking
{
    public int Id { get; set; }
    public string OrchestrationId { get; set; }
    public int AssociatedId { get; set; }
    public ContractNoteScenario ContractNoteScenario { get; set; }
    public string EventData { get; set; }
    public string FileReference { get; set; }
    public PdfXpertEventStatus EventStatus { get; set; }
    public int Retried { get; set; }
    public string CreatedBy { get; set; }
    public string CreatedByName { get; set; }
    public DateTime CreatedOn { get; set; }
    public string UpdatedBy { get; set; }
    public string UpdatedByName { get; set; }
    public DateTime UpdatedOn { get; set; }
}