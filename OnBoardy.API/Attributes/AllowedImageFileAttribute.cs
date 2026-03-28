using System.ComponentModel.DataAnnotations;
using OnBoardy.API.Constants;

namespace OnBoardy.API.Attributes;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public class AllowedImageFileAttribute : ValidationAttribute
{
    private readonly long _maxBytes;

    public AllowedImageFileAttribute(long maxBytes)
    {
        _maxBytes = maxBytes;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not IFormFile file || file.Length == 0)
        {
            return new ValidationResult("File is required.");
        }

        if (file.Length > _maxBytes)
        {
            return new ValidationResult($"File size exceeds the limit of.");
        }

        var contentType = file.ContentType?.ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(contentType) ||
            !MediaValidation.AllowedImageContentTypes.Contains(contentType))
        {
            return new ValidationResult("Invalid image content type.");
        }

        var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(extension) ||
            !MediaValidation.AllowedImageExtensions.Contains(extension))
        {
            return new ValidationResult("Invalid image file extension.");
        }

        return ValidationResult.Success;
    }
}