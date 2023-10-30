using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.Common;
using Xunit;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category;

[CollectionDefinition(nameof(GetCategoryTestFixture))]
public class GetCategoryTestFixtureCollectionFixture : ICollectionFixture<GetCategoryTestFixture> { }

public class GetCategoryTestFixture : CategoryUseCasesBaseFixture
{

}