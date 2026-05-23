using Istapio.Domain.Entities;
using Istapio.Domain.Interfaces;
using Istapio.Domain.Interfaces.Repositories;
using Istapio.Persistence.Contexts;
using Istapio.Persistence.Implementations.Generics;

namespace Istapio.Persistence.Implementations.Repositories;

public sealed class CompanyRepository : AuditableRepository<Company>, ICompanyRepository
{
    public CompanyRepository(AppDbContext context, ICurrentUserService currentUserService)
        : base(context, currentUserService)
    {
    }
}