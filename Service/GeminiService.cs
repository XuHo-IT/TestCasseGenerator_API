using System.Text;
using System.Text.Json;
using TestcaseGenerator.Models;

namespace TestcaseGenerator.Service
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        // before modification - pass 16, fail 16
        //public GeminiService(HttpClient httpClient, IConfiguration configuration)
        //{
        //    _httpClient = httpClient;
        //    _configuration = configuration;
        //    _apiKey = _configuration["GeminiApi:ApiKey"] ?? throw new InvalidOperationException("Gemini API key not configured");
        //    _baseUrl = _configuration["GeminiApi:BaseUrl"] ?? "https://generativelanguage.googleapis.com/v1/models/gemini-1.5-pro:generateContent";
        //}

        // after modification - pass 19, fail 13
        public GeminiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            var apiKey = _configuration["GeminiApi:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("Gemini API key not configured");
            }
            _apiKey = apiKey;

            _baseUrl = _configuration["GeminiApi:BaseUrl"] ?? "...";
        }


        public async Task<GeminiTestCaseResponse> GenerateTestCasesAsync(string userRequirement)
        {
            try
            {
                var prompt = CreatePrompt(userRequirement);
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{_baseUrl}?key={_apiKey}";
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var geminiResponse = JsonSerializer.Deserialize<GeminiApiResponse>(responseContent);
                    
                    if (geminiResponse?.candidates?.Length > 0)
                    {
                        var generatedText = geminiResponse.candidates[0].content.parts[0].text;
                        return ParseGeneratedResponse(generatedText);
                    }
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Gemini API request failed: {response.StatusCode} - {errorContent}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error calling Gemini API: {ex.Message}", ex);
            }
        }

        public async Task<GeminiTestCaseResponse> GenerateUseCaseTableAsync(string useCaseName, string? additionalContext = null)
        {
            try
            {
                var prompt = CreateUseCaseTablePrompt(useCaseName, additionalContext);
                
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{_baseUrl}?key={_apiKey}";
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var geminiResponse = JsonSerializer.Deserialize<GeminiApiResponse>(responseContent);
                    
                    if (geminiResponse?.candidates?.Length > 0)
                    {
                        var generatedText = geminiResponse.candidates[0].content.parts[0].text;
                        return ParseGeneratedResponse(generatedText);
                    }
                    else
                    {
                        return new GeminiTestCaseResponse
                        {
                            Success = false,
                            TestCaseRequest = null,
                            Message = "No candidates found in Gemini response"
                        };
                    }
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Gemini API request failed: {response.StatusCode} - {errorContent}");
            }
            catch (Exception ex)
            {
                return new GeminiTestCaseResponse
                {
                    Success = false,
                    TestCaseRequest = null,
                    Message = $"Error generating use case table: {ex.Message}"
                };
            }
        }

        private string CreatePrompt(string userRequirement)
        {
            return $@"
You are a software testing expert. Based on the user requirement: ""{userRequirement}""

Generate a comprehensive test case specification in JSON format with the following structure:

{{
  ""functionCode"": ""[Generate a meaningful function code]"",
  ""functionName"": ""[Generate a descriptive function name]"",
  ""createdBy"": ""[Generate a developer name]"",
  ""testRequirement"": ""{userRequirement}"",
  ""fields"": [
    {{
      ""name"": ""[field name]"",
      ""type"": ""int"",
      ""min"": [minimum value],
      ""max"": [maximum value]
    }}
  ],
  ""returnConditions"": [
    ""[condition 1]"",
    ""[condition 2]"",
    ""[condition 3]""
  ],
  ""logMessages"": [
    ""[success message]"",
    ""[error message]"",
    ""[validation message]""
  ]
}}

Requirements:
1. Analyze the user requirement and identify the key input fields that need testing
2. For each field, determine appropriate min/max values based on the context
3. Generate realistic and meaningful field names
4. Create a descriptive function name and code
5. Use appropriate data types (int, double, string, etc.)
6. Consider boundary value analysis principles
7. Generate relevant return conditions based on the requirement (not generic Delta conditions)
8. Generate appropriate log messages for success/error scenarios
9. Make the test cases comprehensive and realistic

Examples:
- For age validation (10-18): return conditions like ""Valid age"", ""Age too young"", ""Age too old""
- For age validation: log messages like ""Age is valid"", ""Age must be at least 10"", ""Age must not exceed 18""
- For mathematical functions: return conditions like ""Valid result"", ""Invalid input"", ""Calculation error""
- For mathematical functions: log messages like ""Calculation successful"", ""Invalid input parameters"", ""Division by zero""
- If testing salary, use min: 1000, max: 100000
- If testing percentage, use min: 0, max: 100

Return ONLY the JSON object, no additional text or explanations.";
        }

        public async Task<UseCaseReport> GenerateUseCaseReportAsync(string useCaseName, string? additionalContext = null)
        {
            try
            {
                var prompt = CreateUseCaseReportPrompt(useCaseName, additionalContext);
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{_baseUrl}?key={_apiKey}";
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var geminiResponse = JsonSerializer.Deserialize<GeminiApiResponse>(responseContent);
                    
                    if (geminiResponse?.candidates?.Length > 0)
                    {
                        var generatedText = geminiResponse.candidates[0].content.parts[0].text;
                        return ParseUseCaseReportResponse(generatedText);
                    }
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Gemini API request failed: {response.StatusCode} - {errorContent}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating use case report: {ex.Message}", ex);
            }
        }

        private string CreateUseCaseTablePrompt(string useCaseName, string? additionalContext)
        {
            var contextPart = string.IsNullOrEmpty(additionalContext) ? "" : $"\n\nAdditional Context: {additionalContext}";
            
            return $@"
You are a software testing expert. Based on the use case: ""{useCaseName}""{contextPart}

Generate a comprehensive test case specification in JSON format that will create a complete test case table similar to the image format. The structure should be:

{{
  ""functionCode"": ""[Generate a meaningful function code based on use case]"",
  ""functionName"": ""[Generate a descriptive function name based on use case]"",
  ""createdBy"": ""[Generate a developer name]"",
  ""testRequirement"": ""[Detailed description of what this use case tests]"",
  ""fields"": [
    {{
      ""name"": ""[field name]"",
      ""type"": ""int"",
      ""min"": [minimum value],
      ""max"": [maximum value]
    }}
  ],
  ""returnConditions"": [
    ""[condition 1]"",
    ""[condition 2]"",
    ""[condition 3]""
  ],
  ""logMessages"": [
    ""[success message]"",
    ""[error message]"",
    ""[validation message]""
  ]
}}

Requirements:
1. Analyze the use case name and generate appropriate input fields with realistic min/max values
2. Create comprehensive test scenarios that cover boundary value analysis
3. Generate meaningful return conditions that match the use case context
4. Create relevant log messages for different scenarios
5. Ensure the test cases are comprehensive and cover normal, boundary, and abnormal cases
6. Make the function name and code descriptive and related to the use case
7. Generate realistic field names and data types based on the use case context

Examples of good use case analysis:
- ""User Login"": fields like username, password, loginAttempts; return conditions like ""Login successful"", ""Invalid credentials"", ""Account locked""
- ""Age Validation"": field like age with min/max; return conditions like ""Valid age"", ""Age too young"", ""Age too old""
- ""Payment Processing"": fields like amount, cardNumber, expiryDate; return conditions like ""Payment successful"", ""Invalid card"", ""Insufficient funds""

Return ONLY the JSON object, no additional text or explanations.";
        }

        private string CreateUseCaseReportPrompt(string useCaseName, string? additionalContext)
        {
            var contextPart = string.IsNullOrEmpty(additionalContext) ? "" : $"\n\nAdditional Context: {additionalContext}";
            
            return $@"
You are a software requirements expert. Based on the use case: ""{useCaseName}""{contextPart}

Generate a comprehensive Use Case Report in JSON format that matches the Requirement Specifications format. The structure should be:

{{
  ""ucId"": ""UC-1"",
  ""ucName"": ""[Use case name based on input]"",
  ""createdBy"": ""[Generate a developer name]"",
  ""dateCreated"": ""[Current date in DD/MM/YYYY format]"",
  ""primaryActor"": ""[Main actor for this use case]"",
  ""secondaryActors"": ""[Secondary actors or None]"",
  ""trigger"": ""[What triggers this use case]"",
  ""description"": ""[Brief description of the use case]"",
  ""preconditions"": [
    ""[Precondition 1]"",
    ""[Precondition 2]"",
    ""[Precondition 3]""
  ],
  ""postconditions"": [
    ""[Postcondition 1]"",
    ""[Postcondition 2]""
  ],
  ""normalFlow"": [
    {{
      ""step"": ""1"",
      ""description"": ""[Step 1 description]""
    }},
    {{
      ""step"": ""2"",
      ""description"": ""[Step 2 description]""
    }},
    {{
      ""step"": ""3"",
      ""description"": ""[Step 3 description]""
    }}
  ],
  ""alternativeFlows"": [
    {{
      ""flowId"": ""1.1"",
      ""flowName"": ""[Alternative flow name]"",
      ""steps"": [
        {{
          ""step"": ""1"",
          ""description"": ""[Alternative step 1]""
        }},
        {{
          ""step"": ""2"",
          ""description"": ""[Alternative step 2]""
        }}
      ]
    }}
  ],
  ""exceptions"": [
    {{
      ""exceptionId"": ""1.E1"",
      ""exceptionName"": ""[Exception name]"",
      ""descriptions"": [
        ""[Exception description 1]"",
        ""[Exception description 2]""
      ]
    }}
  ],
  ""priority"": ""[High/Medium/Low]"",
  ""frequencyOfUse"": ""[High/Medium/Low]"",
  ""businessRules"": ""[Business rule references]"",
  ""otherInformation"": [
    ""[Additional information 1]"",
    ""[Additional information 2]""
  ],
  ""assumptions"": [
    ""[Assumption 1]"",
    ""[Assumption 2]""
  ]
}}

Requirements:
1. Analyze the use case name and create a comprehensive use case specification
2. Generate realistic actors, triggers, and flows based on the use case context
3. Create detailed normal flow with 3-6 steps
4. Include 1-2 alternative flows if applicable
5. Generate relevant exceptions and error handling
6. Set appropriate priority and frequency based on use case type
7. Create realistic business rules and assumptions
8. Make all content professional and detailed

Examples of good use case analysis:
- ""User Login"": Primary actor: User, Trigger: User wants to access system, Normal flow: Navigate to login, Enter credentials, Click login, Validate, Redirect
- ""Age Validation"": Primary actor: System, Trigger: User submits age, Normal flow: Receive input, Validate range, Return result
- ""Payment Processing"": Primary actor: Customer, Trigger: User wants to make payment, Normal flow: Enter payment details, Validate, Process, Confirm

Return ONLY the JSON object, no additional text or explanations.";
        }

        private UseCaseReport ParseUseCaseReportResponse(string generatedText)
        {
            try
            {
                // Clean the response text to extract JSON
                var jsonStart = generatedText.IndexOf('{');
                var jsonEnd = generatedText.LastIndexOf('}') + 1;
                
                if (jsonStart >= 0 && jsonEnd > jsonStart)
                {
                    var jsonText = generatedText.Substring(jsonStart, jsonEnd - jsonStart);
                    var report = JsonSerializer.Deserialize<UseCaseReport>(jsonText);
                    
                    if (report != null)
                    {
                        return report;
                    }
                }
                
                throw new Exception($"Could not extract JSON from Gemini response. Raw response: {generatedText}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error parsing use case report response: {ex.Message}. Raw response: {generatedText}", ex);
            }
        }

        private GeminiTestCaseResponse ParseGeneratedResponse(string generatedText)
        {
            try
            {
                // Clean the response text to extract JSON
                var jsonStart = generatedText.IndexOf('{');
                var jsonEnd = generatedText.LastIndexOf('}') + 1;
                
                if (jsonStart >= 0 && jsonEnd > jsonStart)
                {
                    var jsonText = generatedText.Substring(jsonStart, jsonEnd - jsonStart);
                    var testCaseRequest = JsonSerializer.Deserialize<TestCaseRequest>(jsonText);
                    
                    if (testCaseRequest == null)
                    {
                        return new GeminiTestCaseResponse
                        {
                            Success = false,
                            TestCaseRequest = null,
                            Message = "Failed to deserialize test case request"
                        };
                    }
                    
                    return new GeminiTestCaseResponse
                    {
                        Success = true,
                        TestCaseRequest = testCaseRequest,
                        Message = "Test cases generated successfully"
                    };
                }
                
                return new GeminiTestCaseResponse
                {
                    Success = false,
                    TestCaseRequest = null,
                    Message = $"Could not extract JSON from Gemini response. Raw response: {generatedText}"
                };
            }
            catch (Exception ex)
            {
                return new GeminiTestCaseResponse
                {
                    Success = false,
                    TestCaseRequest = null,
                    Message = $"Error parsing Gemini response: {ex.Message}. Raw response: {generatedText}"
                };
            }
        }
    }

    public class GeminiTestCaseResponse
    {
        public bool Success { get; set; }
        public TestCaseRequest? TestCaseRequest { get; set; }
        public string Message { get; set; } = "";
    }

    public class GeminiApiResponse
    {
        public Candidate[]? candidates { get; set; }
    }

    public class Candidate
    {
        public Content content { get; set; } = new();
    }

    public class Content
    {
        public Part[] parts { get; set; } = Array.Empty<Part>();
    }

    public class Part
    {
        public string text { get; set; } = "";
    }
}
