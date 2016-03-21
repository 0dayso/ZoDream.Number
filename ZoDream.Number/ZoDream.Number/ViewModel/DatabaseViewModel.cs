using System;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ZoDream.Number.Helper;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using MySql.Data.MySqlClient;

namespace ZoDream.Number.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class DatabaseViewModel : ViewModelBase
    {

        private NotificationMessageAction _closeView;
        /// <summary>
        /// Initializes a new instance of the DatabaseViewModel class.
        /// </summary>
        public DatabaseViewModel()
        {
            Messenger.Default.Register<NotificationMessageAction>(this, m=> _closeView = m);
        }


        /// <summary>
        /// The <see cref="Host" /> property's name.
        /// </summary>
        public const string HostPropertyName = "Host";

        private string _host = "127.0.0.1";

        /// <summary>
        /// Sets and gets the Host property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Host
        {
            get
            {
                return _host;
            }
            set
            {
                Set(HostPropertyName, ref _host, value);
            }
        }

        /// <summary>
        /// The <see cref="Port" /> property's name.
        /// </summary>
        public const string PortPropertyName = "Port";

        private int _port = 3306;

        /// <summary>
        /// Sets and gets the Port property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                Set(PortPropertyName, ref _port, value);
            }
        }

        /// <summary>
        /// The <see cref="UserName" /> property's name.
        /// </summary>
        public const string UserNamePropertyName = "UserName";

        private string _userName = "root";

        /// <summary>
        /// Sets and gets the UserName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                Set(UserNamePropertyName, ref _userName, value);
            }
        }

        /// <summary>
        /// The <see cref="Password" /> property's name.
        /// </summary>
        public const string PasswordPropertyName = "Password";

        private string _password = string.Empty;

        /// <summary>
        /// Sets and gets the Password property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                Set(PasswordPropertyName, ref _password, value);
            }
        }

        /// <summary>
        /// The <see cref="CharSet" /> property's name.
        /// </summary>
        public const string CharSetPropertyName = "CharSet";

        private string _charSet = "utf8";

        /// <summary>
        /// Sets and gets the CharSet property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string CharSet
        {
            get
            {
                return _charSet;
            }
            set
            {
                Set(CharSetPropertyName, ref _charSet, value);
            }
        }

        /// <summary>
        /// The <see cref="Database" /> property's name.
        /// </summary>
        public const string DatabasePropertyName = "Database";

        private string _database = "zodream";

        /// <summary>
        /// Sets and gets the Database property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Database
        {
            get
            {
                return _database;
            }
            set
            {
                Set(DatabasePropertyName, ref _database, value);
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

        private RelayCommand _connectCommand;

        /// <summary>
        /// Gets the ConnectCommand.
        /// </summary>
        public RelayCommand ConnectCommand
        {
            get
            {
                return _connectCommand
                    ?? (_connectCommand = new RelayCommand(ExecuteConnectCommand));
            }
        }

        private void ExecuteConnectCommand()
        {
            try
            {
                var connectionString =
                    $"Database = '{Database}'; Data Source = '{Host}'; Port = {Port}; User Id = '{UserName}'; Password = '{Password}'; charset = '{CharSet}'; pooling = true; Allow Zero Datetime = True";
                MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();
                _showMessage("连接成功！");
                conn.Close();
                DatabaseHelper.ConnectionString = connectionString;
                _closeView.Execute();
                //Task.Factory.StartNew(() =>
                //{
                //    Thread.Sleep(1000);
                //    _closeView.Execute();
                //});
            }
            catch (Exception e)
            {
                _showMessage("连接失败！" + e.Message);
            }
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