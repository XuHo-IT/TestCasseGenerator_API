using ClosedXML.Excel;
using TestcaseGenerator.Models;

namespace TestcaseGenerator.Service
{
    public class ExcelService
    {
        public byte[] ExportToExcel(TestCaseRequest request)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Test Cases");

            int row = 1;

            // ================= HEADER INFO SECTION =================
            // Row 1: Function Code and Function Name
            ws.Cell(row, 1).Value = "Function Code";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(row, 2).Value = request.FunctionCode;
            ws.Cell(row, 3).Value = "Function Name";
            ws.Cell(row, 3).Style.Font.Bold = true;
            ws.Cell(row, 3).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(row, 4).Value = request.FunctionName;
            row++;

            // Row 2: Created By and Executed By
            ws.Cell(row, 1).Value = "Created By";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(row, 2).Value = request.CreatedBy;
            ws.Cell(row, 3).Value = "Executed By";
            ws.Cell(row, 3).Style.Font.Bold = true;
            ws.Cell(row, 3).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(row, 4).Value = request.ExecutedBy ?? "";
            row++;

            // Row 3: Lines of Code and Test Requirement
            ws.Cell(row, 1).Value = "Lines of code";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(row, 2).Value = request.LinesOfCode ?? "";
            ws.Cell(row, 3).Value = "Test requirement";
            ws.Cell(row, 3).Style.Font.Bold = true;
            ws.Cell(row, 3).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(row, 4).Value = request.TestRequirement;
            row++;

            // Row 4: Lack of test cases
            ws.Cell(row, 1).Value = "Lack of test cases";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(row, 2).Value = "-2";
            row += 2;

            // ================= SUMMARY STATISTICS =================
            // Calculate test case statistics
            var testCaseIds = GenerateTestCaseIds(request.Fields.Count);
            var testResults = CalculateTestResults(request.Fields, testCaseIds);
            
            // Summary labels row - match image layout exactly
            ws.Cell(row, 1).Value = "Passed";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(row, 2).Value = "Failed";
            ws.Cell(row, 2).Style.Font.Bold = true;
            ws.Cell(row, 2).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(row, 3).Value = "Untested";
            ws.Cell(row, 3).Style.Font.Bold = true;
            ws.Cell(row, 3).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(row, 4).Value = "Normal";
            ws.Cell(row, 4).Style.Font.Bold = true;
            ws.Cell(row, 4).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(row, 5).Value = "Abnormal";
            ws.Cell(row, 5).Style.Font.Bold = true;
            ws.Cell(row, 5).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(row, 6).Value = "Boundary";
            ws.Cell(row, 6).Style.Font.Bold = true;
            ws.Cell(row, 6).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(row, 7).Value = "Total Test Cases";
            ws.Cell(row, 7).Style.Font.Bold = true;
            ws.Cell(row, 7).Style.Fill.BackgroundColor = XLColor.LightGray;
            row++;
            
            // Summary values row - match image layout exactly
            ws.Cell(row, 1).Value = testResults.PassedCount.ToString();
            ws.Cell(row, 2).Value = testResults.FailedCount.ToString();
            ws.Cell(row, 3).Value = testResults.UntestedCount.ToString();
            ws.Cell(row, 4).Value = testResults.NormalCount.ToString();
            ws.Cell(row, 5).Value = testResults.AbnormalCount.ToString();
            ws.Cell(row, 6).Value = testResults.BoundaryCount.ToString();
            ws.Cell(row, 7).Value = testCaseIds.Count.ToString();
            row += 2;

            // ================= MAIN TABLE HEADER =================
            ws.Cell(row, 1).Value = "Condition";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(row, 2).Value = "Precondition";
            ws.Cell(row, 2).Style.Font.Bold = true;
            ws.Cell(row, 2).Style.Fill.BackgroundColor = XLColor.LightGray;

            // Generate UTCID columns
            int utcidStartCol = 3;
            for (int i = 0; i < testCaseIds.Count; i++)
            {
                ws.Cell(row, utcidStartCol + i).Value = testCaseIds[i];
                ws.Cell(row, utcidStartCol + i).Style.Font.Bold = true;
                ws.Cell(row, utcidStartCol + i).Style.Fill.BackgroundColor = XLColor.LightGray;
                ws.Cell(row, utcidStartCol + i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
            row++;

            // ================= INPUT CONDITION SECTION =================
            ws.Cell(row, 2).Value = "Input condition";
            ws.Cell(row, 2).Style.Font.Bold = true;
            row++;

            foreach (var field in request.Fields)
            {
                // Field label
                ws.Cell(row, 2).Value = $"Input `{field.Name}`:";
                ws.Cell(row, 2).Style.Font.Bold = true;
                row++;

                // Generate test values for this field
                var testValues = GenerateTestValues(field);
                var fieldCol = utcidStartCol + request.Fields.IndexOf(field);

                foreach (var testValue in testValues)
                {
                    ws.Cell(row, 2).Value = testValue.Value;
                    // Add circle markers for selected test cases
                    foreach (var testCaseId in testValue.SelectedTestCases)
                    {
                        int testCaseCol = utcidStartCol + testCaseIds.IndexOf(testCaseId);
                        ws.Cell(row, testCaseCol).Value = "●";
                        ws.Cell(row, testCaseCol).Style.Font.FontColor = XLColor.Black;
                        ws.Cell(row, testCaseCol).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }
                    row++;
                }
            }

            // ================= CONFIRM SECTION =================
            ws.Cell(row, 1).Value = "Confirm";
            ws.Cell(row, 1).Style.Font.Bold = true;

            // Return subsection
            ws.Cell(row, 2).Value = "Return";
            ws.Cell(row, 2).Style.Font.Bold = true;
            row++;

            // Generate return conditions based on the request
            var returnConditions = request.ReturnConditions.Count > 0 ? request.ReturnConditions : GetDefaultReturnConditions(request);
            var testCaseAssignments = GenerateTestCaseAssignments(returnConditions, testCaseIds.Count);

            for (int i = 0; i < returnConditions.Count; i++)
            {
                ws.Cell(row, 2).Value = returnConditions[i];
                
                // Add markers for test cases assigned to this condition
                foreach (var testCaseIndex in testCaseAssignments[i])
                {
                    if (testCaseIndex < testCaseIds.Count)
                    {
                        ws.Cell(row, utcidStartCol + testCaseIndex).Value = "●";
                        ws.Cell(row, utcidStartCol + testCaseIndex).Style.Font.FontColor = XLColor.Black;
                        ws.Cell(row, utcidStartCol + testCaseIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }
                }
                row++;
            }

            // Exception subsection
            ws.Cell(row, 2).Value = "Exception";
            ws.Cell(row, 2).Style.Font.Bold = true;
            row++;

            // Log message subsection
            ws.Cell(row, 2).Value = "Log message";
            ws.Cell(row, 2).Style.Font.Bold = true;
            row++;

            // Generate log messages based on the request
            var logMessages = request.LogMessages.Count > 0 ? request.LogMessages : GetDefaultLogMessages(request);
            var logAssignments = GenerateTestCaseAssignments(logMessages, testCaseIds.Count);

            for (int i = 0; i < logMessages.Count; i++)
            {
                ws.Cell(row, 2).Value = logMessages[i];
                
                // Add markers for test cases assigned to this log message
                foreach (var testCaseIndex in logAssignments[i])
                {
                    if (testCaseIndex < testCaseIds.Count)
                    {
                        ws.Cell(row, utcidStartCol + testCaseIndex).Value = "●";
                        ws.Cell(row, utcidStartCol + testCaseIndex).Style.Font.FontColor = XLColor.Black;
                        ws.Cell(row, utcidStartCol + testCaseIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }
                }
                row++;
            }


            // ================= RESULT SECTION =================
            ws.Cell(row, 1).Value = "Result";
            ws.Cell(row, 1).Style.Font.Bold = true;

            // Type row
            ws.Cell(row, 2).Value = "Type(N : Normal, A : Abnormal, B : Boundary)";
            for (int i = 0; i < testCaseIds.Count; i++)
            {
                ws.Cell(row, utcidStartCol + i).Value = testResults.TestCaseTypes[i];
                ws.Cell(row, utcidStartCol + i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
            row++;

            // Passed/Failed row
            ws.Cell(row, 2).Value = "Passed/Failed";
            for (int i = 0; i < testCaseIds.Count; i++)
            {
                ws.Cell(row, utcidStartCol + i).Value = testResults.TestCaseResults[i];
                ws.Cell(row, utcidStartCol + i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
            row++;

            // Executed Date row
            ws.Cell(row, 2).Value = "Executed Date";
            for (int i = 0; i < testCaseIds.Count; i++)
            {
                ws.Cell(row, utcidStartCol + i).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                ws.Cell(row, utcidStartCol + i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
            row++;

            // Defect ID row
            ws.Cell(row, 2).Value = "Defect ID";
            for (int i = 0; i < testCaseIds.Count; i++)
            {
                ws.Cell(row, utcidStartCol + i).Value = testResults.DefectIds[i];
                ws.Cell(row, utcidStartCol + i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
            row++;

            // ================= STYLING =================
            var lastCol = utcidStartCol + testCaseIds.Count - 1;
            var tableRange = ws.Range(1, 1, row - 1, lastCol);
            
            // Add borders
            tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // Adjust column widths
            ws.Columns().AdjustToContents();
            ws.Rows().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            return stream.ToArray();
        }

        private List<string> GenerateTestCaseIds(int fieldCount)
        {
            var testCaseIds = new List<string>();
            int testCaseCount = Math.Max(2, fieldCount); // At least 2 test cases
            
            for (int i = 1; i <= testCaseCount; i++)
            {
                testCaseIds.Add($"UTCID{i:D2}");
            }
            
            return testCaseIds;
        }

        private List<(int Value, List<string> SelectedTestCases)> GenerateTestValues(InputField field)
        {
            var testValues = new List<(int Value, List<string> SelectedTestCases)>();
            
            // Generate comprehensive test values covering the full range
            var values = new List<int>();
            
            // Add boundary values
            values.Add(field.Min);
            values.Add(field.Max);
            
            // Add values just outside the boundary (for invalid testing)
            if (field.Min > int.MinValue)
            {
                values.Add(field.Min - 1);
            }
            if (field.Max < int.MaxValue)
            {
                values.Add(field.Max + 1);
            }
            
            // Add intermediate values to cover the range
            if (field.Max - field.Min > 1)
            {
                // Add values at 25%, 50%, 75% of the range
                int range = field.Max - field.Min;
                values.Add(field.Min + (int)(range * 0.25));
                values.Add(field.Min + (int)(range * 0.5));
                values.Add(field.Min + (int)(range * 0.75));
                
                // Add values just inside the boundary
                if (field.Min + 1 < field.Max)
                {
                    values.Add(field.Min + 1);
                }
                if (field.Max - 1 > field.Min)
                {
                    values.Add(field.Max - 1);
                }
            }
            
            // Sort values for better organization
            values.Sort();
            
            // Assign test cases to values
            for (int i = 0; i < values.Count; i++)
            {
                var selectedTestCases = new List<string>();
                
                // Assign test cases based on value type
                int value = values[i];
                
                if (value < field.Min || value > field.Max)
                {
                    // Invalid values - assign to abnormal test cases
                    if (i % 2 == 0)
                    {
                        selectedTestCases.Add("UTCID01");
                    }
                    else
                    {
                        selectedTestCases.Add("UTCID02");
                    }
                }
                else if (value == field.Min || value == field.Max)
                {
                    // Boundary values - assign to boundary test cases
                    if (value == field.Min)
                    {
                        selectedTestCases.Add("UTCID01");
                    }
                    else
                    {
                        selectedTestCases.Add("UTCID02");
                    }
                }
                else
                {
                    // Normal values - assign to normal test cases
                    selectedTestCases.Add($"UTCID{(i % 2) + 1:D2}");
                }
                
                testValues.Add((value, selectedTestCases));
            }
            
            return testValues;
        }

        private TestResult CalculateTestResults(List<InputField> fields, List<string> testCaseIds)
        {
            var result = new TestResult();
            var random = new Random();
            
            // Initialize lists
            result.TestCaseTypes = new List<string>();
            result.TestCaseResults = new List<string>();
            result.DefectIds = new List<string>();
            
            // Generate test cases based on boundary value analysis
            var testCases = GenerateBoundaryTestCases(fields, testCaseIds);
            
            // Calculate results for each test case
            for (int i = 0; i < testCaseIds.Count; i++)
            {
                var testCase = testCases[i];
                var testCaseId = testCaseIds[i];
                
                // Determine test type based on boundary analysis
                string testType = DetermineTestType(testCase, fields);
                result.TestCaseTypes.Add(testType);
                
                // Determine pass/fail based on test type and boundary conditions
                string testResult = DetermineTestResult(testType, testCase, fields);
                result.TestCaseResults.Add(testResult);
                
                // Count results
                if (testResult == "Passed")
                {
                    result.PassedCount++;
                }
                else if (testResult == "Failed")
                {
                    result.FailedCount++;
                }
                else
                {
                    result.UntestedCount++;
                }
                
                // Generate defect ID for failed tests
                if (testResult == "Failed")
                {
                    result.DefectIds.Add($"DF-{random.Next(100, 999)}");
                }
                else
                {
                    result.DefectIds.Add("-");
                }
            }
            
            // Calculate type counts
            result.NormalCount = result.TestCaseTypes.Count(t => t == "N");
            result.AbnormalCount = result.TestCaseTypes.Count(t => t == "A");
            result.BoundaryCount = result.TestCaseTypes.Count(t => t == "B");
            
            return result;
        }

        private List<Dictionary<string, int>> GenerateBoundaryTestCases(List<InputField> fields, List<string> testCaseIds)
        {
            var testCases = new List<Dictionary<string, int>>();
            
            // Generate boundary test cases for each field
            for (int i = 0; i < testCaseIds.Count; i++)
            {
                var testCase = new Dictionary<string, int>();
                
                foreach (var field in fields)
                {
                    // Generate test values based on boundary analysis
                    int testValue;
                    if (i == 0) // First test case - minimum boundary
                    {
                        testValue = field.Min;
                    }
                    else if (i == 1) // Second test case - maximum boundary
                    {
                        testValue = field.Max;
                    }
                    else if (i == 2 && fields.Count > 2) // Third test case - just above minimum
                    {
                        testValue = field.Min + 1;
                    }
                    else if (i == 3 && fields.Count > 3) // Fourth test case - just below maximum
                    {
                        testValue = field.Max - 1;
                    }
                    else // Additional test cases - random valid values
                    {
                        testValue = new Random().Next(field.Min, field.Max + 1);
                    }
                    
                    testCase[field.Name] = testValue;
                }
                
                testCases.Add(testCase);
            }
            
            return testCases;
        }

        private string DetermineTestType(Dictionary<string, int> testCase, List<InputField> fields)
        {
            // Determine test type based on boundary analysis
            bool hasMinBoundary = false;
            bool hasMaxBoundary = false;
            bool hasInvalidValue = false;
            
            foreach (var field in fields)
            {
                if (testCase.ContainsKey(field.Name))
                {
                    int value = testCase[field.Name];
                    
                    // Check for boundary conditions
                    if (value == field.Min || value == field.Max)
                    {
                        if (value == field.Min) hasMinBoundary = true;
                        if (value == field.Max) hasMaxBoundary = true;
                    }
                    
                    // Check for invalid values (outside range)
                    if (value < field.Min || value > field.Max)
                    {
                        hasInvalidValue = true;
                    }
                }
            }
            
            // Determine test type
            if (hasInvalidValue)
            {
                return "A"; // Abnormal - invalid input
            }
            else if (hasMinBoundary || hasMaxBoundary)
            {
                return "B"; // Boundary - at boundary values
            }
            else
            {
                return "N"; // Normal - within valid range
            }
        }

        private string DetermineTestResult(string testType, Dictionary<string, int> testCase, List<InputField> fields)
        {
            // Determine test result based on test type and business logic
            switch (testType)
            {
                case "A": // Abnormal - should fail
                    return "Failed";
                case "B": // Boundary - may pass or fail depending on business logic
                    // For demonstration, boundary tests pass
                    return "Passed";
                case "N": // Normal - should pass
                    return "Passed";
                default:
                    return "Untested";
            }
        }

        private List<string> GetDefaultReturnConditions(TestCaseRequest request)
        {
            // Generate default return conditions based on the test requirement
            var conditions = new List<string>();
            
            if (request.TestRequirement.ToLower().Contains("age"))
            {
                conditions.Add("Valid age");
                conditions.Add("Age too young");
                conditions.Add("Age too old");
            }
            else if (request.TestRequirement.ToLower().Contains("salary") || request.TestRequirement.ToLower().Contains("income"))
            {
                conditions.Add("Valid salary");
                conditions.Add("Salary too low");
                conditions.Add("Salary too high");
            }
            else if (request.TestRequirement.ToLower().Contains("percentage"))
            {
                conditions.Add("Valid percentage");
                conditions.Add("Percentage too low");
                conditions.Add("Percentage too high");
            }
            else
            {
                // Generic conditions
                conditions.Add("Valid result");
                conditions.Add("Invalid input");
                conditions.Add("Calculation error");
            }
            
            return conditions;
        }

        private List<string> GetDefaultLogMessages(TestCaseRequest request)
        {
            // Generate default log messages based on the test requirement
            var messages = new List<string>();
            
            if (request.TestRequirement.ToLower().Contains("age"))
            {
                messages.Add("Age is valid");
                messages.Add("Age must be at least 10");
                messages.Add("Age must not exceed 18");
            }
            else if (request.TestRequirement.ToLower().Contains("salary") || request.TestRequirement.ToLower().Contains("income"))
            {
                messages.Add("Salary is valid");
                messages.Add("Salary is below minimum threshold");
                messages.Add("Salary exceeds maximum limit");
            }
            else if (request.TestRequirement.ToLower().Contains("percentage"))
            {
                messages.Add("Percentage is valid");
                messages.Add("Percentage must be at least 0");
                messages.Add("Percentage must not exceed 100");
            }
            else
            {
                // Generic messages
                messages.Add("Operation successful");
                messages.Add("Invalid input parameters");
                messages.Add("Calculation error occurred");
            }
            
            return messages;
        }

        private List<List<int>> GenerateTestCaseAssignments(List<string> conditions, int testCaseCount)
        {
            var assignments = new List<List<int>>();
            
            // Distribute test cases among conditions
            for (int i = 0; i < conditions.Count; i++)
            {
                var assignedTestCases = new List<int>();
                
                // Assign test cases to conditions in a round-robin fashion
                for (int j = i; j < testCaseCount; j += conditions.Count)
                {
                    assignedTestCases.Add(j);
                }
                
                assignments.Add(assignedTestCases);
            }
            
            return assignments;
        }

        public byte[] ExportUseCaseReportToExcel(UseCaseReport report)
        {
            // Add debugging to see what data we're working with
            Console.WriteLine($"UseCaseReport Debug:");
            Console.WriteLine($"UcId: {report.UcId}");
            Console.WriteLine($"UcName: {report.UcName}");
            Console.WriteLine($"CreatedBy: {report.CreatedBy}");
            Console.WriteLine($"PrimaryActor: {report.PrimaryActor}");
            Console.WriteLine($"SecondaryActors: {report.SecondaryActors}");
            Console.WriteLine($"Trigger: {report.Trigger}");
            Console.WriteLine($"Description: {report.Description}");
            Console.WriteLine($"Preconditions Count: {report.Preconditions?.Count ?? 0}");
            Console.WriteLine($"Postconditions Count: {report.Postconditions?.Count ?? 0}");
            Console.WriteLine($"NormalFlow Count: {report.NormalFlow?.Count ?? 0}");
            Console.WriteLine($"AlternativeFlows Count: {report.AlternativeFlows?.Count ?? 0}");
            Console.WriteLine($"Exceptions Count: {report.Exceptions?.Count ?? 0}");
            Console.WriteLine($"Priority: {report.Priority}");
            Console.WriteLine($"FrequencyOfUse: {report.FrequencyOfUse}");
            Console.WriteLine($"BusinessRules: {report.BusinessRules}");
            Console.WriteLine($"OtherInformation Count: {report.OtherInformation?.Count ?? 0}");
            Console.WriteLine($"Assumptions Count: {report.Assumptions?.Count ?? 0}");

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Use Case Report");

            int row = 1;

            // ================= HEADER SECTION =================
            // Title
            ws.Cell(row, 1).Value = "II. Requirement Specifications";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Font.FontSize = 14;
            row ++;

            // UC ID and Name
            ws.Cell(row, 1).Value = "UC ID and Name:";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(row, 2).Value = $"{report.UcId}: {report.UcName}";
            row++;

            // Created By
            ws.Cell(row, 1).Value = "Created By:";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(row, 2).Value = report.CreatedBy;
            row++;

            // Date Created
            ws.Cell(row, 1).Value = "Date Created:";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(row, 2).Value = report.DateCreated;
            row++;

            // Primary Actor
            ws.Cell(row, 1).Value = "Primary Actor:";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(row, 2).Value = report.PrimaryActor;
            row++;

            // Secondary Actors
            ws.Cell(row, 1).Value = "Secondary Actors:";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(row, 2).Value = !string.IsNullOrEmpty(report.SecondaryActors) ? report.SecondaryActors : "None";
            row++;

            // ================= CORE USE CASE DETAILS =================
            // Trigger
            ws.Cell(row, 1).Value = "Trigger:";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(row, 2).Value = report.Trigger;
            row++;

            // Description
            ws.Cell(row, 1).Value = "Description:";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(row, 2).Value = report.Description;
            row++;

            // Preconditions
            ws.Cell(row, 1).Value = "Preconditions:";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            row++;
            
            for (int i = 0; i < (report.Preconditions?.Count ?? 0); i++)
            {
                ws.Cell(row, 2).Value = $"PRE-{i + 1}: {report.Preconditions?[i] ?? ""}";
                row++;
            }

            // Postconditions
            ws.Cell(row, 1).Value = "Postconditions:";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            row++;
            
            for (int i = 0; i < (report.Postconditions?.Count ?? 0); i++)
            {
                ws.Cell(row, 2).Value = $"POST-{i + 1}: {report.Postconditions?[i] ?? ""}";
                row++;
            }

            // ================= FLOWS SECTION =================
            // Normal Flow
            ws.Cell(row, 1).Value = "Normal Flow:";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            row++;

            foreach (var step in report.NormalFlow ?? new List<FlowStep>())
            {
                ws.Cell(row, 2).Value = $"{step.Step}. {step.Description ?? ""}";
                row++;
            }

            // Alternative Flows
            if ((report.AlternativeFlows?.Count ?? 0) > 0)
            {
                ws.Cell(row, 1).Value = "Alternative Flows:";
                ws.Cell(row, 1).Style.Font.Bold = true;
                ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                row++;

                foreach (var altFlow in report.AlternativeFlows ?? new List<AlternativeFlow>())
                {
                    ws.Cell(row, 2).Value = $"{altFlow.FlowId}. {altFlow.FlowName}";
                    ws.Cell(row, 2).Style.Font.Bold = true;
                    row++;

                    foreach (var step in altFlow.Steps)
                    {
                        ws.Cell(row, 2).Value = $"{step.Step}. {step.Description ?? ""}";
                        row++;
                    }
                }
            }

            // ================= EXCEPTIONS SECTION =================
            if ((report.Exceptions?.Count ?? 0) > 0)
            {
                ws.Cell(row, 1).Value = "Exceptions:";
                ws.Cell(row, 1).Style.Font.Bold = true;
                ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                row++;

                foreach (var exception in report.Exceptions ?? new List<ExceptionFlow>())
                {
                    ws.Cell(row, 2).Value = $"{exception.ExceptionId}: {exception.ExceptionName}";
                    ws.Cell(row, 2).Style.Font.Bold = true;
                    row++;

                    foreach (var description in exception.Descriptions)
                    {
                        ws.Cell(row, 2).Value = $"• {description}";
                        row++;
                    }
                }
            }

            // ================= ADDITIONAL INFORMATION SECTION =================
            ws.Cell(row, 1).Value = "Priority:";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(row, 2).Value = report.Priority;
            row++;

            ws.Cell(row, 1).Value = "Frequency of Use:";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(row, 2).Value = report.FrequencyOfUse;
            row++;

            ws.Cell(row, 1).Value = "Business Rules:";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(row, 2).Value = !string.IsNullOrEmpty(report.BusinessRules) ? report.BusinessRules : "None";
            row++;

            // Other Information
            if ((report.OtherInformation?.Count ?? 0) > 0)
            {
                ws.Cell(row, 1).Value = "Other Information:";
                ws.Cell(row, 1).Style.Font.Bold = true;
                ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                row++;

                foreach (var info in report.OtherInformation ?? new List<string>())
                {
                    ws.Cell(row, 2).Value = $"• {info ?? ""}";
                    row++;
                }
            }

            // Assumptions
            if ((report.Assumptions?.Count ?? 0) > 0)
            {
                ws.Cell(row, 1).Value = "Assumptions:";
                ws.Cell(row, 1).Style.Font.Bold = true;
                ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                row++;

                foreach (var assumption in report.Assumptions ?? new List<string>())
                {
                    ws.Cell(row, 2).Value = $"• {assumption ?? ""}";
                    row++;
                }
            }

            // ================= STYLING =================
            // Add borders and formatting
            var usedRange = ws.Range(1, 1, row - 1, 2);
            usedRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            usedRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            usedRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

            // Adjust column widths
            ws.Column(1).Width = 20;
            ws.Column(2).Width = 80;

            // Adjust row heights
            ws.Rows().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            return stream.ToArray();
        }
    }

    public class TestResult
    {
        public int PassedCount { get; set; }
        public int FailedCount { get; set; }
        public int UntestedCount { get; set; }
        public int NormalCount { get; set; }
        public int AbnormalCount { get; set; }
        public int BoundaryCount { get; set; }
        public List<string> TestCaseTypes { get; set; } = new();
        public List<string> TestCaseResults { get; set; } = new();
        public List<string> DefectIds { get; set; } = new();
    }
}
