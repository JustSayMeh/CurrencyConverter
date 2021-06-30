using CurrencyConverter.Model;
using CurrencyConverter.Model.CBR;
using CurrencyConverter.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CurrencyConverter;
using System.Threading;
using Windows.UI.ViewManagement;
// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace CurrencyConverter
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        string data_for_currenttime_string = (string)Application.Current.Resources["data_for_currenttime_string"];
        FinanceSource source;
        public MainPage()
        {
            this.InitializeComponent();
            source = CBRFinanceSource.GetInstance();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            Update();
            Task.Factory.StartNew(TaskMethod);
            
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            Update();
            Task.Factory.StartNew(TaskMethod);
        }

        private void updateDateTextBox(IFinanceExchange r)
        {
            dateofupdate.Text = $"{data_for_currenttime_string} {r.Date}";
        }
        private void  Update()
        {
            LoadingIndicator.Visibility = Visibility.Visible;
            converter.Visibility = Visibility.Collapsed;
            CommadBar.Visibility = Visibility.Collapsed;
            Datepanel.Visibility = Visibility.Collapsed;
        }
        private void Updated()
        {
            LoadingIndicator.Visibility = Visibility.Collapsed;
            converter.Visibility = Visibility.Visible;
            CommadBar.Visibility = Visibility.Visible;
            Datepanel.Visibility = Visibility.Visible;
        }
        private async void TaskMethod()
        {
            
            var r = source.DoRequestWintHandle();
            Thread.Sleep(2000);
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                LoadingIndicator.Visibility = Visibility.Collapsed;
                updateDateTextBox(r);
                converter.SetParams(r);
                Updated();
            });

        }
    }
}
