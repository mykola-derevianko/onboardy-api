namespace OnBoardy.API.Results
{
    public static class MembershipErrors
    {
        public static readonly Error NotFound =
            new("membership.not_found", "Membership not found.", StatusCodes.Status404NotFound);

        public static readonly Error AlreadyMember =
            new("membership.already_member", "User is already a member of this organization.", StatusCodes.Status409Conflict);

        public static readonly Error InvalidRoleChange =
            new("membership.invalid_role_change", "Invalid membership role update.", StatusCodes.Status400BadRequest);
    }
}