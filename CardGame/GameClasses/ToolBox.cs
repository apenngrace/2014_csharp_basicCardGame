using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame.GameClasses
{
    class ToolBox       //misc utility methods to share
    {
        public static void AddOrUpdateDictionaryEntry(Dictionary<string, string> dict, string key, string value)
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
    }
}
