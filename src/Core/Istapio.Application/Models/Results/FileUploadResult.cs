namespace Istapio.Application.Models.Results;

public sealed record FileUploadResult(
    string Key,           
    string FileName,      
    string ContentType,   
    long SizeInBytes,     
    string Url            
);