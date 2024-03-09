namespace Game.Models;

public enum CardValue
{
    One = 1,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Wild, // this type of card can take any value
}

public enum CardColor
{
    Red,
    Green,
    Blue,
    Yellow,
    Wild,
}

public enum MatchStatus
{
    InProgress,
    Won,
    Lost,
}