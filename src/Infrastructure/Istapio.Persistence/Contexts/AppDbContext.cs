using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Istapio.Domain.Entities;
using Istapio.Domain.Entities.Common;
using Istapio.Domain.Interfaces;
using System.Linq.Expressions;
using System.Reflection;

namespace Istapio.Persistence.Contexts;

public class AppDbContext : IdentityDbContext<AppUser>
{
    private readonly ICurrentUserService _currentUserService;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        ICurrentUserService currentUserService) : base(options)
    {
        _currentUserService = currentUserService;
    }

    // DbSets
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<JobPost> JobPosts => Set<JobPost>();
    public DbSet<JobPostSkill> JobPostSkills => Set<JobPostSkill>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Setting> Settings => Set<Setting>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<UserSkill> UserSkills => Set<UserSkill>();
    public DbSet<VacationType> VacationTypes => Set<VacationType>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        ApplyGlobalQueryFilters(modelBuilder);

        //ConfigureIdentityTables(modelBuilder);

        RenameIdentityTables(modelBuilder);
    }

    private static void ApplyGlobalQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(BaseAuditableEntity).IsAssignableFrom(entityType.ClrType))
                continue;

            var parameter = Expression.Parameter(entityType.ClrType, "e");

            var body = Expression.Equal(
                Expression.Property(parameter, nameof(BaseAuditableEntity.IsDeleted)),
                Expression.Constant(false)
            );

            var lambdaType = typeof(Func<,>).MakeGenericType(entityType.ClrType, typeof(bool));
            var lambda = Expression.Lambda(lambdaType, body, parameter);

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
        }
    }


    private static void RenameIdentityTables(ModelBuilder modelBuilder)
    {
        // Sadəcə adları dəyişir, konfiqurasiyaya toxunmur
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (tableName?.StartsWith("AspNet") == true)
            {
                entity.SetTableName(tableName.Replace("AspNet", ""));
            }
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateAuditableEntities();
        return base.SaveChanges();
    }

    private void UpdateAuditableEntities()
    {
        var entries = ChangeTracker.Entries<BaseAuditableEntity>();
        var currentUser = _currentUserService.UserId;

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = currentUser;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = currentUser;

                    if (entry.Entity.IsDeleted && entry.Entity.DeletedBy == null)
                    {
                        entry.Entity.DeletedBy = currentUser;
                        entry.Entity.DeletedAt = DateTime.UtcNow;
                    }

                    else if (!entry.Entity.IsDeleted && entry.Entity.DeletedBy != null)
                    {
                        entry.Entity.DeletedBy = null;
                        entry.Entity.DeletedAt = null;
                    }

                    break;
                //case EntityState.Deleted:
                //    entry.State = EntityState.Modified;
                //    entry.Entity.IsDeleted = true;
                //    entry.Entity.DeletedAt = DateTime.UtcNow;
                //    entry.Entity.DeletedBy = currentUser;
                //    break;
            }
        }
    }
}
