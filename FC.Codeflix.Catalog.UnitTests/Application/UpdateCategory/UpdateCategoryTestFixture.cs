using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.UnitTests.Common;
using Moq;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.UpdateCategory;

[CollectionDefinition(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTestFixtureCollection : ICollectionFixture<UpdateCategoryTestFixture> { }
public class UpdateCategoryTestFixture : BaseFixture
{
    public Mock<ICategoryRepository> GetRepositoryMock()
    => new();
    public Mock<IUnitOfWork> GetUnitOfWorkMock()
    => new();

    public string GetValidCategoryName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];
        if (categoryName.Length > 255)
            categoryName = categoryName[..255];
        return categoryName;
    }

    public string GetValidCategoryDescription()
    {
        var categoryDescription =
            Faker.Commerce.ProductDescription();
        if (categoryDescription.Length > 10000)
            categoryDescription =
                categoryDescription[..10000];
        return categoryDescription;
    }

    public bool GetRandomBoolean()
        => (new Random().NextDouble() < 0.5);

    public Category GetExampleCategory()
        => new(
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean()
    );

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