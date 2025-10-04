namespace UknfCommunicationPlatform.Core.Enums;

/// <summary>
/// Status of an administrative case
/// </summary>
public enum CaseStatus
{
    /// <summary>
    /// New Case - Just created
    /// </summary>
    New = 0,

    /// <summary>
    /// In Progress - Being handled
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Awaiting UKNF Response
    /// </summary>
    AwaitingUknfResponse = 2,

    /// <summary>
    /// Awaiting User Response
    /// </summary>
    AwaitingUserResponse = 3,

    /// <summary>
    /// Resolved - Case completed
    /// </summary>
    Resolved = 4,

    /// <summary>
    /// Closed - Case archived
    /// </summary>
    Closed = 5,

    /// <summary>
    /// Cancelled - Case was cancelled
    /// </summary>
    Cancelled = 6
}
