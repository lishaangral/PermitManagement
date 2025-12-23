namespace PemitManagement.Authorization
{
    public static class PermissionConstants
    {
        public static readonly string[] All =
        {
            "create_observations",
            "delete_observations",
            "create_permits",
            "delete_permits",
            "edit_permits",
            "manage_locations",
            "manage_permit_types",
            "manage_issuer_roles",
            "manage_permit_type_issuers",
            "manage_permit_type_violations",
            "manage_violations"
        };
    }
}
