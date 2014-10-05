using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame.GameClasses
{
    //Assuming that only valid values for cards will be added here
    //
    class Card
    {
        public int cardValue { get; private set; }
        public int suitValue { get; private set; }
        
        public Card(int card, int suit)
        {
            if (validRangeCard(card) && validRangeSuit(suit))
            {
                this.cardValue = card;
                this.suitValue = suit;
            }
        }

        private bool validRangeCard(int card)
        {
            return (card >= GameConstants.MIN_CARD_VALUE && card <= GameConstants.MAX_CARD_VALUE) || card == GameConstants.PENALTY_CARD;
        }

        private bool validRangeSuit(int suit)
        {
            return (GameConstants.MIN_CARD_SUIT >= 0 && suit <= GameConstants.MAX_CARD_SUIT);
        }
    }   //end Card class}
}