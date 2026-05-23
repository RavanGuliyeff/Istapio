using Istapio.Domain.Entities;
using Istapio.Domain.Interfaces.Repositories;
using Istapio.Persistence.Contexts;
using Istapio.Persistence.Implementations.Generics;

namespace Istapio.Persistence.Implementations.Repositories;

public sealed class VacationTypeRepository : Repository<VacationType>, IVacationTypeRepository
{
    public VacationTypeRepository(AppDbContext context) : base(context)
    {
    }
}