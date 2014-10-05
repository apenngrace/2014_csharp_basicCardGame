using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame.GameClasses
{
    //Encapsulate the details around ASCII art in an object
    class GraphicString
    {
        public int width { get; private set; }
        public int height { get; private set; }
        public string graphicString { get; private set; }
        public List<string> graphicArray { get; private set; }

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
            return (int)Math.Ceiling(Convert.ToDecimal(str.Length) / (width + Environment.NewLine.Length));
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
