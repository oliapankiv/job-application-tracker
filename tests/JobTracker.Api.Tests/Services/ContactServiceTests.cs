using JobTracker.Api.DTOs;
using JobTracker.Api.Models;
using JobTracker.Api.Services;
using Microsoft.EntityFrameworkCore;
using static JobTracker.Api.Tests.Helpers.TestDbContextFactory;

namespace JobTracker.Api.Tests.Services;

public class ContactServiceTests
{
    private static readonly CreateContactDto ContactDto =
        new("Jane Doe", "Recruiter", "jane@acme.test", "+380000000", "https://linkedin.com/in/jane");

    [Fact]
    public async Task CreateContactAsync_AddsContact_WhenApplicationOwned()
    {
        using var db = Create();
        var app = NewApplication();
        db.Applications.Add(app);
        await db.SaveChangesAsync();
        var sut = new ContactService(db);

        var result = await sut.CreateContactAsync(UserId, app.Id, ContactDto);

        Assert.NotNull(result);
        Assert.Equal(app.Id, result.ApplicationId);
        Assert.Equal("Jane Doe", result.Name);
        Assert.Single(db.Contacts);
    }

    [Fact]
    public async Task CreateContactAsync_ReturnsNull_WhenApplicationNotOwned()
    {
        using var db = Create();
        var app = NewApplication(userId: OtherUserId);
        db.Applications.Add(app);
        await db.SaveChangesAsync();
        var sut = new ContactService(db);

        var result = await sut.CreateContactAsync(UserId, app.Id, ContactDto);

        Assert.Null(result);
        Assert.Empty(db.Contacts);
    }

    [Fact]
    public async Task GetContactsAsync_ReturnsOnlyContactsForOwnedApplication()
    {
        using var db = Create();
        var mine = NewApplication();
        mine.Contacts.Add(new Contact { Name = "Mine" });
        var theirs = NewApplication(userId: OtherUserId);
        theirs.Contacts.Add(new Contact { Name = "Theirs" });
        db.Applications.AddRange(mine, theirs);
        await db.SaveChangesAsync();
        var sut = new ContactService(db);

        var ownContacts = await sut.GetContactsAsync(UserId, mine.Id);
        var foreignContacts = await sut.GetContactsAsync(UserId, theirs.Id);

        Assert.Equal("Mine", Assert.Single(ownContacts).Name);
        Assert.Empty(foreignContacts);
    }

    [Fact]
    public async Task UpdateContactAsync_UpdatesFields()
    {
        using var db = Create();
        var app = NewApplication();
        var contact = new Contact { Name = "Old" };
        app.Contacts.Add(contact);
        db.Applications.Add(app);
        await db.SaveChangesAsync();
        var sut = new ContactService(db);

        var updated = await sut.UpdateContactAsync(UserId, contact.Id, ContactDto);

        Assert.True(updated);
        var saved = await db.Contacts.SingleAsync();
        Assert.Equal("Jane Doe", saved.Name);
        Assert.Equal("jane@acme.test", saved.Email);
    }

    [Fact]
    public async Task UpdateContactAsync_ReturnsFalse_ForOtherUsersContact()
    {
        using var db = Create();
        var app = NewApplication(userId: OtherUserId);
        var contact = new Contact { Name = "Old" };
        app.Contacts.Add(contact);
        db.Applications.Add(app);
        await db.SaveChangesAsync();
        var sut = new ContactService(db);

        var updated = await sut.UpdateContactAsync(UserId, contact.Id, ContactDto);

        Assert.False(updated);
        Assert.Equal("Old", (await db.Contacts.SingleAsync()).Name);
    }

    [Fact]
    public async Task DeleteContactAsync_RemovesOwnContact()
    {
        using var db = Create();
        var app = NewApplication();
        var contact = new Contact { Name = "Jane" };
        app.Contacts.Add(contact);
        db.Applications.Add(app);
        await db.SaveChangesAsync();
        var sut = new ContactService(db);

        Assert.True(await sut.DeleteContactAsync(UserId, contact.Id));
        Assert.Empty(db.Contacts);
    }

    [Fact]
    public async Task DeleteContactAsync_ReturnsFalse_ForOtherUsersContact()
    {
        using var db = Create();
        var app = NewApplication(userId: OtherUserId);
        var contact = new Contact { Name = "Jane" };
        app.Contacts.Add(contact);
        db.Applications.Add(app);
        await db.SaveChangesAsync();
        var sut = new ContactService(db);

        Assert.False(await sut.DeleteContactAsync(UserId, contact.Id));
        Assert.Single(db.Contacts);
    }
}
