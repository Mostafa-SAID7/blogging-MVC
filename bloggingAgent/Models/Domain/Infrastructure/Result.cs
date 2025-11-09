using System;
using System.Collections.Generic;

namespace BloggingAgent.Models.Domain.Infrastructure
{
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public T Value { get; private set; }
        public ErrorResponse Error { get; private set; }
        public IEnumerable<string> ValidationErrors { get; private set; }

        private Result(T value)
        {
            IsSuccess = true;
            Value = value;
        }

        private Result(ErrorResponse error)
        {
            IsSuccess = false;
            Error = error;
        }

        private Result(IEnumerable<string> validationErrors)
        {
            IsSuccess = false;
            ValidationErrors = validationErrors;
            Error = new ErrorResponse(validationErrors, "VALIDATION_ERROR");
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(value);
        }

        public static Result<T> Failure(string message, string code = null, int statusCode = 500)
        {
            return new Result<T>(new ErrorResponse(message, code, statusCode));
        }

        public static Result<T> Failure(ErrorResponse error)
        {
            return new Result<T>(error);
        }

        public static Result<T> Failure(Exception exception, string code = null, int statusCode = 500)
        {
            return new Result<T>(new ErrorResponse(exception, code, statusCode));
        }

        public static Result<T> Failure(DomainException domainException)
        {
            return new Result<T>(new ErrorResponse(domainException));
        }

        public static Result<T> ValidationFailure(IEnumerable<string> errors)
        {
            return new Result<T>(errors);
        }

        public Result<TOut> Map<TOut>(Func<T, TOut> mapper)
        {
            return IsSuccess
                ? Result<TOut>.Success(mapper(Value))
                : Result<TOut>.Failure(Error);
        }

        public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> binder)
        {
            return IsSuccess
                ? binder(Value)
                : Result<TOut>.Failure(Error);
        }

        public T GetValueOrDefault(T defaultValue = default)
        {
            return IsSuccess ? Value : defaultValue;
        }

        public T GetValueOrThrow()
        {
            if (!IsSuccess)
                throw new InvalidOperationException("Cannot get value from failed result", Error?.Error?.Details != null ? new Exception(Error.Error.Details) : null);

            return Value;
        }

        public void Match(Action<T> onSuccess, Action<ErrorResponse> onFailure)
        {
            if (IsSuccess)
                onSuccess(Value);
            else
                onFailure(Error);
        }

        public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<ErrorResponse, TResult> onFailure)
        {
            return IsSuccess
                ? onSuccess(Value)
                : onFailure(Error);
        }
    }

    // Non-generic Result for operations that don't return a value
    public class Result
    {
        public bool IsSuccess { get; private set; }
        public ErrorResponse Error { get; private set; }
        public IEnumerable<string> ValidationErrors { get; private set; }

        private Result()
        {
            IsSuccess = true;
        }

        private Result(ErrorResponse error)
        {
            IsSuccess = false;
            Error = error;
        }

        private Result(IEnumerable<string> validationErrors)
        {
            IsSuccess = false;
            ValidationErrors = validationErrors;
            Error = new ErrorResponse(validationErrors, "VALIDATION_ERROR");
        }

        public static Result Success()
        {
            return new Result();
        }

        public static Result Failure(string message, string code = null, int statusCode = 500)
        {
            return new Result(new ErrorResponse(message, code, statusCode));
        }

        public static Result Failure(ErrorResponse error)
        {
            return new Result(error);
        }

        public static Result Failure(Exception exception, string code = null, int statusCode = 500)
        {
            return new Result(new ErrorResponse(exception, code, statusCode));
        }

        public static Result Failure(DomainException domainException)
        {
            return new Result(new ErrorResponse(domainException));
        }

        public static Result ValidationFailure(IEnumerable<string> errors)
        {
            return new Result(errors);
        }

        public void Match(Action onSuccess, Action<ErrorResponse> onFailure)
        {
            if (IsSuccess)
                onSuccess();
            else
                onFailure(Error);
        }

        public TResult Match<TResult>(Func<TResult> onSuccess, Func<ErrorResponse, TResult> onFailure)
        {
            return IsSuccess
                ? onSuccess()
                : onFailure(Error);
        }
    }

    // Result extensions for common operations
    public static class ResultExtensions
    {
        public static Result<T> ToResult<T>(this T value)
        {
            return Result<T>.Success(value);
        }

        public static Result ToResult(this System.Threading.Tasks.Task task)
        {
            try
            {
                task.GetAwaiter().GetResult();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex);
            }
        }

        public static async System.Threading.Tasks.Task<Result<T>> ToResultAsync<T>(this System.Threading.Tasks.Task<T> task)
        {
            try
            {
                var result = await task;
                return Result<T>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<T>.Failure(ex);
            }
        }

        public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, string errorMessage)
        {
            if (!result.IsSuccess)
                return result;

            return predicate(result.Value)
                ? result
                : Result<T>.Failure(errorMessage, "VALIDATION_ERROR", 400);
        }

        public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, ErrorResponse error)
        {
            if (!result.IsSuccess)
                return result;

            return predicate(result.Value)
                ? result
                : Result<T>.Failure(error);
        }
    }
}