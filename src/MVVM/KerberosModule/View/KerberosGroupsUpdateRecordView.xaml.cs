using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IAMHeimdall.MVVM.View
{
    /// <summary>
    /// Interaction logic for KerberosGroupsUpdateRecordView.xaml
    /// </summary>
    public partial class KerberosGroupsUpdateRecordView : UserControl
    {
        public KerberosGroupsUpdateRecordView()
        {
            InitializeComponent();
        }

        private void HistoryBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            popupText.Text = historyBox.Text;
            historyPopup.IsOpen = true;
        }

        private void PopupCloseButton_Click(object sender, RoutedEventArgs e)
        {
            historyBox.Text = popupText.Text;
            historyPopup.IsOpen = false;
        }

        private void TicketsBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ticketsBox.SelectionStart = ticketsBox.Text.Length;
            ticketsBox.ScrollToLine(ticketsBox.GetLineIndexFromCharacterIndex(ticketsBox.SelectionStart));
        }

        private void TicketsBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ticketsBox.SelectionStart = ticketsBox.Text.Length;
            ticketsBox.ScrollToLine(ticketsBox.GetLineIndexFromCharacterIndex(ticketsBox.SelectionStart));
        }
    }
}
