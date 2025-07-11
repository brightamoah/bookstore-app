using System.Text.Json.Serialization;

namespace Backend.Models;

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string ErrorCode { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? TraceId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? ValidationErrors { get; set; }
}

public class ValidationErrorResponse : ErrorResponse
{
    public Dictionary<string, string[]> Errors { get; set; } = [];
}

public static class ErrorCodes
{
    // Authentication & Authorization
    public const string UNAUTHORIZED = "AUTH_001";
    public const string INVALID_CREDENTIALS = "AUTH_002";
    public const string TOKEN_EXPIRED = "AUTH_003";
    public const string ACCESS_DENIED = "AUTH_004";
    public const string USER_ALREADY_EXISTS = "AUTH_005";
    public const string WEAK_PASSWORD = "AUTH_006";
    public const string USER_NOT_FOUND = "AUTH_007";

    // Book Operations
    public const string BOOK_NOT_FOUND = "BOOK_001";
    public const string BOOK_ALREADY_EXISTS = "BOOK_002";
    public const string INVALID_BOOK_DATA = "BOOK_003";
    public const string BOOK_CREATION_FAILED = "BOOK_004";
    public const string BOOK_UPDATE_FAILED = "BOOK_005";
    public const string BOOK_DELETE_FAILED = "BOOK_006";

    // Image Operations
    public const string IMAGE_UPLOAD_FAILED = "IMG_001";
    public const string INVALID_IMAGE_FORMAT = "IMG_002";
    public const string IMAGE_TOO_LARGE = "IMG_003";
    public const string IMAGE_PROCESSING_FAILED = "IMG_004";

    // Rate Limiting
    public const string RATE_LIMIT_EXCEEDED = "RATE_001";

    // Validation
    public const string VALIDATION_FAILED = "VAL_001";
    public const string REQUIRED_FIELD_MISSING = "VAL_002";
    public const string INVALID_FORMAT = "VAL_003";

    // Server Errors
    public const string INTERNAL_SERVER_ERROR = "SRV_001";
    public const string DATABASE_ERROR = "SRV_002";
    public const string EXTERNAL_SERVICE_ERROR = "SRV_003";
}
