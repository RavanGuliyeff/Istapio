using Amazon.S3;
using Amazon.S3.Model;
using Istapio.Application.Models.Results;
using Istapio.Application.Models.Settings;
using Istapio.Application.Services.External.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Istapio.Infrastructure.Services.External.Implementations;

public sealed class FileService : IFileService
{
    private readonly IAmazonS3 _s3Client;
    private readonly AwsSettings _aws;

    private static readonly HashSet<string> AllowedContentTypes = new()
    {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif",
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
    };

    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10MB

    public FileService(
        IAmazonS3 s3Client,
        IOptions<AwsSettings> awsOptions)
    {
        _s3Client = s3Client;
        _aws = awsOptions.Value;
    }

    public async Task<FileUploadResult> UploadAsync(
        Stream fileStream,
        string fileName,
        string folder,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        ValidateFile(fileStream, contentType);

        var sanitizedFileName = SanitizeFileName(fileName);
        var uniqueFileName = $"{Guid.NewGuid()}_{sanitizedFileName}";
        var key = $"{folder.TrimEnd('/')}/{uniqueFileName}";

        var request = new PutObjectRequest
        {
            BucketName = _aws.BucketName,
            Key = key,
            InputStream = fileStream,
            ContentType = contentType,
            ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
        };

        try
        {
            await _s3Client.PutObjectAsync(request, cancellationToken);

            return new FileUploadResult(
                Key: key,
                FileName: sanitizedFileName,
                ContentType: contentType,
                SizeInBytes: fileStream.Length,
                Url: $"https://{_aws.BucketName}.s3.{_aws.Region}.amazonaws.com/{key}"
            );
        }
        catch (AmazonS3Exception ex)
        {
            throw new InvalidOperationException($"File upload failed: {ex.Message}", ex);
        }
    }

    public async Task<string> GeneratePresignedUrlAsync(
        string key,
        int expiresInMinutes = 60,
        CancellationToken cancellationToken = default)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _aws.BucketName,
            Key = key,
            Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
            Protocol = Protocol.HTTPS
        };

        try
        {
            var url = await _s3Client.GetPreSignedURLAsync(request);
            return url;
        }
        catch (AmazonS3Exception ex)
        {
            throw new InvalidOperationException($"Presigned URL generation failed: {ex.Message}", ex);
        }
    }

    public async Task DeleteAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = _aws.BucketName,
            Key = key
        };

        try
        {
            await _s3Client.DeleteObjectAsync(request, cancellationToken);
        }
        catch (AmazonS3Exception ex)
        {
            throw new InvalidOperationException($"File delete failed: {ex.Message}", ex);
        }
    }

    public async Task DeleteManyAsync(
        IEnumerable<string> keys,
        CancellationToken cancellationToken = default)
    {
        var keyList = keys.ToList();
        if (!keyList.Any()) return;

        var request = new DeleteObjectsRequest
        {
            BucketName = _aws.BucketName,
            Objects = keyList
                .Select(k => new KeyVersion { Key = k })
                .ToList()
        };

        try
        {
            var response = await _s3Client.DeleteObjectsAsync(request, cancellationToken);

            if (response.DeleteErrors.Any())
            {
                var errors = string.Join(", ", response.DeleteErrors.Select(e => $"{e.Key}: {e.Message}"));
                throw new InvalidOperationException($"Some files failed to delete: {errors}");
            }

        }
        catch (AmazonS3Exception ex)
        {
            throw new InvalidOperationException($"Bulk file delete failed: {ex.Message}", ex);
        }
    }

    public async Task<string> CopyAsync(
        string sourceKey,
        string destinationFolder,
        CancellationToken cancellationToken = default)
    {
        var fileName = Path.GetFileName(sourceKey);
        var destinationKey = $"{destinationFolder.TrimEnd('/')}/{Guid.NewGuid()}_{fileName}";

        var request = new CopyObjectRequest
        {
            SourceBucket = _aws.BucketName,
            SourceKey = sourceKey,
            DestinationBucket = _aws.BucketName,
            DestinationKey = destinationKey,
            ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
        };

        try
        {
            await _s3Client.CopyObjectAsync(request, cancellationToken);

            return destinationKey;
        }
        catch (AmazonS3Exception ex)
        {
            throw new InvalidOperationException($"File copy failed: {ex.Message}", ex);
        }
    }

    public async Task<bool> ExistsAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _s3Client.GetObjectMetadataAsync(_aws.BucketName, key, cancellationToken);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    private static void ValidateFile(Stream fileStream, string contentType)
    {
        if (fileStream.Length == 0)
            throw new ArgumentException("File is empty.");

        if (fileStream.Length > MaxFileSizeBytes)
            throw new ArgumentException($"File size exceeds the maximum allowed size of {MaxFileSizeBytes / 1024 / 1024}MB.");

        if (!AllowedContentTypes.Contains(contentType.ToLower()))
            throw new ArgumentException($"Content type '{contentType}' is not allowed.");
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = string.Concat(fileName
            .Where(c => !invalidChars.Contains(c)))
            .Replace(" ", "_")
            .ToLower();

        return sanitized;
    }
}