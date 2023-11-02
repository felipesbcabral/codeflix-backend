﻿using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.ListCategories;

[CollectionDefinition(nameof(ListCategoriesApiTestFixture))]
public class ListCategoryApiTestFixtureCollection : ICollectionFixture<ListCategoriesApiTestFixture> { }

public class ListCategoriesApiTestFixture : CategoryBaseFixture
{
    public List<DomainEntity.Category> GetExampleCategoriesListWithNames(List<string> names)
     => names.Select(name =>
     {
         var category = GetExampleCategory();
         category.Update(name);
         return category;
     }).ToList();

    public List<DomainEntity.Category> CloneCategoriesListOrdered(
        List<DomainEntity.Category> categoriesList,
        string orderBy,
        SearchOrder order)
    {
        var listClone = new List<DomainEntity.Category>(categoriesList);
        var orderedEnumerable = (orderBy.ToLower(), order) switch
        {
            ("name", SearchOrder.Asc) => listClone.OrderBy(x => x.Name),
            ("name", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Name),
            ("id", SearchOrder.Asc) => listClone.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => listClone.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.Desc) => listClone.OrderByDescending(x => x.CreatedAt),
            _ => listClone.OrderBy(x => x.Name)
        };

        return orderedEnumerable.ToList();
    }
}