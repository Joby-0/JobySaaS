namespace Models;

public enum PublishJobStatus
{
    Pending,
    Processing,
    Completed,
    CompletedWithErrors,
    Failed
}

public enum PublishJobAccountStatus
{
    Pending,
    Processing,
    Completed,
    Failed
}
