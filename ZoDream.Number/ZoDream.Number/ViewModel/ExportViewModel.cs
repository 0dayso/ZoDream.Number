using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MySql.Data.MySqlClient;
using ZoDream.Number.Helper;
using ZoDream.Number.Model;

namespace ZoDream.Number.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ExportViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the ExportViewModel class.
        /// </summary>
        public ExportViewModel()
        {
        }

        /// <summary>
        /// The <see cref="ShowName" /> property's name.
        /// </summary>
        public const string ShowNamePropertyName = "ShowName";

        private bool _myProperty = false;

        /// <summary>
        /// Sets and gets the ShowName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool ShowName
        {
            get
            {
                return _myProperty;
            }
            set
            {
                Set(ShowNamePropertyName, ref _myProperty, value);
            }
        }

        /// <summary>
        /// The <see cref="Count" /> property's name.
        /// </summary>
        public const string CountPropertyName = "Count";

        private int _count = 10000;

        /// <summary>
        /// Sets and gets the Count property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                Set(CountPropertyName, ref _count, value);
            }
        }

        /// <summary>
        /// The <see cref="SavePath" /> property's name.
        /// </summary>
        public const string SavePathPropertyName = "SavePath";

        private string _savePath = string.Empty;

        /// <summary>
        /// Sets and gets the SavePath property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string SavePath
        {
            get
            {
                return _savePath;
            }
            set
            {
                Set(SavePathPropertyName, ref _savePath, value);
            }
        }

        /// <summary>
        /// The <see cref="Kind" /> property's name.
        /// </summary>
        public const string KindPropertyName = "Kind";

        private int _kind = 0;

        /// <summary>
        /// Sets and gets the Kind property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int Kind
        {
            get
            {
                return _kind;
            }
            set
            {
                Set(KindPropertyName, ref _kind, value);
            }
        }

        /// <summary>
        /// The <see cref="Message" /> property's name.
        /// </summary>
        public const string MessagePropertyName = "Message";

        private string _message = string.Empty;

        /// <summary>
        /// Sets and gets the Message property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                Set(MessagePropertyName, ref _message, value);
            }
        }



        private RelayCommand _openSaveCommand;

        /// <summary>
        /// Gets the OpenSaveCommand.
        /// </summary>
        public RelayCommand OpenSaveCommand
        {
            get
            {
                return _openSaveCommand
                    ?? (_openSaveCommand = new RelayCommand(ExecuteOpenSaveCommand));
            }
        }

        private void ExecuteOpenSaveCommand()
        {
            SavePath = LocalHelper.ChooseSaveFile();
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
            //Message = ShowName.ToString();
            if (string.IsNullOrWhiteSpace(SavePath))
            {
                _showMessage("请选择保存路径！");
                return;
            }
            _exportNumber();
        }

        private void _exportNumber()
        {
            Message = "导出开始。。。";
            Task.Factory.StartNew(() =>
            {
                var watch = new Stopwatch();
                watch.Start();
                var writer = new StreamWriter(SavePath, false, Encoding.UTF8);
                using (var conn = new MySqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    var command = new MySqlCommand("SELECT number FROM `zd_number`;", conn);
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader.HasRows)
                        {
                            writer.WriteLine(reader.GetString(0));
                        }
                    }
                    reader.Close();
                }
                writer.Close();
                watch.Stop();
                _showMessage($"运行{watch.Elapsed},导出完成：{SavePath}");
            });
        }

        private void _showMessage(string message)
        {
            Message = message;
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(3000);
                Message = string.Empty;
            });
        }
    }
}