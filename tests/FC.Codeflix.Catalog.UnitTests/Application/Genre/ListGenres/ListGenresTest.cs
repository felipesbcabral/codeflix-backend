using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenres;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Moq;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenres;
namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.ListGenres;

[Collection(nameof(ListGenresTestFixture))]
public class ListGenresTest
{
    private readonly ListGenresTestFixture _fixture;

    public ListGenresTest(ListGenresTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(List_Genres))]
    [Trait("Application", "ListGenres - Use Cases")]
    public async Task List_Genres()
    {
        var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
        var genresListExample = _fixture.GetExampleGenresList();
        var input = _fixture.GetExampleInput();

        var outputRepositorySearch = new SearchOutput<DomainEntity.Genre>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: (IReadOnlyList<DomainEntity.Genre>)genresListExample,
            total: new Random().Next(50, 200)
         );

        genreRepositoryMock.Setup(x => x.Search(
            It.IsAny<SearchInput>(),
            It.IsAny<CancellationToken>()
            )).ReturnsAsync(outputRepositorySearch);

        var useCase = new UseCase.ListGenres(genreRepositoryMock.Object);


        ListGenresOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepositorySearch.CurrentPage);
        output.PerPage.Should().Be(outputRepositorySearch.PerPage);
        output.Total.Should().Be(outputRepositorySearch.Total);
        output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
        ((List<GenreModelOutput>)output.Items).ForEach(outputItem =>
        {
            var repositoryGenre = outputRepositorySearch.Items
                .FirstOrDefault(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            repositoryGenre.Should().NotBeNull();
            outputItem.Name.Should().Be(repositoryGenre!.Name);
            outputItem.IsActive.Should().Be(repositoryGenre.IsActive);
            outputItem.CreatedAt.Should().Be(repositoryGenre!.CreatedAt);
            outputItem.Categories.Should().HaveCount(repositoryGenre.Categories.Count);

            foreach (var expectedId in repositoryGenre.Categories)
            {
                outputItem.Categories.Should().Contain(relation => relation.Id == expectedId);
            }
        });


        genreRepositoryMock.Verify(
             x => x.Search(
                 It.Is<SearchInput>(searchInput =>
                     searchInput.Page == input.Page
                     && searchInput.PerPage == input.PerPage
                     && searchInput.Search == input.Search
                     && searchInput.OrderBy == input.Sort
                     && searchInput.Order == input.Dir
                 ),
                 It.IsAny<CancellationToken>()
             ),
             Times.Once
         );
    }

    [Fact(DisplayName = nameof(List_Empty))]
    [Trait("Application", "ListGenres - Use Cases")]
    public async Task List_Empty()
    {
        var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
        var input = _fixture.GetExampleInput();

        var outputRepositorySearch = new SearchOutput<DomainEntity.Genre>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: (new List<DomainEntity.Genre>()),
            total: new Random().Next(50, 200)
         );

        genreRepositoryMock.Setup(x => x.Search(
            It.IsAny<SearchInput>(),
            It.IsAny<CancellationToken>()
            )).ReturnsAsync(outputRepositorySearch);

        var useCase = new UseCase.ListGenres(genreRepositoryMock.Object);


        ListGenresOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepositorySearch.CurrentPage);
        output.PerPage.Should().Be(outputRepositorySearch.PerPage);
        output.Total.Should().Be(outputRepositorySearch.Total);
        output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);

        genreRepositoryMock.Verify(
             x => x.Search(
                 It.Is<SearchInput>(searchInput =>
                     searchInput.Page == input.Page
                     && searchInput.PerPage == input.PerPage
                     && searchInput.Search == input.Search
                     && searchInput.OrderBy == input.Sort
                     && searchInput.Order == input.Dir
                 ),
                 It.IsAny<CancellationToken>()
             ),
             Times.Once
         );
    }

    [Fact(DisplayName = nameof(List_Empty))]
    [Trait("Application", "ListGenres - Use Cases")]
    public async Task List_Using_Default_Input_Values()
    {
        var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
        var outputRepositorySearch = new SearchOutput<DomainEntity.Genre>(
            currentPage: 1,
            perPage: 15,
            items: (new List<DomainEntity.Genre>()),
            total: 0
         );

        genreRepositoryMock.Setup(x => x.Search(
            It.IsAny<SearchInput>(),
            It.IsAny<CancellationToken>()
            )).ReturnsAsync(outputRepositorySearch);

        var useCase = new UseCase.ListGenres(genreRepositoryMock.Object);


        ListGenresOutput output = await useCase.Handle(new UseCase.ListGenresInput(), CancellationToken.None);

        genreRepositoryMock.Verify(
             x => x.Search(
                 It.Is<SearchInput>(searchInput =>
                     searchInput.Page == 1
                     && searchInput.PerPage == 15
                     && searchInput.Search == ""
                     && searchInput.OrderBy == ""
                     && searchInput.Order == SearchOrder.Asc
                 ),
                 It.IsAny<CancellationToken>()
             ),
             Times.Once
         );
    }
}
