using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;
using UseCases = FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;

namespace FC.Codeflix.Catalog.UnitTests.Application.CreateCategory;

[Collection(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTest
{
    private readonly CreateCategoryTestFixture _fixture;

    public CreateCategoryTest(CreateCategoryTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Create_Category))]
    [Trait("Application", "CreateCategory - Use Cases")]
    public async void Create_Category()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

        var useCase = new UseCases.CreateCategory(
            repositoryMock.Object,
            unitOfWorkMock.Object
            );

        var input = _fixture.GetInput();

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(
            repository => repository.Insert(
                It.IsAny<Category>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        unitOfWorkMock.Verify(
            uow => uow.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(input.IsActive);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
    }

    [Theory(DisplayName = nameof(Throw_When_Cant_Instantiate_Aggregate))]
    [Trait("Application", "CreateCategory - Use Cases")]
    [MemberData(nameof(GetInvalidInputs))]
    public async void Throw_When_Cant_Instantiate_Aggregate(
        CreateCategoryInput input,
        string exceptionMessage)
    {
        var useCase = new UseCases.CreateCategory(
            _fixture.GetRepositoryMock().Object,
            _fixture.GetUnitOfWorkMock().Object
            );

        Func<Task> task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage(exceptionMessage);
    }

    public static IEnumerable<object[]> GetInvalidInputs()
    {
        var fixture = new CreateCategoryTestFixture();
        var invalidInputsList = new List<object[]>();

        var invalidInputName = fixture.GetInput();
        invalidInputName.Name = invalidInputName.Name[..2];
        invalidInputsList.Add(new object[]
        {
            invalidInputName,
            "Name should be at least 3 characters long"
        });

        var invalidInputNameTooLong = fixture.GetInput();
        var invalidName = fixture.Faker.Lorem.Letter(256);
        invalidInputNameTooLong.Name = invalidName;
        invalidInputsList.Add(new object[]
        {
            invalidInputNameTooLong,
            "Name should be less or equal to 255 characters long"
        });

        var invalidInputDescriptionNull = fixture.GetInput();
        invalidInputDescriptionNull.Description = null!;
        invalidInputsList.Add(new object[]
        {
            invalidInputDescriptionNull,
            "Description should not be null"
        });

        var invalidInputDescriptionTooLong = fixture.GetInput();
        var invalidDescription = fixture.Faker.Lorem.Letter(11000);
        invalidInputDescriptionTooLong.Description = invalidDescription;
        invalidInputsList.Add(new object[]
        {
            invalidInputDescriptionTooLong,
            "Description should be less or equal to 10000 characters long"
        });

        return invalidInputsList;
    }
}
