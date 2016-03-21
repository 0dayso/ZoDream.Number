using System.ComponentModel;
using System.Text.RegularExpressions;
using GalaSoft.MvvmLight;

namespace ZoDream.Number.Model
{
    public class NumberItem : ObservableObject
    {
        public string Number { get; set; }

        private ExecuteStatus _status;

        public ExecuteStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                RaisePropertyChanged("Status");
            }
        }

        public NumberItem()
        {
            
        }

        public NumberItem(string number)
        {
            Number = number;
        }
    }
}
