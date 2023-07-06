using DomainModel.Win32.Enumerations;
using System.Runtime.InteropServices;

namespace DomainModel.Win32.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_DEVICE_INFO_HEADER
    {
        public DISPLAYCONFIG_DEVICE_INFO_TYPE type;
        public int size;
        public LUID adapterId;
        public uint id;
    }
}