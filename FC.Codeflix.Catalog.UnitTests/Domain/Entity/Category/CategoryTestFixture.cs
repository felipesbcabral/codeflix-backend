namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Category;

using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

public class CategoryTestFixture
{
    public DomainEntity.Category GetValidCategory()
        => new("Category name", "Category Description");
}

[CollectionDefinition(nameof(CategoryTestFixture))]
public class CategoryTestFixtureCollection 
    : ICollectionFixture<CategoryTestFixture>
{ }
