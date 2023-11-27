using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
public class GenreRepository : IGenreRepository
{
    private readonly CodeflixCatalogDbContext _context;

    private DbSet<Genre> _genres
        => _context.Set<Genre>();

    private DbSet<GenresCategories> _genresCategories
        => _context.Set<GenresCategories>();

    public GenreRepository(CodeflixCatalogDbContext context)
        => _context = context;

    public async Task Insert(Genre genre, CancellationToken cancellationToken)
    {
        await _genres.AddAsync(genre);
        if (genre.Categories.Count > 0)
        {
            var relatedCategories = genre.Categories
                .Select(categoryId => new GenresCategories(
                    categoryId,
                    genre.Id
                ));
            await _genresCategories.AddRangeAsync(relatedCategories, cancellationToken);
        }
    }
    public async Task<Genre> Get(Guid id, CancellationToken cancellationToken)
    {
        var genre = await _genres.AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
        NotFoundException.ThrowIfNull(genre, $"Genre '{id}' not found.");
        var cetegoryIds = await _genresCategories
            .Where(gc => gc.GenreId == genre.Id)
            .Select(gc => gc.CategoryId)
            .ToListAsync(cancellationToken);
        cetegoryIds.ForEach(genre.AddCategory);

        return genre;
    }

    public Task Delete(Genre aggregate, CancellationToken cancellationToken)
    {
        _genresCategories.RemoveRange(
                       _genresCategories.Where(gc => gc.GenreId == aggregate.Id));
        _genres.Remove(aggregate);

        return Task.CompletedTask;
    }
    public async Task Update(Genre genre, CancellationToken cancellationToken)
    {
        _genres.Update(genre);
        _genresCategories.RemoveRange(
            _genresCategories.Where(gc => gc.GenreId == genre.Id));
        if (genre.Categories.Count > 0)
        {
            var relatedCategories = genre.Categories
                .Select(categoryId => new GenresCategories(
                    categoryId,
                    genre.Id
                ));
            await _genresCategories.AddRangeAsync(relatedCategories, cancellationToken);
        }
    }

    public Task<SearchOutput<Genre>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

}
