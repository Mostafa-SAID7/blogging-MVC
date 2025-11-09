using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BloggingAgent.Models.Domain.Infrastructure
{
    public class ValidationResult
    {
        public bool IsValid { get; private set; }
        public List<ValidationError> Errors { get; private set; }

        private ValidationResult()
        {
            IsValid = true;
            Errors = new List<ValidationError>();
        }

        private ValidationResult(List<ValidationError> errors)
        {
            IsValid = false;
            Errors = errors ?? new List<ValidationError>();
        }

        public static ValidationResult Success()
        {
            return new ValidationResult();
        }

        public static ValidationResult Failure(params ValidationError[] errors)
        {
            return new ValidationResult(errors.ToList());
        }

        public static ValidationResult Failure(IEnumerable<ValidationError> errors)
        {
            return new ValidationResult(errors.ToList());
        }

        public static ValidationResult Failure(string field, string message, string code = null)
        {
            return new ValidationResult(new List<ValidationError>
            {
                new ValidationError(field, message, code)
            });
        }

        public ValidationResult AddError(string field, string message, string code = null)
        {
            Errors.Add(new ValidationError(field, message, code));
            IsValid = false;
            return this;
        }

        public ValidationResult AddErrors(IEnumerable<ValidationError> errors)
        {
            Errors.AddRange(errors);
            IsValid = false;
            return this;
        }

        public IEnumerable<string> GetAllMessages()
        {
            return Errors.Select(e => e.Message);
        }

        public Dictionary<string, string[]> GetFieldErrors()
        {
            return Errors
                .GroupBy(e => e.Field)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Message).ToArray());
        }

        public string GetSummaryMessage()
        {
            if (IsValid)
                return "Validation successful";

            if (Errors.Count == 1)
                return Errors[0].Message;

            return $"{Errors.Count} validation errors occurred";
        }
    }

    public class ValidationError
    {
        public string Field { get; }
        public string Message { get; }
        public string Code { get; }
        public object AttemptedValue { get; }

        public ValidationError(string field, string message, string code = null, object attemptedValue = null)
        {
            Field = field;
            Message = message;
            Code = code;
            AttemptedValue = attemptedValue;
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Field)
                ? Message
                : $"{Field}: {Message}";
        }
    }

    // Validation helper class
    public static class ValidationHelper
    {
        public static ValidationResult ValidateModel<T>(T model)
        {
            var validationContext = new ValidationContext(model);
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            if (isValid)
                return ValidationResult.Success();

            var errors = validationResults.Select(r =>
                new ValidationError(
                    r.MemberNames.FirstOrDefault() ?? "",
                    r.ErrorMessage,
                    "MODEL_VALIDATION",
                    r.MemberNames.Any() ? GetPropertyValue(model, r.MemberNames.First()) : null
                ));

            return ValidationResult.Failure(errors);
        }

        public static ValidationResult ValidateBlogPost(BlogPost blogPost)
        {
            var result = ValidationResult.Success();

            if (string.IsNullOrWhiteSpace(blogPost.Title))
                result.AddError("Title", "Title is required");

            if (blogPost.Title?.Length > 200)
                result.AddError("Title", "Title cannot exceed 200 characters");

            if (string.IsNullOrWhiteSpace(blogPost.Content))
                result.AddError("Content", "Content is required");

            if (string.IsNullOrWhiteSpace(blogPost.Author))
                result.AddError("Author", "Author is required");

            if (blogPost.Excerpt?.Length > 500)
                result.AddError("Excerpt", "Excerpt cannot exceed 500 characters");

            if (blogPost.Tags?.Any(tag => tag.Length > 50) == true)
                result.AddError("Tags", "Individual tags cannot exceed 50 characters");

            return result;
        }

        public static ValidationResult ValidateComment(Comment comment)
        {
            var result = ValidationResult.Success();

            if (string.IsNullOrWhiteSpace(comment.Content))
                result.AddError("Content", "Comment content is required");

            if (comment.Content?.Length > 1000)
                result.AddError("Content", "Comment cannot exceed 1000 characters");

            if (string.IsNullOrWhiteSpace(comment.AuthorName))
                result.AddError("AuthorName", "Author name is required");

            if (comment.AuthorName?.Length > 100)
                result.AddError("AuthorName", "Author name cannot exceed 100 characters");

            if (!string.IsNullOrWhiteSpace(comment.AuthorEmail) &&
                !IsValidEmail(comment.AuthorEmail))
                result.AddError("AuthorEmail", "Invalid email format");

            return result;
        }

        public static ValidationResult ValidateUser(ApplicationUser user)
        {
            var result = ValidationResult.Success();

            if (string.IsNullOrWhiteSpace(user.UserName))
                result.AddError("UserName", "Username is required");

            if (string.IsNullOrWhiteSpace(user.Email))
                result.AddError("Email", "Email is required");

            if (!string.IsNullOrWhiteSpace(user.Email) && !IsValidEmail(user.Email))
                result.AddError("Email", "Invalid email format");

            if (user.FirstName?.Length > 50)
                result.AddError("FirstName", "First name cannot exceed 50 characters");

            if (user.LastName?.Length > 50)
                result.AddError("LastName", "Last name cannot exceed 50 characters");

            if (user.Bio?.Length > 500)
                result.AddError("Bio", "Bio cannot exceed 500 characters");

            return result;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private static object GetPropertyValue(object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName);
            return property?.GetValue(obj);
        }
    }

    // Fluent validation extensions
    public static class ValidationExtensions
    {
        public static ValidationResult Validate(this BlogPost blogPost)
        {
            return ValidationHelper.ValidateBlogPost(blogPost);
        }

        public static ValidationResult Validate(this Comment comment)
        {
            return ValidationHelper.ValidateComment(comment);
        }

        public static ValidationResult Validate(this ApplicationUser user)
        {
            return ValidationHelper.ValidateUser(user);
        }

        public static ValidationResult Combine(this ValidationResult first, ValidationResult second)
        {
            if (first.IsValid && second.IsValid)
                return ValidationResult.Success();

            var allErrors = new List<ValidationError>();
            allErrors.AddRange(first.Errors);
            allErrors.AddRange(second.Errors);

            return ValidationResult.Failure(allErrors);
        }

        public static Result<T> ToResult<T>(this ValidationResult validation, T value)
        {
            return validation.IsValid
                ? Result<T>.Success(value)
                : Result<T>.ValidationFailure(validation.GetAllMessages());
        }

        public static Result ToResult(this ValidationResult validation)
        {
            return validation.IsValid
                ? Result.Success()
                : Result.ValidationFailure(validation.GetAllMessages());
        }
    }
}