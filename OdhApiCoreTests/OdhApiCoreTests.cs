using Microsoft.Extensions.Configuration;
using OdhApiCore.Controllers.api;
using System;
using Xunit;

namespace OdhApiCoreTests
{
    public class OdhApiCoreTests
    {
        TestController _controller;
        IConfiguration configuration;

        public OdhApiCoreTests()
        {
            configuration = TestsHelper.GetIConfigurationRoot();
            _controller = new TestController(configuration);
        }

        [Fact]
        public void Test1()
        {
            // Act
            var getResult = _controller.Get();

            // Assert
            Assert.Equal("hallo", getResult);
        }
    }
}
