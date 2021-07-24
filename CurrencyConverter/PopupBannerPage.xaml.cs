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
    public sealed partial class PopupBannerPage : Page
    {
        public PopupBannerPage()
        {
            this.InitializeComponent();
            
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            (TappedEventHandler, Type) d = ((TappedEventHandler, Type))e.Parameter;
            background.Tapped += d.Item1;
            frames.Navigate(d.Item2);
            base.OnNavigatedTo(e);

        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}
