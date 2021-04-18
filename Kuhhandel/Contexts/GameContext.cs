using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kuhhandel
{
    public class GameContext
    {
        public IEnumerable<PlayerContext> Players
        {
            get
            {
                foreach (Player p in game.Players)
                    yield return p.Context;
            }
        }

        public List<DrawContext> PastDraws { get { return game.Draws; } }
        public PlayerContext CurrentPlayer { get { return game.CurrentPlayer.Context; } }

        public Card CurrentCard { get { return game.Talon.CurrentCard; } }
        public int TalonCount { get { return game.Talon.Count; } }
        public int CashCowsPlayed { get { return game.CashCowsPlayed; } }

        public int TotalMoney
        {
            get
            {
                return game.Players.Sum(p => p.Money);
            }
        }

        private readonly Game game;

        public GameContext(Game g)
        {
            game = g;
        }

        public int getCntOfCardInTalon(Card c)
        {
            return game.Talon.getCntOfCard(c);
        }

        public Dictionary<PlayerContext,int> getDistributionOfCard(Card c)
        {
            Dictionary<PlayerContext, int> result = new Dictionary<PlayerContext, int>();
            foreach (PlayerContext p in Players)
            {
                result.Add(p, p.getCntOfCard(c));
            }
            return result;
        }
    }
}
