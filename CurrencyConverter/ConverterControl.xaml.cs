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
    class Converter
    {
        public Currency A { get; private set; }
        public Currency B { get; private set; }
        private double coef;
        public Converter(Currency A, Currency B)
        {
            SetCoef(A, B);
        }
        public void SetCoef(Currency A, Currency B)
        {
            this.A = A;
            this.B = B;
            coef = (A.Value / A.Nominal) / (B.Value / B.Nominal);
        }
        public double AtoB(double v)
        {
            return v * coef;
        }
        public double BtoA(double v)
        {
            return v / coef;
        }
        public void swap()
        {
            SetCoef(B, A);
        }
        
    }
    public sealed partial class ConverterControl : UserControl
    {
        private string change_valutes_string = (string)Application.Current.Resources["change_valutes_string"];
        private char culture_separator = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0];
        private IFinanceExchange financeExchange;
        private Converter converterCalculator = null;
        public ConverterControl()
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
                newvalue = double.Parse(ValueA.Text);
            ValueB.Text = converterCalculator.AtoB(newvalue).ToString();
        }

        private void ValueB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ValueB.FocusState == FocusState.Unfocused)
                return;
            double newvalue = 0;
            if (ValueB.Text.Length > 0)
                newvalue = double.Parse(ValueB.Text);
            ValueA.Text = converterCalculator.BtoA(newvalue).ToString();
        }


        public void SetParams(IFinanceExchange financeExchange)
        {
            this.financeExchange = financeExchange;
            if (converterCalculator != null)
            {
                converterCalculator = new Converter(financeExchange.Valute[converterCalculator.A.CharCode], financeExchange.Valute[converterCalculator.B.CharCode]);
                ValuteA.Text = converterCalculator.A.CharCode;
                ValuteB.Text = converterCalculator.B.CharCode;
                if (ValueA.Text.Length > 0)
                    ValueB.Text = converterCalculator.AtoB(double.Parse(ValueA.Text)).ToString();
            }
            else
            {
                ValueA.IsEnabled = false;
                ValueB.IsEnabled = false;
                ValuteA.Text = "---";
                ValuteB.Text = "---";

            }
            
    
        }


        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            Action<(string A,  string B)> _backAction = new Action<(string A, string B)>((para) =>
            {
                Currency A = financeExchange.Valute[para.A];
                Currency B = financeExchange.Valute[para.B];
                ValuteA.Text = A.CharCode;
                ValuteB.Text = B.CharCode;
                ValueA.IsEnabled = true;
                ValueB.IsEnabled = true;
                converterCalculator = new Converter(A, B);
                button.Content = change_valutes_string;
                if (ValueA.Text.Length > 0)
                    ValueB.Text = converterCalculator.AtoB(double.Parse(ValueA.Text)).ToString();
            });
            if (converterCalculator != null)
                rootFrame.Navigate(typeof(CurrencyChangeWindow), (financeExchange.Valute, _backAction, converterCalculator.A.CharCode, converterCalculator.B.CharCode));
            else
                rootFrame.Navigate(typeof(CurrencyChangeWindow), (financeExchange.Valute, _backAction, "", ""));
        }

        private void Transfer_Button_Click(object sender, RoutedEventArgs e)
        {
            if (converterCalculator == null)
                return;
            converterCalculator.swap();
            ValuteA.Text = converterCalculator.A.CharCode;
            ValuteB.Text = converterCalculator.B.CharCode;
            if (ValueA.Text.Length > 0)
                ValueB.Text = converterCalculator.AtoB(double.Parse(ValueA.Text)).ToString();
        }
    }
}
