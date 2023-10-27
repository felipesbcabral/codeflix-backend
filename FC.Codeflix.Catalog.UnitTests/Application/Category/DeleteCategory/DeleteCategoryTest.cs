using FC.Codeflix.Catalog.Application.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.DeleteCategory;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.DeleteCategory;

[Collection(nameof(DeleteCategoryTestFixture))]
public class DeleteCategoryTest
{
    private readonly DeleteCategoryTestFixture _fixture;

    public DeleteCategoryTest(DeleteCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(Delete_Category))]
    [Trait("Application", "DeleteCategory - Use Cases")]
    public async Task Delete_Category()
    {
        //arrange
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var categoryExample = _fixture.GetExampleCategory();
        repositoryMock.Setup(x => x.Get(categoryExample.Id,
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(categoryExample);
        var input = new UseCase.DeleteCategoryInput(categoryExample.Id);
        var useCase = new UseCase.DeleteCategory(
            repositoryMock.Object,
            unitOfWorkMock.Object);

        //act
        await useCase.Handle(input, CancellationToken.None);

        //assert
        repositoryMock.Verify(x => x.Get(
            categoryExample.Id,
            It.IsAny<CancellationToken>()),
            Times.Once);
        repositoryMock.Verify(x => x.Delete(
            categoryExample,
            It.IsAny<CancellationToken>()),
            Times.Once);
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(Throw_When_Category_Not_Found))]
    [Trait("Application", "DeleteCategory - Use Cases")]
    public async Task Throw_When_Category_Not_Found()
    {
        //arrange
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGuid = Guid.NewGuid();
        repositoryMock.Setup(x => x.Get(
            exampleGuid,
            It.IsAny<CancellationToken>())
        ).ThrowsAsync(new NotFoundException($"Category '{exampleGuid}' not found"));
        var input = new UseCase.DeleteCategoryInput(exampleGuid);
        var useCase = new UseCase.DeleteCategory(
            repositoryMock.Object,
            unitOfWorkMock.Object);

        //act
        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should()
            .ThrowAsync<NotFoundException>();

        //assert
        repositoryMock.Verify(x => x.Get(
            exampleGuid,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
