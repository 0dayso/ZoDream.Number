using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ZoDream.Number.Helper;
using ZoDream.Number.Model;
using ZoDream.Helper.Local;

namespace ZoDream.Number.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MergeFileViewModel : ViewModelBase
    {

        private List<string> _numberList = new List<string>();
        /// <summary>
        /// Initializes a new instance of the MergeFileViewModel class.
        /// </summary>
        public MergeFileViewModel()
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
        /// The <see cref="FileList" /> property's name.
        /// </summary>
        public const string FileListPropertyName = "FileList";

        private ObservableCollection<FileItem> _fileList = new ObservableCollection<FileItem>();

        /// <summary>
        /// Sets and gets the FileList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<FileItem> FileList
        {
            get
            {
                return _fileList;
            }
            set
            {
                Set(FileListPropertyName, ref _fileList, value);
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
            var files = Open.ChooseFiles();
            foreach (var item in files)
            {
                _addOne(item);
            }
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
            _findDir(Open.ChooseFolder());
        }

        private RelayCommand _visitCommand;

        /// <summary>
        /// Gets the VisitCommand.
        /// </summary>
        public RelayCommand VisitCommand
        {
            get
            {
                return _visitCommand
                    ?? (_visitCommand = new RelayCommand(ExecuteVisitCommand));
            }
        }

        private void ExecuteVisitCommand()
        {
            Open.ExplorePath(AppDomain.CurrentDomain.BaseDirectory + "Mobile");
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
                foreach (var item in FileList)
                {
                    item.Status = ExecuteStatus.Waiting;
                }
                _begin();
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
            FileList.Clear();
            _numberList.Clear();
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
            var files = (System.Array)parameter.Data.GetData(DataFormats.FileDrop);
            //        as FileInfo[];

            foreach (string item in files)
            {
                if (File.Exists(item))
                {
                    _addOne(item);
                }
                else if (Directory.Exists(item))
                {
                    _findDir(item);
                }
            }
        }

        private RelayCommand _exportTxtCommand;

        /// <summary>
        /// Gets the ExportTxtComand.
        /// </summary>
        public RelayCommand ExportTxtComand
        {
            get
            {
                return _exportTxtCommand
                    ?? (_exportTxtCommand = new RelayCommand(ExecuteExportTxtComand));
            }
        }

        private void ExecuteExportTxtComand()
        {
            if (_numberList.Count < 1) return;
            _showMessage("导出文本成功！路径：" + ExportHelper.ExportRandomName(_numberList, "AllNumbers"));
        }

        private RelayCommand _exportVcardCommand;

        /// <summary>
        /// Gets the ExportVcardCommand.
        /// </summary>
        public RelayCommand ExportVcardCommand
        {
            get
            {
                return _exportVcardCommand
                    ?? (_exportVcardCommand = new RelayCommand(ExecuteExportVcardCommand));
            }
        }

        private void ExecuteExportVcardCommand()
        {
            if (_numberList.Count < 1) return;
            _showMessage("导出VCard开始！总共会导出"+ Math.Ceiling((double)_numberList.Count / 5000) +"个文件");
            Task.Factory.StartNew(() =>
            {
                _showMessage("导出VCard成功！路径：" + ExportHelper.ExportVcard(_numberList, "AllNumbers"));
            });
        }

        private RelayCommand _exceptCommand;

        /// <summary>
        /// Gets the ExceptCommand.
        /// </summary>
        public RelayCommand ExceptCommand
        {
            get
            {
                return _exceptCommand
                    ?? (_exceptCommand = new RelayCommand(ExecuteExceptCommand));
            }
        }

        private void ExecuteExceptCommand()
        {
            var numbers = LocalHelper.GetNumber(Open.ChooseFile());
            if (numbers.Count <= 0) return;
            numbers = _numberList.Except(numbers).ToList();
            _showMessage("导出差集文本成功！路径：" + ExportHelper.ExportRandomName(numbers, "ExceptNumbers"));
        }

        /// <summary>
        /// 遍历文件夹
        /// </summary>
        /// <param name="dir"></param>
        private void _findDir(string dir)
        {
            var files = Open.GetAllFile(dir);
            foreach (var item in files)
            {
                _addOne(item);
            }
        }

        /// <summary>
        /// 添加单个文件
        /// </summary>
        /// <param name="path"></param>
        private void _addOne(string path)
        {
            var item = new FileItem(path);
            if (FileList.Contains(item)) return;
            FileList.Add(item);
        }

        private void _begin()
        {
            foreach (var item in FileList)
            {
                if (item.Status == ExecuteStatus.Complete)
                {
                    item.Message = "跳过";
                    break;
                }
                item.Status = ExecuteStatus.Waiting;
                _numberList.AddRange(LocalHelper.GetNumber(item.Path));
                item.Status = ExecuteStatus.Complete;
            }
            _numberList = _numberList.Distinct().ToList();
            _showMessage($"总共合并了{_numberList.Count}个号码!请导出!");
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
            FileList.Clear();
            _numberList.Clear();
            base.Cleanup();
        }
    }
}