﻿using FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenres;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenres;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.ListGenres;

[Collection(nameof(ListGenresTestFixture))]
public class ListGenresTest
{
    private readonly ListGenresTestFixture _fixture;

    public ListGenresTest(ListGenresTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(ListGenres))]
    [Trait("Integration/Application", "ListGenres - Use Cases")]
    public async Task ListGenres()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
        CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(exampleGenres);
        await arrangeDbContext.SaveChangesAsync();

        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);

        UseCase.ListGenres useCase = new(
            new GenreRepository(actDbContext),
            new CategoryRepository(actDbContext)
        );

        ListGenresInput input = new(1, 20);

        ListGenresOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleGenres.Count);
        output.Items.Should().HaveCount(exampleGenres.Count);
        output.Items.ToList().ForEach(outputItem =>
        {
            DomainEntity.Genre? exampleItem =
                exampleGenres.Find(example => example.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
        });
    }

    [Fact(DisplayName = nameof(ListGenres_Returns_Empty_When_Persistence_Is_Empty))]
    [Trait("Integration/Application", "ListGenres - Use Cases")]
    public async Task ListGenres_Returns_Empty_When_Persistence_Is_Empty()
    {
        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext();

        UseCase.ListGenres useCase = new(
            new GenreRepository(actDbContext),
            new CategoryRepository(actDbContext)
        );

        ListGenresInput input = new(1, 20);

        ListGenresOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);
        output.Items.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(ListGenres_Verify_Relations))]
    [Trait("Integration/Application", "ListGenres - Use Cases")]
    public async Task ListGenres_Verify_Relations()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoriesList(10);
        Random random = new();
        exampleGenres.ForEach(genre =>
        {
            int relationsCount = random.Next(0, 3);
            for (int i = 0; i < relationsCount; i++)
            {
                int selectedCategoryIndex = exampleCategories.Count - 1;
                DomainEntity.Category selectedCategory = exampleCategories[random.Next(0, selectedCategoryIndex)];
                if (!genre.Categories.Contains(selectedCategory.Id))
                    genre.AddCategory(selectedCategory.Id);
            }
        });

        List<GenresCategories> genresCategories = new();
        exampleGenres.ForEach(
            genre => genre.Categories.ToList().ForEach(
               categoryId => genresCategories.Add(
                   new GenresCategories(categoryId, genre.Id))
            )
        );
        CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(exampleGenres);
        await arrangeDbContext.AddRangeAsync(exampleCategories);
        await arrangeDbContext.AddRangeAsync(genresCategories);
        await arrangeDbContext.SaveChangesAsync();

        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);

        UseCase.ListGenres useCase = new(
            new GenreRepository(actDbContext),
            new CategoryRepository(actDbContext)
        );

        ListGenresInput input = new(1, 20);

        ListGenresOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleGenres.Count);
        output.Items.Should().HaveCount(exampleGenres.Count);
        output.Items.ToList().ForEach(outputItem =>
        {
            DomainEntity.Genre? exampleItem =
                exampleGenres.Find(example => example.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            List<Guid> outputItemCategoryIds = outputItem.Categories.Select(x => x.Id).ToList();
            outputItemCategoryIds.Should().BeEquivalentTo(exampleItem.Categories);
            outputItem.Categories.ToList().ForEach(outputItemCategory =>
            {
                DomainEntity.Category? exampleCategory =
                    exampleCategories.Find(example => example.Id == outputItemCategory.Id);
                exampleCategory.Should().NotBeNull();
                outputItemCategory.Name.Should().Be(exampleCategory!.Name);
            });
        });
    }

    [Theory(DisplayName = nameof(ListGenres_Paginated))]
    [Trait("Integration/Application", "ListGenres - Use Cases")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task ListGenres_Paginated(
        int quantityToGenerate,
        int page,
        int perPage,
        int expectedQuantityItems
    )
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(quantityToGenerate);
        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoriesList(10);
        Random random = new();
        exampleGenres.ForEach(genre =>
        {
            int relationsCount = random.Next(0, 3);
            for (int i = 0; i < relationsCount; i++)
            {
                int selectedCategoryIndex = exampleCategories.Count - 1;
                DomainEntity.Category selectedCategory = exampleCategories[random.Next(0, selectedCategoryIndex)];
                if (!genre.Categories.Contains(selectedCategory.Id))
                    genre.AddCategory(selectedCategory.Id);
            }
        });

        List<GenresCategories> genresCategories = new();
        exampleGenres.ForEach(
            genre => genre.Categories.ToList().ForEach(
               categoryId => genresCategories.Add(
                   new GenresCategories(categoryId, genre.Id))
            )
        );
        CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(exampleGenres);
        await arrangeDbContext.AddRangeAsync(exampleCategories);
        await arrangeDbContext.AddRangeAsync(genresCategories);
        await arrangeDbContext.SaveChangesAsync();

        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);

        UseCase.ListGenres useCase = new(
            new GenreRepository(actDbContext),
            new CategoryRepository(actDbContext)
        );

        ListGenresInput input = new(page, perPage);

        ListGenresOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleGenres.Count);
        output.Items.Should().HaveCount(expectedQuantityItems);
        output.Items.ToList().ForEach(outputItem =>
        {
            DomainEntity.Genre? exampleItem =
                exampleGenres.Find(example => example.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            List<Guid> outputItemCategoryIds = outputItem.Categories.Select(x => x.Id).ToList();
            outputItemCategoryIds.Should().BeEquivalentTo(exampleItem.Categories);
            outputItem.Categories.ToList().ForEach(outputItemCategory =>
            {
                DomainEntity.Category? exampleCategory =
                    exampleCategories.Find(example => example.Id == outputItemCategory.Id);
                exampleCategory.Should().NotBeNull();
                outputItemCategory.Name.Should().Be(exampleCategory!.Name);
            });
        });
    }

    [Theory(DisplayName = nameof(Search_By_Text))]
    [Trait("Integration/Application", "ListGenres - Use Cases")]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Horror", 2, 5, 0, 3)]
    [InlineData("Sci-fi", 1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("Sci-fi Other", 1, 3, 0, 0)]
    [InlineData("Robots", 1, 5, 2, 2)]
    public async Task Search_By_Text(
        string search,
        int page,
        int perPage,
        int expectedQuantityItemsReturned,
        int expectedQuantityTotalItems
    )
    {

        var exampleGenres = _fixture.GetExampleListGenresByNames(
            new List<string>() {
                "Action",
                "Horror",
                "Horror - Robots",
                "Horror - Based on Real Facts",
                "Drama",
                "Sci-fi IA",
                "Sci-fi Space",
                "Sci-fi Robots",
                "Sci-fi Future",
            }
        );

        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoriesList(10);
        Random random = new();
        exampleGenres.ForEach(genre =>
        {
            int relationsCount = random.Next(0, 3);
            for (int i = 0; i < relationsCount; i++)
            {
                int selectedCategoryIndex = exampleCategories.Count - 1;
                DomainEntity.Category selectedCategory = exampleCategories[random.Next(0, selectedCategoryIndex)];
                if (!genre.Categories.Contains(selectedCategory.Id))
                    genre.AddCategory(selectedCategory.Id);
            }
        });

        List<GenresCategories> genresCategories = new();
        exampleGenres.ForEach(
            genre => genre.Categories.ToList().ForEach(
               categoryId => genresCategories.Add(
                   new GenresCategories(categoryId, genre.Id))
            )
        );
        CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(exampleGenres);
        await arrangeDbContext.AddRangeAsync(exampleCategories);
        await arrangeDbContext.AddRangeAsync(genresCategories);
        await arrangeDbContext.SaveChangesAsync();

        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);

        UseCase.ListGenres useCase = new(
            new GenreRepository(actDbContext),
            new CategoryRepository(actDbContext)
        );

        ListGenresInput input = new(page, perPage, search);

        ListGenresOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(expectedQuantityTotalItems);
        output.Items.Should().HaveCount(expectedQuantityItemsReturned);
        output.Items.ToList().ForEach(outputItem =>
        {
            DomainEntity.Genre? exampleItem =
                exampleGenres.Find(example => example.Id == outputItem.Id);
            outputItem.Name.Should().Contain(search);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            List<Guid> outputItemCategoryIds = outputItem.Categories.Select(x => x.Id).ToList();
            outputItemCategoryIds.Should().BeEquivalentTo(exampleItem.Categories);
            outputItem.Categories.ToList().ForEach(outputItemCategory =>
            {
                DomainEntity.Category? exampleCategory =
                    exampleCategories.Find(example => example.Id == outputItemCategory.Id);
                exampleCategory.Should().NotBeNull();
                outputItemCategory.Name.Should().Be(exampleCategory!.Name);
            });
        });
    }

    [Theory(DisplayName = nameof(Ordered))]
    [Trait("Integration/Application", "ListGenres - Use Cases")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("", "asc")]
    public async Task Ordered(
            string orderBy,
            string order
    )
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoriesList(10);
        Random random = new();
        exampleGenres.ForEach(genre =>
        {
            int relationsCount = random.Next(0, 3);
            for (int i = 0; i < relationsCount; i++)
            {
                int selectedCategoryIndex = exampleCategories.Count - 1;
                DomainEntity.Category selectedCategory = exampleCategories[random.Next(0, selectedCategoryIndex)];
                if (!genre.Categories.Contains(selectedCategory.Id))
                    genre.AddCategory(selectedCategory.Id);
            }
        });

        List<GenresCategories> genresCategories = new();
        exampleGenres.ForEach(
            genre => genre.Categories.ToList().ForEach(
               categoryId => genresCategories.Add(
                   new GenresCategories(categoryId, genre.Id))
            )
        );

        CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(exampleGenres);
        await arrangeDbContext.AddRangeAsync(exampleCategories);
        await arrangeDbContext.AddRangeAsync(genresCategories);
        await arrangeDbContext.SaveChangesAsync();

        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);

        UseCase.ListGenres useCase = new(
            new GenreRepository(actDbContext),
            new CategoryRepository(actDbContext)
        );

        var orderEnum = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc;

        ListGenresInput input = new(1, 20, sort: orderBy, dir: orderEnum);

        ListGenresOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleGenres.Count);
        output.Items.Should().HaveCount(exampleGenres.Count);
        var expectedOrderedList = _fixture.CloneGenresListOrdered(
            exampleGenres,
            orderBy,
            orderEnum
        );
        for (int indice = 0; indice < expectedOrderedList.Count; indice++)
        {
            var expectedItem = expectedOrderedList[indice];
            var outputItem = output.Items[indice];
            expectedItem.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem!.Name.Should().Be(expectedItem.Name);
            outputItem!.Id.Should().Be(expectedItem!.Id);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            outputItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
            expectedItem.Should().NotBeNull();
            outputItem.Name.Should().Be(expectedItem!.Name);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            List<Guid> outputItemCategoryIds = outputItem.Categories.Select(x => x.Id).ToList();
            outputItemCategoryIds.Should().BeEquivalentTo(expectedItem.Categories);
            outputItem.Categories.ToList().ForEach(outputItemCategory =>
            {
                DomainEntity.Category? exampleCategory =
                    exampleCategories.Find(example => example.Id == outputItemCategory.Id);
                exampleCategory.Should().NotBeNull();
                outputItemCategory.Name.Should().Be(exampleCategory!.Name);
            });
        }
    }
}
