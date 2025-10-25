using System.Net;
using System.Text;
using System.Text.Json;
using TestcaseGenerator.Models;
using Xunit;
using FluentAssertions;

namespace TestcaseGenerator.Integration.Tests
{
    public class UseCaseReportIntegrationTests
    {
        #region Model Integration Tests

        [Fact]
        public void UseCaseReport_ModelSerialization_WorksCorrectly()
        {
            // Arrange
            var useCaseReport = CreateMockUseCaseReport();

            // Act
            var json = JsonSerializer.Serialize(useCaseReport);
            var deserializedReport = JsonSerializer.Deserialize<UseCaseReport>(json);

            // Assert
            deserializedReport.Should().NotBeNull();
            deserializedReport.UcId.Should().Be(useCaseReport.UcId);
            deserializedReport.UcName.Should().Be(useCaseReport.UcName);
            deserializedReport.PrimaryActor.Should().Be(useCaseReport.PrimaryActor);
        }

        [Fact]
        public void UseCaseTableRequest_ModelSerialization_WorksCorrectly()
        {
            // Arrange
            var request = new UseCaseTableRequest
            {
                UseCaseName = "User Login",
                AdditionalContext = "Authentication system"
            };

            // Act
            var json = JsonSerializer.Serialize(request);
            var deserializedRequest = JsonSerializer.Deserialize<UseCaseTableRequest>(json);

            // Assert
            deserializedRequest.Should().NotBeNull();
            deserializedRequest.UseCaseName.Should().Be(request.UseCaseName);
            deserializedRequest.AdditionalContext.Should().Be(request.AdditionalContext);
        }

        #endregion

        #region Data Validation Integration Tests

        [Fact]
        public void UseCaseReport_WithComplexData_SerializesCorrectly()
        {
            // Arrange
            var complexReport = CreateComplexUseCaseReport();

            // Act
            var json = JsonSerializer.Serialize(complexReport);
            var deserializedReport = JsonSerializer.Deserialize<UseCaseReport>(json);

            // Assert
            deserializedReport.Should().NotBeNull();
            deserializedReport.NormalFlow.Should().HaveCount(complexReport.NormalFlow.Count);
            deserializedReport.AlternativeFlows.Should().HaveCount(complexReport.AlternativeFlows.Count);
            deserializedReport.Exceptions.Should().HaveCount(complexReport.Exceptions.Count);
        }

        [Fact]
        public void UseCaseReport_WithEmptyCollections_HandlesCorrectly()
        {
            // Arrange
            var reportWithEmptyCollections = new UseCaseReport
            {
                UcId = "UC-001",
                UcName = "Test Use Case",
                Preconditions = new List<string>(),
                Postconditions = new List<string>(),
                NormalFlow = new List<FlowStep>(),
                AlternativeFlows = new List<AlternativeFlow>(),
                Exceptions = new List<ExceptionFlow>(),
                OtherInformation = new List<string>(),
                Assumptions = new List<string>()
            };

            // Act
            var json = JsonSerializer.Serialize(reportWithEmptyCollections);
            var deserializedReport = JsonSerializer.Deserialize<UseCaseReport>(json);

            // Assert
            deserializedReport.Should().NotBeNull();
            deserializedReport.Preconditions.Should().NotBeNull();
            deserializedReport.Postconditions.Should().NotBeNull();
            deserializedReport.NormalFlow.Should().NotBeNull();
        }

        #endregion

        #region Edge Case Integration Tests

        [Fact]
        public void UseCaseReport_WithVeryLongText_HandlesCorrectly()
        {
            // Arrange
            var longText = new string('A', 10000);
            var reportWithLongText = new UseCaseReport
            {
                UcId = "UC-001",
                UcName = longText,
                Description = longText,
                BusinessRules = longText
            };

            // Act
            var json = JsonSerializer.Serialize(reportWithLongText);
            var deserializedReport = JsonSerializer.Deserialize<UseCaseReport>(json);

            // Assert
            deserializedReport.Should().NotBeNull();
            deserializedReport.UcName.Should().Be(longText);
            deserializedReport.Description.Should().Be(longText);
        }

        [Fact]
        public void UseCaseReport_WithSpecialCharacters_HandlesCorrectly()
        {
            // Arrange
            var specialText = "User Login @#$%^&*()_+{}|:<>?[]\\;'\",./";
            var reportWithSpecialChars = new UseCaseReport
            {
                UcId = "UC-001",
                UcName = specialText,
                Description = specialText,
                BusinessRules = specialText
            };

            // Act
            var json = JsonSerializer.Serialize(reportWithSpecialChars);
            var deserializedReport = JsonSerializer.Deserialize<UseCaseReport>(json);

            // Assert
            deserializedReport.Should().NotBeNull();
            deserializedReport.UcName.Should().Be(specialText);
            deserializedReport.Description.Should().Be(specialText);
        }

        #endregion

        #region Flow Integration Tests

        [Fact]
        public void FlowStep_Serialization_WorksCorrectly()
        {
            // Arrange
            var flowStep = new FlowStep
            {
                Step = "1",
                Description = "User navigates to login page"
            };

            // Act
            var json = JsonSerializer.Serialize(flowStep);
            var deserializedStep = JsonSerializer.Deserialize<FlowStep>(json);

            // Assert
            deserializedStep.Should().NotBeNull();
            deserializedStep.Step.Should().Be(flowStep.Step);
            deserializedStep.Description.Should().Be(flowStep.Description);
        }

        [Fact]
        public void AlternativeFlow_Serialization_WorksCorrectly()
        {
            // Arrange
            var alternativeFlow = new AlternativeFlow
            {
                FlowId = "1.1",
                FlowName = "Invalid Credentials",
                Steps = new List<FlowStep>
                {
                    new() { Step = "1", Description = "System displays error message" },
                    new() { Step = "2", Description = "User can retry login" }
                }
            };

            // Act
            var json = JsonSerializer.Serialize(alternativeFlow);
            var deserializedFlow = JsonSerializer.Deserialize<AlternativeFlow>(json);

            // Assert
            deserializedFlow.Should().NotBeNull();
            deserializedFlow.FlowId.Should().Be(alternativeFlow.FlowId);
            deserializedFlow.FlowName.Should().Be(alternativeFlow.FlowName);
            deserializedFlow.Steps.Should().HaveCount(2);
        }

        [Fact]
        public void ExceptionFlow_Serialization_WorksCorrectly()
        {
            // Arrange
            var exceptionFlow = new ExceptionFlow
            {
                ExceptionId = "1.E1",
                ExceptionName = "System Unavailable",
                Descriptions = new List<string>
                {
                    "Database connection failed",
                    "Authentication service is down"
                }
            };

            // Act
            var json = JsonSerializer.Serialize(exceptionFlow);
            var deserializedException = JsonSerializer.Deserialize<ExceptionFlow>(json);

            // Assert
            deserializedException.Should().NotBeNull();
            deserializedException.ExceptionId.Should().Be(exceptionFlow.ExceptionId);
            deserializedException.ExceptionName.Should().Be(exceptionFlow.ExceptionName);
            deserializedException.Descriptions.Should().HaveCount(2);
        }

        #endregion

        #region End-to-End Integration Tests

        [Fact]
        public void UseCaseReport_CompleteWorkflow_DataFlowWorksCorrectly()
        {
            // Arrange
            var request = new UseCaseTableRequest
            {
                UseCaseName = "User Login",
                AdditionalContext = "Authentication system"
            };

            // Act - Simulate the complete workflow
            var requestJson = JsonSerializer.Serialize(request);
            var deserializedRequest = JsonSerializer.Deserialize<UseCaseTableRequest>(requestJson);
            
            var useCaseReport = CreateMockUseCaseReport();
            var reportJson = JsonSerializer.Serialize(useCaseReport);
            var deserializedReport = JsonSerializer.Deserialize<UseCaseReport>(reportJson);

            // Assert
            deserializedRequest.Should().NotBeNull();
            deserializedRequest.UseCaseName.Should().Be("User Login");
            deserializedRequest.AdditionalContext.Should().Be("Authentication system");
            
            deserializedReport.Should().NotBeNull();
            deserializedReport.UcId.Should().Be("UC-001");
            deserializedReport.UcName.Should().Be("User Login");
        }

        [Fact]
        public void UseCaseReport_WithMultipleFlows_ComplexDataHandlesCorrectly()
        {
            // Arrange
            var complexReport = CreateComplexUseCaseReport();

            // Act
            var json = JsonSerializer.Serialize(complexReport);
            var deserializedReport = JsonSerializer.Deserialize<UseCaseReport>(json);

            // Assert
            deserializedReport.Should().NotBeNull();
            deserializedReport.NormalFlow.Should().HaveCount(5);
            deserializedReport.AlternativeFlows.Should().HaveCount(2);
            deserializedReport.Exceptions.Should().HaveCount(2);
            deserializedReport.Preconditions.Should().HaveCount(3);
            deserializedReport.Postconditions.Should().HaveCount(3);
        }

        #endregion

        #region Performance Integration Tests

        [Fact]
        public void UseCaseReport_LargeDataSet_PerformanceIsAcceptable()
        {
            // Arrange
            var largeReport = CreateLargeUseCaseReport();

            // Act
            var startTime = DateTime.Now;
            var json = JsonSerializer.Serialize(largeReport);
            var deserializedReport = JsonSerializer.Deserialize<UseCaseReport>(json);
            var endTime = DateTime.Now;

            // Assert
            deserializedReport.Should().NotBeNull();
            var duration = endTime - startTime;
            duration.TotalMilliseconds.Should().BeLessThan(1000); // Should complete within 1 second
        }

        #endregion

        #region Helper Methods

        private UseCaseReport CreateMockUseCaseReport()
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
                    new() { Step = "2", Description = "User enters username and password" }
                },
                AlternativeFlows = new List<AlternativeFlow>
                {
                    new()
                    {
                        FlowId = "1.1",
                        FlowName = "Invalid Credentials",
                        Steps = new List<FlowStep>
                        {
                            new() { Step = "1", Description = "System displays error message" }
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
                            "Database connection failed"
                        }
                    }
                },
                Priority = "High",
                FrequencyOfUse = "High",
                BusinessRules = "Users must have valid credentials",
                OtherInformation = new List<string>
                {
                    "Supports SSO integration"
                },
                Assumptions = new List<string>
                {
                    "User has internet connection"
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
                    "Payment system is available",
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

        private UseCaseReport CreateLargeUseCaseReport()
        {
            var report = new UseCaseReport
            {
                UcId = "UC-003",
                UcName = "Large Enterprise System Integration",
                CreatedBy = "Test Developer",
                DateCreated = DateTime.Now.ToString("dd/MM/yyyy"),
                PrimaryActor = "System Administrator",
                SecondaryActors = "Multiple External Systems",
                Trigger = "System integration required",
                Description = "Complex enterprise system integration with multiple external services",
                Preconditions = new List<string>(),
                Postconditions = new List<string>(),
                NormalFlow = new List<FlowStep>(),
                AlternativeFlows = new List<AlternativeFlow>(),
                Exceptions = new List<ExceptionFlow>(),
                OtherInformation = new List<string>(),
                Assumptions = new List<string>()
            };

            // Add many items to test performance
            for (int i = 1; i <= 100; i++)
            {
                report.Preconditions.Add($"Precondition {i}");
                report.Postconditions.Add($"Postcondition {i}");
                report.NormalFlow.Add(new FlowStep { Step = i.ToString(), Description = $"Step {i} description" });
                report.OtherInformation.Add($"Information {i}");
                report.Assumptions.Add($"Assumption {i}");
            }

            for (int i = 1; i <= 50; i++)
            {
                report.AlternativeFlows.Add(new AlternativeFlow
                {
                    FlowId = $"A{i}",
                    FlowName = $"Alternative Flow {i}",
                    Steps = new List<FlowStep>
                    {
                        new() { Step = "1", Description = $"Alternative step {i}" }
                    }
                });

                report.Exceptions.Add(new ExceptionFlow
                {
                    ExceptionId = $"E{i}",
                    ExceptionName = $"Exception {i}",
                    Descriptions = new List<string> { $"Exception description {i}" }
                });
            }

            return report;
        }

        #endregion
    }
}
