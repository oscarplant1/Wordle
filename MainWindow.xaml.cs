using System;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Xceed;

namespace Wordle
{
    public partial class MainWindow : Window
    {
        static Random rnd = new Random();

        //Lists
        internal List<string> WordsToFind = new List<string>();
        internal List<string> CombinedWordlist = new List<string>();
        internal List<string> GuessedWords = new List<string>();
        internal List<Button> KeyboardButtons = new List<Button>();

        //Colors
        internal string Green = "#6aaa64";
        internal string DarkGrey = "#787c7e";
        internal string LightGrey = "#d3d6da";
        internal string Yellow = "#c9b458";

        internal bool GameOver = false;

        internal string[] GuessLetters = ["","","","",""];

        internal int CurrentRow = 0;

        internal string Target;

        public MainWindow()
        {
            InitializeComponent();

            //Reading in Word lists from text files
            StreamReader sr1 = new StreamReader("shuffled_real_wordles.txt");
            string line1 = sr1.ReadLine();
            line1 = sr1.ReadLine();
            while (line1 != null)
            {
                WordsToFind.Add(line1);
                line1 = sr1.ReadLine();
            }
            sr1.Close();

            StreamReader sr2 = new StreamReader("combined_wordlist.txt");
            string line2 = sr2.ReadLine();
            line2 = sr2.ReadLine();
            while (line2 != null)
            {
                CombinedWordlist.Add(line2);
                line2 = sr2.ReadLine();
            }
            sr2.Close();

            Loaded += TestWindow_Loaded;

            //Adding Grid to display
            AddSquareOutlines();

            //Selecting word to be guessed at random
            int r = rnd.Next(WordsToFind.Count);

            Target = WordsToFind[r];

            //Adding preexisting buttons to a list
            KeyboardButtons.Add(QButton);
            KeyboardButtons.Add(WButton);
            KeyboardButtons.Add(EButton);
            KeyboardButtons.Add(RButton);
            KeyboardButtons.Add(TButton);
            KeyboardButtons.Add(YButton);
            KeyboardButtons.Add(UButton);
            KeyboardButtons.Add(IButton);
            KeyboardButtons.Add(OButton);
            KeyboardButtons.Add(PButton); 
            KeyboardButtons.Add(AButton);
            KeyboardButtons.Add(SButton);
            KeyboardButtons.Add(DButton);
            KeyboardButtons.Add(FButton); 
            KeyboardButtons.Add(GButton);
            KeyboardButtons.Add(HButton);
            KeyboardButtons.Add(JButton);
            KeyboardButtons.Add(KButton);
            KeyboardButtons.Add(LButton);
            KeyboardButtons.Add(ZButton);
            KeyboardButtons.Add(XButton);
            KeyboardButtons.Add(CButton);
            KeyboardButtons.Add(VButton);
            KeyboardButtons.Add(BButton);
            KeyboardButtons.Add(NButton);
            KeyboardButtons.Add(MButton);
        }

        private void TestWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox1.Focus();
        }

        //Converts a Hex string to a SolidColorBrush object
        public SolidColorBrush GetColorFromHexa(string hexaColor)
        {
            byte R = Convert.ToByte(hexaColor.Substring(1, 2), 16);
            byte G = Convert.ToByte(hexaColor.Substring(3, 2), 16);
            byte B = Convert.ToByte(hexaColor.Substring(5, 2), 16);
            SolidColorBrush scb = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, R, G, B));
            return scb;
        }

        //Adds outlines to each grid square
        public void AddSquareOutlines()
        {
            for (int i = 0; i <= 5; i++)
            {
                for (int j = 0; j <= 6; j++)
                {
                    Border border = new Border();
                    border.Width = 60;
                    border.Height = 60;
                    border.BorderThickness = new Thickness(1);
                    border.BorderBrush = new SolidColorBrush(Colors.Gray);
                    border.SetValue(Grid.ColumnProperty, i);
                    border.SetValue(Grid.RowProperty, j);

                    Squares.Children.Add(border);
                }
            }
        }

        //Changes the color of a given key on the on screen keyboard
        public void ChangeKeyColour(char Key, string HexColor)
        {
            SolidColorBrush scb = GetColorFromHexa(HexColor);

            string StringKey = Key.ToString().ToUpper();
            char UpperKey = StringKey.ToCharArray()[0];

            string letters = "QWERTYUIOPASDFGHJKLZXCVBNM";

            for (int i = 0; i < 26; i++)
            {
                if (UpperKey == letters[i])
                {
                    KeyboardButtons[i].Background = scb;
                    KeyboardButtons[i].Foreground = new SolidColorBrush(Colors.White);
                }
            }
        }

        //Types a letter on the screen at the input column index
        public void SendToBoard(string LetterToAdd, int index)
        {
            StackPanel stack = new StackPanel();
            Border border = new Border();
            TextBlock text = new TextBlock();

            border.BorderThickness = new Thickness(1);
            border.BorderBrush = new SolidColorBrush(Colors.Black);

            text.Padding = new Thickness(13);
            text.TextAlignment = TextAlignment.Center;
            text.Text = LetterToAdd;
            text.FontSize = 32;
            text.HorizontalAlignment = HorizontalAlignment.Center;
            text.VerticalAlignment = VerticalAlignment.Center;
            text.Height = 60;
            text.Width = 60;
            text.FontWeight = FontWeights.SemiBold;
            text.FontFamily = new FontFamily("Clear Sans");

            stack.Height = 62;
            stack.Width = 62;
            stack.HorizontalAlignment = HorizontalAlignment.Center;
            stack.VerticalAlignment = VerticalAlignment.Center;
            stack.SetValue(Grid.RowProperty, CurrentRow);
            stack.SetValue(Grid.ColumnProperty, index);

            border.Child = text;
            stack.Children.Add(border);
            Squares.Children.Add(stack);
        }

        //Converts a keyboard press to its letter
        private char Translatekey(char X)
        {
            char[] AcceptedKeys = ['<', 'B', '0', '=', '?', 'D', '@', '4', ':', ';', ',', '>', '/', '1', '2', '3', '5', '6', '7', 'E', 'C', '.', 'A', '-', '9', '8'];

            if (AcceptedKeys.Contains(X))
            {
                switch (X)
                {
                    case ('<'):
                        X = 'Q';
                        break;
                    case ('B'):
                        X = 'W';
                        break;
                    case ('0'):
                        X = 'E';
                        break;
                    case ('='):
                        X = 'R';
                        break;
                    case ('?'):
                        X = 'T';
                        break;
                    case ('D'):
                        X = 'Y';
                        break;
                    case ('@'):
                        X = 'U';
                        break;
                    case ('4'):
                        X = 'I';
                        break;
                    case (':'):
                        X = 'O';
                        break;
                    case (';'):
                        X = 'P';
                        break;
                    case (','):
                        X = 'A';
                        break;
                    case ('>'):
                        X = 'S';
                        break;
                    case ('/'):
                        X = 'D';
                        break;
                    case ('1'):
                        X = 'F';
                        break;
                    case ('2'):
                        X = 'G';
                        break;
                    case ('3'):
                        X = 'H';
                        break;
                    case ('5'):
                        X = 'J';
                        break;
                    case ('6'):
                        X = 'K';
                        break;
                    case ('7'):
                        X = 'L';
                        break;
                    case ('E'):
                        X = 'Z';
                        break;
                    case ('C'):
                        X = 'X';
                        break;
                    case ('.'):
                        X = 'C';
                        break;
                    case ('A'):
                        X = 'V';
                        break;
                    case ('-'):
                        X = 'B';
                        break;
                    case ('9'):
                        X = 'N';
                        break;
                    case ('8'):
                        X = 'M';
                        break;
                }

                return X;
            }
            else
            {
                return '_';
            }
        }

        //Checks if a guess is valid and either moves onto the next row, outputs a win message or displays a "Not in Word List" message
        private void SubmitGuess()
        {
            if(!GameOver)
            { 
                string Guess = (GuessLetters[0] + GuessLetters[1] + GuessLetters[2] + GuessLetters[3] + GuessLetters[4]).ToLower();
                if (CombinedWordlist.Contains(Guess))
                {
                    GuessedWords.Add(Guess);
                    LoadGuessedWords();
                    CurrentRow++;
                    TextBox1.Focus();
                    GuessLetters = ["", "", "", "", ""];
                }
                else
                {
                    NotInWordList();
                }

                //Win
                if (Guess.ToLower() == Target)
                {
                    OutcomeDisplay.Visibility = Visibility.Visible;
                    OutcomeDisplayText.Content = "Well Done!";
                    GameOver = true;
                }
                //Lose - reveal word
                else if (CurrentRow == 6)
                {
                    OutcomeDisplay.Visibility = Visibility.Visible;
                    OutcomeDisplayText.Content = Target.ToUpper();
                    GameOver = true;
                }
            }
        }

        //Displays a message saying "Not in Word list" for 1.5 seconds
        private async Task NotInWordList()
        {
            OutcomeDisplayText.Width = 200;
            OutcomeDisplay.Width = 200;
            OutcomeDisplayText.Content = "Not in Word List";
            OutcomeDisplay.Visibility = Visibility.Visible;

            await Task.Delay(1500);

            OutcomeDisplayText.Content = "";
            OutcomeDisplay.Visibility = Visibility.Hidden;
        }

        //Displays all previously guessed words on the screen, colored accordingly
        private void LoadGuessedWords()
        {
            int count = 0;
            foreach (string Guess in GuessedWords)
            {
                for (int i = 0; i < 5; i++)
                {
                    List<char> CorrectChars = new List<char>();

                    char LetterToAdd = Guess[i];
                    StackPanel stack = new StackPanel();
                    Border border = new Border();
                    TextBlock text = new TextBlock();

                    int charsInGuess = Guess.Length - Guess.Replace(LetterToAdd.ToString(), "").Length;
                    int charsInTarget = Target.Length - Target.Replace(LetterToAdd.ToString(), "").Length;

                    text.Foreground = new SolidColorBrush(Colors.White);
                    text.Padding = new Thickness(13);
                    text.TextAlignment = TextAlignment.Center;
                    text.Text = LetterToAdd.ToString();
                    text.FontSize = 32;
                    text.HorizontalAlignment = HorizontalAlignment.Center;
                    text.VerticalAlignment = VerticalAlignment.Center;
                    text.Height = 60;
                    text.Width = 60;
                    text.FontWeight = FontWeights.SemiBold;
                    text.FontFamily = new FontFamily("Clear Sans");

                    if (Target.Contains(LetterToAdd))
                    {
                        if (Target[i] == LetterToAdd)
                        {
                            stack.Background = GetColorFromHexa(Green);
                            ChangeKeyColour(LetterToAdd,Green);
                            CorrectChars.Add(LetterToAdd);
                        }
                        else
                        {
                            if (charsInGuess <= charsInTarget)
                            {
                                stack.Background = GetColorFromHexa(Yellow);

                                int CorrectCharsInGuess = 0;
                                for (int j = 0; j < 5; j++)
                                {
                                    if ((LetterToAdd == Target[j]) & (LetterToAdd == Guess[j]))
                                    {
                                        CorrectCharsInGuess++;
                                    }
                                }

                                if (CorrectCharsInGuess > 0)
                                {
                                    ChangeKeyColour(LetterToAdd, Green);
                                }
                                else
                                {
                                    ChangeKeyColour(LetterToAdd, Yellow);
                                }
                            }
                            else
                            {
                                int CorrectCharsInGuess = 0;
                                int PreviousLettersCount = 0;
                                for (int j = 0; j < 5; j++)
                                {
                                    if ((LetterToAdd == Target[j]) & (LetterToAdd == Guess[j])) 
                                    {
                                        CorrectCharsInGuess++;
                                    }
                                    else if (LetterToAdd == Guess[j] & j<i)
                                    {
                                        PreviousLettersCount++;
                                    }
                                }

                                int NumberofYellows = charsInTarget - CorrectCharsInGuess;

                                if (NumberofYellows > PreviousLettersCount)
                                {
                                    stack.Background = GetColorFromHexa(Yellow);
                                    if (CorrectCharsInGuess > 0)
                                    {
                                        ChangeKeyColour(LetterToAdd, Green);
                                    }
                                    else
                                    {
                                        ChangeKeyColour(LetterToAdd, Yellow);
                                    }
                                }
                                else
                                {
                                    stack.Background = GetColorFromHexa(DarkGrey);
                                    ChangeKeyColour(LetterToAdd, DarkGrey);
                                }

                            }
                        }
                    }
                    else
                    {
                        stack.Background = GetColorFromHexa(DarkGrey);
                        ChangeKeyColour(LetterToAdd, DarkGrey);

                    }

                    text.Text = LetterToAdd.ToString().ToUpper();
                    stack.Height = 62;
                    stack.Width = 62;
                    stack.HorizontalAlignment = HorizontalAlignment.Center;
                    stack.VerticalAlignment = VerticalAlignment.Center;
                    stack.SetValue(Grid.RowProperty, count);
                    stack.SetValue(Grid.ColumnProperty, i);

                    stack.Children.Add(text);
                    Squares.Children.Add(stack);

                    TextBox1.Focus();
                }
                count++;
            }
        }

        //Adds a letter to the current guess list unless the lit is full, then updates the screen accordingly
        private void AddGuessLetter(string letter)
        {
            if (!GameOver)
            {
                if (GuessLetters[4] == "")
                {
                    int index = 0;
                    while (GuessLetters[index] != "")
                    {
                        index++;
                    }
                    GuessLetters[index] = letter;
                    SendToBoard(letter, index);
                }
                TextBox1.Focus();
            }
        }

        //Removes the most recent letter from the current guess list unless the list is empty, then updates the screen accordingly
        private void RemoveLatestGuessLetter()
        {
            if (!GameOver)
            {
                int index = -1;

                for (int i = 0; i < 4; i++)
                {
                    if (GuessLetters[i] != "" & GuessLetters[i + 1] == "")
                    {
                        index = i;
                    }
                }
                if (GuessLetters[4] != "")
                {
                    index = 4;
                }

                if (index >= 0)
                {
                    GuessLetters[index] = "";
                }

                Squares.Children.Clear();
                AddSquareOutlines();
                for (int i = 0; i < index; i++)
                {
                    SendToBoard(GuessLetters[i], i);
                }
                LoadGuessedWords();
            }
        }

        //Handles keyboard inputs - hidden textbox on screen only allows letter entries
        private void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //Delete key or back key pressed ---> Remove a letter
            if ((e.Key == Key.Delete) || (e.Key == Key.Back))
            {
                RemoveLatestGuessLetter();
            }
            //Enter key pressed ---> Submit guess
            else if (e.Key == Key.Enter)
            {
                if (GuessLetters[4] != "")
                {
                    SubmitGuess();
                }
            }
            //Any other key ---> Add the corresponding letter to the guess
            else
            {
                char LastKey = (Char)e.Key;

                if (Translatekey(LastKey) != '_')
                {
                    LastKey = Translatekey(LastKey);
                    AddGuessLetter(LastKey.ToString());
                }
            }
        }


        //On Screen Button Click events

        //Enter Button ---> Submit Guess if guess is 5 letters long
        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            if (GuessLetters[4] != "")
            {
                SubmitGuess();
            }
        }

        //Back Button ---> Remove last letter
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveLatestGuessLetter();
        }

        //New Word ---> Generate a new random target letter, reset attributes and reset screen
        private void NewWordButton_Click(object sender, RoutedEventArgs e)
        {
            string oldTarget = Target;

            while (oldTarget == Target)
            {
                int r = rnd.Next(WordsToFind.Count);
                Target = WordsToFind[r];
            }

            GuessLetters = ["", "", "", "", ""];
            GuessedWords = new List<string>();
            CurrentRow = 0;
            GameOver = false;

            Squares.Children.Clear();
            OutcomeDisplay.Visibility = Visibility.Hidden;
            OutcomeDisplayText.Content = "";
            AddSquareOutlines();

            foreach (Button button in KeyboardButtons)
            {
                button.Background = GetColorFromHexa(LightGrey);
                button.Foreground = new SolidColorBrush(Colors.Black);
            }

            TextBox1.Focus();
        }


        //All Keyboard keys ---> Add corresponding letter
        private void QButton_Click(object sender, RoutedEventArgs e)
        {
            AddGuessLetter("Q");
        }

        private void WButton_Click(object sender, RoutedEventArgs e)
        {
            AddGuessLetter("W");
        }

        private void EButton_Click(object sender, RoutedEventArgs e)
        {
            AddGuessLetter("E");
        }

        private void RButton_Click(object sender, RoutedEventArgs e)
        {
            AddGuessLetter("R");
        }

        private void TButton_Click(object sender, RoutedEventArgs e)
        {
            AddGuessLetter("T");
        }

        private void YButton_Click(object sender, RoutedEventArgs e)
        {
            AddGuessLetter("Y");
        }

        private void UButton_Click(object sender, RoutedEventArgs e)
        {
            AddGuessLetter("U");
        }

        private void IButton_Click(object sender, RoutedEventArgs e)
        {
            AddGuessLetter("I");
        }

        private void OButton_Click(object sender, RoutedEventArgs e)
        {
            AddGuessLetter("O");
        }

        private void PButton_Click(object sender, RoutedEventArgs e)
        {
            AddGuessLetter("P");
        }
        private void AButton_Click(object sender, RoutedEventArgs e)
        {
            AddGuessLetter("A");
        }

        private void SButton_Click(object sender, RoutedEventArgs e)
        {
            AddGuessLetter("S");
        }

        private void DButton_Click(object sender, RoutedEventArgs e)
        {
            AddGuessLetter("D");
        }
        private void FButton_Click(object sender, RoutedEventArgs e)
        {
            AddGuessLetter("F");
        }

        private void GButton_Click(object sender, RoutedEventArgs e)
        {
            AddGuessLetter("G");
        }

        private void HButton_Click(object sender, RoutedEventArgs e)
        {
            AddGuessLetter("H");
        }

        private void JButton_Click(object sender, RoutedEventArgs e)
        {
            AddGuessLetter("J");
        }

        private void KButton_Click(object sender, RoutedEventArgs e)
        {
            AddGuessLetter("K");
        }

        private void LButton_Click(object sender, RoutedEventArgs e)
        {
            AddGuessLetter("L");
        }

        private void ZButton_Click(object sender, RoutedEventArgs e)
        {
            AddGuessLetter("Z");
        }

        private void XButton_Click(object sender, RoutedEventArgs e)
        {
            AddGuessLetter("X");
        }

        private void CButton_Click(object sender, RoutedEventArgs e)
        {
            AddGuessLetter("C");
        }

        private void VButton_Click(object sender, RoutedEventArgs e)
        {
            AddGuessLetter("V");
        }

        private void BButton_Click(object sender, RoutedEventArgs e)
        {
            AddGuessLetter("B");
        }

        private void NButton_Click(object sender, RoutedEventArgs e)
        {
            AddGuessLetter("N");
        }

        private void MButton_Click(object sender, RoutedEventArgs e)
        {
            AddGuessLetter("M");
        }
    }
}