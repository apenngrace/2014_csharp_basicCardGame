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
            Game thisGame = new Game();
            thisGame.startGame();              

        }
    }

    class Round
    {
        public List<Player> players { get; set; }       //store players 
        public int roundId { get; set; }               //1 based round number to display on screen
        public Game thisGame { get; set; }          //hold onto a reference of Game
        
        private int turnNumber = 0;                    //0 base turn number
        private List<Card> cards;

        public void playRound()
        {
            
            Console.Clear();
            Console.SetCursorPosition(0, 0);

            string leftPadding;
            string roundString = Properties.Resources.roundNumber;
                        
            GraphicString g = new GraphicString(Properties.Resources.roundNumber);
            for (int i = 0; i < g.height; i++)
            {
                string thisRowString = g.getRow(i);     //get one row at a time of the string to be displayed
                
                if (thisRowString.IndexOf("Round") != -1)
                    thisRowString = thisRowString.Replace("#", roundId.ToString());

                Console.Write(GameGraphics.paddingToCenterString(thisRowString));
                GameGraphics.WriteLineWithColor(thisRowString, ConsoleColor.Yellow, ConsoleColor.DarkGreen);
            }
            
//            roundString = GameGraphics.centeredString(roundString);
//            GameGraphics.WriteLineWithColor("blah" + roundString, ConsoleColor.Yellow, ConsoleColor.Black);
 //           Console.Write(GameGraphics.repeatString("\r\n", 5));       //20 line feeds
            
            //each player gets their turn
            for (int i = 0; i < players.Count; i++)
            {
                string chooseCard = " choose a card.";
                leftPadding = GameGraphics.paddingToCenterString((players[i].playerName + chooseCard));
                Console.Write(leftPadding);
                GameGraphics.WriteWithColor(players[i].playerName, ConsoleColor.Cyan, ConsoleColor.Black);
                GameGraphics.WriteLineWithColor(chooseCard, ConsoleColor.Yellow, ConsoleColor.Black);
                
                GameGraphics.PressEnterToContinue();

                players[i].card = thisGame.thisDeck.nextCard();         //get next card from the deck
                Console.SetCursorPosition(10, 4);
                Console.WriteLine("blah blah blah");
                Console.WriteLine("blah blah blah");
                Console.WriteLine("blah blah blah");
                Console.ReadLine();

           }
            //display Round & Header
            //display which player's turn it is
            //display question to hit enter to continue (to reveal the cards)

        } //end class
    }

    class Player
    {
        public string id { get; set; }
        public string playerName { get; set; }
        public long playerScore { get; set; }
        public Card card { get; set; }

        public Player(int number)
        {
            id = number.ToString();
            playerName = "Player" + id;
        }
    }

    class Game
    {
        private int roundCount = 0;     
        private int numPlayers = 2;     //default number of players, can be between 2 and 4
        private List<Player> players;   //array of players
        
        //private List<Round> rounds;     //array of rounds
        private bool isGameOver = false;

        public Deck thisDeck { get; set; }

        public void startGame()
        {
            GameGraphics.splashScreen();
            
            //game loop, so that additional games can be started after the first ends
            do
            {
                    numPlayers = GameGraphics.getNumPlayers();    //get the number of players for the game
                    players = new List<Player>();
                    
                    for (int i = 0; i < numPlayers; i++)
                        players.Add(new Player(i+1));

                    //create deck, shuffle deck
                    thisDeck = new Deck();
                    thisDeck.shuffle(5);        //shuffle deck 5 times
                    Round thisRound = new Round();
                    //rounds.Add(thisRound);
                    thisRound.roundId = roundCount + 1;
                    thisRound.players = players;
                    thisRound.thisGame = this;  //pass reference of game to the round
                    
                    thisRound.playRound();
                        

                //ask players to get a card, then display the card on screen
                //check results of the round, if a player wins, then display a win result
                //display the results on screen
                //ask to begin the next round               
                //if the game ended, then ask if the user wants to start another game

            } while (!isGameOver); //end game loop
            
        }
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
 
        public void shuffle(int times)
        {
            for (int i = 0; i < times; i++)
                this.shuffle();
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
            } //end for loop

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

    class CardGraphics
    {
        private static Dictionary<int, GraphicString> cardVisuals;
        private static List<string> suitChars;
        private int cardWidth;
        
        public CardGraphics()   //constructor
        {
            loadCardsFromFile();
        }

        private void loadCardsFromFile()
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
                    cardBuffer += thisRowString;
                    if (isBottomOfCard(thisRowString))
                    {
                        cardVisuals.Add(cardValue, new GraphicString(cardBuffer));
                    }                        
                }
            }
        }   //end Method

            private bool isCardHeader(string str)
            {
                return (str.IndexOf("CARD") != -1);
            }

            private int parseCardValue(string str)
            {
                int rangeStart = "CARD_".Length;
                int rangeEnd = str.IndexOf(":");
                return Convert.ToInt32(str.Substring(rangeStart, rangeEnd - rangeStart));
            }

            private bool isBottomOfCard(string str)
            {
                return (str.IndexOf("└") != -1);
            }

        //=========================================================
        // Visual Display of Cards
        //=========================================================

        public void displayCardList(List<Card> cardList)
        {
            //int screenWidth = Console.LargestWindowWidth;                        
            int numSpaces = cardList.Count + 1;
            
            //define amount of space between cards on screen
            int cardPaddingSpace = (int)(GameGraphics.screenWidth - (cardList.Count * cardWidth)) / numSpaces;   
            
            //get a card to get card height.
            Card thisCard = cardList[0];
            GraphicString thisCardGraphics = cardVisuals[thisCard.cardValue];
            int height = thisCardGraphics.height;

            // print between 2 to 4 cards horizontally.  Print the cards, one line at a time.
            for (int i = 0; i < height; i++)    //the line number to be put on screen
            {
                for (int j = 0; j < cardList.Count; j++)    //the card to be printed in console
                {
                    thisCard = cardList[i];
                    thisCardGraphics = cardVisuals[thisCard.cardValue];
                    GameGraphics.WritePadding(cardPaddingSpace);
                    printCardRow(thisCard, i);
                }
            }
        }       //end method

        //print char by char with colors
        private static void printCardRow(Card thisCard, int row)
        {
            GraphicString g = cardVisuals[thisCard.cardValue];
            string thisRowString = g.getRow(row);

            bool isPenaltyCard = (thisCard.suitValue == -1);
            
            if (!isPenaltyCard)   //don't try to replace any characters if it is the penalty card
                thisRowString = thisRowString.Replace("♦", suitChars[thisCard.suitValue]);

            bool isHeartsOrDiamonds = (thisCard.suitValue == 1 || thisCard.suitValue == 2);

            for (int i = 0; i < thisRowString.Length; i++)            //write character by character to maintain the black border on red cards
            {
                bool isLineChar = ((int)thisRowString[i] >= 9472 && (int)thisRowString[i] <= 9496);     //range for ascii chars for drawing boxes

                if (isHeartsOrDiamonds && !isLineChar)  
                    GameGraphics.WriteWithColor(thisRowString[i].ToString(), ConsoleColor.Red, ConsoleColor.White);
                else
                    GameGraphics.WriteWithColor(thisRowString[i].ToString(), ConsoleColor.Black, ConsoleColor.White);
            }   //end for 
        }   //end method

        //=========================================================

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

        public static string paddingToCenterString(string str)
        {
            int numSpaces = (int)(screenWidth / 2) - (str.Length / 2);
            return repeatString(" ", numSpaces);
            
            //return (int)(totalWidth / 2) - (stringLength / 2);
        }

        public static string centeredString(string str)
        {
            return paddingToCenterString(str) + str;
        }

        
        //show a splash screen
        public static void splashScreen()
        {
            GraphicString g = new GraphicString(Properties.Resources.splashScreen);
            Console.Clear();    //first clear the screen
            for (int i = 0; i < g.height; i++)
            {
                string thisRowString = g.getRow(i);     //get one row at a time of the string to be displayed
                Console.Write(GameGraphics.paddingToCenterString(thisRowString));
                
                //Console.WriteLine(g.getRow(i));
                switch (i)
                {
                    case 17:
                    case 18:
                        GameGraphics.WriteLineWithColor(thisRowString, ConsoleColor.Gray, ConsoleColor.Blue);                        
                        break;

                    default:
                        GameGraphics.WriteLineWithColor(thisRowString, ConsoleColor.Yellow, ConsoleColor.Blue);
                        break;
                }
            }            
            PressEnterToContinue();     //get the prompt to hit enter
        } //end splash Screen

        public static int getNumPlayers()
        {
            bool correctlyAnswered = false;
            int numPlayers = 2;
            int timesAsked = 0;  //how many times the player has been asked to enter how many players.

            do
            {
                GraphicString g = new GraphicString(Properties.Resources.howManyPlayers);
                Console.Clear();    //first clear the screen
                
                //display the header at top of screen
                for (int i = 0; i < g.height; i++)
                {
                    string thisRowString = g.getRow(i);     //get one row at a time of the string to be displayed
                    Console.Write(GameGraphics.paddingToCenterString(thisRowString));
                    GameGraphics.WriteLineWithColor(thisRowString, ConsoleColor.Yellow,ConsoleColor.DarkGreen);                    
                } //end loop

                //display area below where user answers questions
                Console.WriteLine();
                if (timesAsked == 0)
                {
                    Console.WriteLine();
                }
                else
                {
                    //WritePadding(20);
                    string sorry = "Sorry, try again. Please enter either: 2, 3 or 4.";
                    Console.Write(paddingToCenterString(sorry));
                    WriteLineWithColor(sorry, ConsoleColor.Red, ConsoleColor.Black);
                }
                
                WritePadding(20);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("->");
                Console.ForegroundColor = ConsoleColor.White;
                string input = Console.ReadLine();

                Regex rgx = new Regex(@"^[2-4]$");
                if (rgx.IsMatch(input))
                {
                    numPlayers = int.Parse(input);
                    correctlyAnswered = true;

                    Console.WriteLine();
                    Console.WriteLine();

                    string confirmation = "You chose to have " + numPlayers + " players.";
                    Console.Write(paddingToCenterString(confirmation));
                    GameGraphics.WriteLineWithColor(confirmation, ConsoleColor.White, ConsoleColor.Black);                    

                    PressEnterToContinue();                               
                }
                else
                {
                    timesAsked++;
                }

            } while (!correctlyAnswered);

            return numPlayers;
        }   //end method

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

        public static void WriteWithColor(string str, ConsoleColor front, ConsoleColor back)
        {
            //save the current state
            ConsoleColor oldFront = Console.ForegroundColor;
            ConsoleColor oldBack = Console.BackgroundColor;

            //write with new color
            Console.ForegroundColor = front;
            Console.BackgroundColor = back;
            Console.Write(str);

            //restore state
            Console.ForegroundColor = oldFront;
            Console.BackgroundColor = oldBack;
        }

        public static void WritePadding(int width)
        {
            Console.Write(repeatString(" ", width));
        }

        public static void PressEnterToContinue()
        {
            string pressKey = "[Press Enter to Continue]";

            Console.WriteLine();
            Console.WriteLine();
            Console.Write(paddingToCenterString(pressKey));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(pressKey);
            Console.ResetColor();
            Console.ReadLine();
        }
        
    } //end GameGraphics class

    //Encapsulate the details around ASCII art in an object
    class GraphicString
    {
        public int width { get; private set; }
        public int height { get; private set; }
        public string graphicString {get; private set;}
        public List<string> graphicArray {get; private set;}

        public GraphicString(string graphicString)
        {
            graphicArray = new List<string>();  //instantiate property
            width = detectWidth(graphicString);
            height = detectHeight(graphicString);
            graphicArray = createArray(graphicString);
        }

        private int detectWidth(string str)
        {
            string buffer = "";
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '\r')
                    break;

                buffer += str[i];
            }
            return buffer.Length;
        }

        private int detectHeight(string str)
        {
            return (int) str.Length / width;
        }

        //create array with single string per row of graphic
        private List<string> createArray(string str)
        {
            List<string> tempArray = new List<string>();
            int startPos;

            startPos = 0;
            for (int i = 0; i < height; i++)    //rows
            {
                tempArray.Add(str.Substring(startPos, width));
                startPos += width + Environment.NewLine.Length;                
            }

            return tempArray;
        }

        public string getRow(int number)
        {
            return graphicArray[number];
        }
    } //end class



}
