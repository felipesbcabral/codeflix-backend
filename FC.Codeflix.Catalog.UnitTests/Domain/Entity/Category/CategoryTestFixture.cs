namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Category;

using Bogus.DataSets;
using FC.Codeflix.Catalog.UnitTests.Common;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

public class CategoryTestFixture : BaseFixture
{
    public CategoryTestFixture() : base() { }

    public string GetValidCategoryName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];
        if(categoryName.Length > 255)
            categoryName = categoryName[..255];
        return categoryName;
    }

    public string GetValidCategoryDescription()
    {
        var categoryDescription = 
            Faker.Commerce.ProductDescription();
        if(categoryDescription.Length > 10000)
            categoryDescription =
                categoryDescription[..10000];
        return categoryDescription;
    }

    public DomainEntity.Category GetValidCategory()
        => new(
            GetValidCategoryName(),
            GetValidCategoryDescription()
        );
}

[CollectionDefinition(nameof(CategoryTestFixture))]
public class CategoryTestFixtureCollection 
    : ICollectionFixture<CategoryTestFixture>
{ }
