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

        

        int cardWidth = 25;
        public void printCard()
        {
            int width = 25;
            string horizontalLine = new String('-', width);
            string str = "";
            
            str += horizontalLine + "\n";
            str += cardValue + "\n";
            str += suitChars[suitValue] + "\n";
            
            str += patternString(cardValue, suitValue);

            str += String.Format("{0," + width + "}", suitChars[suitValue]) + "\n";
            str += String.Format("{0," + width + "}", cardValue, width) + "\n";
            str += horizontalLine;

            Console.WriteLine(str);
        }
        
        private string patternString(int value, int suit)
        {
            string str;

            switch (value)
            {
                case 2:
                    
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                case 7:
                    break;
                case 8:
                    break;
                case 9:
                    break;
                case 10:
                    break;
                case 11:
                    break;
                case 12:
                    break;
                case 13:
                    break;
            }
            return str;
        }
    }

    

    class Player
    {
        public string playerName;
        public long playerScore;
    }

}
