using System.ComponentModel;
using System.Text.RegularExpressions;
using GalaSoft.MvvmLight;

namespace ZoDream.Number.Model
{
    public class FileItem : ObservableObject
    {
        public string Name { get; set; }

        public string Kind { get; set; }

        public string Path { get; set; }


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

        private string _message;

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                RaisePropertyChanged("Message");
            }
        }

        public FileItem()
        {

        }

        public FileItem(string path)
        {
            var m = Regex.Match(path, @"\\(?<name>[^\\]+?)\.(?<ext>[^\.\\]+?)$", RegexOptions.RightToLeft);
            Name = m.Groups["name"].Value;
            Kind = m.Groups["ext"].Value;
            Path = path;
        }
    }
}
