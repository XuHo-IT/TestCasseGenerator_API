using TestcaseGenerator.Models;
using Xunit;
using FluentAssertions;
using System.Text.Json;

namespace TestcaseGenerator.Tests.Models
{
    public class UseCaseReportModelTests
    {
        #region UseCaseReport Model Tests

        [Fact]
        public void UseCaseReport_DefaultConstructor_InitializesCorrectly()
        {
            // Act
            var report = new UseCaseReport();

            // Assert
            report.Should().NotBeNull();
            report.UcId.Should().Be("");
            report.UcName.Should().Be("");
            report.CreatedBy.Should().Be("");
            report.DateCreated.Should().Be("");
            report.PrimaryActor.Should().Be("");
            report.SecondaryActors.Should().Be("");
            report.Trigger.Should().Be("");
            report.Description.Should().Be("");
            report.Preconditions.Should().NotBeNull();
            report.Postconditions.Should().NotBeNull();
            report.NormalFlow.Should().NotBeNull();
            report.AlternativeFlows.Should().NotBeNull();
            report.Exceptions.Should().NotBeNull();
            report.Priority.Should().Be("");
            report.FrequencyOfUse.Should().Be("");
            report.BusinessRules.Should().Be("");
            report.OtherInformation.Should().NotBeNull();
            report.Assumptions.Should().NotBeNull();
        }

        [Fact]
        public void UseCaseReport_WithValidData_PropertiesSetCorrectly()
        {
            // Arrange
            var report = new UseCaseReport
            {
                UcId = "UC-001",
                UcName = "User Login",
                CreatedBy = "Test Developer",
                DateCreated = "01/01/2024",
                PrimaryActor = "User",
                SecondaryActors = "Authentication System",
                Trigger = "User wants to access the system",
                Description = "User provides credentials to access the system",
                Priority = "High",
                FrequencyOfUse = "High",
                BusinessRules = "Users must have valid credentials"
            };

            // Assert
            report.UcId.Should().Be("UC-001");
            report.UcName.Should().Be("User Login");
            report.CreatedBy.Should().Be("Test Developer");
            report.DateCreated.Should().Be("01/01/2024");
            report.PrimaryActor.Should().Be("User");
            report.SecondaryActors.Should().Be("Authentication System");
            report.Trigger.Should().Be("User wants to access the system");
            report.Description.Should().Be("User provides credentials to access the system");
            report.Priority.Should().Be("High");
            report.FrequencyOfUse.Should().Be("High");
            report.BusinessRules.Should().Be("Users must have valid credentials");
        }

        [Fact]
        public void UseCaseReport_WithCollections_CollectionsSetCorrectly()
        {
            // Arrange
            var report = new UseCaseReport
            {
                Preconditions = new List<string> { "User has valid account", "System is available" },
                Postconditions = new List<string> { "User is logged in", "Session is created" },
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
                        Descriptions = new List<string> { "Database connection failed" }
                    }
                },
                OtherInformation = new List<string> { "Supports SSO integration" },
                Assumptions = new List<string> { "User has internet connection" }
            };

            // Assert
            report.Preconditions.Should().HaveCount(2);
            report.Postconditions.Should().HaveCount(2);
            report.NormalFlow.Should().HaveCount(2);
            report.AlternativeFlows.Should().HaveCount(1);
            report.Exceptions.Should().HaveCount(1);
            report.OtherInformation.Should().HaveCount(1);
            report.Assumptions.Should().HaveCount(1);
        }

        [Fact]
        public void UseCaseReport_JsonSerialization_WorksCorrectly()
        {
            // Arrange
            var report = CreateValidUseCaseReport();

            // Act
            var json = JsonSerializer.Serialize(report);
            var deserializedReport = JsonSerializer.Deserialize<UseCaseReport>(json);

            // Assert
            deserializedReport.Should().NotBeNull();
            deserializedReport.UcId.Should().Be(report.UcId);
            deserializedReport.UcName.Should().Be(report.UcName);
            deserializedReport.PrimaryActor.Should().Be(report.PrimaryActor);
            deserializedReport.Preconditions.Should().HaveCount(report.Preconditions.Count);
            deserializedReport.Postconditions.Should().HaveCount(report.Postconditions.Count);
            deserializedReport.NormalFlow.Should().HaveCount(report.NormalFlow.Count);
        }

        [Fact]
        public void UseCaseReport_JsonDeserializationWithNullValues_WorksCorrectly()
        {
            // Arrange
            var json = """
                {
                    "ucId": "UC-001",
                    "ucName": "User Login",
                    "createdBy": "Test Developer",
                    "dateCreated": "01/01/2024",
                    "primaryActor": "User",
                    "secondaryActors": null,
                    "trigger": null,
                    "description": null,
                    "preconditions": ["User has valid account"],
                    "postconditions": ["User is logged in"],
                    "normalFlow": [{"step": "1", "description": "User navigates to login page"}],
                    "alternativeFlows": [],
                    "exceptions": [],
                    "priority": "High",
                    "frequencyOfUse": "High",
                    "businessRules": "Users must have valid credentials",
                    "otherInformation": [],
                    "assumptions": []
                }
                """;

            // Act
            var report = JsonSerializer.Deserialize<UseCaseReport>(json);

            // Assert
            report.Should().NotBeNull();
            report.UcId.Should().Be("UC-001");
            report.UcName.Should().Be("User Login");
            report.SecondaryActors.Should().BeNull();
            report.Trigger.Should().BeNull();
            report.Description.Should().BeNull();
            report.Preconditions.Should().NotBeNull();
            report.Postconditions.Should().NotBeNull();
            report.NormalFlow.Should().NotBeNull();
            report.AlternativeFlows.Should().NotBeNull();
            report.Exceptions.Should().NotBeNull();
        }

        #endregion

        #region FlowStep Model Tests

        [Fact]
        public void FlowStep_DefaultConstructor_InitializesCorrectly()
        {
            // Act
            var flowStep = new FlowStep();

            // Assert
            flowStep.Should().NotBeNull();
            flowStep.Step.Should().Be("");
            flowStep.Description.Should().Be("");
        }

        [Fact]
        public void FlowStep_WithValidData_PropertiesSetCorrectly()
        {
            // Arrange
            var flowStep = new FlowStep
            {
                Step = "1",
                Description = "User navigates to login page"
            };

            // Assert
            flowStep.Step.Should().Be("1");
            flowStep.Description.Should().Be("User navigates to login page");
        }

        [Fact]
        public void FlowStep_JsonSerialization_WorksCorrectly()
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

        #endregion

        #region AlternativeFlow Model Tests

        [Fact]
        public void AlternativeFlow_DefaultConstructor_InitializesCorrectly()
        {
            // Act
            var alternativeFlow = new AlternativeFlow();

            // Assert
            alternativeFlow.Should().NotBeNull();
            alternativeFlow.FlowId.Should().Be("");
            alternativeFlow.FlowName.Should().Be("");
            alternativeFlow.Steps.Should().NotBeNull();
        }

        [Fact]
        public void AlternativeFlow_WithValidData_PropertiesSetCorrectly()
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

            // Assert
            alternativeFlow.FlowId.Should().Be("1.1");
            alternativeFlow.FlowName.Should().Be("Invalid Credentials");
            alternativeFlow.Steps.Should().HaveCount(2);
        }

        [Fact]
        public void AlternativeFlow_JsonSerialization_WorksCorrectly()
        {
            // Arrange
            var alternativeFlow = new AlternativeFlow
            {
                FlowId = "1.1",
                FlowName = "Invalid Credentials",
                Steps = new List<FlowStep>
                {
                    new() { Step = "1", Description = "System displays error message" }
                }
            };

            // Act
            var json = JsonSerializer.Serialize(alternativeFlow);
            var deserializedFlow = JsonSerializer.Deserialize<AlternativeFlow>(json);

            // Assert
            deserializedFlow.Should().NotBeNull();
            deserializedFlow.FlowId.Should().Be(alternativeFlow.FlowId);
            deserializedFlow.FlowName.Should().Be(alternativeFlow.FlowName);
            deserializedFlow.Steps.Should().HaveCount(1);
        }

        #endregion

        #region ExceptionFlow Model Tests

        [Fact]
        public void ExceptionFlow_DefaultConstructor_InitializesCorrectly()
        {
            // Act
            var exceptionFlow = new ExceptionFlow();

            // Assert
            exceptionFlow.Should().NotBeNull();
            exceptionFlow.ExceptionId.Should().Be("");
            exceptionFlow.ExceptionName.Should().Be("");
            exceptionFlow.Descriptions.Should().NotBeNull();
        }

        [Fact]
        public void ExceptionFlow_WithValidData_PropertiesSetCorrectly()
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

            // Assert
            exceptionFlow.ExceptionId.Should().Be("1.E1");
            exceptionFlow.ExceptionName.Should().Be("System Unavailable");
            exceptionFlow.Descriptions.Should().HaveCount(2);
        }

        [Fact]
        public void ExceptionFlow_JsonSerialization_WorksCorrectly()
        {
            // Arrange
            var exceptionFlow = new ExceptionFlow
            {
                ExceptionId = "1.E1",
                ExceptionName = "System Unavailable",
                Descriptions = new List<string>
                {
                    "Database connection failed"
                }
            };

            // Act
            var json = JsonSerializer.Serialize(exceptionFlow);
            var deserializedException = JsonSerializer.Deserialize<ExceptionFlow>(json);

            // Assert
            deserializedException.Should().NotBeNull();
            deserializedException.ExceptionId.Should().Be(exceptionFlow.ExceptionId);
            deserializedException.ExceptionName.Should().Be(exceptionFlow.ExceptionName);
            deserializedException.Descriptions.Should().HaveCount(1);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void UseCaseReport_WithVeryLongText_HandlesCorrectly()
        {
            // Arrange
            var longText = new string('A', 10000);
            var report = new UseCaseReport
            {
                UcId = "UC-001",
                UcName = longText,
                Description = longText,
                BusinessRules = longText
            };

            // Act
            var json = JsonSerializer.Serialize(report);
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
            var report = new UseCaseReport
            {
                UcId = "UC-001",
                UcName = specialText,
                Description = specialText,
                BusinessRules = specialText
            };

            // Act
            var json = JsonSerializer.Serialize(report);
            var deserializedReport = JsonSerializer.Deserialize<UseCaseReport>(json);

            // Assert
            deserializedReport.Should().NotBeNull();
            deserializedReport.UcName.Should().Be(specialText);
            deserializedReport.Description.Should().Be(specialText);
        }

        [Fact]
        public void UseCaseReport_WithEmptyCollections_HandlesCorrectly()
        {
            // Arrange
            var report = new UseCaseReport
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
            var json = JsonSerializer.Serialize(report);
            var deserializedReport = JsonSerializer.Deserialize<UseCaseReport>(json);

            // Assert
            deserializedReport.Should().NotBeNull();
            deserializedReport.Preconditions.Should().NotBeNull();
            deserializedReport.Postconditions.Should().NotBeNull();
            deserializedReport.NormalFlow.Should().NotBeNull();
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

        #endregion
    }
}
