using Istapio.Domain.Entities;
using Istapio.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Istapio.Persistence.Seeders;

public static class VacationTypeSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.VacationTypes.AnyAsync())
            return;

        var vacationTypes = new List<VacationType>
        {
            new() { Name = "Full Time" },
            new() { Name = "Part Time" },
            new() { Name = "Remote" },
            new() { Name = "Hybrid" },
            new() { Name = "On-site" },
            new() { Name = "Internship" },
            new() { Name = "Contract" },
            new() { Name = "Freelance" },
            new() { Name = "Temporary" },
            new() { Name = "Seasonal" },
            new() { Name = "Volunteer" },
            new() { Name = "Apprenticeship" }
        };

        await context.VacationTypes.AddRangeAsync(vacationTypes);
        await context.SaveChangesAsync();
    }
}