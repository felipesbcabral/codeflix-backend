using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.DeleteGenre;
public class DeleteGenreInput : IRequest<Unit>
{
    public DeleteGenreInput(Guid id) => Id = id;

    public Guid Id { get; set; }
}