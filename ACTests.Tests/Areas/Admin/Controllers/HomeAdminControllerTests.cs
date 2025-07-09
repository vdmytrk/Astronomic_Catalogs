using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Infrastructure.Interfaces;
using Astronomic_Catalogs.Models.Connection;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Astronomic_Catalogs.Areas.Admin.Controllers.Tests;

[TestFixture]
public class HomeAdminControllerTests
{
    private ApplicationDbContext _context;
    private HomeAdminController _controller;
    private ILogger<HomeAdminController> _logger;
    private IConnectionStringProvider _connectionStringProvider;
    private DateTime date_1 = new DateTime(2025, 02, 01);
    private DateTime date_2 = new DateTime(2025, 01, 01);
    private List<ActualDate> testData = new ();

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new ApplicationDbContext(options);
        _logger = A.Fake<ILogger<HomeAdminController>>();
        _connectionStringProvider = A.Fake<IConnectionStringProvider>();
        A.CallTo(() => _connectionStringProvider.ConnectionString).Returns("FakeConnectionString");
        _controller = new HomeAdminController(_context, _logger, _connectionStringProvider);
        testData =
            [
                new ActualDate { Id = 1, ActualDateProperty = date_1 },
                new ActualDate { Id = 2, ActualDateProperty = date_2 }
            ];

    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _controller.Dispose();
    }

    [Test()]
    public async Task Index_ReturnsViewWithActualDates()
    {
        // Arrange
        await _context.ActualDates.AddRangeAsync(testData);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Index() as ViewResult;
        var model = result?.Model as List<ActualDate>;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(model, Is.Not.Null);

        Assert.That(model.Count, Is.EqualTo(2));
        Assert.That(result!.Model, Is.AssignableTo<List<ActualDate>>());
        Assert.That(model!.Select(d => d.ActualDateProperty), Is.EquivalentTo([date_1, date_2]));
    }

    [Test]
    public async Task DeleteConfirmed_RemovesActualDateAndRedirects()
    {
        // Arrange
        await _context.ActualDates.AddRangeAsync(testData);
        await _context.SaveChangesAsync();

        var initialCount = await _context.ActualDates.CountAsync();

        // Act
        var result = await _controller.DeleteConfirmed(1) as RedirectToActionResult;
        var exists = await _context.ActualDates.FindAsync(1);
        var remainingCount = await _context.ActualDates.CountAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ActionName, Is.EqualTo("Index"));
        Assert.That(exists, Is.Null, "The deleted entity should not exist in the database.");
        Assert.That(remainingCount, Is.EqualTo(initialCount - 1), "The count of records should decrease by 1.");

    }

    [Test]
    public async Task Delete_ReturnsView_WhenActualDateExists()
    {
        // Arrange
        await _context.ActualDates.AddRangeAsync(testData);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Delete(1) as ViewResult;
        var model = result?.Model as ActualDate;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(model, Is.Not.Null);
        Assert.That(model.Id, Is.EqualTo(1));
    }

    [Test]
    public async Task Delete_ReturnsNotFound_WhenIdIsNull()
    {
        // Act
        var result = await _controller.Delete(null);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task Delete_ReturnsNotFound_WhenActualDateDoesNotExist()
    {
        // Act
        var result = await _controller.Delete(999);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

}



