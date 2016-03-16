using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ZoDream.Number.Model;
using ZoDream.Number.View;

namespace ZoDream.Number.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        /// <summary>
        /// The <see cref="WelcomeTitle" /> property's name.
        /// </summary>
        public const string WelcomeTitlePropertyName = "WelcomeTitle";

        private string _welcomeTitle = string.Empty;

        /// <summary>
        /// Gets the WelcomeTitle property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string WelcomeTitle
        {
            get
            {
                return _welcomeTitle;
            }
            set
            {
                Set(ref _welcomeTitle, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.GetData(
                (item, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }

                    WelcomeTitle = item.Title;
                });
        }

        private RelayCommand _openWebCommand;

        /// <summary>
        /// Gets the OpenWebCommand.
        /// </summary>
        public RelayCommand OpenWebCommand
        {
            get
            {
                return _openWebCommand
                    ?? (_openWebCommand = new RelayCommand(ExecuteOpenWebCommand));
            }
        }

        private void ExecuteOpenWebCommand()
        {
            (new WebView()).Show();
        }

        private RelayCommand _openSearchCommand;

        /// <summary>
        /// Gets the OpenSearchCommand.
        /// </summary>
        public RelayCommand OpenSearchCommand
        {
            get
            {
                return _openSearchCommand
                    ?? (_openSearchCommand = new RelayCommand(ExecuteOpenSearchCommand));
            }
        }

        private void ExecuteOpenSearchCommand()
        {
            (new SearchView()).Show();
        }

        private RelayCommand _openFileCommand;

        /// <summary>
        /// Gets the OpenFileCommand.
        /// </summary>
        public RelayCommand OpenFileCommand
        {
            get
            {
                return _openFileCommand
                    ?? (_openFileCommand = new RelayCommand(ExecuteOpenFileCommand));
            }
        }

        private void ExecuteOpenFileCommand()
        {
            (new FileView()).Show();
        }

        private RelayCommand _openSpiderCommand;

        /// <summary>
        /// Gets the OpenSpiderCommand.
        /// </summary>
        public RelayCommand OpenSpiderCommand
        {
            get
            {
                return _openSpiderCommand
                    ?? (_openSpiderCommand = new RelayCommand(ExecuteOpenSpiderCommand));
            }
        }

        private void ExecuteOpenSpiderCommand()
        {
            (new SpiderView()).Show();
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}