using System.Runtime.InteropServices;

namespace DomainModel.Win32.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINTL
    {
        public int x;
        public int y;
    }
}