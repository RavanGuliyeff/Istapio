using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Istapio.Domain.Entities.Common;

namespace Istapio.Persistence.Configurations;

public abstract class BaseAuditableEntityConfiguration<TEntity> : BaseEntityConfiguration<TEntity>
    where TEntity : BaseAuditableEntity
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        // Audit fields
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(128);

        builder.Property(e => e.UpdatedAt);

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(128);

        builder.Property(e => e.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.DeletedAt);

        builder.Property(e => e.DeletedBy)
            .HasMaxLength(128);

    }
}
