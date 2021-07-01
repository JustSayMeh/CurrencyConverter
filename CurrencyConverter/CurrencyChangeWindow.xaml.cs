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



namespace CurrencyConverter
{
    /// <summary>
    /// Страница изменения валют
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
            // Получить параметры, переданные из главной страницы
            var bind = ((SortedDictionary<string, Currency>, Action<(string A, string B)>, string, string))e.Parameter;
            dictionary = bind.Item1;
            listA.ItemsSource = dictionary;
            listB.ItemsSource = dictionary;
            action = bind.Item2;
            A = bind.Item3;
            B = bind.Item4;
            // Если валюты не были заданы, то задаем первую и вторую
            if (A.Length > 0 || B.Length > 0)
            {
                listA.SelectedIndex = dictionary.Keys.ToList().IndexOf(A);
                listB.SelectedIndex = dictionary.Keys.ToList().IndexOf(B);
            }
            else
            {
                listA.SelectedIndex = 0;
                listB.SelectedIndex = 1;
            }
           
            
        }
        private KeyValuePair<string, Currency> getSelectedItem(ListBox list)
        {
            if (list.SelectedItem != null)
                return (KeyValuePair<string, Currency>)list.SelectedItem;
            return default(KeyValuePair<string, Currency>);
        }
        private void Button_Ok(object sender, RoutedEventArgs e)
        {
            action.Invoke((getSelectedItem(listA).Key, getSelectedItem(listB).Key));
            Frame.GoBack();
        }

        private void Button_Back(object sender, RoutedEventArgs e) => Frame.GoBack();


        private void TextBoxA_KeyUp(object sender, KeyRoutedEventArgs e) => TextBox_KeyUp(TextBoxA, listA);

        private void TextBoxB_KeyUp(object sender, KeyRoutedEventArgs e) => TextBox_KeyUp(TextBoxB, listB);

        private void listA_SelectionChanged(object sender, SelectionChangedEventArgs e)=> list_SelectionChanged(listA, CurrentValueA);
     
        private void listB_SelectionChanged(object sender, SelectionChangedEventArgs e) => list_SelectionChanged(listB, CurrentValueB);

            
       

        private void list_SelectionChanged(ListBox list, TextBlock text)
        {
            
            if (listA.SelectedIndex > -1 && listB.SelectedIndex > -1)
            {
                ok_button.Opacity = 1;
                ok_button.IsEnabled = true;
            }
            // Валюты могут быть не заданы в случае, если фильтр не дал результатов
            if (list.SelectedIndex == -1)
            {
                if (list.Items.Count == 0)
                {
                    ok_button.Opacity = 0.1;
                    ok_button.IsEnabled = false;
                }
                else
                    list.SelectedIndex = 0;
                return;
            }
            text.Text = getSelectedItem(list).Key;
        }


        private void TextBox_KeyUp(TextBox textbox, ListBox ItemList)
        {     
            // Фильтрация ввода
            string mask = textbox.Text;
            if (mask.Length == 0)
                ItemList.ItemsSource = dictionary;
            ItemList.ItemsSource = dictionary.Where(item => item.Key.StartsWith(mask));
            if (ItemList.SelectedIndex == -1 && ItemList.Items.Count > 0)
                ItemList.SelectedIndex = 0;

        }

    }
}
