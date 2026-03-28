namespace OnBoardy.API.Results
{
    public static class AuthErrors
    {
        public static readonly Error InvalidCredentials =
            new("auth.invalid_credentials", "Invalid credentials.", StatusCodes.Status401Unauthorized);

        public static readonly Error EmailNotVerified =
            new("auth.email_not_verified", "Email not verified.", StatusCodes.Status403Forbidden);

        public static readonly Error AccountDisabled =
            new("auth.account_disabled", "Account disabled.", StatusCodes.Status403Forbidden);

        public static readonly Error InvalidRefreshToken =
            new("auth.invalid_refresh_token", "Invalid refresh token.", StatusCodes.Status401Unauthorized);

        public static readonly Error RefreshTokenExpired =
            new("auth.refresh_token_expired", "Refresh token expired.", StatusCodes.Status401Unauthorized);

        public static readonly Error InvalidEmailVerificationToken =
            new("auth.invalid_email_verification_token", "Invalid or expired email verification token.", StatusCodes.Status400BadRequest);
    }
}