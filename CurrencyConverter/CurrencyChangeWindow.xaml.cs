using CurrencyConverter.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        Action<(string A, string B)> action;
        public CurrencyChangeWindow()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            IEnumerable<string> list = (((IEnumerable<string>, Action<(string A, string B)>)) e.Parameter).Item1;
            listA.ItemsSource = list;
            listB.ItemsSource = list;
            action = (((IEnumerable<string>, Action<(string A, string B)>))e.Parameter).Item2;
            return;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            action.Invoke(((string)listB.SelectedItem, (string)listA.SelectedItem));
            Frame.GoBack();
        }
    }
}
