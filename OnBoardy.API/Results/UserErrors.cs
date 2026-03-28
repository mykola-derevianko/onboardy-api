namespace OnBoardy.API.Results
{
    public static class UserErrors
    {
        public static readonly Error NotFound =
            new("user.not_found", "User not found.", StatusCodes.Status404NotFound);

        public static readonly Error EmailAlreadyRegistered =
            new("user.email_already_registered", "Email already registered.", StatusCodes.Status409Conflict);

        public static readonly Error EmptyUpdatePayload =
            new("user.empty_update_payload", "No fields were provided for update.", StatusCodes.Status400BadRequest);

        public static readonly Error EmailAlreadyVerified =
            new("user.email_already_verified", "Email is already verified.", StatusCodes.Status400BadRequest);
    }
}