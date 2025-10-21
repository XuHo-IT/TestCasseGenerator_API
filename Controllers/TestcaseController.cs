using Microsoft.AspNetCore.Mvc;
using TestcaseGenerator.Models;
using TestcaseGenerator.Service;

namespace TestcaseGenerator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestcaseController : ControllerBase
    {
        private readonly ExcelService _excelService = new();
        private readonly GeminiService _geminiService;

        public TestcaseController(HttpClient httpClient, IConfiguration configuration)
        {
            _geminiService = new GeminiService(httpClient, configuration);
        }

        [HttpPost("generate-and-download")]
        public async Task<IActionResult> GenerateAndDownload([FromBody] AITestCaseRequest request)
        {
            try
            {
                // Generate test case data using Gemini AI
                var aiResponse = await _geminiService.GenerateTestCasesAsync(request.Request);
                
                if (!aiResponse.Success || aiResponse.TestCaseRequest == null)
                {
                    return BadRequest(new { message = aiResponse.Message });
                }

                // Generate Excel file using the AI-generated data
                var fileBytes = _excelService.ExportToExcel(aiResponse.TestCaseRequest);
                var fileName = $"testcases_{aiResponse.TestCaseRequest.FunctionCode}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error generating test cases: {ex.Message}" });
            }
        }

        [HttpPost("generate-use-case-table")]
        public async Task<IActionResult> GenerateUseCaseTable([FromBody] UseCaseTableRequest request)
        {
            try
            {
                // Generate comprehensive test case table using AI
                var aiResponse = await _geminiService.GenerateUseCaseTableAsync(request.UseCaseName, request.AdditionalContext);
                
                if (!aiResponse.Success || aiResponse.TestCaseRequest == null)
                {
                    return BadRequest(new { message = aiResponse.Message });
                }

                // Generate Excel file using the AI-generated data
                var fileBytes = _excelService.ExportToExcel(aiResponse.TestCaseRequest);
                var fileName = $"usecase_{aiResponse.TestCaseRequest.FunctionCode}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error generating use case table: {ex.Message}" });
            }
        }

        [HttpPost("generate-use-case-report")]
        public async Task<IActionResult> GenerateUseCaseReport([FromBody] UseCaseTableRequest request)
        {
            try
            {
                // Generate comprehensive use case report using AI
                var useCaseReport = await _geminiService.GenerateUseCaseReportAsync(request.UseCaseName, request.AdditionalContext);
                
                // Generate Excel file using the AI-generated use case report
                var fileBytes = _excelService.ExportUseCaseReportToExcel(useCaseReport);
                var fileName = $"usecase_report_{useCaseReport.UcId}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error generating use case report: {ex.Message}" });
            }
        }

    }
}
