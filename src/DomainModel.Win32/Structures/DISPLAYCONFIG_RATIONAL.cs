using System.Runtime.InteropServices;

namespace DomainModel.Win32.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_RATIONAL
    {
        public uint Numerator;
        public uint Denominator;

        public override string ToString() => Numerator + " / " + Denominator;
    }
}