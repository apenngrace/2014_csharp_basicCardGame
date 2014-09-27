using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Card myCard = new Card(10,2);
            //myCard.printCard();
            //Console.ReadLine();
           
        }
    }

    class Card
    {
        private int myCardValue; //card value can be between 2 to 10, J10, Q11, K12, A13
        private int mySuitValue;  //suit values will be 0 to 3, based on arrays above

        public Card(int value, int suit)
        {
            myCardValue = value;
            mySuitValue = suit;
        }

        public int cardValue
        {
            get { return myCardValue; }
        }
        public int suitValue
        {
            get { return mySuitValue; }
        }
        
        //List<string> suitNames = new List<string> { "club", "diamond", "heart", "spade" };
        //List<string> suitChars = new List<string> { "\u2663", "\u2666", "\u2665", "\u2660" };

        

        

        //int cardWidth = 25;
        //public void printCard()
        //{
        //    int width = 25;
        //    string horizontalLine = new String('-', width);
        //    string str = "";
            
        //    str += horizontalLine + "\n";
        //    str += cardValue + "\n";
        //    str += suitChars[suitValue] + "\n";
            
        //    str += patternString(cardValue, suitValue);

        //    str += String.Format("{0," + width + "}", suitChars[suitValue]) + "\n";
        //    str += String.Format("{0," + width + "}", cardValue, width) + "\n";
        //    str += horizontalLine;

        //    Console.WriteLine(str);
        //}
        
        //private string patternString(int value, int suit)
        //{
        //    string str;

        //    switch (value)
        //    {
        //        case 2:
                    
        //            break;
        //        case 3:
        //            break;
        //        case 4:
        //            break;
        //        case 5:
        //            break;
        //        case 6:
        //            break;
        //        case 7:
        //            break;
        //        case 8:
        //            break;
        //        case 9:
        //            break;
        //        case 10:
        //            break;
        //        case 11:
        //            break;
        //        case 12:
        //            break;
        //        case 13:
        //            break;
        //    }
        //    return str;
        //}
    }

    class CardView
    {
        static Dictionary<int, string> cardVisuals;
        static bool visualsSetup = false;
                
        public static void displayCard(Card thisCard)
        {
            if (!visualsSetup)
            { setupVisuals(); }

            Console.WriteLine(cardVisuals[thisCard.]);
            
        }
        private static void setupVisuals()
        {
            cardVisuals = new Dictionary<int, string>();
            
            string str = "";
            str = @"┌───────┐
                    │2      │
                    │♦      │
                    │       │
                    │   ♦   │
                    │       │
                    │       │
                    │   ♦   │
                    │       │
                    │      ♦│
                    │      2│
                    └───────┘";
            cardVisuals.Add(2,str);
            str = @"┌───────┐
                    │3      │
                    │♦      │
                    │       │
                    │   ♦   │
                    │       │
                    │   ♦   │
                    │       │
                    │   ♦   │
                    │      ♦│
                    │      3│
                    └───────┘";
            cardVisuals.Add(3, str);

            str = @"┌───────┐
                    │4      │
                    │♦      │
                    │       │
                    │  ♦ ♦  │
                    │       │
                    │       │
                    │  ♦ ♦  │
                    │       │
                    │      ♦│
                    │      4│
                    └───────┘";
            cardVisuals.Add(4, str);
            str = @"┌───────┐
                    │5      │
                    │♦      │
                    │       │
                    │  ♦ ♦  │
                    │       │
                    │   ♦   │
                    │       │
                    │  ♦ ♦  │
                    │      ♦│
                    │      5│
                    └───────┘";
            cardVisuals.Add(5, str);
            str = @"┌───────┐
                    │6      │
                    │♦      │
                    │       │
                    │  ♦ ♦  │
                    │  ♦ ♦  │
                    │  ♦ ♦  │
                    │       │
                    │       │
                    │      ♦│
                    │      6│
                    └───────┘";
            cardVisuals.Add(6, str);
            str = @"┌───────┐
                    │7      │
                    │♦      │
                    │       │
                    │  ♦ ♦  │
                    │ ♦ ♦ ♦ │
                    │  ♦ ♦  │
                    │       │
                    │       │
                    │      ♦│
                    │      7│
                    └───────┘";
            cardVisuals.Add(7, str);
            str = @"┌───────┐
                    │9      │
                    │♦      │
                    │       │
                    │  ♦ ♦  │
                    │ ♦ ♦ ♦ │
                    │ ♦   ♦ │
                    │  ♦ ♦  │
                    │       │
                    │      ♦│
                    │      9│
                    └───────┘";
            cardVisuals.Add(8, str);
            str = @"┌───────┐
                    │10     │
                    │♦♦     │
                    │       │
                    │  ♦ ♦  │
                    │ ♦ ♦ ♦ │
                    │ ♦ ♦ ♦ │
                    │  ♦ ♦  │
                    │       │
                    │     ♦♦│
                    │     10│
                    └───────┘";
            cardVisuals.Add(9, str);
            str = @"┌───────┐
                    │J      │
                    │♦      │
                    │       │
                    │ ♦ ♦ ♦ │
                    │   ♦   │
                    │   ♦   │
                    │ ♦ ♦   │
                    │  ♦♦   │
                    │      ♦│
                    │      J│
                    └───────┘";
            cardVisuals.Add(10, str);
            str = @"┌───────┐
                    │Q      │
                    │♦      │
                    │       │
                    │ ♦ ♦ ♦ │
                    │ ♦   ♦ │
                    │ ♦   ♦ │
                    │ ♦ ♦ ♦ │
                    │    ♦  │
                    │    ♦ ♦│
                    │      Q│
                    └───────┘";
            cardVisuals.Add(11, str);
            str = @"┌───────┐
                    │K      │
                    │♦      │
                    │       │
                    │ ♦  ♦  │
                    │ ♦ ♦   │
                    │ ♦♦    │
                    │ ♦ ♦   │
                    │ ♦  ♦  │
                    │      ♦│
                    │      K│
                    └───────┘";
            cardVisuals.Add(12, str);

            str = @"┌───────┐
                    │A      │
                    │♦      │
                    │   ♦   │
                    │ ♦   ♦ │
                    │ ♦   ♦ │
                    │ ♦ ♦ ♦ │
                    │ ♦   ♦ │
                    │ ♦   ♦ │
                    │      ♦│
                    │      A│
                    └───────┘";
            cardVisuals.Add(13, str);

            visualsSetup = true;
        }
        
    }

    

    class Player
    {
        public string playerName;
        public long playerScore;
    }

}
