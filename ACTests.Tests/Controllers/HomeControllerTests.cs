using Astronomic_Catalogs.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Astronomic_Catalogs.Controllers.Tests;

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

    [Test]
    public void Error_Returns_ViewResult_With_ErrorViewModel()
    {
        // Act
        var result = _controller.Error() as ViewResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Model, Is.InstanceOf<ErrorViewModel>());
        Assert.That(((ErrorViewModel)result.Model).RequestId, Is.EqualTo("test-trace-id"));
    }
}
