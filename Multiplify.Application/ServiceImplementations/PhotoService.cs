using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Multiplify.Application.Config;
using Multiplify.Application.Contracts.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Multiplify.Application.ServiceImplementations;
public class PhotoService : IPhotoService
{
    private readonly CloudinarySettings _cloudSettings;
    private readonly Cloudinary _cloudinary;

    public PhotoService(IOptions<CloudinarySettings> cloudSettings)
    {
        _cloudSettings = cloudSettings.Value;

        var acc = new Account
        (
            _cloudSettings.CloudName,
            _cloudSettings.ApiKey,
            _cloudSettings.ApiSecret
        );

        _cloudinary = new Cloudinary(acc);
    }

    public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file, string filename, string folder)
    {
        var uploadResult = new ImageUploadResult();
        //string mimeType = file.ContentType;
        if (file.Length > 0)
        {
            if (file.Length > (5 * 1024 * 1024))
            {
                uploadResult.Error = new CloudinaryDotNet.Actions.Error
                {
                    Message = "Max bytes exceeded, image size should be 5mb or less"
                };
                return uploadResult;
            }
            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(filename, stream),
                Transformation = new Transformation()
                    .AspectRatio("0.5").Gravity("auto")
                    .Height(500).Width(500).Crop("fill").Chain()
                    .Quality("auto").Chain()
                    .FetchFormat("auto"),

                Folder = folder,
                DisplayName = filename,
                UseAssetFolderAsPublicIdPrefix = true,
            };

            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }


        return uploadResult;
    }

    public async Task<DeletionResult> DeletePhotoAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        return await _cloudinary.DestroyAsync(deleteParams);
    }
}
