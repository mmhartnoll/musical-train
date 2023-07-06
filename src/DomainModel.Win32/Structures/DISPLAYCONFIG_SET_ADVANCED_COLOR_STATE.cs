using System.Runtime.InteropServices;

namespace DomainModel.Win32.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_SET_ADVANCED_COLOR_STATE
    {
        public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
        public uint enableAdvancedColor;
    }
}