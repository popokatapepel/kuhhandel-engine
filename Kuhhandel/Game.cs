using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSScriptLibrary;
using System.IO;

namespace Kuhhandel
{
    public class Game
    {
        public GameContext Context { get; private set; }
        public List<Player> Players { get; private set; }
        public int CurrentPlayerNr { get; private set; }
        public List<DrawContext> Draws { get; private set; }
        public Talon Talon { get; private set; }
        public int CashCowsPlayed { get; private set; }

        public Player CurrentPlayer { get { return Players[CurrentPlayerNr]; } }
        public Player getPlayerFromContext(PlayerContext context)
        {
            return Players.First(p => p.Context == context);
        }

        public Game()
        {
            Context = new GameContext(this);
            CurrentPlayerNr = -1;
            Draws = new List<DrawContext>();
            Talon = new Talon();
            CashCowsPlayed = 0;
        }

        public void setup()
        {
            int nrPlayers;
            string input;
            do
            {
                Console.WriteLine("Wieviele Spieler? (3-5)");
                input = Console.ReadLine();
            } while (!isValidNrOfPlayers(input, out nrPlayers));

            Players = new List<Player>(nrPlayers);

            List<string> strategyFiles = new List<string>();
            DirectoryInfo dir = new DirectoryInfo("Strategies");
            dir.EnumerateFiles().ToList().ForEach(f => strategyFiles.Add(f.Name));

            Console.WriteLine();
            Console.WriteLine("Available Strategies:");
            for (int i = 0; i < strategyFiles.Count; i++ )
            {
                try
                {
                    PlayerStrategy s = CSScript.Evaluator.LoadFile<PlayerStrategy>(Path.Combine("Strategies",strategyFiles[i]));
                    Console.WriteLine("Strategy [" + i + "]: " + s.StrategyName + " by " + s.Author);
                }
                catch (Exception e)
                {
                    throw new Exception("Error interpreting PlayerStrategy " + strategyFiles[i], e);
                }
            }

            Console.WriteLine();
            for (int i = 0; i < nrPlayers; i++)
            {
                int selectedStratNr;
                do
                {
                    Console.Write("Strategy of Player " + (i + 1) + "? ");
                    input = Console.ReadLine();
                } while (!(Int32.TryParse(input, out selectedStratNr)
                    && selectedStratNr >= 0 && selectedStratNr < strategyFiles.Count));
                
                PlayerStrategy selectedStrategy = CSScript.Evaluator.LoadFile<PlayerStrategy>
                    (Path.Combine("Strategies",strategyFiles[selectedStratNr]));
                Player p = new Player("Player " + (i + 1).ToString(), this, selectedStrategy);
                p.Strategy.init(new OwnPlayerContext(p));
                p.transferMoney(90);
                Players.Add(p);
            }
        }

        public void play()
        {
            while(!isFinished())
            {
                executeDraw();
            }
        }

        public void showScores()
        {
            Console.WriteLine();
            Console.WriteLine(" Score   Player");
            Console.WriteLine("------------------\\");
            foreach(Player p in Players.OrderByDescending(p => p.Deck.Score))
            {
                Console.Write(String.Format("  {0,4}   ", p.Deck.Score));
                Console.WriteLine(p.Name + " | " + p.Strategy.StrategyName + " by " + p.Strategy.Author);
            }
            Console.WriteLine("------------------/");
        }

        private void executeDraw()
        {
            CurrentPlayerNr = (CurrentPlayerNr + 1) % Players.Count;
            if (Talon.Count > 0 || !CurrentPlayer.Deck.hasOnlyQuartets())
            {
                DrawDecision decision = CurrentPlayer.Strategy.selectDraw();
                if (!isDrawDecisionValid(decision))
                    throw new Exception("Player " + CurrentPlayerNr + " tried to cheat");
                Draws.Add(decision.execute(this));
            }
        }



        private bool isFinished()
        {
            return (Talon.Count == 0 && Players.TrueForAll(p => p.Deck.hasOnlyQuartets()));
        }

        private bool isValidNrOfPlayers(string input, out int nr)
        {
            if (Int32.TryParse(input, out nr))
            {
                if (nr >= 3 && nr <= 5)
                    return true;
            }
            return false;
        }

        private bool isDrawDecisionValid(DrawDecision decision)
        {
            if (decision == null) return false;

            Type t = decision.GetType();
            return (t == typeof(HorseTradeDrawDecision) || t == typeof(AuctionDrawDecision))
                && (decision.isValid(this));
        }


        public void performPayout()
        {
            switch (CashCowsPlayed)
            {
                case 0: Players.ForEach(p => p.transferMoney(50));
                    break;
                case 1: Players.ForEach(p => p.transferMoney(100));
                    break;
                case 2: Players.ForEach(p => p.transferMoney(200));
                    break;
                case 3: Players.ForEach(p => p.transferMoney(500));
                    break;
                default:
                    throw new Exception("Too many cashcows played");
            }
            CashCowsPlayed++;
        }
    }

    public class Talon
    {
        List<Card> stack;

        public Card CurrentCard { get { return stack[0]; } }
        public int Count { get { return stack.Count; } }

        public Talon()
        {
            stack = new List<Card>();
            foreach (Card c in Card.enumerateAll())
            {
                for (int i = 0; i < 4; i++)
                {
                    stack.Add(c);
                }
                new AuctionDrawDecision();
            }
            stack.Shuffle();
        }

        public int getCntOfCard(Card c)
        {
            return stack.Count(t => t == c);
        }

        public void removeTopCard()
        {
            stack.RemoveAt(0);
        }
    }
}