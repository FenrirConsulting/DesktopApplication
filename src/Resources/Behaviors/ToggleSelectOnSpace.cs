using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace IAMHeimdall.Resources.Behaviors
{
    #region Methods
    public class ToggleSelectOnSpace : Behavior<DataGrid>
    {
        public static readonly DependencyProperty toggleSelectCommand =
            DependencyProperty.Register("ToggleSelectCommand",
            typeof(ICommand),
            typeof(ToggleSelectOnSpace));

        public ICommand ToggleSelectCommand
        {
            get { return this.GetValue(toggleSelectCommand) as ICommand; }
            set { this.SetValue(toggleSelectCommand, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.PreviewKeyUp += PreviewKeyUp;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.PreviewKeyUp -= PreviewKeyUp;
            base.OnDetaching();
        }

        private void PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (ToggleSelectCommand != null)
                {
                    ToggleSelectCommand.Execute(this.AssociatedObject.SelectedItems);
                }
            }
        }
    }
    #endregion
}
