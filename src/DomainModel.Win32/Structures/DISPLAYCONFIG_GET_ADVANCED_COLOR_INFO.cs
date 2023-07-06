using DomainModel.Win32.Enumerations;
using System.Runtime.InteropServices;

namespace DomainModel.Win32.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO
    {
        public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
        public DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO_FLAGS flags;
        public DISPLAYCONFIG_COLOR_ENCODING colorEncoding;
        public int bitsPerColorChannel;
    }
}