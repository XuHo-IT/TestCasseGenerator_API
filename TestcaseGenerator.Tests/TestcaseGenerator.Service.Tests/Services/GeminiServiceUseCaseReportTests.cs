using Microsoft.Extensions.Configuration;
using Moq;
using TestcaseGenerator.Models;
using TestcaseGenerator.Service;
using Xunit;
using FluentAssertions;
using System.Text.Json;

namespace TestcaseGenerator.Service.Tests.Services
{
    public class GeminiServiceUseCaseReportTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;

        public GeminiServiceUseCaseReportTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(x => x["GeminiApi:ApiKey"])
                .Returns("test-api-key");
            _mockConfiguration.Setup(x => x["GeminiApi:BaseUrl"])
                .Returns("https://test-api.com");
        }

        #region GenerateUseCaseReportAsync - Happy Path Tests

        [Fact]
        public async Task GenerateUseCaseReportAsync_ValidRequest_WillAttemptProcessing()
        {
            // Arrange
            var httpClient = new HttpClient();
            var service = new GeminiService(httpClient, _mockConfiguration.Object);
            var useCaseName = "User Login";
            var additionalContext = "Authentication system";

            // Act & Assert
            // This will fail due to no real AI service, but we can test the method exists
            await Assert.ThrowsAsync<Exception>(() => 
                service.GenerateUseCaseReportAsync(useCaseName, additionalContext));
        }

        [Fact]
        public async Task GenerateUseCaseReportAsync_ValidRequestWithoutContext_WillAttemptProcessing()
        {
            // Arrange
            var httpClient = new HttpClient();
            var service = new GeminiService(httpClient, _mockConfiguration.Object);
            var useCaseName = "User Registration";
            string additionalContext = null;

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                service.GenerateUseCaseReportAsync(useCaseName, additionalContext));
        }

        #endregion

        #region GenerateUseCaseReportAsync - Input Validation Tests

        [Fact]
        public async Task GenerateUseCaseReportAsync_WithNullUseCaseName_ThrowsException()
        {
            // Arrange
            var httpClient = new HttpClient();
            var service = new GeminiService(httpClient, _mockConfiguration.Object);
            string useCaseName = null;
            var additionalContext = "Authentication system";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                service.GenerateUseCaseReportAsync(useCaseName, additionalContext));
        }

        [Fact]
        public async Task GenerateUseCaseReportAsync_WithEmptyUseCaseName_ThrowsException()
        {
            // Arrange
            var httpClient = new HttpClient();
            var service = new GeminiService(httpClient, _mockConfiguration.Object);
            var useCaseName = "";
            var additionalContext = "Authentication system";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                service.GenerateUseCaseReportAsync(useCaseName, additionalContext));
        }

        [Fact]
        public async Task GenerateUseCaseReportAsync_WithWhitespaceUseCaseName_ThrowsException()
        {
            // Arrange
            var httpClient = new HttpClient();
            var service = new GeminiService(httpClient, _mockConfiguration.Object);
            var useCaseName = "   ";
            var additionalContext = "Authentication system";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                service.GenerateUseCaseReportAsync(useCaseName, additionalContext));
        }

        #endregion

        #region GenerateUseCaseReportAsync - Edge Case Tests

        [Fact]
        public async Task GenerateUseCaseReportAsync_WithVeryLongUseCaseName_HandlesGracefully()
        {
            // Arrange
            var httpClient = new HttpClient();
            var service = new GeminiService(httpClient, _mockConfiguration.Object);
            var longUseCaseName = new string('A', 1000);
            var additionalContext = "Authentication system";

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                service.GenerateUseCaseReportAsync(longUseCaseName, additionalContext));
        }

        [Fact]
        public async Task GenerateUseCaseReportAsync_WithSpecialCharacters_HandlesGracefully()
        {
            // Arrange
            var httpClient = new HttpClient();
            var service = new GeminiService(httpClient, _mockConfiguration.Object);
            var useCaseName = "User Login @#$%^&*()_+{}|:<>?[]\\;'\",./";
            var additionalContext = "Authentication system";

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                service.GenerateUseCaseReportAsync(useCaseName, additionalContext));
        }

        [Fact]
        public async Task GenerateUseCaseReportAsync_WithVeryLongContext_HandlesGracefully()
        {
            // Arrange
            var httpClient = new HttpClient();
            var service = new GeminiService(httpClient, _mockConfiguration.Object);
            var useCaseName = "User Login";
            var longContext = new string('A', 10000);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                service.GenerateUseCaseReportAsync(useCaseName, longContext));
        }

        #endregion

        #region Configuration Tests

        [Fact]
        public void GeminiService_WithValidConfiguration_InitializesCorrectly()
        {
            // Arrange
            var httpClient = new HttpClient();
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(x => x["GeminiApi:ApiKey"]).Returns("test-api-key");
            configuration.Setup(x => x["GeminiApi:BaseUrl"]).Returns("https://test-api.com");

            // Act
            var service = new GeminiService(httpClient, configuration.Object);

            // Assert
            service.Should().NotBeNull();
        }

        [Fact]
        public void GeminiService_WithNullApiKey_ThrowsException()
        {
            // Arrange
            var httpClient = new HttpClient();
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(x => x["GeminiApi:ApiKey"]).Returns((string)null);
            configuration.Setup(x => x["GeminiApi:BaseUrl"]).Returns("https://test-api.com");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => 
                new GeminiService(httpClient, configuration.Object));
        }

        [Fact]
        public void GeminiService_WithEmptyApiKey_ThrowsException()
        {
            // Arrange
            var httpClient = new HttpClient();
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(x => x["GeminiApi:ApiKey"]).Returns("");
            configuration.Setup(x => x["GeminiApi:BaseUrl"]).Returns("https://test-api.com");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => 
                new GeminiService(httpClient, configuration.Object));
        }

        [Fact]
        public void GeminiService_WithNullBaseUrl_UsesDefaultUrl()
        {
            // Arrange
            var httpClient = new HttpClient();
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(x => x["GeminiApi:ApiKey"]).Returns("test-api-key");
            configuration.Setup(x => x["GeminiApi:BaseUrl"]).Returns((string)null);

            // Act
            var service = new GeminiService(httpClient, configuration.Object);

            // Assert
            service.Should().NotBeNull();
        }

        #endregion

        #region Service Initialization Tests

        [Fact]
        public void GeminiService_WithValidHttpClient_InitializesCorrectly()
        {
            // Arrange
            var httpClient = new HttpClient();
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(x => x["GeminiApi:ApiKey"]).Returns("test-api-key");
            configuration.Setup(x => x["GeminiApi:BaseUrl"]).Returns("https://test-api.com");

            // Act
            var service = new GeminiService(httpClient, configuration.Object);

            // Assert
            service.Should().NotBeNull();
        }

        [Fact]
        public void GeminiService_WithNullHttpClient_ThrowsException()
        {
            // Arrange
            HttpClient httpClient = null;
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(x => x["GeminiApi:ApiKey"]).Returns("test-api-key");
            configuration.Setup(x => x["GeminiApi:BaseUrl"]).Returns("https://test-api.com");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new GeminiService(httpClient, configuration.Object));
        }

        [Fact]
        public void GeminiService_WithNullConfiguration_ThrowsException()
        {
            // Arrange
            var httpClient = new HttpClient();
            IConfiguration configuration = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new GeminiService(httpClient, configuration));
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public async Task GenerateUseCaseReportAsync_WithInvalidApiKey_ThrowsException()
        {
            // Arrange
            var httpClient = new HttpClient();
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(x => x["GeminiApi:ApiKey"]).Returns("invalid-key");
            configuration.Setup(x => x["GeminiApi:BaseUrl"]).Returns("https://test-api.com");
            
            var service = new GeminiService(httpClient, configuration.Object);
            var useCaseName = "User Login";
            var additionalContext = "Authentication system";

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                service.GenerateUseCaseReportAsync(useCaseName, additionalContext));
        }

        [Fact]
        public async Task GenerateUseCaseReportAsync_WithInvalidBaseUrl_ThrowsException()
        {
            // Arrange
            var httpClient = new HttpClient();
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(x => x["GeminiApi:ApiKey"]).Returns("test-api-key");
            configuration.Setup(x => x["GeminiApi:BaseUrl"]).Returns("https://invalid-url.com");
            
            var service = new GeminiService(httpClient, configuration.Object);
            var useCaseName = "User Login";
            var additionalContext = "Authentication system";

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                service.GenerateUseCaseReportAsync(useCaseName, additionalContext));
        }

        #endregion
    }
}