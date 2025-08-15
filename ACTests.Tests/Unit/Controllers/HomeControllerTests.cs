using Astronomic_Catalogs.Controllers;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ACTests.Tests.Unit.Controllers;

[TestFixture]
public class HomeControllerTests
{
    private HomeController _controller;

    [SetUp]
    public void Setup()
    {
        _controller = new HomeController();

        var httpContext = A.Fake<HttpContext>();
        var traceIdentifier = "test-trace-id";

        A.CallTo(() => httpContext.TraceIdentifier).Returns(traceIdentifier);

        var tempDataProvider = A.Fake<ITempDataProvider>();
        var tempDataFactory = A.Fake<ITempDataDictionaryFactory>();
        var tempData = new TempDataDictionary(httpContext, tempDataProvider);

        A.CallTo(() => tempDataFactory.GetTempData(httpContext)).Returns(tempData);

        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(typeof(ITempDataDictionaryFactory))).Returns(tempDataFactory);
        A.CallTo(() => httpContext.RequestServices).Returns(serviceProvider);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
        _controller.TempData = tempData;
    }

    [TearDown]
    public void TearDown()
    {
        _controller?.Dispose();
    }

    [Test]
    public void Index_Returns_ViewResult()
    {
        // Act
        var result = _controller.Index();

        // Assert
        Assert.That(result, Is.InstanceOf<ViewResult>());
    }

    [Test]
    public void Privacy_Returns_ViewResult()
    {
        // Act
        var result = _controller.Privacy();

        // Assert
        Assert.That(result, Is.TypeOf<ViewResult>());
    }

}
