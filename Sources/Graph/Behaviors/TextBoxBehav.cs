using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SPR.Graph.Behaviors
{
    public static class TextBoxBehav
    {



        public static readonly DependencyProperty IsFolderInputProperty = DependencyProperty.RegisterAttached(
            "IsFolderInput",
            typeof(bool),
            typeof(TextBoxBehav),
            new FrameworkPropertyMetadata(false, OnIsFolderInputChanged));


        /*
         * Get et set vont permettre de gérer dans le wpf IsFolderInput, même si ça peut paraitre étrange
         */
        public static bool GetIsFolderInput(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFolderInputProperty);
        }


        public static void SetIsFolderInput(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFolderInputProperty, value);
        }

        private static TextCompositionEventHandler OnIsFolderInputPreviewTextInputHandler = new TextCompositionEventHandler(OnTextBox_PreviewTextInput);


        private static void OnIsFolderInputChanged(DependencyObject owner, DependencyPropertyChangedEventArgs args)
        {
            TextBox textBox = owner as TextBox;
            bool? isFolderInput = args.NewValue as bool?;

            if (isFolderInput == null || textBox == null)
                return;

            if (isFolderInput ?? false)
            {
                textBox.PreviewTextInput += OnIsFolderInputPreviewTextInputHandler;

                //  textBox.TextChanged += OnIsFolderInput_TextChangedHandler;

                /* textBox.MouseLeave += OnIsFolderInput_MouseLeaveHandler;
                 textBox.LostMouseCapture += TextBox_LostMouseCapture;
                 textBox.TextInput += TextBox_TextInput;*/
            }
            else
            {
                textBox.PreviewTextInput -= OnIsFolderInputPreviewTextInputHandler;
            }
        }


        private static void OnIsFolderInput_TextChangedHandler(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            Regex r = new Regex("[*?\\\\:/|<>]");

            Debug.WriteLine("OnIsFolderInput_TextChangedHandler: " + tb.Text);
            if (r.IsMatch(tb.Text))
                tb.Text = string.Empty;
        }

        /*
        private static void TextBox_TextInput(object sender, TextCompositionEventArgs e)
        {
            
        }
        
        private static void TextBox_LostMouseCapture(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("Lostmousecapture");
        }

        private static void OnIsFolderInput_MouseLeaveHandler(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            Regex r = new Regex("[*?\\\\:/|<>]");
            
            Debug.WriteLine("Mouseleavehandler: " + tb.Text);
        }
        */

        ///<summary>
        /// Empêche de rajouter certains symboles à une chaine
        /// </summary>  
        private static void OnTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox control = sender as TextBox;
            if (control == null)
                return;

            if (string.IsNullOrEmpty(e.Text))
                return;

            Regex r = new Regex("[*?\\\\:/|<>]");
            e.Handled = r.IsMatch(e.Text);


        }

    }
}
