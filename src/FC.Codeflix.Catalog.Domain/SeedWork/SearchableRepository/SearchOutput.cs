namespace FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
public class SearchOutput<TAggegate>
    where TAggegate : AggregateRoot
{
    public int CurrentPage { get; set; }

    public int PerPage { get; set; }

    public int Total { get; set; }

    public IReadOnlyList<TAggegate> Items { get; set; }

    public SearchOutput(
        int currentPage,
        int perPage,
        int total,
        IReadOnlyList<TAggegate> items)
    {
        CurrentPage = currentPage;
        PerPage = perPage;
        Total = total;
        Items = items;
    }
}
