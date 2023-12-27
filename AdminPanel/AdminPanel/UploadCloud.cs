using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace AdminPanel
{
    public static class UploadCloud
    {
        public static UploadResult UploadImage(IFormFile _file)
        {
            Cloudinary cloudinary = new Cloudinary("cloudinary://528743424672781:y3zLxM9yIt_2y49_a4qaQraRW10@dehc2lku2");
            cloudinary.Api.Secure = true;

            var uploadResult = new ImageUploadResult();

            if (_file.Length > 0) 
            {
                using (var stream = _file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(_file.FileName, stream),
                        Transformation = new Transformation().Crop("fill").Gravity("face")
                    };
                    uploadResult = cloudinary.Upload(uploadParams);
                }
            }

            if (uploadResult.Error != null)
            {
                throw new Exception(uploadResult.Error.Message);
            }
            return uploadResult;
        }
    }
}
