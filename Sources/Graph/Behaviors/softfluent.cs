using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SPR.Graph.Behaviors
{
    public static partial class softfluent
    {
        public static readonly DependencyProperty IsAlphaNumericInputProperty = DependencyProperty.RegisterAttached(
            "IsAlphaNumericInput",
            typeof(bool),
            typeof(softfluent),
            new FrameworkPropertyMetadata(false, OnIsAlphaNumericInputChanged));

        public static bool GetIsAlphaNumericInput(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsAlphaNumericInputProperty);
        }

        public static void SetIsAlphaNumericInput(DependencyObject obj, bool value)
        {
            obj.SetValue(IsAlphaNumericInputProperty, value);
        }

        private static TextCompositionEventHandler OnAlphaNumericInputPreviewTextInputHandler = new TextCompositionEventHandler(OnTextBox_PreviewTextInput);

        private static void OnIsAlphaNumericInputChanged(DependencyObject owner, DependencyPropertyChangedEventArgs args)
        {
            Control textBox = owner as TextBox;
            bool? isAlphaNumericInput = args.NewValue as bool?;

            if (isAlphaNumericInput == null || textBox == null)
                return;

            if (isAlphaNumericInput ?? false)
            {
                textBox.PreviewTextInput += OnAlphaNumericInputPreviewTextInputHandler;
            }
            else
            {
                textBox.PreviewTextInput -= OnAlphaNumericInputPreviewTextInputHandler;
            }
        }

        private static void OnTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Control control = sender as TextBox;
            if (control == null)
                return;

            if (string.IsNullOrEmpty(e.Text))
                return;

            Regex r = new Regex("^[a-zA-Z0-9]*$");
            e.Handled = !r.IsMatch(e.Text);
        }
    }


}
