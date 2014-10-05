using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CardGame.GameClasses
{
    class Deck
    {
        static int currentCardIndex = 0;        //use a pointer to position top of deck.        
        List<Card> cardDeck = new List<Card>();

        public Deck()
        {
            currentCardIndex = 0;
            // the deck has 13 cards per suit, 4 suits each (52 cards)
            // plus there are special penalty cards

            for (int j = 0; j < GameConstants.NUM_SUITS; j++)
                for (int i = GameConstants.MIN_CARD_VALUE; i <= GameConstants.MAX_CARD_VALUE; i++)
                    cardDeck.Add(new Card(i, j));

            //add the requisite penalty cards to deck
            for (int i = 0; i < GameConstants.PENALTY_QUANTITY; i++)
                cardDeck.Add(new Card(GameConstants.PENALTY_CARD, GameConstants.PENALTY_CARD));

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
}
