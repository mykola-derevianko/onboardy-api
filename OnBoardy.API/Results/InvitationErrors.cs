namespace OnBoardy.API.Results
{
    public static class InvitationErrors
    {
        public static readonly Error OrganizationNotFound =
            new("organization.not_found", "Organization not found.", StatusCodes.Status404NotFound);

        public static readonly Error OwnerRoleNotAllowed =
            new("invitation.owner_role_not_allowed", "Owner role cannot be assigned through invitation.", StatusCodes.Status400BadRequest);

        public static readonly Error TokenGenerationFailed =
            new("invitation.token_generation_failed", "Failed to generate a unique invitation token.", StatusCodes.Status500InternalServerError);

        public static readonly Error InvalidInvitation =
            new("invitation.invalid", "Invitation is invalid, expired, or not pending.", StatusCodes.Status400BadRequest);

        public static readonly Error EmailMismatch =
            new("invitation.email_mismatch", "Invitation email does not match current user.", StatusCodes.Status400BadRequest);

        public static readonly Error NotFound =
            new("invitation.not_found", "Invitation not found.", StatusCodes.Status404NotFound);

        public static readonly Error OnlyPendingCanBeDeleted =
            new("invitation.delete_pending_only", "Only pending invitations can be deleted.", StatusCodes.Status400BadRequest);
    }
}