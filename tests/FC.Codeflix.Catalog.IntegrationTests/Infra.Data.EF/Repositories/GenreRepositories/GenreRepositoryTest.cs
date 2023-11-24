using FC.Codeflix.Catalog.Infra.Data.EF;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories.GenreRepository;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.GenreRepositories;

[Collection(nameof(GenreRepositoryTestFixture))]
public class GenreRepositoryTest
{
    private readonly GenreRepositoryTestFixture _fixture;

    public GenreRepositoryTest(GenreRepositoryTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Insert))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task Insert()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);
        categoriesListExample.ForEach(
            category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        var genreRepository = new Repository(dbContext);
        await dbContext.SaveChangesAsync(CancellationToken.None);



        await genreRepository.Insert(exampleGenre, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var assertsDbContext = _fixture.CreateDbContext(true);


        var dbGenre = await assertsDbContext
            .Genres.FindAsync(exampleGenre.Id);

        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(exampleGenre.Name);
        dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
        dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        var genresCategories = await assertsDbContext
            .GenresCategories
            .Where(gc => gc.GenreId == exampleGenre.Id)
            .ToListAsync();
        genresCategories.Should()
            .HaveCount(categoriesListExample.Count);
        genresCategories.ForEach(genreCategory =>
        {
            var category = categoriesListExample
                .FirstOrDefault(c => c.Id == genreCategory.CategoryId);
            category.Should().NotBeNull();
        });
    }
}
