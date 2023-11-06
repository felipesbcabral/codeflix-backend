using FC.Codeflix.Catalog.Api.ApiModels.Category;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.UpdateCategory;

[Collection(nameof(UpdateCategoryApiTestFixture))]
public class UpdateCategoryApiTest : IDisposable
{
    private readonly UpdateCategoryApiTestFixture _fixture;

    public UpdateCategoryApiTest(UpdateCategoryApiTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Update_Category))]
    [Trait("EndToEnd/API", "Category/Update - Endpoints")]
    public async void Update_Category()
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList();
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = exampleCategoriesList[10];
        var input = _fixture.GetExampleInput();

        var (response, output) = await _fixture.ApiClient.Put<CategoryModelOutput>(
            $"/categories/{exampleCategory.Id}",
            input
        );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Id.Should().Be(exampleCategory.Id);
        output!.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be((bool)input.IsActive!);
        var dbCategory = await _fixture
            .Persistence.GetById(exampleCategory.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be((bool)input.IsActive!);
    }

    [Fact(DisplayName = nameof(Update_Category_Only_Name))]
    [Trait("EndToEnd/API", "Category/Update - Endpoints")]
    public async void Update_Category_Only_Name()
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList();
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = exampleCategoriesList[10];
        var input = new UpdateCategoryApiInput(
            _fixture.GetValidCategoryName()
        );

        var (response, output) = await _fixture.ApiClient.Put<CategoryModelOutput>(
            $"/categories/{exampleCategory.Id}",
            input
        );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Id.Should().Be(exampleCategory.Id);
        output!.Name.Should().Be(input.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be((bool)exampleCategory.IsActive!);
        var dbCategory = await _fixture
            .Persistence.GetById(exampleCategory.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(exampleCategory.Description);
        dbCategory.IsActive.Should().Be((bool)exampleCategory.IsActive!);
    }

    [Fact(DisplayName = nameof(Update_Category_Only_Name_And_Description))]
    [Trait("EndToEnd/API", "Category/Update - Endpoints")]
    public async void Update_Category_Only_Name_And_Description()
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList();
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = exampleCategoriesList[10];
        var input = new UpdateCategoryApiInput(
            _fixture.GetValidCategoryName(),
            _fixture.GetValidCategoryDescription()
        );

        var (response, output) = await _fixture.ApiClient.Put<CategoryModelOutput>(
            $"/categories/{exampleCategory.Id}",
            input
        );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Id.Should().Be(exampleCategory.Id);
        output!.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be((bool)exampleCategory.IsActive!);
        var dbCategory = await _fixture
            .Persistence.GetById(exampleCategory.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be((bool)exampleCategory.IsActive!);
    }

    [Fact(DisplayName = nameof(Error_When_Not_Found))]
    [Trait("EndToEnd/API", "Category/Update - Endpoints")]
    public async void Error_When_Not_Found()
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList();
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var randomGuid = Guid.NewGuid();
        var input = _fixture.GetExampleInput();

        var (response, output) = await _fixture.ApiClient.Put<ProblemDetails>(
            $"/categories/{randomGuid}",
            input
        );

        response.Should().NotBeNull();
        response!.StatusCode.Should()
            .Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output!.Type.Should().Be("NotFound");
        output!.Title.Should().Be("Not Found");
        output!.Detail.Should().Be($"Category '{randomGuid}' not found.");
        output!.Status.Should().Be((int)StatusCodes.Status404NotFound);
        output!.Type.Should().Be("NotFound");
    }

    [Theory(DisplayName = nameof(Error_When_Cant_Instantiate_Aggregate))]
    [Trait("EndToEnd/API", "Category/Update - Endpoints")]
    [MemberData(
    nameof(UpdateCategoryApiTestDataGenerator.GetInvalidInputs),
    MemberType = typeof(UpdateCategoryApiTestDataGenerator))]
    public async void Error_When_Cant_Instantiate_Aggregate(
    UpdateCategoryApiInput input,
    string expectedDetail)
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList();
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = exampleCategoriesList[10];

        var (response, output) = await _fixture.ApiClient.Put<ProblemDetails>(
            $"/categories/{exampleCategory.Id}",
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
