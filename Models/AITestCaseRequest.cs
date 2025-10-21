using System.Text.Json.Serialization;

namespace TestcaseGenerator.Models
{
    public class AITestCaseRequest
    {
        [JsonPropertyName("userRequirement")]
        public string Request { get; set; } = string.Empty;
    }

    public class AITestCaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public TestCaseRequest? TestCaseRequest { get; set; }
        public byte[]? ExcelFile { get; set; }
        public string? FileName { get; set; }
    }
}
