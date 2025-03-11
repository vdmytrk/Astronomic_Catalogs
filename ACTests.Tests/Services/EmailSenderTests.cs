using NUnit.Framework;
using Astronomic_Catalogs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models.Services;
using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Astronomic_Catalogs.Models;
using System.Net.Mail;

namespace Astronomic_Catalogs.Services.Tests;

[TestFixture]
public class EmailSenderTests
{
    private EmailSender _emailSender;
    private IOptions<AuthMessageSenderOptions> _options;
    private ApplicationDbContext _context;
    private ILogger<EmailSender> _logger;
    private string _email = string.Empty;
    private string _subject = string.Empty;
    private string _message = string.Empty;

    [SetUp]
    public void Setup()
    {
        _options = Options.Create(new AuthMessageSenderOptions
        {
            Email = "test@example.com",
            Password = "password123"
        });

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        _context = new ApplicationDbContext(options);
        _logger = A.Fake<ILogger<EmailSender>>();
        _emailSender = CreateEmailSender();
        
        _email = "recipient@example.com";
        _subject = "Test Subject";
        _message = "<p>Test Email</p>";

    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    private EmailSender CreateEmailSender()
    {
        var emailSender = A.Fake<EmailSender>(x =>
            x.WithArgumentsForConstructor(() => new EmailSender(_options, _context, _logger))
             .CallsBaseMethods()); 

        A.CallTo(() => emailSender.SendEmailInternalAsync(A<MailMessage>._)).Returns(Task.CompletedTask);
        return emailSender;
    }

    [Test]
    public void SendEmailAsync_ShouldSendEmailWithoutUserId()
    {
        // Act & Assert
        Assert.DoesNotThrowAsync(async () => await _emailSender.SendEmailAsync(_email, _subject, _message));
    }

    [Test]
    public async Task SendEmailAsync_ShouldSendEmailAndUpdateUser_WhenUserExists()
    {
        // Arrange
        var user = new AspNetUser { Id = "emailTestUser", Email = _email, CountRegisterEmailSent = 1, LastRegisterEmailSent = DateTime.Now };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        await _emailSender.SendEmailAsync(_email, _subject, _message, "emailTestUser");

        // Assert
        var updatedUser = await _context.Users.FindAsync("emailTestUser");
        Assert.That(updatedUser, Is.Not.Null);
        Assert.That(updatedUser!.LastRegisterEmailSent, Is.Not.Null);
        Assert.That(updatedUser.CountRegisterEmailSent, Is.EqualTo(2));
    }

    [Test]
    public async Task SendEmailAsync_ShouldCallSendEmailInternalAsync()
    {
        // Act
        await _emailSender.SendEmailAsync(_email, _subject, _message);

        // Assert
        A.CallTo(() => _emailSender.SendEmailInternalAsync(A<MailMessage>._)).MustHaveHappenedOnceExactly();
    }
}