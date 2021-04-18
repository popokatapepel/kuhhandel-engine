using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kuhhandel
{
    public class Player
    {
        public string Name { get; private set; }
        public Game Game { get; private set; }
        public PlayerStrategy Strategy { get; private set; }
        public PlayerContext Context { get; private set; }
        public PlayerDeck Deck { get; private set; }
        public int Money { get; private set; }

        public Player(string n, Game g, PlayerStrategy s)
        {
            Name = n;
            Game = g;
            Strategy = s;
            Context = new PlayerContext(this);
            Deck = new PlayerDeck();
            Money = 0;
        }

        public void transferMoney(int amount)
        {
            Money += amount;
            if (Money < 0) 
            {
                throw new Exception("Tried to transfer not existing money");
            }   
        }
    }

    public class PlayerDeck
    {
        private Dictionary<Card, int> cards = new Dictionary<Card, int>();

        public int Score
        {
            get
            {
                return cards.Where(c => c.Value == 4).Sum(c => c.Key.Value);
            }
        }

        public PlayerDeck()
        {
            foreach (Card c in Card.enumerateAll())
            {
                cards.Add(c, 0);
            }
        }

        public bool hasOnlyQuartets()
        {
            foreach (KeyValuePair<Card, int> pair in cards)
            {
                if (pair.Value != 0 && pair.Value != 4)
                    return false;
            }
            return true;
        }

        public void transferCards(Card card, int amount = 1)
        {
            cards[card] = cards[card] + amount;
            if (cards[card] < 0 || cards[card] > 4)
                throw new Exception("Tried to transfer not existing cards");
        }

        public int getCntOfCard(Card card)
        {
            return cards[card];
        }

    }
}
