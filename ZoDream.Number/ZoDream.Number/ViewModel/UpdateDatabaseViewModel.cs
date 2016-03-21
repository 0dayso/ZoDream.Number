using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ZoDream.Number.Helper;

namespace ZoDream.Number.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class UpdateDatabaseViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the UpdateDatabaseViewModel class.
        /// </summary>
        public UpdateDatabaseViewModel()
        {
        }

        /// <summary>
        /// The <see cref="Number" /> property's name.
        /// </summary>
        public const string NumberPropertyName = "Number";

        private string _number = string.Empty;

        /// <summary>
        /// Sets and gets the Number property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Number
        {
            get
            {
                return _number;
            }
            set
            {
                Set(NumberPropertyName, ref _number, value);
            }
        }

        /// <summary>
        /// The <see cref="City" /> property's name.
        /// </summary>
        public const string CityPropertyName = "City";

        private string _city = string.Empty;

        /// <summary>
        /// Sets and gets the City property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string City
        {
            get
            {
                return _city;
            }
            set
            {
                Set(CityPropertyName, ref _city, value);
            }
        }

        /// <summary>
        /// The <see cref="Kind" /> property's name.
        /// </summary>
        public const string KindPropertyName = "Kind";

        private string _kind = string.Empty;

        /// <summary>
        /// Sets and gets the Kind property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Kind
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
        /// The <see cref="CityCode" /> property's name.
        /// </summary>
        public const string CityCodePropertyName = "CityCode";

        private string _cityCode = string.Empty;

        /// <summary>
        /// Sets and gets the CityCode property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string CityCode
        {
            get
            {
                return _cityCode;
            }
            set
            {
                Set(CityCodePropertyName, ref _cityCode, value);
            }
        }

        /// <summary>
        /// The <see cref="PostCode" /> property's name.
        /// </summary>
        public const string PostCodePropertyName = "PostCode";

        private string _postCode = string.Empty;

        /// <summary>
        /// Sets and gets the PostCode property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string PostCode
        {
            get
            {
                return _postCode;
            }
            set
            {
                Set(PostCodePropertyName, ref _postCode, value);
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

        private RelayCommand _updateCommand;

        /// <summary>
        /// Gets the UpdateCommand.
        /// </summary>
        public RelayCommand UpdateCommand
        {
            get
            {
                return _updateCommand
                    ?? (_updateCommand = new RelayCommand(ExecuteUpdateCommand));
            }
        }

        private RelayCommand _getNumberCommand;

        /// <summary>
        /// Gets the GetNumberCommand.
        /// </summary>
        public RelayCommand GetNumberCommand
        {
            get
            {
                return _getNumberCommand
                    ?? (_getNumberCommand = new RelayCommand(ExecuteGetNumberCommand));
            }
        }

        private void ExecuteGetNumberCommand()
        {
            if (!Regex.IsMatch(Number, @"^1[34578]\d{5}"))
            {
                _showMessage("您输入的手机号段有误！");
                return;
            }
            var item = DatabaseHelper.GetMobile(Number);
            if (string.IsNullOrWhiteSpace(item.Number))
            {
                _showMessage("数据库中此手机号段不存在请更新！");
                return;
            }
            City = item.City;
            Kind = item.Type;
            CityCode = item.CityCode;
            PostCode = item.PostCode;
            _showMessage("查询完成！");
        }

        private void ExecuteUpdateCommand()
        {
            if (string.IsNullOrWhiteSpace(Number) || string.IsNullOrWhiteSpace(City) || string.IsNullOrWhiteSpace(Kind) ||
                string.IsNullOrWhiteSpace(PostCode) || string.IsNullOrWhiteSpace(CityCode))
            {
                _showMessage("不能为空！");
                return;
            }
            var num = NumberHelper.GetSectionNumber(Number);
            if (string.IsNullOrWhiteSpace(num))
            {
                _showMessage("号段不合要求！");
            }
            DatabaseHelper.InsertMobile(num, City, Kind, CityCode, PostCode);
            _showMessage("更新完成！");
            Number = City = Kind = CityCode = PostCode = string.Empty;
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