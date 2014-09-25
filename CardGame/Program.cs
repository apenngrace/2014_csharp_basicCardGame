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
            myCard.printCard();
            Console.ReadLine();
           
        }
    }
    class Card
    {
        public Card(int value, int suit)
        {
            cardValue = value;
            suitValue = suit;
        }
        
        List<string> suitNames = new List<string> { "club", "diamond", "heart", "spade" };
        List<string> suitChars = new List<string> { "\u2663", "\u2666", "\u2665", "\u2660" };

        private int cardValue; //card value can be between 2 to 10, J10, Q11, K12, A13
        private int suitValue;  //suit values will be 0 to 3, based on arrays above

        

        public void printCard()
        {
            int width = 25;
            string horizontalLine = new String('-', width);
            string faceString = cardValue + suitChars[suitValue];
            
            Console.WriteLine(horizontalLine);
            Console.WriteLine(cardValue);
            Console.WriteLine(suitChars[suitValue]);
            Console.WriteLine(String.Format("{0,"+ width +"}", suitChars[suitValue]));
            Console.WriteLine(String.Format("{0,"+ width +"}", cardValue, width));
            Console.WriteLine(horizontalLine);

        }
    }

    

    class Player
    {
        public string playerName;
        public long playerScore;
    }

}
