using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame.GameClasses
{
    class GameConstants
    {
        public static int MIN_CARD_VALUE = 2;
        public static int MAX_CARD_VALUE = 14;
        
        public static int NUM_SUITS = 4;
        public static int MIN_CARD_SUIT = 0;
        public static int MAX_CARD_SUIT = 3;
        
        public static int SUIT_CLUBS = 0;       //values defined by game specs that Clubs < Dimaonds < Hearts < Spades
        public static int SUIT_DIAMONDS = 1;
        public static int SUIT_HEARTS = 2;
        public static int SUIT_SPADES = 3;

        public static int MIN_NUM_PLAYERS = 2;      //this is not changable b/c of expected space for graphics on screen
        public static int MAX_NUM_PLAYERS = 4;      //this is not changable b/c of expected graphic space on screen
                
        public static int PENALTY_CARD = -1;        
        public static int PENALTY_QUANTITY = 4;

        public static int POINTS_WON = 2;           //expected to be positive
        public static int PENALTY_POINTS = -1;      //expected to be negative

        public static int MIN_WINNING_SCORE = 21;
        public static int WINNING_SPREAD = 2;       //the amount of points ahead you have to be against your opponent to win.
    }
}
