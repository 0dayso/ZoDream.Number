using System.Windows;
using GalaSoft.MvvmLight.Messaging;

namespace ZoDream.Number.View
{
    /// <summary>
    /// Description for DatabaseView.
    /// </summary>
    public partial class DatabaseView : Window
    {
        /// <summary>
        /// Initializes a new instance of the DatabaseViewView class.
        /// </summary>
        public DatabaseView()
        {
            InitializeComponent();
            Messenger.Default.Send(new NotificationMessageAction(null, () =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (!this.ShowActivated) return;
                    this.DialogResult = true;
                    Close();
                });
            }));
        }
    }
}