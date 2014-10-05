using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CardGame.GameClasses
{
    class CardGraphics
    {
        private static Dictionary<int, GraphicString> cardVisuals;
        private static List<string> suitChars;
        private static int cardWidth;
        private static bool cardsLoaded = false;


        private static void loadCardsFromFile()
        {
            cardVisuals = new Dictionary<int, GraphicString>();
            suitChars = new List<string> { "\u2663", "\u2666", "\u2665", "\u2660" };       //club, diamond, heart, spade (lowest value to highest value)

            string cardsFile = Properties.Resources.cardFaces;          //load file to string
            GraphicString g = new GraphicString(cardsFile);             //load string to object to manage it
            int cardValue = 0;
            string cardBuffer = "";

            cardWidth = g.width;    //save to instance var

            //loop through file to build dictionary of card values and objects representing the card graphics
            for (int i = 0; i < g.height; i++)
            {
                string thisRowString = g.getRow(i);
                if (isCardHeader(thisRowString))                    //find the header for the card, and save the card of the cared
                {
                    cardValue = parseCardValue(thisRowString);
                }
                else          //build the cardBuffer string, and when it ends clear the cardBuffer
                {
                    cardBuffer += thisRowString + Environment.NewLine;      //include the new line for GraphicString to work properly
                    if (isBottomOfCard(thisRowString))
                    {
                        cardVisuals.Add(cardValue, new GraphicString(cardBuffer));
                        cardBuffer = "";
                    }
                }
            }
        }   //end Method

        private static bool isCardHeader(string str)
        {
            return (str.IndexOf("CARD") != -1);
        }

        private static int parseCardValue(string str)
        {
            int rangeStart = str.IndexOf("_") + 1;
            int rangeEnd = str.IndexOf(":");
            return Convert.ToInt32(str.Substring(rangeStart, rangeEnd - rangeStart));
        }

        private static bool isBottomOfCard(string str)
        {
            return (str.IndexOf("└") != -1);
        }

        //=========================================================
        // Visual Display of Cards
        //=========================================================

        public static void displayCardList(List<Player> players, int cursorTop)
        {
            if (!cardsLoaded)           //load the cards from file just once
                loadCardsFromFile();
            //------------------------'

            int numSpaces = players.Count + 1;

            //define amount of space between cards on screen
            int cardPaddingSpace = (int)(GameGraphics.SCREEN_WIDTH - (players.Count * cardWidth)) / numSpaces;

            //get a card to get card height.
            Card thisCard = players[0].card;
            GraphicString thisCardGraphics = cardVisuals[thisCard.cardValue];
            int height = thisCardGraphics.height;

            Console.SetCursorPosition(0, cursorTop);

            // print between 2 to 4 cards horizontally.  Print the cards, one line at a time.
            for (int i = 0; i < height; i++)    //the line number to be put on screen
            {
                for (int j = 0; j < players.Count; j++)    //the card to be printed in console
                {
                    if (players[j].card != null)
                    {
                        thisCard = players[j].card;
                        thisCardGraphics = cardVisuals[thisCard.cardValue];
                        GameGraphics.WritePadding(cardPaddingSpace);
                        printCardRow(thisCardGraphics.getRow(i), thisCard.suitValue);
                    }
                } // end for j

                Console.WriteLine();
            } // end for i

        }       //end method

        //print char by char with colors
        private static void printCardRow(string thisRowString, int suit)
        {
            bool isPenaltyCard = (suit == GameConstants.PENALTY_CARD);

            if (!isPenaltyCard)   //don't try to replace any characters if it is the penalty card
                thisRowString = thisRowString.Replace("♦", suitChars[suit]);

            bool isHeartsOrDiamonds = (suit == GameConstants.SUIT_HEARTS || suit == GameConstants.SUIT_DIAMONDS);

            for (int i = 0; i < thisRowString.Length; i++)            //write character by character to maintain the black border on red cards
            {
                bool isLineChar = ((int)thisRowString[i] >= GameGraphics.MIN_BOX_CODE && (int)thisRowString[i] <= GameGraphics.MAX_BOX_CODE);     //range for ascii chars for drawing boxes

                if (isHeartsOrDiamonds && !isLineChar)
                    GameGraphics.WriteWithColor(thisRowString[i].ToString(), ConsoleColor.Red, ConsoleColor.White);
                else
                    GameGraphics.WriteWithColor(thisRowString[i].ToString(), ConsoleColor.Black, ConsoleColor.White);
            }   //end for 
        }   //end method

        //=========================================================

    }   //end class
}
