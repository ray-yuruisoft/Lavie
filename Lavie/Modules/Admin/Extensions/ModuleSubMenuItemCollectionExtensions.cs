using System.Collections.Generic;
using System.Linq;
using Lavie.Extensions;
using Lavie.Infrastructure;
using Lavie.Modules.Admin.Models;

namespace Lavie.Modules.Admin.Extensions
{
    public static class ModuleSubMenuItemCollectionExtensions
    {
        public static void FilterByPermission(this List<ModuleSubMenuItem> items, IUser user)
        {
            if (items.IsNullOrEmpty()) return;
            if (user == null) items.Clear();

            items.RemoveAll(it =>
            {
                var isMatch = it.Permission.IsNullOrWhiteSpace() || it.Permission.Split('|').Any(m=> user.HasPermission(m));
                return !isMatch;
            });

            var itemGroup = from p in items
                            where p is ModuleSubMenuItemGroup
                            select p as ModuleSubMenuItemGroup;

            foreach (var it in itemGroup)
            {
                FilterByPermission(it.Items, user);
            }
        }
        public static void FilterByRole(this List<ModuleSubMenuItem> items, IUser user)
        {
            if (items.IsNullOrEmpty()) return;
            if (user == null) items.Clear();

            items.RemoveAll(it =>
            {
                var isMatch = it.Role.IsNullOrWhiteSpace() || it.Role.Split('|').Any(m => user.IsInRole(m));
                return !isMatch;
            });


            var itemGroup = from p in items
                            where p is ModuleSubMenuItemGroup
                            select p as ModuleSubMenuItemGroup;

            foreach (var it in itemGroup)
            {
                FilterByRole(it.Items, user);
            }
        }
        public static void FilterByGroup(this List<ModuleSubMenuItem> items, IUser user)
        {
            if (items.IsNullOrEmpty()) return;
            if (user == null) items.Clear();

            items.RemoveAll(it =>
            {
                var isMatch = it.Group.IsNullOrWhiteSpace() || it.Group.Split('|').Any(m => user.IsInGroup(m));
                return !isMatch;
            });

            var itemGroup = from p in items
                            where p is ModuleSubMenuItemGroup
                            select p as ModuleSubMenuItemGroup;

            foreach (var it in itemGroup)
            {
                FilterByGroup(it.Items, user);
            }
        }

    }
}
