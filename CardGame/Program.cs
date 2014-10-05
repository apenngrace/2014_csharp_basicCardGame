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
        public int roundId { get; set; }               //1 based round number to display on screen
        public Game game { get; set; }          //hold onto a reference of Game
        private int WINNER_POINTS = 2;
        Dictionary<string, string> messages;
        
        public Round(Game g)
        {
            game = g;
            roundId = game.roundCount;
            messages = new Dictionary<string, string>();
        }

        public void playRound()
        {
            displayHeader();
            int cursorTopBelowHeader = Console.CursorTop;
            int cardHeight = 12;
            int cursorBelowCards = cursorTopBelowHeader + cardHeight;

            //GameGraphics.showBottomHalf(game.players, null, cursorBelowCards);
            //Console.ReadLine();
             
            //each player gets their turn to choose a card
            for (int i = 0; i < game.players.Count; i++)
            {
                promptPlayerPickCard(i, cursorBelowCards);
                game.players[i].card = game.deck.nextCard();         //get next card from the deck
                CardGraphics.displayCardList(game.players, cursorTopBelowHeader);               
            }

           //Console.ReadLine();

            int winnerIndex = WhoWonRound();
            
            if (game.players[winnerIndex].card.cardValue != -1)
            { game.players[winnerIndex].playerScore += WINNER_POINTS; }
                        
            checkPenalty();

            showWinnerAndScores(winnerIndex, cursorBelowCards);

            if (isGameOver())
            {
                GameGraphics.winner(game.players[winnerIndex].playerName, game.players[winnerIndex].playerScore.ToString());
            }
            
            clearPlayerCards();
        } //method


        private void showWinnerAndScores(int winnerIndex, int cursorBelowCards)
        {
            clearMessages();

            char letter;
            //if multiple penalties are given, then no winner for round.
            if (game.players[winnerIndex].card.cardValue != -1)
            {
                string roundWinnerString = game.players[winnerIndex].playerName + " won this round. (+2 Points)";
                letter = 'B';
                AddOrUpdateDictionaryEntry(messages, letter.ToString(), roundWinnerString);
            }

            letter = 'C';            
            bool penaltyFound = false;
            foreach (var p in game.players)
            {              
                if (p.card.cardValue == -1)
                {
                    penaltyFound = true;
                    string penaltyMessage = p.playerName + " got a penalty card. (-1 Point)";
                    AddOrUpdateDictionaryEntry(messages, letter.ToString(), penaltyMessage);
                    letter++;   //advance to letter D (only 2 possible penalty cards in deck).
                }
            }

            if (!isGameOver())
            {
                letter = 'F';
                AddOrUpdateDictionaryEntry(messages, letter.ToString(), "[Press Enter to Continue]");
            }

            //letter = 'F';
            //AddOrUpdateDictionaryEntry(messages, letter.ToString(), "[Press Enter to Continue]");

            GameGraphics.showBottomHalf(game.players, messages, cursorBelowCards);

            if (!isGameOver())          //do not pause if the game is over, to allow winning screen to pop up.
            { Console.ReadLine(); }
            
        }   //end method

        public bool isGameOver()
        {
            int WINNING_SPREAD = 2;
            int MIN_WINNING_SCORE = 21;
            List<int> scores = new List<int>();
            
            foreach (Player player in game.players)
            {
                scores.Add(player.playerScore);
            }

            scores.Sort((a, b) => -1 * a.CompareTo(b));     //sort scores descending
            
            if (scores[0] >= MIN_WINNING_SCORE)     //check highest score to see if there is a win.
            {
                return scores[0] > (scores[1] + WINNING_SPREAD);
            }

            return false;
        }

        private void checkPenalty()
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
        
        private void displayHeader()
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
                    thisRowString = thisRowString.Remove(NumberPosition,roundId.ToString().Length).Insert(NumberPosition,roundId.ToString());
                }
                    

                Console.Write(GameGraphics.paddingToCenterString(thisRowString));
                GameGraphics.WriteLineWithColor(thisRowString, ConsoleColor.Yellow, ConsoleColor.DarkGreen);
            }
        } //end method
        
        private void promptPlayerPickCard(int i, int cursorTop)
        {
            Console.SetCursorPosition(0, cursorTop);
            
            
            //string leftPadding = GameGraphics.paddingToCenterString((game.players[i].playerName + chooseCard));
            //Console.Write(leftPadding);
            //GameGraphics.WriteWithColor(game.players[i].playerName, ConsoleColor.Cyan, ConsoleColor.Black);
            //GameGraphics.WriteLineWithColor(chooseCard, ConsoleColor.Yellow, ConsoleColor.Black);

            //messages.Add("A", game.players[i].playerName + chooseCard);
            //messages.Add("F", "[Press Enter to Continue]");

            string chooseCard = " choose a card.";
            AddOrUpdateDictionaryEntry(messages,"A", game.players[i].playerName + chooseCard);
            AddOrUpdateDictionaryEntry(messages, "F", "[Press Enter to Continue]");
            GameGraphics.showBottomHalf(game.players, messages, cursorTop);
            Console.ReadLine();
            
            //GameGraphics.PressEnterToContinue();
        }

        private void AddOrUpdateDictionaryEntry(Dictionary<string, string> dict, string key, string value)
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
        private void clearMessages()
        {
            messages = new Dictionary<string, string>();
        }

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
                
                Round round = new Round(this);
                round.playRound();

                roundCount++;

                //ask players to get a card, then display the card on screen
                //check results of the round, if a player wins, then display a win result
                //display the results on screen
                //ask to begin the next round               
                //if the game ended, then ask if the user wants to start another game
                isGameOver = round.isGameOver();

            } while (!isGameOver); //end game loop

            //Console.WriteLine("Somebody Won!");
            //Console.ReadLine();
            
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

        public static void winner(string name, string score)
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



}
