using DomainModel.Entities;
using System.ComponentModel.DataAnnotations;
using Tools.Extensions;

namespace DomainModel.Commands
{
    public class SaveProfileCommand : ServiceCommand
    {
        private string profileName = string.Empty;
        private List<DisplayConfiguration> displayConfigurations = new();
        private bool allowOverwrite = false;

        [MinLength(1, ErrorMessage = "Profile name cannot be empty.")]
        [StringLength(24, ErrorMessage = "Profile name is too long.")]
        public string ProfileName
        {
            get => profileName;
            set => SetProperty(ref profileName, value, nameof(ProfileName));
        }

        [MinLength(1, ErrorMessage = "You must include at least one display.")]
        public IEnumerable<DisplayConfiguration> DisplayConfigurations
        {
            get => displayConfigurations;
            set => SetProperty(ref displayConfigurations, value.ToList(), nameof(DisplayConfigurations));
        }

        public bool AllowOverwrite
        {
            get => allowOverwrite;
            set => SetProperty(ref allowOverwrite, value, nameof(AllowOverwrite));
        }
    }
}