using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame.GameClasses
{
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
    } //end Game class}
}