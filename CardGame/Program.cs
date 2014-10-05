using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using CardGame.GameClasses;


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
} 
