namespace OnBoardy.API.Results
{
    public static class ModuleErrors
    {
        public static readonly Error NotFound =
            new("module.not_found", "Module not found.", StatusCodes.Status404NotFound);

        public static readonly Error OrganizationNotFound =
            new("organization.not_found", "Organization not found.", StatusCodes.Status404NotFound);

        public static readonly Error EmptyUpdatePayload =
            new("module.empty_update_payload", "No fields were provided for update.", StatusCodes.Status400BadRequest);
    }
}