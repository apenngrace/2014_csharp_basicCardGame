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
            bool continuePlaying = false;

            do
            {
                Game thisGame = new Game();
                thisGame.startGame();
                continuePlaying = GameGraphics.promptPlayerContinuePlaying();
            } while (continuePlaying);
        }
    }

    class Round
    {
        public int roundId { get; set; }               //1 based round number to display on screen
        public Game game { get; set; }          //hold onto a reference of Game
        Dictionary<string, string> messages;
        
        public Round(Game g)
        {
            game = g;
            roundId = game.roundCount;
            messages = new Dictionary<string, string>();
        }

        public void playRound()
        {         
            GameGraphics.displayRoundHeader(roundId);
            int cursorTopBelowHeader = Console.CursorTop;
            int cardHeight = 12;
            int cursorBelowCards = cursorTopBelowHeader + cardHeight;
             
            //each player gets their turn to choose a card
            for (int i = 0; i < game.players.Count; i++)
            {
                GameGraphics.promptPlayerPickCard(this, i, cursorBelowCards);
                game.players[i].card = game.deck.nextCard();         //get next card from the deck
                CardGraphics.displayCardList(game.players, cursorTopBelowHeader);               
            }

            //determine who won round (if everyone gets a penalty, then no winner is declared)
            int winnerIndex = WhoWonRound();
            
            if (game.players[winnerIndex].card.cardValue != GameConstants.PENALTY_CARD)
            { game.players[winnerIndex].playerScore += GameConstants.POINTS_WON; }
                        
            applyPenaltyScore();    //add the penalty amount to the players' scores

            //update the scores below in bottom half of screen
            GameGraphics.showWinnerAndScores(this, winnerIndex, cursorBelowCards);

            if (isGameOver())
            {
                GameGraphics.showWinner(game.players[winnerIndex].playerName, game.players[winnerIndex].playerScore.ToString());
            }
            
            clearPlayerCards();
        } //method


        //create and return a list of players who got a penalty card in the round.
        public List<Player> getPenalizedPlayers()
        {
            //bool penaltyFound = false;
            //int howManyPenalties = 0;
            List<Player> temp = new List<Player>();

            foreach (var p in game.players)
            {
                if (p.card.cardValue == GameConstants.PENALTY_CARD)
                {
                    temp.Add(p);

                    //string penaltyMessage = p.playerName + " got a penalty card. (-1 Point)";
                    //AddOrUpdateDictionaryEntry(messages, lineDisplay.ToString(), penaltyMessage);
                    //lineDisplay++;   //advance to letter D (only 2 possible penalty cards in deck).
                }
            }

            if (temp.Count == 0)
                return null;
            else
                return temp;
        }

        public bool isGameOver()
        {
            List<int> scores = new List<int>();
            
            foreach (Player player in game.players)
                scores.Add(player.playerScore);
            
            scores.Sort((a, b) => -1 * a.CompareTo(b));     //sort scores descending
            
            if (scores[0] >= GameConstants.MIN_WINNING_SCORE)     //check highest score to see if there is a win.
            {
                return scores[0] > (scores[1] + GameConstants.WINNING_SPREAD);
            }

            return false;
        }

        private void applyPenaltyScore()
        {
            foreach (Player player in game.players)
            {
                if (player.card.cardValue == -1)
                {
                    if (player.playerScore - 1 < 0)
                        player.playerScore = 0;
                    else
                        player.playerScore--;
                }
            }
        }

        // delete the cards from the players so that they do not leak into next round
        private void clearPlayerCards()
        {
            foreach (Player player in game.players)
            {
                player.card = null;
            }
        }

        private int WhoWonRound()
        {
            int highValue = 0;
            int winnerIndex = 0;  
            
            for (int i = 0; i < game.players.Count; i++)
            {
                int currentValue = game.players[i].card.cardValue;
                if (highValue < currentValue)
                {
                    winnerIndex = i;
                    highValue = currentValue;
                }
                else if (highValue == currentValue)         //compare suit values if winning face values are the same.
                {
                    Card winningCard = game.players[winnerIndex].card;
                    Card currentCard = game.players[i].card;
                    
                    if (winningCard.suitValue < currentCard.suitValue)
                    {
                        winnerIndex = i;
                    }                    
                }
            } //end for 

            return winnerIndex;
        }
        
        //private void clearMessages()
        //{
        //    messages = new Dictionary<string, string>();
        //}

    } //end class

    class Player
    {
        public string id { get; set; }
        public string playerName { get; set; }
        public int playerScore { get; set; }
        public Card card { get; set; }

        public Player(int number)
        {
            id = number.ToString();
            playerName = "Player" + id;
        }
    }

    class Game
    {             
        public Deck deck { get; set; }
        public List<Player> players;
        public int roundCount { get; set; }

        public void startGame()
        {
            GameGraphics.splashScreen();
            roundCount = 1;
            bool isGameOver;
            
            //game loop, so that additional games can be started after the first ends
            int numPlayers = GameGraphics.getNumPlayers();    //get the number of players for the game
            players = new List<Player>();

            for (int i = 0; i < numPlayers; i++)
                players.Add(new Player(i + 1));

            //create deck, shuffle deck
            deck = new Deck();
            deck.shuffle(5);        //shuffle deck 5 times

            do
            {
                if (deck.cardsLeftInDeck() < players.Count)
                {
                    GameGraphics.needToShuffleDeck();
                    deck = new Deck();
                    deck.shuffle(5);        //shuffle deck 5 times
                }
                
                Round round = new Round(this);          //create a Round object to represent round of play, pass ref to game obj
                round.playRound();                      //start playing the round
                roundCount++;
                isGameOver = round.isGameOver();

            } while (!isGameOver); //end game loop
           
        }
    } //end Game class


    class Deck
    {
        static int lowestCard = 2; 
        static int highestCard = 14;
        static int numSuits = 4;
        static int currentCardIndex = 0;        //use a pointer to position top of deck.
        static int PENALTY_QUANTITY = 4;
        
        List<Card> cardDeck = new List<Card>();

        public Deck()
        {
            currentCardIndex = 0;
            // the deck has 13 cards per suit, 4 suits each (52 cards)
            // plus there are 2 special penalty cards

            for (int j = 0; j < numSuits; j++)
                for (int i = lowestCard; i <= highestCard; i++)
                    cardDeck.Add(new Card(i,j));               
            
            //add the requisite penalty cards to deck
            for (int i = 0; i < PENALTY_QUANTITY; i++)
                cardDeck.Add(new Card(-1, -1));            

        }   //end constructor      

        public Card nextCard()      //grab cards from current top of deck, and if deck ends then start from beginning again.
        {
            Card thisCard;
            thisCard = cardDeck[currentCardIndex];

            if (currentCardIndex + 1 > cardDeck.Count)
                currentCardIndex = 0;
            else
                currentCardIndex++; 
                        
            return thisCard;
        }

        public int cardsLeftInDeck()
        {
            return cardDeck.Count - (currentCardIndex + 1);
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

            //Get a random number to position card in deck.  If the number
            //is already taken, then just keep checking if adjacent indices are taken until an empty one if found.
            for (int i = 0; i < numCards; i++)
            {
                int someNumber = randomGenerator.Next(0, highestIndex);
                if (tempDeck[someNumber] == null)
                {
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

    //Assuming that only valid values for cards will be added here
    //
    class Card
    {
        public int cardValue { get; private set; }
        public int suitValue { get; private set; }
        private int MIN_CARD = 2, MAX_CARD = 14;
        private int PENALTY_CARD = -1;
        private int MIN_SUIT = 0, MAX_SUIT = 3;
        
        public Card(int card, int suit)
        {
            if (validRangeCard(card) && validRangeSuit(suit))
            {
                this.cardValue = card;
                this.suitValue = suit;
            }
        }

        private bool validRangeCard (int card)
        {
            return (card >= MIN_CARD && card <= MAX_CARD) || card == PENALTY_CARD;
        }

        private bool validRangeSuit(int suit)
        {
            return (MIN_SUIT >= 0 && suit <= MAX_SUIT);
        }
    }   //end Card class

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
            if (!cardsLoaded)
                loadCardsFromFile();
            //------------------------'
   
            int numSpaces = players.Count + 1;
            
            //define amount of space between cards on screen
            int cardPaddingSpace = (int)(GameGraphics.screenWidth - (players.Count * cardWidth)) / numSpaces;   
            
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
            bool isPenaltyCard = (suit == -1);
            
            if (!isPenaltyCard)   //don't try to replace any characters if it is the penalty card
                thisRowString = thisRowString.Replace("♦", suitChars[suit]);

            bool isHeartsOrDiamonds = (suit == 1 || suit == 2);

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
        public static int screenHeight = 25;
        public static int cardWidth = 9;
        public static int cardHeight = 12;
        public static char BOTTOM_DISPLAY_MSG_1 = 'A';
        public static char BOTTOM_DISPLAY_MSG_2 = 'B';
        public static char BOTTOM_DISPLAY_MSG_3 = 'C';
        public static char BOTTOM_DISPLAY_MSG_4 = 'D';
        public static char BOTTOM_DISPLAY_MSG_5 = 'E';
        public static char BOTTOM_DISPLAY_MSG_6 = 'F';

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
                WriteWithColor("->", ConsoleColor.Yellow, ConsoleColor.Black);
                
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
        
        public static void needToShuffleDeck()
        {
            GraphicString g = new GraphicString(Properties.Resources.reshuffling);
            int horizontal = (int)(80/2) - (int)(g.width / 2);
            int vertical = (int)(25/2) - (int)(g.height / 2);
            
            for (int i = 0; i < g.height; i++)
            {
                Console.SetCursorPosition(horizontal, vertical + i);
                WriteWithColor(g.getRow(i), ConsoleColor.Black, ConsoleColor.Yellow);
            }
            Console.CursorVisible = false;
            Console.ReadLine();            
        }

        //Render the bottom half of the screen with data.
        public static void showBottomHalf(List<Player> players, Dictionary<string, string> messages, int cursorTop)
        {
            GraphicString g = new GraphicString(Properties.Resources.bottomHalf);
            int playerDataPlaced = 0;
                        
            for (int i = 0; i < g.height; i++)
            {
                Console.SetCursorPosition(0, cursorTop + i);
                string thisLine = g.getRow(i);

                //Populate Scores
                int playerStringPosition = thisLine.IndexOf("Player");
                if (playerStringPosition != -1)    //a row that has scores
                {
                    if (playerDataPlaced < players.Count)   // still more scores to add
                    {
                        int scorePosition = playerStringPosition + "Player# : ".Length;
                        string score = players[playerDataPlaced].playerScore.ToString();
                        int targetLength = "###".Length;
                        if (score.Length < targetLength)
                        { score = repeatString(" ", targetLength - score.Length) + score; }

                        thisLine = thisLine.Remove(scorePosition, targetLength).Insert(scorePosition, score);
                        playerDataPlaced++;
                    }
                    else
                    {
                        int targetLength = "Player# : ###".Length;
                        thisLine = thisLine.Remove(playerStringPosition, targetLength).Insert(playerStringPosition, repeatString(" ",targetLength));
                    }                    
                }

                //Find line where messages to user are intended to go
                int messagePosition = thisLine.IndexOf("$");
                if (messagePosition != -1)
                {
                    bool blankMessage = true;
                    
                    if (messages != null)
                    {
                        string messageLetter = thisLine.Substring(messagePosition + 1, 1);  //get the letter
                        if (messages.ContainsKey(messageLetter))
                        {
                            string newMessage = messages[messageLetter];    //look it up in the incoming dictionary
                            int targetLength = newMessage.Length;

                            thisLine = thisLine.Remove(messagePosition, targetLength).Insert(messagePosition, newMessage);
                            blankMessage = false;
                        }
                    }
                                        
                    if (blankMessage)
                    {
                        int targetLength = "$#".Length;
                        thisLine = thisLine.Remove(messagePosition, targetLength).Insert(messagePosition, repeatString(" ", targetLength));
                    }
                }

                //Populate Message Area 
                                
                WritePadding(paddingToCenterString(thisLine).Length);              
                WriteLineWithColor(thisLine, ConsoleColor.White, ConsoleColor.DarkBlue);
            }
        }

        public static void showWinner(string name, string score)
        {
            GraphicString g = new GraphicString(Properties.Resources.winner);
            int horizontal = (int)(80 / 2) - (int)(g.width / 2);
            int vertical = (int)(25 / 2) - (int)(g.height / 2);

            int targetLength;
            for (int i = 0; i < g.height; i++)
            {
                string thisLine = g.getRow(i);
                
                int namePosition = thisLine.IndexOf("Player");
                if (namePosition != -1)
                {
                    targetLength = name.Length;
                    thisLine = thisLine.Remove(namePosition, targetLength).Insert(namePosition, name);
                }

                int scorePosition = thisLine.IndexOf("Score");
                if (scorePosition != -1)
                {
                    targetLength = "###".Length;
                    if (score.Length < targetLength)
                    { score = repeatString(" ", targetLength - score.Length) + score; }

                    scorePosition += "Score ".Length + 1;                   
                    thisLine = thisLine.Remove(scorePosition, targetLength).Insert(scorePosition, score);
                }
                
                Console.SetCursorPosition(horizontal, vertical + i);
                WriteWithColor(thisLine, ConsoleColor.White, ConsoleColor.DarkGreen);
            }
            Console.CursorVisible = false;
            Console.ReadLine();            
        }

        public static void displayRoundHeader(int roundId)
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);

            string roundString = Properties.Resources.roundNumber;

            GraphicString g = new GraphicString(Properties.Resources.roundNumber);
            for (int i = 0; i < g.height; i++)
            {
                string thisRowString = g.getRow(i);     //get one row at a time of the string to be displayed

                int roundPosition = thisRowString.IndexOf("Round");

                if (roundPosition != -1)
                {
                    int NumberPosition = roundPosition + ("Round ".Length);
                    //adding the round number on screen without messing up the box line drawing
                    thisRowString = thisRowString.Remove(NumberPosition, roundId.ToString().Length).Insert(NumberPosition, roundId.ToString());
                }
                
                Console.Write(GameGraphics.paddingToCenterString(thisRowString));
                GameGraphics.WriteLineWithColor(thisRowString, ConsoleColor.Yellow, ConsoleColor.DarkGreen);
            }
        } //end method


        public static void showWinnerAndScores(Round round, int winnerIndex, int cursorBelowCards)
        {
            Dictionary<string, string> messages = new Dictionary<string, string>();
            char lineDisplay;

            //if multiple penalties are given, then no winner for round.
            if (round.game.players[winnerIndex].card.cardValue != GameConstants.PENALTY_CARD)
            {
                string roundWinnerString = round.game.players[winnerIndex].playerName + " won this round. (+2 Points)";
                lineDisplay = GameGraphics.BOTTOM_DISPLAY_MSG_2;
                //letter = 'B';
                ToolBox.AddOrUpdateDictionaryEntry(messages, lineDisplay.ToString(), roundWinnerString);
            }

            //lineDisplay = 'C';            
            List<Player> penalties = round.getPenalizedPlayers();
            if (penalties != null)
            {
                string penaltyString;

                if (penalties.Count == 4)   //everyone got a penalty card
                    lineDisplay = GameGraphics.BOTTOM_DISPLAY_MSG_2;
                else
                    lineDisplay = GameGraphics.BOTTOM_DISPLAY_MSG_3;

                for (int i = 0; i < penalties.Count; i++)
                {
                    penaltyString = penalties[i].playerName + " got a penalty. (-1 Point)";
                    ToolBox.AddOrUpdateDictionaryEntry(messages, lineDisplay.ToString(), penaltyString);
                    lineDisplay++;
                }
            }


            if (!round.isGameOver())
            {
                lineDisplay = GameGraphics.BOTTOM_DISPLAY_MSG_6;
                ToolBox.AddOrUpdateDictionaryEntry(messages, lineDisplay.ToString(), "[Press Enter to Continue]");
            }

            GameGraphics.showBottomHalf(round.game.players, messages, cursorBelowCards);

            if (!round.isGameOver())          //do not pause if the game is over, to allow winning screen to pop up.
            { Console.ReadLine(); }

        }   //end method

        public static void promptPlayerPickCard(Round round, int i, int cursorTop)
        {
            Dictionary<string, string> messages = new Dictionary<string, string>();
            
            Console.SetCursorPosition(0, cursorTop);

            string chooseCard = " choose a card.";
            ToolBox.AddOrUpdateDictionaryEntry(messages, "A", round.game.players[i].playerName + chooseCard);
            ToolBox.AddOrUpdateDictionaryEntry(messages, "F", "[Press Enter to Continue]");
            GameGraphics.showBottomHalf(round.game.players, messages, cursorTop);
            Console.ReadLine();

            //GameGraphics.PressEnterToContinue();
        }

        public static bool promptPlayerContinuePlaying()
        {
            bool answer = false;
            bool correctlyAnswered = false;
            int timesAsked = 0;

            do
            {
                Console.Clear();    //first clear the screen
                GraphicString g = new GraphicString(Properties.Resources.playAgain);
                int horizontal = (int)(screenWidth / 2) - (int)(g.width / 2);
                int vertical = (int)(screenHeight / 2) - (int)(g.height / 2);

                //displaying question centered on screen
                for (int i = 0; i < g.height; i++)
                {
                    string thisLine = g.getRow(i);
                    Console.SetCursorPosition(horizontal, vertical + i);
                    WriteWithColor(thisLine, ConsoleColor.White, ConsoleColor.DarkGreen);
                }                
                
                //display area below where user answers questions
                Console.WriteLine();
                if (timesAsked == 0)
                {
                    Console.WriteLine();
                }
                else
                {
                    //WritePadding(20);
                    string sorry = "Sorry, try again. Please enter either: Yes or No.";
                    Console.Write(paddingToCenterString(sorry));
                    WriteLineWithColor(sorry, ConsoleColor.Red, ConsoleColor.Black);
                }

                WritePadding(20);
                WriteWithColor("->", ConsoleColor.Yellow, ConsoleColor.Black);

                Console.ForegroundColor = ConsoleColor.White;
                string input = Console.ReadLine();

                //test for either YES or Y, NO or N
                bool gotMatch;
                Regex rgx = new Regex(@"^(?:YES|NO)$");
                gotMatch = rgx.IsMatch(input.ToUpper());

                if (!gotMatch)
                {
                    rgx = new Regex(@"^[Y|N]$");
                    gotMatch = rgx.IsMatch(input.ToUpper());
                }

                if (gotMatch)
                {
                    if (input.ToUpper().Substring(0, 1) == "Y")
                    { answer = true; }
                    else
                    { answer = false; }
                    
                    correctlyAnswered = true;

                    //Console.WriteLine();
                    //Console.WriteLine();

                    //string confirmation = "You chose to have " + numPlayers + " players.";
                    //Console.Write(paddingToCenterString(confirmation));
                    //GameGraphics.WriteLineWithColor(confirmation, ConsoleColor.White, ConsoleColor.Black);

                    //PressEnterToContinue();
                }
                else
                {
                    timesAsked++;
                }

            } while (!correctlyAnswered);


            return answer;

        } //end continue playing method

        
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
            return (int) Math.Ceiling(Convert.ToDecimal(str.Length) / (width + Environment.NewLine.Length));
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

    class GameConstants
    {
        public static int PENALTY_CARD = -1;
        public static int POINTS_WON = 2;
        public static int PENALTY_POINTS = -1;
        public static int MIN_WINNING_SCORE = 21;
        public static int WINNING_SPREAD = 2;       //the amount of points ahead you have to be against your opponent to win.
    }

    class ToolBox       //misc utility methods to share
    {
        public static void AddOrUpdateDictionaryEntry(Dictionary<string, string> dict, string key, string value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }
    }


}
