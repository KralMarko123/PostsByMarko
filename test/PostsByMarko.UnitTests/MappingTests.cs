using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;

namespace PostsByMarko.UnitTests
{
    public class MappingTests
    {
        private readonly Mock<ILogger> loggerMock = new();
        private readonly Mock<ILoggerFactory> loggerFactoryMock = new();

        public MappingTests() 
        {
            loggerFactoryMock
                .Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(loggerMock.Object);
        }

        [Fact]
        public void validate_mapping_configuration()
        {
            // Arrange
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Program).Assembly), loggerFactoryMock.Object);

            // Act

            // Assert
            mapperConfiguration.AssertConfigurationIsValid();
        }
    }
}
