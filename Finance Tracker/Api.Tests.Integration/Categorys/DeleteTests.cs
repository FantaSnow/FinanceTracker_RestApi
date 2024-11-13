using System.Net.Http.Headers;
using Domain.Categorys;
using Domain.Transactions;
using Domain.Users;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Categorys;

public class DeleteTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Category _category1;
    private readonly Category _category2;

    private readonly User _mainUser;
    private readonly User _secondUser;
    private readonly User _adminUser;
    
    public DeleteTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser();
        _secondUser = UsersData.AnotherUser();
        _adminUser = UsersData.AdminUser();
        
        _category1 = CategorysData.Category1();
        _category2 = CategorysData.Category2();
    }
    [Fact]
    public async Task DeleteCategory_Success_WhenAdminDeletesCategory()
    {
        var authToken = await GenerateAuthTokenAsync(_adminUser.Login, _adminUser.Password);
        
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await Client.DeleteAsync($"categorys/delete/{_category1.Id}");

        response.EnsureSuccessStatusCode();

        var deletedCategory = await Context.Categorys.FindAsync(_category1.Id);
        Assert.Null(deletedCategory);
    }

    [Fact]
    public async Task DeleteCategory_Fails_WhenUserTriesToDeleteCategory()
    {
        var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
        
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await Client.DeleteAsync($"categorys/delete/{_category1.Id}");

        Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCategory_Fails_WhenUserNotAuthorized()
    {
        var response = await Client.DeleteAsync($"categorys/delete/{_category1.Id}");

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
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