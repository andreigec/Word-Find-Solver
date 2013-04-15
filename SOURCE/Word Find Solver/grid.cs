using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ANDREICSLIB;
using ANDREICSLIB.ClassExtras;

namespace Word_Find_Solver
{
    public static class Grid
    {
        public class GridPoint
        {
            public TextBox Tb;
            public char C;
            public int LetterMultiplier = 1;
            public int WordMultiplierExtra = 0;
            public int X;
            public int Y;

            public GridPoint()
            {

            }

            public GridPoint(char c, TextBox tb, int x, int y)
            {
                X = x;
                Y = y;
                Tb = tb;
                C = c;
            }

            public GridPoint(TextBox tb, int x, int y)
            {
                Tb = tb;
                X = x;
                Y = y;
            }
        }

        public static Form1 Baseform;
        private static int width;
        private static int height;
        /// <summary>
        /// a textbox/char for each grid point
        /// </summary>
        private static GridPoint[][] grid;
        private static PanelReplacement pu;
        private static readonly Color Normalback = Color.CornflowerBlue;
        private static readonly Color Normalfront = Color.Yellow;
        private static readonly Color Findback = Color.Yellow;
        private static readonly Color Findfront = Color.CornflowerBlue;
        private static Trie trieroot;
        private static Dictionary<char, int> letterScores = new Dictionary<char, int>();
        private static readonly Random R = new Random();

        /// <summary>
        /// direction to an x,y tuple for relative direction
        /// </summary>
        private static Dictionary<Direction, Tuple<int, int>> directions;

        private static Direction GetDirection(GridPoint basetb, GridPoint comptb)
        {
            int xrel = comptb.X - basetb.X;
            int yrel = comptb.Y - basetb.Y;

            var t = new Tuple<int, int>(xrel, yrel);
            var d = directions.Where(s => s.Value.Equals(t));

            if (d.Count() == 1)
                return d.First().Key;

            return Direction.NotSet;
        }

        private static char GenNewLetter(int max)
        {
            int current = R.Next() % max;
            foreach (var kvp in letterScores)
            {
                if (current < kvp.Value)
                    return kvp.Key;
                current -= kvp.Value;
            }
            return 'x';
        }

        public static void RandomiseLetters(FindMode fm)
        {
            //create stack
            int max = letterScores.Sum(s => s.Value);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var c = GenNewLetter(max);
                    grid[y][x].C = c;
                    grid[y][x].Tb.Text = c.ToString();
                }
            }
        }

        public enum FindMode
        {
            NotSet,
            Crossword,
            AllDirFromLast
        }

        public enum Direction
        {
            NotSet,
            Down,
            DownLeft,
            Left,
            UpLeft,
            Up,
            UpRight,
            Right,
            DownRight
        }

        public static void InitPanel(PanelReplacement pu)
        {
            Grid.pu = pu;
        }

        public static void InitWords()
        {
            var raw = EmbeddedResources.ReadEmbeddedResource("dictionarywords.txt").Split(new[] { "\r\n" },
                                                                                          StringSplitOptions.
                                                                                              RemoveEmptyEntries);
            trieroot = new Trie();
            foreach (var s in raw)
            {
                trieroot.AddWord(s);
            }

            //add relative directions
            directions = new Dictionary<Direction, Tuple<int, int>>
                             {
                                 {Direction.NotSet, new Tuple<int, int>(0, 0)},
                                 {Direction.DownLeft, new Tuple<int, int>(-1, +1)},
                                 {Direction.Left, new Tuple<int, int>(-1, +0)},
                                 {Direction.UpLeft, new Tuple<int, int>(-1, -1)},
                                 {Direction.Up, new Tuple<int, int>(+0, -1)},
                                 {Direction.UpRight, new Tuple<int, int>(+1, -1)},
                                 {Direction.Right, new Tuple<int, int>(+1, +0)},
                                 {Direction.DownRight, new Tuple<int, int>(+1, +1)},
                                 {Direction.Down, new Tuple<int, int>(+0, +1)}
                             };

            //letter scores
            letterScores = new Dictionary<char, int>
                               {
                                   {'A', 1},
                                   {'B', 3},
                                   {'C', 3},
                                   {'D', 2},
                                   {'E', 1},
                                   {'F', 4},
                                   {'G', 2},
                                   {'H', 4},
                                   {'I', 1},
                                   {'J', 8},
                                   {'K', 5},
                                   {'L', 1},
                                   {'M', 3},
                                   {'N', 1},
                                   {'O', 1},
                                   {'P', 3},
                                   {'Q', 10},
                                   {'R', 1},
                                   {'S', 1},
                                   {'T', 1},
                                   {'U', 1},
                                   {'V', 4},
                                   {'W', 4},
                                   {'X', 8},
                                   {'Y', 4},
                                   {'Z', 10}
                               };
        }

        /// <summary>
        /// initialise the grid
        /// </summary>
        /// <param name="widthI"></param>
        /// <param name="heightI"></param>
        /// <param name="rows"></param>
        public static void InitGrid(int widthI, int heightI, string[] rows = null)
        {
            width = widthI;
            height = heightI;

            pu.clearControls();
            grid = new GridPoint[heightI][];
            var f = new Font(FontFamily.GenericSansSerif, 20, FontStyle.Regular);

            for (var y = 0; y < heightI; y++)
            {
                grid[y] = new GridPoint[widthI];

                for (int x = 0; x < widthI; x++)
                {
                    //create the grid point and textbox
                    var tb = new TextBox
                    {
                        Width = 30,
                        Height = 16,
                        BorderStyle = BorderStyle.None,
                        BackColor = Normalback,
                        ForeColor = Normalfront,
                        MaxLength = 1,
                        ContextMenuStrip = Baseform.GridLetterContext,
                        TextAlign = HorizontalAlignment.Center,
                        Font = f,
                        //x,y
                        Name = x + "," + y,
                    };
                    tb.TextChanged += Baseform.GridTBChangeEvent;
                    pu.addControl(tb, x != (width - 1));
                    grid[y][x] = new GridPoint(tb, x, y);

                    //if text has been passed in, set the text now
                    if (rows != null)
                    {
                        if (x >= rows[y].Length)
                            break;
                        if (rows[y].Length >= x)
                        {
                            var c = rows[y][x].ToString(CultureInfo.InvariantCulture).ToUpper()[0];
                            grid[y][x].C = c;
                            grid[y][x].Tb.Text = c.ToString(CultureInfo.InvariantCulture);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// get the very last textbox for panel resize
        /// </summary>
        /// <returns></returns>
        public static TextBox GetLastTextbox()
        {
            var gp = grid[height - 1][width - 1];
            return gp.Tb;
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
                var s = grid[y].Aggregate("", (a, b) => a + b.C);
                sw.WriteLine(s);
            }
            sw.Close();
            fs.Close();
        }

        /// <summary>
        /// from a starting letter/position, find all possible words from this
        /// </summary>
        /// <param name="word"></param>
        /// <param name="history"></param>
        /// <param name="lastdir"></param>
        /// <param name="fm"></param>
        /// <param name="minx"></param>
        /// <param name="maxx"></param>
        /// <param name="miny"></param>
        /// <param name="maxy"></param>
        private static IEnumerable<Tuple<string, int>> Solve(String word, List<GridPoint> history, Direction lastdir, FindMode fm, int minx,
                                          int maxx, int miny, int maxy)
        {
            var retwords = new List<Tuple<string, int>>();
            //if its a word, add
            if (trieroot.ContainsWord(word, false))
            {
                var sc = GetWordScore(history);
                retwords.Add(new Tuple<string, int>(word, sc));
            }

            for (int y = miny; y <= maxy; y++)
            {
                if (OutOfBounds(y, false))
                    continue;

                for (int x = minx; x <= maxx; x++)
                {
                    if (OutOfBounds(x, true))
                        continue;

                    var p = grid[y][x];

                    if (history.Contains(p))
                        continue;

                    Direction newdir = history.Count == 0 ? Direction.NotSet : GetDirection(history.Last(), p);

                    if (newdir != lastdir && lastdir != Direction.NotSet && fm == FindMode.Crossword)
                        continue;

                    String newword = word + p.C;

                    if (trieroot.ContainsWord(newword, true))
                    {
                        var newhistory = new List<GridPoint>();
                        newhistory.AddRange(history);
                        newhistory.Add(p);

                        int nminx = -1, nminy, nmaxx = -1, nmaxy;
                        SetSearchDimensions(x, y, fm, out nminy, out nmaxy, out nminx, out nmaxx, newdir);
                        retwords.AddRange(Solve(newword, newhistory, newdir, fm, nminx, nmaxx, nminy, nmaxy));
                    }
                }
            }
            return retwords;
        }

        private static int GetWordScore(IEnumerable<GridPoint> history)
        {
            int score = 0;
            int wordmult = 1;

            foreach(var h in history)
            {
                wordmult += h.WordMultiplierExtra;
                score += letterScores[h.C]*h.LetterMultiplier;
            }

            score *= wordmult;
            
            return score;
        }

        /// <summary>
        /// find all words and their scores in a grid and output the words found to a listbox
        /// </summary>
        /// <param name="fm"></param>
        /// <param name="maxret"></param>
        public static List<Tuple<string, int>> Solve(FindMode fm, int maxret = 100)
        {
            var foundwords = new List<Tuple<string, int>>();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var gp = grid[y][x];
                    int miny;
                    int maxy;
                    int minx;
                    int maxx;
                    SetSearchDimensions(x, y, fm, out miny, out maxy, out minx, out maxx);
                    foundwords.AddRange(Solve(grid[y][x].C.ToString(), new List<GridPoint> { gp }, Direction.NotSet, fm,
                                              minx, maxx, miny, maxy));
                }
            }

            foundwords = foundwords.Distinct().ToList();

            return foundwords;
        }
       
private static bool OutOfBounds(int a, bool isX)
        {
            bool change = a < 0;
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

        /*
        private static Tuple<int, int> GetControlPos(GridPoint tb)
        {
            var p = tb.Name.IndexOf(':');

            int x = int.Parse(tb.Name.Substring(0, p));
            int y = int.Parse(tb.Name.Substring(p + 1));

            return new Tuple<int, int>(x, y);
        }
        */

        private static List<GridPoint> FindMatch(string word, int posc, FindMode fm, List<GridPoint> lastaccum = null,
                                               Direction lastdir = Direction.NotSet)
        {
            if (string.IsNullOrWhiteSpace(word))
                return new List<GridPoint>();

            int x = -1;
            int y = -1;
            if (lastaccum != null)
            {
                var lgp = lastaccum.Last();
                var lastpos = new Tuple<int, int>(lgp.X, lgp.Y);
                x = lastpos.Item1;
                y = lastpos.Item2;
            }

            foreach (var f in GetSquare(word[posc], fm, x, y, lastaccum, lastdir))
            {
                //bad branch
                if (f == null)
                    continue;

                var lastaccumnew = new List<GridPoint>();
                if (lastaccum != null)
                    lastaccumnew.AddRange(lastaccum);
                lastaccumnew.Add(f);

                var found = new List<GridPoint>();
                var ret = new List<GridPoint>();

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

            return new List<GridPoint>();
        }

        private static void SetSearchDimensions(int startX, int startY, FindMode fm, out int miny, out int maxy,
                                                out int minx, out int maxx, Direction d = Direction.NotSet)
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
                minx = maxx = startX + directions[d].Item1;
                miny = maxy = startY + directions[d].Item2;
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
        private static IEnumerable<GridPoint> GetSquare(char letter, FindMode fm, int startX = -1, int startY = -1,
                                                      List<GridPoint> ignorelist = null, Direction d = Direction.NotSet)
        {
            //if we want a direction, must have a position as well
            if (!((startX == -1 || startY == -1) && d != Direction.NotSet))
            {
                int minx, miny, maxx, maxy;
                SetSearchDimensions(startX, startY, fm, out miny, out maxy, out minx, out maxx, d);

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

                        var gp = grid[y][x];

                        if (ignorelist != null && ignorelist.Contains(gp))
                            continue;

                        if (grid[y][x].C == letter)
                            yield return grid[y][x];
                    }
                }
            }
            yield return null;
        }

        public static void FindAndHighlight(String word, FindMode fm)
        {
            word = word.ToUpper();
            var matches = FindMatch(word, 0, fm);

            foreach (TextBox c in pu.Controls)
            {
                SetTbColour(c , matches.Any(s=>s.Tb==c));
            }
        }

        private static void SetTbColour(TextBox tb, bool find)
        {
            if (find)
            {
                tb.ForeColor = Findfront;
                tb.BackColor = Findback;
            }
            else
            {
                tb.ForeColor = Normalfront;
                tb.BackColor = Normalback;
            }
        }

        /// <summary>
        /// get x,y of text box passed in.
        /// </summary>
        /// <param name="tb"> </param>
        /// <returns>x,y tuple</returns>
        public static GridPoint GetPointFromTextBox(TextBox tb)
        {
            var res = ((String)tb.Name).Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            int x = int.Parse(res[0]);
            int y = int.Parse(res[1]);
            return grid[y][x];
        }

        public static void SetGridPointValue(TextBox tb, string sFirstOnly, bool setTextboxValue = true)
        {
            var gp = GetPointFromTextBox(tb);
            if (sFirstOnly.Length == 0)
                sFirstOnly = "\0";
            var c2 = sFirstOnly.ToUpper()[0];
            gp.Tb.Text = c2.ToString(CultureInfo.InvariantCulture);
            gp.C = c2;
        }
    }
}
