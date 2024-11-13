﻿using System.Net.Http.Json;
using Api.Dtos.Users;
using Domain.Categorys;
using Domain.Transactions;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Transactions;

public class GetAllByUserTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _mainUser;
    private readonly User _secondUser;
    private readonly User _adminUser;

    private readonly Transaction _transaction1;
    private readonly Transaction _transaction2;

    private readonly Category _category1;
    private readonly Category _category2;


    public GetAllByUserTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser();
        _secondUser = UsersData.AnotherUser();
        _adminUser = UsersData.AdminUser();

        _category1 = CategorysData.Category1();
        _category2 = CategorysData.Category2();

        _transaction1 = TranasctionsData.Transactin1(_mainUser.Id, _category1.Id);
        _transaction2 = TranasctionsData.Transactin2(_secondUser.Id, _category2.Id);
    }
    [Fact]
    public async Task GetAllByUser_Success_WhenTransactionsExist()
    {
        // Arrange
        var userId = _mainUser.Id;

        // Act
        var response = await Client.GetAsync($"transactions/getAllByUser/{userId}");

        // Assert
        response.EnsureSuccessStatusCode();
        
        var transactionsInDb = await Context.Transactions
            .Where(t => t.UserId == userId)
            .ToListAsync();

        Assert.NotNull(transactionsInDb);
        Assert.Contains(transactionsInDb, t => t.Id == _transaction1.Id);
        Assert.DoesNotContain(transactionsInDb, t => t.Id == _transaction2.Id); 
    }
    

    public async Task InitializeAsync()
    {
        await Context.Users.AddRangeAsync(_mainUser, _adminUser, _secondUser);
        await Context.Categorys.AddRangeAsync(_category1, _category2);
        await Context.Transactions.AddRangeAsync(_transaction1, _transaction2);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Users.RemoveRange(Context.Users);
        Context.Categorys.RemoveRange(Context.Categorys);
        Context.Transactions.RemoveRange(Context.Transactions);
        await SaveChangesAsync();
    }
}