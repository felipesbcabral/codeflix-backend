using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Genre;

[Collection(nameof(GenreTestFixture))]
public class GenreTest
{
    private readonly GenreTestFixture _fixture;

    public GenreTest(GenreTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Genre - Aggregates")]
    public void Instantiate()
    {
        var genreName = _fixture.GetValidName();

        var dateTimeBefore = DateTime.Now;

        var genre = new DomainEntity.Genre(genreName);

        Thread.Sleep(1);

        var dateTimeAfter = DateTime.Now;

        genre.Should().NotBeNull();
        genre.Id.Should().NotBeEmpty();
        genre.Name.Should().Be(genreName);
        genre.IsActive.Should().BeTrue();
        genre.CreatedAt.Should().NotBeSameDateAs(default);
        genre.CreatedAt.Should().BeAfter(dateTimeBefore);
        genre.CreatedAt.Should().BeBefore(dateTimeAfter);
    }

    [Theory(DisplayName = nameof(Instantiate_With_Is_Active))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void Instantiate_With_Is_Active(bool isActive)
    {
        var genreName = _fixture.GetValidName();

        var dateTimeBefore = DateTime.Now;

        var genre = new DomainEntity.Genre(genreName, isActive);

        Thread.Sleep(1);

        var dateTimeAfter = DateTime.Now;

        genre.Should().NotBeNull();
        genre.Id.Should().NotBeEmpty();
        genre.Name.Should().Be(genreName);
        genre.IsActive.Should().Be(isActive);
        genre.CreatedAt.Should().NotBeSameDateAs(default);
        genre.CreatedAt.Should().BeAfter(dateTimeBefore);
        genre.CreatedAt.Should().BeBefore(dateTimeAfter);
    }

    [Theory(DisplayName = nameof(Activate))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void Activate(bool isActive)
    {
        var genre = _fixture.GetExampleGenre(isActive);
        var oldName = genre.Name;

        genre.Activate();

        genre.Should().NotBeNull();
        genre.Id.Should().NotBeEmpty();
        genre.IsActive.Should().BeTrue();
        genre.Name.Should().Be(oldName);
        genre.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Theory(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void Deactivate(bool isActive)
    {
        var genre = _fixture.GetExampleGenre(isActive);
        var oldName = genre.Name;

        genre.Deactivate();

        genre.Should().NotBeNull();
        genre.Id.Should().NotBeEmpty();
        genre.IsActive.Should().BeFalse();
        genre.Name.Should().Be(oldName);
        genre.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Genre - Aggregates")]
    public void Update()
    {
        var genre = _fixture.GetExampleGenre();
        var newName = _fixture.GetValidName();
        var oldIsActive = genre.IsActive;

        genre.Update(newName);

        genre.Should().NotBeNull();
        genre.Id.Should().NotBeEmpty();
        genre.Name.Should().Be(newName);
        genre.IsActive.Should().Be(oldIsActive);
        genre.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Theory(DisplayName = nameof(Instantiate_Throw_When_Name_Empty))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Instantiate_Throw_When_Name_Empty(string? name)
    {
        var action =
            () => new DomainEntity.Genre(name!);

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }

    [Theory(DisplayName = nameof(Instantiate_Throw_When_Name_Empty))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Update_Throw_When_Name_Empty(string? name)
    {
        var genre = _fixture.GetExampleGenre();

        var action =
            () => genre.Update(name!);

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }

    [Fact(DisplayName = nameof(AddMultipleCategories))]
    [Trait("Domain", "Genre - Aggregates")]
    public void AddMultipleCategories()
    {
        var genre = _fixture.GetExampleGenre();
        var categoryGuid1 = Guid.NewGuid();
        var categoryGuid2 = Guid.NewGuid();

        genre.AddCategory(categoryGuid1);
        genre.AddCategory(categoryGuid2);

        genre.Categories.Should().HaveCount(2);
        genre.Categories.Should().Contain(categoryGuid1);
        genre.Categories.Should().Contain(categoryGuid2);
    }

    [Fact(DisplayName = nameof(AddCategory))]
    [Trait("Domain", "Genre - Aggregates")]
    public void AddCategory()
    {
        var genre = _fixture.GetExampleGenre();
        var categoryGuid = Guid.NewGuid();

        genre.AddCategory(categoryGuid);

        genre.Categories.Should().HaveCount(1);
        genre.Categories.Should().Contain(categoryGuid);
    }

    [Fact(DisplayName = nameof(Remove_Category))]
    [Trait("Domain", "Genre - Aggregates")]
    public void Remove_Category()
    {
        var exampleGuid = Guid.NewGuid();
        var genre = _fixture.GetExampleGenre(
            categoriesIdLisList: new List<Guid>()
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                exampleGuid,
                Guid.NewGuid(),
                Guid.NewGuid()
            });

        genre.RemoveCategory(exampleGuid);

        genre.Categories.Should().HaveCount(4);
        genre.Categories.Should().NotContain(exampleGuid);
    }

    [Fact(DisplayName = nameof(Remove_All_Categories))]
    [Trait("Domain", "Genre - Aggregates")]
    public void Remove_All_Categories()
    {
        var genre = _fixture.GetExampleGenre(
            categoriesIdLisList: new List<Guid>()
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()
            });

        genre.RemoveAllCategories();

        genre.Categories.Should().HaveCount(0);
    }
}
