using DomainModel.Enumerations;
using System.Collections.ObjectModel;

namespace DomainModel.Entities
{
    public class DisplayProfile : Entity
    {
        public string Name { get; private init; }

        public ObservableCollection<DisplayConfiguration> DisplayConfigurations { get; private init; }

        public DisplayProfileStatusFlags StatusFlags { get; private init; }

        public DisplayProfile(string name, IEnumerable<DisplayConfiguration> displayConfigurations, bool isLoaded)
            : this(name, displayConfigurations, isLoaded ? DisplayProfileStatusFlags.Saved : DisplayProfileStatusFlags.Active) { }

        private DisplayProfile(string name, IEnumerable<DisplayConfiguration> displayConfigurations, DisplayProfileStatusFlags statusFlags)
        {
            Name = name;
            DisplayConfigurations = new(displayConfigurations);
            StatusFlags = statusFlags;
        }

        public DisplayProfile AsActiveProfile()
            => new(Name, DisplayConfigurations, StatusFlags | DisplayProfileStatusFlags.Active);

        public DisplayProfile WithTargetAvailabilityStatus(Func<DisplayConfiguration, DisplayConfiguration> function)
        {
            IEnumerable<DisplayConfiguration> displayConfigurations = DisplayConfigurations.Select(function);

            if (displayConfigurations.All(config => config.IsTargetAvailable))
                return new(Name, displayConfigurations, StatusFlags | DisplayProfileStatusFlags.Available);
            else
                return new(Name, displayConfigurations, StatusFlags);
        }

        public bool HasMatchngDisplayConfigurations(IEnumerable<DisplayConfiguration> displayConfigurations)
        {
            if (displayConfigurations.Count() != DisplayConfigurations.Count)
                return false;
            foreach (DisplayConfiguration displayConfiguration in displayConfigurations)
                if (!DisplayConfigurations.Any(profileDisplayConfiguration => displayConfiguration.Matches(profileDisplayConfiguration)))
                    return false;
            return true;
        }

        public static readonly DisplayProfile Empty = new("Empty", Enumerable.Empty<DisplayConfiguration>(), false);
    }
}