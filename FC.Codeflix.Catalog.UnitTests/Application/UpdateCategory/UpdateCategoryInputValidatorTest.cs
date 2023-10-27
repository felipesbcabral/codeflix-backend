using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FluentAssertions;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.UpdateCategory;

[Collection(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryInputValidatorTest
{
    private readonly UpdateCategoryTestFixture _fixture;

    public UpdateCategoryInputValidatorTest(UpdateCategoryTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Dont_Validate_When_Empty_Guid))]
    [Trait("Application", "UpdateCategoryInputValidator - Use Cases")]
    public void Dont_Validate_When_Empty_Guid()
    {
        var input = _fixture.GetValidInput(Guid.Empty);
        var validator = new UpdateCategoryInputValidator();

        var validateResult = validator.Validate(input);

        validateResult.Should().NotBeNull();
        validateResult.IsValid.Should().BeFalse();
        validateResult.Errors.Should().HaveCount(1);
        validateResult.Errors[0].ErrorMessage.Should().Be("'Id' must not be empty.");
    }

    [Fact(DisplayName = nameof(Validate_When_Is_Valid))]
    [Trait("Application", "UpdateCategoryInputValidator - Use Cases")]
    public void Validate_When_Is_Valid()
    {
        var input = _fixture.GetValidInput();
        var validator = new UpdateCategoryInputValidator();

        var validateResult = validator.Validate(input);

        validateResult.Should().NotBeNull();
        validateResult.IsValid.Should().BeTrue();
        validateResult.Errors.Should().HaveCount(0);
    }
}
