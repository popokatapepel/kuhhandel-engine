using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuhhandel.Strategies
{
    public class SimplePlayerStrategy : PlayerStrategy
    {
        private Random rnd = new Random();
        private OwnPlayerContext PlayerContext;

        public void init(OwnPlayerContext p)
        {
            PlayerContext = p;
        }

        private int randomBid()
        {
            return rnd.Next(PlayerContext.Money);
        }

        private bool randomDecision()
        {
            return rnd.Next(1) == 1;
        }

        public string Author { get { return "Maxim Power"; } }
        public string StrategyName { get { return "Simple Randomized Behaviour"; } }

        public DrawDecision selectDraw()
        {
            if (PlayerContext.Game.TalonCount > 0)
                return DrawDecision.Auction();

            foreach (Card c in Card.enumerateAll())
            {
                int cntOfCard = PlayerContext.OwnPublicProperties.getCntOfCard(c);
                if (cntOfCard > 0 && cntOfCard < 4)
                {
                    Dictionary<PlayerContext, int> dist = PlayerContext.Game.getDistributionOfCard(c);
                    foreach (KeyValuePair<PlayerContext, int> entry in dist)
                    {
                        if (entry.Value > 0 && entry.Key != PlayerContext.OwnPublicProperties)
                        {
                            return DrawDecision.HorseTrade(entry.Key, c, randomBid());
                        }
                    }
                }
            }
            throw new Exception("No possible draw (in SimplePlayerStrategy.selectDraw()");
        }

        public int getBid(AuctionContext auctionContext)
        {
            return randomBid();
        }

        public bool exertPreemption(AuctionContext auctionContext)
        {
            return randomDecision();
        }

        public HorseTradeResponse getHorseTradeResponse(HorseTradeContext tradeContext)
        {
            if (randomDecision())
                return HorseTradeResponse.acceptOffer();
            return HorseTradeResponse.counterOffer(randomBid());
        }

        public void notifyCompletedAuction(AuctionContext auctionContext)
        {

        }

        public void notifyCompletedHorseTrade(HorseTradeContext tradeContext)
        {

        }

        public void notifyGameEnded() { }
    }
}
