using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Category;

[Collection(nameof(CategoryTestFixture))]
public class CategoryTest
{
    private readonly CategoryTestFixture _categoryTestFixture;

    public CategoryTest(CategoryTestFixture categoryTestFixture)
        => _categoryTestFixture = categoryTestFixture;

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Instantiate()
    {
        //Arrange
        var validCategory = _categoryTestFixture.GetValidCategory();

        var dateTimeBefore = DateTime.Now;

        //Act
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description);

        var dateTimeAfter = DateTime.Now;
        //Assert
        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
        category.Id.Should().NotBe(default(Guid));
        category.CreatedAt.Should().NotBeSameDateAs(default);
        category.CreatedAt.Should().BeAfter(dateTimeBefore);
        category.CreatedAt.Should().BeBefore(dateTimeAfter);
        category.IsActive.Should().BeTrue();
    }

    [Theory(DisplayName = nameof(Instantiate_With_Is_Active))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void Instantiate_With_Is_Active(bool isActive)
    {
        // Arrange
        var validCategory = _categoryTestFixture.GetValidCategory();
        var dateTimeBefore = DateTime.Now;

        // Act
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, isActive);

        Thread.Sleep(1);

        var dateTimeAfter = DateTime.Now;

        // Assert
        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
        category.Id.Should().NotBe(default(Guid));
        category.CreatedAt.Should().NotBeSameDateAs(default);
        category.CreatedAt.Should().BeAfter(dateTimeBefore);
        category.CreatedAt.Should().BeBefore(dateTimeAfter);
        category.IsActive.Should().Be(isActive);
    }

    [Theory(DisplayName = nameof(Instantiate_Error_When_Name_Is_Empty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Instantiate_Error_When_Name_Is_Empty(string? name)
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        Action action =
            () => new DomainEntity.Category(name!, validCategory.Description);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }

    [Fact(DisplayName = nameof(Instantiate_Error_When_Description_Is_Null))]
    [Trait("Domain", "Category - Aggregates")]
    public void Instantiate_Error_When_Description_Is_Null()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();


        Action action =
            () => new DomainEntity.Category(validCategory.Name, null!);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Description should not be null");
    }

    [Theory(DisplayName = nameof(Instantiate_Error_When_Name_Has_Less_Of_Three_Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("1")]
    [InlineData("12")]
    [InlineData("a")]
    [InlineData("ca")]
    public void Instantiate_Error_When_Name_Has_Less_Of_Three_Characters(string invalidName)
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        Action action =
            () => new DomainEntity.Category(invalidName, validCategory.Description);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be at least 3 characters long");
    }

    [Fact(DisplayName = nameof(Instantiate_Error_When_Name_Has_More_Than_255_Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void Instantiate_Error_When_Name_Has_More_Than_255_Characters()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        var invalidName = String.Join(null, Enumerable.Range(1, 256).Select(_ => "a").ToArray());

        Action action =
            () => new DomainEntity.Category(invalidName, validCategory.Description);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be less or equal to 255 characters long");
    }

    [Fact(DisplayName = nameof(Instantiate_Error_When_Description_Has_More_Than_10000_Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void Instantiate_Error_When_Description_Has_More_Than_10000_Characters()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        var invalidDescription = String.Join(null, Enumerable.Range(1, 10001).Select(_ => "a").ToArray());

        Action action =
            () => new DomainEntity.Category(validCategory.Name, invalidDescription);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Description should be less or equal to 10000 characters long");
    }

    [Fact(DisplayName = nameof(Activate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Activate()
    {
        //Arrange
        var validCategory = _categoryTestFixture.GetValidCategory();

        //Act
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, false);
        category.Activate();

        //Assert
        category.IsActive.Should().BeTrue();
    }

    [Fact(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Deactivate()
    {
        //Arrange
        var validCategory = _categoryTestFixture.GetValidCategory();

        //Act
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, true);
        category.Deactivate();

        //Assert
        category.IsActive.Should().BeFalse();
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Category - Aggregates")]
    public void Update()
    {
        var category = _categoryTestFixture.GetValidCategory();
        var newValues = new { Name = "New name", Description = "New Description" };

        category.Update(newValues.Name, newValues.Description);

        category.Name.Should().Be(newValues.Name);
        category.Description.Should().Be(newValues.Description);
    }

    [Fact(DisplayName = nameof(Update_Only_Name))]
    [Trait("Domain", "Category - Aggregates")]
    public void Update_Only_Name()
    {
        var category = _categoryTestFixture.GetValidCategory();
        var newValues = new { Name = "New name" };
        var currentDescription = category.Description;

        category.Update(newValues.Name);

        category.Name.Should().Be(newValues.Name);
        category.Description.Should().Be(currentDescription);
    }


    [Theory(DisplayName = nameof(Update_Error_When_Name_IsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Update_Error_When_Name_IsEmpty(string? name)
    {
        var category = _categoryTestFixture.GetValidCategory();

        Action action =
            () => category.Update(name!);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }

    [Theory(DisplayName = nameof(Update_Error_When_Name_Has_Less_Of_Three_Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("1")]
    [InlineData("12")]
    [InlineData("a")]
    [InlineData("ca")]
    public void Update_Error_When_Name_Has_Less_Of_Three_Characters(string invalidName)
    {
        var category = _categoryTestFixture.GetValidCategory();

        Action action =
            () => category.Update(invalidName);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be at least 3 characters long");
    }

    [Fact(DisplayName = nameof(Update_Error_When_Name_Has_More_Than_255_Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void Update_Error_When_Name_Has_More_Than_255_Characters()
    {
        var category = _categoryTestFixture.GetValidCategory();
        var invalidName = String.Join(null, Enumerable.Range(1, 256).Select(_ => "a").ToArray());

        Action action =
            () => category.Update(invalidName);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be less or equal to 255 characters long");
    }

    [Fact(DisplayName = nameof(Update_Error_When_Description_Has_More_Than_10000_Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void Update_Error_When_Description_Has_More_Than_10000_Characters()
    {
        var category = _categoryTestFixture.GetValidCategory();
        var invalidDescription = String.Join(null, Enumerable.Range(1, 10001).Select(_ => "a").ToArray());

        Action action =
            () => category.Update("Category Name ok", invalidDescription);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Description should be less or equal to 10000 characters long");
    }
}
