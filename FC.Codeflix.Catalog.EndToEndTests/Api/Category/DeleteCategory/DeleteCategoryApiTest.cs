using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.DeleteCategory;

[Collection(nameof(DeleteCategoryApiTestFixture))]
public class DeleteCategoryApiTest : IDisposable
{
    private readonly DeleteCategoryApiTestFixture _fixture;

    public DeleteCategoryApiTest(DeleteCategoryApiTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Delete_Category))]
    [Trait("EndToEnd/API", "Category/Delete - Endpoints")]
    public async Task Delete_Category()
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList();
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = exampleCategoriesList[10];

        var (response, output) = await _fixture.ApiClient.Delete<object>(
            $"/categories/{exampleCategory.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should()
            .Be((HttpStatusCode)StatusCodes.Status204NoContent);
        output.Should().BeNull();
        var persistenceCategory = await _fixture.Persistence.GetById(exampleCategory.Id);
        persistenceCategory.Should().BeNull();
    }

    [Fact(DisplayName = nameof(Error_When_Not_Found))]
    [Trait("EndToEnd/API", "Category/Get - Endpoints")]
    public async Task Error_When_Not_Found()
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList();
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var randomGuid = Guid.NewGuid();

        var (response, output) = await _fixture.ApiClient.Delete<ProblemDetails>(
            $"/categories/{randomGuid}");

        response.Should().NotBeNull();
        response!.StatusCode.Should()
            .Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output!.Type.Should().Be("NotFound");
        output!.Title.Should().Be("Not Found");
        output!.Detail.Should().Be($"Category '{randomGuid}' not found.");
        output!.Status.Should().Be((int)StatusCodes.Status404NotFound);
        output!.Type.Should().Be("NotFound");
    }

    public void Dispose()
    => _fixture.CleanPersistence();
}
