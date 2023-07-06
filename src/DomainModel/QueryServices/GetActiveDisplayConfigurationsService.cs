using DomainModel.Entities;
using DomainModel.Queries;
using DomainModel.Win32.Enumerations;
using DomainModel.Win32.Structures;
using DomainModel.Win32;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace DomainModel.QueryServices
{
    public class GetActiveDisplayConfigurationsService : IAsyncQueryService<GetActiveDisplayConfiguationsQuery, IEnumerable<DisplayConfiguration>>
    {
        public IEnumerable<DisplayConfiguration> Execute(GetActiveDisplayConfiguationsQuery query)
        {
            int errorCode = NativeMethods.GetDisplayConfigBufferSizes(QDC.QDC_ONLY_ACTIVE_PATHS, out var pathCount, out var modeCount);
            if (errorCode != 0)
                throw new Win32Exception(errorCode);

            DISPLAYCONFIG_PATH_INFO[] paths = new DISPLAYCONFIG_PATH_INFO[pathCount];
            DISPLAYCONFIG_MODE_INFO[] modes = new DISPLAYCONFIG_MODE_INFO[modeCount];

            errorCode = NativeMethods.QueryDisplayConfig(QDC.QDC_ONLY_ACTIVE_PATHS, ref pathCount, paths, ref modeCount, modes, IntPtr.Zero);
            if (errorCode != 0)
                throw new Win32Exception(errorCode);

            foreach (DISPLAYCONFIG_PATH_INFO path in paths)
            {
                DISPLAYCONFIG_SOURCE_DEVICE_NAME source = new();
                source.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME;
                source.header.size = Marshal.SizeOf<DISPLAYCONFIG_SOURCE_DEVICE_NAME>();
                source.header.adapterId = path.sourceInfo.adapterId;
                source.header.id = path.sourceInfo.id;

                errorCode = NativeMethods.DisplayConfigGetDeviceInfo(ref source);
                if (errorCode != 0)
                    throw new Win32Exception(errorCode);

                DISPLAYCONFIG_TARGET_DEVICE_NAME target = new();
                target.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME;
                target.header.size = Marshal.SizeOf<DISPLAYCONFIG_TARGET_DEVICE_NAME>();
                target.header.adapterId = path.targetInfo.adapterId;
                target.header.id = path.targetInfo.id;

                errorCode = NativeMethods.DisplayConfigGetDeviceInfo(ref target);
                if (errorCode != 0)
                    throw new Win32Exception(errorCode);

                DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO colorInfo = new();
                colorInfo.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO;
                colorInfo.header.size = Marshal.SizeOf<DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO>();
                colorInfo.header.adapterId = path.targetInfo.adapterId;
                colorInfo.header.id = path.targetInfo.id;

                errorCode = NativeMethods.DisplayConfigGetDeviceInfo(ref colorInfo);
                if (errorCode != 0)
                    throw new Win32Exception(errorCode);

                DISPLAYCONFIG_MODE_INFO sourceMode = path.sourceInfo.modeInfoIdx != DISPLAYCONFIG_PATH_MODE_IDX_INVALID ?
                    sourceMode = modes[path.sourceInfo.modeInfoIdx] :
                    throw new Exception("Source mode info index is invalid.");

                DISPLAYCONFIG_MODE_INFO targetMode = path.targetInfo.modeInfoIdx != DISPLAYCONFIG_PATH_MODE_IDX_INVALID ?
                    targetMode = modes[path.targetInfo.modeInfoIdx] :
                    throw new Exception("Target mode info index is invalid.");

                yield return new(
                    path.sourceInfo.id,
                    source.viewGdiDeviceName,
                    path.targetInfo.id,
                    target.monitorFriendlyDeviceName,
                    sourceMode.info.sourceMode.width,
                    sourceMode.info.sourceMode.height,
                    sourceMode.info.sourceMode.position.x,
                    sourceMode.info.sourceMode.position.y,
                    targetMode.info.targetMode.targetVideoSignalInfo.vSyncFreq.Numerator,
                    targetMode.info.targetMode.targetVideoSignalInfo.vSyncFreq.Denominator,
                    colorInfo.flags.HasFlag(DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO_FLAGS.advancedColorSupported),
                    colorInfo.flags.HasFlag(DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO_FLAGS.advancedColorEnabled),
                    path.targetInfo.targetAvailable);
            }
        }

        public Task<IEnumerable<DisplayConfiguration>> ExecuteAsync(GetActiveDisplayConfiguationsQuery query)
            => Task.FromResult(Execute(query));

        private const uint DISPLAYCONFIG_PATH_MODE_IDX_INVALID = 0xffffffff;
    }
}