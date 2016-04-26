using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZoDream.Helper.Local;
using ZoDream.Number.Helper;
using ZoDream.Number.Model;

namespace ZoDream.Number.View
{
    /// <summary>
    /// Description for WebView.
    /// </summary>
    public partial class WebView : Window
    {
        private ObservableCollection<string> _numberList = new ObservableCollection<string>();

        /// <summary>
        /// Initializes a new instance of the WebView class.
        /// </summary>
        public WebView()
        {
            InitializeComponent();
            NumberList.ItemsSource = _numberList;
        }

        private void EnterBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigateUrl(UrlTb.Text);
        }

        private void NavigateUrl(string url)
        {
            url = UrlHelper.GetUrl(url, (SearchKind) SearchCb.SelectedIndex);
            UrlTb.Text = url;
            Browser.Navigate(url);
        }

        private void GetNumber()
        {
            //System.IO.StreamReader getReader = new System.IO.StreamReader(Browser.DocumentStream, 
               // System.Text.Encoding.GetEncoding(Browser.Document.Encoding));
            //string html = getReader.ReadToEnd();
            var list = NumberHelper.Get(Browser.Document.Body.OuterHtml);
            foreach (var item in list)
            {
                if (!_numberList.Contains(item))
                {
                    _numberList.Add(item);
                }
            }
            _showMessage($"找到 {list.Count} 条手机号码！");
        }

        private void _showMessage(string message)
        {
            MessageTb.Visibility = Visibility.Visible;
            MessageTb.Text = message;
            Task.Run(() =>
            {
                Thread.Sleep(3000);
                MessageTb.Dispatcher.Invoke(() =>
                {
                    MessageTb.Visibility = Visibility.Collapsed;
                });
            });
        }

        private void UrlTb_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NavigateUrl(UrlTb.Text);
            }
        }

        private void Browser_Navigated(object sender, System.Windows.Forms.WebBrowserNavigatedEventArgs e)
        {
            UrlTb.Text = Browser.Url.ToString();
            Title = Browser.DocumentTitle;
            BeforeBtn.Visibility = Browser.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
            ForwardBtn.Visibility = Browser.CanGoForward ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Browser_NewWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Browser.Url = new System.Uri(((System.Windows.Forms.WebBrowser)sender).StatusText);
            e.Cancel = true;
        }

        private void Browser_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            //将所有的链接的目标，指向本窗体
            foreach (System.Windows.Forms.HtmlElement archor in Browser.Document.Links)
            {
                archor.SetAttribute("target", "_self");
            }

            //将所有的FORM的提交目标，指向本窗体
            foreach (System.Windows.Forms.HtmlElement form in Browser.Document.Forms)
            {
                form.SetAttribute("target", "_self");
            }
            if (AutoCb.IsChecked == true)
            {
                GetNumber();
            }
        }

        private void HomeBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigateUrl("https://www.baidu.com");
        }

        private void BeforeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Browser.CanGoBack)
            {
                Browser.GoBack();
            }
        }

        private void ForwardBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Browser.CanGoForward)
            {
                Browser.GoForward();
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;
            if (item == null) return;
            switch (item.Header.ToString())
            {
                case "查看":
                    Open.ExplorePath(AppDomain.CurrentDomain.BaseDirectory + "Mobile");
                    break;
                case "导出":
                    _showMessage($"导出成功！位置：{ExportHelper.ExportRandomName(_numberList)}");
                    break;
                case "清空":
                    _numberList.Clear();
                    break;
            }
        }

        private void GetBtn_Click(object sender, RoutedEventArgs e)
        {
            GetNumber();
        }

        private void AutoCb_Unchecked(object sender, RoutedEventArgs e)
        {
            GetBtn.IsEnabled = true;
        }

        private void AutoCb_Checked(object sender, RoutedEventArgs e)
        {
            GetBtn.IsEnabled = false;
        }
    }
}