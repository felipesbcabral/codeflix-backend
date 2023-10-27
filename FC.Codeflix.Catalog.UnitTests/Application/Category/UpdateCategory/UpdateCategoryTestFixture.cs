using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.Codeflix.Catalog.UnitTests.Application.Category.Common;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.UpdateCategory;

[CollectionDefinition(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTestFixtureCollection : ICollectionFixture<UpdateCategoryTestFixture> { }
public class UpdateCategoryTestFixture : CategoryUseCasesBaseFixture
{
    public UpdateCategoryInput GetValidInput(Guid? id = null)
        => new(
                id ?? Guid.NewGuid(),
                GetValidCategoryName(),
                GetValidCategoryDescription(),
                GetRandomBoolean()
            );

    public UpdateCategoryInput GetInvalidInputShortName()
    {
        var invalidInputName = GetValidInput();
        invalidInputName.Name = invalidInputName.Name[..2];
        return invalidInputName;
    }

    public UpdateCategoryInput GetInvalidInputTooLongName()
    {
        var invalidInputTooLongName = GetValidInput();
        var invalidName = Faker.Lorem.Letter(256);
        invalidInputTooLongName.Name = invalidName;
        return invalidInputTooLongName;
    }

    public UpdateCategoryInput GetInvalidInputDescriptionTooLong()
    {
        var invalidInputDescriptionTooLong = GetValidInput();
        var invalidDescription = Faker.Lorem.Letter(11000);
        invalidInputDescriptionTooLong.Description = invalidDescription;
        return invalidInputDescriptionTooLong;
    }
}