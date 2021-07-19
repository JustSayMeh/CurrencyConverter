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
        public decimal AtoB(decimal v) => v * coef;
        public decimal BtoA(decimal v) => v / coef;
        public void swap() => SetCoef(B, A);
        public string BtoAString(decimal v) => BtoA(v).ToString("F5");
        public string AtoBString(decimal v) => AtoB(v).ToString("F5");
    }
    public sealed partial class ConverterControl : UserControl
    {
        private string change_valutes_string = (string)Application.Current.Resources["change_valutes_string"];
        private char culture_separator = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0];
        private IFinanceExchange financeExchange;
        private Converter converterCalculator = null;
        public ConverterControl() => this.InitializeComponent();

        private bool IsProgramTextChanging(TextBox sender) => sender.FocusState == FocusState.Unfocused;
        private void ValueA_TextChanged(object sender, TextChangedEventArgs e) => Value_TextChanged(ValueA, ValueB);
        private void ValueB_TextChanged(object sender, TextChangedEventArgs e) => Value_TextChanged(ValueB, ValueA);

        private void ValidateTextBoxInput(TextBoxBeforeTextChangingEventArgs args)
        {
            bool delimeter_exist_flag = true;
            if (args.NewText.Length > 10)
                args.Cancel = true;
            else
                args.Cancel = args.NewText.Any(current_char =>
                {
                    bool skip_flag = !char.IsDigit(current_char) && (current_char != culture_separator || !delimeter_exist_flag);
                    if (current_char == culture_separator && delimeter_exist_flag)
                        delimeter_exist_flag = false;
                    return skip_flag;
                });
        }

        private void Value_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            if (IsProgramTextChanging(sender))
                return;
            ValidateTextBoxInput(args);
        }

        private void SetNewValueFromA2B(TextBox A, TextBox B)
        {
            decimal newvalue = 0;
            if (A.Text.Length > 0)
                newvalue = decimal.Parse(A.Text);

            B.Text = converterCalculator.AtoBString(newvalue);
        }

        private void Value_TextChanged(TextBox A, TextBox B)
        {
            if (IsProgramTextChanging(A))
                return;
            SetNewValueFromA2B(A, B);
        }

        private void InitConverterCalculator()
        {
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
        private void SetValutesTextBoxesText()
        {
            ValuteA.Text = converterCalculator.A.CharCode;
            ValuteB.Text = converterCalculator.B.CharCode;
        }
        /// <summary>
        /// Задать параметры конвертора через объект IFinanceExchange
        /// </summary>
        /// <param name="financeExchange"></param>
        public void SetParams(IFinanceExchange financeExchange)
        {
            this.financeExchange = financeExchange;
            InitConverterCalculator();
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
            SetValutesTextBoxesText();
            if (ValueA.Text.Length > 0)
                ValueB.Text = converterCalculator.AtoBString(decimal.Parse(ValueA.Text)).ToString();
            information_textblock.Text = $"1 {converterCalculator.A.CharCode} = {converterCalculator.AtoBString(1)} {converterCalculator.B.CharCode}";
        }
    }
}
