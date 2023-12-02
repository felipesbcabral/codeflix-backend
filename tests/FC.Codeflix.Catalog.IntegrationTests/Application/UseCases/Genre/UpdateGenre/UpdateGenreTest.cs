using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;
namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.UpdateGenre;

[Collection(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTest
{
    private readonly UpdateGenreTestFixture _fixture;

    public UpdateGenreTest(UpdateGenreTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(UpdateGenre))]
    [Trait("Integration/Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenre()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
        CodeflixCatalogDbContext arrangeContext = _fixture.CreateDbContext();
        DomainEntity.Genre targetGenre = exampleGenres[5];
        await arrangeContext.AddRangeAsync(exampleGenres);
        await arrangeContext.SaveChangesAsync();
        CodeflixCatalogDbContext actDbContet = _fixture.CreateDbContext(true);
        UseCase.UpdateGenre updateGenre = new(
            new GenreRepository(actDbContet),
            new UnitOfWork(actDbContet),
            new CategoryRepository(actDbContet)
        );

        UpdateGenreInput input = new(
            targetGenre.Id,
            _fixture.GetValidGenreName(),
            !targetGenre.IsActive
        );

        GenreModelOutput output = await updateGenre.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.IsActive!);
        CodeflixCatalogDbContext codeflixCatalogDbContext = _fixture.CreateDbContext(true);
        DomainEntity.Genre? genreFromDb =
            await codeflixCatalogDbContext.Genres.FindAsync(targetGenre.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Id.Should().Be(targetGenre.Id);
        genreFromDb.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be((bool)input.IsActive!);
    }

    [Fact(DisplayName = nameof(UpdateGenre_With_Categories_Relations))]
    [Trait("Integration/Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenre_With_Categories_Relations()
    {
        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoriesList(10);
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
        CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
        DomainEntity.Genre targetGenre = exampleGenres[5];
        List<DomainEntity.Category> relatedCategories = exampleCategories.GetRange(0, 5);
        List<DomainEntity.Category> newRelatedCategories = exampleCategories.GetRange(5, 3);
        relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
        List<GenresCategories> relations = targetGenre.Categories
            .Select(categoryId => new GenresCategories(categoryId, targetGenre.Id))
            .ToList();
        await arrangeDbContext.AddRangeAsync(exampleGenres);
        await arrangeDbContext.AddRangeAsync(exampleCategories);
        await arrangeDbContext.AddRangeAsync(relations);
        await arrangeDbContext.SaveChangesAsync();
        CodeflixCatalogDbContext actDbContet = _fixture.CreateDbContext(true);
        UseCase.UpdateGenre updateGenre = new(
            new GenreRepository(actDbContet),
            new UnitOfWork(actDbContet),
            new CategoryRepository(actDbContet)
        );

        UpdateGenreInput input = new(
            targetGenre.Id,
            _fixture.GetValidGenreName(),
            !targetGenre.IsActive,
            newRelatedCategories.Select(category => category.Id).ToList()
        );

        GenreModelOutput output = await updateGenre.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.IsActive!);
        output.Categories.Should().HaveCount(newRelatedCategories.Count);
        List<Guid> relatedCategoryIdsFromOutput = output.Categories
            .Select(relatedCategory => relatedCategory.Id)
            .ToList();
        relatedCategoryIdsFromOutput.Should().BeEquivalentTo(input.CategoriesIds);
        CodeflixCatalogDbContext codeflixCatalogDbContext = _fixture.CreateDbContext(true);
        DomainEntity.Genre? genreFromDb =
            await codeflixCatalogDbContext.Genres.FindAsync(targetGenre.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Id.Should().Be(targetGenre.Id);
        genreFromDb.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be((bool)input.IsActive!);
        List<Guid> relatedCategoryIdsFromDb = await arrangeDbContext
            .GenresCategories.AsNoTracking()
            .Where(relation => relation.GenreId == input.Id)
            .Select(relation => relation.CategoryId)
            .ToListAsync();
        relatedCategoryIdsFromDb.Should().BeEquivalentTo(input.CategoriesIds);
    }

    [Fact(DisplayName = nameof(UpdateGenre_Throws_When_CategoryDoesnt_Exists))]
    [Trait("Integration/Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenre_Throws_When_CategoryDoesnt_Exists()
    {
        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoriesList(10);
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
        CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
        DomainEntity.Genre targetGenre = exampleGenres[5];
        List<DomainEntity.Category> relatedCategories = exampleCategories.GetRange(0, 5);
        List<DomainEntity.Category> newRelatedCategories = exampleCategories.GetRange(5, 3);
        relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
        List<GenresCategories> relations = targetGenre.Categories
            .Select(categoryId => new GenresCategories(categoryId, targetGenre.Id))
            .ToList();
        await arrangeDbContext.AddRangeAsync(exampleGenres);
        await arrangeDbContext.AddRangeAsync(exampleCategories);
        await arrangeDbContext.AddRangeAsync(relations);
        await arrangeDbContext.SaveChangesAsync();
        CodeflixCatalogDbContext actDbContet = _fixture.CreateDbContext(true);
        UseCase.UpdateGenre updateGenre = new(
            new GenreRepository(actDbContet),
            new UnitOfWork(actDbContet),
            new CategoryRepository(actDbContet)
        );

        List<Guid> categoryIdsToRelate = newRelatedCategories.Select(category => category.Id).ToList();
        Guid invalidCategoryId = Guid.NewGuid();
        categoryIdsToRelate.Add(invalidCategoryId);

        UpdateGenreInput input = new(
            targetGenre.Id,
            _fixture.GetValidGenreName(),
            !targetGenre.IsActive,
            categoryIdsToRelate
        );

        Func<Task<GenreModelOutput>> action = async () => await updateGenre.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related category id (or ids) not found: {invalidCategoryId}");
    }

    [Fact(DisplayName = nameof(UpdateGenre_Throws_When_Not_Found))]
    [Trait("Integration/Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenre_Throws_When_Not_Found()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
        CodeflixCatalogDbContext arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.AddRangeAsync(exampleGenres);
        await arrangeContext.SaveChangesAsync();
        CodeflixCatalogDbContext actDbContet = _fixture.CreateDbContext(true);
        UseCase.UpdateGenre updateGenre = new(
            new GenreRepository(actDbContet),
            new UnitOfWork(actDbContet),
            new CategoryRepository(actDbContet)
        );

        Guid randomGuid = Guid.NewGuid();

        UpdateGenreInput input = new(
            randomGuid,
            _fixture.GetValidGenreName(),
            true
        );

        Func<Task<GenreModelOutput>> action = async () => await updateGenre.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Genre '{randomGuid}' not found.");
    }

    [Fact(DisplayName = nameof(UpdateGenre_Without_New_Categories_Relations))]
    [Trait("Integration/Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenre_Without_New_Categories_Relations()
    {
        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoriesList(10);
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
        CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
        DomainEntity.Genre targetGenre = exampleGenres[5];
        List<DomainEntity.Category> relatedCategories = exampleCategories.GetRange(0, 5);
        relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
        List<GenresCategories> relations = targetGenre.Categories
            .Select(categoryId => new GenresCategories(categoryId, targetGenre.Id))
            .ToList();
        await arrangeDbContext.AddRangeAsync(exampleGenres);
        await arrangeDbContext.AddRangeAsync(exampleCategories);
        await arrangeDbContext.AddRangeAsync(relations);
        await arrangeDbContext.SaveChangesAsync();
        CodeflixCatalogDbContext actDbContet = _fixture.CreateDbContext(true);
        UseCase.UpdateGenre updateGenre = new(
            new GenreRepository(actDbContet),
            new UnitOfWork(actDbContet),
            new CategoryRepository(actDbContet)
        );

        UpdateGenreInput input = new(
            targetGenre.Id,
            _fixture.GetValidGenreName(),
            !targetGenre.IsActive
        );

        GenreModelOutput output = await updateGenre.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.IsActive!);
        output.Categories.Should().HaveCount(relatedCategories.Count);
        List<Guid> expectedRelatedCategoryIds = relatedCategories
            .Select(category => category.Id)
            .ToList();
        List<Guid> relatedCategoryIdsFromOutput = output.Categories
            .Select(relatedCategory => relatedCategory.Id)
            .ToList();
        relatedCategoryIdsFromOutput.Should().BeEquivalentTo(expectedRelatedCategoryIds);
        CodeflixCatalogDbContext codeflixCatalogDbContext = _fixture.CreateDbContext(true);
        DomainEntity.Genre? genreFromDb =
            await codeflixCatalogDbContext.Genres.FindAsync(targetGenre.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Id.Should().Be(targetGenre.Id);
        genreFromDb.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be((bool)input.IsActive!);
        List<Guid> relatedCategoryIdsFromDb = await arrangeDbContext
            .GenresCategories.AsNoTracking()
            .Where(relation => relation.GenreId == input.Id)
            .Select(relation => relation.CategoryId)
            .ToListAsync();
        relatedCategoryIdsFromDb.Should().BeEquivalentTo(expectedRelatedCategoryIds);
    }

    [Fact(DisplayName = nameof(UpdateGenre_With_Empty_Category_Ids_Clean_Relations))]
    [Trait("Integration/Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenre_With_Empty_Category_Ids_Clean_Relations()
    {
        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoriesList(10);
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
        CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
        DomainEntity.Genre targetGenre = exampleGenres[5];
        List<DomainEntity.Category> relatedCategories = exampleCategories.GetRange(0, 5);
        relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
        List<GenresCategories> relations = targetGenre.Categories
            .Select(categoryId => new GenresCategories(categoryId, targetGenre.Id))
            .ToList();
        await arrangeDbContext.AddRangeAsync(exampleGenres);
        await arrangeDbContext.AddRangeAsync(exampleCategories);
        await arrangeDbContext.AddRangeAsync(relations);
        await arrangeDbContext.SaveChangesAsync();
        CodeflixCatalogDbContext actDbContet = _fixture.CreateDbContext(true);
        UseCase.UpdateGenre updateGenre = new(
            new GenreRepository(actDbContet),
            new UnitOfWork(actDbContet),
            new CategoryRepository(actDbContet)
        );

        UpdateGenreInput input = new(
            targetGenre.Id,
            _fixture.GetValidGenreName(),
            !targetGenre.IsActive,
            new List<Guid>()
        );

        GenreModelOutput output = await updateGenre.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.IsActive!);
        output.Categories.Should().HaveCount(0);
        List<Guid> relatedCategoryIdsFromOutput = output.Categories
            .Select(relatedCategory => relatedCategory.Id)
            .ToList();
        relatedCategoryIdsFromOutput.Should().BeEquivalentTo(new List<Guid>());
        CodeflixCatalogDbContext codeflixCatalogDbContext = _fixture.CreateDbContext(true);
        DomainEntity.Genre? genreFromDb =
            await codeflixCatalogDbContext.Genres.FindAsync(targetGenre.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Id.Should().Be(targetGenre.Id);
        genreFromDb.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be((bool)input.IsActive!);
        List<Guid> relatedCategoryIdsFromDb = await arrangeDbContext
            .GenresCategories.AsNoTracking()
            .Where(relation => relation.GenreId == input.Id)
            .Select(relation => relation.CategoryId)
            .ToListAsync();
        relatedCategoryIdsFromDb.Should().BeEquivalentTo(new List<Guid>());
    }

    //[Fact(DisplayName = nameof(UpdateGenre))]
    //[Trait("Integration/Application", "UpdateGenre - Use Cases")]
    //public async Task UpdateGenre()
    //{
    //    // Arrange
    //    (DomainEntity.Genre targetGenre,
    //     CodeflixCatalogDbContext arrangeContext,
    //     CodeflixCatalogDbContext actDbContext) = await ArrangeTest();

    //    // Act
    //    UseCase.UpdateGenre updateGenre = new(
    //        new GenreRepository(actDbContext),
    //        new UnitOfWork(actDbContext),
    //        new CategoryRepository(actDbContext)
    //    );

    //    UpdateGenreInput updateGenreInput = new(
    //        targetGenre.Id,
    //        _fixture.GetValidGenreName(),
    //        !targetGenre.IsActive
    //    );

    //    GenreModelOutput output = await updateGenre.Handle(updateGenreInput, CancellationToken.None);

    //    // Assert
    //    output.Should().NotBeNull();
    //    output.Id.Should().Be(targetGenre.Id);
    //    output.Name.Should().Be(updateGenreInput.Name);
    //    output.IsActive.Should().Be((bool)updateGenreInput.IsActive!);

    //    CodeflixCatalogDbContext assertDbContext = _fixture.CreateDbContext(true);
    //    DomainEntity.Genre? genreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);

    //    genreFromDb.Should().NotBeNull();
    //    genreFromDb!.Id.Should().Be(targetGenre.Id);
    //    genreFromDb.Name.Should().Be(updateGenreInput.Name);
    //    genreFromDb.IsActive.Should().Be((bool)updateGenreInput.IsActive!);
    //}

    //private async Task<(DomainEntity.Genre, CodeflixCatalogDbContext, CodeflixCatalogDbContext)> ArrangeTest()
    //{
    //    List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
    //    CodeflixCatalogDbContext arrangeContext = _fixture.CreateDbContext();
    //    DomainEntity.Genre targetGenre = exampleGenres[5];
    //    await arrangeContext.AddRangeAsync(exampleGenres);
    //    await arrangeContext.SaveChangesAsync();
    //    CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);

    //    return (targetGenre, arrangeContext, actDbContext);
    //}

}
