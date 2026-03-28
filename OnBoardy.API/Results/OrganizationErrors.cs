namespace OnBoardy.API.Results
{
    public static class OrganizationErrors
    {
        public static readonly Error NotFound =
            new("organization.not_found", "Organization not found.", StatusCodes.Status404NotFound);

        public static readonly Error EmptyUpdatePayload =
            new("organization.empty_update_payload", "No fields were provided for update.", StatusCodes.Status400BadRequest);
    }
}