

using Domain.Users;

namespace Tests.Data;

public static class UsersData
{
    public static User MainUser()
        => User.New(UserId.New(), "login1", "password1");
    public static User AdminUser()
        => User.New(UserId.New(), "admin", "password2");
    public static User AnotherUser()
        => User.New(UserId.New(), "login2", "password2");
}