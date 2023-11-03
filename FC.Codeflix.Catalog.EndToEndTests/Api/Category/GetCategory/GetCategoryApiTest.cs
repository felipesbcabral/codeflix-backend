using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.EndToEndTests.Api.Category.GetCategoryById;
using FC.Codeflix.Catalog.EndToEndTests.Extensions.DateTime;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.GetCategory;

[Collection(nameof(GetCategoryApiTestFixture))]
public class GetCategoryApiTest : IDisposable
{
    private readonly GetCategoryApiTestFixture _fixture;

    public GetCategoryApiTest(GetCategoryApiTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Get_Category))]
    [Trait("EndToEnd/API", "Category/Get - Endpoints")]
    public async Task Get_Category()
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList();
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = exampleCategoriesList[10];

        var (response, output) = await _fixture.ApiClient.Get<CategoryModelOutput>(
            $"/categories/{exampleCategory.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output!.Id.Should().Be(exampleCategory.Id);
        output.Name.Should().Be(exampleCategory.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        output.CreatedAt.TrimMillisseconds().Should()
            .Be(exampleCategory.CreatedAt.TrimMillisseconds());
    }

    [Fact(DisplayName = nameof(Error_When_Not_Found))]
    [Trait("EndToEnd/API", "Category/Get - Endpoints")]
    public async Task Error_When_Not_Found()
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList();
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var randomGuid = Guid.NewGuid();

        var (response, output) = await _fixture.ApiClient.Get<ProblemDetails>(
            $"/categories/{randomGuid}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output!.Status.Should().Be((int)StatusCodes.Status404NotFound);
        output!.Type.Should().Be("NotFound");
        output!.Title.Should().Be("Not Found");
        output!.Detail.Should().Be($"Category '{randomGuid}' not found.");
        output!.Type.Should().Be("NotFound");
    }

    public void Dispose()
    => _fixture.CleanPersistence();
}
