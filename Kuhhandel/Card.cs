using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuhhandel
{
    public class Card
    {
        public static Card A = new Card(10);
        public static Card B = new Card(20);
        public static Card C = new Card(50);
        public static Card D = new Card(100);
        public static Card E = new Card(160);
        public static Card CashCow = new Card(200);
        public static Card G = new Card(500);
        public static Card H = new Card(800);
        public static Card I = new Card(900);
        public static Card J = new Card(1000);

        public int Value { get; private set; }

        private Card(int val)
        {
            Value = val;
        }

        public static IEnumerable<Card> enumerateAll()
        {
            yield return A;
            yield return B;
            yield return C;
            yield return D;
            yield return E;
            yield return CashCow;
            yield return G;
            yield return H;
            yield return I;
            yield return J;
        }
    }
}
