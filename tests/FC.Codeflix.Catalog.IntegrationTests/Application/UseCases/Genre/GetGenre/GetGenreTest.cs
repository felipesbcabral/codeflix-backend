using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.GetGenre;


namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.GetGenre;

[Collection(nameof(GetGenreTestFixture))]
public class GetGenreTest
{
    private readonly GetGenreTestFixture _fixture;

    public GetGenreTest(GetGenreTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(GetGenre))]
    [Trait("Integration/Application", "GetGenre - Use Cases")]
    public async Task GetGenre()
    {
        // Arrange
        var genresExampleList = _fixture.GetExampleListGenres(10);
        var expectedGenre = genresExampleList[5];
        var dbArrangeContext = _fixture.CreateDbContext();
        await dbArrangeContext.Genres.AddRangeAsync(genresExampleList);
        await dbArrangeContext.SaveChangesAsync();
        var genreRepository = new GenreRepository(_fixture.CreateDbContext(true));

        var useCase = new UseCase.GetGenre(genreRepository);

        var input = new UseCase.GetGenreInput(expectedGenre.Id);

        var output = await useCase.Handle(input, CancellationToken.None);

        // Assert
        output.Should().NotBeNull();
        output.Id.Should().Be(expectedGenre.Id);
        output.Name.Should().Be(expectedGenre.Name);
        output.IsActive.Should().Be(expectedGenre.IsActive);
        output.CreatedAt.Should().Be(expectedGenre.CreatedAt);
        output.CreatedAt.Should().NotBe(default);
    }

    [Fact(DisplayName = nameof(GetGenre_Throw_When_Not_Found))]
    [Trait("Integration/Application", "GetGenre - Use Cases")]
    public async Task GetGenre_Throw_When_Not_Found()
    {
        // Arrange
        var genresExampleList = _fixture.GetExampleListGenres(10);
        var randomGuid = Guid.NewGuid();
        var dbArrangeContext = _fixture.CreateDbContext();
        await dbArrangeContext.Genres.AddRangeAsync(genresExampleList);
        await dbArrangeContext.SaveChangesAsync();
        var genreRepository = new GenreRepository(_fixture.CreateDbContext(true));
        var useCase = new UseCase.GetGenre(genreRepository);
        var input = new UseCase.GetGenreInput(randomGuid);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Genre '{randomGuid}' not found.");
    }

    [Fact(DisplayName = nameof(GetGenre_With_Category_Relations))]
    [Trait("Integration/Application", "GetGenre - Use Cases")]
    public async Task GetGenre_With_Category_Relations()
    {
        // Arrange
        var genresExampleList = _fixture.GetExampleListGenres(10);
        var categoriesEXampleList = _fixture.GetExampleCategoriesList(5);
        var expectedGenre = genresExampleList[5];
        categoriesEXampleList.ForEach(
            category => expectedGenre.AddCategory(category.Id)
        );
        var dbArrangeContext = _fixture.CreateDbContext();
        await dbArrangeContext.Categories.AddRangeAsync(categoriesEXampleList);
        await dbArrangeContext.Genres.AddRangeAsync(genresExampleList);
        await dbArrangeContext.GenresCategories.AddRangeAsync(
            expectedGenre.Categories.Select(
                categoryId => new GenresCategories(
                    categoryId,
                    expectedGenre.Id
                )
            )
        );
        await dbArrangeContext.SaveChangesAsync();
        var genreRepository = new GenreRepository(_fixture.CreateDbContext(true));
        var useCase = new UseCase.GetGenre(genreRepository);
        var input = new UseCase.GetGenreInput(expectedGenre.Id);

        var output = await useCase.Handle(input, CancellationToken.None);

        // Assert
        output.Should().NotBeNull();
        output.Id.Should().Be(expectedGenre.Id);
        output.Name.Should().Be(expectedGenre.Name);
        output.IsActive.Should().Be(expectedGenre.IsActive);
        output.CreatedAt.Should().Be(expectedGenre.CreatedAt);
        output.CreatedAt.Should().NotBe(default);
        output.Categories.Should().NotBeEmpty();
        output.Categories.Should().HaveCount(expectedGenre.Categories.Count);
        output.Categories.ToList().ForEach(
            relationModel =>
            {
                expectedGenre.Categories.Should().Contain(relationModel.Id);
                relationModel.Name.Should().BeNull();
            }
        );
    }
}
