using Domain.Categorys;
using Domain.Transactions;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Categorys;

public class GetByIdTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Category _category1;
    private readonly Category _category2;

    private readonly User _mainUser;
    private readonly User _secondUser;
    private readonly User _adminUser;
    
    public GetByIdTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser();
        _secondUser = UsersData.AnotherUser();
        _adminUser = UsersData.AdminUser();
        
        _category1 = CategorysData.Category1();
        _category2 = CategorysData.Category2();
    }
    [Fact]
    public async Task GetCategoryById_Success_WhenRequestsCategoryById()
    {
        var response = await Client.GetAsync($"categorys/getbyid/{_category1.Id}");

        response.EnsureSuccessStatusCode();
        
        var categoryInDb = await Context.Categorys
            .FirstOrDefaultAsync(c => c.Id == _category1.Id);

        Assert.NotNull(categoryInDb);
        Assert.Equal(_category1.Id, categoryInDb.Id);
        Assert.Equal(_category1.Name, categoryInDb.Name);
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