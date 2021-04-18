using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kuhhandel
{
    public interface PlayerStrategy
    {
        //Metainfos zur Stategie
        string Author { get; }
        string StrategyName { get; }

        //Der Context sollte im Strategie-Objekt gespeichert werden
        void init(OwnPlayerContext p);

        //Spielzüge
        DrawDecision selectDraw(); //Spieler ist an der Reihe --> Zug auswählen (Kuhhandel oder Auktion?)
        int getBid(AuctionContext auctionContext); //Auktion im Gange. Gebot abgeben
        bool exertPreemption(AuctionContext auctionContext); //Vorkaufsrecht anwenden (ja/nein)
        HorseTradeResponse getHorseTradeResponse(HorseTradeContext tradeContext); //Antwort auf einen vorgeschlagenen Kuhhandel (Angebot annehmen oder Gegenvorschlag?)

        //Verschiedene Benachrichtigungen. Optional für Strategieverbesserungen
        void notifyCompletedAuction(AuctionContext auctionContext);
        void notifyCompletedHorseTrade(HorseTradeContext tradeContext);
        void notifyGameEnded(); //Auch als Reset zwischen Spielen gedacht.
    }
}
