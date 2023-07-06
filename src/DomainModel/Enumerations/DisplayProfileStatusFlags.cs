namespace DomainModel.Enumerations
{
    [Flags]
    public enum DisplayProfileStatusFlags
    {
        Active      = 0x1,
        Saved       = 0x2,
        Available   = 0x4,
        Unavailable = 0x8,
    }
}