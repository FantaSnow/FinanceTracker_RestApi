﻿using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Dtos.Categorys;
using Domain.Banks;
using Domain.Categorys;
using Domain.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Banks;

public class GetAllByUserTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Bank _bank1;
    private readonly Bank _bank2;

    private readonly User _mainUser;
    private readonly User _secondUser;
    private readonly User _adminUser;
    
    public GetAllByUserTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser();
        _secondUser = UsersData.AnotherUser();
        _adminUser = UsersData.AdminUser();
        
        _bank1 = BanksData.Bank1(_mainUser.Id);
        _bank2 = BanksData.Bank2(_secondUser.Id);
    }
    [Fact]
    public async Task GetAllByUser_Success_WhenBanksExist()
    {
        // Arrange
        var userId = _mainUser.Id;

        // Act
        var response = await Client.GetAsync($"banks/getAllByUser/{userId}");

        // Assert
        response.EnsureSuccessStatusCode();

        var banksInDb = await Context.Banks
            .Where(b => b.UserId == userId)
            .ToListAsync();

        banksInDb.Should().NotBeNull();
        banksInDb.Should().Contain(b => b.Id == _bank1.Id);
        banksInDb.Should().NotContain(b => b.Id == _bank2.Id);
    }
    public async Task InitializeAsync()
    {
        await Context.Users.AddRangeAsync(_mainUser, _adminUser, _secondUser);
        await Context.Banks.AddRangeAsync(_bank1, _bank2);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Users.RemoveRange(Context.Users);
        Context.Banks.RemoveRange(Context.Banks);
        await SaveChangesAsync();
    }
}