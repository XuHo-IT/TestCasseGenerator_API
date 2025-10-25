# Use Case Report Feature - TestCase Generator API

## Overview

The **Use Case Report** feature is a core component of the TestCase Generator API that generates comprehensive use case specifications using AI (Google Gemini) and exports them to Excel format. This feature enables software engineers to create detailed use case reports for requirements analysis and documentation.

## Feature Architecture

```
Use Case Report Feature
├── API Endpoint: POST /api/testcase/generate-use-case-report
├── Input: UseCaseTableRequest (useCaseName, additionalContext)
├── Processing: GeminiService.GenerateUseCaseReportAsync()
├── Output: UseCaseReport (structured data)
└── Export: ExcelService.ExportUseCaseReportToExcel()
```

## Functions to Test

### 1. GenerateUseCaseReportAsync(useCaseName, additionalContext)
- **Main**: Generate comprehensive use case report using AI
- **Inputs**: 
  - `useCaseName` (string) - Name of the use case
  - `additionalContext` (string, optional) - Additional context for the use case
- **Returns**: `UseCaseReport` object with complete use case specification
- **Edge cases**: 
  - Empty use case name
  - Null additional context
  - Very long use case names
  - Special characters in use case name
- **Dependencies**: 
  - Google Gemini API
  - HTTP client configuration
  - JSON parsing and validation

### 2. CreateUseCaseReportPrompt(useCaseName, additionalContext)
- **Main**: Generate AI prompt for use case report generation
- **Inputs**: 
  - `useCaseName` (string) - Use case name
  - `additionalContext` (string, optional) - Additional context
- **Returns**: `string` - Formatted prompt for AI
- **Edge cases**: 
  - Null or empty use case name
  - Very long context strings
  - Special characters and formatting
- **Dependencies**: 
  - String formatting
  - Template generation

### 3. ParseUseCaseReportResponse(generatedText)
- **Main**: Parse AI response into UseCaseReport object
- **Inputs**: 
  - `generatedText` (string) - Raw AI response text
- **Returns**: `UseCaseReport` object
- **Edge cases**: 
  - Malformed JSON response
  - Missing required fields
  - Invalid data types
  - Empty or null response
- **Dependencies**: 
  - JSON deserialization
  - Data validation
  - Error handling

### 4. ExportUseCaseReportToExcel(useCaseReport)
- **Main**: Export use case report to Excel format
- **Inputs**: 
  - `useCaseReport` (UseCaseReport) - Complete use case data
- **Returns**: `byte[]` - Excel file bytes
- **Edge cases**: 
  - Null use case report
  - Empty collections (preconditions, postconditions, flows)
  - Very long text fields
  - Missing required fields
- **Dependencies**: 
  - ClosedXML library
  - Excel formatting
  - Memory stream handling

### 5. GenerateUseCaseReport(UseCaseTableRequest)
- **Main**: Controller endpoint for use case report generation
- **Inputs**: 
  - `request` (UseCaseTableRequest) - HTTP request body
- **Returns**: `IActionResult` - Excel file download or error response
- **Edge cases**: 
  - Invalid request body
  - Missing required fields
  - AI service failures
  - Excel generation errors
- **Dependencies**: 
  - GeminiService
  - ExcelService
  - HTTP response handling

## Data Models to Test

### UseCaseReport Model
```csharp
public class UseCaseReport
{
    public string UcId { get; set; }           // Use case identifier
    public string UcName { get; set; }         // Use case name
    public string CreatedBy { get; set; }      // Creator information
    public string DateCreated { get; set; }    // Creation date
    public string PrimaryActor { get; set; }   // Main actor
    public string SecondaryActors { get; set; } // Secondary actors
    public string Trigger { get; set; }        // Use case trigger
    public string Description { get; set; }    // Use case description
    public List<string> Preconditions { get; set; }     // Preconditions
    public List<string> Postconditions { get; set; }   // Postconditions
    public List<FlowStep> NormalFlow { get; set; }      // Normal flow steps
    public List<AlternativeFlow> AlternativeFlows { get; set; } // Alternative flows
    public List<ExceptionFlow> Exceptions { get; set; } // Exception flows
    public string Priority { get; set; }       // Priority level
    public string FrequencyOfUse { get; set; } // Frequency of use
    public string BusinessRules { get; set; }  // Business rules
    public List<string> OtherInformation { get; set; } // Additional info
    public List<string> Assumptions { get; set; }       // Assumptions
}
```

### FlowStep Model
```csharp
public class FlowStep
{
    public string Step { get; set; }           // Step number
    public string Description { get; set; }     // Step description
}
```

### AlternativeFlow Model
```csharp
public class AlternativeFlow
{
    public string FlowId { get; set; }        // Flow identifier
    public string FlowName { get; set; }       // Flow name
    public List<FlowStep> Steps { get; set; }  // Flow steps
}
```

### ExceptionFlow Model
```csharp
public class ExceptionFlow
{
    public string ExceptionId { get; set; }    // Exception identifier
    public string ExceptionName { get; set; }  // Exception name
    public List<string> Descriptions { get; set; } // Exception descriptions
}
```

## Test Scenarios

### 1. Happy Path Testing
- **Valid use case name with context**
- **Complete use case report generation**
- **Successful Excel export**
- **Proper HTTP response with file download**

### 2. Input Validation Testing
- **Empty use case name**
- **Null additional context**
- **Very long use case names (>100 characters)**
- **Special characters in use case name**
- **Invalid request body format**

### 3. AI Service Integration Testing
- **Successful AI response parsing**
- **Malformed AI responses**
- **AI service timeout scenarios**
- **Invalid JSON from AI service**
- **Missing required fields in AI response**

### 4. Excel Generation Testing
- **Valid use case report to Excel conversion**
- **Empty collections handling**
- **Very long text fields in Excel**
- **Missing required fields**
- **Excel formatting and styling**

### 5. Error Handling Testing
- **Network failures**
- **AI service errors**
- **Excel generation failures**
- **Memory allocation issues**
- **Configuration errors**

## API Endpoint Testing

### POST /api/testcase/generate-use-case-report

#### Request Format
```json
{
  "useCaseName": "User Login",
  "additionalContext": "Authentication system with username and password validation"
}
```

#### Response Format
- **Success**: Excel file download (application/vnd.openxmlformats-officedocument.spreadsheetml.sheet)
- **Error**: JSON error response with appropriate HTTP status code

#### Test Cases
1. **Valid Request**
   - Input: Valid use case name and context
   - Expected: 200 OK with Excel file
   - File name format: `usecase_report_{UcId}_{timestamp}.xlsx`

2. **Invalid Request**
   - Input: Empty or null use case name
   - Expected: 400 Bad Request with error message

3. **AI Service Failure**
   - Input: Valid request but AI service fails
   - Expected: 500 Internal Server Error with error message

4. **Excel Generation Failure**
   - Input: Valid request but Excel generation fails
   - Expected: 500 Internal Server Error with error message

## Performance Testing

### Load Testing Scenarios
- **Concurrent requests**: 10+ simultaneous use case report generations
- **Large use case reports**: Complex use cases with many flows and exceptions
- **Memory usage**: Monitor memory consumption during Excel generation
- **Response time**: Measure end-to-end response times

### Stress Testing Scenarios
- **High volume**: 100+ requests per minute
- **Large data**: Use cases with extensive flows and descriptions
- **Long-running requests**: AI service timeout scenarios

## Security Testing

### Input Validation
- **SQL injection attempts** in use case names
- **XSS attacks** in additional context
- **Path traversal** in file names
- **Buffer overflow** in large inputs

### API Security
- **Authentication** (if implemented)
- **Rate limiting** (if implemented)
- **CORS configuration**
- **Input sanitization**

## Integration Testing

### End-to-End Testing
1. **Complete workflow**: Request → AI processing → Excel generation → Download
2. **Service dependencies**: HTTP client, configuration, file system
3. **Error propagation**: Service errors to API responses
4. **Data consistency**: Input validation through output verification

### Service Integration
- **GeminiService integration** with HTTP client
- **ExcelService integration** with ClosedXML
- **Configuration service** integration
- **Logging service** integration

## Monitoring and Logging

### Key Metrics to Monitor
- **Request count** per endpoint
- **Response times** for each service
- **Error rates** by error type
- **AI service usage** and costs
- **Excel generation performance**

### Logging Requirements
- **Request/response logging** for debugging
- **Error logging** with stack traces
- **Performance logging** for optimization
- **AI service interaction logging**

## Configuration Testing

### Environment Configuration
- **Development environment** testing
- **Production environment** testing
- **Configuration validation**
- **API key management**

### Service Configuration
- **Gemini API configuration**
- **HTTP client configuration**
- **Excel generation settings**
- **CORS configuration**

## Deployment Testing

### Build and Deployment
- **Build process** validation
- **Dependency resolution**
- **Configuration deployment**
- **Service startup** verification

### Runtime Testing
- **Service health checks**
- **Configuration loading**
- **External service connectivity**
- **File system permissions**

---

## Quick Start Testing

### 1. Unit Testing
```bash
# Run unit tests
dotnet test --filter "UseCaseReport"

# Run specific test class
dotnet test --filter "TestcaseControllerTests"
```

### 2. Integration Testing
```bash
# Run integration tests
dotnet test --filter "Integration"

# Run with test server
dotnet test --filter "TestcaseControllerIntegrationTests"
```

### 3. Manual Testing
```bash
# Test the endpoint
curl -X POST "https://localhost:5001/api/testcase/generate-use-case-report" \
  -H "Content-Type: application/json" \
  -d '{
    "useCaseName": "User Login",
    "additionalContext": "Authentication system"
  }'
```

---

**Note**: This feature is part of the AI FOR SE project and provides comprehensive use case report generation capabilities for software engineering documentation and requirements analysis.
