namespace BloggingAgent.Models.ViewModels
{
    public class ErrorViewModel
    {
        public int? StatusCode { get; set; }
        public string Title { get; set; } = "Error";
        public string Message { get; set; } = "An unexpected error occurred while processing your request.";
        public string DetailedMessage { get; set; }
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
