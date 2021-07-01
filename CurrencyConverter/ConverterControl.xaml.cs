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


namespace CurrencyConverter
{
    class Converter
    {
        public Currency A { get; private set; }
        public Currency B { get; private set; }
        private decimal coef;
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
        public decimal AtoB(decimal v)
        {
            return v * coef;
        }
        public decimal BtoA(decimal v)
        {
            return v / coef;
        }
        public void swap()
        {
            SetCoef(B, A);
        }
        public string BtoAString(decimal v)
        {
            return BtoA(v).ToString("F5");
        }
        public string AtoBString(decimal v)
        {
            return AtoB(v).ToString("F5");
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
            // фильтрация цифр 
            bool flag = true;
            // скипнуть обработку, если изменения были програмными
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
            // скипнуть обработку, если изменения были програмными
            if (ValueA.FocusState == FocusState.Unfocused)
                return;
            decimal newvalue = 0;
            if (ValueA.Text.Length > 0)
                newvalue = decimal.Parse(ValueA.Text);
            ValueB.Text = converterCalculator.AtoBString(newvalue);
        }

        private void ValueB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ValueB.FocusState == FocusState.Unfocused)
                return;
            decimal newvalue = 0;
            if (ValueB.Text.Length > 0)
                newvalue = decimal.Parse(ValueB.Text);
            ValueA.Text = converterCalculator.BtoAString(newvalue);
        }

        /// <summary>
        /// Задать параметры конвертора через объект IFinanceExchange
        /// </summary>
        /// <param name="financeExchange"></param>
        public void SetParams(IFinanceExchange financeExchange)
        {
            this.financeExchange = financeExchange;
            // Если ковертор не задан, то задать его
            if (converterCalculator != null)
            {
                converterCalculator = new Converter(financeExchange.Valute[converterCalculator.A.CharCode], financeExchange.Valute[converterCalculator.B.CharCode]);
                setTextBoxtext();
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
            // Вызов страницы изменения валют
            Frame rootFrame = Window.Current.Content as Frame;
            // делегат обратного вызова, чтобы изменить параметры конвертора
            Action<(string A,  string B)> _backAction = new Action<(string A, string B)>((para) =>
            {
                Currency A = financeExchange.Valute[para.A];
                Currency B = financeExchange.Valute[para.B];
                converterCalculator = new Converter(A, B);
                ValueA.IsEnabled = true;
                ValueB.IsEnabled = true;  
                button.Content = change_valutes_string;
                setTextBoxtext(); 
            });
            if (converterCalculator != null)
                rootFrame.Navigate(typeof(CurrencyChangeWindow), (financeExchange.Valute, _backAction, converterCalculator.A.CharCode, converterCalculator.B.CharCode));
            else
                rootFrame.Navigate(typeof(CurrencyChangeWindow), (financeExchange.Valute, _backAction, "", ""));
        }
        /// <summary>
        /// Свапнуть валюты
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Transfer_Button_Click(object sender, RoutedEventArgs e)
        {
            if (converterCalculator == null)
                return;
            converterCalculator.swap();
            setTextBoxtext();

        }
        /// <summary>
        /// Задать значения TextBlock и TextBox
        /// </summary>
        private void setTextBoxtext()
        {
            ValuteA.Text = converterCalculator.A.CharCode;
            ValuteB.Text = converterCalculator.B.CharCode;
            if (ValueA.Text.Length > 0)
                ValueB.Text = converterCalculator.AtoBString(decimal.Parse(ValueA.Text)).ToString();
            information_textblock.Text = $"1 {converterCalculator.A.CharCode} = {converterCalculator.AtoBString(1)} {converterCalculator.B.CharCode}";
        }
    }
}
