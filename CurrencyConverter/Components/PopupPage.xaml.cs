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

// Документацию по шаблону элемента "Пользовательский элемент управления" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234236

namespace CurrencyConverter.Components
{
    public sealed partial class PopupPage : UserControl
    {
        public PopupPage()
        {
            this.InitializeComponent();
            Visibility = Visibility.Collapsed;
            frame.IsNavigationStackEnabled = false;
        }
        public void Navigate(Type frame_type, object frame_args) => frame.Navigate(frame_type, frame_args);
        public void SetVisible() => Visibility = Visibility.Visible;
        public void SetHide() => Visibility = Visibility.Collapsed;
        private void Grid_Tapped(object sender, TappedRoutedEventArgs e) => Visibility = Visibility.Collapsed;
      
    }
}
