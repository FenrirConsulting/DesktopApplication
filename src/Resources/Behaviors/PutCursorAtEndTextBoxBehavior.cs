﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace IAMHeimdall.Resources.Behaviors
{
    public class PutCursorAtEndTextBoxBehavior : Behavior<UIElement>
    {
        #region Functions
        private TextBox _textBox;

        protected override void OnAttached()
        {
            base.OnAttached();

            _textBox = AssociatedObject as TextBox;

            if (_textBox == null)
            {
                return;
            }
            _textBox.GotFocus += TextBoxGotFocus;
        }

        protected override void OnDetaching()
        {
            if (_textBox == null)
            {
                return;
            }
            _textBox.GotFocus -= TextBoxGotFocus;

            base.OnDetaching();
        }

        private void TextBoxGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            _textBox.CaretIndex = _textBox.Text.Length;
        }
        #endregion
    }
}
