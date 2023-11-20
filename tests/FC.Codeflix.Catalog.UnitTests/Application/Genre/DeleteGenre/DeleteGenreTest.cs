using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Genre.DeleteGenre;
using FluentAssertions;
using Moq;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.DeleteGenre;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.DeleteGenre;


[Collection(nameof(DeleteGenreTestFixture))]
public class DeleteGenreTest
{
    private readonly DeleteGenreTestFixture _fixture;

    public DeleteGenreTest(DeleteGenreTestFixture deleteGenreTestFixture)
    {
        _fixture = deleteGenreTestFixture;
    }

    [Fact(DisplayName = nameof(Update_Genre))]
    [Trait("Application", "DeleteGenre - Use Cases")]
    public async Task Update_Genre()
    {
        var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre();
        genreRepositoryMock.Setup(x => x.Get(
            It.Is<Guid>(x => x == exampleGenre.Id),
            It.IsAny<CancellationToken>()
            )).ReturnsAsync(exampleGenre);

        var useCase = new UseCase.DeleteGenre(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var input = new DeleteGenreInput(exampleGenre.Id);

        await useCase.Handle(input, CancellationToken.None);


        genreRepositoryMock.Verify(
            x => x.Get(
                It.Is<Guid>(x => x == exampleGenre.Id),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        genreRepositoryMock.Verify(
            x => x.Delete(
                It.Is<DomainEntity.Genre>(x => x.Id == exampleGenre.Id),
                It.IsAny<CancellationToken>()
                    ),
            Times.Once
        );

        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(Throw_When_Not_Found))]
    [Trait("Application", "DeleteGenre - Use Cases")]
    public async Task Throw_When_Not_Found()
    {
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
        var exampleId = Guid.NewGuid();

        genreRepositoryMock.Setup(x => x.Get(
            It.Is<Guid>(x => x == exampleId),
            It.IsAny<CancellationToken>()
            )).ThrowsAsync(new NotFoundException(
                $"Genre '{exampleId}' not found"
            ));

        var useCase = new UseCase.DeleteGenre(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var input = new DeleteGenreInput(exampleId);

        var action = async ()
            => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Genre '{exampleId}' not found");

        genreRepositoryMock.Verify(x => x.Get(
            It.Is<Guid>(x => x == exampleId),
            It.IsAny<CancellationToken>()
            ), Times.Once
        );
    }
}
