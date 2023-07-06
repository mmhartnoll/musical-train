using DomainModel.Entities;
using DomainModel.Queries;
using DomainModel.Win32.Enumerations;
using DomainModel.Win32;
using System.ComponentModel;
using DomainModel.Win32.Structures;
using System.Runtime.InteropServices;
using Tools.Extensions;

namespace DomainModel.QueryServices
{
    public class GetAvailableDisplayTargetsService : IAsyncQueryService<GetAvailableDisplayTargetsQuery, IEnumerable<DisplayTarget>>
    {
        public IEnumerable<DisplayTarget> Execute(GetAvailableDisplayTargetsQuery query)
        {
            int errorCode = NativeMethods.GetDisplayConfigBufferSizes(QDC.QDC_ALL_PATHS, out var pathCount, out var modeCount);
            if (errorCode != 0)
                throw new Win32Exception(errorCode);

            DISPLAYCONFIG_PATH_INFO[] paths = new DISPLAYCONFIG_PATH_INFO[pathCount];
            DISPLAYCONFIG_MODE_INFO[] modes = new DISPLAYCONFIG_MODE_INFO[modeCount];

            errorCode = NativeMethods.QueryDisplayConfig(QDC.QDC_ALL_PATHS, ref pathCount, paths, ref modeCount, modes, IntPtr.Zero);
            if (errorCode != 0)
                throw new Win32Exception(errorCode);

            IList<DisplayTarget> displayTargets = new List<DisplayTarget>();
            foreach (DISPLAYCONFIG_PATH_INFO path in paths.Where(path => path.targetInfo.targetAvailable))
            {
                DISPLAYCONFIG_TARGET_DEVICE_NAME target = new();
                target.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME;
                target.header.size = Marshal.SizeOf<DISPLAYCONFIG_TARGET_DEVICE_NAME>();
                target.header.adapterId = path.targetInfo.adapterId;
                target.header.id = path.targetInfo.id;

                errorCode = NativeMethods.DisplayConfigGetDeviceInfo(ref target);
                if (errorCode != 0)
                    throw new Win32Exception(errorCode);

                displayTargets.Add(new(path.targetInfo.id, target.monitorFriendlyDeviceName, path.targetInfo.targetAvailable));
            }

            return displayTargets
                .Where(target => !string.IsNullOrEmpty(target.DisplayName))
                .GroupBy(target => target.Id)
                .Select(group => group.First())
                .Enumerate();
        }

        public Task<IEnumerable<DisplayTarget>> ExecuteAsync(GetAvailableDisplayTargetsQuery query)
            => Task.FromResult(Execute(query));
    }
} 