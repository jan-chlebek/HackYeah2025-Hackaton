namespace UknfCommunicationPlatform.Core.Enums;

/// <summary>
/// Status of a report validation
/// </summary>
public enum ReportStatus
{
    /// <summary>
    /// Draft - Set after adding file with report
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Submitted - Set after starting validation process, confirmed with unique ID
    /// </summary>
    Submitted = 1,

    /// <summary>
    /// In Progress - Processing is in progress
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// Validation Successful - Processing completed successfully, no errors found
    /// </summary>
    ValidationSuccessful = 3,

    /// <summary>
    /// Validation Errors - Errors found in validation rules
    /// </summary>
    ValidationErrors = 4,

    /// <summary>
    /// Technical Error - Processing ended with technical error
    /// </summary>
    TechnicalError = 5,

    /// <summary>
    /// Timeout - Set automatically if processing not completed within 24h
    /// </summary>
    Timeout = 6,

    /// <summary>
    /// Questioned by UKNF - Set manually by UKNF employee
    /// </summary>
    QuestionedByUKNF = 7
}
