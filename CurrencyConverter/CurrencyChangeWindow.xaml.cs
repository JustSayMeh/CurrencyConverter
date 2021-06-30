using CurrencyConverter.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace CurrencyConverter
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class CurrencyChangeWindow : Page
    {
        private Action<(string A, string B)> action;
        private string A, B;
        private SortedDictionary<string, Currency> dictionary;
        public CurrencyChangeWindow()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var bind = ((SortedDictionary<string, Currency>, Action<(string A, string B)>, string, string))e.Parameter;
            dictionary = bind.Item1;
            listA.ItemsSource = dictionary;
            listB.ItemsSource = dictionary;
            action = bind.Item2;
            A = bind.Item3;
            B = bind.Item4;
            listA.SelectedIndex = dictionary.Keys.ToList().IndexOf(A);
            listB.SelectedIndex = dictionary.Keys.ToList().IndexOf(B);
        }

        private void Button_Ok(object sender, RoutedEventArgs e)
        {
        
            action.Invoke((((KeyValuePair<string, Currency>)listA.SelectedItem).Key, (((KeyValuePair<string, Currency>)listB.SelectedItem).Key)));
            Frame.GoBack();
        }

        protected void Button_Back(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }


        private void TextBoxA_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            TextBox_KeyUp(TextBoxA, listA);
        }

        private void TextBoxB_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            TextBox_KeyUp(TextBoxB, listB);
        }






        private void TextBox_KeyUp(TextBox textbox, ListBox ItemList)
        {
            string mask = textbox.Text;
            if (mask.Length == 0)
                ItemList.ItemsSource = dictionary;
            ItemList.ItemsSource = dictionary.Where(item => item.Key.StartsWith(mask));
        }

    }
}
