namespace PdfGenerator.Infrastructure;

public enum PdfXpertEventStatus
{
    New = 1,
    Processing = 2,
    ContentSaved = 3,
    Completed = 4,
    Error = 5,
    Cancelled = 6,
}