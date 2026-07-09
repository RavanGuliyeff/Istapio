using Istapio.Application.Models.Results;

namespace Istapio.Application.Services.External.Interfaces;

public interface IFileService
{

    Task<FileUploadResult> UploadAsync(
        Stream fileStream,
        string fileName,
        string folder,
        string contentType,
        CancellationToken cancellationToken = default);


    Task<string> GeneratePresignedUrlAsync(
        string key,
        int expiresInMinutes = 60,
        CancellationToken cancellationToken = default);


    Task DeleteAsync(
        string key,
        CancellationToken cancellationToken = default);

 
    Task DeleteManyAsync(
        IEnumerable<string> keys,
        CancellationToken cancellationToken = default);


    Task<string> CopyAsync(
        string sourceKey,
        string destinationFolder,
        CancellationToken cancellationToken = default);

 
    Task<bool> ExistsAsync(
        string key,
        CancellationToken cancellationToken = default);
}
