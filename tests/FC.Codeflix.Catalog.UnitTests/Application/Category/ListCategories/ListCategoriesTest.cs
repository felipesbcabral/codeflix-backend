﻿using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Moq;
using Xunit;
using Entity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.ListCategories;

[Collection(nameof(ListCategoriesTestFixture))]
public class ListCategoriesTest
{
    private readonly ListCategoriesTestFixture _fixture;

    public ListCategoriesTest(ListCategoriesTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(List))]
    [Trait("Application", "ListCategories - Use Cases")]
    public async Task List()
    {
        //arrange
        var categoriesExampleList = _fixture.GetExampleCategoryList();
        var repositoryMock = _fixture.GetRepositoryMock();
        var input = _fixture.GetExampleInput();

        var outputRepositorySearch = new SearchOutput<Entity.Category>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: categoriesExampleList,
            total: new Random().Next(50, 200)
         );

        repositoryMock.Setup(x => x.Search(
            It.Is<SearchInput>(
                searchInput => searchInput.Page == input.Page &&
                searchInput.PerPage == input.PerPage &&
                searchInput.Search == input.Search &&
                searchInput.OrderBy == input.Sort &&
                searchInput.Order == input.Dir
            ),
            It.IsAny<CancellationToken>()
         )).ReturnsAsync(outputRepositorySearch);

        //act
        var useCase = new UseCase.ListCategories(repositoryMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        //assert
        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepositorySearch.CurrentPage);
        output.PerPage.Should().Be(outputRepositorySearch.PerPage);
        output.Total.Should().Be(outputRepositorySearch.Total);
        output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
        ((List<CategoryModelOutput>)output.Items).ForEach(outputItem =>
        {
            var repositoryCategory = outputRepositorySearch.Items
                .FirstOrDefault(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(repositoryCategory!.Name);
            outputItem.Description.Should().Be(repositoryCategory!.Description);
            outputItem.IsActive.Should().Be(repositoryCategory!.IsActive);
            outputItem.CreatedAt.Should().Be(repositoryCategory!.CreatedAt);
        });

        repositoryMock.Verify(x => x.Search(
            It.Is<SearchInput>(
             searchInput => searchInput.Page == input.Page
             && searchInput.PerPage == input.PerPage
             && searchInput.Search == input.Search
             && searchInput.OrderBy == input.Sort
             && searchInput.Order == input.Dir
            ),
            It.IsAny<CancellationToken>()
         ), Times.Once);
    }

    [Fact(DisplayName = nameof(List_Must_Return_Ok_When_Empty))]
    [Trait("Application", "ListCategories - Use Cases")]
    public async Task List_Must_Return_Ok_When_Empty()
    {
        //arrange
        var repositoryMock = _fixture.GetRepositoryMock();
        var input = _fixture.GetExampleInput();

        var outputRepositorySearch = new SearchOutput<Entity.Category>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: new List<Entity.Category>().AsReadOnly(),
            total: 0
         );

        repositoryMock.Setup(x => x.Search(
            It.Is<SearchInput>(
                searchInput => searchInput.Page == input.Page &&
                searchInput.PerPage == input.PerPage &&
                searchInput.Search == input.Search &&
                searchInput.OrderBy == input.Sort &&
                searchInput.Order == input.Dir
            ),
            It.IsAny<CancellationToken>()
         )).ReturnsAsync(outputRepositorySearch);

        //act
        var useCase = new UseCase.ListCategories(repositoryMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        //assert
        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepositorySearch.CurrentPage);
        output.PerPage.Should().Be(outputRepositorySearch.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);

        repositoryMock.Verify(x => x.Search(
            It.Is<SearchInput>(
             searchInput => searchInput.Page == input.Page
             && searchInput.PerPage == input.PerPage
             && searchInput.Search == input.Search
             && searchInput.OrderBy == input.Sort
             && searchInput.Order == input.Dir
            ),
            It.IsAny<CancellationToken>()
         ), Times.Once);
    }

    [Theory(DisplayName = nameof(List_Whithout_All_Parameters))]
    [Trait("Application", "ListCategories - Use Cases")]
    [MemberData(nameof(ListCategoriesTestDataGenerator.GetInputsWhithoutAllParameters),
        parameters: 10,
        MemberType = typeof(ListCategoriesTestDataGenerator)
    )]
    public async Task List_Whithout_All_Parameters(UseCase.ListCategoriesInput input)
    {
        //arrange
        var categoriesExampleList = _fixture.GetExampleCategoryList();
        var repositoryMock = _fixture.GetRepositoryMock();
        var outputRepositorySearch = new SearchOutput<Entity.Category>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: categoriesExampleList,
            total: new Random().Next(50, 200)
         );

        repositoryMock.Setup(x => x.Search(
            It.Is<SearchInput>(
                searchInput => searchInput.Page == input.Page &&
                searchInput.PerPage == input.PerPage &&
                searchInput.Search == input.Search &&
                searchInput.OrderBy == input.Sort &&
                searchInput.Order == input.Dir
            ),
            It.IsAny<CancellationToken>()
         )).ReturnsAsync(outputRepositorySearch);

        //act
        var useCase = new UseCase.ListCategories(repositoryMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        //assert
        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepositorySearch.CurrentPage);
        output.PerPage.Should().Be(outputRepositorySearch.PerPage);
        output.Total.Should().Be(outputRepositorySearch.Total);
        output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
        ((List<CategoryModelOutput>)output.Items).ForEach(outputItem =>
        {
            var repositoryCategory = outputRepositorySearch.Items
                .FirstOrDefault(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(repositoryCategory!.Name);
            outputItem.Description.Should().Be(repositoryCategory!.Description);
            outputItem.IsActive.Should().Be(repositoryCategory!.IsActive);
            outputItem.CreatedAt.Should().Be(repositoryCategory!.CreatedAt);
        });

        repositoryMock.Verify(x => x.Search(
            It.Is<SearchInput>(
             searchInput => searchInput.Page == input.Page
             && searchInput.PerPage == input.PerPage
             && searchInput.Search == input.Search
             && searchInput.OrderBy == input.Sort
             && searchInput.Order == input.Dir
            ),
            It.IsAny<CancellationToken>()
         ), Times.Once);
    }
}
