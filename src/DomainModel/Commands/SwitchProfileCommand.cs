using DomainModel.Entities;

namespace DomainModel.Commands
{
    public class SwitchProfileCommand : ServiceCommand
    {
        private DisplayProfile displayProfile;

        public DisplayProfile DisplayProfile
        {
            get => displayProfile;
            set => SetProperty(ref displayProfile, value, nameof(DisplayProfile));
        }
    }
}