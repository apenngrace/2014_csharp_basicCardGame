using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CardGame
{
    class Program
    {
        static void Main(string[] args)
        {
            //for (int j = 0; j < 4; j++)
            //    for (int i = 2; i <= 14; i++)
            //    {
            //        Card myCard = new Card(i, j);
            //        CardView.displayCard(myCard);
            //        Console.ReadLine();

            //        Console.Clear();
            //    }

            //for (int i = 0; i < 8; i++)
            //{
            //    for (int j = 0; j < 10; j++)
            //        Console.Write(j);
            //}
                
            //        Console.ReadLine();

            //List<Card> cards = new List<Card>();
            //Deck thisDeck = new Deck();
            //for (int i = 0; i < 50; i++ )
            //    thisDeck.shuffle();
            

            ////cards.Add(new Card(14, 0));
            ////cards.Add(new Card(10, 1));
            ////cards.Add(new Card(5, 2));
            ////cards.Add(new Card(-1, -1));

            //cards.Add(thisDeck.nextCard());
            //cards.Add(thisDeck.nextCard());
            //cards.Add(thisDeck.nextCard());
            //cards.Add(thisDeck.nextCard());
            
            //CardView.displayCardList(cards);
            //Console.ReadLine();
            
            //myCard.printCard();

            Game thisGame = new Game();
            thisGame.startGame();              
        }
    }

    class Game
    {
        private int roundCount = 0;
        private int numPlayers = 2;     //default number of players, can be between 2 and 4
        private List<Player> players;   //array of players

        public void startGame()
        {
            GameGraphics.splashScreen();
            numPlayers = GameGraphics.getNumPlayers();    //get the number of players for the game
            //gameLoop();         //begin the game loop
        }


        private void gameLoop()
        {

        }


    }

    class Player
    {
        public string id {get; set;}
        public string playerName {get; set;}
        public long  playerScore {get; set;}        
    }

    class Deck
    {
        static int lowestCard = 2; 
        static int highestCard = 14;
        static int numSuits = 4;
        static int currentCardIndex = 0;        //use a pointer to position top of deck.
        
        List<Card> cardDeck = new List<Card>();

        public Deck()
        {
            currentCardIndex = 0;
            // the deck has 13 cards per suit, 4 suits each (52 cards)
            // plus there are 2 special penalty cards

            for (int j = 0; j < numSuits; j++)
            {
                for (int i = lowestCard; i <= highestCard; i++)
                {
                    cardDeck.Add(new Card(i,j));
                }
            }

            //the 2 penalty cards
            cardDeck.Add(new Card(-1, -1));
            cardDeck.Add(new Card(-1, -1));

        }   //end constructor      

        public Card nextCard()      //grab cards from current top of deck, and if deck ends then start from beginning again.
        {
            Card thisCard;
            thisCard = cardDeck[currentCardIndex];

            if (currentCardIndex + 1 > cardDeck.Count)
            {
                currentCardIndex = 0;
            }
            else
            {
                currentCardIndex++;
            }            
            return thisCard;
        }
 
        public void shuffle()   //shuffle the deck of cards
        {
            int numCards = cardDeck.Count;
            int highestIndex = numCards - 1;
            List<Card> tempDeck = new List<Card>();

            Random randomGenerator = new Random();
            
            for (int i = 0; i < numCards; i++)
            {
                tempDeck.Add(null);
            }

            for (int i = 0; i < numCards; i++)
            {
                int someNumber = randomGenerator.Next(0, highestIndex);
                if (tempDeck[someNumber] == null)
                {
                    //tempDeck.Insert(someNumber, cardDeck[i]);   //insert the card at the random spot
                    tempDeck[someNumber] = cardDeck[i];
                }
                else   //insert at next closest spot in tempDeck
                {
                    //get next index to try
                    if (someNumber + 1 <= highestIndex)
                        someNumber++;
                    else
                        someNumber = 0;

                    bool done = false;
                    do 
                    {
                        if (tempDeck[someNumber] == null)
                        {
                            //tempDeck.Insert(someNumber, cardDeck[i]);
                            tempDeck[someNumber] = cardDeck[i];
                            done = true;
                        }
                        else
                        {
                            if (someNumber + 1 <= highestIndex)
                                someNumber++;
                            else
                                someNumber = 0;
                        }
                    } while (!done);  // end do loop
                }
            }

            cardDeck = tempDeck;
        
        }   //end shuffle       
    }


    class Card
    {
        private int myCardValue; //card value can be between 2 to 10, J10, Q11, K12, A13
        private int mySuitValue;  //suit values will be 0 to 3, based on arrays above

        public Card(int value, int suit)
        {
            myCardValue = value;
            mySuitValue = suit;             //suits: 0 club, 1 diamond, 2 heart, 3 spade
        }

        public int cardValue
        {
            get { return myCardValue; }
        }
        public int suitValue
        {
            get { return mySuitValue; }
        } 
    }   //end Card class

    class CardView
    {
        static Dictionary<int, string> cardVisuals;
        static bool visualsSetup = false;
        static List<string> suitChars;
        static int cardWidth = GameGraphics.cardWidth;
        static int cardHeight = GameGraphics.cardHeight;
        static int screenWidth = GameGraphics.screenWidth;            

        public static void displayCardList(List<Card> cardList)
        {
            if (!visualsSetup)
            { setupVisuals(); }
            
            //int screenWidth = Console.LargestWindowWidth;                        
            int numSpaces = cardList.Count + 1;
            int numCards = cardList.Count;
            int spacesWidth = (int)(screenWidth - (numCards * cardWidth)) / numSpaces;   //define amount of space between cards on screen
            
            
            for (int i = 0; i < cardHeight; i++)      //loop through each line to draw
            {
                for (int j = 0; j < numCards; j++)      //loop through each card to draw
                {
                    Card thisCard = cardList[j];
                    Console.Write(GameGraphics.repeatString(" ", spacesWidth));     //start with padding on the line
                    printCardRow(thisCard, i);                              //print the row with colors
                }
                Console.WriteLine();    //get to new line
            }            

        }       //end method

        //print char by char with colors
        private static void printCardRow(Card thisCard, int row)
        {
            string thisRow = cardVisuals[thisCard.cardValue];
            int startPoint = row * (cardWidth + Environment.NewLine.Length);
            thisRow = thisRow.Substring(startPoint, cardWidth);

            if (thisCard.suitValue != -1)   //don't try to replace any characters if it is the penalty card
            { 
                thisRow = thisRow.Replace("♦", suitChars[thisCard.suitValue]); 
            }

            if (thisCard.suitValue == 1 || thisCard.suitValue == 2)     //just hearts & diamonds are red
            {
                Console.BackgroundColor = ConsoleColor.White;

                for (int i = 0; i < thisRow.Length; i++)            //write character by character to maintain the black border on red cards
                {
                    if ((int)thisRow[i] >= 9472 && (int)thisRow[i] <= 9496)         //check if one of the chars is one of the box line chars
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    Console.Write(thisRow[i]);
                }
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                for (int i = 0; i < thisRow.Length; i++)            //write character by character to maintain the black border on red cards
                {
                    Console.Write(thisRow[i]);
                }
            }
            Console.ResetColor();
        }   //end method
                
        //public static void displayCard(Card thisCard)
        //{
        //    if (!visualsSetup)
        //    { setupVisuals(); }

        //    string str = cardVisuals[thisCard.cardValue].Replace("♦", suitChars[thisCard.suitValue]);

            
        //        if (thisCard.suitValue == 1 || thisCard.suitValue == 2)     //just hearts & diamonds are red
        //        {
        //            Console.BackgroundColor = ConsoleColor.White;

        //            for (int i = 0; i < str.Length; i++)            //write character by character to maintain the black border on red cards
        //            {
        //                if ((int)str[i] >= 9472 && (int)str[i] <= 9496)
        //                {
        //                    Console.ForegroundColor = ConsoleColor.Black;
        //                }
        //                else
        //                {
        //                    Console.ForegroundColor = ConsoleColor.Red;
        //                }
        //                Console.Write(str[i]);    
        //            }
        //        }
        //        else
        //        {
        //            Console.BackgroundColor = ConsoleColor.White;
        //            Console.ForegroundColor = ConsoleColor.Black;
        //            Console.WriteLine(str);         //look up in the dictionary
        //        }
        //        Console.ResetColor();

        //        Debug.WriteLine("width:" + Console.LargestWindowWidth);
        //}

  
       
        private static void setupVisuals()
        {
            cardVisuals = new Dictionary<int, string>();
            suitChars = new List<string> { "\u2663", "\u2666", "\u2665", "\u2660" };       //club, diamond, heart, spade (lowest value to highest value)
            
            string cardsFile = Properties.Resources.cardFaces;      // http://stackoverflow.com/questions/3314140/how-to-read-embedded-resource-text-file
            string buffer = "";
            int cardValue = 0; 
            string cardString = "";            

            //load the text file into a dictionary
            //move through the file one char at a time

            //find the first card header, then exit loop            
            int i;
            for (i = 0; i < cardsFile.Length; i++)
            {
                buffer += cardsFile[i];

                if (cardsFile[i] == '\n')   //end of line reached'
                {
                    if (buffer != "" && buffer.Length > "\r\n".Length)  //make sure line is not blank
                    {
                        cardValue = parseCardValue(buffer);
                        buffer = "";
                        break;                           
                    }
                }                
             }  //end for       

            //resume where last loop left off
            for (i++; i < cardsFile.Length; i++)    
            {
                buffer += cardsFile[i];

                if (cardsFile[i] == '\n')   //end of line reached'
                {
                    //Debug.WriteLine("Comparison worked");
                    
                    if (isCardHeader(buffer))  // the next card header was found, so add the card to the dictionary
                    {
                        //save existing card value & cardstring into dictionary
                        cardVisuals.Add(cardValue, cardString);                        
                        buffer = buffer.Substring(buffer.IndexOf("CARD"));
                        cardValue = parseCardValue(buffer);
                        
                        //reset strings
                        buffer = "";
                        cardString = "";
                    }
                    else
                    {
                        cardString += buffer;   //continue building out string representing the card
                        buffer = "";
                    }                                                               
                }

            } //end for

            //file ends before another header is found, so save last item in the text file to dictionary
            cardString += buffer;
            cardVisuals.Add(cardValue, cardString);
 
     
        }   //end setupVisuals Method


        private static bool isCardHeader(string str)
        {
            return (str.IndexOf(":") != -1);
        }

        private static int parseCardValue(string str)
        {
            int rangeStart = "CARD_".Length;
            int rangeEnd = str.IndexOf(":");
            return Convert.ToInt32(str.Substring(rangeStart, rangeEnd - rangeStart));            
        }

        
    }   //end class

    //Class to capture some of the game graphics routines
    class GameGraphics
    {
        public static int screenWidth = 80;
        public static int cardWidth = 9;
        public static int cardHeight = 12;

        public static string repeatString(string str, int times)
        {
            string temp = "";

            for (int i = 0; i < times; i++)
                temp += str;

            return temp;
        }

        public static int paddingToCenterString(int stringLength, int totalWidth)
        {
            return (int)(totalWidth / 2) - (stringLength / 2);
        }

        public static void splashScreen()
        {
            int width = 69;
            int height = 20;
            int screenWidth = GameGraphics.screenWidth;
            int numObjects = 1;

            string screenText = Properties.Resources.splashScreen;
            int spacesWidth = (int)(screenWidth - (numObjects * width)) / 2;

            Console.Clear();    //first clear the screen
            

            for (int row = 0; row < height; row++)
            {
                int startPoint = row * (width + Environment.NewLine.Length);
                string thisRow = screenText.Substring(startPoint, width);

                Console.Write(GameGraphics.repeatString(" ", spacesWidth));  //print padding


                switch (row)
                {
                    case 17:
                    case 18:
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;

                    default:
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                }


                for (int j = 0; j < width; j++)
                {
                    Console.Write(thisRow[j]);
                }
                Console.WriteLine();
                Console.ResetColor();
            } //end loop

            string pressKey = "[Press Enter to Continue]";

            Console.WriteLine();
            Console.WriteLine();
            Console.Write(GameGraphics.repeatString(" ", GameGraphics.paddingToCenterString(pressKey.Length, screenWidth)));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(pressKey);
            Console.ResetColor();
            Console.ReadLine();

        } //end splash Screen

        public static int getNumPlayers()
        {
            string title = "Choose the Quantity of Players";
            string title2 = "Between 2 and 4 Players Can Join";
            string question = "How many players? (default is 2) ->";
            int numPlayers = 2;
            ConsoleColor yellow = ConsoleColor.Yellow;
            ConsoleColor white = ConsoleColor.White;
            ConsoleColor blue = ConsoleColor.Blue;

            Console.Clear();
            Console.ResetColor();

            Console.WriteLine();
            Console.WriteLine();
            
            Console.Write(repeatString(" ", paddingToCenterString(title.Length, screenWidth)));            
            WriteLineWithColor(title, yellow, blue);            
            Console.Write(repeatString(" ", paddingToCenterString(title2.Length, screenWidth)));
            WriteLineWithColor(title, yellow, blue);            
            
            Console.WriteLine();
            
            Console.Write(question);
            Console.ForegroundColor = ConsoleColor.White;
            string input = Console.ReadLine();

            Regex rgx = new Regex(@"^[2-4]$");
            if (rgx.IsMatch(input))
            {
                Console.WriteLine("Good");
                Console.ReadLine();
                numPlayers = int.Parse(input);
            }
            else
            {
                Console.WriteLine("Problem");
                Console.ReadLine();
            }

            return numPlayers;
        }

        public static void WriteLineWithColor(string str, ConsoleColor front, ConsoleColor back)
        {
            //save the current state
            ConsoleColor oldFront = Console.ForegroundColor;    
            ConsoleColor oldBack = Console.BackgroundColor;

            //write with new color
            Console.ForegroundColor = front;
            Console.BackgroundColor = back;
            Console.WriteLine(str);

            //restore state
            Console.ForegroundColor = oldFront;
            Console.BackgroundColor = oldBack;
        }
        
    } //end GameGraphics class





}
