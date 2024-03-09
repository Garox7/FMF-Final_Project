using System.Security.Cryptography.Xml;

namespace Game.Models;
public class ComputerPlayer
{
    public List<Card> Hand { get; private set; }

    public ComputerPlayer()
    {
        Hand = new List<Card>();
    }

    public (List<Card>? bestChoise, int? deckPileChoise) PlayTurn(List<Card>[] discardPiles, CardDeck drawPile, int recursionCount = 0)
    {
        // tango conto se il pc ha pescato
        bool hasDrawn = false;

        int? bestDeckId = null;
        List<Card>? bestCombination = null;
        int? bestScore = null;

        // cerco la combinazione migliore per ultima carta dei mazzi di scarto
        for (byte i = 0; i < discardPiles.Length; i++)
        {
            // tengo conto del mazzo scelto attraverso il suo id 
            int deckId = i;
            Card lastCard = discardPiles[i].Last();
            
            // valuto la combinazione migliore per la carta in cima al mazzo di scarto (uno dei due)
            List<Card>? currentCombination = EvalutateBestMove(lastCard);
            if (currentCombination is not null)
            {
                // sommo lo score delle carte trovate che corrispondono alla migliore combinazione trovata
                int currentScore = currentCombination.Sum(c => c.Score);

                // se hanno uno score maggiore a quello trovato in precedenza (se non nullo)
                if (currentScore > bestScore || bestCombination is null)
                {
                    bestDeckId = deckId;
                    bestCombination = currentCombination;
                    bestScore = currentScore;
                }
            }
        }

        // se non è stata trovata nessuna combinazione per entrabi i mazzi di scarto
        // allora pesco e rieseguo PlayTurn, altrimenti il computer passa il turno senza scartare.
        if (bestCombination is null && !hasDrawn && recursionCount < 1)
        {
            bestDeckId = null;
            Hand.Add(drawPile.Draw());
            hasDrawn = true;
        }

        if (hasDrawn)
        {
            var (recursiveBestChoise, recursiveBestDeckId) = PlayTurn(discardPiles, drawPile, recursionCount + 1);

            if (recursiveBestChoise is not null && recursiveBestDeckId is not null)
            {
                bestCombination = recursiveBestChoise;
                bestDeckId = recursiveBestDeckId;
            }
        }

        hasDrawn = false;
        
        if (bestCombination is not null && recursionCount is 0)
        {
            foreach (var discardCard in bestCombination)
            {
                Card computerCardToRemove = Hand.First(computerCard =>
                    computerCard.Value == discardCard.Value &&
                    computerCard.Color == discardCard.Color &&
                    computerCard.Score == discardCard.Score
                );

                Hand.Remove(computerCardToRemove);
            }

        }

        return (bestCombination, bestDeckId);
    }

    private List<Card>? EvalutateBestMove(Card targetCard)
    {
        // 1) la migliore combinazione è dove il computer può scartare un 2 (carta jolly)
        // e un'ulteriore carta che sommata al 2 è uguale al targetValue.
        if (CanDiscardTwoAndSumToTarget(targetCard))
        {
            Card twoCard = Hand.First(c => c.Value == CardValue.Two);

            if (twoCard is not null)
            {
                Card otherMatchCard = Hand.First(c => c.Value != CardValue.Two && (int)c.Value + 2 == (int)targetCard.Value);

                if (otherMatchCard is not null)
                {
                    return new List<Card> { twoCard, otherMatchCard };
                }
            }
        }

        // 2) la seconda migliore combinazione è quella in cui in computer ha una carta wild
        // e una seconda carta minore del valore target e che entrambe siano dello stesso colore target
        if (CanDiscardWildAndSumToTargetCardOfTheSameColors(targetCard))
        {
            var matchingColorCards = Hand.Where(c => c.Color == targetCard.Color);

            var selectedCard = matchingColorCards.SelectMany(card1 => matchingColorCards.Select(card2 => (card1, card2)))
                .FirstOrDefault(pair => pair.card1.Value == CardValue.Wild && 
                    pair.card2.Value != CardValue.Wild && (int)pair.card2.Value < (int)targetCard.Value);

            if (selectedCard != default)
            {
                return new List<Card> {selectedCard.card1, selectedCard.card2};
            }
        }
        
        // 3) la terza migliore combinazione è dove il computer può selezionare una carta #
        // e un'ulteriore carta che sia almeno più bassa del valore target di 1
        if (CanDiscardWildAndSumToTarget(targetCard))
        {
            Card wildCard = Hand.First(c => c.Value == CardValue.Wild);

            if (wildCard is not null)
            {
                Card otherMatchCard = Hand.First(c => c != wildCard && c.Value != CardValue.Two  && (int)c.Value < (int)targetCard.Value);

                if (otherMatchCard is not null)
                {
                    return new List<Card> { wildCard, otherMatchCard };
                }
            }
        }

        // 4) la quarta migliore combinazione è quella in cui prende 2 carte di ugual colore alla carta target
        // la cui somma è uguale al valore della stessa carta target
        if (CanDiscardTwoCardsOfTheSameColor(targetCard))
        {
            var matchingColorCards = Hand.Where(c => c.Color == targetCard.Color);

            foreach (Card card1 in matchingColorCards)
            {
                foreach (Card card2 in matchingColorCards)
                {
                    if (card1 != card2 && (int)card1.Value + (int)card2.Value == (int)targetCard.Value)
                    {
                        return new List<Card> { card1, card2 };
                    }
                }
            }
        }

        // 5) la quinta migliore combinazione è quella in cui il computer seleziona 2 carte di qualsiasi colore
        // la cui somma è uguale al valore della carta target
        if (CanDiscardTwoCards(targetCard))
        {
            foreach (Card card1 in Hand)
            {
                if (card1.Value != CardValue.Two)
                {
                    foreach (Card card2 in Hand)
                    {
                        if (card1 != card2 && card2.Value != CardValue.Two && (int)card1.Value + (int)card2.Value == (int) targetCard.Value)
                        {
                            return new List<Card> { card1, card2 };
                        }
                    }
                }
            }
        }

        // 6) la sesta migliore combinazione è quella in cui il computer seleziona 1 sola carta che abbia 
        // valore uguale alla carta target o di colore uguale
        if (Hand.Any(c => (int)c.Value == (int)targetCard.Value) || Hand.Any(c => c.Color == targetCard.Color))
        {
            var cardsByValue = Hand.Where(c => (int)c.Value == (int)targetCard.Value).ToList();
            var cardsByColor = Hand.Where(c => c.Color == targetCard.Color).ToList();

            Card? selectedCardValue = cardsByValue.FirstOrDefault();
            Card? selectedCardColor = cardsByColor.FirstOrDefault();

            Card? selectedCard = selectedCardValue?.Score >= (selectedCardColor?.Score ?? 0) ? selectedCardValue : selectedCardColor;

            if (selectedCard is not null)
            {
                return new List<Card> { selectedCard };
            }
            else return null;
        }
        else return null;
    }

    // vero se il computer ha in mano un 2 e un'ulteriore carta 
    // che sia minore di 2 rispetto al valore della carta target
    // in modo tale che la somma tra le due sia sicuramente uguale al valore della carta target
    private bool CanDiscardTwoAndSumToTarget(Card targetCard)
    {
        return Hand.Any(c1 => c1.Value == CardValue.Two) &&
            Hand.Any(c2 => c2.Value != CardValue.Two &&
            c2.Value != CardValue.Wild && (int)c2.Value + 2 == (int)targetCard.Value);
    }

    // ritorna vero se il computer ha in mano una carta wild, un'ulteriore carta il cui valore è minore
    // della carta target (in modo che la carta wild, #, possa valere almeno 1) e che entrambe siano 
    // dello stesso colore della carta target.
    private bool CanDiscardWildAndSumToTargetCardOfTheSameColors (Card targetCard)
    {
        var matchingColorCards = Hand.Where(c => c.Color == targetCard.Color);

        return matchingColorCards.Any(c1 => c1.Value == CardValue.Wild) &&
            matchingColorCards.Any(c2 => c2.Value != CardValue.Wild && (int)c2.Value < (int)targetCard.Value);
    }

    // ritorna vero se il computer ha in mano una carta Wild e un'ulteriore carta
    // che sia almeno minore di 1 rispetto al valore della carta target
    // in modo tale che la somma tra le due sia sicuramente uguale al valore della carta target
    private bool CanDiscardWildAndSumToTarget(Card targetCard)
    {
        return Hand.Any(c1 => c1.Value == CardValue.Wild) &&
            Hand.Any(c2 => c2.Value != CardValue.Wild && 
                c2.Value != CardValue.Two && (int)c2.Value < (int)targetCard.Value);
    }

    // ritorna vero se il computer ha in mano due carte diverse tra loro che siano dello stesso
    // colore della carta target ma che sommate diano il suo valore
    private bool CanDiscardTwoCardsOfTheSameColor(Card targetCard)
    {
        var matchingColorCards = Hand.Where(c => c.Color == targetCard.Color);

        return matchingColorCards.Any(c1 => 
            matchingColorCards.Any(c2 => 
            c1 != c2 && (int)c1.Value + (int)c2.Value == (int)targetCard.Value));
    }

    // ritorna vero se il computer ha in mano due carte di qualsiasi colore che sommate
    // diano il valore della carta target.
    private bool CanDiscardTwoCards(Card targetCard)
    {
        return Hand.Any(c1 => 
            Hand.Any(c2 =>
            c1 != c2 && (int)c1.Value + (int)c2.Value == (int)targetCard.Value));
    }
}