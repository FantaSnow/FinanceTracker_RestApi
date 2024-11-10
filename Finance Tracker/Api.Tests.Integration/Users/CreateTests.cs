using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Dtos.Users;
using Domain.Users;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Users
{
    public class CreateTests : BaseIntegrationTest, IAsyncLifetime
    {
        private readonly User _mainUser;
        private readonly User _adminUser;

        public CreateTests(IntegrationTestWebFactory factory) : base(factory)
        {
            _mainUser = UsersData.MainUser();
            _adminUser = UsersData.AdminUser();
        }
        
        [Fact]
        public async Task CreateUser_Success_WhenValidDataProvided()
        {
            // Arrange
            var authToken = await GenerateAuthTokenAsync(_adminUser.Login, _adminUser.Password);
            var newUserDto = new UserCreateDto
            (
                Login: "newUser",
                Password: "password123"
            );

            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            // Act
            var response = await Client.PostAsJsonAsync("users/create/", newUserDto);

            // Assert
            response.EnsureSuccessStatusCode();
            var createdUser = await response.Content.ReadFromJsonAsync<UserDto>();
            Assert.NotNull(createdUser);
            Assert.Equal(newUserDto.Login, createdUser?.Login);
            Assert.Equal(newUserDto.Password, createdUser?.Password); 
        }

        [Fact]
        public async Task CreateUser_Fails_WhenLoginAlreadyExists()
        {
            // Arrange
            var existingUser = new UserCreateDto
            (
                Login: _mainUser.Login,
                Password: "newpassword"
            );

            // Act
            var response = await Client.PostAsJsonAsync("users/create/", existingUser);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Conflict, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Contains("User already exists", responseBody); 
        }

        [Fact]
        public async Task CreateUser_Fails_WhenPasswordIsTooShort()
        {
            // Arrange
            var authToken = await GenerateAuthTokenAsync(_adminUser.Login, _adminUser.Password);
            var newUser = new UserCreateDto
            (
                Login: "shortpassworduser",
                Password: "sh" 
            );


            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            // Act
            var response = await Client.PostAsJsonAsync("users/create/", newUser);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        [Fact]
        public async Task CreateUser_Fails_WhenLoginIsTooShort()
        {
            // Arrange
            var authToken = await GenerateAuthTokenAsync(_adminUser.Login, _adminUser.Password);
            var newUser = new UserCreateDto
            (
                Login: "sh",
                Password: "password"
            );


            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            // Act
            var response = await Client.PostAsJsonAsync("users/create/", newUser);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        public async Task InitializeAsync()
        {
            await Context.Users.AddRangeAsync(_mainUser, _adminUser);
            await SaveChangesAsync();
        }

        public async Task DisposeAsync()
        {
            Context.Users.RemoveRange(Context.Users);
            await SaveChangesAsync();
        }
    }
}