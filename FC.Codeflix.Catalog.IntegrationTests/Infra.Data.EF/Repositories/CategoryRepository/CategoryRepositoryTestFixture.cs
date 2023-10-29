using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.IntegrationTests.Base;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CategoryRepository;

[CollectionDefinition(nameof(CategoryRepositoryTestFixture))]
public class CategoryRepositoryTestFixtureCollection
    : ICollectionFixture<CategoryRepositoryTestFixture>
{ }

public class CategoryRepositoryTestFixture : BaseFixture
{
    public Category GetExampleCategory()
    => new(
        GetValidCategoryName(),
        GetValidCategoryDescription(),
        GetRandomBoolean()
    );

    public List<Category> GetExampleCategoriesList(int length = 10)
    => Enumerable.Range(1, length)
        .Select(_ => GetExampleCategory()).ToList();

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

    public CodeflixCatalogDbContext CreateDbContext()
        => new(
            new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
            .UseInMemoryDatabase("integration-tests-db")
            .Options
        );
}
