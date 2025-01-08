using Microsoft.Web.WebView2.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;

namespace IAMHeimdall.Resources
{
    public class BrowserBehavior
    {
        #region Methods
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
             "Html",
             typeof(string),
             typeof(BrowserBehavior),
             new FrameworkPropertyMetadata(OnHtmlChanged));

        [AttachedPropertyBrowsableForType(typeof(WebBrowser))]
        public static string GetHtml(WebBrowser d)
        {
            return (string)d.GetValue(HtmlProperty);
        }

        public static void SetHtml(WebBrowser d, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                d.SetValue(HtmlProperty, value);
            }
        }

        static void OnHtmlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is WebBrowser webBrowser)
            {
                if (e.NewValue != null)
                {
                    if (!string.IsNullOrEmpty(e.NewValue.ToString())) { webBrowser.NavigateToString(e.NewValue as string ?? "&nbsp;"); }
                }
            }
        }
        #endregion
    }
}
