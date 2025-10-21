namespace TestcaseGenerator.Models
{
    public class TestCaseRequest
    {
        public string FunctionCode { get; set; } = "";
        public string FunctionName { get; set; } = "";
        public string CreatedBy { get; set; } = "";
        public string? ExecutedBy { get; set; }
        public string? LinesOfCode { get; set; }
        public string TestRequirement { get; set; } = "";
        public string Status { get; set; } = "Passed";
        public List<InputField> Fields { get; set; } = new();
        public List<string> ReturnConditions { get; set; } = new();
        public List<string> LogMessages { get; set; } = new();
    }
}
