using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using BirthdayGreetingsKata2.Application;
using BirthdayGreetingsKata2.Core;
using BirthdayGreetingsKata2.Infrastructure.Repositories;
using NUnit.Framework;
using static BirthdayGreetingsKata2.Tests.helpers.OurDateFactory;

namespace BirthdayGreetingsKata2.Tests.Application;

public class AcceptanceTest
{
    private const int SmtpPort = 25;
    private const string SmtpHost = "localhost";
    private const string From = "sender@here.com";
    private List<MailMessage> _messagesSent;
    private BirthdayService _service;
    private const string EmployeesFilePath = "Application/employee_data.txt";

    private class BirthdayServiceForTesting : BirthdayService
    {
        private readonly List<MailMessage> _messages;

        public BirthdayServiceForTesting(List<MailMessage> messages, IEmployeesRepository employeesRepository) : base(
            employeesRepository)
        {
            _messages = messages;
        }

        protected override void SendMessage(MailMessage msg, SmtpClient smtpClient)
        {
            _messages.Add(msg);
        }
    }

    [SetUp]
    public void SetUp()
    {
        _messagesSent = new List<MailMessage>();
        _service = new BirthdayServiceForTesting(_messagesSent,
            new FileEmployeesRepository(GetAbsolutePath(EmployeesFilePath)));
    }

    [Test]
    public void Base_Scenario()
    {
        var today = OurDate("2008/10/08");

        _service.SendGreetings(today, SmtpHost, SmtpPort, From);

        Assert.That(_messagesSent, Has.Exactly(1).Items);
        var message = _messagesSent[0];
        Assert.That(message.Body, Is.EqualTo("Happy Birthday, dear John!"));
        Assert.That(message.Subject, Is.EqualTo("Happy Birthday!"));
        Assert.That(message.To, Has.Exactly(1).Items);
        Assert.That(message.To[0].Address, Is.EqualTo("john.doe@foobar.com"));
    }

    [Test]
    public void Will_Not_Send_Emails_When_Nobodies_Birthday()
    {
        var today = OurDate("2008/01/01");

        _service.SendGreetings(today, SmtpHost, SmtpPort, From);

        Assert.That(_messagesSent, Is.Empty);
    }

    private static string GetAbsolutePath(string filePath)
    {
        var dir = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        while (dir != null && !dir.GetFiles("*.csproj").Any())
        {
            dir = dir.Parent;
        }

        if (dir == null)
        {
            throw new InvalidOperationException("project not found.");
        }

        return Path.Combine(new List<string> { dir.FullName }
            .Concat(filePath.Split("/").ToList()).ToArray());
    }
}