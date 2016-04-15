using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ZoDream.Number.Helper;
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

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            
        }

        /// <summary>
        /// The <see cref="DatabaseVisibility" /> property's name.
        /// </summary>
        public const string DatabaseVisibilityPropertyName = "DatabaseVisibility";

        private Visibility _databaseVisibility = Visibility.Collapsed;

        /// <summary>
        /// 必须链接上数据库才能进行操作
        /// </summary>
        public Visibility DatabaseVisibility
        {
            get
            {
                return _databaseVisibility;
            }
            set
            {
                Set(DatabaseVisibilityPropertyName, ref _databaseVisibility, value);
            }
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
            new WebView().Show();
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
            new SearchView().Show();
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
            new MergeFileView().Show();
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

        private RelayCommand _openFilterCommand;

        /// <summary>
        /// Gets the OpenFilterCommand.
        /// </summary>
        public RelayCommand OpenFilterCommand
        {
            get
            {
                return _openFilterCommand
                    ?? (_openFilterCommand = new RelayCommand(ExecuteOpenFilterCommand));
            }
        }

        private void ExecuteOpenFilterCommand()
        {
            new FilterView().Show();
        }

        private RelayCommand _openWeChatCommand;

        /// <summary>
        /// Gets the OpenWeChatCommand.
        /// </summary>
        public RelayCommand OpenWeChatCommand
        {
            get
            {
                return _openWeChatCommand
                    ?? (_openWeChatCommand = new RelayCommand(ExecuteOpenWeChatCommand));
            }
        }

        private void ExecuteOpenWeChatCommand()
        {
            new WeChatView().Show();
        }

        private RelayCommand _openDatabaseCommand;

        /// <summary>
        /// Gets the OpenDatabaseCommand.
        /// </summary>
        public RelayCommand OpenDatabaseCommand
        {
            get
            {
                return _openDatabaseCommand
                    ?? (_openDatabaseCommand = new RelayCommand(ExecuteOpenDatabaseCommand));
            }
        }

        private void ExecuteOpenDatabaseCommand()
        {
            var result = new DatabaseView().ShowDialog();
            if (result == true)
            {
                DatabaseVisibility = Visibility.Visible;
            }
        }

        private RelayCommand _importCommand;

        /// <summary>
        /// Gets the ImportCommand.
        /// </summary>
        public RelayCommand ImportCommand
        {
            get
            {
                return _importCommand
                    ?? (_importCommand = new RelayCommand(ExecuteImportCommand));
            }
        }

        private void ExecuteImportCommand()
        {
            new ImportView().ShowDialog();

        }

        private RelayCommand _exportCommand;

        /// <summary>
        /// Gets the ExportCommand.
        /// </summary>
        public RelayCommand ExportCommand
        {
            get
            {
                return _exportCommand
                    ?? (_exportCommand = new RelayCommand(ExecuteExportCommand));
            }
        }

        private void ExecuteExportCommand()
        {
            new ExportView().ShowDialog();
        }

        private RelayCommand _updateDatabaseCommand;

        /// <summary>
        /// Gets the UpdateDatabaseCommand.
        /// </summary>
        public RelayCommand UpdateDatabaseCommand
        {
            get
            {
                return _updateDatabaseCommand
                    ?? (_updateDatabaseCommand = new RelayCommand(ExecuteUpdateDatabaseCommand));
            }
        }

        private void ExecuteUpdateDatabaseCommand()
        {
            new UpdateDatabaseView().Show();
        }

        public override void Cleanup()
        {
            //DatabaseHelper.GetConnection().Close();
            base.Cleanup();
        }
    }
}