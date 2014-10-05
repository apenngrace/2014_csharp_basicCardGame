using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CardGame.GameClasses
{
    //Class to capture some of the game graphics routines
    class GameGraphics
    {
        //graphics related constants
        
        public static int SCREEN_WIDTH = Console.WindowWidth;
        public static int SCREEN_HEIGHT = Console.WindowHeight;

        public static int CARD_HEIGHT = 12;     //this can be less, but not more unless the console size is bigger than 80x25 chars
                
        //these message locations refer to the resource called BottomHalf.txt
        //these are used to place strings in predefined ascii art.
        public static char BOTTOM_DISPLAY_MSG_1 = 'A';
        public static char BOTTOM_DISPLAY_MSG_2 = 'B';
        public static char BOTTOM_DISPLAY_MSG_3 = 'C';
        public static char BOTTOM_DISPLAY_MSG_4 = 'D';
        public static char BOTTOM_DISPLAY_MSG_5 = 'E';
        public static char BOTTOM_DISPLAY_MSG_6 = 'F';

        //box art ascii codes
        public static int MIN_BOX_CODE = 9472;
        public static int MAX_BOX_CODE = 9496;
        
        //================================
        //User Display/Interaction Methods
        //================================
        
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
                    case 17:        //intentionally allowing fall-through
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
                    GameGraphics.WriteLineWithColor(thisRowString, ConsoleColor.Yellow, ConsoleColor.DarkGreen);
                } //end loop

                //display area below where user answers questions
                Console.WriteLine();
                if (timesAsked == 0)
                {
                    Console.WriteLine();
                }
                else
                {
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
            int horizontal = (int)(SCREEN_WIDTH / 2) - (int)(g.width / 2);          //center box in middle of screen
            int vertical = (int)(SCREEN_HEIGHT / 2) - (int)(g.height / 2);

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
                        thisLine = thisLine.Remove(playerStringPosition, targetLength).Insert(playerStringPosition, repeatString(" ", targetLength));
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
            int horizontal = (int)(SCREEN_WIDTH / 2) - (int)(g.width / 2);
            int vertical = (int)(SCREEN_HEIGHT / 2) - (int)(g.height / 2);

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
                string pointsWord = (GameConstants.POINTS_WON > 1) ? "Points." : "Point.";
                                
                string roundWinnerString = round.game.players[winnerIndex].playerName + " won this round. (+" 
                                            + GameConstants.POINTS_WON + " "+ pointsWord + ")";
                
                lineDisplay = GameGraphics.BOTTOM_DISPLAY_MSG_2;
                ToolBox.AddOrUpdateDictionaryEntry(messages, lineDisplay.ToString(), roundWinnerString);
            }

            //lineDisplay = 'C';            
            List<Player> penalties = round.getPenalizedPlayers();
            if (penalties != null)
            {
                string penaltyString;

                if (penalties.Count == GameConstants.MAX_NUM_PLAYERS)   //everyone got a penalty card
                    lineDisplay = GameGraphics.BOTTOM_DISPLAY_MSG_2;
                else
                    lineDisplay = GameGraphics.BOTTOM_DISPLAY_MSG_3;

                for (int i = 0; i < penalties.Count; i++)
                {
                    int pointsAmount = Math.Abs(GameConstants.PENALTY_POINTS);
                    string pointsWord = (pointsAmount > 1) ? "Points." : "Point.";

                    penaltyString = penalties[i].playerName + " got a penalty. (-"+ pointsAmount + " " + pointsWord + ")";
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
            ToolBox.AddOrUpdateDictionaryEntry(messages, GameGraphics.BOTTOM_DISPLAY_MSG_1.ToString(), round.game.players[i].playerName + chooseCard);
            ToolBox.AddOrUpdateDictionaryEntry(messages, GameGraphics.BOTTOM_DISPLAY_MSG_6.ToString(), "[Press Enter to Continue]");
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
                int horizontal = (int)(SCREEN_WIDTH / 2) - (int)(g.width / 2);
                int vertical = (int)(SCREEN_HEIGHT / 2) - (int)(g.height / 2);

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
                }
                else
                {
                    timesAsked++;
                }

            } while (!correctlyAnswered);

            return answer;
        } //end continue playing method

        //====================================
        //Utility Methods for Graphic Display
        //====================================

        public static string repeatString(string str, int times)
        {
            string temp = "";

            for (int i = 0; i < times; i++)
                temp += str;

            return temp;
        }

        public static string paddingToCenterString(string str)
        {
            int numSpaces = (int)(SCREEN_WIDTH / 2) - (str.Length / 2);
            return repeatString(" ", numSpaces);

            //return (int)(totalWidth / 2) - (stringLength / 2);
        }

        public static string centeredString(string str)
        {
            return paddingToCenterString(str) + str;
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


    } //end GameGraphics class
}
