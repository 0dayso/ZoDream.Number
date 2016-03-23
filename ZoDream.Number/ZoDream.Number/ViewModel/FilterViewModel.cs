using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ZoDream.Number.Model;
using System.Windows;
using MySql.Data.MySqlClient;
using ZoDream.Number.Helper;

namespace ZoDream.Number.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class FilterViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the FilterViewModel class.
        /// </summary>
        public FilterViewModel()
        {
            _getRules();
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
        /// The <see cref="SearchKind" /> property's name.
        /// </summary>
        public const string SearchKindPropertyName = "SearchKind";

        private int _searchKind = 0;

        /// <summary>
        /// Sets and gets the SearchKind property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int SearchKind
        {
            get
            {
                return _searchKind;
            }
            set
            {
                Set(SearchKindPropertyName, ref _searchKind, value);
            }
        }


        /// <summary>
        /// The <see cref="NumberList" /> property's name.
        /// </summary>
        public const string NumberListPropertyName = "NumberList";

        private ObservableCollection<NumberInformation> _numberList = new ObservableCollection<NumberInformation>();

        /// <summary>
        /// Sets and gets the NumberList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<NumberInformation> NumberList
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
        /// The <see cref="NumberRules" /> property's name.
        /// </summary>
        public const string NumberRulesPropertyName = "NumberRules";

        private ObservableCollection<NumberRule> _numberRules = new ObservableCollection<NumberRule>();

        /// <summary>
        /// Sets and gets the NumberRules property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<NumberRule> NumberRules
        {
            get
            {
                return _numberRules;
            }
            set
            {
                Set(NumberRulesPropertyName, ref _numberRules, value);
            }
        }

        /// <summary>
        /// The <see cref="Pattern" /> property's name.
        /// </summary>
        public const string PatternPropertyName = "Pattern";

        private string _pattern = string.Empty;

        /// <summary>
        /// Sets and gets the Pattern property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Pattern
        {
            get
            {
                return _pattern;
            }
            set
            {
                Set(PatternPropertyName, ref _pattern, value);
            }
        }

        private RelayCommand _searchCommand;

        /// <summary>
        /// Gets the SearchCommand.
        /// </summary>
        public RelayCommand SearchCommand
        {
            get
            {
                return _searchCommand
                    ?? (_searchCommand = new RelayCommand(ExecuteSearchCommand));
            }
        }

        private void ExecuteSearchCommand()
        {
            switch (SearchKind)
            {
                case 0:
                    _getNumberByRules(Pattern);
                    break;
                case 1:
                    _getNumberByCity("m.city LIKE ?city", new MySqlParameter("?city", $"%{Pattern}%"));
                    break;
                case 2:
                    _getNumberByCity("m.type LIKE ?type", new MySqlParameter("?type", $"%{Pattern}%"));
                    break;
                case 3:
                    _getNumberByCity("n.number LIKE ?num", new MySqlParameter("?num", $"%{Pattern}"));
                    break;
                case 4:
                    _getNumberByCity("n.number LIKE ?num", new MySqlParameter("?num", $"{Pattern}%"));
                    break;
                case 5:
                    _getNumberByCity("n.number LIKE ?num", new MySqlParameter("?num", $"%{Pattern}%"));
                    break;
            }
        }

        private RelayCommand<NumberRule> _selectionChangedCommand;

        /// <summary>
        /// Gets the SelectionChangedCommand.
        /// </summary>
        public RelayCommand<NumberRule> SelectionChangedCommand
        {
            get
            {
                return _selectionChangedCommand
                    ?? (_selectionChangedCommand = new RelayCommand<NumberRule>(ExecuteSelectionChangedCommand));
            }
        }

        private void ExecuteSelectionChangedCommand(NumberRule parameter)
        {
            if (SearchKind != 0 || parameter == null)
            {
                _showMessage("下拉选项的规则只使用于手机号");
                return;
            }
            _getNumberByRules(parameter.Pattern);
        }

        private RelayCommand _exportNumberCommand;

        /// <summary>
        /// Gets the ExportNumberCommand.
        /// </summary>
        public RelayCommand ExportNumberCommand
        {
            get
            {
                return _exportNumberCommand
                    ?? (_exportNumberCommand = new RelayCommand(ExecuteExportNumberCommand));
            }
        }

        private void ExecuteExportNumberCommand()
        {
            Task.Factory.StartNew(() =>
            {
                var file = ExportHelper.GetRandomPath("NumblerRules-");
                var writer = new StreamWriter(file, false, Encoding.UTF8);
                foreach (var information in NumberList)
                {
                    writer.WriteLine(information.Number);
                }
                writer.Close();
                _showMessage($"导出完成！路径：{file}");
            });
        }

        private RelayCommand _exportAllCommand;

        /// <summary>
        /// Gets the ExportAllCommand.
        /// </summary>
        public RelayCommand ExportAllCommand
        {
            get
            {
                return _exportAllCommand
                    ?? (_exportAllCommand = new RelayCommand(ExecuteExportAllCommand));
            }
        }

        private void ExecuteExportAllCommand()
        {
            Task.Factory.StartNew(() =>
            {
                var file = ExportHelper.GetRandomPath("NumblerRules-");
                var writer = new StreamWriter(file, false, Encoding.UTF8);
                foreach (var information in NumberList)
                {
                    writer.WriteLine($"{information.Number} {information.Section} {information.City} {information.Type} {information.CityCode} {information.PostCode} ");
                }
                writer.Close();
                _showMessage($"导出完成！路径：{file}");
            });
        }

        private void _getRules()
        {
            var streamResourceInfo = Application.GetResourceStream(new Uri("/Rules.txt", UriKind.Relative));
            if (streamResourceInfo != null)
            {
                var sr = new StreamReader(streamResourceInfo.Stream);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    NumberRules.Add(new NumberRule(line));
                }
                sr.Close();
            }
        }

        private void _getNumberByCity(string where, params MySqlParameter[] commandParameters)
        {
            Message = "查询开始。。。";
            NumberList.Clear();
            Task.Factory.StartNew(() =>
            {
                var watch = new Stopwatch();
                watch.Start();
                using (var conn = new MySqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    var command = new MySqlCommand($"SELECT n.number,m.number,m.city,m.type,m.city_code,m.postcode FROM `zd_number` n LEFT JOIN zd_mobile m ON n.mobile_id = m.id WHERE {where};", conn);
                    foreach (var parm in commandParameters)
                    {
                        command.Parameters.Add(parm);
                    }
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader.HasRows)
                        {
                            var item = new NumberInformation(
                                reader.GetString(0),
                                reader.GetString(1),
                                reader.GetString(2),
                                reader.GetString(3),
                                reader.GetString(4),
                                reader.GetString(5)
                            );
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                NumberList.Add(item);
                            });
                        }
                    }
                    reader.Close();
                }
                watch.Stop();
                _showMessage($"运行{watch.Elapsed},获得{NumberList.Count}条数据");
            });
        }

        /// <summary>
        /// 获取全部结果进行正则匹配
        /// </summary>
        /// <param name="rule"></param>
        private void _getNumberByRules(string rule)
        {
            Message = "查询开始。。。";
            NumberList.Clear();
            Task.Factory.StartNew(() =>
            {
                var watch = new Stopwatch();
                watch.Start();
                using (var conn = new MySqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    var command = new MySqlCommand("SELECT n.number,m.number,m.city,m.type,m.city_code,m.postcode FROM `zd_number` n LEFT JOIN zd_mobile m ON n.mobile_id = m.id;", conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader.HasRows)
                        {
                            string number = reader.GetString(0);
                            if (Regex.IsMatch(number, rule))
                            {
                                var item = new NumberInformation(number,
                                    reader.GetString(1),
                                    reader.GetString(2),
                                    reader.GetString(3),
                                    reader.GetString(4),
                                    reader.GetString(5)
                                    );
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    NumberList.Add(item);
                                });
                            }
                        }
                    }
                    reader.Close();
                }
                watch.Stop();
                _showMessage($"运行{watch.Elapsed},获得{NumberList.Count}条数据");
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