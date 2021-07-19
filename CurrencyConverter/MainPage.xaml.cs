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
using Windows.ApplicationModel.Core;
using System.Net;


namespace CurrencyConverter
{
    /// <summary>
    /// Главная страница
    /// </summary>
    public sealed partial class MainPage : Page
    {
        string data_for_currenttime_string = (string)Application.Current.Resources["data_for_currenttime_string"];
        string error_string = (string)Application.Current.Resources["error_string"];
        string net_error_string = (string)Application.Current.Resources["net_error_string"];
        string ok_string = (string)Application.Current.Resources["ok_string"];
        FinanceSource source;
        public MainPage()
        {
            this.InitializeComponent();
            source = CBRFinanceSource.GetInstance();
            // включить кеширование страниц
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            UpdateQuotes();
            return;
            
        }

        private void Update_Click(object sender, RoutedEventArgs e) => UpdateQuotes();
        private void updateDateTextBox(IFinanceExchange r) => dateofupdate.Text = $"{data_for_currenttime_string} {r.Date}";

        private void toLoadingState()
        {
            LoadingIndicator.Visibility = Visibility.Visible;
            converter.Visibility = Visibility.Collapsed;
            CommadBar.Visibility = Visibility.Collapsed;
            Datepanel.Visibility = Visibility.Collapsed;
        }
        
        private void toWorkState()
        {
            LoadingIndicator.Visibility = Visibility.Collapsed;
            converter.Visibility = Visibility.Visible;
            CommadBar.Visibility = Visibility.Visible;
            Datepanel.Visibility = Visibility.Visible;
        }

        private void  UpdateQuotes()
        {
            toLoadingState();
            TaskMethod();
        }

        private async Task LoadQuotesAndShowIt()
        {
            var quotes = await source.DoRequestWintHandle();
            updateDateTextBox(quotes);
            converter.SetParams(quotes);
            toWorkState();
        }

        private async Task ProvideErrorContentDialogAndExit()
        {
            ContentDialog contentDialog = new ContentDialog
            {
                Title = error_string,
                Content = net_error_string,
                PrimaryButtonText = ok_string,
            };
            await contentDialog.ShowAsync();
            CoreApplication.Exit();
        }

        private async void TaskMethod()
        {
            try
            {
                await LoadQuotesAndShowIt();
            }
            catch (WebException e)
            {
                await ProvideErrorContentDialogAndExit();
            }
        }
    }
}
