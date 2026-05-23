using Istapio.Domain.Entities;
using Istapio.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Istapio.Persistence.Configurations;

public sealed class SettingConfiguration : BaseAuditableEntityConfiguration<Setting>
{
    public override void Configure(EntityTypeBuilder<Setting> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Key)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Value)
            .IsRequired()
            .HasMaxLength(2000);

        builder.HasIndex(x => x.Key)
            .IsUnique()
            .HasDatabaseName("IX_Settings_Key");
    }
}
