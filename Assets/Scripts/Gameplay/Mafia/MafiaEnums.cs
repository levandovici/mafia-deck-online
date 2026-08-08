namespace Mafia
{
    public enum EPlayerRole
    {
        Civilian,
        Mafia,
        Doctor,
        Commissioner
    }

    public enum EGamePhase
    {
        None,
        RoleReveal,
        DayDiscussion,
        DayVoting,
        NightPhase,
        GameOver
    }

    public enum ECardType
    {
        Accusation, // Red
        Defense,    // Green
        Analysis    // Yellow
    }
}
