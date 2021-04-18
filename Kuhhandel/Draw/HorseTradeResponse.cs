using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kuhhandel
{
    public sealed class HorseTradeResponse
    {
        public HorseTradeDrawDecision.Status State { get; private set; }
        public int Bid { get; private set; }

        private HorseTradeResponse(HorseTradeDrawDecision.Status state, int bid = 0) 
        { 
            State = state;
            Bid = bid;
        }
 
        public static HorseTradeResponse acceptOffer()
        {
            return new HorseTradeResponse(HorseTradeDrawDecision.Status.Accepted);
        }

        public static HorseTradeResponse counterOffer(int offer)
        {
            return new HorseTradeResponse(HorseTradeDrawDecision.Status.CounterOffered, offer);
        }
    }
}
