using Istapio.Domain.Entities;
using Istapio.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Istapio.Persistence.Seeders;

public static class SkillSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Skills.AnyAsync())
            return;

        var skills = new List<Skill>
        {
            // Programming Languages
            new() { Name = "C#" },
            new() { Name = ".NET" },
            new() { Name = "ASP.NET Core" },
            new() { Name = "Java" },
            new() { Name = "Spring Boot" },
            new() { Name = "Python" },
            new() { Name = "Django" },
            new() { Name = "Flask" },
            new() { Name = "FastAPI" },
            new() { Name = "JavaScript" },
            new() { Name = "TypeScript" },
            new() { Name = "Node.js" },
            new() { Name = "Express.js" },
            new() { Name = "PHP" },
            new() { Name = "Laravel" },
            new() { Name = "Go" },
            new() { Name = "Rust" },
            new() { Name = "C++" },
            new() { Name = "C" },
            new() { Name = "Kotlin" },
            new() { Name = "Swift" },
            new() { Name = "Dart" },
            new() { Name = "Ruby" },
            new() { Name = "Ruby on Rails" },
            new() { Name = "Scala" },
        
            // Frontend
            new() { Name = "HTML5" },
            new() { Name = "CSS3" },
            new() { Name = "SASS" },
            new() { Name = "Bootstrap" },
            new() { Name = "Tailwind CSS" },
            new() { Name = "React" },
            new() { Name = "Next.js" },
            new() { Name = "Angular" },
            new() { Name = "Vue.js" },
            new() { Name = "Nuxt.js" },
            new() { Name = "Redux" },
            new() { Name = "jQuery" },
        
            // Mobile
            new() { Name = "Flutter" },
            new() { Name = "React Native" },
            new() { Name = "Android Development" },
            new() { Name = "iOS Development" },
            new() { Name = "Xamarin" },
            new() { Name = ".NET MAUI" },
        
            // Databases
            new() { Name = "SQL" },
            new() { Name = "SQL Server" },
            new() { Name = "PostgreSQL" },
            new() { Name = "MySQL" },
            new() { Name = "SQLite" },
            new() { Name = "Oracle Database" },
            new() { Name = "MongoDB" },
            new() { Name = "Redis" },
            new() { Name = "Elasticsearch" },
            new() { Name = "Firebase" },
        
            // Cloud & DevOps
            new() { Name = "Docker" },
            new() { Name = "Docker Compose" },
            new() { Name = "Kubernetes" },
            new() { Name = "Azure" },
            new() { Name = "AWS" },
            new() { Name = "Google Cloud" },
            new() { Name = "Nginx" },
            new() { Name = "Linux" },
            new() { Name = "Ubuntu" },
            new() { Name = "CI/CD" },
            new() { Name = "GitHub Actions" },
            new() { Name = "GitLab CI/CD" },
            new() { Name = "Jenkins" },
            new() { Name = "Terraform" },
            new() { Name = "Ansible" },
        
            // Backend
            new() { Name = "REST API" },
            new() { Name = "GraphQL" },
            new() { Name = "SignalR" },
            new() { Name = "gRPC" },
            new() { Name = "JWT" },
            new() { Name = "OAuth 2.0" },
            new() { Name = "OpenID Connect" },
            new() { Name = "Entity Framework Core" },
            new() { Name = "LINQ" },
            new() { Name = "Dapper" },
            new() { Name = "MediatR" },
            new() { Name = "AutoMapper" },
            new() { Name = "FluentValidation" },
            new() { Name = "Microservices" },
        
            // Messaging
            new() { Name = "RabbitMQ" },
            new() { Name = "Apache Kafka" },
            new() { Name = "Azure Service Bus" },
        
            // Version Control
            new() { Name = "Git" },
            new() { Name = "GitHub" },
            new() { Name = "GitLab" },
            new() { Name = "Bitbucket" },
        
            // Testing
            new() { Name = "Unit Testing" },
            new() { Name = "Integration Testing" },
            new() { Name = "xUnit" },
            new() { Name = "NUnit" },
            new() { Name = "MSTest" },
            new() { Name = "Selenium" },
            new() { Name = "Playwright" },
            new() { Name = "Postman" },
            new() { Name = "Swagger" },
        
            // Data & AI
            new() { Name = "Machine Learning" },
            new() { Name = "Deep Learning" },
            new() { Name = "TensorFlow" },
            new() { Name = "PyTorch" },
            new() { Name = "OpenCV" },
            new() { Name = "Pandas" },
            new() { Name = "NumPy" },
            new() { Name = "Power BI" },
            new() { Name = "Tableau" },
        
            // Design
            new() { Name = "Figma" },
            new() { Name = "Adobe XD" },
            new() { Name = "Adobe Photoshop" },
            new() { Name = "Adobe Illustrator" },
            new() { Name = "Adobe Premiere Pro" },
            new() { Name = "After Effects" },
            new() { Name = "Canva" },
            new() { Name = "Blender" },
        
            // Office
            new() { Name = "Microsoft Word" },
            new() { Name = "Microsoft Excel" },
            new() { Name = "Microsoft PowerPoint" },
            new() { Name = "Microsoft Outlook" },
        
            // Management
            new() { Name = "Agile" },
            new() { Name = "Scrum" },
            new() { Name = "Kanban" },
            new() { Name = "Jira" },
            new() { Name = "Confluence" },
            new() { Name = "Notion" },
            new() { Name = "Trello" },
        
            // Finance
            new() { Name = "Financial Analysis" },
            new() { Name = "Bookkeeping" },
            new() { Name = "Payroll Management" },
            new() { Name = "Tax Accounting" },
            new() { Name = "SAP" },
            new() { Name = "QuickBooks" },
        
            // Marketing
            new() { Name = "SEO" },
            new() { Name = "SEM" },
            new() { Name = "Google Analytics" },
            new() { Name = "Google Ads" },
            new() { Name = "Meta Ads" },
            new() { Name = "Email Marketing" },
            new() { Name = "Content Writing" },
            new() { Name = "Copywriting" },
            new() { Name = "Social Media Marketing" },
        
            // HR
            new() { Name = "Recruitment" },
            new() { Name = "Talent Acquisition" },
            new() { Name = "Employee Relations" },
            new() { Name = "Performance Management" },
        
            // Customer Service
            new() { Name = "Customer Support" },
            new() { Name = "CRM" },
            new() { Name = "Zendesk" },
            new() { Name = "Salesforce" },
        
            // Engineering
            new() { Name = "AutoCAD" },
            new() { Name = "SolidWorks" },
            new() { Name = "Revit" },
            new() { Name = "MATLAB" },
            new() { Name = "PLC Programming" },
        
            // Logistics
            new() { Name = "Supply Chain Management" },
            new() { Name = "Inventory Management" },
            new() { Name = "Warehouse Management" },
        
            // Languages
            new() { Name = "English" },
            new() { Name = "Azerbaijani" },
            new() { Name = "Turkish" },
            new() { Name = "Russian" },
        
            // Soft Skills
            new() { Name = "Communication" },
            new() { Name = "Leadership" },
            new() { Name = "Teamwork" },
            new() { Name = "Problem Solving" },
            new() { Name = "Critical Thinking" },
            new() { Name = "Time Management" },
            new() { Name = "Adaptability" },
            new() { Name = "Creativity" },
            new() { Name = "Presentation Skills" },
            new() { Name = "Negotiation" },

            // .NET Ecosystem
            new() { Name = "ASP.NET MVC" },
            new() { Name = "Minimal APIs" },
            new() { Name = "Blazor" },
            new() { Name = "SignalR" },
            new() { Name = "Identity Server" },
            new() { Name = "Hangfire" },
            new() { Name = "Serilog" },
            new() { Name = "NLog" },
            new() { Name = "Polly" },
            new() { Name = "Background Services" },
            
            // Architecture
            new() { Name = "Clean Architecture" },
            new() { Name = "Onion Architecture" },
            new() { Name = "Domain-Driven Design" },
            new() { Name = "CQRS" },
            new() { Name = "Repository Pattern" },
            new() { Name = "Unit of Work" },
            new() { Name = "Dependency Injection" },
            new() { Name = "Design Patterns" },
            
            // Databases
            new() { Name = "Database Design" },
            new() { Name = "Database Optimization" },
            new() { Name = "Stored Procedures" },
            new() { Name = "Database Migration" },
            new() { Name = "Database Performance Tuning" },
            
            // API
            new() { Name = "API Integration" },
            new() { Name = "API Documentation" },
            new() { Name = "Webhook Integration" },
            new() { Name = "SOAP" },
            new() { Name = "WebSockets" },
            
            // Security
            new() { Name = "Authentication" },
            new() { Name = "Authorization" },
            new() { Name = "OWASP" },
            new() { Name = "Data Encryption" },
            new() { Name = "Secure Coding" },
            new() { Name = "Penetration Testing" },
            new() { Name = "Vulnerability Assessment" },
            
            // DevOps
            new() { Name = "Azure DevOps" },
            new() { Name = "Helm" },
            new() { Name = "Prometheus" },
            new() { Name = "Grafana" },
            new() { Name = "ELK Stack" },
            new() { Name = "SonarQube" },
            new() { Name = "Bash" },
            new() { Name = "PowerShell" },
            
            // AI
            new() { Name = "Generative AI" },
            new() { Name = "Prompt Engineering" },
            new() { Name = "Large Language Models" },
            new() { Name = "Natural Language Processing" },
            new() { Name = "Computer Vision" },
            new() { Name = "Data Visualization" },
            
            // Networking
            new() { Name = "TCP/IP" },
            new() { Name = "DNS" },
            new() { Name = "VPN" },
            new() { Name = "Firewall Management" },
            new() { Name = "Windows Server" },
            new() { Name = "Active Directory" },
            
            // ERP / CRM
            new() { Name = "Microsoft Dynamics 365" },
            new() { Name = "Odoo" },
            new() { Name = "SAP ERP" },
            
            // Design
            new() { Name = "Adobe InDesign" },
            new() { Name = "Adobe Lightroom" },
            new() { Name = "CorelDRAW" },
            new() { Name = "Sketch" },
            new() { Name = "Framer" },
            new() { Name = "Wireframing" },
            new() { Name = "Prototyping" },
            
            // Marketing
            new() { Name = "Google Search Console" },
            new() { Name = "Google Tag Manager" },
            new() { Name = "Meta Business Suite" },
            new() { Name = "TikTok Ads" },
            new() { Name = "LinkedIn Ads" },
            new() { Name = "Email Campaign Management" },
            new() { Name = "Affiliate Marketing" },
            new() { Name = "Influencer Marketing" },
            
            // Sales
            new() { Name = "B2B Sales" },
            new() { Name = "B2C Sales" },
            new() { Name = "Lead Generation" },
            new() { Name = "Cold Calling" },
            new() { Name = "Sales Negotiation" },
            new() { Name = "Upselling" },
            
            // HR
            new() { Name = "HR Management" },
            new() { Name = "HRIS" },
            new() { Name = "Payroll Processing" },
            new() { Name = "Interviewing" },
            new() { Name = "Onboarding" },
            
            // Accounting
            new() { Name = "1C Accounting" },
            new() { Name = "Microsoft Dynamics NAV" },
            new() { Name = "Financial Reporting" },
            new() { Name = "Budget Planning" },
            new() { Name = "Cost Analysis" },
            
            // Engineering
            new() { Name = "Project Estimation" },
            new() { Name = "Blueprint Reading" },
            new() { Name = "HVAC Systems" },
            new() { Name = "Electrical Installation" },
            new() { Name = "Mechanical Design" },
            
            // Construction
            new() { Name = "Construction Planning" },
            new() { Name = "Site Management" },
            new() { Name = "Building Codes" },
            new() { Name = "Health & Safety" },
            
            // Healthcare
            new() { Name = "Patient Care" },
            new() { Name = "Medical Terminology" },
            new() { Name = "First Aid" },
            new() { Name = "Clinical Research" },
            
            // Logistics
            new() { Name = "Procurement Management" },
            new() { Name = "Demand Planning" },
            new() { Name = "Fleet Management" },
            new() { Name = "International Logistics" },
            
            // Hospitality
            new() { Name = "Food Safety" },
            new() { Name = "Reservation Systems" },
            new() { Name = "Guest Relations" },
            
            // Office
            new() { Name = "Microsoft Teams" },
            new() { Name = "Zoom" },
            new() { Name = "Slack" },
            new() { Name = "Google Workspace" },
            
            // Soft Skills
            new() { Name = "Decision Making" },
            new() { Name = "Conflict Resolution" },
            new() { Name = "Emotional Intelligence" },
            new() { Name = "Analytical Thinking" },
            new() { Name = "Attention to Detail" },
            new() { Name = "Customer Orientation" },
            new() { Name = "Mentoring" },
            new() { Name = "Public Speaking" },
            new() { Name = "Research Skills" },
            new() { Name = "Multitasking" },
            new() { Name = "Self Motivation" },
            new() { Name = "Stress Management" }
        };

        await context.Skills.AddRangeAsync(skills);
        await context.SaveChangesAsync();
    }
}
