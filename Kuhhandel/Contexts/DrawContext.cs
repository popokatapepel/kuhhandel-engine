using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kuhhandel
{
    public interface DrawContext
    {
        bool isHorseTrade();
        bool isAuction();
    }

    public class HorseTradeContext : DrawContext
    {
        private HorseTradeDrawDecision decision;

        public HorseTradeDrawDecision.Status State { get { return decision.State; } }
        public PlayerContext ChallengedPlayer { get { return decision.ChallengedPlayer; } }
        public PlayerContext ChallengingPlayer { get { return decision.ChallengingPlayer; } }
        public Card Card { get { return decision.Card; } }
        public int CounterBid { get {return decision.CounterBid; } }
        public int Amount { get { return decision.Amount; } }
        public int Bid
        {
            get
            {
                if (State == HorseTradeDrawDecision.Status.CounterOffered)
                    return decision.Bid;
                return 0;
            }
        }


        public HorseTradeContext(HorseTradeDrawDecision d)
        {
            decision = d;
        }

        public bool isHorseTrade() { return true; }
        public bool isAuction() { return false; }

    }

    public class AuctionContext : DrawContext
    {
        private AuctionDrawDecision decision;

        public bool PreemptionExerted { get { return decision.PreemptionExerted; } }
        public PlayerContext Auctioneer { get { return decision.Auctioneer; } }
        public Card Card { get { return decision.Card; } }

        public IEnumerable<Tuple<PlayerContext,int>> Bids
        {
            get
            {
                foreach(Tuple<Player,int> b in decision.Bids)
                {
                    yield return new Tuple<PlayerContext, int>(b.Item1.Context, b.Item2);
                }
            }
        }

        public Tuple<PlayerContext, int> HighestBid
        { 
            get 
            {
                return new Tuple<PlayerContext, int>(decision.HighestBid.Item1.Context, decision.HighestBid.Item2);
            } 
        }

        public AuctionContext(AuctionDrawDecision d)
        {
            decision = d;
        }

        public bool isHorseTrade() { return false; }
        public bool isAuction() { return true; }
    }
}