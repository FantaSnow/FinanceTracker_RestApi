using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Dtos.Categorys;
using Api.Dtos.Users;
using Domain.Categorys;
using Domain.Transactions;
using Domain.Users;
using FluentAssertions;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Categorys;

public class UpdateTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Category _category1;
    private readonly Category _category2;

    private readonly User _mainUser;
    private readonly User _secondUser;
    private readonly User _adminUser;
    
    public UpdateTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser();
        _secondUser = UsersData.AnotherUser();
        _adminUser = UsersData.AdminUser();
        
        _category1 = CategorysData.Category1();
        _category2 = CategorysData.Category2();
    }
    [Fact]
    public async Task UpdateCategory_Success_WhenAdminUpdatesCategory()
    {
        var authToken = await GenerateAuthTokenAsync(_adminUser.Login, _adminUser.Password);
        var request = new CategoryUpdateDto(
            Name: "Updated Category Name"
        );

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await Client.PutAsJsonAsync($"categorys/update/{_category1.Id}", request);

        response.EnsureSuccessStatusCode();

        var updatedCategory = await Context.Categorys.FindAsync(_category1.Id);
        updatedCategory.Should().NotBeNull();
        updatedCategory.Name.Should().Be("Updated Category Name");
    }

    [Fact]
    public async Task UpdateCategory_Fails_WhenUserTriesToUpdateCategory()
    {
        var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
        var request = new CategoryUpdateDto(
            Name: "User Updated Category Name"
        );

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await Client.PutAsJsonAsync($"categorys/update/{_category1.Id}", request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCategory_Fails_WhenUserNotAuthorized()
    {
        var request = new CategoryUpdateDto(
            Name: "Updated Category Name"
        );

        var response = await Client.PutAsJsonAsync($"categorys/update/{_category1.Id}", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
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