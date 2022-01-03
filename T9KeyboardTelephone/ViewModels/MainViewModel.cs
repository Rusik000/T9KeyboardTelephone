using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace T9KeyboardTelephone.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string _buttonName;
        private int _clickCount;
        private bool _saveText;
        private bool _deleteCharacterState;
        private List<string> _readyWords;
        private List<string> _willBeAdded;
        //Task Thread
        private static object _obj;
        private int _counter;


        public MainWindow T9Keyboard { get; set; }

        public MainViewModel()
        {
            _buttonName = string.Empty;
            _clickCount = 0;
            _saveText = false;
            _deleteCharacterState = false;
            _readyWords = new List<string>();
            _willBeAdded = new List<string>();
            _obj = new object();

            _readyWords.Add("Muellim");
            _readyWords.Add("Ev");
            _readyWords.Add("Masin");
            _readyWords.Add("Meyve");
            _readyWords.Add("Banan");
            _readyWords.Add("Cahil");
            _readyWords.Add("Davidov");
            _readyWords.Add("Elsever");
            _readyWords.Add("Fariz");
            _readyWords.Add("Hakim");

            Thread setAllData = new Thread(() =>
            {
                T9Keyboard.PreviewKeyUp += T9Keyboard_KeyboardWithButtonsControl;
                T9Keyboard.Advice.SelectionChanged += AdviceOnSelectionChanged;
                T9Keyboard.MainRichTextBox.TextChanged += MainRichTextBoxOnTextChanged;

                T9Keyboard.Dispatcher.BeginInvoke(new Action(() =>
                {
                    T9Keyboard.Advice.SelectionMode = SelectionMode.Single;
                    T9Keyboard.AddWord.IsEnabled = false;
                    T9Keyboard.UpArrow.IsEnabled = false;
                    T9Keyboard.DownArrow.IsEnabled = false;
                    T9Keyboard.RightArrow.IsEnabled = false;
                    T9Keyboard.LastCharacterDeleter.IsEnabled = false;
                }));

                T9Keyboard.LastCharacterDeleter.Click += ButtonsClick_AddCharacter;
                T9Keyboard.Clear.Click += ButtonsClick_AddCharacter;
                T9Keyboard.AddWord.Click += ButtonsClick_AddCharacter;
                T9Keyboard.One.Click += ButtonsClick_AddCharacter;
                T9Keyboard.Two.Click += ButtonsClick_AddCharacter;
                T9Keyboard.Three.Click += ButtonsClick_AddCharacter;
                T9Keyboard.Four.Click += ButtonsClick_AddCharacter;
                T9Keyboard.Five.Click += ButtonsClick_AddCharacter;
                T9Keyboard.Six.Click += ButtonsClick_AddCharacter;
                T9Keyboard.Seven.Click += ButtonsClick_AddCharacter;
                T9Keyboard.Eight.Click += ButtonsClick_AddCharacter;
                T9Keyboard.Nine.Click += ButtonsClick_AddCharacter;
                T9Keyboard.Zero.Click += ButtonsClick_AddCharacter;
                T9Keyboard.Star.Click += ButtonsClick_AddCharacter;
                T9Keyboard.Hash.Click += ButtonsClick_AddCharacter;
                T9Keyboard.UpArrow.Click += UpArrowWork;
                T9Keyboard.DownArrow.Click += DownArrowWork;
                T9Keyboard.RightArrow.Click += RightArrowOnClick;

                T9Keyboard.One.MouseRightButtonUp += SaveCharacter_RightClick;
                T9Keyboard.Two.MouseRightButtonUp += SaveCharacter_RightClick;
                T9Keyboard.Three.MouseRightButtonUp += SaveCharacter_RightClick;
                T9Keyboard.Four.MouseRightButtonUp += SaveCharacter_RightClick;
                T9Keyboard.Five.MouseRightButtonUp += SaveCharacter_RightClick;
                T9Keyboard.Six.MouseRightButtonUp += SaveCharacter_RightClick;
                T9Keyboard.Seven.MouseRightButtonUp += SaveCharacter_RightClick;
                T9Keyboard.Eight.MouseRightButtonUp += SaveCharacter_RightClick;
                T9Keyboard.Nine.MouseRightButtonUp += SaveCharacter_RightClick;
                T9Keyboard.Zero.MouseRightButtonUp += SaveCharacter_RightClick;
                T9Keyboard.Star.MouseRightButtonUp += SaveCharacter_RightClick;
                T9Keyboard.Hash.MouseRightButtonUp += SaveCharacter_RightClick;

            });
            setAllData.Start();
        }

        private async void RightArrowOnClick(object sender, RoutedEventArgs e)
        {
            if (T9Keyboard.Advice.SelectedIndex >= 0 && T9Keyboard.Advice.SelectedItem != null)
            {
                SaveCharacter_RightClick(null, null);

                string text = new TextRange(T9Keyboard.MainRichTextBox.Document.ContentStart,
                    T9Keyboard.MainRichTextBox.Document.ContentEnd).Text;

                string[] allInfo = text.Split(' ');
                string value = value = T9Keyboard.Advice.SelectedItem as string;
                T9Keyboard.MainRichTextBox.Document.Blocks.Clear();

                for (int i = 0; i < allInfo.Length; i++)
                {
                    if (i == allInfo.Length - 1)
                        T9Keyboard.MainRichTextBox.AppendText(value + " ");
                    else
                        T9Keyboard.MainRichTextBox.AppendText(allInfo[i] + " ");
                }

                Task task = new Task(() => wordContains());
                task.Start();
                await task;
            }
        }

        private void MainRichTextBoxOnTextChanged(object sender, TextChangedEventArgs e)
        {
            string text = new TextRange(T9Keyboard.MainRichTextBox.Document.ContentStart, T9Keyboard.MainRichTextBox.Document.ContentEnd).Text;
            if (text.Length >= 3)
            {
                T9Keyboard.Dispatcher.BeginInvoke(new Action(() =>
                {
                    T9Keyboard.LastCharacterDeleter.IsEnabled = true;
                    T9Keyboard.Clear.IsEnabled = true;
                }));
            }
            else
            {
                T9Keyboard.Dispatcher.BeginInvoke(new Action(() =>
                {
                    T9Keyboard.LastCharacterDeleter.IsEnabled = false;
                    T9Keyboard.Clear.IsEnabled = false;
                }));
            }
        }

        private void AdviceOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (T9Keyboard.Advice.Items.Count > 0)
                T9Keyboard.DownArrow.IsEnabled = true;
            else T9Keyboard.DownArrow.IsEnabled = false;

            if (T9Keyboard.Advice.SelectedIndex >= 1)
                T9Keyboard.UpArrow.IsEnabled = true;
            else T9Keyboard.UpArrow.IsEnabled = false;

            if (T9Keyboard.Advice.Items.Count > 0)
                T9Keyboard.RightArrow.IsEnabled = true;
            else T9Keyboard.RightArrow.IsEnabled = false;
        }
        private void T9Keyboard_KeyboardWithButtonsControl(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
                DownArrowWork(null, null);
            else if (e.Key == Key.Up)
                UpArrowWork(null, null);
            else if (e.Key == Key.Right)
                RightArrowOnClick(null, null);
            else if (e.Key == Key.Left)
                ButtonsClick_AddCharacter(new Button() { Name = "LastCharacterDeleter" }, null);
            else if (e.Key == Key.Back)
                ButtonsClick_AddCharacter(new Button() { Name = "Clear" }, null);
        }

        private void UpArrowWork(object sender, RoutedEventArgs e)
        {
            if (T9Keyboard.Advice.Items.Count >= 0)
                if (T9Keyboard.Advice.Items.Count >= 1)
                    T9Keyboard.Advice.SelectedIndex--;
        }

        private void DownArrowWork(object sender, RoutedEventArgs e)
        {
            if (T9Keyboard.Advice.Items.Count >= 0)
                if (T9Keyboard.Advice.Items.Count >= 1)
                    T9Keyboard.Advice.SelectedIndex++;
        }

        private void SaveCharacter_RightClick(object sender, MouseButtonEventArgs e)
        {
            _buttonName = string.Empty;
            _clickCount = 0;
            _saveText = true;

            var range = new TextRange(T9Keyboard.MainRichTextBox.Document.ContentStart,
                T9Keyboard.MainRichTextBox.Document.ContentEnd);
            range.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.White);
            range.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);
        }

        private void ButtonsClick_AddCharacter(object sender, RoutedEventArgs e)
        {
            lock (_obj)
            {
                Button button = sender as Button;

                if (button.Name == "Clear")
                {
                    T9Keyboard.MainRichTextBox.Document.Blocks.Clear();
                    _saveText = false;
                    _deleteCharacterState = true;
                    _buttonName = string.Empty;
                    _clickCount = 0;
                }
                else if (button.Name == "LastCharacterDeleter")
                {
                    Remove1();
                    _saveText = false;
                    _deleteCharacterState = true;
                    _buttonName = string.Empty;
                    _clickCount = 0;
                }
                else if (button.Name == "AddWord")
                {
                    foreach (var word in _willBeAdded)
                        _readyWords.Add(word);

                    T9Keyboard.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        T9Keyboard.AddWord.IsEnabled = false;
                    }));

                    _willBeAdded.Clear();
                }
                else if (button.Name == "One")
                {
                    if (_buttonName != "One")
                    {
                        _buttonName = "One";
                        if (!_saveText)
                        {
                            if (!_deleteCharacterState)
                                Remove1();
                            else _deleteCharacterState = false;
                        }
                        else _saveText = false;

                        T9Keyboard.MainRichTextBox.AppendText("1");
                        _clickCount = 0;
                    }
                    else
                    {
                        if (!_deleteCharacterState)
                            Remove1();
                        else _deleteCharacterState = false;

                        if (_clickCount == 1)
                            T9Keyboard.MainRichTextBox.AppendText(" ");
                        else
                        {
                            _clickCount = 0;
                            T9Keyboard.MainRichTextBox.AppendText("1");
                        }
                    }
                }
                else if (button.Name == "Two")
                {
                    if (_buttonName != "Two")
                    {
                        _buttonName = "Two";
                        if (!_saveText)
                        {
                            if (!_deleteCharacterState)
                                Remove1();
                            else _deleteCharacterState = false;
                        }
                        else _saveText = false;

                        T9Keyboard.MainRichTextBox.AppendText("2");
                        _clickCount = 0;
                    }
                    else
                    {
                        if (!_deleteCharacterState)
                            Remove1();
                        else _deleteCharacterState = false;

                        if (_clickCount == 1)
                            T9Keyboard.MainRichTextBox.AppendText("A");
                        else if (_clickCount == 2)
                            T9Keyboard.MainRichTextBox.AppendText("B");
                        else if (_clickCount == 3)
                            T9Keyboard.MainRichTextBox.AppendText("C");
                        else
                        {
                            _clickCount = 0;
                            T9Keyboard.MainRichTextBox.AppendText("2");
                        }
                    }
                }
                else if (button.Name == "Three")
                {
                    if (_buttonName != "Three")
                    {
                        _buttonName = "Three";
                        if (!_saveText)
                        {
                            if (!_deleteCharacterState)
                                Remove1();
                            else _deleteCharacterState = false;
                        }
                        else _saveText = false;

                        T9Keyboard.MainRichTextBox.AppendText("3");
                        _clickCount = 0;
                    }
                    else
                    {
                        if (!_deleteCharacterState)
                            Remove1();
                        else _deleteCharacterState = false;

                        if (_clickCount == 1)
                            T9Keyboard.MainRichTextBox.AppendText("D");
                        else if (_clickCount == 2)
                            T9Keyboard.MainRichTextBox.AppendText("E");
                        else if (_clickCount == 3)
                            T9Keyboard.MainRichTextBox.AppendText("F");
                        else
                        {
                            _clickCount = 0;
                            T9Keyboard.MainRichTextBox.AppendText("3");
                        }
                    }
                }
                else if (button.Name == "Four")
                {
                    if (_buttonName != "Four")
                    {
                        _buttonName = "Four";
                        if (!_saveText)
                        {
                            if (!_deleteCharacterState)
                                Remove1();
                            else _deleteCharacterState = false;
                        }
                        else _saveText = false;

                        T9Keyboard.MainRichTextBox.AppendText("4");
                        _clickCount = 0;
                    }
                    else
                    {
                        if (!_deleteCharacterState)
                            Remove1();
                        else _deleteCharacterState = false;

                        if (_clickCount == 1)
                            T9Keyboard.MainRichTextBox.AppendText("G");
                        else if (_clickCount == 2)
                            T9Keyboard.MainRichTextBox.AppendText("H");
                        else if (_clickCount == 3)
                            T9Keyboard.MainRichTextBox.AppendText("I");
                        else
                        {
                            _clickCount = 0;
                            T9Keyboard.MainRichTextBox.AppendText("4");
                        }
                    }
                }
                else if (button.Name == "Five")
                {
                    if (_buttonName != "Five")
                    {
                        _buttonName = "Five";
                        if (!_saveText)
                        {
                            if (!_deleteCharacterState)
                                Remove1();
                            else _deleteCharacterState = false;
                        }
                        else _saveText = false;

                        T9Keyboard.MainRichTextBox.AppendText("5");
                        _clickCount = 0;
                    }
                    else
                    {
                        if (!_deleteCharacterState)
                            Remove1();
                        else _deleteCharacterState = false;

                        if (_clickCount == 1)
                            T9Keyboard.MainRichTextBox.AppendText("J");
                        else if (_clickCount == 2)
                            T9Keyboard.MainRichTextBox.AppendText("K");
                        else if (_clickCount == 3)
                            T9Keyboard.MainRichTextBox.AppendText("L");
                        else
                        {
                            _clickCount = 0;
                            T9Keyboard.MainRichTextBox.AppendText("5");
                        }
                    }
                }
                else if (button.Name == "Six")
                {
                    if (_buttonName != "Six")
                    {
                        _buttonName = "Six";
                        if (!_saveText)
                        {
                            if (!_deleteCharacterState)
                                Remove1();
                            else _deleteCharacterState = false;
                        }
                        else _saveText = false;

                        T9Keyboard.MainRichTextBox.AppendText("6");
                        _clickCount = 0;
                    }
                    else
                    {
                        if (!_deleteCharacterState)
                            Remove1();
                        else _deleteCharacterState = false;

                        if (_clickCount == 1)
                            T9Keyboard.MainRichTextBox.AppendText("M");
                        else if (_clickCount == 2)
                            T9Keyboard.MainRichTextBox.AppendText("N");
                        else if (_clickCount == 3)
                            T9Keyboard.MainRichTextBox.AppendText("O");
                        else
                        {
                            _clickCount = 0;
                            T9Keyboard.MainRichTextBox.AppendText("6");
                        }
                    }
                }
                else if (button.Name == "Seven")
                {
                    if (_buttonName != "Seven")
                    {
                        _buttonName = "Seven";
                        if (!_saveText)
                        {
                            if (!_deleteCharacterState)
                                Remove1();
                            else _deleteCharacterState = false;
                        }
                        else _saveText = false;

                        T9Keyboard.MainRichTextBox.AppendText("7");
                        _clickCount = 0;
                    }
                    else
                    {
                        if (!_deleteCharacterState)
                            Remove1();
                        else _deleteCharacterState = false;

                        if (_clickCount == 1)
                            T9Keyboard.MainRichTextBox.AppendText("P");
                        else if (_clickCount == 2)
                            T9Keyboard.MainRichTextBox.AppendText("Q");
                        else if (_clickCount == 3)
                            T9Keyboard.MainRichTextBox.AppendText("R");
                        else if (_clickCount == 4)
                            T9Keyboard.MainRichTextBox.AppendText("S");
                        else
                        {
                            _clickCount = 0;
                            T9Keyboard.MainRichTextBox.AppendText("7");
                        }
                    }
                }
                else if (button.Name == "Eight")
                {
                    if (_buttonName != "Eight")
                    {
                        _buttonName = "Eight";
                        if (!_saveText)
                        {
                            if (!_deleteCharacterState)
                                Remove1();
                            else _deleteCharacterState = false;
                        }
                        else _saveText = false;

                        T9Keyboard.MainRichTextBox.AppendText("8");
                        _clickCount = 0;
                    }
                    else
                    {
                        if (!_deleteCharacterState)
                            Remove1();
                        else _deleteCharacterState = false;

                        if (_clickCount == 1)
                            T9Keyboard.MainRichTextBox.AppendText("T");
                        else if (_clickCount == 2)
                            T9Keyboard.MainRichTextBox.AppendText("U");
                        else if (_clickCount == 3)
                            T9Keyboard.MainRichTextBox.AppendText("V");
                        else
                        {
                            _clickCount = 0;
                            T9Keyboard.MainRichTextBox.AppendText("8");
                        }
                    }
                }
                else if (button.Name == "Nine")
                {
                    if (_buttonName != "Nine")
                    {
                        _buttonName = "Nine";
                        if (!_saveText)
                        {
                            if (!_deleteCharacterState)
                                Remove1();
                            else _deleteCharacterState = false;
                        }
                        else _saveText = false;

                        T9Keyboard.MainRichTextBox.AppendText("9");
                        _clickCount = 0;
                    }
                    else
                    {
                        if (!_deleteCharacterState)
                            Remove1();
                        else _deleteCharacterState = false;

                        if (_clickCount == 1)
                            T9Keyboard.MainRichTextBox.AppendText("W");
                        else if (_clickCount == 2)
                            T9Keyboard.MainRichTextBox.AppendText("X");
                        else if (_clickCount == 3)
                            T9Keyboard.MainRichTextBox.AppendText("Y");
                        else if (_clickCount == 4)
                            T9Keyboard.MainRichTextBox.AppendText("Z");
                        else
                        {
                            _clickCount = 0;
                            T9Keyboard.MainRichTextBox.AppendText("9");
                        }
                    }
                }
                else if (button.Name == "Zero")
                {
                    if (_buttonName != "Zero")
                    {
                        _buttonName = "Zero";
                        if (!_saveText)
                        {
                            if (!_deleteCharacterState)
                                Remove1();
                            else _deleteCharacterState = false;
                        }
                        else _saveText = false;

                        T9Keyboard.MainRichTextBox.AppendText("0");
                        _clickCount = 0;
                    }
                    else
                    {
                        if (!_deleteCharacterState)
                            Remove1();
                        else _deleteCharacterState = false;

                        if (_clickCount == 1)
                            T9Keyboard.MainRichTextBox.AppendText("+");
                        else
                        {
                            _clickCount = 0;
                            T9Keyboard.MainRichTextBox.AppendText("0");
                        }
                    }
                }
                else if (button.Name == "Star")
                {
                    if (_buttonName != "Star")
                    {
                        _buttonName = "Star";
                        if (!_saveText)
                        {
                            if (!_deleteCharacterState)
                                Remove1();
                            else _deleteCharacterState = false;
                        }
                        else _saveText = false;

                        T9Keyboard.MainRichTextBox.AppendText("*");
                        _clickCount = 0;
                    }
                }
                else if (button.Name == "Hash")
                {
                    if (_buttonName != "Hash")
                    {
                        _buttonName = "Hash";
                        if (!_saveText)
                        {
                            if (!_deleteCharacterState)
                                Remove1();
                            else _deleteCharacterState = false;
                        }
                        else _saveText = false;

                        T9Keyboard.MainRichTextBox.AppendText("#");
                        _clickCount = 0;
                    }
                }

                LastCharacterSetColor();
            }
        }

        private void Remove1()
        {
            TextRange range = new TextRange(T9Keyboard.MainRichTextBox.Document.ContentStart,
                T9Keyboard.MainRichTextBox.Document.ContentEnd);

            string text = range.Text;

            if (range.Text.Length > 2)
            {
                text = text.Remove(text.Length - 1);
                text = text.Remove(text.Length - 1);
                text = text.Remove(text.Length - 1);
            }

            range.Text = text;
        }

        private async void LastCharacterSetColor()
        {
            //Check
            Task task = new Task(() => { wordContains(); });
            task.Start();
            await task;

            //Color and other
            var check = new TextRange(T9Keyboard.MainRichTextBox.Document.ContentStart,
                T9Keyboard.MainRichTextBox.Document.ContentEnd);

            TextPointer start = T9Keyboard.MainRichTextBox.Document.ContentStart;

            var ofset = T9Keyboard.MainRichTextBox.Document.ContentStart.GetOffsetToPosition(T9Keyboard
                .MainRichTextBox.Document.ContentEnd);

            if (check.Text.Length > 3)
            {
                if (_deleteCharacterState)
                    start = T9Keyboard.MainRichTextBox.Document.ContentStart.GetPositionAtOffset(ofset - 2, LogicalDirection.Forward);
                else
                    start = T9Keyboard.MainRichTextBox.Document.ContentStart.GetPositionAtOffset(ofset - 3, LogicalDirection.Forward);
            }

            var end = T9Keyboard.MainRichTextBox.Document.ContentEnd;
            var range = new TextRange(start, end);

            range.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Black);
            range.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.White);

            _clickCount++;
            _counter = 0;
        }

        private void wordContains()
        {
            T9Keyboard.Advice.Dispatcher.BeginInvoke(new Action(() => { T9Keyboard.Advice.Items.Clear(); }));

            TextRange range = new TextRange(T9Keyboard.MainRichTextBox.Document.ContentStart,
                T9Keyboard.MainRichTextBox.Document.ContentEnd);

            string text = range.Text;
            if (text != "")
            {
                text = text.Remove(text.Length - 1);
                text = text.Remove(text.Length - 1);

                string[] temp = text.Split(' ');
                _counter = 0;
                if (temp.Length > 0 && temp[temp.Length - 1] != "")
                {
                    foreach (var readyWord in _readyWords)
                    {
                        if (readyWord.Contains(temp[temp.Length - 1]))
                        {
                            T9Keyboard.Advice.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                T9Keyboard.Advice.Items.Add(readyWord);
                            }));
                            _counter++;
                        }
                    }

                    if (_counter == 0)
                    {
                        T9Keyboard.AddWord.Dispatcher.BeginInvoke(
                            new Action((() => { T9Keyboard.AddWord.IsEnabled = true; })));
                        _willBeAdded.Add(temp[temp.Length - 1]);
                    }

                    object _obj1 = new object();

                    lock (_obj1)
                    {
                        if (_counter > 0)
                            T9Keyboard.Advice.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                T9Keyboard.Advice.SelectedItem = T9Keyboard.Advice.Items[0];
                            }));
                    }
                }
            }
        }
    }
}
