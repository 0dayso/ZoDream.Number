using GalaSoft.MvvmLight;

namespace ZoDream.Number.Model
{
    public class UrlItem: ObservableObject
    {
        public string Url { get; set; }

        private UrlStatus _status;

        /// <summary>
        /// Sets and gets the Status property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public UrlStatus Status
        {
            get
            {
                return _status;
            }

            set
            {
                if (_status == value)
                {
                    return;
                }

                _status = value;
                RaisePropertyChanged("Status");
            }
        }

        public int Depth { get; set; }

        public UrlItem()
        {
            
        }

        public UrlItem(string url)
        {
            Url = url;
        }

        public UrlItem(string url, int depth)
        {
            Depth = depth;
            Url = url;
        }


        public UrlItem(string url, int depth, UrlStatus status)
        {
            Depth = depth;
            Url = url;
            _status = status;
        }
    }

    public enum UrlStatus
    {
        None,
        Waiting,
        Completed,
        Failure
    }
}
