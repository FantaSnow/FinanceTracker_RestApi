using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Dtos.Users;
using Domain.Users;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Users
{
    public class UpdateTests : BaseIntegrationTest, IAsyncLifetime
    {
        private readonly User _mainUser;
        private readonly User _secondUser;
        private readonly User _adminUser;

        public UpdateTests(IntegrationTestWebFactory factory) : base(factory)
        {
            _mainUser = UsersData.MainUser();
            _secondUser = UsersData.AnotherUser();
            _adminUser = UsersData.AdminUser();
        }

        [Fact]
        public async Task UpdateUser_Fails_WhenUserIsNotAuthorized()
        {
            // Arrange
            var userId = _mainUser.Id;
            var updateUserDto = new UserUpdateDto
            (
                Login: "updatedLogin",
                Password: "newpassword123",
                Balance: 10m
            );

            // Act
            var response = await Client.PutAsJsonAsync($"users/update/{userId}", updateUserDto);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUser_Fails_WhenUserIsNotAdmin()
        {
            // Arrange
            var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
            var userId = _secondUser.Id;
            var updateUserDto = new UserUpdateDto
            (
                Login: "updatedLogin",
                Password: "newpassword123",
                Balance: 10m
            );

            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            // Act
            var response = await Client.PutAsJsonAsync($"users/update/{userId}", updateUserDto);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUser_Success_WhenAdminUpdatesAnotherUser()
        {
            // Arrange
            var authToken = await GenerateAuthTokenAsync(_adminUser.Login, _adminUser.Password);
            var userId = _secondUser.Id;
            var updateUserDto = new UserUpdateDto
            (
                Login: "updatedLogin",
                Password: "newpassword123",
                Balance: 10m
            );

            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            // Act
            var response = await Client.PutAsJsonAsync($"users/update/{userId}", updateUserDto);

            // Assert
            response.EnsureSuccessStatusCode();
            var updatedUser = await Context.Users.FindAsync(userId);
            Assert.NotNull(updatedUser);
            Assert.Equal(updateUserDto.Login, updatedUser?.Login);
            Assert.Equal(updateUserDto.Password, updatedUser?.Password);
            Assert.Equal(updateUserDto.Balance, updatedUser?.Balance);
        }

        [Fact]
        public async Task UpdateUser_Fails_WhenUserNotFound()
        {
            // Arrange
            var authToken = await GenerateAuthTokenAsync(_adminUser.Login, _adminUser.Password);
            var nonExistentUserId = Guid.NewGuid();
            var updateUserDto = new UserUpdateDto
            (
                Login: "updatedLogin",
                Password: "newpassword123",
                Balance: 10m
            );

            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            // Act
            var response = await Client.PutAsJsonAsync($"users/update/{nonExistentUserId}", updateUserDto);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        public async Task InitializeAsync()
        {
            await Context.Users.AddRangeAsync(_mainUser, _secondUser, _adminUser);
            await SaveChangesAsync();
        }

        public async Task DisposeAsync()
        {
            Context.Users.RemoveRange(Context.Users);
            await SaveChangesAsync();
        }
    }
}