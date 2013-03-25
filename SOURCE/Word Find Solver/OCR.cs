using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using ANDREICSLIB;
using tessnet2;

namespace Word_Find_Solver
{
    public static class OCR
    {
        /// <summary>
        /// replace all colours apart from the one passed in with white, and the passed in colour as black
        /// </summary>
        /// <param name="Bmp"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Bitmap OnlyAllowBlackAndColour(Bitmap Bmp, int r, int g, int b)
        {
            //int rgb;
            Color c;

            for (int y = 0; y < Bmp.Height; y++)
                for (int x = 0; x < Bmp.Width; x++)
                {
                    c = Bmp.GetPixel(x, y);
                    int rgb = 255;
                    const int blackd = 50;
                    if ((c.R <= blackd && c.G <= blackd && c.B <= blackd) || (c.R == r && c.G == g && c.B == b))
                    {
                        rgb = 0;
                    }
                    //rgb = (int)((c.R + c.G + c.B) / 3);
                    Bmp.SetPixel(x, y, Color.FromArgb(rgb, rgb, rgb));
                }
            return Bmp;
        }
      
        /// <summary>
        /// return avg x,avg y,max x, max y (max rules out outliers)
        /// </summary>
        /// <param name="words"></param>
        /// <param name="ignorecount"></param>
        /// <param name="maxdev"></param>
        /// <returns></returns>
        public static Tuple<int, int,int,int> GetDistanceValues(List<Character> words, int ignorecount = 5, double maxdevx = .72,double maxdevy=20)
        {
            //double maxdev = 5;
            //int ignorecount =8;

            //average space between letters
            double avgSpaceBetweenX = -1;
            int lastx = -1;

            double avgSpaceBetweenY = -1;
            int lasty = -1;

            int maxx = -1;
            int maxy = -1;

            int avgcount = 0;
            foreach (var w2 in words)
            {
                if (lastx==-1)
                {
                    lastx = w2.Left;
                }

                if (lasty==-1)
                {
                    lasty = w2.Bottom;
                }

                int difx = Math.Abs(w2.Left - lastx);
                int dify = Math.Abs(w2.Bottom - lasty);

                //running avg if more than X items
                double ravgx = -1, ravgy = -1;
                if (avgcount > ignorecount)
                {
                    ravgx = avgSpaceBetweenX / avgcount;
                    ravgy = avgSpaceBetweenY / avgcount;
                }
                var gox = (ravgx != -1 && Math.Abs(difx - ravgx) > (ravgx*maxdevx));
                var goy = (ravgy != -1 && Math.Abs(dify - ravgy) > (ravgy*maxdevy));
                //if the difference is too large (>?x), then just add the average
                if (gox)
                    avgSpaceBetweenX += ravgx;
                else
                    avgSpaceBetweenX += difx;

                if (goy)
                    avgSpaceBetweenY += ravgy;
                else
                    avgSpaceBetweenY += dify;

                //max
                if ((maxx == -1 || difx > maxx) && (gox == false))
                    maxx = difx;

                if ((maxy == -1 || dify > maxy) && (goy == false))
                    maxy = dify;

                avgcount++;
                lastx = w2.Left;
                lasty = w2.Bottom;
            }

            avgSpaceBetweenX /= avgcount;
            avgSpaceBetweenY /= avgcount;
            return new Tuple<int, int, int, int>((int)avgSpaceBetweenX, (int)avgSpaceBetweenY,maxx,maxy );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="words"></param>
        /// <param name="b"></param>
        /// <returns>width,height,array</returns>
        public static Tuple<int, int, string[]> LettersOnLines(List<tessnet2.Character> words, Bitmap b)
        {
           /*
            for (int i = 5; i < 50; i++)
            {
                for (int i1 = 5; i1 < 50; i1++)
                {
                    var ret1=GetSpaceMax(words, i, i1);
                    if ((ret1.Item1 >= 60 && ret1.Item1 <= 80) && (ret1.Item2 >= 60 && ret1.Item2 <= 80))
                    {
                        int a = 0;
                        a = 0;
                    }
                } 
            }
            */
            int maxDeviationY = -1;
            int maxDeviationX = -1;

            var v = GetDistanceValues(words);
            maxDeviationX = v.Item3;
            maxDeviationY =v.Item4;

            var charrow = new List<string>();
            int lasty = -1;
            int lastx = -1;

            string row = "";

                foreach (var w2 in words)
                {
                    var c = (char)w2.Value;
                    int difHeight = Math.Abs(w2.Bottom - lasty);
                    int difWidth = Math.Abs(w2.Left - lastx);

                    if (lasty == -1 || difHeight > maxDeviationY)
                    {
                        //if dif between right and w2.right > 50, theres a space at the end
                        int difright = Math.Abs(lastx - b.Width);
                        if (difright > maxDeviationX)
                        {
                            row += ' ';
                        }

                        if (string.IsNullOrWhiteSpace(row) == false)
                            charrow.Add(row);

                        row = "";

                        //space at start
                        if (w2.Left >= maxDeviationX)
                        {
                            row += ' ';
                        }

                        lasty = w2.Bottom;
                        lastx = -1;
                    }

                    //dif x> deviation = space
                    if (lastx != -1 && difWidth > maxDeviationX)
                    {
                        row += ' ';
                    }
                    lastx = w2.Left;

                    row += c;
                }

            //add last row
            charrow.Add(row);

            //get average row width
            var rowlengthavg = (double)charrow.Average(s => s.Length);
            //max dev=avg*1.2
            rowlengthavg *= 1.2;
            /*
            //manual replacements
            //if>11 && contains ixi = n
            for (int a = 0; a < charrow.Count; a++)
            {
                var l = charrow[a];
                if (l.Length > rowlengthavg)
                {
                    l = StringExtras.ReplaceAllChars(l, "IXI", "N");
                    charrow[a] = l;
                }
            }
            */
            //averages
            var avgw = charrow.Max(s => s.Length);
            int h = charrow.Count;

            var ret = new Tuple<int, int, string[]>((int)avgw, h, charrow.ToArray());
            return ret;
        }

        public static Tuple<int, int, string[]> LoadImage(Bitmap image1, bool IgnorePartY)
        {
            Bitmap image = image1;
            //crop a bit if wanted
            if (IgnorePartY)
            {
                var rect = new Rectangle(0, 100, image1.Width, image1.Height - 100);
                image = image1.Clone(rect, PixelFormat.DontCare);
            }

            //only get letters
            image = OnlyAllowBlackAndColour(image, 0, 0, 0);

            //expand
            image=BitmapExtras.ResizeBitmap(image, (int) (image.Width * .7), (int) (image.Height * .7));

            var ocr = new tessnet2.Tesseract();
            ocr.SetVariable("tessedit_char_whitelist", "0123456789ABCDEFGHIJKLMOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");
            ocr.Init(null, "eng", false);
            var result = ocr.DoOCR(image, Rectangle.Empty);

            var r1 = new List<Character>();
            foreach (var x in result)
            {
                r1.AddRange(x.CharList);
            }

            return LettersOnLines(r1, image);
        }
    }
}
