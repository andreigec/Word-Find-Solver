using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ANDREICSLIB;

namespace Word_Find_Solver
{
    public partial class Form1 : Form
    {
        #region licensing

        private const string AppTitle = "Word Find Solver";
        private const double AppVersion = 0.2;
        private const String HelpString = "";

        private const String UpdatePath = "https://github.com/EvilSeven/Word-Find-Solver/zipball/master";
        private const String VersionPath = "https://raw.github.com/EvilSeven/Word-Find-Solver/master/INFO/version.txt";
        private const String ChangelogPath = "https://raw.github.com/EvilSeven/Word-Find-Solver/master/INFO/changelog.txt";

        private readonly String OtherText =
            @"©" + DateTime.Now.Year +
            @" Andrei Gec (http://www.andreigec.net)

Licensed under GNU LGPL (http://www.gnu.org/)

Zip Assets © SharpZipLib (http://www.sharpdevelop.net/OpenSource/SharpZipLib/)
";
        #endregion

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

            int lx = Grid.GetLastTextbox().Location.X;
            lx += 240;

            int ly = Grid.GetLastTextbox().Location.Y;
            ly += 280;

            if (lx > w)
                w = lx;

            if (ly > h)
                h = ly;

            Size = new Size(w, h);
        }

        private void InitGrid(int x, int y)
        {
            Grid.InitGrid(x, y);
            ResizeWindow();
        }

        private void InitGrid(string[] rows)
        {
            Grid.InitGrid(rows);
            ResizeWindow();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Licensing.CreateLicense(this, HelpString, AppTitle, AppVersion, OtherText, VersionPath, UpdatePath, ChangelogPath, menuStrip1);
            Grid.baseform = this;
            Grid.InitPanel(grid);
            Grid.InitWords();
            InitGrid(8, 8);
        }

        private void ApplySolve()
        {
            var op = GetFindMode();
            if (op == null)
                return;

            Grid.FindAndHighlight(manualentry.Text, (Grid.FindMode)op);
        }

        private void Solve()
        {
            var words = Grid.Solve(GetFindMode());
            foundwords.Items.Clear();
            foundwords.Items.AddRange(words.ToArray());
        }

        public void LoadGridFromFile(String filename, ref ListBox lb, Grid.FindMode fm)
        {
            var fs = new FileStream(filename, FileMode.Open);
            var sr = new StreamReader(fs);
            var f = sr.ReadToEnd();
            sr.Close();
            fs.Close();

            var rows = f.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            InitGrid(rows);
            Solve();
        }

        public void GridTBChangeEvent(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            tb.Text = tb.Text.ToUpper();
            Grid.ChangeTextBoxValue(tb);
            Solve();
            ApplySolve();
        }

        private void manualentry_TextChanged(object sender, EventArgs e)
        {
            foundwords.SelectedIndex = -1;
            if (foundwords.Items.Contains(manualentry.Text))
                foundwords.SelectedItem = manualentry.Text;

            ApplySolve();
        }

        private Grid.FindMode GetFindMode()
        {
            if (crosswordRB.Checked)
                return Grid.FindMode.Crossword;
            if (lastletterllRB.Checked)
                return Grid.FindMode.AllDirFromLast;

            return Grid.FindMode.NotSet;
        }

        private void crosswordRB_CheckedChanged(object sender, EventArgs e)
        {
            Solve();
            ApplySolve();
        }

        private void lastletterllRB_CheckedChanged(object sender, EventArgs e)
        {
            ApplySolve();
        }

        private void loadwordgrid_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.InitialDirectory = Environment.CurrentDirectory;
            ofd.Filter = "|*.txt";
            var res = ofd.ShowDialog();
            if (res != DialogResult.OK)
                return;

            LoadGridFromFile(ofd.FileName, ref foundwords, GetFindMode());
        }

        private void foundwords_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (foundwords.SelectedIndex == -1)
                return;

            String w = foundwords.Text;
            manualentry.Text = w;
            ApplySolve();
        }

        private void SortListBoxLength(ListBox lb)
        {
            var list = new List<String>(lb.Items.Cast<String>());
            list = new List<string>(list.OrderByDescending(s => s.Length));
            lb.Items.Clear();
            lb.Items.AddRange(list.ToArray<String>());
        }

        private void SortListBoxScore(ListBox lb)
        {
            var list = new List<String>(lb.Items.Cast<String>());
            list = new List<string>(list.OrderByDescending(Grid.GetWordScore));
            lb.Items.Clear();
            lb.Items.AddRange(list.ToArray<String>());
        }

        private void SortListBoxName(ListBox lb)
        {
            var list = new List<String>(lb.Items.Cast<String>());
            list = new List<string>(list.OrderBy(s => s));
            lb.Items.Clear();
            lb.Items.AddRange(list.ToArray<String>());
        }

        private void sortlengthbutton_Click(object sender, EventArgs e)
        {
            SortListBoxLength(foundwords);
        }

        private void sortscorebutton_Click(object sender, EventArgs e)
        {
            SortListBoxScore(foundwords);
        }

        private void savebutton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Choose a file name for the new grid";
            sfd.Filter = "|*.txt";
            var res = sfd.ShowDialog();
            if (res != DialogResult.OK)
                return;

            Grid.SaveGridToFile(sfd.FileName);
        }

        private void clearbutton_Click(object sender, EventArgs e)
        {
            Grid.Clear();
            foundwords.Items.Clear();
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
            e.Handled = TextboxUpdates.HandleInput(TextboxUpdates.InputType.Create(false,true), e.KeyChar,createwidthTB);
        }

        private void createheightTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = TextboxUpdates.HandleInput(TextboxUpdates.InputType.Create(false, true), e.KeyChar, createheightTB);
        }

        private void sortnamebutton_Click(object sender, EventArgs e)
        {
            SortListBoxName(foundwords);
        }

        private void randomlettersbutton_Click(object sender, EventArgs e)
        {
            Grid.Clear();
            Grid.RandomiseLetters(GetFindMode());
            Solve();
            ApplySolve();
        }
    }
}
