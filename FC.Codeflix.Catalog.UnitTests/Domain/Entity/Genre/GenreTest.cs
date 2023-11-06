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
        genre.Name.Should().Be(newName);
        genre.IsActive.Should().Be(oldIsActive);
        genre.CreatedAt.Should().NotBeSameDateAs(default);
    }
}
