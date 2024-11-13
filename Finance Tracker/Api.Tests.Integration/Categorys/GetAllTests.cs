using Domain.Categorys;
using Domain.Transactions;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Categorys;

public class GetAllTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Category _category1;
    private readonly Category _category2;

    private readonly User _mainUser;
    private readonly User _secondUser;
    private readonly User _adminUser;
    
    public GetAllTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser();
        _secondUser = UsersData.AnotherUser();
        _adminUser = UsersData.AdminUser();
        
        _category1 = CategorysData.Category1();
        _category2 = CategorysData.Category2();
    }
    [Fact]
    public async Task GetAllCategories_Success_WhenRequestsAllCategories()
    {
        
        var response = await Client.GetAsync("categorys/getall");

        response.EnsureSuccessStatusCode();

        var categoriesInDb = await Context.Categorys.ToListAsync();

        Assert.NotNull(categoriesInDb);
        Assert.Contains(categoriesInDb, c => c.Id == _category1.Id);
        Assert.Contains(categoriesInDb, c => c.Id == _category2.Id);
    }
    public async Task InitializeAsync()
    {
        await Context.Users.AddRangeAsync(_mainUser, _adminUser, _secondUser);
        await Context.Categorys.AddRangeAsync(_category1, _category2);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Users.RemoveRange(Context.Users);
        Context.Categorys.RemoveRange(Context.Categorys);
        await SaveChangesAsync();
    }
}