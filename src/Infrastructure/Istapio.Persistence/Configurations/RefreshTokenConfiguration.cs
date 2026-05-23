using Istapio.Domain.Entities;
using Istapio.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Istapio.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : BaseEntityConfiguration<RefreshToken>
{
    public override void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Token)
            .IsRequired();

        builder.Property(x => x.ExpiresAt)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedByIp)
            .IsRequired();

        builder.Property(x => x.RevokedByIp);

        builder.Property(x => x.ReplacedByToken);

        builder.Property(x => x.ReasonRevoked);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.HasIndex(x => x.UserId);
    }
}