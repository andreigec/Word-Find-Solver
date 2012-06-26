using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ANDREICSLIB;

namespace Word_Find_Solver
{
    public static class Grid
    {
        public static Form1 baseform;
        private static int width;
        private static int height;
        private static char[][] grid;
        private static PanelUpdates PU;
        private static Color normalback = Color.CornflowerBlue;
        private static Color normalfront = Color.Yellow;
        private static Color findback = Color.Yellow;
        private static Color findfront = Color.CornflowerBlue;
        private static Trie trieroot;
        private static Dictionary<char, int> LetterScores = new Dictionary<char, int>();
        private static Random r = new Random();
        private static bool AvoidUpdate = false;
        /// <summary>
        /// direction to an x,y tuple for relative direction
        /// </summary>
        private static Dictionary<Direction, Tuple<int, int>> Directions;

        private static Direction GetDirection(TextBox basetb, TextBox comptb)
        {
            var basep = GetControlPos(basetb);
            var compp = GetControlPos(comptb);

            int xrel = compp.Item1 - basep.Item1;
            int yrel = compp.Item2 - basep.Item2;

            var t = new Tuple<int, int>(xrel, yrel);
            var d = Directions.Where(s => s.Value.Equals(t));

            if (d.Count() == 1)
                return d.First().Key;

            return Direction.NotSet;
        }

        private static char GenNewLetter(int max)
        {
            int current = r.Next() % max;
            foreach (var kvp in LetterScores)
            {
                if (current < kvp.Value)
                    return kvp.Key;
                current -= kvp.Value;
            }
            return 'x';
        }

        public static void RandomiseLetters(FindMode fm)
        {
            AvoidUpdate = true;
            //create stack
            int max = LetterScores.Sum(s => s.Value);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    grid[y][x] = GenNewLetter(max);
                    var tb = GetControl(x, y);
                    tb.Text = grid[y][x].ToString();
                    ChangeTextBoxValue(tb);
                }
            }
            AvoidUpdate = false;
        }

        public enum FindMode
        {
            NotSet, Crossword, AllDirFromLast
        }

        public enum Direction
        {
            NotSet, Down, DownLeft, Left, UpLeft, Up, UpRight, Right, DownRight
        }

        public static void InitPanel(PanelUpdates pu)
        {
            PU = pu;
        }

        public static void InitWords()
        {
            var raw = EmbeddedResources.ReadEmbeddedResource("dictionarywords.txt").Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            trieroot = new Trie();
            foreach (var s in raw)
            {
                trieroot.AddWord(s);
            }

            //add relative directions
            Directions = new Dictionary<Direction, Tuple<int, int>>();
            Directions.Add(Direction.NotSet, new Tuple<int, int>(0, 0));

            Directions.Add(Direction.DownLeft, new Tuple<int, int>(-1, +1));
            Directions.Add(Direction.Left, new Tuple<int, int>(-1, +0));
            Directions.Add(Direction.UpLeft, new Tuple<int, int>(-1, -1));
            Directions.Add(Direction.Up, new Tuple<int, int>(+0, -1));
            Directions.Add(Direction.UpRight, new Tuple<int, int>(+1, -1));
            Directions.Add(Direction.Right, new Tuple<int, int>(+1, +0));
            Directions.Add(Direction.DownRight, new Tuple<int, int>(+1, +1));
            Directions.Add(Direction.Down, new Tuple<int, int>(+0, +1));

            //letter scores
            LetterScores = new Dictionary<char, int>();
            LetterScores.Add('A', 1);
            LetterScores.Add('B', 3);
            LetterScores.Add('C', 3);
            LetterScores.Add('D', 2);
            LetterScores.Add('E', 1);
            LetterScores.Add('F', 4);
            LetterScores.Add('G', 2);
            LetterScores.Add('H', 4);
            LetterScores.Add('I', 1);
            LetterScores.Add('J', 8);
            LetterScores.Add('K', 5);
            LetterScores.Add('L', 1);
            LetterScores.Add('M', 3);
            LetterScores.Add('N', 1);
            LetterScores.Add('O', 1);
            LetterScores.Add('P', 3);
            LetterScores.Add('Q', 10);
            LetterScores.Add('R', 1);
            LetterScores.Add('S', 1);
            LetterScores.Add('T', 1);
            LetterScores.Add('U', 1);
            LetterScores.Add('V', 4);
            LetterScores.Add('W', 4);
            LetterScores.Add('X', 8);
            LetterScores.Add('Y', 4);
            LetterScores.Add('Z', 10);
        }

        public static int GetWordScore(String s)
        {
            int score = s.Sum(b => LetterScores[b]);
            return score;
        }

        /// <summary>
        /// initialise the grid
        /// </summary>
        /// <param name="widthI"></param>
        /// <param name="heightI"></param>
        /// <param name="rows"></param>
        public static void InitGrid(int widthI, int heightI, string[] rows = null)
        {
            grid = new char[heightI][];
            for (var y = 0; y < heightI; y++)
            {
                grid[y] = new char[widthI];
                if (rows != null)
                {
                    for (int x = 0; x < widthI; x++)
                    {
                        if (rows[y].Length >= x)
                            grid[y][x] = rows[y][x].ToString().ToUpper()[0];
                    }
                }
            }
            width = widthI;
            height = heightI;

            CreatePanel();
        }

        public static TextBox GetLastTextbox()
        {
            return GetControl(width - 1, height - 1);
        }

        public static void Clear()
        {
            InitGrid(width, height);
        }

        public static void InitGrid(string[] rows)
        {
            int heightI = rows.Count();
            int widthI = rows.Max(s => s.Length);
            InitGrid(widthI, heightI, rows);
        }

        public static void SaveGridToFile(String filename)
        {
            var fs = new FileStream(filename, FileMode.Create);
            var sw = new StreamWriter(fs);
            for (int y = 0; y < height; y++)
            {
                var s = new string(grid[y]);
                sw.WriteLine(s);
            }
            sw.Close();
            fs.Close();
        }

        /// <summary>
        /// from a starting letter/position, find all possible words from this
        /// </summary>
        /// <param name="lb"></param>
        /// <param name="word"></param>
        /// <param name="history"></param>
        /// <param name="lastdir"></param>
        /// <param name="fm"></param>
        /// <param name="minx"></param>
        /// <param name="maxx"></param>
        /// <param name="miny"></param>
        /// <param name="maxy"></param>
        private static List<string> Solve(String word, List<TextBox> history, Direction lastdir, FindMode fm, int minx, int maxx, int miny, int maxy)
        {
            var retwords = new List<string>();
            //if its a word, add
            if (trieroot.ContainsWord(word, false))
            {
                retwords.Add(word);
            }

            for (int y = miny; y <= maxy; y++)
            {
                if (OutOfBounds(y, false))
                    continue;

                for (int x = minx; x <= maxx; x++)
                {
                    if (OutOfBounds(x, true))
                        continue;

                    char gc = grid[y][x];
                    var thistb = GetControl(x, y);

                    if (history.Contains(thistb))
                        continue;

                    Direction newdir;
                    if (history.Count == 0)
                        newdir = Direction.NotSet;
                    else
                        newdir = GetDirection(history.Last(), thistb);

                    if (newdir != lastdir && lastdir != Direction.NotSet && fm == FindMode.Crossword)
                        continue;

                    String newword = word + gc;

                    if (trieroot.ContainsWord(newword, true))
                    {
                        var newhistory = new List<TextBox>();
                        newhistory.AddRange(history);
                        newhistory.Add(thistb);

                        int nminx = -1, nminy = -1, nmaxx = -1, nmaxy = -1;
                        SetSearchDimensions(x, y, fm, ref nminy, ref nmaxy, ref nminx, ref nmaxx, newdir);
                        retwords.AddRange(Solve(newword, newhistory, newdir, fm, nminx, nmaxx, nminy, nmaxy));
                    }
                }
            }
            return retwords;
        }

        /// <summary>
        /// find all words in a grid and output the words found to a listbox
        /// </summary>
        /// <param name="lb"></param>
        /// <param name="fm"></param>
        /// <param name="maxret"></param>
        public static List<string> Solve(FindMode fm, int maxret = 100)
        {
            var foundwords = new List<string>();

            int minx = -1, miny = -1, maxx = -1, maxy = -1;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var tb = GetControl(x, y);
                    SetSearchDimensions(x, y, fm, ref miny, ref maxy, ref minx, ref maxx);
                    foundwords.AddRange(Solve(grid[y][x].ToString(), new List<TextBox>() { tb }, Direction.NotSet, fm, minx, maxx, miny, maxy));
                }
            }

            foundwords = foundwords.Distinct().ToList();

            return foundwords;
        }

        private static void CreatePanel()
        {
            PU.clearControls();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var f = new Font(FontFamily.GenericSansSerif, 20);

                    var tb = new TextBox
                    {
                        Width = 30,
                        Height = 16,
                        BorderStyle = BorderStyle.None,
                        BackColor = normalback,
                        ForeColor = normalfront,
                        MaxLength = 1,
                        Text = grid[y][x].ToString(),
                        TextAlign = HorizontalAlignment.Center,
                        Font = f,
                        Name = GetControlName(x, y),
                    };

                    tb.TextChanged += baseform.GridTBChangeEvent;

                    PU.addControl(tb, x != (width - 1));
                }
            }
        }

        private static string GetControlName(int x, int y)
        {
            return x.ToString() + ":" + y.ToString();
        }

        public static void ChangeTextBoxValue(TextBox tb)
        {
            var pos = GetControlPos(tb);

            if (tb.Text.Length == 1)
                grid[pos.Item2][pos.Item1] = tb.Text[0];
            else
                grid[pos.Item2][pos.Item1] = ' ';
        }

        public static TextBox GetControl(int x, int y)
        {
            var name = GetControlName(x, y);
            var c = PU.controlStack.Where(s => s.Name.Equals(name));
            if (c.Count() == 1)
                return c.First() as TextBox;

            return null;
        }

        private static bool OutOfBounds(int a, bool isX)
        {
            bool change = false;
            if (a < 0)
            {
                change = true;
            }
            if (a >= width && isX)
            {
                change = true;
            }
            if (a >= height && isX == false)
            {
                change = true;
            }
            return change;
        }

        private static Tuple<int, int> GetControlPos(TextBox tb)
        {
            var p = tb.Name.IndexOf(':');

            int x = int.Parse(tb.Name.Substring(0, p));
            int y = int.Parse(tb.Name.Substring(p + 1));

            return new Tuple<int, int>(x, y);
        }

        private static List<TextBox> FindMatch(string word, int posc, FindMode fm, List<TextBox> lastaccum = null, Direction lastdir = Direction.NotSet)
        {
            if (string.IsNullOrWhiteSpace(word))
                return new List<TextBox>();

            Tuple<int, int> lastpos = null;

            int x = -1;
            int y = -1;
            if (lastaccum != null)
            {
                lastpos = GetControlPos(lastaccum.Last());
                x = lastpos.Item1;
                y = lastpos.Item2;
            }

            foreach (var f in GetSquare(word[posc], fm, x, y, lastaccum, lastdir))
            {
                //bad branch
                if (f == null)
                    continue;

                var lastaccumnew = new List<TextBox>();
                if (lastaccum != null)
                    lastaccumnew.AddRange(lastaccum);
                lastaccumnew.Add(f);

                var found = new List<TextBox>();
                var ret = new List<TextBox>();

                //finished
                if (posc == (word.Length - 1))
                {
                    ret.Add(f);
                    return ret;
                }

                if (lastaccum == null && lastdir == Direction.NotSet)
                {
                    found = FindMatch(word, posc + 1, fm, lastaccumnew);
                }
                else if (lastaccum != null)
                {
                    var thisdir = GetDirection(lastaccum.Last(), f);
                    found = FindMatch(word, posc + 1, fm, lastaccumnew, thisdir);
                }

                if (found.Count == 0)
                    continue;

                ret.Add(f);
                ret.AddRange(found);
                return ret;
            }

            return new List<TextBox>();
        }

        private static void SetSearchDimensions(int startX, int startY, FindMode fm, ref int miny, ref int maxy, ref int minx, ref int maxx, Direction d = Direction.NotSet)
        {
            miny = 0;
            maxy = height;
            minx = 0;
            maxx = width;

            if (startX != -1)
            {
                minx = startX - 1;
                maxx = startX + 1;
            }

            if (startY != -1)
            {
                miny = startY - 1;
                maxy = startY + 1;
            }

            //constrain the possible 
            if (d != Direction.NotSet && fm == FindMode.Crossword)
            {
                minx = maxx = startX + Directions[d].Item1;
                miny = maxy = startY + Directions[d].Item2;
            }
        }

        /// <summary>
        /// from a starting point, find a letter
        /// </summary>
        /// <param name="letter"></param>
        /// <param name="fm"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="ignorelist"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        private static IEnumerable<TextBox> GetSquare(char letter, FindMode fm, int startX = -1, int startY = -1, List<TextBox> ignorelist = null, Direction d = Direction.NotSet)
        {
            //if we want a direction, must have a position as well
            if (!((startX == -1 || startY == -1) && d != Direction.NotSet))
            {
                int minx = -1, miny = -1, maxx = -1, maxy = -1;
                SetSearchDimensions(startX, startY, fm, ref miny, ref maxy, ref minx, ref maxx, d);

                for (int y = miny; y <= maxy; y++)
                {
                    if (OutOfBounds(y, false))
                        continue;

                    for (int x = minx; x <= maxx; x++)
                    {
                        if (OutOfBounds(x, true))
                            continue;

                        if (y == startY && x == startX)
                            continue;

                        var c = GetControl(x, y);

                        if (ignorelist != null && ignorelist.Contains(c))
                            continue;

                        if (grid[y][x] == letter)
                            yield return GetControl(x, y);
                    }
                }
            }
            yield return null;
        }

        public static void FindAndHighlight(String word, FindMode fm)
        {
            word = word.ToUpper();
            var matches = FindMatch(word, 0, fm);

            foreach (var c in PU.Controls)
            {
                SetTBColour(c as TextBox, matches.Contains(c));
            }
        }

        private static void SetTBColour(TextBox tb, bool find)
        {
            if (find)
            {
                tb.ForeColor = findfront;
                tb.BackColor = findback;
            }
            else
            {
                tb.ForeColor = normalfront;
                tb.BackColor = normalback;
            }
        }



    }
}
