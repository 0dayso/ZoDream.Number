using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZoDream.Helper.Http;
using ZoDream.Helper.Local;
using ZoDream.Number.Comparer;
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
    public class WeChatViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the WeChatViewModel class.
        /// </summary>
        public WeChatViewModel()
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
        /// The <see cref="Source" /> property's name.
        /// </summary>
        public const string SourcePropertyName = "Source";

        private ImageSource _source = null;

        /// <summary>
        /// Sets and gets the Source property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ImageSource Source
        {
            get
            {
                return _source;
            }
            set
            {
                Set(SourcePropertyName, ref _source, value);
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

        /// <summary>
        /// The <see cref="Code" /> property's name.
        /// </summary>
        public const string CodePropertyName = "Code";

        private string _code = string.Empty;

        /// <summary>
        /// Sets and gets the Code property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                Set(CodePropertyName, ref _code, value);
            }
        }

        private RelayCommand _imageCommand;

        /// <summary>
        /// Gets the ImageCommand.
        /// </summary>
        public RelayCommand ImageCommand
        {
            get
            {
                return _imageCommand
                    ?? (_imageCommand = new RelayCommand(ExecuteImageCommand));
            }
        }

        private void ExecuteImageCommand()
        {
            _getImage();
        }

        private void _getImage()
        {
            //https://weixin110.qq.com/security/verifycode
            var http = new Request();
            using (var response = http.GetResponse("https://weixin110.qq.com/security/verifycode"))
            {
                _cookies = response.Cookies;
                var stream = http.GetMemoryStream(response);
                if (null == stream)
                {
                    return;
                }
                stream.Position = 0;
                BitmapDecoder decoder = JpegBitmapDecoder.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                Source = decoder.Frames[0];
                stream.Close();
            }
        }

        private RelayCommand _enterCommand;

        /// <summary>
        /// Gets the EnterCommand.
        /// </summary>
        public RelayCommand EnterCommand
        {
            get
            {
                return _enterCommand
                    ?? (_enterCommand = new RelayCommand(ExecuteEnterCommand));
            }
        }

        private void ExecuteEnterCommand()
        {
            if (_cookies == null || NumberList.Count <= 0)
            {
                return;
            }
            for (; _index < NumberList.Count; _index++)
            {
                if (NumberList[_index].Status == ExecuteStatus.Complete)
                {
                    continue;
                }
                break;
            }
            var http = new Request($"https://weixin110.qq.com/security/frozen?action=2&lang=zh_CN&step=1&deviceid=&accttype=mobile&acctcc={NumberList[_index].Number}&imgcode={Code}&&callback=jsonp1");
            http.Accept = Accepts.Js;
            http.UserAgent = UserAgents.WeChat;
            http.Referer = "https://weixin110.qq.com/security/frozen?action=2&lang=zh_CN&step=1&deviceid=&";
            http.Cookies = _cookies;
            var html = http.Post("");
            LocalHelper.SaveTxt(html);
            _showMessage("保存成功！");
            //_getImage();
        }

        private int _index = 0;

        private CookieCollection _cookies;

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
            _getImage();
            _index = 0;
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
                    if (information.Status != ExecuteStatus.Complete)
                    {
                        continue;
                    }
                    writer.WriteLine(information.Number);
                }
                writer.Close();
                _showMessage($"已注册的号码导出完成！路径：{file}");
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
            NumberList.Clear();
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