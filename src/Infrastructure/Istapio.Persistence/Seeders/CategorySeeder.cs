using Istapio.Domain.Entities;
using Istapio.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Istapio.Persistence.Seeders;

public static class CategorySeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Categories.AnyAsync())
            return;

        var technology = new Category { Name = "Technology" };
        var business = new Category { Name = "Business" };
        var marketing = new Category { Name = "Marketing" };
        var finance = new Category { Name = "Finance" };
        var engineering = new Category { Name = "Engineering" };
        var design = new Category { Name = "Design" };
        var customerService = new Category { Name = "Customer Service" };
        var education = new Category { Name = "Education" };
        var healthcare = new Category { Name = "Healthcare" };
        var legal = new Category { Name = "Legal" };
        var logistics = new Category { Name = "Logistics" };
        var media = new Category { Name = "Media & Communication" };
        var hospitality = new Category { Name = "Hospitality & Tourism" };
        var manufacturing = new Category { Name = "Manufacturing" };
        var retail = new Category { Name = "Retail & E-Commerce" };
        var construction = new Category { Name = "Construction" };
        var agriculture = new Category { Name = "Agriculture" };

        await context.Categories.AddRangeAsync(
            technology,
            business,
            marketing,
            finance,
            engineering,
            design,
            customerService,
            education,
            healthcare,
            legal,
            logistics,
            media,
            hospitality,
            manufacturing,
            retail,
            construction,
            agriculture
        );

        await context.SaveChangesAsync();

        var categories = new List<Category>
        {
            // Technology
            new() { Name = "Backend Development", ParentId = technology.Id },
            new() { Name = "Frontend Development", ParentId = technology.Id },
            new() { Name = "Full Stack Development", ParentId = technology.Id },
            new() { Name = "Mobile Development", ParentId = technology.Id },
            new() { Name = "Game Development", ParentId = technology.Id },
            new() { Name = "Desktop Development", ParentId = technology.Id },
            new() { Name = "Embedded Systems", ParentId = technology.Id },
            new() { Name = "DevOps", ParentId = technology.Id },
            new() { Name = "Site Reliability Engineering", ParentId = technology.Id },
            new() { Name = "Cloud Computing", ParentId = technology.Id },
            new() { Name = "Cyber Security", ParentId = technology.Id },
            new() { Name = "Network Administration", ParentId = technology.Id },
            new() { Name = "System Administration", ParentId = technology.Id },
            new() { Name = "IT Support", ParentId = technology.Id },
            new() { Name = "Database Administration", ParentId = technology.Id },
            new() { Name = "Data Engineering", ParentId = technology.Id },
            new() { Name = "Data Science", ParentId = technology.Id },
            new() { Name = "Artificial Intelligence", ParentId = technology.Id },
            new() { Name = "Machine Learning", ParentId = technology.Id },
            new() { Name = "Business Intelligence", ParentId = technology.Id },
            new() { Name = "Blockchain", ParentId = technology.Id },
            new() { Name = "Internet of Things", ParentId = technology.Id },
            new() { Name = "ERP Development", ParentId = technology.Id },
            new() { Name = "CRM Development", ParentId = technology.Id },
            new() { Name = "Quality Assurance", ParentId = technology.Id },
            new() { Name = "Automation Testing", ParentId = technology.Id },
            new() { Name = "Manual Testing", ParentId = technology.Id },
            new() { Name = "Software Architecture", ParentId = technology.Id },
            new() { Name = "API Development", ParentId = technology.Id },
            new() { Name = "Software Integration", ParentId = technology.Id },
            new() { Name = "IT Infrastructure", ParentId = technology.Id },
        
            // Business
            new() { Name = "Human Resources", ParentId = business.Id },
            new() { Name = "Recruitment", ParentId = business.Id },
            new() { Name = "Marketing", ParentId = business.Id },
            new() { Name = "Digital Marketing", ParentId = business.Id },
            new() { Name = "Content Marketing", ParentId = business.Id },
            new() { Name = "Social Media Management", ParentId = business.Id },
            new() { Name = "SEO", ParentId = business.Id },
            new() { Name = "Sales", ParentId = business.Id },
            new() { Name = "Business Development", ParentId = business.Id },
            new() { Name = "Business Analysis", ParentId = business.Id },
            new() { Name = "Project Management", ParentId = business.Id },
            new() { Name = "Product Management", ParentId = business.Id },
            new() { Name = "Operations Management", ParentId = business.Id },
            new() { Name = "Procurement", ParentId = business.Id },
            new() { Name = "Administration", ParentId = business.Id },
            // Engineering
            new() { Name = "Civil Engineering", ParentId = engineering.Id },
            new() { Name = "Mechanical Engineering", ParentId = engineering.Id },
            new() { Name = "Electrical Engineering", ParentId = engineering.Id },
            new() { Name = "Industrial Engineering", ParentId = engineering.Id },
            new() { Name = "Petroleum Engineering", ParentId = engineering.Id },
            new() { Name = "Chemical Engineering", ParentId = engineering.Id },
            new() { Name = "Environmental Engineering", ParentId = engineering.Id },
            new() { Name = "Construction Management", ParentId = engineering.Id },
            new() { Name = "Mining Engineering", ParentId = engineering.Id },
            new() { Name = "Automation Engineering", ParentId = engineering.Id },
        
            // Design
            new() { Name = "UI/UX Design", ParentId = design.Id },
            new() { Name = "Graphic Design", ParentId = design.Id },
            new() { Name = "Motion Design", ParentId = design.Id },
            new() { Name = "Product Design", ParentId = design.Id },
            new() { Name = "3D Design", ParentId = design.Id },
            new() { Name = "Interior Design", ParentId = design.Id },
            new() { Name = "Fashion Design", ParentId = design.Id },
            new() { Name = "Illustration", ParentId = design.Id },
        
            // Customer Service
            new() { Name = "Call Center", ParentId = customerService.Id },
            new() { Name = "Technical Support", ParentId = customerService.Id },
            new() { Name = "Customer Relationship Management", ParentId = customerService.Id },
            new() { Name = "Customer Support", ParentId = customerService.Id },
            new() { Name = "Customer Success", ParentId = customerService.Id },
            new() { Name = "Client Relations", ParentId = customerService.Id },
        
            // Education
            new() { Name = "Teaching", ParentId = education.Id },
            new() { Name = "Academic Research", ParentId = education.Id },
            new() { Name = "Training & Development", ParentId = education.Id },
            new() { Name = "Curriculum Development", ParentId = education.Id },
            new() { Name = "Educational Management", ParentId = education.Id },
        
        
            // Healthcare
            new() { Name = "Medicine", ParentId = healthcare.Id },
            new() { Name = "Nursing", ParentId = healthcare.Id },
            new() { Name = "Pharmacy", ParentId = healthcare.Id },
            new() { Name = "Dentistry", ParentId = healthcare.Id },
            new() { Name = "Medical Laboratory", ParentId = healthcare.Id },
            new() { Name = "Radiology", ParentId = healthcare.Id },
            new() { Name = "Physiotherapy", ParentId = healthcare.Id },
            new() { Name = "Emergency Medicine", ParentId = healthcare.Id },
        
            // Legal
            new() { Name = "Legal Consulting", ParentId = legal.Id },
            new() { Name = "Corporate Law", ParentId = legal.Id },
            new() { Name = "Compliance", ParentId = legal.Id },
            new() { Name = "Contract Management", ParentId = legal.Id },
        
            // Logistics
            new() { Name = "Supply Chain", ParentId = logistics.Id },
            new() { Name = "Warehouse Management", ParentId = logistics.Id },
            new() { Name = "Transportation", ParentId = logistics.Id },
            new() { Name = "Import & Export", ParentId = logistics.Id },
            new() { Name = "Inventory Management", ParentId = logistics.Id },
        
            // Media & Communication
            new() { Name = "Journalism", ParentId = media.Id },
            new() { Name = "Content Creation", ParentId = media.Id },
            new() { Name = "Public Relations", ParentId = media.Id },
            new() { Name = "Copywriting", ParentId = media.Id },
            new() { Name = "Video Production", ParentId = media.Id },
            new() { Name = "Photography", ParentId = media.Id },
            new() { Name = "Broadcasting", ParentId = media.Id },
            new() { Name = "Podcast Production", ParentId = media.Id },
            new() { Name = "Media Planning", ParentId = media.Id },
            
        
            //Marketing
           
            new() { Name = "SEM", ParentId = marketing.Id },
            new() { Name = "Brand Management", ParentId = marketing.Id },
            new() { Name = "Market Research", ParentId = marketing.Id },
        
        
            //Finance
            new() { Name = "Accounting", ParentId = finance.Id },
            new() { Name = "Auditing", ParentId = finance.Id },
            new() { Name = "Financial Analysis", ParentId = finance.Id },
            new() { Name = "Investment", ParentId = finance.Id },
            new() { Name = "Tax", ParentId = finance.Id },
            new() { Name = "Payroll", ParentId = finance.Id },
            new() { Name = "Risk Management", ParentId = finance.Id },
            new() { Name = "Insurance", ParentId = finance.Id },
        
        
            // Hospitality & Tourism
            new() { Name = "Hotel Management", ParentId = hospitality.Id },
            new() { Name = "Restaurant Management", ParentId = hospitality.Id },
            new() { Name = "Travel Consultant", ParentId = hospitality.Id },
            new() { Name = "Event Management", ParentId = hospitality.Id },
            new() { Name = "Front Office", ParentId = hospitality.Id },
            new() { Name = "Housekeeping", ParentId = hospitality.Id },
        
        // Manufacturing
            new() { Name = "Production", ParentId = manufacturing.Id },
            new() { Name = "Quality Control", ParentId = manufacturing.Id },
            new() { Name = "Maintenance", ParentId = manufacturing.Id },
            new() { Name = "Process Engineering", ParentId = manufacturing.Id },
        
            // Retail & E-Commerce
            new() { Name = "Retail Management", ParentId = retail.Id },
            new() { Name = "Merchandising", ParentId = retail.Id },
            new() { Name = "Cashier", ParentId = retail.Id },
            new() { Name = "E-Commerce", ParentId = retail.Id },
            new() { Name = "Store Operations", ParentId = retail.Id },
            new() { Name = "Inventory Control", ParentId = retail.Id },
            new() { Name = "Visual Merchandising", ParentId = retail.Id },
        
        
            // Construction
            new() { Name = "Architecture", ParentId = construction.Id },
            new() { Name = "Site Supervision", ParentId = construction.Id },
            new() { Name = "Quantity Surveying", ParentId = construction.Id },
            new() { Name = "Building Inspection", ParentId = construction.Id },
        
        
            // agriculture
            new() { Name = "Agronomy", ParentId = agriculture.Id },
            new() { Name = "Farming", ParentId = agriculture.Id },
            new() { Name = "Veterinary", ParentId = agriculture.Id },
            new() { Name = "Food Production", ParentId = agriculture.Id },
        };

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
    }
}
