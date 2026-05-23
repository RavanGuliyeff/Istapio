using Istapio.Domain.Entities;
using Istapio.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Istapio.Persistence.Configurations;

public sealed class VacationTypeConfiguration : BaseEntityConfiguration<VacationType>
{
    public override void Configure(EntityTypeBuilder<VacationType> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);
    }
}