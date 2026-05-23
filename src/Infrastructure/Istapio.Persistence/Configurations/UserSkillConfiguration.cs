using Istapio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Istapio.Persistence.Configurations;

public sealed class UserSkillConfiguration : IEntityTypeConfiguration<UserSkill>
{
    public void Configure(EntityTypeBuilder<UserSkill> builder)
    {
        builder.HasKey(x => new { x.UserId, x.SkillId });

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.SkillId)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.Skills)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Skill)
            .WithMany(x => x.UserSkills)
            .HasForeignKey(x => x.SkillId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.SkillId);
    }
}