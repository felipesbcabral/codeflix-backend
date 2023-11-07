using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.UnitTests.Common;
using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.Common;
public class GenreUseCasesBaseFixture : BaseFixture
{
    public string GetValidGenreName()
        => Faker.Commerce.Categories(1)[0];

    //public string GetInvalidGenreName()
    //    => string.Empty;

    public Mock<IGenreRepository> GetGenreRepositoryMock()
        => new();
    public Mock<IUnitOfWork> GetUnitOfWorkMock()
        => new();
}

