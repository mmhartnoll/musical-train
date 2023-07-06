using DomainModel.Exceptions;

namespace DomainModel.Entities
{
    public class DisplayConfiguration : Entity
    {
        private readonly bool? isTargetAvailable;

        public uint SourceId { get; private init; }
        public string DeviceName { get; private init; }

        public uint TargetId { get; private init; }
        public string DisplayName { get; private init; }

        public uint ResolutionX { get; private init; }
        public uint ResolutionY { get; private init; }

        public int PositionX { get; private init; }
        public int PositionY { get; private init; }

        public uint VSyncNumerator { get; private init; }
        public uint VSyncDenominator {  get; private init; }

        public bool IsHdrSupported { get; private init; }
        public bool IsHdrEnabled { get; private init; }

        public bool IsTargetAvailabilityStatusKnown => isTargetAvailable.HasValue;
        public bool IsTargetAvailable => isTargetAvailable ?? throw new Exception($"Target status is unknown. Please check the value of '{nameof(IsTargetAvailabilityStatusKnown)}' before accessing '{nameof(IsTargetAvailable)}'.");

        public DisplayConfiguration(
            uint sourceId,
            string deviceName,
            uint targetId,
            string displayName,
            uint resolutionX,
            uint resolutionY,
            int positionX,
            int positionY,
            uint vSyncNumerator,
            uint vSyncDenominator,
            bool isHdrSupported,
            bool isHdrEnabled,
            bool? isTargetAvailable = null)
        {
            if (!isHdrSupported && isHdrEnabled)
                throw new DisplayConfigurationException("Configuration has HDR enabled where it is not supported.");

            SourceId = sourceId;
            DeviceName = deviceName;
            TargetId = targetId;
            DisplayName = displayName;
            ResolutionX = resolutionX;
            ResolutionY = resolutionY;
            PositionX = positionX;
            PositionY = positionY;
            VSyncNumerator = vSyncNumerator;
            VSyncDenominator = vSyncDenominator;
            IsHdrSupported = isHdrSupported;
            IsHdrEnabled = isHdrEnabled;
            this.isTargetAvailable = isTargetAvailable;
        }

        public DisplayConfiguration WithTargetAvailabilityStatus(bool isTargetAvailable, bool allowReset = false)
            => this.isTargetAvailable.HasValue && !allowReset ? 
                throw new InvalidOperationException("Availability status has already been set for this configuration.") :
                new(SourceId, DeviceName, TargetId, DisplayName, ResolutionX, ResolutionY, PositionX, PositionY, VSyncNumerator, VSyncDenominator, IsHdrSupported, IsHdrEnabled, isTargetAvailable);

        public bool Matches(DisplayConfiguration other)
        {
            return TargetId == other.TargetId &&
                DisplayName == other.DisplayName &&
                ResolutionX == other.ResolutionX &&
                ResolutionY == other.ResolutionY &&
                PositionX == other.PositionX &&
                PositionY == other.PositionY &&
                IsHdrSupported == other.IsHdrSupported &&
                IsHdrEnabled == other.IsHdrEnabled;
        }
    }
}