using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;


namespace SPR.Graph.Behaviors
{
     class TextBox4Folder: Behavior<TextBox>
    {

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.TextChanged += OnAssociatedObjectTextChanged;
            Update();
        }
        protected override void OnDetaching()
        {
            AssociatedObject.TextChanged -= OnAssociatedObjectTextChanged;
            base.OnDetaching();
        }

        void OnAssociatedObjectTextChanged(object sender, TextChangedEventArgs e)
        {
            Update();
        }

        void Update()
        {
            if (AssociatedObject == null) return;
            /*if (AssociatedObject.Text == InvalidValue)
                AssociatedObject.Foreground = InvalidForeground;
            else AssociatedObject.Foreground = ValidForeground;*/
        }









        public static readonly DependencyProperty IsFolderInputProperty =
            DependencyProperty.RegisterAttached("IsFolderInput", typeof(bool),
            typeof(TextBox4Folder), new UIPropertyMetadata(false, OnValueChanged));

        public static bool GetIsFolderInputProperty(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFolderInputProperty);
        }

        public static void SetIsFolderInputProperty(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFolderInputProperty, value);
        }


        private static void OnValueChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {/*
            var uiElement = dependencyObject as Control;
            if (uiElement == null) return;
            if (e.NewValue is bool && (bool)e.NewValue)
            {
                uiElement.PreviewTextInput += OnTextInput;
                uiElement.PreviewKeyDown += OnPreviewKeyDown;
                DataObject.AddPastingHandler(uiElement, OnPaste);
            }

            else
            {
                uiElement.PreviewTextInput -= OnTextInput;
                uiElement.PreviewKeyDown -= OnPreviewKeyDown;
                DataObject.RemovePastingHandler(uiElement, OnPaste);
            }*/
        }
    }
}
