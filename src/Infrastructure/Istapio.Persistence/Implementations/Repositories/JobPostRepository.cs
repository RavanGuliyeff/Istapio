using Istapio.Domain.Entities;
using Istapio.Domain.Interfaces;
using Istapio.Domain.Interfaces.Repositories;
using Istapio.Persistence.Contexts;
using Istapio.Persistence.Implementations.Generics;

namespace Istapio.Persistence.Implementations.Repositories;

public sealed class JobPostRepository : AuditableRepository<JobPost>, IJobPostRepository
{
    public JobPostRepository(AppDbContext context)
        : base(context)
    {
    }
}