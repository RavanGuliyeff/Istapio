using Istapio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Istapio.Persistence.Configurations;

public sealed class JobPostSkillConfiguration : IEntityTypeConfiguration<JobPostSkill>
{
    public void Configure(EntityTypeBuilder<JobPostSkill> builder)
    {
        builder.HasKey(x => new { x.JobPostId, x.SkillId });

        builder.Property(x => x.JobPostId)
            .IsRequired();

        builder.Property(x => x.SkillId)
            .IsRequired();

        builder.HasOne(x => x.JobPost)
            .WithMany(x => x.RequiredSkills)
            .HasForeignKey(x => x.JobPostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Skill)
            .WithMany(x => x.JobPostSkills)
            .HasForeignKey(x => x.SkillId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.SkillId);
    }
}