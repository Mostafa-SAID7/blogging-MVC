using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace BloggingAgent.Models.Domain.Responses
{
    public class ErrorResponse
    {
        [JsonPropertyName("success")]
        public bool Success => false;

        [JsonPropertyName("error")]
        public ErrorDetails Error { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("requestId")]
        public string RequestId { get; set; }

        public ErrorResponse(string message, string code = null, int statusCode = 500)
        {
            Error = new ErrorDetails
            {
                Message = message,
                Code = code ?? "INTERNAL_ERROR",
                StatusCode = statusCode
            };
            Timestamp = DateTime.UtcNow;
            RequestId = Guid.NewGuid().ToString();
        }

        public ErrorResponse(Exception exception, string code = null, int statusCode = 500)
        {
            Error = new ErrorDetails
            {
                Message = exception.Message,
                Code = code ?? "INTERNAL_ERROR",
                StatusCode = statusCode,
                Details = exception.InnerException?.Message
            };
            Timestamp = DateTime.UtcNow;
            RequestId = Guid.NewGuid().ToString();
        }

        public ErrorResponse(DomainException domainException)
        {
            Error = new ErrorDetails
            {
                Message = domainException.Message,
                Code = domainException.ErrorCode,
                StatusCode = GetStatusCodeFromDomainException(domainException),
                Parameters = domainException.Parameters
            };
            Timestamp = DateTime.UtcNow;
            RequestId = Guid.NewGuid().ToString();
        }

        public ErrorResponse(IEnumerable<string> validationErrors, string code = "VALIDATION_ERROR")
        {
            Error = new ErrorDetails
            {
                Message = "Validation failed",
                Code = code,
                StatusCode = 400,
                ValidationErrors = validationErrors.ToList()
            };
            Timestamp = DateTime.UtcNow;
            RequestId = Guid.NewGuid().ToString();
        }

        private static int GetStatusCodeFromDomainException(DomainException exception)
        {
            return exception switch
            {
                BlogPostDomainException => 400,
                CommentDomainException => 400,
                UserDomainException => 400,
                ValidationDomainException => 400,
                _ => 500
            };
        }
    }

    public class ErrorDetails
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("details")]
        public string Details { get; set; }

        [JsonPropertyName("validationErrors")]
        public List<string> ValidationErrors { get; set; }

        [JsonPropertyName("parameters")]
        public object[] Parameters { get; set; }

        [JsonPropertyName("help")]
        public string Help { get; set; }

        [JsonPropertyName("traceId")]
        public string TraceId { get; set; }
    }

    // Specific error response types
    public class ValidationErrorResponse : ErrorResponse
    {
        public ValidationErrorResponse(IEnumerable<string> errors)
            : base(errors, "VALIDATION_ERROR")
        {
        }
    }

    public class NotFoundErrorResponse : ErrorResponse
    {
        public NotFoundErrorResponse(string resourceType, object resourceId)
            : base($"{resourceType} with ID '{resourceId}' was not found", "NOT_FOUND", 404)
        {
        }
    }

    public class UnauthorizedErrorResponse : ErrorResponse
    {
        public UnauthorizedErrorResponse(string message = "Authentication required")
            : base(message, "UNAUTHORIZED", 401)
        {
        }
    }

    public class ForbiddenErrorResponse : ErrorResponse
    {
        public ForbiddenErrorResponse(string message = "Access denied")
            : base(message, "FORBIDDEN", 403)
        {
        }
    }

    public class ConflictErrorResponse : ErrorResponse
    {
        public ConflictErrorResponse(string message, string code = "CONFLICT")
            : base(message, code, 409)
        {
        }
    }

    public class RateLimitErrorResponse : ErrorResponse
    {
        public RateLimitErrorResponse(string message = "Rate limit exceeded", int retryAfterSeconds = 60)
            : base(message, "RATE_LIMIT_EXCEEDED", 429)
        {
            Error.Help = $"Retry after {retryAfterSeconds} seconds";
        }
    }

    // Error response factory
    public static class ErrorResponseFactory
    {
        public static ErrorResponse CreateFromException(Exception exception)
        {
            return exception switch
            {
                DomainException domainEx => new ErrorResponse(domainEx),
                ArgumentException argEx => new ErrorResponse(argEx.Message, "INVALID_ARGUMENT", 400),
                InvalidOperationException invalidOpEx => new ErrorResponse(invalidOpEx.Message, "INVALID_OPERATION", 400),
                KeyNotFoundException keyNotFoundEx => new ErrorResponse(keyNotFoundEx.Message, "NOT_FOUND", 404),
                UnauthorizedAccessException unauthorizedEx => new ErrorResponse(unauthorizedEx.Message, "UNAUTHORIZED", 401),
                TimeoutException timeoutEx => new ErrorResponse("Request timed out", "TIMEOUT", 408),
                _ => new ErrorResponse(exception, "INTERNAL_ERROR", 500)
            };
        }

        public static ErrorResponse CreateValidationError(IEnumerable<string> errors)
        {
            return new ValidationErrorResponse(errors);
        }

        public static ErrorResponse CreateNotFound(string resourceType, object id)
        {
            return new NotFoundErrorResponse(resourceType, id);
        }

        public static ErrorResponse CreateUnauthorized(string message = null)
        {
            return new UnauthorizedErrorResponse(message);
        }

        public static ErrorResponse CreateForbidden(string message = null)
        {
            return new ForbiddenErrorResponse(message);
        }

        public static ErrorResponse CreateConflict(string message)
        {
            return new ConflictErrorResponse(message);
        }

        public static ErrorResponse CreateRateLimitExceeded(int retryAfterSeconds = 60)
        {
            return new RateLimitErrorResponse(retryAfterSeconds: retryAfterSeconds);
        }
    }
}