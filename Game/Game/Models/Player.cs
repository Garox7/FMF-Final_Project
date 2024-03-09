namespace Game.Models;
public class Player
{
    public string Email { get; set; }
    public List<Card> Hand { get; set; }

    public Player(string email)
    {
        Email = email;
        Hand = new List<Card>();
    }
}