using System.Runtime.InteropServices;

namespace DomainModel.Win32.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LUID
    {
        public uint LowPart;
        public int HighPart;

        public long Value => ((long)HighPart << 32) | LowPart;
        public override string ToString() => Value.ToString();
    }
}