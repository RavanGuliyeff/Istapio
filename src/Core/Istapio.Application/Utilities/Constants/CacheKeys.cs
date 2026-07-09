namespace Istapio.Application.Utilities.Constants;

public static class CacheKeys
{
    public static class Otp
    {
        public static string EmailVerify(string email)
            => $"otp:verify-email:{email.ToLowerInvariant()}";

        public static string PasswordReset(string email)
            => $"otp:reset-password:{email.ToLowerInvariant()}";

        public static string DailyRate(string email)
            => $"otp:rate:{email.ToLowerInvariant()}";

        public static string FailCount(string email)
            => $"otp:fail:{email.ToLowerInvariant()}";
    }

    public static class Categories
    {
        public static string ById(Guid id)
            => $"category:id:{id}";

        public const string All = "categories:all";
    }

    public static class Companies
    {
        public static string ById(Guid id)
            => $"company:id:{id}";

        public const string All = "companies:all";
    }

    public static class JobPosts
    {
        public static string ById(Guid id)
            => $"jobpost:id:{id}";

        public const string All = "jobposts:all";
    }

    public static class Skills
    {
        public static string ById(Guid id)
            => $"skill:id:{id}";

        public const string All = "skills:all";
    }

    public static class VacationTypes
    {
        public static string ById(Guid id)
            => $"vacationtype:id:{id}";

        public const string All = "vacationtypes:all";
    }

    public static class Settings
    {
        public static string ById(Guid id)
            => $"setting:id:{id}";

        public static string ByKey(string key)
            => $"setting:key:{key}";

        public const string All = "settings:all";
    }
}
