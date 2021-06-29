using CurrencyConverter.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace CurrencyConverter
{
    public sealed partial class Converter : UserControl
    {
        private char culture_separator = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0];
        private Currency A = null, B = null;
        private IFinanceExchange financeExchange;
        private double coef = 1;
        public Converter()
        {
            this.InitializeComponent();
        }

        private void Value_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            bool flag = true;
            if (((TextBox)sender).FocusState == FocusState.Unfocused)
                return;
            args.Cancel = args.NewText.Any(c =>
            {
                bool ret = !char.IsDigit(c) && (c != culture_separator || !flag);
                if (c == culture_separator && flag)
                    flag = false;
                return ret;
            });
        }

        private void ValueA_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ValueA.FocusState == FocusState.Unfocused)
                return;
            double newvalue = 0;
            if (ValueA.Text.Length > 0)
                newvalue = double.Parse(ValueA.Text) * coef;
            ValueB.Text = newvalue.ToString();
        }

        private void ValueB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ValueB.FocusState == FocusState.Unfocused)
                return;
            double newvalue = 0;
            if (ValueB.Text.Length > 0)
                newvalue = double.Parse(ValueB.Text) / coef;
            ValueA.Text = newvalue.ToString();
        }


        public void SetParams(IFinanceExchange financeExchange)
        {
            this.financeExchange = financeExchange;
            (A, B) = financeExchange.GetInitPair();
            ValuteA.Text = A.CharCode;
            ValuteB.Text = B.CharCode;
            coef = (A.Value / A.Nominal) / (B.Value / B.Nominal);
            ConverterMain.Visibility = Visibility.Visible;
        }


        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            Action<(string A,  string B)> _backAction = new Action<(string A, string B)>((para) =>
            {
                A = financeExchange.Valute[para.A];
                B = financeExchange.Valute[para.B];
                ValuteA.Text = A.CharCode;
                ValuteB.Text = B.CharCode;
                coef = A.Value / B.Value;
            });
            rootFrame.Navigate(typeof(CurrencyChangeWindow), (financeExchange.GetCurrenciesNames(), _backAction));
        }
    }
}
