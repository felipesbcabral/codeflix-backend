using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.IntegrationTests.Base;
using Xunit;

namespace FC.Codeflix.Catalog.IntegrationTests.UnitOfWorkTests;

[CollectionDefinition(nameof(UnitOfWorkTestFixture))]
public class UnitOfWorkTestFixtureCollectionFixture : ICollectionFixture<UnitOfWorkTestFixture> { }

public class UnitOfWorkTestFixture : BaseFixture
{
    public string GetValidCategoryName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];
        if (categoryName.Length > 255)
            categoryName = categoryName[..255];
        return categoryName;
    }

    public string GetValidCategoryDescription()
    {
        var categoryDescription =
            Faker.Commerce.ProductDescription();
        if (categoryDescription.Length > 10000)
            categoryDescription =
                categoryDescription[..10000];
        return categoryDescription;
    }

    public static bool GetRandomBoolean()
        => new Random().NextDouble() < 0.5;

    public Category GetExampleCategory()
        => new(
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean()
        );

    public List<Category> GetExampleCategoriesList(int length = 10)
    => Enumerable.Range(1, length)
        .Select(_ => GetExampleCategory()).ToList();
}
