using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.UnitTests.Application.Category.Common;
using Xunit;
namespace FC.Codeflix.Catalog.UnitTests.Application.Category.CreateCategory;

[CollectionDefinition(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTestFixtureCollection : ICollectionFixture<CreateCategoryTestFixture> { }

public class CreateCategoryTestFixture : CategoryUseCasesBaseFixture
{
    public CreateCategoryInput GetInvalidInputShortName()
    {
        var invalidInputName = GetInput();
        invalidInputName.Name = invalidInputName.Name[..2];
        return invalidInputName;
    }

    public CreateCategoryInput GetInvalidInputTooLongName()
    {
        var invalidInputTooLongName = GetInput();
        var invalidName = Faker.Lorem.Letter(256);
        invalidInputTooLongName.Name = invalidName;
        return invalidInputTooLongName;
    }

    public CreateCategoryInput GetInvalidInputDescriptionNull()
    {
        var invalidInputDescriptionNull = GetInput();
        invalidInputDescriptionNull.Description = null!;
        return invalidInputDescriptionNull;
    }

    public CreateCategoryInput GetInvalidInputDescriptionTooLong()
    {
        var invalidInputDescriptionTooLong = GetInput();
        var invalidDescription = Faker.Lorem.Letter(11000);
        invalidInputDescriptionTooLong.Description = invalidDescription;
        return invalidInputDescriptionTooLong;
    }
}
