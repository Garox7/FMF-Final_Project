using System.IO.Compression;
using Game.Exceptions;
using Newtonsoft.Json;

namespace Game.Models;
public class Match
{
    public int Id { get; }
    [JsonProperty]
    public Player Player { get; private set; }
    public int PlayerScore { get; set; }
    [JsonProperty("ComputerPlayer")]
    private ComputerPlayer _computerPlayer;
    public int ComputerHandSize { get; set; }
    public int ComputerScore { get; set; }
    [JsonProperty("DrawPile")]
    private CardDeck _drawPile;
    public List<Card>[] DiscardPiles { get; set;}
    public MatchStatus Status { get; set; }
    [JsonProperty("PlayerHasDrawn")]
    private bool _playerHasDrawn;
    public int Round { get; set; }
    public bool IsDeleted {get; set;}

    [JsonConstructor]
    public Match(int id, string playerEmail)
    {
        Id = id;
        Player = new Player(playerEmail);
        _computerPlayer = new ComputerPlayer();
        ComputerHandSize = 7;
        _drawPile = new CardDeck();
        Round = 1;

        _drawPile.Shuffle();

        for (int i = 0; i < 7; i++)
        {
            Player.Hand.Add(_drawPile.Draw());
            _computerPlayer.Hand.Add(_drawPile.Draw());
        }

        DiscardPiles = new List<Card>[2];

        for (int i = 0; i < 2; i++)
        {
            DiscardPiles[i] = new List<Card>();
            Card discardCard = _drawPile.Draw();

            if (discardCard.Color is CardColor.Wild || discardCard.Value is CardValue.Wild)
            {
                _drawPile.ReturnCardToDeckAndShuffle(discardCard);
                DiscardPiles[i].Add(_drawPile.Draw());
            }
            else DiscardPiles[i].Add(discardCard);
        }
    }

    public Card DrawFromDeck()
    {
        if (Status is MatchStatus.InProgress)
        {
            if (!_playerHasDrawn)
            {
                Card cardDrawn = _drawPile.Draw();
                Player.Hand.Add(cardDrawn);
                _playerHasDrawn = true;
                return cardDrawn;
            }
            else throw new InvalidMoveException("You can only draw once per turn");
        }
        else throw new InvalidMoveException("This match is already over, you cannot make any moves");
    }

    public Match PlayTurn(List<Card> discardCards, int discardPileId = 0)
    {
        if (Status is MatchStatus.InProgress)
        {
            if (_drawPile._cards.Count <= 1)
            {
                _drawPile.ShuffleFromDiscardPiles(DiscardPiles);
            }

            // controllo se il player non ha passato il turno
            if (discardCards is not null)
            {
                /*
                * prima di iniziare tutti i controlli suelle regole
                * è necessario verificare che le carte che il player ha scartato
                * erano effettivamente contenute nella sua mano.
                */
                CheckPlayerHand(discardCards);
                // Effettua un controllo sulla validità della mossa
                CheckPlayerMove(discardCards, discardPileId);
            }

            /*
             * Se dopo aver effettuato la mossa rimangono 2 carte in mano
             * allora il match può considerarsi concluso e vinto dalla parte del player
            */
            if (Player.Hand.Count <= 2)
            {
                ConcludeRound();
                return this;
            }

            _playerHasDrawn = false;

            var (bestChoise, deckPileChoise) = _computerPlayer.PlayTurn(DiscardPiles, _drawPile);

            ComputerHandSize = _computerPlayer.Hand.Count;

            // significa che il pc non ha trovato nessuna combinazione, quindi passa il turno
            if (bestChoise is null || deckPileChoise is null)
            {
                return this;
            }

            // scarta le carte nel mazzo scelto
            DiscardPiles[(int)deckPileChoise].AddRange(bestChoise);

            // se il computer rimane con 2 o meno carte in mano allora imposta
            // lo stato del match su Lost
            if (_computerPlayer.Hand.Count <= 2)
            {
                ConcludeRound();
                return this;
            }

            return this;
        }
        else throw new InvalidMoveException("This match is already over, you cannot make any moves");
    }
    
    // ritorna vero se le carte giocate sono davvero in mano al player
    private bool CheckPlayerHand(List<Card> discardCards)
    {
        bool cardsInHand = discardCards.All(discardCard =>
            Player.Hand.Any(playerCard =>
                playerCard.Value == discardCard.Value && playerCard.Color == discardCard.Color));


        if (cardsInHand)
        {
            return true;
        }
        throw new InvalidMoveException("This card is not present in Player Hands");
    }

    // ritorna vero se la mossa effettuata dal player è consentita secondo le regole del gioco
    // tutti i metodi sotto validano la sua mossa a seconda delle diverse condizioni
    private bool CheckPlayerMove(List<Card> discardCards, int discardPileId)
    {
        int discardCardsCount = discardCards.Count;
        Card cardToMatch = DiscardPiles[discardPileId].Last();

        /*
         *  - se ne scarta solo 1:
         *      deve avere necessariamente lo stesso identico valore,
         *      può essere anche di un colore diverso.
         *      non può essere una WildCard.
        */

        if (discardCardsCount is 1)
        {
            return ValidateSingleCardMove(discardCards, cardToMatch, discardPileId);
        }

        /* 
         *  - se ne scarta 2:
         *      solo una carta scartata può essere di valore wild, e la seconda necessariamente minore del valore da metchare - 1
         *      la somma delle carte scartare è uguale alla carta in pila (qualsiasi colore)
         *      se anche il colore delle carte scartare è uguale allora l'avversario pesca.
        */

        else if (discardCardsCount is 2)
        {
            return ValidateDoubleCardMove(discardCards, cardToMatch, discardPileId);
        }
        
        if (discardCardsCount is 0)
        {
            return true;
        }
        else throw new InvalidMoveException("Invalid number of cards discarded");
    }

    private bool ValidateSingleCardMove(List<Card> discardCards, Card cardToMatch, int discardPileId)
    {
        // estraggo la carta
        Card discardCard = discardCards.Single();

        // controllo se il suo valore è uguale all' ultima carta nel mazzo dello scarto scelto
        // e che non sia una cartaWild oppure che sia una carta di diverso valore ma di uguale colore tranne un 2.
        if ((int)discardCard.Value == (int)cardToMatch.Value && discardCard.Color != CardColor.Wild ||
            discardCard.Color == cardToMatch.Color && discardCard.Value != CardValue.Wild)
        {
            HandleValidMove(discardCards, discardPileId);

            return true;
        }
        else throw new InvalidMoveException("Move not allowed");

    }

    private bool ValidateDoubleCardMove(List<Card> discardCards, Card cardToMatch, int discardPileId)
    {
        // controllo qual'è il valore da raggiungere in caso si selezionino 2 carte
        int valueToMatch = (int)cardToMatch.Value;

        // Conto quante carte Wild ci siano tra le carte scartate
        int wildCount = discardCards.Count(card => card.Value is CardValue.Wild);

        if (wildCount is 1)
        {
            return ValidateDoubleCardMoveWithWildCard(discardCards, valueToMatch, discardPileId);
        }
        else if (wildCount is 0)
        {
            return ValidateDoubleCardMoveWithoutWildCard(discardCards, valueToMatch, discardPileId);
        }
        else
        {
            throw new InvalidMoveException("It is not possible to select multiple wild cards in one move");
        }
    }

    private bool ValidateDoubleCardMoveWithWildCard(List<Card> discardCards, int valueToMatch, int discardPileId)
    {
        // contrllo se l'altra carta oltre quella wild sia minore del valore da metchare in modo che la wild possa minimo di valore 1
        if (discardCards.Any(cardWild => cardWild.Value is CardValue.Wild) && 
            discardCards.Any(card => card.Value != CardValue.Wild && card.Value != CardValue.Two && (int)card.Value < valueToMatch)
        )
        {
            HandleValidMove(discardCards, discardPileId);

            return true;
        }
        else throw new InvalidMoveException("this move is not allowed because the value reached is incorrect or because the selection is not allowed");
    }

    private bool ValidateDoubleCardMoveWithoutWildCard(List<Card> discardCards, int valueToMatch, int discardPileId)
    {
        int cardValueSum = discardCards.Sum(c => (int)c.Value);

        if (valueToMatch == cardValueSum)
        {
            HandleValidMove(discardCards, discardPileId);

            return true;
        }
        else throw new InvalidMoveException("Move not allowed");
    }

    private void HandleValidMove(List<Card> discardCards, int discardPileId)
    {
        try
        {
            // il player scarta la carta
            foreach (var discardCard in discardCards)
            {
                Card playerCardToRemove = Player.Hand.First(playerCard =>
                    playerCard.Value == discardCard.Value &&
                    playerCard.Color == discardCard.Color &&
                    playerCard.Score == discardCard.Score
                );

                Player.Hand.Remove(playerCardToRemove);
            }

            if (discardCards.Any(c => c.Value == CardValue.Two || c.Value == CardValue.Wild))
            {
                var specialCards = discardCards.Where(c =>
                    c.Value == CardValue.Two || c.Value == CardValue.Wild).ToList();
                
                discardCards.RemoveAll(c =>
                    c.Value == CardValue.Two || c.Value == CardValue.Wild);
                
                discardCards.InsertRange(0, specialCards);
            }

            // aggiungo la combinazione scartata alla pila di scarto
            DiscardPiles[discardPileId].AddRange(discardCards);
        }
        catch
        {
            throw new InvalidMoveException("Error updating match status");
        }
    }

    // questo metodo conclude il round controllando se entrambi i player
    // hanno raggiunto un punteggio di 200 (o superiore)
    private void ConcludeRound()
    {
        // Aggiorno lo score dei player
        PlayerScore += Player.Hand.Sum(card => card.Score);
        ComputerScore += _computerPlayer.Hand.Sum(card => card.Score);

        // Verifica se uno dei giocatori ha raggiunto il punteggio massimo (200)
        // se si la partita è conclusa e setto lo stato appropriato
        if (PlayerScore >= 150)
        {
            if (PlayerScore > ComputerScore)
            {
                Status = MatchStatus.Lost;
                return;
            }
        }
        else if (ComputerScore >= 150)
        {
            if (ComputerScore > PlayerScore)
            {
                Status = MatchStatus.Won;
                return;
            }
        }
        else // altrimenti ridistribuisco nuovamente le carte e comincia un nuovo round
        {
            Round += 1;
            RedistributeCards();
        }
    }

    // redistribuisce le carte facendo cominciare un nuovo round
    private void RedistributeCards()
    {
        _drawPile._cards.Clear();
        _drawPile = new CardDeck();
        _drawPile.Shuffle();

        Player.Hand.Clear();
        _computerPlayer.Hand.Clear();

        for (int i = 0; i < 7; i++)
        {
            Player.Hand.Add(_drawPile.Draw());
            _computerPlayer.Hand.Add(_drawPile.Draw());
        }

        for (int i = 0; i < 2; i++)
        {
            DiscardPiles[i].Clear();
            Card discardCard = _drawPile.Draw();

            if (discardCard.Color == CardColor.Wild || discardCard.Value == CardValue.Wild)
            {
                _drawPile.ReturnCardToDeckAndShuffle(discardCard);
                DiscardPiles[i].Add(_drawPile.Draw());
            }
            else DiscardPiles[i].Add(discardCard);
        }
        
        ComputerHandSize = 7;
    }
}