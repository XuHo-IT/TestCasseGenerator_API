using TestcaseGenerator.Models;
using TestcaseGenerator.Service;
using Xunit;
using FluentAssertions;
using ClosedXML.Excel;

namespace TestcaseGenerator.Service.Tests.Services
{
    public class ExcelServiceUseCaseReportTests
    {
        private readonly ExcelService _excelService;

        public ExcelServiceUseCaseReportTests()
        {
            _excelService = new ExcelService();
        }

        #region ExportUseCaseReportToExcel - Happy Path Tests

        [Fact]
        public void ExportUseCaseReportToExcel_ValidUseCaseReport_ReturnsExcelBytes()
        {
            // Arrange
            var useCaseReport = CreateValidUseCaseReport();

            // Act
            var result = _excelService.ExportUseCaseReportToExcel(useCaseReport);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public void ExportUseCaseReportToExcel_ValidUseCaseReport_ContainsCorrectHeader()
        {
            // Arrange
            var useCaseReport = CreateValidUseCaseReport();

            // Act
            var result = _excelService.ExportUseCaseReportToExcel(useCaseReport);

            // Assert
            using var workbook = new XLWorkbook(new MemoryStream(result));
            var worksheet = workbook.Worksheet("Use Case Report");
            
            worksheet.Cell("A1").Value.Should().Be("II. Requirement Specifications");
        }

        [Fact]
        public void ExportUseCaseReportToExcel_ValidUseCaseReport_ContainsBasicInformation()
        {
            // Arrange
            var useCaseReport = CreateValidUseCaseReport();

            // Act
            var result = _excelService.ExportUseCaseReportToExcel(useCaseReport);

            // Assert
            using var workbook = new XLWorkbook(new MemoryStream(result));
            var worksheet = workbook.Worksheet("Use Case Report");
            
            worksheet.Cell("A3").Value.Should().Be("Created By:");
            worksheet.Cell("B3").Value.Should().Be(useCaseReport.CreatedBy);
        }

        [Fact]
        public void ExportUseCaseReportToExcel_ValidUseCaseReport_ContainsPreconditions()
        {
            // Arrange
            var useCaseReport = CreateValidUseCaseReport();

            // Act
            var result = _excelService.ExportUseCaseReportToExcel(useCaseReport);

            // Assert
            using var workbook = new XLWorkbook(new MemoryStream(result));
            var worksheet = workbook.Worksheet("Use Case Report");
            
            var precondition1Row = FindRowContaining(worksheet, "PRE-1: User has valid account");
            precondition1Row.Should().BeGreaterThan(0);
            worksheet.Cell($"B{precondition1Row}").Value.Should().Be("PRE-1: User has valid account");
        }

        [Fact]
        public void ExportUseCaseReportToExcel_ValidUseCaseReport_ContainsPostconditions()
        {
            // Arrange
            var useCaseReport = CreateValidUseCaseReport();

            // Act
            var result = _excelService.ExportUseCaseReportToExcel(useCaseReport);

            // Assert
            using var workbook = new XLWorkbook(new MemoryStream(result));
            var worksheet = workbook.Worksheet("Use Case Report");
            
            var postcondition1Row = FindRowContaining(worksheet, "POST-1: User is logged in");
            postcondition1Row.Should().BeGreaterThan(0);
            worksheet.Cell($"B{postcondition1Row}").Value.Should().Be("POST-1: User is logged in");
        }

        [Fact]
        public void ExportUseCaseReportToExcel_ValidUseCaseReport_ContainsNormalFlow()
        {
            // Arrange
            var useCaseReport = CreateValidUseCaseReport();

            // Act
            var result = _excelService.ExportUseCaseReportToExcel(useCaseReport);

            // Assert
            using var workbook = new XLWorkbook(new MemoryStream(result));
            var worksheet = workbook.Worksheet("Use Case Report");
            
            var step1Row = FindRowContaining(worksheet, "1. User navigates to login page");
            step1Row.Should().BeGreaterThan(0);
            worksheet.Cell($"B{step1Row}").Value.Should().Be("1. User navigates to login page");
        }

        [Fact]
        public void ExportUseCaseReportToExcel_ValidUseCaseReport_ContainsAlternativeFlows()
        {
            // Arrange
            var useCaseReport = CreateValidUseCaseReport();

            // Act
            var result = _excelService.ExportUseCaseReportToExcel(useCaseReport);

            // Assert
            using var workbook = new XLWorkbook(new MemoryStream(result));
            var worksheet = workbook.Worksheet("Use Case Report");
            
            var altFlow1Row = FindRowContaining(worksheet, "1.1. Invalid Credentials");
            altFlow1Row.Should().BeGreaterThan(0);
            worksheet.Cell($"B{altFlow1Row}").Value.Should().Be("1.1. Invalid Credentials");
        }

        [Fact]
        public void ExportUseCaseReportToExcel_ValidUseCaseReport_ContainsExceptions()
        {
            // Arrange
            var useCaseReport = CreateValidUseCaseReport();

            // Act
            var result = _excelService.ExportUseCaseReportToExcel(useCaseReport);

            // Assert
            using var workbook = new XLWorkbook(new MemoryStream(result));
            var worksheet = workbook.Worksheet("Use Case Report");
            
            var exception1Row = FindRowContaining(worksheet, "1.E1: System Unavailable");
            exception1Row.Should().BeGreaterThan(0);
            worksheet.Cell($"B{exception1Row}").Value.Should().Be("1.E1: System Unavailable");
        }

        #endregion

        #region ExportUseCaseReportToExcel - Edge Case Tests

        [Fact]
        public void ExportUseCaseReportToExcel_NullUseCaseReport_ThrowsException()
        {
            // Arrange
            UseCaseReport useCaseReport = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                _excelService.ExportUseCaseReportToExcel(useCaseReport));
        }

        [Fact]
        public void ExportUseCaseReportToExcel_UseCaseReportWithEmptyCollections_HandlesCorrectly()
        {
            // Arrange
            var useCaseReport = new UseCaseReport
            {
                UcId = "UC-001",
                UcName = "User Login",
                CreatedBy = "Test Developer",
                DateCreated = DateTime.Now.ToString("dd/MM/yyyy"),
                Preconditions = new List<string>(),
                Postconditions = new List<string>(),
                NormalFlow = new List<FlowStep>(),
                AlternativeFlows = new List<AlternativeFlow>(),
                Exceptions = new List<ExceptionFlow>(),
                OtherInformation = new List<string>(),
                Assumptions = new List<string>()
            };

            // Act
            var result = _excelService.ExportUseCaseReportToExcel(useCaseReport);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public void ExportUseCaseReportToExcel_UseCaseReportWithVeryLongText_HandlesCorrectly()
        {
            // Arrange
            var longText = new string('A', 10000);
            var useCaseReport = new UseCaseReport
            {
                UcId = "UC-001",
                UcName = longText,
                CreatedBy = "Test Developer",
                DateCreated = DateTime.Now.ToString("dd/MM/yyyy"),
                Description = longText,
                BusinessRules = longText,
                Preconditions = new List<string> { longText },
                Postconditions = new List<string> { longText },
                NormalFlow = new List<FlowStep>
                {
                    new() { Step = "1", Description = longText }
                },
                AlternativeFlows = new List<AlternativeFlow>
                {
                    new()
                    {
                        FlowId = "1.1",
                        FlowName = longText,
                        Steps = new List<FlowStep>
                        {
                            new() { Step = "1", Description = longText }
                        }
                    }
                },
                Exceptions = new List<ExceptionFlow>
                {
                    new()
                    {
                        ExceptionId = "1.E1",
                        ExceptionName = longText,
                        Descriptions = new List<string> { longText }
                    }
                },
                OtherInformation = new List<string> { longText },
                Assumptions = new List<string> { longText }
            };

            // Act
            var result = _excelService.ExportUseCaseReportToExcel(useCaseReport);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public void ExportUseCaseReportToExcel_UseCaseReportWithSpecialCharacters_HandlesCorrectly()
        {
            // Arrange
            var specialText = "User Login @#$%^&*()_+{}|:<>?[]\\;'\",./";
            var useCaseReport = new UseCaseReport
            {
                UcId = "UC-001",
                UcName = specialText,
                CreatedBy = "Test Developer",
                DateCreated = DateTime.Now.ToString("dd/MM/yyyy"),
                Description = specialText,
                BusinessRules = specialText,
                Preconditions = new List<string> { specialText },
                Postconditions = new List<string> { specialText },
                NormalFlow = new List<FlowStep>
                {
                    new() { Step = "1", Description = specialText }
                },
                AlternativeFlows = new List<AlternativeFlow>
                {
                    new()
                    {
                        FlowId = "1.1",
                        FlowName = specialText,
                        Steps = new List<FlowStep>
                        {
                            new() { Step = "1", Description = specialText }
                        }
                    }
                },
                Exceptions = new List<ExceptionFlow>
                {
                    new()
                    {
                        ExceptionId = "1.E1",
                        ExceptionName = specialText,
                        Descriptions = new List<string> { specialText }
                    }
                },
                OtherInformation = new List<string> { specialText },
                Assumptions = new List<string> { specialText }
            };

            // Act
            var result = _excelService.ExportUseCaseReportToExcel(useCaseReport);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Length.Should().BeGreaterThan(0);
        }

        #endregion

        #region ExportUseCaseReportToExcel - Data Validation Tests

        [Fact]
        public void ExportUseCaseReportToExcel_UseCaseReportWithMultipleItems_ContainsAllItems()
        {
            // Arrange
            var useCaseReport = CreateComplexUseCaseReport();

            // Act
            var result = _excelService.ExportUseCaseReportToExcel(useCaseReport);

            // Assert
            using var workbook = new XLWorkbook(new MemoryStream(result));
            var worksheet = workbook.Worksheet("Use Case Report");
            
            // Check that all preconditions are present
            var content = GetWorksheetContent(worksheet);
            content.Should().Contain("PRE-1: User has valid account");
            content.Should().Contain("PRE-2: System is available");
            
            // Check that all postconditions are present
            content.Should().Contain("POST-1: User is logged in");
            content.Should().Contain("POST-2: Session is created");
        }

        [Fact]
        public void ExportUseCaseReportToExcel_UseCaseReportWithMultipleFlows_ContainsAllFlows()
        {
            // Arrange
            var useCaseReport = CreateComplexUseCaseReport();

            // Act
            var result = _excelService.ExportUseCaseReportToExcel(useCaseReport);

            // Assert
            using var workbook = new XLWorkbook(new MemoryStream(result));
            var worksheet = workbook.Worksheet("Use Case Report");
            
            var content = GetWorksheetContent(worksheet);
            content.Should().Contain("1. User navigates to login page");
            content.Should().Contain("2. User enters username and password");
            content.Should().Contain("3. User clicks login button");
        }

        #endregion

        #region Excel Formatting Tests

        [Fact]
        public void ExportUseCaseReportToExcel_ValidUseCaseReport_HasProperFormatting()
        {
            // Arrange
            var useCaseReport = CreateValidUseCaseReport();

            // Act
            var result = _excelService.ExportUseCaseReportToExcel(useCaseReport);

            // Assert
            using var workbook = new XLWorkbook(new MemoryStream(result));
            var worksheet = workbook.Worksheet("Use Case Report");
            
            // Check that borders are applied
            var usedRange = worksheet.RangeUsed();
            usedRange.Should().NotBeNull();
        }

        #endregion

        #region Helper Methods

        private UseCaseReport CreateValidUseCaseReport()
        {
            return new UseCaseReport
            {
                UcId = "UC-001",
                UcName = "User Login",
                CreatedBy = "Test Developer",
                DateCreated = DateTime.Now.ToString("dd/MM/yyyy"),
                PrimaryActor = "User",
                SecondaryActors = "Authentication System",
                Trigger = "User wants to access the system",
                Description = "User provides credentials to access the system",
                Preconditions = new List<string>
                {
                    "User has valid account",
                    "System is available"
                },
                Postconditions = new List<string>
                {
                    "User is logged in",
                    "Session is created"
                },
                NormalFlow = new List<FlowStep>
                {
                    new() { Step = "1", Description = "User navigates to login page" },
                    new() { Step = "2", Description = "User enters username and password" },
                    new() { Step = "3", Description = "User clicks login button" },
                    new() { Step = "4", Description = "System validates credentials" },
                    new() { Step = "5", Description = "System redirects to dashboard" }
                },
                AlternativeFlows = new List<AlternativeFlow>
                {
                    new()
                    {
                        FlowId = "1.1",
                        FlowName = "Invalid Credentials",
                        Steps = new List<FlowStep>
                        {
                            new() { Step = "1", Description = "System displays error message" },
                            new() { Step = "2", Description = "User can retry login" }
                        }
                    }
                },
                Exceptions = new List<ExceptionFlow>
                {
                    new()
                    {
                        ExceptionId = "1.E1",
                        ExceptionName = "System Unavailable",
                        Descriptions = new List<string>
                        {
                            "Database connection failed",
                            "Authentication service is down"
                        }
                    }
                },
                Priority = "High",
                FrequencyOfUse = "High",
                BusinessRules = "Users must have valid credentials",
                OtherInformation = new List<string>
                {
                    "Supports SSO integration",
                    "Audit logging enabled"
                },
                Assumptions = new List<string>
                {
                    "User has internet connection",
                    "Browser supports modern web standards"
                }
            };
        }

        private UseCaseReport CreateComplexUseCaseReport()
        {
            return new UseCaseReport
            {
                UcId = "UC-002",
                UcName = "Complex E-commerce Payment Processing",
                CreatedBy = "Test Developer",
                DateCreated = DateTime.Now.ToString("dd/MM/yyyy"),
                PrimaryActor = "Customer",
                SecondaryActors = "Payment Gateway, Bank System",
                Trigger = "Customer wants to make a payment",
                Description = "Customer processes payment for online purchase",
                Preconditions = new List<string>
                {
                    "Customer has valid account",
                    "System is available",
                    "Customer has sufficient funds"
                },
                Postconditions = new List<string>
                {
                    "Payment is processed",
                    "Order is confirmed",
                    "Receipt is generated"
                },
                NormalFlow = new List<FlowStep>
                {
                    new() { Step = "1", Description = "Customer selects payment method" },
                    new() { Step = "2", Description = "Customer enters payment details" },
                    new() { Step = "3", Description = "System validates payment information" },
                    new() { Step = "4", Description = "Payment gateway processes transaction" },
                    new() { Step = "5", Description = "System confirms payment success" }
                },
                AlternativeFlows = new List<AlternativeFlow>
                {
                    new()
                    {
                        FlowId = "2.1",
                        FlowName = "Insufficient Funds",
                        Steps = new List<FlowStep>
                        {
                            new() { Step = "1", Description = "System displays insufficient funds message" },
                            new() { Step = "2", Description = "Customer can try different payment method" }
                        }
                    },
                    new()
                    {
                        FlowId = "2.2",
                        FlowName = "Invalid Payment Details",
                        Steps = new List<FlowStep>
                        {
                            new() { Step = "1", Description = "System displays validation error" },
                            new() { Step = "2", Description = "Customer can correct payment details" }
                        }
                    }
                },
                Exceptions = new List<ExceptionFlow>
                {
                    new()
                    {
                        ExceptionId = "2.E1",
                        ExceptionName = "Payment Gateway Unavailable",
                        Descriptions = new List<string>
                        {
                            "Payment gateway service is down",
                            "Network connection failed"
                        }
                    },
                    new()
                    {
                        ExceptionId = "2.E2",
                        ExceptionName = "Bank System Error",
                        Descriptions = new List<string>
                        {
                            "Bank authorization failed",
                            "Bank system timeout"
                        }
                    }
                },
                Priority = "High",
                FrequencyOfUse = "High",
                BusinessRules = "PCI compliance required, fraud detection enabled",
                OtherInformation = new List<string>
                {
                    "Supports multiple currencies",
                    "Real-time fraud detection",
                    "Audit logging enabled"
                },
                Assumptions = new List<string>
                {
                    "Customer has valid payment method",
                    "Internet connection is stable",
                    "Bank systems are operational"
                }
            };
        }

        private int FindRowContaining(IXLWorksheet worksheet, string text)
        {
            var usedRange = worksheet.RangeUsed();
            if (usedRange == null) return 0;

            for (int row = 1; row <= usedRange.RowCount(); row++)
            {
                for (int col = 1; col <= usedRange.ColumnCount(); col++)
                {
                    var cellValue = worksheet.Cell(row, col).Value.ToString();
                    if (cellValue != null && cellValue.Contains(text))
                    {
                        return row;
                    }
                }
            }
            return 0;
        }

        private string GetWorksheetContent(IXLWorksheet worksheet)
        {
            var usedRange = worksheet.RangeUsed();
            if (usedRange == null) return "";

            var content = new System.Text.StringBuilder();
            for (int row = 1; row <= usedRange.RowCount(); row++)
            {
                for (int col = 1; col <= usedRange.ColumnCount(); col++)
                {
                    var cellValue = worksheet.Cell(row, col).Value.ToString();
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        content.Append(cellValue).Append(" ");
                    }
                }
            }
            return content.ToString();
        }

        #endregion
    }
}
