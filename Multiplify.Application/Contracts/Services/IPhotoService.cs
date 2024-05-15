using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Multiplify.Application.Contracts.Services;
public interface IPhotoService
{
    Task<ImageUploadResult> AddPhotoAsync(IFormFile file, string filename, string folder);
    Task<DeletionResult> DeletePhotoAsync(string publicId);
}
