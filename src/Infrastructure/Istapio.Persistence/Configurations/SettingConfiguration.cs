using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Istapio.Domain.Entities;

namespace Istapio.Persistence.Configurations;

public class SettingConfiguration : BaseAuditableEntityConfiguration<Setting>
{
    public override void Configure(EntityTypeBuilder<Setting> builder)
    {
        base.Configure(builder);

        builder.Property(s => s.Key)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Value)
            .IsRequired()
            .HasMaxLength(2000);

        builder.HasIndex(s => s.Key)
            .IsUnique()
            .HasDatabaseName("IX_Settings_Key");
    }
}
