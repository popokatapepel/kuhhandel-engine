using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuhhandel
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Game g = new Game();
                g.setup();
                g.play();
                g.showScores();
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exection interrupted by exception:");
                Console.WriteLine(e.ToString());
            }
        }
    }
}
