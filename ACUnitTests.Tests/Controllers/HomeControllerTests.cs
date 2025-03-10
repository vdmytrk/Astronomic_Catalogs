using NUnit.Framework;
using Astronomic_Catalogs.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Astronomic_Catalogs.Data;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Astronomic_Catalogs.Controllers.Tests;

[TestFixture()]
public class HomeControllerTests
{
    private HomeController _controller;
    private ApplicationDbContext _context;
    private ILogger<HomeController> _logger;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
        .Options;

        _context = new ApplicationDbContext(options);
        _logger = A.Fake<ILogger<HomeController>>();

        _controller = new HomeController(_context, _logger)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [TearDown]
    public void TearDown()
    {
        _controller?.Dispose();
        _context.Dispose();
    }

    [Test]
    public async Task Index_ReturnsViewResult()
    {
        // Arrange

        // Act
        var result = await _controller.Index();

        // Assert
        Assert.That(result, Is.InstanceOf<ViewResult>());
    }
}