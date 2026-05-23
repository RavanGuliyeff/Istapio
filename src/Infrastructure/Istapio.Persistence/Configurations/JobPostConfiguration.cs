using Istapio.Domain.Entities;
using Istapio.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Istapio.Persistence.Configurations;

public sealed class JobPostConfiguration : BaseAuditableEntityConfiguration<JobPost>
{
    public override void Configure(EntityTypeBuilder<JobPost> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(x => x.Requirements)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.ViewCount)
            .IsRequired();

        builder.Property(x => x.LastDate);

        builder.Property(x => x.CompanyId)
            .IsRequired();

        builder.Property(x => x.CategoryId)
            .IsRequired();

        builder.HasOne(x => x.Company)
            .WithMany(x => x.JobPosts)
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.JobPosts)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.RequiredSkills)
            .WithOne(x => x.JobPost)
            .HasForeignKey(x => x.JobPostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.CompanyId);
        builder.HasIndex(x => x.CategoryId);
    }
}