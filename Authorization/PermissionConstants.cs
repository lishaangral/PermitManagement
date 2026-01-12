namespace PemitManagement.Authorization
{
    public static class PermissionConstants
    {
        public const string CreateObservations = "create_observations";
        public const string CloseObservations = "close_observations";
        //public const string CreatePermits = "create_permits";
        public const string ClosePermits = "close_permits";
        //public const string EditPermits = "edit_permits";
        //public const string ManageLocations = "manage_locations";
        //public const string ManagePermitTypes = "manage_permit_types";
        //public const string ManageIssuerRoles = "manage_issuer_roles";
        //public const string ManagePermitTypeIssuers = "manage_permit_type_issuers";
        //public const string ManagePermitTypeViolations = "manage_permit_type_violations";
        public const string ManageViolations = "manage_violations";
        public const string ViewReports = "view_reports";

        public static readonly string[] All =
        {
            CreateObservations,
            CloseObservations,
            //CreatePermits,
            ClosePermits,
            //EditPermits,
            //ManageLocations,
            //ManagePermitTypes,
            //ManageIssuerRoles,
            //ManagePermitTypeIssuers,
            //ManagePermitTypeViolations,
            ManageViolations,
            ViewReports
        };
    }
}
