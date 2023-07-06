namespace DomainModel.Win32.Enumerations
{
    [Flags]
    public enum DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS
    {
        none                 = 0x0,
        friendlyNameFromEdid = 0x1,
        friendlyNameForced   = 0x2,
        edidIdsValid         = 0x4,
    }
}