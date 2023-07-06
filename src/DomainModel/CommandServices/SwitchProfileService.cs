using DomainModel.Commands;
using DomainModel.Entities;
using DomainModel.Enumerations;
using DomainModel.Events;
using DomainModel.Exceptions;
using DomainModel.Win32;
using DomainModel.Win32.Enumerations;
using DomainModel.Win32.Structures;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace DomainModel.CommandServices
{
    public class SwitchProfileService : DomainService, IAsyncCommandService<SwitchProfileCommand>
    {
        public SwitchProfileService(DomainEventHandler<DomainLogEvent> logEventHandler)
            : base(logEventHandler) { }

        public void Execute(SwitchProfileCommand command)
        {
            int errorCode = NativeMethods.GetDisplayConfigBufferSizes(QDC.QDC_ALL_PATHS, out var pathCount, out var modeCount);
            if (errorCode != 0)
            {
                Win32Exception exception = new(errorCode);
                LogEventHandler.HandleEvent(new DomainLogEvent(LogLevel.Error, exception.Message));
                throw exception;
            }

            DISPLAYCONFIG_PATH_INFO[] existingPaths = new DISPLAYCONFIG_PATH_INFO[pathCount];
            DISPLAYCONFIG_MODE_INFO[] existingModes = new DISPLAYCONFIG_MODE_INFO[modeCount];

            DISPLAYCONFIG_PATH_INFO[] newPaths = new DISPLAYCONFIG_PATH_INFO[pathCount];
            DISPLAYCONFIG_MODE_INFO[] newModes = new DISPLAYCONFIG_MODE_INFO[modeCount];

            List<DISPLAYCONFIG_SET_ADVANCED_COLOR_STATE> colorStateChanges = new();

            errorCode = NativeMethods.QueryDisplayConfig(QDC.QDC_ALL_PATHS, ref pathCount, existingPaths, ref modeCount, existingModes, IntPtr.Zero);
            if (errorCode != 0)
            {
                Win32Exception exception = new(errorCode);
                LogEventHandler.HandleEvent(new DomainLogEvent(LogLevel.Error, exception.Message));
                throw exception;
            }

            Array.Copy(existingPaths, newPaths, pathCount);

            for (uint i = 0; i < newPaths.Length; i++)
            {
                newPaths[i].flags = 0;
                newPaths[i].sourceInfo.modeInfoIdx = DISPLAYCONFIG_PATH_MODE_IDX_INVALID;
            }

            foreach (DisplayConfiguration displayConfiguration in command.DisplayProfile.DisplayConfigurations.OrderBy(config => config.SourceId))
                for (uint iPath = 0; iPath < newPaths.Length; iPath++)
                {
                    if (newPaths[iPath].sourceInfo.modeInfoIdx != DISPLAYCONFIG_PATH_MODE_IDX_INVALID ||
                        displayConfiguration.SourceId != newPaths[iPath].sourceInfo.id ||
                        displayConfiguration.TargetId != newPaths[iPath].targetInfo.id)
                        continue;

                    DISPLAYCONFIG_SOURCE_DEVICE_NAME source = new();
                    source.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME;
                    source.header.size = Marshal.SizeOf<DISPLAYCONFIG_SOURCE_DEVICE_NAME>();
                    source.header.adapterId = newPaths[iPath].sourceInfo.adapterId;
                    source.header.id = newPaths[iPath].sourceInfo.id;

                    errorCode = NativeMethods.DisplayConfigGetDeviceInfo(ref source);
                    if (errorCode != 0)
                    {
                        Win32Exception exception = new(errorCode);
                        LogEventHandler.HandleEvent(new DomainLogEvent(LogLevel.Error, exception.Message));
                        throw exception;
                    }

                    if (displayConfiguration.DeviceName != source.viewGdiDeviceName)
                        continue;

                    DISPLAYCONFIG_TARGET_DEVICE_NAME target = new();
                    target.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME;
                    target.header.size = Marshal.SizeOf<DISPLAYCONFIG_TARGET_DEVICE_NAME>();
                    target.header.adapterId = newPaths[iPath].targetInfo.adapterId;
                    target.header.id = newPaths[iPath].targetInfo.id;

                    errorCode = NativeMethods.DisplayConfigGetDeviceInfo(ref target);
                    if (errorCode != 0)
                    {
                        Win32Exception exception = new(errorCode);
                        LogEventHandler.HandleEvent(new DomainLogEvent(LogLevel.Error, exception.Message));
                        throw exception;
                    }

                    if (displayConfiguration.DisplayName != target.monitorFriendlyDeviceName)
                        continue;

                    newPaths[iPath].flags = DISPLAYCONFIG_PATH.DISPLAYCONFIG_PATH_ACTIVE;

                    for (uint iMode = 0; iMode < newModes.Length; iMode++)
                        if (newModes[iMode].infoType == 0)
                        {
                            DISPLAYCONFIG_TARGET_PREFERRED_MODE pMode = new();
                            pMode.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_PREFERRED_MODE;
                            pMode.header.size = Marshal.SizeOf<DISPLAYCONFIG_TARGET_PREFERRED_MODE>();
                            pMode.header.adapterId = newPaths[iPath].targetInfo.adapterId;
                            pMode.header.id = newPaths[iPath].targetInfo.id;
                            errorCode = NativeMethods.DisplayConfigGetDeviceInfo(ref pMode);
                            if (errorCode != 0)
                            {
                                Win32Exception exception = new(errorCode);
                                LogEventHandler.HandleEvent(new DomainLogEvent(LogLevel.Error, exception.Message));
                                throw exception;
                            }

                            newPaths[iPath].targetInfo.modeInfoIdx = iMode;
                            newModes[iMode].id = displayConfiguration.TargetId;
                            newModes[iMode].adapterId = newPaths[iPath].sourceInfo.adapterId;
                            newModes[iMode].infoType = DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_TARGET;
                            newModes[iMode].info.targetMode.targetVideoSignalInfo = pMode.targetMode.targetVideoSignalInfo;
                            newModes[iMode].info.targetMode.targetVideoSignalInfo.vSyncFreq.Numerator = displayConfiguration.VSyncNumerator;
                            newModes[iMode].info.targetMode.targetVideoSignalInfo.vSyncFreq.Denominator = displayConfiguration.VSyncDenominator;

                            newPaths[iPath].sourceInfo.modeInfoIdx = iMode + 1;
                            newModes[iMode + 1].adapterId = newPaths[iPath].sourceInfo.adapterId;
                            newModes[iMode + 1].infoType = DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE;
                            newModes[iMode + 1].info.sourceMode.width = displayConfiguration.ResolutionX;
                            newModes[iMode + 1].info.sourceMode.height = displayConfiguration.ResolutionY;
                            newModes[iMode + 1].info.sourceMode.position.x = displayConfiguration.PositionX;
                            newModes[iMode + 1].info.sourceMode.position.y = displayConfiguration.PositionY;
                            newModes[iMode + 1].info.sourceMode.pixelFormat = DISPLAYCONFIG_PIXELFORMAT.DISPLAYCONFIG_PIXELFORMAT_32BPP;

                            if (displayConfiguration.IsHdrSupported)
                            {
                                DISPLAYCONFIG_SET_ADVANCED_COLOR_STATE setColorState = new();
                                setColorState.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_SET_ADVANCED_COLOR_STATE;
                                setColorState.header.size = Marshal.SizeOf<DISPLAYCONFIG_SET_ADVANCED_COLOR_STATE>();
                                setColorState.header.adapterId = newPaths[iPath].targetInfo.adapterId;
                                setColorState.header.id = newPaths[iPath].targetInfo.id;
                                setColorState.enableAdvancedColor = displayConfiguration.IsHdrEnabled ? 1u : 0u;

                                colorStateChanges.Add(setColorState);
                            }

                            break;
                        }

                    IEnumerable<DISPLAYCONFIG_PATH_INFO> activePaths = newPaths.Where(path => path.flags.HasFlag(DISPLAYCONFIG_PATH.DISPLAYCONFIG_PATH_ACTIVE));
                    IEnumerable<DISPLAYCONFIG_PATH_INFO> inactivePaths = newPaths.Where(path => !path.flags.HasFlag(DISPLAYCONFIG_PATH.DISPLAYCONFIG_PATH_ACTIVE));
                    newPaths = activePaths.Concat(inactivePaths).ToArray();

                    for (uint i = 0; i < newPaths.Length; i++)
                        if (newPaths[i].sourceInfo.modeInfoIdx != DISPLAYCONFIG_PATH_MODE_IDX_INVALID)
                        {
                            uint modeIndex = newPaths[i].sourceInfo.modeInfoIdx;
                            newModes[modeIndex].id = i;
                        }
                }

            SetDisplayConfigFlags flags =
                SetDisplayConfigFlags.SDC_USE_SUPPLIED_DISPLAY_CONFIG |
                SetDisplayConfigFlags.SDC_SAVE_TO_DATABASE |
                SetDisplayConfigFlags.SDC_ALLOW_CHANGES;

            try
            {
                errorCode = NativeMethods.SetDisplayConfig(newPaths.Length, newPaths, newModes.Length, newModes, flags | SetDisplayConfigFlags.SDC_VALIDATE);
                if (errorCode != 0)
                {
                    Win32Exception win32Exception = new(errorCode);
                    DisplayConfigurationException exception = new($"Failed to vaildate new display configuration. \nWin32({errorCode}) {win32Exception.Message}", win32Exception);
                    //LogEventHandler.HandleEvent(new DomainLogEvent(LogLevel.Error, exception.Message));
                    throw exception;
                }

                errorCode = NativeMethods.SetDisplayConfig(newPaths.Length, newPaths, newModes.Length, newModes, flags | SetDisplayConfigFlags.SDC_APPLY );
                if (errorCode != 0)
                {
                    Win32Exception win32Exception = new(errorCode);
                    DisplayConfigurationException exception = new($"Failed to set new display configuration after successful validation. \nWin32({errorCode}) {win32Exception.Message}", win32Exception);
                    //LogEventHandler.HandleEvent(new DomainLogEvent(LogLevel.Error, exception.Message));
                    throw exception;
                }

                foreach (DISPLAYCONFIG_SET_ADVANCED_COLOR_STATE setColorState in colorStateChanges)
                {
                    DISPLAYCONFIG_SET_ADVANCED_COLOR_STATE setColorStateRef = setColorState;
                    errorCode = NativeMethods.DisplayConfigSetDeviceInfo(ref setColorStateRef);
                    if (errorCode != 0)
                    {
                        Win32Exception win32Exception = new(errorCode);
                        DisplayConfigurationException exception = new($"Failed to change HDR settings. \nWin32({errorCode}) {win32Exception.Message}", win32Exception);
                        //LogEventHandler.HandleEvent(new DomainLogEvent(LogLevel.Error, exception.Message));
                        throw exception;
                    }
                }
            }
            catch (DisplayConfigurationException ex)
            {
                errorCode = NativeMethods.SetDisplayConfig(existingPaths.Length, existingPaths, existingModes.Length, existingModes, flags | SetDisplayConfigFlags.SDC_VALIDATE);
                if (errorCode != 0)
                {
                    Win32Exception win32Exception = new(errorCode);
                    DisplayConfigurationException exception = new($"Failed to vaildate existing display configuration in preparation for rollback.\nWin32({errorCode}) {win32Exception.Message}\n{ex.Message}", win32Exception);
                    LogEventHandler.HandleEvent(new DomainLogEvent(LogLevel.Error, exception.Message));
                    throw exception;
                }

                errorCode = NativeMethods.SetDisplayConfig(existingPaths.Length, existingPaths, existingModes.Length, existingModes, flags | SetDisplayConfigFlags.SDC_APPLY);
                if (errorCode != 0)
                {
                    Win32Exception win32Exception = new(errorCode);
                    DisplayConfigurationException exception = new($"Failed to rollback to existing display configuration after successful validation.\nWin32({errorCode}) {win32Exception.Message}\n{ex.Message}", win32Exception);
                    LogEventHandler.HandleEvent(new DomainLogEvent(LogLevel.Error, exception.Message));
                    throw exception;
                }

                DisplayConfigurationException finalException = new($"{ex.Message}\nExisting configuration was succesfully rolled back.", ex.InnerException);
                LogEventHandler.HandleEvent(new DomainLogEvent(LogLevel.Error, finalException.Message));
                throw finalException;
            }
        }

        public Task ExecuteAsync(SwitchProfileCommand command)
            => Task.Run(() => Execute(command));

        private const uint DISPLAYCONFIG_PATH_MODE_IDX_INVALID = 0xffffffff;
    }
}