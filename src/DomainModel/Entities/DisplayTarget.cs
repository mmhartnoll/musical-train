namespace DomainModel.Entities
{
    public class DisplayTarget
    {
        public uint Id { get; private init; }
        public string DisplayName { get; private init; }

        public bool IsAvailable { get; private init; }

        public DisplayTarget(uint id, string displayName, bool isAvailable)
        {
            Id = id;
            DisplayName = displayName;
            IsAvailable = isAvailable;
        }
    }
}