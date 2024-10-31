namespace Domain.Categorys;

public class Category
{
    public CategoryId Id { get; }
    public string Name { get; private set; }

    private Category(CategoryId id, string name)
    {
        Id = id;
        Name = name;
    }

    public static Category New(CategoryId id, string name)
        => new(id, name);

    public void UpdateName(string name)
    {
        Name = name;
    }
}