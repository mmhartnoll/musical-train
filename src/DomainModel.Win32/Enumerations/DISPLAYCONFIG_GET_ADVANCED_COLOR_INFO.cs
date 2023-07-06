namespace DomainModel.Win32.Enumerations
{
    [Flags]
    public enum DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO_FLAGS
    {
        none = 0x0,
        advancedColorSupported = 0x1,
        advancedColorEnabled = 0x2,
        wideColorEnforced = 0x4,
        advancedColorForceDisabled = 0x8,
    }
}
