using System.Text.RegularExpressions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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
    public class SearchViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the SearchViewModel class.
        /// </summary>
        public SearchViewModel()
        {
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
            if (!Regex.IsMatch(Number, @"^1[34578]\d{9}$"))
            {
                Message = "您输入的手机号有误！";
                return;
            }
            Message = Kind == 1 ? HttpHelper.Get(Number).ToString() : LocalHelper.Get(Number).ToString();

        }
    }
}