namespace UknfCommunicationPlatform.Core.Enums;

/// <summary>
/// Status of FAQ question
/// </summary>
public enum FaqQuestionStatus
{
    /// <summary>
    /// Submitted - Waiting for answer
    /// </summary>
    Submitted = 0,

    /// <summary>
    /// In Progress - Being answered
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Answered - Has answer but not published
    /// </summary>
    Answered = 2,

    /// <summary>
    /// Published - Visible to all users
    /// </summary>
    Published = 3,

    /// <summary>
    /// Rejected - Will not be answered
    /// </summary>
    Rejected = 4
}
