using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace CJP.OutputCachedParts
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission ManageCachedKeys = new Permission { Name = "ManageCachedPartsKeys", Description = "Access the UI that enables the management of cached items" };

        public Feature Feature { get; set; }
        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                ManageCachedKeys,
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {ManageCachedKeys}
                }
            };
        }

    }
}