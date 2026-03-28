namespace OnBoardy.API.Constants
{
    public static class MediaValidation
    {
        public const long MaxProfilePictureBytes = 2 * 1024 * 1024;
        public const long MaxOrganizationLogoBytes = 2 * 1024 * 1024;
        public const long MaxOrganizationBannerBytes = 5 * 1024 * 1024;

        public static readonly HashSet<string> AllowedImageExtensions =
        [
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        ];

        public static readonly HashSet<string> AllowedImageContentTypes =
        [
            "image/jpg",
            "image/jpeg",
            "image/png",
            "image/webp"
        ];

    }
}