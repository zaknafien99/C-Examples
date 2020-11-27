using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UserInput
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //Gets or sets the value of the first number that the arithmetic will be performed on.
        private double? FirstNumber { get; set; } /*The ? after double declares that it's nullable. Null is different than 0. It means there's no value at all. Null is important because later, we check to see if the user entered a value in the first TextBox. We want to do the calculation if the user entered 0 but not if the user left it empty.*/

        // Gets or sets the calue of the second number that the arithmetic will be performed on.
        private double? SecondNUmber { get; set; }

        // Gets or sets the selected aritmetic operation.
        private Func<double, double, double> SelectedMathFunction { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
        }

        private double Add(double a, double b)
        {
            double result = a + b;
            return result;
        }

        private double Subtract(double a, double b)
        {
            double result = a - b;
            return result;
        }

        private double Multiply(double a, double b)
        {
            double result = a * b;
            return result;
        }

        private double Divide(double a, double b)
        {
            double result = a / b;
            return result;
        }


        private void FirstNumberBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Check if the text in FirstNumberBox.Text is empty
            if (string.IsNullOrEmpty(FirstNumberBox?.Text))
            {
                FirstNumber = null;
                return;
            }

            // If text was entered, check to see if it can be parsed into a number
            if(double.TryParse(FirstNumberBox.Text, out double parsedNumber))
            {
                //If the text is an integer, then set the value of the FirstNUmber property
                FirstNumber = parsedNumber;
            }
            else
            {
                // If it is not a number, remove the last entered character with Trim() method.
                FirstNumberBox.Text = FirstNumberBox.Text.TrimEnd(FirstNumberBox.Text.LastOrDefault());
            }
        }

        private void RadioButton_OnChecked(object sender, RoutedEventArgs e)
        {
            // We can use a single line of code to get the RadioButton reference
            var radioButton = sender as RadioButton;

            // Get the value of the RadioBUtton's Content (this contains the name of the math operation we want).
            string radioButtonContent = radioButton?.Content?.ToString();

            // Set the appropriate arithmetic operation to use by checking radioButtonContent's value.
            switch(radioButtonContent)
            {
                case "Add":
                    SelectedMathFunction = Add;
                    break;
                case "Subtract":
                    SelectedMathFunction = Subtract;
                    break;
                case "Multiply":
                    SelectedMathFunction = Multiply;
                    break;
                case "Divide":
                    SelectedMathFunction = Divide;
                    break;
                default:
                    SelectedMathFunction = null;
                    break;
            }
        }

        private void SecondNumberBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // We can reuse the same logic we did for the FirstNumberBox
            if (string.IsNullOrEmpty(SecondNumberBox?.Text))
            {
                SecondNUmber = null;
                return;
            }

            if(double.TryParse(SecondNumberBox.Text, out double parsedNumber))
            {
                SecondNUmber = parsedNumber;
            }
            else
            {
                SecondNumberBox.Text = SecondNumberBox.Text.TrimEnd(SecondNumberBox.Text.LastOrDefault());
            }
        }

        private void SecondNumberSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            // You would typically use e.NewValue to get the Slider's value as it changes.
            // However, the SecondNumberBox.TextChanged event is already getting the value,
            // we can just set the SecondNumberBox.Text instead.
            SecondNumberBox.Text = e.NewValue.ToString("N");
        }

        private void IncludeDateCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // When the CheckBox is checked, show date pickers.
            CalculationDatePicker.Visibility = Visibility.Visible;
            CalculationDatePicker.SelectedDate = DateTimeOffset.Now;
        }

        private void IncludeDateCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // When the CheckBox is unchecked, hide the DatePickers.
            CalculationDatePicker.Visibility = Visibility.Collapsed;
        }

     
        private async void EqualsButton_OnClick(object sender, RoutedEventArgs e)
        {
            // Before doing any calculation, confirm the user entered both numbers.
            if (FirstNumber == null || SecondNUmber == null)
            {
                await new MessageDialog("You need to set both numbers to calculate a result.").ShowAsync();
                return;
            }

            // Now is a good time to do some validation on the numbers and prevent any serious problems.
            // For example, here we make sure the user isn't trying to divide from zero, this can crash your app!
            if (SecondNUmber == 0 && SelectedMathFunction == Divide)
            {
                await new MessageDialog("You cannot divide by zero, please pick a different 2nd number.").ShowAsync();
                return;
            }

            // Next, it's time to do the actual math. We only need to pass the two numbers into the SelectedMathFunction.
            double result = SelectedMathFunction((double)FirstNumber, (double)SecondNUmber);

            // Finally, show the result to the user!
            if (IncludeDateCheckBox.IsChecked == true)
            {
                // If the CheckBox was checked, show the number with the "N2" string format (a number to 2 decimal points),
                // but also include the Date in the output with the "d" string format (a short date format).
                ResultsTextBlock.Text = $"Result: {result:N2}, Date: {CalculationDatePicker.SelectedDate:d}";
            }
            else
            {
                // If the CheckBox was not checked, show the number with the "N2" string format (a number to 2 decimal points).
                ResultsTextBlock.Text = $"Result: {result:N2}";
            }
        }
    }
}
