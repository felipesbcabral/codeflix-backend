using FC.Codeflix.Catalog.UnitTests.Application.Genre.Common;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.DeleteGenre;

[CollectionDefinition(nameof(DeleteGenreTestFixture))]
public class DeleteTestFixtureCollection : ICollectionFixture<DeleteGenreTestFixture> { }

public class DeleteGenreTestFixture : GenreUseCasesBaseFixture
{

}