using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kuhhandel
{
    public class PlayerContext
    {
        private Player player;

        public int Score { get { return player.Deck.Score; } }

        public PlayerContext(Player p)
        {
            player = p;
        }

        public int getCntOfCard(Card card)
        {
            return player.Deck.getCntOfCard(card);
        }
    }

    public class OwnPlayerContext
    {
        private Player player;

        public GameContext Game { get { return player.Game.Context; } }
        public PlayerContext OwnPublicProperties { get { return player.Context; } }
        public int Money { get { return player.Money; } }

        public OwnPlayerContext(Player p)
        {
            player = p;
        }
    }
}
