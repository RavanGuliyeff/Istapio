namespace Istapio.Application.Models.Settings
{
    public sealed class AwsSettings
    {
        public string AccessKey { get; set; } = default!;
        public string SecretKey { get; set; } = default!;
        public string Region { get; set; } = default!;
        public string BucketName { get; set; } = default!;
    }
}
