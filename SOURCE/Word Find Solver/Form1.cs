using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ANDREICSLIB;
using ANDREICSLIB.ClassExtras;
using ANDREICSLIB.NewControls;
using Word_Find_Solver.ServiceReference1;

namespace Word_Find_Solver
{
    public partial class Form1 : Form
    {
        #region licensing

        private const string AppTitle = "Word Find Solver";
        private const double AppVersion = 0.7;
        private const String HelpString = "";

        private readonly String OtherText =
            @"©" + DateTime.Now.Year +
            @" Andrei Gec (http://www.andreigec.net)

Licensed under GNU LGPL (http://www.gnu.org/)

OCR © Tessnet2/Tesseract (http://www.pixel-technology.com/freeware/tessnet2/)(https://code.google.com/p/tesseract-ocr/)
Zip Assets © SharpZipLib (http://www.sharpdevelop.net/OpenSource/SharpZipLib/)
";
        #endregion

        #region locks
        public static bool AllowLookupEvents = true;
        public static bool AllowTbChangeEvent = true;
        #endregion locks

        private void Form1_Load(object sender, EventArgs e)
        {
            Licensing.CreateLicense(this, menuStrip1, new Licensing.SolutionDetails(GetDetails, HelpString, AppTitle, AppVersion, OtherText));
            Grid.Baseform = this;
            Grid.InitPanel(grid);
            Grid.InitWords();
            InitGrid(8, 8);
        }

        public Licensing.DownloadedSolutionDetails GetDetails()
        {
            try
            {
                var sr = new ServicesClient();
                var ti = sr.GetTitleInfo(AppTitle);
                if (ti == null)
                    return null;
                return ToDownloadedSolutionDetails(ti);

            }
            catch (Exception)
            {
            }
            return null;
        }

        public static Licensing.DownloadedSolutionDetails ToDownloadedSolutionDetails(TitleInfoServiceModel tism)
        {
            return new Licensing.DownloadedSolutionDetails()
            {
                ZipFileLocation = tism.LatestTitleDownloadPath,
                ChangeLog = tism.LatestTitleChangelog,
                Version = tism.LatestTitleVersion
            };
        }

        public enum SortMode
        {
            Invalid, Length, Score, Alphabet
        }

        public SortMode CurrentSortMode = SortMode.Score;
        public bool SortModeDesc = true;

        public void ChangeSortMode(SortMode sm)
        {
            if (sm == CurrentSortMode)
            {
                SortModeDesc = !SortModeDesc;
                return;
            }

            SortModeDesc = true;
            CurrentSortMode = sm;
        }

        /// <summary>
        /// get results and order based on search
        /// </summary>
        /// <returns></returns>
        private void ApplySort()
        {
            switch (CurrentSortMode)
            {
                case SortMode.Invalid:
                    return;
                case SortMode.Length:
                    if (SortModeDesc)
                        results = results.OrderByDescending(s => s.Word.Length).ToList();
                    else
                        results = results.OrderBy(s => s.Word.Length).ToList();
                    break;

                case SortMode.Score:
                    if (SortModeDesc)
                        results = results.OrderByDescending(s => s.WordScore).ToList();
                    else
                        results = results.OrderBy(s => s.WordScore).ToList();
                    break;

                case SortMode.Alphabet:
                    if (SortModeDesc)
                        results = results.OrderByDescending(s => s.Word).ToList();
                    else
                        results = results.OrderBy(s => s.Word).ToList();
                    break;

                default:
                    return;
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ResizeWindow()
        {
            int w = MinimumSize.Width;
            int h = MinimumSize.Height;

            var lastTb = Grid.GetLastTextbox();
            int lx = lastTb.Location.X;
            lx += 340;

            int ly = Grid.GetLastTextbox().Location.Y;
            ly += 380;

            if (lx > w)
                w = lx;

            if (ly > h)
                h = ly;

            Size = new Size(w, h);
        }

        private void InitGrid(int width, int height)
        {
            Grid.InitGrid(width, height);
            ResizeWindow();
        }

        private void InitGrid(string[] rows)
        {
            Grid.InitGrid(rows);
            ResizeWindow();
        }

        private void ApplySolve()
        {
            if (string.IsNullOrEmpty(manualentry.Text) == false)
            {
                var op = GetFindMode();
                Grid.FindAndHighlight(manualentry.Text, op);
            }
            else if (foundwordsLB.SelectedItems.Count == 1 && foundwordsLB.SelectedItems[0].Tag is Grid.FoundWord)
            {
                var sel = (Grid.FoundWord)foundwordsLB.SelectedItems[0].Tag;

                Grid.FindAndHighlight(sel);
            }
        }

        private List<Grid.FoundWord> results = null;
        private void Solve(bool refreshResults)
        {
            if (results == null || refreshResults)
                results = Grid.Solve(GetFindMode(), bestonlyCB.Checked, top100onlyCB.Checked ? 100 : -1);

            foundwordsLB.Items.Clear();

            ApplySort();

            foreach (var r in results)
            {
                var lvi = new ListViewItem();
                lvi.Text = r.Word;
                lvi.Name = lvi.Text;
                lvi.SubItems.Add(r.WordScore.ToString(CultureInfo.InvariantCulture));
                lvi.Tag = r;
                foundwordsLB.Items.Add(lvi);
            }
        }

        public void LoadGridFromFile(String filename)
        {
            AllowLookupEvents = AllowTbChangeEvent = false;
            var fs = new FileStream(filename, FileMode.Open);
            var sr = new StreamReader(fs);
            var f = sr.ReadToEnd();
            sr.Close();
            fs.Close();

            var rows = f.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            InitGrid(rows);
            Solve(true);
            ApplySolve();
            AllowLookupEvents = AllowTbChangeEvent = true;
        }

        private void LoadFromImage(string fn)
        {
            AllowLookupEvents = AllowTbChangeEvent = false;
            Bitmap b;
            try
            {
                b = new Bitmap(fn);
                string[] rows = null;

                //crop a bit if wanted
                if (whenLoadingImageIgnoreTopBitOfImageToolStripMenuItem.Checked)
                {
                    var rect = new Rectangle(0, 100, b.Width, b.Height - 100);
                    b = b.Clone(rect, PixelFormat.DontCare);
                }

                b = BitmapExtras.OnlyAllowBlackAndColour(b, 0, 0, 0);

                if (tesseractOCRToolStripMenuItem.Checked)
                    rows = OCR.LoadImage(b).Item3;
                else
                {
                    if (File.Exists("hist.hist") == false)
                    {
                        MessageBox.Show(
                            "error, histogram file 'hist.hist' does not exist.\r\nYou can generate manually via the histogram OCR trainer");
                        return;
                    }
                    var h = HistogramOCR.DeSerialise("hist.hist");
                    rows = h.PerformOCR(b, 100);
                }

                InitGrid(rows);
            }
            catch (Exception ex)
            {
                MessageBox.Show("error:" + ex);
                AllowLookupEvents = AllowTbChangeEvent = true;
                return;
            }

            Solve(true);
            ApplySolve();
            AllowLookupEvents = AllowTbChangeEvent = true;
        }


        private Grid.FindMode GetFindMode()
        {
            if (crosswordRB.Checked)
                return Grid.FindMode.Crossword;
            if (lastletterllRB.Checked)
                return Grid.FindMode.AllDirFromLast;
            if (anywhereRB.Checked)
                return Grid.FindMode.Anywhere;

            return Grid.FindMode.NotSet;
        }

        private void loadwordgrid_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.InitialDirectory = Environment.CurrentDirectory;
            ofd.Filter = "|*.txt";
            var res = ofd.ShowDialog();
            if (res != DialogResult.OK)
                return;

            LoadGridFromFile(ofd.FileName);
        }

        private void limitToTop100ResultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            top100onlyCB.Checked = !top100onlyCB.Checked;
            Solve(true);
            ApplySolve();
        }

        #region search type
        private void crosswordRB_CheckedChanged(object sender, EventArgs e)
        {
            Solve(top100onlyCB.Checked);
            ApplySolve();
        }

        private void lastletterllRB_CheckedChanged(object sender, EventArgs e)
        {
            Solve(top100onlyCB.Checked);
            ApplySolve();
        }

        private void anywhereRB_CheckedChanged(object sender, EventArgs e)
        {
            Solve(top100onlyCB.Checked);
            ApplySolve();
        }

        #endregion search type

        #region sort type
        private void sortlengthbutton_Click(object sender, EventArgs e)
        {
            ChangeSortMode(SortMode.Length);
            Solve(top100onlyCB.Checked);
            ApplySolve();
        }

        private void sortscorebutton_Click(object sender, EventArgs e)
        {
            ChangeSortMode(SortMode.Score);
            Solve(false);
            ApplySolve();
        }

        private void sortnamebutton_Click(object sender, EventArgs e)
        {
            ChangeSortMode(SortMode.Alphabet);
            Solve(false);
            ApplySolve();
        }

        #endregion sort type

        private void savebutton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Choose a file name for the new grid";
            sfd.Filter = "|*.txt";
            sfd.InitialDirectory = Environment.CurrentDirectory;
            var res = sfd.ShowDialog();
            if (res != DialogResult.OK)
                return;

            Grid.SaveGridToFile(sfd.FileName);
        }

        private void clearbutton_Click(object sender, EventArgs e)
        {
            Grid.Clear();
            foundwordsLB.Items.Clear();
        }

        private void createbutton_Click(object sender, EventArgs e)
        {
            try
            {
                int w = int.Parse(createwidthTB.Text);
                int h = int.Parse(createheightTB.Text);

                InitGrid(w, h);
            }
            catch (Exception ex)
            {
                MessageBox.Show("error creating grid:" + ex);
            }
            return;
        }

        private void createwidthTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = TextboxExtras.HandleInput(TextboxExtras.InputType.Create(false, true), e.KeyChar, createwidthTB);
        }

        private void createheightTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = TextboxExtras.HandleInput(TextboxExtras.InputType.Create(false, true), e.KeyChar, createheightTB);
        }

        private void randomlettersbutton_Click(object sender, EventArgs e)
        {
            Grid.Clear();
            Grid.RandomiseLetters();
            Solve(true);
            ApplySolve();
        }

        private void loadfromimageB_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.InitialDirectory = Environment.CurrentDirectory;
            ofd.Filter = "|*.*";
            ofd.Title = "choose file to load letters from";
            var res = ofd.ShowDialog();
            if (res != DialogResult.OK)
                return;

            LoadFromImage(ofd.FileName);
        }

        private void histogramOCRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tesseractOCRToolStripMenuItem.Checked = histogramOCRToolStripMenuItem.Checked;
            histogramOCRToolStripMenuItem.Checked = !histogramOCRToolStripMenuItem.Checked;
        }

        private void tesseractOCRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            histogramOCRToolStripMenuItem.Checked = tesseractOCRToolStripMenuItem.Checked;
            tesseractOCRToolStripMenuItem.Checked = !tesseractOCRToolStripMenuItem.Checked;
        }

        private void foundwordsLB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AllowLookupEvents)
            {
                AllowLookupEvents = false;

                if (foundwordsLB.SelectedItems.Count == 1 && foundwordsLB.SelectedItems[0].Tag is Grid.FoundWord)
                {
                    manualentry.Text = "";
                    ApplySolve();
                }
                AllowLookupEvents = true;
            }
        }

        private void manualentry_TextChanged(object sender, EventArgs e)
        {
            if (AllowLookupEvents)
            {
                AllowLookupEvents = false;
                foundwordsLB.SelectedItems.Clear();
                var mk = manualentry.Text.ToUpper();
                var i = foundwordsLB.Items.IndexOfKey(mk);
                if (i != -1)
                {
                    foundwordsLB.Items[i].Selected = true;
                    foundwordsLB.EnsureVisible(i);
                }
                ApplySolve();
                AllowLookupEvents = true;
            }
        }

        public void GridTBChangeEvent(object sender, EventArgs e)
        {
            if (AllowTbChangeEvent)
            {
                AllowTbChangeEvent = false;
                var tb = sender as TextBox;
                Grid.SetGridPointValue(tb, tb.Text, false);

                Solve(true);
                ApplySolve();
                AllowTbChangeEvent = true;
            }
        }

        public void changeMultiplierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tb = (TextBox)ContextMenuStripExtras.GetContextParent(sender, typeof(TextBox));

            var gp = Grid.GetPointFromTextBox(tb);
            if (gp == null)
            {
                MessageBox.Show("error setting multiplier value");
            }
            if (gp.C == 0)
                return;

            var items = new List<MassVariableEdit.TextBoxItems>
                            {
                                new MassVariableEdit.TextBoxItems("Enter a letter multiplier for this grid square", "1",
                                                                  KeypressEvent, AcceptFinalTextBoxTextH,"Must be >=0 and a number"),
                                new MassVariableEdit.TextBoxItems("Enter an extra word bonus multiplier. eg 1=2x score",
                                    "0",KeypressEvent, AcceptFinalTextBoxTextH, "Must be >=0 and a number")
                            };

            var mve = new MassVariableEdit();
            var res = mve.ShowDialog("Enter the values", items);
            if (res == null)
                return;

            var lm1 = res[0].Item2;
            int lm11;
            int.TryParse(lm1, out lm11);

            var wm1 = res[1].Item2;
            int wm11;
            int.TryParse(wm1, out wm11);

            gp.LetterMultiplier = lm11;
            gp.WordMultiplierExtra = wm11;
            Solve(true);

            //set font
            if (lm11 != 1 || wm11 != 0)
                tb.Font = new Font(tb.Font.FontFamily, tb.Font.Size, FontStyle.Bold);
            else
                tb.Font = new Font(tb.Font.FontFamily, tb.Font.Size, FontStyle.Regular);

        }

        private bool AcceptFinalTextBoxTextH(string s)
        {
            try
            {
                var i = int.Parse(s);
                if (i < 0)
                    return false;
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        private bool KeypressEvent(char keyChar, Control control)
        {
            return TextboxExtras.HandleInput(TextboxExtras.InputType.Create(false, true), keyChar, control);
        }

        private void GridLetterContext_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var tb = (TextBox)ContextMenuStripExtras.GetContextParent(sender, typeof(TextBox));

            var gp = Grid.GetPointFromTextBox(tb);
            if (gp == null)
            {
                MessageBox.Show("error setting multiplier value");
            }
            if (gp.C == 0)
                e.Cancel = true;
        }

        private void onlyReturnTheHighestScoringOfDuplicateWordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bestonlyCB.Checked =
                !bestonlyCB.Checked;

            Solve(true);
            ApplySolve();
        }
    }
}
