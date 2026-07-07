using Istapio.Domain.Entities;
using Istapio.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Istapio.Persistence.Seeders;

public static class SettingSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Settings.AnyAsync())
            return;

        var settings = new List<Setting>
        {
            new() { Key = "SiteName", Value = "Istapio" },
            new() { Key = "SiteLogo", Value = "Istapio.png" },
            new() { Key = "SiteDescription", Value = "Find your next career opportunity." },
            new() { Key = "SupportEmail", Value = "support@istapio.com" },
            new() { Key = "SupportPhone", Value = "+994501234567" },

            new() { Key = "FacebookUrl", Value = "" },
            new() { Key = "InstagramUrl", Value = "" },
            new() { Key = "LinkedInUrl", Value = "" },
            new() { Key = "TwitterUrl", Value = "" },

            new() { Key = "DefaultLanguage", Value = "en" },
            new() { Key = "DefaultTheme", Value = "light" },

            new() { Key = "MaintenanceMode", Value = "false" },
            new() { Key = "MaintenanceMessage", Value = "The system is under maintenance." },

            new() { Key = "EmailVerificationRequired", Value = "true" },
            new() { Key = "CompanyApprovalRequired", Value = "true" },
            new() { Key = "JobApprovalRequired", Value = "true" },

            new() { Key = "DefaultPageSize", Value = "10" },
            new() { Key = "MaximumPageSize", Value = "100" },

            new() { Key = "MaxLogoSizeMB", Value = "5" },
            new() { Key = "MaxResumeSizeMB", Value = "10" },
            new() { Key = "AllowedFileExtensions", Value = ".pdf,.doc,.docx" },

            new() { Key = "DefaultJobExpireDays", Value = "30" },
            new() { Key = "MaxJobDurationDays", Value = "90" },

            new() { Key = "EnableCompanyProfiles", Value = "true" },
            new() { Key = "EnablePublicProfiles", Value = "true" },
            new() { Key = "EnableJobAlerts", Value = "true" },
            new() { Key = "EnableNotifications", Value = "true" },

            new() { Key = "CacheDurationMinutes", Value = "10" },
            new() { Key = "Version", Value = "1.0.0" }
        };

        await context.Settings.AddRangeAsync(settings);
        await context.SaveChangesAsync();
    }
}