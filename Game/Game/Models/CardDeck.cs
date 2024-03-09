using Game.Exceptions;

namespace Game.Models;
public class CardDeck
{
    public List<Card> _cards { get; private set;}

    public CardDeck()
    {
        _cards = new List<Card>();

        // in each deck there are 3 cards from 1 to 5 for each color (excluding 2, a wild card)
        // 2 cards of 6 to Wild Card for each color
        // and three cards with the value "2" for each color (which represents the wild), with the wild type color
        foreach (CardColor color in Enum.GetValues(typeof(CardColor)))
        {
            if (color != CardColor.Wild)
            {
                foreach (CardValue value in Enum.GetValues(typeof(CardValue)))
                {
                    switch (value)
                    {   // 3 cards from 1 to 5 for each color (excluding 2, a wild card)
                        case CardValue.One:
                        case CardValue.Three:
                        case CardValue.Four:
                        case CardValue.Five:
                            
                            for (int i = 0; i < 3; i++)
                            {
                                _cards.Add(new Card()
                                {
                                    Color = color,
                                    Value = value,
                                    Score = (int)value,
                                });
                            }
                        break;

                        // 2 cards of 6 to Wild Card for each color
                        case CardValue.Six:
                        case CardValue.Seven:
                        case CardValue.Eight:
                        case CardValue.Nine:
                        case CardValue.Ten:

                            for (int i = 0; i < 2; i++)
                            {
                                _cards.Add(new Card()
                                {
                                    Color = color,
                                    Value = value,
                                    Score = (int)value,
                                });
                            }
                        break;
                        // cards with a wild value have a higher score
                        case CardValue.Wild:
                        
                            for (int i = 0; i < 2; i++)
                            {
                                _cards.Add(new Card()
                                {
                                    Color = color,
                                    Value = value,
                                    Score = 20,
                                });
                            }
                        break;
                    }
                }
            }
            else // three cards with the value "2" for each color (which represents the wild), with the wild type color
            {
                for (int i = 0; i < 12; i++)
                {
                    _cards.Add(new Card() 
                    {
                        Color = CardColor.Wild,
                        Value = CardValue.Two,
                        Score = 50,
                    });
                }
            }
        }
    }

    public void Shuffle()
    {
        Random r = new();

        for (int i = _cards.Count - 1; i >= 1 ; i--)
        {
            int k = r.Next(i + 1); 
            (_cards[k], _cards[i]) = (_cards[i], _cards[k]);
        }
    }

    public void ShuffleFromDiscardPiles(List<Card>[] discardPiles)
    {
        // the two discard decks are drawn
        List<Card> allDiscardCards = discardPiles.SelectMany(cards => cards).ToList();

        _cards.AddRange(allDiscardCards);

        Shuffle();
    }

    public Card Draw()
    {
        if (_cards.Count == 0)
        {
            throw new EmptyDeckException("Deck is Empty");
        }

        Card drawnCard = _cards.First();
        _cards.Remove(drawnCard);

        return drawnCard;
    }

    public void ReturnCardToDeckAndShuffle(Card card)
    {
        _cards.Add(card);
        Shuffle();
    }
}