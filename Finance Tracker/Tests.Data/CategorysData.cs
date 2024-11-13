using Domain.Categorys;
using Domain.Transactions;
using Domain.Users;

namespace Tests.Data;

public static class CategorysData
{
    public static Category Category1()
        => Category.New(CategoryId.New(),"name1");
    public static Category Category2()
        => Category.New(CategoryId.New(),"name2");

}