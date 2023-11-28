using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.CreateGenre;

[Collection(nameof(CreateGenreTestFixture))]
public class CreateGenreTest
{
    private readonly CreateGenreTestFixture _fixture;

    public CreateGenreTest(CreateGenreTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(CreateGenre))]
    [Trait("Integration/Application", "CreateGenre - Use Cases")]
    public async Task CreateGenre()
    {
        CreateGenreInput input = _fixture.GetExampleInput();
        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext();
        UseCase.CreateGenre createGenre = new(
            new GenreRepository(actDbContext),
            new UnitOfWork(actDbContext),
            new CategoryRepository(actDbContext)
        );

        GenreModelOutput output = await createGenre.Handle(
            input,
            CancellationToken.None
        );

        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBe(default);
        output.Categories.Should().BeEmpty();
        output.Categories.Should().HaveCount(0);
        CodeflixCatalogDbContext assertDbContext = _fixture.CreateDbContext(true);
        DomainEntity.Genre? genreFromDb = await assertDbContext.Genres
            .FindAsync(output.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Id.Should().Be(output.Id);
        genreFromDb.Name.Should().Be(output.Name);
        genreFromDb.IsActive.Should().Be(output.IsActive);
    }

    [Fact(DisplayName = nameof(CreateGenre_With_Categories_Relations))]
    [Trait("Integration/Application", "CreateGenre - Use Cases")]
    public async Task CreateGenre_With_Categories_Relations()
    {
        List<DomainEntity.Category> exampleCategories =
            _fixture.GetExampleCategoriesList(5);
        CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
        await arrangeDbContext.SaveChangesAsync();
        CreateGenreInput input = _fixture.GetExampleInput();
        input.CategoriesIds = exampleCategories
            .Select(category => category.Id).ToList();
        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
        UseCase.CreateGenre createGenre = new(
            new GenreRepository(actDbContext),
            new UnitOfWork(actDbContext),
            new CategoryRepository(actDbContext)
        );

        GenreModelOutput output = await createGenre.Handle(
            input,
            CancellationToken.None
        );

        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBe(default);
        output.Categories.Should().HaveCount(input.CategoriesIds.Count);
        List<Guid> relatedCategoriesIdsFromOutput = output.Categories
            .Select(relation => relation.Id).ToList();
        relatedCategoriesIdsFromOutput.Should().BeEquivalentTo(input.CategoriesIds);
        CodeflixCatalogDbContext assertDbContext = _fixture.CreateDbContext(true);
        DomainEntity.Genre? genreFromDb = await assertDbContext.Genres
            .FindAsync(output.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Id.Should().Be(output.Id);
        genreFromDb.Name.Should().Be(output.Name);
        genreFromDb.IsActive.Should().Be(output.IsActive);
        List<GenresCategories>? relations =
            await assertDbContext.GenresCategories.AsNoTracking()
                .Where(genreCategory => genreCategory.GenreId == output.Id)
                .ToListAsync();
        relations.Should().HaveCount(input.CategoriesIds.Count);
        List<Guid> categoryIdsRelatedFromDb = relations.Select(relation => relation.CategoryId).ToList();
        categoryIdsRelatedFromDb.Should().BeEquivalentTo(input.CategoriesIds);
    }

    [Fact(DisplayName = nameof(CreateGenre_Throws_When_Category_Doesnt_Exists))]
    [Trait("Integration/Application", "CreateGenre - Use Cases")]
    public async Task CreateGenre_Throws_When_Category_Doesnt_Exists()
    {
        List<DomainEntity.Category> exampleCategories =
            _fixture.GetExampleCategoriesList(5);
        CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
        await arrangeDbContext.SaveChangesAsync();
        CreateGenreInput input = _fixture.GetExampleInput();
        input.CategoriesIds = exampleCategories
            .Select(category => category.Id).ToList();
        Guid randomGuid = Guid.NewGuid();
        input.CategoriesIds.Add(randomGuid);
        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
        UseCase.CreateGenre createGenre = new(
            new GenreRepository(actDbContext),
            new UnitOfWork(actDbContext),
            new CategoryRepository(actDbContext)
        );

        Func<Task<GenreModelOutput>> action = async () =>
            await createGenre.Handle(input, CancellationToken.None);


        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related category id (or ids) not found: {randomGuid}");
    }
}
