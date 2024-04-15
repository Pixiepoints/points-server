using PointsServer.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace PointsServer.Permissions;

public class PointsServerPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(PointsServerPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(PointsServerPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<PointsServerResource>(name);
    }
}
