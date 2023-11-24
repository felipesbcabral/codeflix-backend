using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Configurations;
internal class GenresCategoriesConfigurations : IEntityTypeConfiguration<GenresCategories>
{
    public void Configure(EntityTypeBuilder<GenresCategories> builder)
    {
        builder.HasKey(category => new
        {
            category.CategoryId,
            category.GenreId
        });

    }
}
