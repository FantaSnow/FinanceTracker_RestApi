namespace Domain.Users;

public class User
{
    public UserId Id { get; private set; }
    public string Login { get; private set; }
    public string Password { get; private set; }
    public decimal Balance { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsAdmin { get; private set; }

    private User(UserId id, string login, string password)
    {
        Id = id;
        Login = login;
        Password = password;
        Balance = 0m;
        CreatedAt = DateTime.UtcNow;
        IsAdmin = login == "admin"; 
    }

    public static User New(UserId id, string login, string password)
        => new User(id, login, password);

    public void AddToBalance(decimal balance)
    {
        Balance += balance;
    }

    public void SetBalance(decimal balance)
    {
        Balance = balance;
    }

    public void ChangeLogin(string login)
    {
        Login = login;
        IsAdmin = login == "admin"; 
    }

    public void ChangePassword(string password)
    {
        Password = password;
    }
}