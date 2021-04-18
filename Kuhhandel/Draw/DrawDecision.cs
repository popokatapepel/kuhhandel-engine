using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kuhhandel
{
    public abstract class DrawDecision
    {
        protected DrawDecision() { }

        public static DrawDecision HorseTrade(PlayerContext player, Card card, int bid)
        {
            return new HorseTradeDrawDecision(player,card,bid);
        }

        public static DrawDecision Auction()
        {
            return new AuctionDrawDecision();
        }

        public abstract DrawContext execute(Game Game);
        public abstract bool isValid(Game Game);
    }

    public sealed class HorseTradeDrawDecision : DrawDecision
    {
        public enum Status { Offered, Accepted, CounterOffered }

        public HorseTradeContext Context { get; private set; }

        public Status State { get; private set; }
        public PlayerContext ChallengedPlayer { get; private set; }
        public PlayerContext ChallengingPlayer { get; private set; }
        public Card Card { get; private set; }
        public int Bid { get; private set; }
        public int CounterBid { get; private set; }
        public int Amount { get; private set; }

        public HorseTradeDrawDecision(PlayerContext challenged, Card c, int bid)
        {
            Context = new HorseTradeContext(this);
            State = Status.Offered;
            ChallengedPlayer = challenged;
            Card = c;
            Bid = bid;
            CounterBid = 0;
        }

        public override bool isValid(Game Game)
        {
            return Game.CurrentPlayer.Money >= Bid
                && Game.CurrentPlayer.Deck.getCntOfCard(Card) > 0
                && Game.getPlayerFromContext(ChallengedPlayer).Deck.getCntOfCard(Card) > 0;
        }

        public override DrawContext execute(Game Game)
        {
            ChallengingPlayer = Game.CurrentPlayer.Context;
            Player challenged = Game.getPlayerFromContext(ChallengedPlayer);
            Amount = Math.Min(Game.CurrentPlayer.Deck.getCntOfCard(Card), challenged.Deck.getCntOfCard(Card));
            HorseTradeResponse response = challenged.Strategy.getHorseTradeResponse(Context);
            if (response == null || response.GetType() != typeof(HorseTradeResponse))
                throw new Exception("Player tried to cheat");

            State = response.State;
            if(State == Status.Accepted)
            {
                Game.CurrentPlayer.transferMoney(-Bid);
                challenged.transferMoney(Bid);
                Game.CurrentPlayer.Deck.transferCards(Card, Amount);
                challenged.transferMoney(-Amount);
            }
            else
            {
                CounterBid = (response.Bid > challenged.Money) ? challenged.Money : response.Bid;
                if (CounterBid < 0) CounterBid = 0;
                Player winner = (CounterBid > Bid) ? challenged : Game.CurrentPlayer;
                Player looser = (winner == challenged) ? Game.CurrentPlayer : challenged;
                winner.Deck.transferCards(Card, Amount);
                looser.Deck.transferCards(Card, -Amount);
                challenged.transferMoney(Bid - CounterBid);
                Game.CurrentPlayer.transferMoney(CounterBid - Bid);
            }
            Game.Players.ForEach(p => p.Strategy.notifyCompletedHorseTrade(Context));
            return Context;
        }
    }

    public sealed class AuctionDrawDecision : DrawDecision
    {
        public AuctionContext Context { get; private set; }

        public PlayerContext Auctioneer { get; private set; }
        public List<Tuple<Player, int>> Bids { get; private set; }
        public bool PreemptionExerted { get; private set; }
        public Card Card { get; private set; }

        public Tuple<Player, int> HighestBid 
        { 
            get 
            { 
                return Bids.Count > 0 ? Bids[Bids.Count - 1] : new Tuple<Player,int>(null,0); 
            } 
        }

        public AuctionDrawDecision()
        {
            Context = new AuctionContext(this);
            Bids = new List<Tuple<Player, int>>();
        }

        public override bool isValid(Game Game)
        {
            return Game.Talon.Count > 0;
        }

        public override DrawContext execute(Game Game)
        {
            Auctioneer = Game.CurrentPlayer.Context;
            Card = Game.Talon.CurrentCard;
            if(Card == Card.CashCow)
            {
                Game.performPayout();
            }

            int passCount = 0;
            for (int currentBidderNr = getFollowingBidderNr(Game.CurrentPlayerNr, Game);
                passCount < Game.Players.Count - 1;
                currentBidderNr = getFollowingBidderNr(currentBidderNr, Game))
            {
                Player currentBidder = Game.Players[currentBidderNr];
                int bid = currentBidder.Strategy.getBid(Context);
                if (bid > currentBidder.Money)
                    bid = currentBidder.Money;
                if (bid <= HighestBid.Item2 || bid < 0)
                    bid = 0;
                
                if (bid == 0)
                {
                    passCount++;
                }
                else
                {
                    Bids.Add(new Tuple<Player, int>(currentBidder, bid));
                    passCount = 0;
                }
            }

            Player highestBidder = HighestBid.Item1;
            if (highestBidder == null)
            {
                PreemptionExerted = true;
                Game.CurrentPlayer.Deck.transferCards(Game.Talon.CurrentCard);
            }
            else
            {
                int cost = HighestBid.Item2;
                if (Game.CurrentPlayer.Money >= cost
                    && Game.CurrentPlayer.Strategy.exertPreemption(Context))
                {
                    PreemptionExerted = true;
                    Game.CurrentPlayer.transferMoney(-cost);
                    highestBidder.transferMoney(cost);
                    Game.CurrentPlayer.Deck.transferCards(Game.Talon.CurrentCard);
                }
                else
                {
                    PreemptionExerted = false;
                    Game.CurrentPlayer.transferMoney(cost);
                    HighestBid.Item1.transferMoney(-cost);
                    highestBidder.Deck.transferCards(Game.Talon.CurrentCard);

                }
            }
            Game.Talon.removeTopCard();
            Game.Players.ForEach(p => p.Strategy.notifyCompletedAuction(Context));
            return Context;
        }

        private int getFollowingBidderNr(int currentBidder, Game Game)
        {
            int following = (currentBidder + 1) % Game.Players.Count;
            if (following == Game.CurrentPlayerNr) 
                return getFollowingBidderNr(following, Game) ;
            return following;
        }
    }
}
