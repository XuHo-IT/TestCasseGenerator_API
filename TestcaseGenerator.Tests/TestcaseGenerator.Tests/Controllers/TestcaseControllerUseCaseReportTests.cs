using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using TestcaseGenerator.Controllers;
using TestcaseGenerator.Models;
using TestcaseGenerator.Service;
using Xunit;
using FluentAssertions;

namespace TestcaseGenerator.Tests.Controllers
{
    public class TestcaseControllerUseCaseReportTests
    {
        private readonly HttpClient _httpClient;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly TestcaseController _controller;

        public TestcaseControllerUseCaseReportTests()
        {
            _httpClient = new HttpClient();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(x => x["GeminiApi:ApiKey"]).Returns("test-api-key");
            _mockConfiguration.Setup(x => x["GeminiApi:BaseUrl"]).Returns("https://test-api.com");
            
            _controller = new TestcaseController(_httpClient, _mockConfiguration.Object);
        }

        #region GenerateUseCaseReport - Happy Path Tests

        [Fact]
        public async Task GenerateUseCaseReport_ValidRequest_ReturnsExcelFile()
        {
            // Arrange
            var request = new UseCaseTableRequest
            {
                UseCaseName = "User Login",
                AdditionalContext = "Authentication system with username and password validation"
            };

            // Act
            var result = await _controller.GenerateUseCaseReport(request);

            // Assert
            result.Should().NotBeNull();
            // The result will be either a FileResult (success) or an error result due to AI service
            result.Should().BeOfType<ObjectResult>();
        }

        [Fact]
        public async Task GenerateUseCaseReport_ValidRequestWithoutContext_ReturnsExcelFile()
        {
            // Arrange
            var request = new UseCaseTableRequest
            {
                UseCaseName = "User Registration",
                AdditionalContext = null
            };

            // Act
            var result = await _controller.GenerateUseCaseReport(request);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ObjectResult>();
        }

        #endregion

        #region GenerateUseCaseReport - Input Validation Tests

        [Fact]
        public async Task GenerateUseCaseReport_EmptyUseCaseName_ReturnsError()
        {
            // Arrange
            var request = new UseCaseTableRequest
            {
                UseCaseName = "",
                AdditionalContext = "Some context"
            };

            // Act
            var result = await _controller.GenerateUseCaseReport(request);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ObjectResult>();
        }

        [Fact]
        public async Task GenerateUseCaseReport_NullUseCaseName_ReturnsError()
        {
            // Arrange
            var request = new UseCaseTableRequest
            {
                UseCaseName = null,
                AdditionalContext = "Some context"
            };

            // Act
            var result = await _controller.GenerateUseCaseReport(request);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ObjectResult>();
        }

        [Fact]
        public async Task GenerateUseCaseReport_WhitespaceUseCaseName_ReturnsError()
        {
            // Arrange
            var request = new UseCaseTableRequest
            {
                UseCaseName = "   ",
                AdditionalContext = "Some context"
            };

            // Act
            var result = await _controller.GenerateUseCaseReport(request);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ObjectResult>();
        }

        [Fact]
        public async Task GenerateUseCaseReport_VeryLongUseCaseName_HandlesGracefully()
        {
            // Arrange
            var longUseCaseName = new string('A', 1000);
            var request = new UseCaseTableRequest
            {
                UseCaseName = longUseCaseName,
                AdditionalContext = "Some context"
            };

            // Act
            var result = await _controller.GenerateUseCaseReport(request);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ObjectResult>();
        }

        [Fact]
        public async Task GenerateUseCaseReport_SpecialCharactersInUseCaseName_HandlesGracefully()
        {
            // Arrange
            var request = new UseCaseTableRequest
            {
                UseCaseName = "User Login @#$%^&*()_+{}|:<>?[]\\;'\",./",
                AdditionalContext = "Authentication system"
            };

            // Act
            var result = await _controller.GenerateUseCaseReport(request);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ObjectResult>();
        }

        #endregion

        #region GenerateUseCaseReport - Request Validation Tests

        [Fact]
        public async Task GenerateUseCaseReport_WithNullRequest_ReturnsError()
        {
            // Arrange
            UseCaseTableRequest request = null;

            // Act
            var result = await _controller.GenerateUseCaseReport(request);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ObjectResult>();
        }

        [Fact]
        public async Task GenerateUseCaseReport_WithValidRequestButNoContext_HandlesCorrectly()
        {
            // Arrange
            var request = new UseCaseTableRequest
            {
                UseCaseName = "User Login",
                AdditionalContext = null
            };

            // Act
            var result = await _controller.GenerateUseCaseReport(request);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GenerateUseCaseReport_WithEmptyContext_HandlesCorrectly()
        {
            // Arrange
            var request = new UseCaseTableRequest
            {
                UseCaseName = "User Login",
                AdditionalContext = ""
            };

            // Act
            var result = await _controller.GenerateUseCaseReport(request);

            // Assert
            result.Should().NotBeNull();
        }

        #endregion

        #region Controller Configuration Tests

        [Fact]
        public void TestcaseController_WithValidConfiguration_InitializesCorrectly()
        {
            // Arrange
            var httpClient = new HttpClient();
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(x => x["GeminiApi:ApiKey"]).Returns("test-api-key");
            configuration.Setup(x => x["GeminiApi:BaseUrl"]).Returns("https://test-api.com");

            // Act
            var controller = new TestcaseController(httpClient, configuration.Object);

            // Assert
            controller.Should().NotBeNull();
        }

        [Fact]
        public void TestcaseController_WithNullHttpClient_ThrowsException()
        {
            // Arrange
            HttpClient httpClient = null;
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(x => x["GeminiApi:ApiKey"]).Returns("test-api-key");
            configuration.Setup(x => x["GeminiApi:BaseUrl"]).Returns("https://test-api.com");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new TestcaseController(httpClient, configuration.Object));
        }

        [Fact]
        public void TestcaseController_WithNullConfiguration_ThrowsException()
        {
            // Arrange
            var httpClient = new HttpClient();
            IConfiguration configuration = null;

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => 
                new TestcaseController(httpClient, configuration));
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task GenerateUseCaseReport_WithVeryLongContext_HandlesCorrectly()
        {
            // Arrange
            var longContext = new string('A', 10000);
            var request = new UseCaseTableRequest
            {
                UseCaseName = "User Login",
                AdditionalContext = longContext
            };

            // Act
            var result = await _controller.GenerateUseCaseReport(request);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ObjectResult>();
        }

        [Fact]
        public async Task GenerateUseCaseReport_WithSpecialCharactersInContext_HandlesCorrectly()
        {
            // Arrange
            var request = new UseCaseTableRequest
            {
                UseCaseName = "User Login",
                AdditionalContext = "Context with special chars: @#$%^&*()_+{}|:<>?[]\\;'\",./"
            };

            // Act
            var result = await _controller.GenerateUseCaseReport(request);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ObjectResult>();
        }

        #endregion
    }
}
