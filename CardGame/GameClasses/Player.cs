using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame.GameClasses
{
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
}
