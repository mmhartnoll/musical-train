using DomainModel.Win32.Enumerations;
using DomainModel.Win32.Structures;
using System.Runtime.InteropServices;

namespace DomainModel.Win32
{
    public static class NativeMethods
    {
        [DllImport("user32")]
        public static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_SOURCE_DEVICE_NAME requestPacket);

        [DllImport("user32")]
        public static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_TARGET_DEVICE_NAME requestPacket);

        [DllImport("user32")]
        public static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_TARGET_PREFERRED_MODE requestPacket);

        [DllImport("user32")]
        public static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO requestPacket);

        [DllImport("user32")]
        public static extern int DisplayConfigSetDeviceInfo(ref DISPLAYCONFIG_SET_ADVANCED_COLOR_STATE setPacket);

        [DllImport("user32")]
        public static extern int GetDisplayConfigBufferSizes(QDC flags, out int numPathArrayElements, out int numModeInfoArrayElements);

        [DllImport("user32")]
        public static extern int QueryDisplayConfig(QDC flags, ref int numPathArrayElements, [In, Out] DISPLAYCONFIG_PATH_INFO[] pathArray, ref int numModeInfoArrayElements, [In, Out] DISPLAYCONFIG_MODE_INFO[] modeInfoArray, IntPtr currentTopologyId);

        [DllImport("user32")]
        public static extern int SetDisplayConfig(int numPathArrayElements, [In] DISPLAYCONFIG_PATH_INFO[] pathArray, int numModeInfoArrayElements, [In] DISPLAYCONFIG_MODE_INFO[] modeInfoArray, SetDisplayConfigFlags flags);
    }
}