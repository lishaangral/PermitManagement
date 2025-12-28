using System.Security.Claims;
using PemitManagement.ViewModels.Sidebar;

namespace PemitManagement.Services;

public class SidebarService
{
    public List<SidebarItemViewModel> GetSidebar(ClaimsPrincipal user)
    {
        //bool Has(string permission) =>
        //    user.HasClaim("permission", permission);

        return new List<SidebarItemViewModel>
        {
            new()
            {
                Title = "Dashboard",
                Icon = "fa-home",
                Controller = "Dashboard"
            },

            new()
            {
                Title = "Report Analysis",
                Icon = "fa-chart-line",
                Controller = "ReportAnalysis"
            },

            new()
            {
                Title = "Violation Analysis",
                Icon = "fa-triangle-exclamation",
                Controller = "ViolationAnalysis"
            },

            new()
            {
                Title = "Create Observations",
                Icon = "fa-eye",
                Controller = "Observations",
                Action = "Create",
                RequiredPermission = "create_observations"
            },

            new()
            {
                Title = "Manage Permits",
                Icon = "fa-file-contract",
                Children =
                {
                    new()
                    {
                        Title = "Permits",
                        Icon = "fa-list",
                        Controller = "Permits"
                    },
                    new()
                    {
                        Title = "Create Permit",
                        Icon = "fa-plus",
                        Controller = "Permits",
                        Action = "Create",
                        RequiredPermission = "create_permits"
                    },
                    new()
                    {
                        Title = "Permit Types",
                        Icon = "fa-layer-group",
                        Controller = "PermitTypes",
                        RequiredPermission = "manage_permit_types"
                    },
                    new()
                    {
                        Title = "Permit Type Issuers",
                        Icon = "fa-users",
                        Controller = "PermitTypeIssuers",
                        RequiredPermission = "manage_permit_type_issuers"
                    },
                    new()
                    {
                        Title = "Permit Type Violations",
                        Icon = "fa-ban",
                        Controller = "PermitTypeViolations",
                        RequiredPermission = "manage_permit_type_violations"
                    }
                }
            },

            new()
            {
                Title = "Manage Violations",
                Icon = "fa-exclamation-circle",
                Controller = "Violations",
                RequiredPermission = "manage_violations"
            },

            new()
            {
                Title = "Manage Issuer Roles",
                Icon = "fa-user-shield",
                Controller = "IssuerRoles",
                RequiredPermission = "manage_issuer_roles"
            },

            new()
            {
                Title = "Manage Locations",
                Icon = "fa-location-dot",
                Controller = "Locations",
                RequiredPermission = "manage_locations"
            }
        };
    }
}
