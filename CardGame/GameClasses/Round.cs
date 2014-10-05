using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame.GameClasses
{
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
            int cursorBelowCards = cursorTopBelowHeader + GameGraphics.CARD_HEIGHT;

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
            List<Player> temp = new List<Player>();

            foreach (var p in game.players)
            {
                if (p.card.cardValue == GameConstants.PENALTY_CARD)
                {
                    temp.Add(p);
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
                if (player.card.cardValue == GameConstants.PENALTY_CARD)
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

    } //end class
}
