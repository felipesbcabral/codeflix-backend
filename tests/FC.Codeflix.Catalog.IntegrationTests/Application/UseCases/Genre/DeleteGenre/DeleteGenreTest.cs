using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.DeleteGenre;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.DeleteGenre;

[Collection(nameof(DeleteGenreTestFixture))]
public class DeleteGenreTest
{
    private readonly DeleteGenreTestFixture _fixture;

    public DeleteGenreTest(DeleteGenreTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(DeleteGenre))]
    [Trait("Integration/Application", "DeleteGenre - Use Cases")]
    public async Task DeleteGenre()
    {
        var genresExampleList = _fixture.GetExampleListGenres(10);
        var targetGenre = genresExampleList[5];
        var dbArrangeContext = _fixture.CreateDbContext();
        await dbArrangeContext.Genres.AddRangeAsync(genresExampleList);
        await dbArrangeContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);

        var useCase = new UseCase.DeleteGenre(
            new GenreRepository(actDbContext),
            new UnitOfWork(actDbContext)
        );

        var input = new UseCase.DeleteGenreInput(targetGenre.Id);

        await useCase.Handle(input, CancellationToken.None);

        var assertDbContext = _fixture.CreateDbContext(true);
        var genreFromDb = assertDbContext.Genres.Find(targetGenre.Id);
        genreFromDb.Should().BeNull();
    }

    [Fact(DisplayName = nameof(DeleteGenre_Throws_When_NotFound))]
    [Trait("Integration/Application", "DeleteGenre - Use Cases")]
    public async Task DeleteGenre_Throws_When_NotFound()
    {
        var genresExampleList = _fixture.GetExampleListGenres(10);
        var dbArrangeContext = _fixture.CreateDbContext();
        await dbArrangeContext.Genres.AddRangeAsync(genresExampleList);
        await dbArrangeContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);

        var useCase = new UseCase.DeleteGenre(
            new GenreRepository(actDbContext),
            new UnitOfWork(actDbContext)
        );
        var randomGuid = Guid.NewGuid();
        var input = new UseCase.DeleteGenreInput(randomGuid);

        Func<Task> action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Genre '{randomGuid}' not found.");
    }

    [Fact(DisplayName = nameof(DeleteGenre_With_Relations))]
    [Trait("Integration/Application", "DeleteGenre - Use Cases")]
    public async Task DeleteGenre_With_Relations()
    {
        var genresExampleList = _fixture.GetExampleListGenres(10);
        var targetGenre = genresExampleList[5];
        var exampleCategories = _fixture.GetExampleCategoriesList(5);
        var dbArrangeContext = _fixture.CreateDbContext();
        await dbArrangeContext.Genres.AddRangeAsync(genresExampleList);
        await dbArrangeContext.Categories.AddRangeAsync(exampleCategories);
        await dbArrangeContext.GenresCategories.AddRangeAsync(
            exampleCategories.Select(category => new GenresCategories(
                category.Id,
                targetGenre.Id
            ))
        );
        await dbArrangeContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);

        var useCase = new UseCase.DeleteGenre(
            new GenreRepository(actDbContext),
            new UnitOfWork(actDbContext)
        );

        var input = new UseCase.DeleteGenreInput(targetGenre.Id);

        await useCase.Handle(input, CancellationToken.None);

        var assertDbContext = _fixture.CreateDbContext(true);
        var genreFromDb = assertDbContext.Genres.Find(targetGenre.Id);
        genreFromDb.Should().BeNull();
        var relations = await assertDbContext.GenresCategories.AsNoTracking()
            .Where(genreCategory => genreCategory.GenreId == targetGenre.Id)
            .ToListAsync();
        relations.Should().BeEmpty();
        relations.Should().HaveCount(0);
    }
}
