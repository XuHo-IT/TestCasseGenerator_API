using System.Text.Json;
using TestcaseGenerator.Service; // For the GeminiApiResponse models

namespace TestcaseGenerator.Service.Tests
{
    /// <summary>
    /// Holds realistic mock JSON payloads for testing the GeminiService.
    /// </summary>
    public static class GeminiMockData
    {
        // Scenario 1: Inner payload for a successful UseCaseReport
        public static readonly string SuccessUseCaseReportPayload = @"
        {
          ""ucId"": ""UC-001"",
          ""ucName"": ""User Login"",
          ""createdBy"": ""AI Assistant"",
          ""dateCreated"": ""25/10/2025"",
          ""primaryActor"": ""User"",
          ""secondaryActors"": ""Authentication System"",
          ""trigger"": ""User wants to access a protected resource."",
          ""description"": ""This use case describes how a user logs into the system."",
          ""preconditions"": [ ""User has a valid account"", ""System is online"" ],
          ""postconditions"": [ ""User is successfully authenticated"", ""A session is created"" ],
          ""normalFlow"": [
            { ""step"": ""1"", ""description"": ""User enters username and password."" },
            { ""step"": ""2"", ""description"": ""User clicks the 'Login' button."" }
          ],
          ""alternativeFlows"": [
            { ""flowId"": ""A1"", ""flowName"": ""Invalid Credentials"", ""steps"": [ { ""step"": ""1"", ""description"": ""Show 'Invalid username or password' error."" } ] }
          ],
          ""exceptions"": [
            { ""exceptionId"": ""E1"", ""exceptionName"": ""System Timeout"", ""descriptions"": [ ""If the system does not respond in 5 seconds."" ] }
          ],
          ""priority"": ""High"",
          ""frequencyOfUse"": ""High"",
          ""businessRules"": ""Password must be 8+ characters."",
          ""otherInformation"": [],
          ""assumptions"": [ ""User has a keyboard."" ]
        }";

        // Scenario 2: Inner payload for a successful TestCaseRequest (for GenerateTestCases/GenerateUseCaseTable)
        public static readonly string SuccessTestCaseRequestPayload = @"
        {
          ""functionCode"": ""LOGIN-001"",
          ""functionName"": ""User Login Validation"",
          ""createdBy"": ""AI Assistant"",
          ""testRequirement"": ""Test the user login function"",
          ""fields"": [
            { ""name"": ""username"", ""type"": ""string"", ""min"": 5, ""max"": 50 },
            { ""name"": ""password"", ""type"": ""string"", ""min"": 8, ""max"": 100 }
          ],
          ""returnConditions"": [ ""Login successful"", ""Invalid credentials"", ""Account locked"" ],
          ""logMessages"": [ ""User logged in"", ""Login failed for user"" ]
        }";

        // Scenario 3: Outer payload for an API-level error (e.g., 400 Bad Request)
        public static readonly string ApiErrorPayload = @"
        {
          ""error"": {
            ""code"": 400,
            ""message"": ""API key not valid. Please pass a valid API key."",
            ""status"": ""INVALID_ARGUMENT""
          }
        }";

        // Scenario 4: Outer payload for a 200 OK response with no candidates
        public static readonly string EmptyCandidatesPayload = @"
        {
          ""candidates"": []
        }";

        // Scenario 5: Inner payload that is malformed JSON (to test parsing failure)
        public static readonly string MalformedJsonPayload = @"
        {
          ""ucId"": ""UC-001"",
          ""ucName"": ""User Login"",
          ""preconditions"": [ ""User has a valid account"" 
          // Missing closing bracket and brace
        ";

        /// <summary>
        /// Helper to build the full Gemini API response string, wrapping the inner JSON payload.
        /// </summary>
        public static string CreateSuccessApiResponse(string innerPayloadText)
        {
            var response = new GeminiApiResponse
            {
                candidates = new[]
                {
                    new Candidate
                    {
                        content = new Content
                        {
                            parts = new[]
                            {
                                new Part { text = innerPayloadText }
                            }
                        }
                    }
                }
            };
            return JsonSerializer.Serialize(response);
        }
    }
}