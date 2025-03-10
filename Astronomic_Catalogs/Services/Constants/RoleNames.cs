namespace Astronomic_Catalogs.Services.Constants;

public static class RoleNames
{
    public const string Admin = "OwnerAdministratorPower";
    public const string User = "UserPower";
    public const string AutoUser = "AutoUserPower";

    public static readonly string[] AllRoles = { Admin, User, AutoUser };
}

