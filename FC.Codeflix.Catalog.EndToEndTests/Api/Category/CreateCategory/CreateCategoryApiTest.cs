using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.CreateCategory;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.CreateCategory;

[Collection(nameof(CreateCategoryApiTestFixture))]
public class CreateCategoryApiTest : IDisposable
{
    private readonly CreateCategoryApiTestFixture _fixture;

    public CreateCategoryApiTest(CreateCategoryApiTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Create_Category))]
    [Trait("EndToEnd/API", "Category/Create - Endpoints")]
    public async Task Create_Category()
    {
        var input = _fixture.GetExampleInput();

        var (response, output) = await _fixture.
            ApiClient.Post<CategoryModelOutput>(
                "/categories",
                input
            );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.Created);
        output.Should().NotBeNull();
        output!.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(input.IsActive);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should()
            .NotBeSameDateAs(default);
        var dbCategory = await _fixture
            .Persistence.GetById(output.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be(input.IsActive);
        dbCategory.Id.Should().NotBeEmpty();
        dbCategory.CreatedAt.Should()
            .NotBeSameDateAs(default);
    }

    [Theory(DisplayName = nameof(Error_When_Cant_Instantiate_Aggregate))]
    [Trait("EndToEnd/API", "Category/Create - Endpoints")]
    [MemberData(
        nameof(CreateCategoryApiTestDataGenerator.GetInvalidInputs),
        MemberType = typeof(CreateCategoryApiTestDataGenerator)
    )]
    public async Task Error_When_Cant_Instantiate_Aggregate(
        CreateCategoryInput input,
        string expectedDetail
    )
    {
        var (response, output) = await _fixture.
            ApiClient.Post<ProblemDetails>(
                "/categories",
                input
            );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Title.Should().Be("One or more validation errors ocurred");
        output!.Type.Should().Be("UnprocessableEntity");
        output!.Status.Should().Be(StatusCodes.Status422UnprocessableEntity);
        output!.Detail.Should().Be(expectedDetail);
    }


    public void Dispose()
        => _fixture.CleanPersistence();
}
