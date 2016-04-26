using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ZoDream.Number.Helper;
using ZoDream.Number.Model;
using System.IO;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using ZoDream.Number.Comparer;
using ZoDream.Helper.Local;

namespace ZoDream.Number.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ImportViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the ImportViewModel class.
        /// </summary>
        public ImportViewModel()
        {

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

        /// <summary>
        /// The <see cref="NumberList" /> property's name.
        /// </summary>
        public const string NumberListPropertyName = "NumberList";

        private ObservableCollection<NumberItem> _numberList = new ObservableCollection<NumberItem>();

        /// <summary>
        /// Sets and gets the NumberList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<NumberItem> NumberList
        {
            get
            {
                return _numberList;
            }
            set
            {
                Set(NumberListPropertyName, ref _numberList, value);
            }
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
            _addNumber(Open.ChooseFiles());
            _showMessage($"总共有{NumberList.Count}条号码！请在开始前执行 去重 操作");
        }

        private RelayCommand _openFolderCommand;

        /// <summary>
        /// Gets the OpenFolderCommand.
        /// </summary>
        public RelayCommand OpenFolderCommand
        {
            get
            {
                return _openFolderCommand
                    ?? (_openFolderCommand = new RelayCommand(ExecuteOpenFolderCommand));
            }
        }

        private void ExecuteOpenFolderCommand()
        {
             _addNumber(Open.GetAllFile(Open.ChooseFolder()));
            _showMessage($"总共有{NumberList.Count}条号码！请在开始前执行 去重 操作");
        }

        private RelayCommand _startCommand;

        /// <summary>
        /// Gets the StartCommand.
        /// </summary>
        public RelayCommand StartCommand
        {
            get
            {
                return _startCommand
                    ?? (_startCommand = new RelayCommand(ExecuteStartCommand));
            }
        }

        private void ExecuteStartCommand()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    _insertNumber();
                }
                catch (Exception ex)
                {
                    _showMessage("导入数据库错误！" + ex.Message);
                }
                
            });
        }

        private RelayCommand _clearCommand;

        /// <summary>
        /// Gets the ClearCommand.
        /// </summary>
        public RelayCommand ClearCommand
        {
            get
            {
                return _clearCommand
                    ?? (_clearCommand = new RelayCommand(ExecuteClearCommand));
            }
        }

        private void ExecuteClearCommand()
        {
            for (int i = NumberList.Count - 1; i >= 0; i--)
            {
                if (NumberList[i].Status == ExecuteStatus.Complete)
                {
                    NumberList.RemoveAt(i);
                }
            }
        }

        private RelayCommand _clearAllCommand;

        /// <summary>
        /// Gets the ClearAllCommand.
        /// </summary>
        public RelayCommand ClearAllCommand
        {
            get
            {
                return _clearAllCommand
                    ?? (_clearAllCommand = new RelayCommand(ExecuteClearAllCommand));
            }
        }

        private void ExecuteClearAllCommand()
        {
            NumberList.Clear();
            _showMessage("已清空");
        }

        private RelayCommand _repeatCommand;

        /// <summary>
        /// Gets the RepeatCommand.
        /// </summary>
        public RelayCommand RepeatCommand
        {
            get
            {
                return _repeatCommand
                    ?? (_repeatCommand = new RelayCommand(ExecuteRepeatCommand));
            }
        }

        private void ExecuteRepeatCommand()
        {
            Task.Factory.StartNew(() =>
            {
                var watch = new Stopwatch();
                watch.Start();
                var enumerable = NumberList.ToList().Distinct<NumberItem>(new NumberItemComparer());
                Application.Current.Dispatcher.Invoke(() =>
                {
                    NumberList.Clear();
                });
                foreach (var item in enumerable)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NumberList.Add(item);
                    });
                }
                watch.Stop();
                _showMessage($"去重完成！耗时{watch.Elapsed},剩余{NumberList.Count}条号码！");
            });
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
            Task.Factory.StartNew(() =>
            {
                var file = ExportHelper.GetRandomPath("ImportFailure-");
                var writer = new StreamWriter(file, false, Encoding.UTF8);
                foreach (var information in NumberList)
                {
                    writer.WriteLine(information.Number);
                }
                writer.Close();
                _showMessage($"导出完成！路径：{file}");
            });
        }

        private RelayCommand _exportRealCommand;

        /// <summary>
        /// Gets the ExportRealCommand.
        /// </summary>
        public RelayCommand ExportRealCommand
        {
            get
            {
                return _exportRealCommand
                    ?? (_exportRealCommand = new RelayCommand(ExecuteExportRealCommand));
            }
        }

        private void ExecuteExportRealCommand()
        {
            Task.Factory.StartNew(() =>
            {
                var file = ExportHelper.GetRandomPath("ExportReal-");
                var writer = new StreamWriter(file, false, Encoding.UTF8);
                foreach (var information in NumberList)
                {
                    if (information.Status != ExecuteStatus.Real)
                    {
                        continue;
                    }
                    writer.WriteLine(information.Number);
                }
                writer.Close();
                _showMessage($"导出完成！路径：{file}");
            });
        }

        private RelayCommand<int> _doubleCommand;

        /// <summary>
        /// Gets the DoubleCommand.
        /// </summary>
        public RelayCommand<int> DoubleCommand
        {
            get
            {
                return _doubleCommand
                    ?? (_doubleCommand = new RelayCommand<int>(ExecuteDoubleCommand));
            }
        }

        private void ExecuteDoubleCommand(int index)
        {
            if (index < 0 || index >= NumberList.Count)
            {
                return;
            }
            NumberList[index].Status = NumberList[index].Status == ExecuteStatus.Real ? ExecuteStatus.None : ExecuteStatus.Real;

        }

        private RelayCommand<DragEventArgs> _fileDrogCommand;

        /// <summary>
        /// Gets the FileDrogCommand.
        /// </summary>
        public RelayCommand<DragEventArgs> FileDrogCommand
        {
            get
            {
                return _fileDrogCommand
                    ?? (_fileDrogCommand = new RelayCommand<DragEventArgs>(ExecuteFileDrogCommand));
            }
        }

        private void ExecuteFileDrogCommand(DragEventArgs parameter)
        {
            if (parameter == null)
            {
                return;
            }
            Array files = (System.Array)parameter.Data.GetData(DataFormats.FileDrop);
            Task.Factory.StartNew(() =>
            {
                foreach (string item in files)
                {
                    if (File.Exists(item))
                    {
                        _addNumberByOne(item);
                    }
                    else if (Directory.Exists(item))
                    {
                        _addNumber(Open.GetAllFile(item));
                    }
                }
                _showMessage($"总共有{NumberList.Count}条号码！请在开始前执行 去重 操作");
            });
        }

        private void _addNumber(IList<string> files)
        {
            foreach (var file in files)
            {
                _addNumberByOne(file);
            }
        }

        private void _addNumberByOne(string file)
        {
            var numbers = LocalHelper.GetNumber(file);
            var comparer = new NumberItemComparer();
            foreach (var number in numbers)
            {
                var item = new NumberItem(number);
                //if (NumberList.ToList().Contains<NumberItem>(item, comparer)) continue;
                NumberList.Add(item);
            }
            Message = $"从{file}成功添加{numbers.Count}条号码！";
        }

        private void _insertNumber()
        {
            using (var conn = new MySqlConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                for (var i = 0; i < NumberList.Count; i++)
                {
                    NumberList[i].Status = DatabaseHelper.InsertNumber(conn, NumberList[i].Number) ? ExecuteStatus.Complete : ExecuteStatus.Failure;
                }
            }
            _showMessage("导入数据库完成！");
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


        public override void Cleanup()
        {
            NumberList.Clear();
            base.Cleanup();
        }
    }
}