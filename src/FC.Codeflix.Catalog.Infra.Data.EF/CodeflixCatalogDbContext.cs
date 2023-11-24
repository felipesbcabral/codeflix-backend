﻿using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Infra.Data.EF.Configurations;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF;
public class CodeflixCatalogDbContext : DbContext
{
    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Genre> Genres => Set<Genre>();

    public DbSet<GenresCategories> GenresCategories
        => Set<GenresCategories>();

    public CodeflixCatalogDbContext(DbContextOptions<CodeflixCatalogDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new GenreConfigurations());

        modelBuilder.ApplyConfiguration(new GenresCategoriesConfigurations());

    }
}
