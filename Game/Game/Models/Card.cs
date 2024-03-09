using System.Drawing;

namespace Game.Models;

public class Card
{
    public CardValue Value { get; set; }
    public CardColor Color { get; set; }
    public int Score { get; set; }
}