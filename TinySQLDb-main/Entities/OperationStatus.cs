namespace Entities
{
    public enum OperationStatus
    {
        Warning,
        Success,
        DatabaseAlreadyExists,
        DatabaseNotFound,
        TableAlreadyExists,
        Error,
        TableNotFound,
        InvalidColumn,
        TableNotEmpty,
        IndexAlreadyExists,
        InvalidIndexType
    }
}
