using System;
using System.Collections.Generic;

namespace Word_Find_Solver
{
    public class Trie
    {
        public bool end = false;
        public char thischar;
        public Dictionary<char, Trie> tn = new Dictionary<char, Trie>();

        public void AddWord(string w)
        {
            Trie t = this;
            foreach (char c2 in w)
            {
                var c = c2.ToString().ToUpper()[0];

                if (t.tn.ContainsKey(c) == false)
                    t.tn[c] = new Trie();

                t = t.tn[c];
            }
            t.end = true;
        }

        public bool ContainsWord(String w, bool partial)
        {
            Trie t = this;
            foreach (char c in w)
            {
                if (t.tn.ContainsKey(c) == false)
                    return false;

                t = t.tn[c];
            }
            if (partial)
                return true;
            return t.end;
        }
    }
}
