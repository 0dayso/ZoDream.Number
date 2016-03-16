using System;
using System.Collections.ObjectModel;
using System.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ZoDream.Number.Helper;
using ZoDream.Number.Model;
using ZoDream.Number.Helper.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ZoDream.Number.Comparer;

namespace ZoDream.Number.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SpiderViewModel : ViewModelBase,IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the SpiderViewModel class.
        /// </summary>
        public SpiderViewModel()
        {
        }

        /// <summary>
        /// The <see cref="BaseUrl" /> property's name.
        /// </summary>
        public const string BaseUrlPropertyName = "BaseUrl";

        private string _baseUrl = "https://www.baidu.com";

        /// <summary>
        /// Sets and gets the BaseUrl property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string BaseUrl
        {
            get
            {
                return _baseUrl;
            }
            set
            {
                Set(BaseUrlPropertyName, ref _baseUrl, value);
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
        /// The <see cref="Depth" /> property's name.
        /// </summary>
        public const string DepthPropertyName = "Depth";

        private int _depth = 0;

        /// <summary>
        /// Sets and gets the Depth property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int Depth
        {
            get
            {
                return _depth;
            }
            set
            {
                Set(DepthPropertyName, ref _depth, value);
            }
        }

        /// <summary>
        /// The <see cref="Space" /> property's name.
        /// </summary>
        public const string SpacePropertyName = "Space";

        private int _space = 20;

        /// <summary>
        /// Sets and gets the Space property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int Space
        {
            get
            {
                return _space;
            }
            set
            {
                if (value < 1)
                {
                    value = 20;
                }
                Set(SpacePropertyName, ref _space, value);
            }
        }

        /// <summary>
        /// The <see cref="Count" /> property's name.
        /// </summary>
        public const string CountPropertyName = "Count";

        private int _count = 20;

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
                if (value < 1)
                {
                    value = 20;
                }
                Set(CountPropertyName, ref _count, value);
            }
        }


        /// <summary>
        /// The <see cref="UrlsList" /> property's name.
        /// </summary>
        public const string UrlsListPropertyName = "UrlsList";

        private ObservableCollection<UrlItem> _urlsList = new ObservableCollection<UrlItem>();

        /// <summary>
        /// Sets and gets the UrlsList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<UrlItem> UrlsList
        {
            get
            {
                return _urlsList;
            }
            set
            {
                Set(UrlsListPropertyName, ref _urlsList, value);
            }
        }

        private CancellationTokenSource _tokenSource;

        private readonly object _object = new object();

        private List<string> _numberList = new List<string>();

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
            if (UrlsList.Count == 0)
            {
                UrlsList.Add(new UrlItem(UrlHelper.GetUrl(BaseUrl)));
            }
            _init();
            #region 创造主线程，去分配多个下载线程
            _tokenSource = new CancellationTokenSource();
            var token = _tokenSource.Token;
            Task.Factory.StartNew(() =>
            {
                var index = 0;
                while (!token.IsCancellationRequested)
                {
                    #region 创建执行下载的线程数组
                    var tasksLength = Math.Min(Count, UrlsList.Count - index);
                    var tasks = new Task[tasksLength];
                    for (var i = 0; i < tasksLength; i++)
                    {
                        var index1 = index;
                        tasks[i] = new Task(() =>
                        {
                            _begin(UrlsList[index1]);
                        });
                        index ++;
                    }
                    #endregion

                    #region 监视线程数组完成
                    var continuation =  Task.Factory.ContinueWhenAll(tasks, (task) =>
                    { }, token);
                    foreach (var task in tasks)
                    {
                        task.Start();
                    }
                    while (!continuation.IsCompleted)
                    {
                        Thread.Sleep(1000);
                    }
                    #endregion
                    _changedStatus(index - tasksLength, tasksLength, UrlStatus.Completed);
                    if (index >= UrlsList.Count)
                    {
                        _tokenSource.Cancel();
                        break;
                    }
                }
            }, token);
            #endregion
            #region 计时器 做自动导出操作
            var token1 = _tokenSource.Token;
            Task.Factory.StartNew(() =>
            {
                var time = Space;
                while (!token1.IsCancellationRequested)
                {
                    Thread.Sleep(10000);
                    time --;
                    if (time > 0 && !token1.IsCancellationRequested) continue;
                    ExportHelper.Export(_numberList);
                    lock (_object)
                    {
                        _numberList.Clear();
                    }
                    time = Space;
                }
            }, token1);
            #endregion
        }

        /// <summary>
        /// 初始化 把url的状态设为等待
        /// </summary>
        private void _init()
        {
            foreach (var item in UrlsList)
            {
                item.Status = UrlStatus.Waiting;
            }
        }

        /// <summary>
        /// 改变状态
        /// </summary>
        /// <param name="index">初始长度</param>
        /// <param name="length"></param>
        /// <param name="status"></param>
        private void _changedStatus(int index,int length, UrlStatus status)
        {
            lock (_object)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    for (var i = 0; i < length; i++)
                    {
                        UrlsList[index + i].Status = status;
                    }
                });
                
            }
        }
        /// <summary>
        /// 开始下载提取
        /// </summary>
        /// <param name="item"></param>
        private void _begin(UrlItem item)
        {
            var request = new Request();
            var html = request.Get(item.Url);
            var helper = new NumberHelper();
            var lists = helper.GetNumber(html);
            #region 控制深度
            if (item.Depth < Depth || Depth <= 0)
            {
                var htmlHelper = new Html(html);
                var links = htmlHelper.GetLinks(item.Url);
                foreach (var newItem in from link in links let newItem = new UrlItem(link, item.Depth + 1, UrlStatus.Waiting) where !UrlsList.Contains<UrlItem>(newItem, new UrlItemComparer()) && (Kind != 1 || htmlHelper.Comparer(item.Url, link)) select newItem)
                {
                    lock (_object)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            UrlsList.Add(newItem);
                        });
                    }
                }
            }
            #endregion
            lock (_object)
            {
                _numberList.AddRange(lists);
                _numberList = _numberList.Distinct().ToList();
            }
        }

        private RelayCommand _stopCommand;

        /// <summary>
        /// Gets the StopCommand.
        /// </summary>
        public RelayCommand StopCommand
        {
            get
            {
                return _stopCommand
                    ?? (_stopCommand = new RelayCommand(ExecuteStopCommand));
            }
        }

        private void ExecuteStopCommand()
        {
            _tokenSource.Cancel();
            foreach (var item in UrlsList.Where(item => item.Status == UrlStatus.Waiting))
            {
                item.Status = UrlStatus.None;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (_tokenSource == null) return;
            _tokenSource.Cancel();
            _tokenSource.Dispose();
            _tokenSource = null;
        }

        ~SpiderViewModel()
        {
            Dispose(false);
        }
    }
}