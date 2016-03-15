using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using ANDREICSLIB.ClassExtras;

namespace HistogramOCRTrainer
{
    /// <summary>
    ///     example usage: https://github.com/andreigec/Word-Find-Solver
    /// </summary>
    public class HistogramOCR
    {
        private const char v = '\v';
        /// <summary>
        /// The white bitmap
        /// </summary>
        public static Bitmap WhiteBitmap = new Bitmap(1, 1);
        public int HistogramHeight;
        public int HistogramWidth;
        public List<HistogramLetter> Letters;

        /// <summary>
        /// initialise with a file, or blank
        /// </summary>
        /// <param name="HWidth">Width of the h.</param>
        /// <param name="HHeight">Height of the h.</param>
        public HistogramOCR(int HWidth = 20, int HHeight = 20)
        {
            HistogramHeight = HHeight;
            HistogramWidth = HWidth;
            Letters = new List<HistogramLetter>();
            WhiteBitmap.SetPixel(0, 0, Color.White);
        }

        /// <summary>
        /// Serialises the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void Serialise(string filename)
        {
            var ret = HistogramWidth + v.ToString(CultureInfo.InvariantCulture) + HistogramHeight +
                      v.ToString(CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
            var text = Letters.Aggregate("", (a, b) => a + (b.Serialise() + v.ToString(CultureInfo.InvariantCulture)));
            ret += text;

            FileExtras.SaveToFile(filename, ret);
        }

        /// <summary>
        /// Des the serialise.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public static HistogramOCR DeSerialise(string filename)
        {
            var ret = new HistogramOCR();
            try
            {
                var text = FileExtras.LoadFile(filename);

                var t1 = text.Split(new[] { v }, StringSplitOptions.RemoveEmptyEntries);

                var w = int.Parse(t1[0]);
                var h = int.Parse(t1[1]);
                var letters = new List<HistogramLetter>();
                for (var a = 2; a < t1.Count(); a++)
                {
                    var ltext = t1[a];
                    var l = HistogramLetter.DeSerialise(ltext);
                    if (letters.Any(s => s.Letter.Equals(l.Letter)) == false)
                        letters.Add(l);
                }

                //set
                ret.HistogramHeight = h;
                ret.HistogramWidth = w;
                ret.Letters = letters;
            }
            catch (ArgumentException)
            {
            }


            return ret;
        }

        /// <summary>
        /// Splits up.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="whiteToSpace">default = will calculate</param>
        /// <returns></returns>
        public Bitmap[][] SplitUp(Bitmap b, int whiteToSpace = -1)
        {
            var retlists = new List<List<Bitmap>>();
            //get rows
            var rows = Split(b, true, ref whiteToSpace);
            //for each of the rows, get the columns
            var row = 0;
            foreach (var r in rows)
            {
                var cols = Split(r, false, ref whiteToSpace);
                if (retlists.Count <= row)
                    retlists.Add(new List<Bitmap>());

                retlists[row].AddRange(cols);
                row++;
            }
            //return results as array
            //normalise
            redo:
            for (var y = 0; y < retlists.Count(); y++)
            {
                for (var x = 0; x < retlists[y].Count(); x++)
                {
                    var norm = Normalise(retlists[y][x]);
                    if (norm == null)
                    {
                        retlists[y].RemoveAt(x);
                        goto redo;
                    }
                    retlists[y][x] = norm;
                }
            }
            return retlists.Select(s => s.ToArray()).ToArray();
        }

        /// <summary>
        /// Splits the specified b.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="byRow">if set to <c>true</c> [by row].</param>
        /// <param name="whiteToSpace">The white to space.</param>
        /// <returns></returns>
        /// <exception cref="Exception">error splitting</exception>
        private static IEnumerable<Bitmap> Split(Bitmap b, bool byRow, ref int whiteToSpace)
        {
            //split rows then cols
            var w = b.Width;
            var h = b.Height;
            var to = byRow ? h : w;

            var items = new List<Bitmap>();
            var whiteStart = 0;
            var lastValue = -1;

            for (var v = 0; v < to; v++)
            {
                //keep going until we find a row with black
                var isWhite = BitmapExtras.IsRowOrColSameColour(b, v, byRow, Color.White);

                if (isWhite)
                {
                    if (whiteStart == v)
                    {
                        whiteStart++;
                        continue;
                    }

                    //see if there is a space at the start
                    if (whiteToSpace != -1 && items.Count == 0 && v >= whiteToSpace)
                    {
                        items.Add(WhiteBitmap);
                    }

                    //if there is more than a certain amount of space check/add a white item
                    if (whiteToSpace != -1 && lastValue != -1 && ((v - lastValue) >= whiteToSpace))
                    {
                        items.Add(WhiteBitmap);
                    }

                    //if this occurs, there is a character before
                    Bitmap i = null;
                    if (byRow)
                    {
                        //if by row, and the second time, we can estimate the white space for a space
                        if (whiteToSpace == -1 && items.Count == 1)
                            whiteToSpace = whiteStart;

                        i = BitmapExtras.Crop(b, -1, v - whiteStart, 0, whiteStart);
                    }
                    else
                    {
                        i = BitmapExtras.Crop(b, v - whiteStart, -1, whiteStart, 0);
                    }
                    if (i == null)
                    {
                        throw new Exception("error splitting");
                    }

                    items.Add(i);

                    lastValue = v;
                    whiteStart = v + 1;
                }
            }

            //see if there is a space at the end
            if (whiteToSpace != -1 && lastValue != -1 && ((to - lastValue) >= whiteToSpace))
            {
                items.Add(WhiteBitmap);
            }

            return items;
        }

        /// <summary>
        /// Performs the ocr.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="spaceForWhiteSpace">The space for white space.</param>
        /// <returns></returns>
        public string[] PerformOCR(Bitmap b, int spaceForWhiteSpace = -1)
        {
            //split bitmap here
            var bits = SplitUp(b, spaceForWhiteSpace);
            //for each bitmap, find the corresponding character
            var ret = new string[bits.Count()];
            var row = 0;
            foreach (var brow in bits)
            {
                foreach (var bcol in brow)
                {
                    var c = PerformOCRCharacter(bcol);
                    ret[row] += c;
                }
                row++;
            }
            return ret;
        }

        /// <summary>
        /// Performs the ocr.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="spaceForWhiteSpace">The space for white space.</param>
        /// <returns></returns>
        public string[] PerformOCR(string filename, int spaceForWhiteSpace = -1)
        {
            try
            {
                var b = new Bitmap(filename);
                return PerformOCR(b);
            }
            catch (Exception)
            {
            }
            return null;
        }

        /// <summary>
        /// Performs the ocr character perfect.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public char? PerformOCRCharacterPerfect(Bitmap b)
        {
            //go through all the saved characters, and print the most likely
            var res = Letters.FirstOrDefault(s => GetOffScore(b, s) == 0);
            if (res == null)
                return null;
            return res.Letter;
        }

        /// <summary>
        /// Performs the ocr character.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="perfectOnly">if set to <c>true</c> [perfect only].</param>
        /// <returns></returns>
        private char PerformOCRCharacter(Bitmap b, bool perfectOnly = false)
        {
            //go through all the saved characters, and print the most likely
            var best = Letters.OrderBy(s => GetOffScore(b, s));
            var best1 = best.First().Letter;
            return best1;
        }

        /// <summary>
        /// Trains the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="letters">The letters.</param>
        /// <param name="spaceForWhiteSpace">The space for white space.</param>
        /// <returns></returns>
        public bool Train(string filename, string[] letters, int spaceForWhiteSpace = -1)
        {
            //split up file into separate letters
            try
            {
                var b = new Bitmap(filename);
                //split bitmap here
                var bits = SplitUp(b, spaceForWhiteSpace);

                //train each character
                var column = 0;
                var row = 0;
                foreach (var bline in bits)
                {
                    row = 0;
                    //can only train if letters has this
                    if (letters.Count() <= column)
                        break;

                    foreach (var bchar in bline)
                    {
                        if (letters[column].Count() <= row)
                            break;

                        var l = letters[column][row];
                        row++;

                        //only if doesnt already exist
                        if (Letters.Any(s => s.Letter.Equals(l)))
                            continue;

                        Train(bchar, l);
                        //   bchar.Save("out_" + l.ToString() + ".bmp");
                    }
                    column++;
                }
                return true;
            }
            catch (ArgumentNullException)
            {
                return false;
            }
        }

        /// <summary>
        /// take an image of a letter and a character of what it is, and train the ocr
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="letterChar">The letter character.</param>
        /// <returns></returns>
        public bool Train(Bitmap b, char letterChar)
        {
            try
            {
                var l = new HistogramLetter(b, letterChar);
                Letters.Add(l);
                return true;
            }
            catch (ArgumentNullException)
            {
            }

            return false;
        }

        /// <summary>
        /// get score. score of 0 is perfect
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="l">The l.</param>
        /// <returns></returns>
        private int GetOffScore(Bitmap b, HistogramLetter l)
        {
            var bba = BitmapBoolArray.GetBitmapBoolArray(b);
            var h = HistogramLetter.GetHistogram(bba, b);
            var score = l.GetHistogramOffsetScore(h.Item1, h.Item2);
            return score;
        }

        /// <summary>
        /// trim white and resize to created dimensions
        /// </summary>
        /// <param name="b1">The b1.</param>
        /// <returns></returns>
        public Bitmap Normalise(Bitmap b1)
        {
            //white 1x1=empty
            if (b1.Width == 1 && b1.Height == 1)
            {
                return b1;
            }

            //trim white
            var b = BitmapExtras.RemoveExcessWhitespace(b1);
            if (b == null)
                return null;

            //resize to internal size
            b = BitmapExtras.StretchBitmap(b, HistogramWidth, HistogramHeight);

            //convert to black and white
            b = BitmapExtras.ChangeAllNonWhitePixelsToColour(b, Color.Black);

            return b;
        }

        /// <summary>
        /// 
        /// </summary>
        public class BitmapBoolArray
        {
            /// <summary>
            ///     true if value, false if white
            /// </summary>
            public bool[][] bitmapBool;

            private BitmapBoolArray(int w, int h)
            {
                bitmapBool = ArrayExtras.InstantiateArray<bool>(w, h);
            }

            /// <summary>
            /// Gets the bitmap bool array.
            /// </summary>
            /// <param name="b">The b.</param>
            /// <returns></returns>
            public static BitmapBoolArray GetBitmapBoolArray(Bitmap b)
            {
                var w = b.Width;
                var h = b.Height;

                var bba = new BitmapBoolArray(w, h);
                for (var y = 0; y < h; y++)
                {
                    for (var x = 0; x < w; x++)
                    {
                        var p = b.GetPixel(x, y);
                        var white = p.R == 255 && p.G == 255 && p.B == 255;
                        bba.bitmapBool[y][x] = !white;
                    }
                }
                return bba;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class HistogramLetter
        {
            private const char b = '\b';
            private const char f = '\f';
            internal char Letter;
            internal int[] XValues;
            internal int[] YValues;

            private HistogramLetter()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="HistogramLetter"/> class.
            /// </summary>
            /// <param name="b">The b.</param>
            /// <param name="letter">The letter.</param>
            public HistogramLetter(Bitmap b, char letter)
            {
                var bba = BitmapBoolArray.GetBitmapBoolArray(b);
                Letter = letter;
                var val = GetHistogram(bba, b);
                XValues = val.Item1;
                YValues = val.Item2;
            }

            /// <summary>
            /// Serialises this instance.
            /// </summary>
            /// <returns></returns>
            public string Serialise()
            {
                var ret = Letter + b.ToString(CultureInfo.InvariantCulture);

                for (var a = 0; a < XValues.Count(); a++)
                {
                    ret += XValues[a] + f.ToString(CultureInfo.InvariantCulture);
                }

                ret += b;

                for (var a = 0; a < YValues.Count(); a++)
                {
                    ret += YValues[a] + f.ToString(CultureInfo.InvariantCulture);
                }

                return ret;
            }

            /// <summary>
            /// Des the serialise.
            /// </summary>
            /// <param name="text">The text.</param>
            /// <returns></returns>
            public static HistogramLetter DeSerialise(string text)
            {
                HistogramLetter ret = null;
                try
                {
                    var t1 = text.Split(new[] { b }, StringSplitOptions.RemoveEmptyEntries);

                    var let = t1[0][0];

                    //get values

                    var xarr = t1[1];
                    var xval = xarr.Split(new[] { f }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

                    var yarr = t1[2];
                    var yval = yarr.Split(new[] { f }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

                    //set values
                    ret = new HistogramLetter { XValues = xval, YValues = yval, Letter = let };
                }
                catch (ArgumentException)
                {
                }

                return ret;
            }

            /// <summary>
            /// Gets the histogram.
            /// </summary>
            /// <param name="bba">The bba.</param>
            /// <param name="b">The b.</param>
            /// <returns></returns>
            public static Tuple<int[], int[]> GetHistogram(BitmapBoolArray bba, Bitmap b)
            {
                var w = b.Width;
                var h = b.Height;
                var XValues = new int[w];
                var YValues = new int[h];

                for (var y = 0; y < h; y++)
                {
                    for (var x = 0; x < w; x++)
                    {
                        var v = bba.bitmapBool[y][x] ? 1 : 0;
                        YValues[y] += v;
                        XValues[x] += v;
                    }
                }
                return new Tuple<int[], int[]>(XValues, YValues);
            }

            /// <summary>
            /// Gets the histogram offset score.
            /// </summary>
            /// <param name="xv">The xv.</param>
            /// <param name="yv">The yv.</param>
            /// <returns></returns>
            public int GetHistogramOffsetScore(int[] xv, int[] yv)
            {
                //go by the largest
                var letterwidth = XValues.Count();
                var letterheight = YValues.Count();
                var inwidth = xv.Count();
                var inheight = yv.Count();
                var w = Math.Max(letterwidth, inwidth);
                var h = Math.Max(letterheight, inheight);

                var score = 0;
                for (var y = 0; y < h; y++)
                {
                    var letterval = 0;
                    var inval = 0;
                    if (y < letterheight)
                        letterval += YValues[y];

                    if (y < inheight)
                        inval += yv[y];

                    score += Math.Abs(letterval - inval);
                }

                for (var x = 0; x < w; x++)
                {
                    var letterval = 0;
                    var inval = 0;
                    if (x < letterwidth)
                        letterval += XValues[x];

                    if (x < inwidth)
                        inval += xv[x];

                    score += Math.Abs(letterval - inval);
                }

                return score;
            }
        }
    }
}