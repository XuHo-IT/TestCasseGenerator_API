# TestCaseGenerator_API

A .NET Core Web API for generating comprehensive test cases and use case reports using AI (Google Gemini). This is a side feature of the main project **AI FOR SE**.

## Features

- 🤖 **AI-Powered Test Case Generation** - Generate test cases using Google Gemini AI
- 📊 **Use Case Report Generation** - Create detailed use case reports with Excel export
- 📈 **Excel Export** - Export generated test cases and reports to Excel format
- 🔧 **RESTful API** - Clean and well-documented API endpoints

## API Endpoints

### Test Case Generation
- `POST /api/testcase/generate-and-download` - Generate test cases with AI and download Excel
- `POST /api/testcase/generate-use-case-table` - Generate use case table with AI
- `POST /api/testcase/generate-use-case-report` - Generate use case report with AI

## Technology Stack

- **.NET 8.0** - Latest .NET framework
- **ASP.NET Core Web API** - RESTful API framework
- **Google Gemini AI** - AI-powered content generation
- **ClosedXML** - Excel file generation
- **System.Text.Json** - JSON serialization

## Configuration

The API requires a Google Gemini API key. Configure it in `appsettings.Development.json`:

```json
{
  "GeminiApi": {
    "ApiKey": "YOUR_GEMINI_API_KEY_HERE",
    "BaseUrl": "https://generativelanguage.googleapis.com/v1/models/gemini-2.5-flash:generateContent"
  }
}
```

## Getting Started

1. **Clone the repository**
   ```bash
   git clone https://github.com/XuHo-IT/TestCasseGenerator_API.git
   cd TestCasseGenerator_API
   ```

2. **Configure API Key**
   - Get a Google Gemini API key from [Google AI Studio](https://makersuite.google.com/app/apikey)
   - Update `appsettings.Development.json` with your API key

3. **Run the application**
   ```bash
   dotnet run
   ```

4. **Access the API**
   - API: `https://localhost:5001` (HTTPS) or `http://localhost:5000` (HTTP)
   - Swagger UI: `https://localhost:5001/swagger`

## Project Structure

```
TestcaseGenerator/
├── Controllers/          # API Controllers
├── Models/              # Data Models
├── Service/             # Business Logic Services
├── wwwroot/             # Static Files
├── appsettings.json     # Configuration
└── Program.cs           # Application Entry Point
```

## Usage Examples

### Generate Use Case Report
```bash
curl -X POST "https://localhost:5001/api/testcase/generate-use-case-report" \
  -H "Content-Type: application/json" \
  -d '{
    "useCaseName": "User Login",
    "additionalContext": "Authentication system with username and password validation"
  }'
```

### Generate Test Case Table
```bash
curl -X POST "https://localhost:5001/api/testcase/generate-use-case-table" \
  -H "Content-Type: application/json" \
  -d '{
    "useCaseName": "Payment Processing",
    "additionalContext": "Credit card payment with amount validation"
  }'
```

## Development

This project is part of the **AI FOR SE** ecosystem, providing AI-powered test case generation capabilities for software engineering projects.

## License

This project is part of the AI FOR SE initiative and follows the same licensing terms.

## Contributing

This is a side feature of the main AI FOR SE project. For contributions, please refer to the main project repository.

---

**Note:** This is a side feature of the main project **AI FOR SE**.
