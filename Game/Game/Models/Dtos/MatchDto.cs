using Newtonsoft.Json;

namespace Game.Models;
public class MatchDto
{
    public int Id { get; set; }
    public List<Card> DiscardCards { get; set; }
    public int DiscardPileId { get; set; }

    [JsonConstructor]
    public MatchDto(List<Card> discardCards, int discardPileId) 
    {
        DiscardCards = discardCards;
        DiscardPileId = discardPileId;
    }
}