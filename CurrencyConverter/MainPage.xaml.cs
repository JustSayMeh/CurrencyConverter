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
            // получить инстанс ресурса данных
            source = CBRFinanceSource.GetInstance();
            // включить кеширование страниц
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            // перевести в режим обновления
            Update();
            
        }

        private void Update_Click(object sender, RoutedEventArgs e) => Update();
        

        private void updateDateTextBox(IFinanceExchange r) => dateofupdate.Text = $"{data_for_currenttime_string} {r.Date}";



        private void  Update()
        {
            LoadingIndicator.Visibility = Visibility.Visible;
            converter.Visibility = Visibility.Collapsed;
            CommadBar.Visibility = Visibility.Collapsed;
            Datepanel.Visibility = Visibility.Collapsed;
            // запуск параллельной задачи
            Task.Factory.StartNew(TaskMethod);
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
            try
            {
                // получить результаты запроса
                var r = source.DoRequestWintHandle();
                // Искуственная задержка для демострации спинера
                Thread.Sleep(1000);
                // запустить в потоке UI
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                    LoadingIndicator.Visibility = Visibility.Collapsed;
                    updateDateTextBox(r);
                    // задать параметры для контрола ковертора
                    converter.SetParams(r);
                    Updated();
                });
            }catch (WebException e)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () => {
                    ContentDialog contentDialog = new ContentDialog
                    {
                        Title = error_string,
                        Content = net_error_string,
                        PrimaryButtonText = ok_string,
                    };
                    await contentDialog.ShowAsync();
                    CoreApplication.Exit();
                });
            }
           

        }
    }
}
