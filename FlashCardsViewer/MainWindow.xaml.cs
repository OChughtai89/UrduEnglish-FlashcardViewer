﻿//-------------------------------------------------------------------
// 2015- All rights reserved. 
// Created by:	Omar Chughtai
//-------------------------------------------------------------------

using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
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

namespace FlashCardsViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region private variables
        private const string flashCardNumber = "FlashCard #"; //represents a flashcard by what number it is
        private bool urduSide = true; //denotes whether the urdu side is showing or english side is
        private int handle = 0; //represents position inside collection
        private ObservableCollection<KeyValuePair> dict; //collection of key values
        private const string flashCardNotSelected = "No Flash Card Selected!!";
        private string defaultFilePath = @"C:\Users\" + Environment.UserName + @"\Desktop\urdu_to_english.csv";
        private bool exists = false;
        
        #endregion
        

        public MainWindow()
        {
            SplashScreen sc = new SplashScreen(@"Resources\pakistan_splash2.jpg");
            sc.Show(false);
            sc.Close(TimeSpan.FromSeconds(3));
            System.Threading.Thread.Sleep(3000);
            sc = null;
            InitializeComponent();            
            dict = new ObservableCollection<KeyValuePair>();        
            listBoxFlashcards.DataContext = dict;
            if (string.IsNullOrEmpty(Properties.Settings.Default.filePath))
                Properties.Settings.Default.filePath = defaultFilePath;
            Properties.Settings.Default.Save();
            exists = File.Exists(Properties.Settings.Default.filePath);
            if (!File.Exists(Properties.Settings.Default.filePath))
            {               
                using (StreamWriter sw = new StreamWriter(Properties.Settings.Default.filePath))
                {
                    sw.WriteLine("Urdu, English");
                    sw.WriteLine("ab" + ", " + "now");
                    sw.WriteLine("in" + ", " + "those");
                    sw.WriteLine("aisa" + ", " + "such");
                    sw.WriteLine("amir" + ", " + "rich");
                    sw.WriteLine("aur" + ", " + "and");
                    sw.WriteLine("mera" + ", " + "my");
                    sw.WriteLine("muhjhe" + ", " + "me");
                    sw.WriteLine("eik" + ", " + "one");
                    sw.WriteLine("nam" + ", " + "name");
                    sw.WriteLine("batie" + ", " + "sit down");
                    sw.WriteLine("kahan" + ", " + "where");
                    sw.WriteLine("hamara" + ", " + "our");
                    sw.WriteLine("ham" + ", " + "we");
                    sw.WriteLine("voh" + ", " + "he/she/it");
                    sw.WriteLine("yih" + ", " + "he/she/it");
                    sw.WriteLine("hal" + ", " + "condition");
                    sw.WriteLine("hai" + ", " + "is");
                    sw.WriteLine("larka" + ", " + "boy");
                    sw.WriteLine("larkee" + ", " + "girl");
                    sw.WriteLine("bachay" + ", " + "children");
                    sw.WriteLine("bacha" + ", " + "child");
                    sw.WriteLine("tum" + ", " + "you(informal)");
                    sw.WriteLine("ap" + ", " + "you(formal)");
                    sw.WriteLine("kitna" + ", " + "how many");
                    sw.WriteLine("sar" + ", " + "head");
                    sw.WriteLine("sayhat" + ", " + "health");
                    sw.WriteLine("kab" + ", " + "when");
                    sw.WriteLine("koi" + ", " + "someone");
                    sw.WriteLine("sunna" + ", " + "hear");
                    sw.WriteLine("suna" + ", " + "heard");
                    sw.WriteLine("batana" + ", " + "tell");
                    sw.WriteLine("naachna" + ", " + "dance");
                    sw.WriteLine("bemar" + ", " + "sick");
                    sw.WriteLine("kuch" + ", " + "some/something");         
                }            
            }
        }

        ~MainWindow()
        {         
            using (StreamWriter sw = new StreamWriter(Properties.Settings.Default.filePath))          
            {
                sw.WriteLine("Urdu, English");
                foreach (KeyValuePair kvp in dict)
                {
                    sw.WriteLine(kvp.Value.UrduPhrase + ", " + kvp.Value.EnglishPhrase);
                }
            }
           
        }
     
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TextFieldParser parser;
            if (File.Exists(Properties.Settings.Default.filePath))
            {
                try
                {
                    parser = new TextFieldParser(Properties.Settings.Default.filePath);
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    parser.ReadFields(); //skips first line
                    while (!parser.EndOfData)
                    {
                        string[] phrases = parser.ReadFields();
                        FlashCard fc = new FlashCard();
                        fc.UrduPhrase = phrases[0];
                        fc.EnglishPhrase = phrases[1];
                        dict.Add(new KeyValuePair() { Key = flashCardNumber + ++handle, Value = fc });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
               
                MessageBox.Show("File does not exist or invalid file path");
            }
        }

        private void listBoxFlashcards_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxFlashcards.SelectedItem ==null)
            {
                e.Handled = true;
                return;
            }
            urduSide = true;
            string lbi = listBoxFlashcards.SelectedItem.ToString();            
            this.txtBlockCardData.Text = dict.Single(x => x.Key == lbi).Value.UrduPhrase;
            this.txtBlockCardData.Visibility = Visibility.Visible;
            this.flashCardBorder.Visibility = Visibility.Visible;                    
        }

        private void Button_ShowFlashCard(object sender, RoutedEventArgs e)
        {
            if (listBoxFlashcards.SelectedItem == null)
            {
                MessageBox.Show(flashCardNotSelected);
                e.Handled = true;            
                return;
            }
            string lbi = listBoxFlashcards.SelectedItem.ToString();
            this.txtBlockCardData.Text = dict.Single(x => x.Key == lbi).Value.UrduPhrase.ToString();
            this.txtBlockCardData.Visibility = Visibility.Visible;
            this.flashCardBorder.Visibility = Visibility.Visible;

        }

        private void Button_FlipFlashCard(object sender, RoutedEventArgs e)
        {
            if (listBoxFlashcards.SelectedItem == null)
            {
                MessageBox.Show(flashCardNotSelected);
                e.Handled = true;
                return;
            }
            string lbi = listBoxFlashcards.SelectedItem.ToString();

            if (urduSide)
            {
                this.txtBlockCardData.Text = dict.Single(x => x.Key == lbi).Value.EnglishPhrase.ToString();
                urduSide = false;
            }
            else
            {
                this.txtBlockCardData.Text = dict.Single(x => x.Key == lbi).Value.UrduPhrase.ToString();
                urduSide = true;
            }
        }

        private void Button_AddFlashCard(object sender, RoutedEventArgs e)
        {
            AddWindow ad = new AddWindow();
            ad.Show();
            
            ad.AddFlashCardEvent += ad_AddFlashCardEvent;
        }

        void ad_AddFlashCardEvent(string urduWord, string englishWord)
        {
            FlashCard fc = new FlashCard();
            fc.UrduPhrase = urduWord;
            fc.EnglishPhrase = englishWord;
            dict.Add(new KeyValuePair() { Key = flashCardNumber + ++handle, Value = fc });
               
        }

        private void Button_DeleteFlascard(object sender, RoutedEventArgs e)
        {
            if (listBoxFlashcards.SelectedItem==null)
            {
                MessageBox.Show(flashCardNotSelected);
                e.Handled = true;
                return;
            }
            string key = listBoxFlashcards.SelectedItem.ToString();
            dict.Remove(dict.First(x => x.Key == key));

            
            KeyValuePair[] dictCopy=new KeyValuePair[dict.Count()];
            dict.CopyTo(dictCopy,0);
            dict.Clear();
            handle = 0;
            foreach(KeyValuePair kvp in dictCopy)
            {
                dict.Add(new KeyValuePair() { Key = flashCardNumber + ++handle, Value = new FlashCard() { UrduPhrase = kvp.Value.UrduPhrase, EnglishPhrase = kvp.Value.EnglishPhrase } });
                
            }
            this.txtBlockCardData.Visibility = Visibility.Hidden;
            this.flashCardBorder.Visibility = Visibility.Hidden;           
        }

        private void Button_Edit(object sender, RoutedEventArgs e)
        {
            if (listBoxFlashcards.SelectedItem == null)
            {
                MessageBox.Show(flashCardNotSelected);
                e.Handled = true;
                return;
            }
            KeyValuePair kvp = listBoxFlashcards.SelectedItem as KeyValuePair;                        
            EditWindow ew = new EditWindow(kvp);
            ew.Show();

            ew.ApplyChangeEvent += ew_ApplyChangeEvent;
        }

        void ew_ApplyChangeEvent(string text1, string text2)
        {
            string key = listBoxFlashcards.SelectedItem.ToString();
            dict.Single(x => x.Key == key).Value.UrduPhrase = text1;
            dict.Single(x => x.Key == key).Value.EnglishPhrase = text2;            
        }

        private void Button_Speak(object sender, RoutedEventArgs e)
        {
            if (listBoxFlashcards.SelectedItem == null)
            {
                MessageBox.Show(flashCardNotSelected);
                e.Handled = true;
                return;
            }
            SpeechSynthesizer speech = new SpeechSynthesizer();
            KeyValuePair kvp = listBoxFlashcards.SelectedItem as KeyValuePair;
            if (urduSide)
                speech.Speak(kvp.Value.UrduPhrase);
            else
                speech.Speak(kvp.Value.EnglishPhrase);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                txtBlockCardData.FontSize = 150;
                //ShowFlashButton.Height = 50;
                //ShowFlashButton.Width = 180;
                AddCardButton.Height = 50;
                AddCardButton.Width = 180;
                DeleteButton.Height = 50;
                DeleteButton.Width = 180;
                FlipButton.Height = 50;
                FlipButton.Width = 180;
                EditButton.Height = 50;
                EditButton.Width = 180;
                HearButton.Height = 50;
                HearButton.Width = 180;
            }
            else
            {
                txtBlockCardData.FontSize = 70;
                //ShowFlashButton.Height = 25;
                //ShowFlashButton.Width = 90;
                AddCardButton.Height = 25;
                AddCardButton.Width = 90;
                DeleteButton.Height = 25;
                DeleteButton.Width = 90;
                FlipButton.Height = 25;
                FlipButton.Width = 90;
                EditButton.Height = 25;
                EditButton.Width = 90;
                HearButton.Height = 25;
                HearButton.Width = 90;
            }
        }

    }
}
