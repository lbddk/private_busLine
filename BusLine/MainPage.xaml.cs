using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using LBHttpPost;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using BusLine.NetworkService;
using BusLine.NetworkService.entity;
using BusLine.NetworkService.result;
using BusLine.view;

namespace BusLine
{
    public partial class MainPage : PhoneApplicationPage
    {
        private static string operStr;//记录当前操作
        private static int tempNum;

        public delegate void oppositeClickDelegate();

        public event oppositeClickDelegate oppositeClick;
        // 构造函数
        public MainPage()
        {
            InitializeComponent();
        }

        IList<Object> viewList = new List<Object>();
        BusStopSearchResult StopSearchResult = new BusStopSearchResult();

        #region 用户操作事件

        private void busLine_GotFocus(object sender, RoutedEventArgs e)
        {
            if (((TextBox)sender).Text == "请输入线路")
            {
                ((TextBox)sender).Text = "";
            }
        }

        private void busStop_GotFocus(object sender, RoutedEventArgs e)
        {
            if (((TextBox)sender).Text == "请输入站点")
            {
                ((TextBox)sender).Text = "";
            }
        }

        private void busLineButton_Click(object sender, RoutedEventArgs e)
        {
            operStr = "SearchBus";
            busLineSearch(busLine.Text,"0");
        }

        private void busLineSearch(string lineNo, string direction)
        {
            Random rd = new Random();
            viewList = new List<Object>();
            LBHttpRequest request = new LBHttpRequest();
            request.httpSuccess += new LBHttpRequest.httpResquestEventHandler(request_httpSuccess);
            request.httpFaild += new LBHttpRequest.httpResquestEventHandler(request_httpFaild);
            request.httpError += new LBHttpRequest.httpResquestEventHandler(request_httpError);
            request.requestUrl = "/bus/datasource/query.do";
            request.appendParameter("Type", "LineDetail");
            request.appendParameter("lineNo", lineNo);
            request.appendParameter("direction", direction);
            request.appendParameter("Para", rd.Next().ToString());
            request.responseType = typeof(BusLineResult);
            request.requestMethod = requestType.POST;
            request.request();
            indicator.Text = "加载中";
            indicator.IsIndeterminate = true;
            indicator.IsVisible = true;
        }

        private void busStopButton_Click(object sender, RoutedEventArgs e)
        {
            operStr = "SearchLine";
            busStopSearch(busStop.Text);
        }

        private void busStopSearch(string busStop)
        {
            operStr = "SearchLine";
            viewList = new List<Object>();
            LBHttpRequest request = new LBHttpRequest();            
            request.httpSuccess += new LBHttpRequest.httpResquestEventHandler(request_httpSuccess);
            request.httpFaild += new LBHttpRequest.httpResquestEventHandler(request_httpFaild);
            request.httpError += new LBHttpRequest.httpResquestEventHandler(request_httpError);
            request.requestUrl = "/bus/stop!stoplist.action";
            request.appendParameter("stopName", busStop);
            request.responseType = typeof(BusStopLineResult);
            request.requestMethod = requestType.POST;
            request.request();
            indicator.Text = "加载中";
            indicator.IsIndeterminate = true;
            indicator.IsVisible = true;
        }

        #endregion

        #region 请求事件
        void request_httpSuccess(object sender, BaseResult baseResult)
        {
            try
            {
                if (baseResult.GetType() == typeof(BusLineResult))
                {
                    BusLineResult result = baseResult as BusLineResult;
                    Dictionary<string, int> dic = new Dictionary<string, int>();
                    int getCount;
                    foreach (Bus bus in result.data.busList)
                    {
                        getCount=0;
                        if (dic.ContainsKey(bus.stopId+bus.arrived))
                        {
                            if (dic.TryGetValue(bus.stopId + bus.arrived, out getCount))
                                dic[bus.stopId + bus.arrived] = getCount + 1;
                        }
                        else
                            dic.Add(bus.stopId + bus.arrived, 1);
                    }
                    bool isFirst = true;
                    foreach (Map map in result.data.stations)
                    {
                        StopText stopText = new StopText();
                        stopText.stopNameButton.Content = map.stopName;
                        stopText.stopName = map.stopName;
                        StopImage stopImage = new StopImage();
                        if (isFirst)
                        {
                            oppositeButton.Visibility = Visibility.Visible;
                            this.oppositeClick += MainPage_oppositeClick;
                            isFirst = false;
                        }
                        foreach (Bus bus in result.data.busList)
                        {
                            if (map.stopId==bus.stopId)
                            {
                                if (bus.arrived == 1)
                                {
                                    stopText.busImage.Visibility = Visibility.Visible;
                                    stopText.busNum.Text = dic[map.stopNo + bus.arrived].ToString();
                                    stopText.busNum.Visibility = Visibility.Visible;
                                    stopText.stopNameButton.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 146, 206));
                                }
                                else
                                {
                                    stopImage.busImage.Visibility = Visibility.Visible;
                                    stopImage.busNum.Text = dic[map.stopNo + bus.arrived].ToString();
                                    stopImage.busNum.Visibility = Visibility.Visible;
                                }
                            }
                        }
                        stopText.stopClick += new StopText.stopNameClick(stopTextClick);
                        viewList.Add(stopImage);
                        viewList.Add(stopText);
                    }
                    resultListBox.ItemsSource = viewList;
                    resultListBox.UpdateLayout();
                    resultListBox.ScrollIntoView(resultListBox.Items.First());
                    indicator.IsVisible = false;
                    tempNum = 0;
                }
                else if (baseResult.GetType() == typeof(BusStopSearchResult))
                {
                    viewList = new List<Object>();
                    StopSearchResult = baseResult as BusStopSearchResult;
                    sendBusStopSearchRequest();
                }
                else if (baseResult.GetType() == typeof(BusStopLineResult))
                {
                    oppositeButton.Visibility = Visibility.Collapsed;
                    BusStopLineResult result= baseResult as BusStopLineResult;
                    StopSearch stopSearch = new StopSearch();
                    stopSearch.selectionChanged += new StopSearch.listPickerSelectionChanged(lineSelectionChanged);
                    stopSearch.titleTextBlock.Text="途经\""+result.data.stopdetail.stopName+"\"的公交线路有：";
                    List<string> lineList = new List<string>();
                    lineList.Add("_请选择线路_");
                    foreach( Lines line in result.data.lines)
                    {
                        if (!lineList.Contains(line.lineName))
                        {
                            lineList.Add(line.lineName);
                            stopSearch.linesTextBlock.Text += line.lineName + "路，";
                        }
                    }
                    lineList.Sort();
                    stopSearch.lineList = lineList;
                    stopSearch.lineListPicker.DataContext = stopSearch.lineList;
                    stopSearch.start = true;
                    viewList.Add(stopSearch);
                    sendBusStopSearchRequest();
                }
            }
            catch
            {
                this.request_httpError(sender, baseResult);
            }
        }

        void MainPage_oppositeClick()
        {
            if (tempNum<1)
            { 
                oppositeLineSearch();
                tempNum++;
            }
        }

        void request_httpFaild(object sender, BaseResult baseResult)
        {
            MessageBox.Show("连接服务器失败，请打开数据连接");
            indicator.IsVisible = false;
        }

        void request_httpError(object sender, BaseResult baseResult)
        {
            MessageBox.Show("输入有误");
            indicator.IsVisible = false;
        }
        #endregion

        #region
        public void sendBusStopSearchRequest()
        {
            LBHttpRequest request = new LBHttpRequest();
            request.httpSuccess += new LBHttpRequest.httpResquestEventHandler(request_httpSuccess);
            request.httpFaild += new LBHttpRequest.httpResquestEventHandler(request_httpFaild);
            request.httpError += new LBHttpRequest.httpResquestEventHandler(request_httpError);
            request.requestUrl = "/bus/stop!stoplist.action";
            request.responseType = typeof(BusStopLineResult);
            request.requestMethod = requestType.GET;

            if (StopSearchResult.data!=null && StopSearchResult.data.stoplist.Count > 0)
            {
                StopList list = StopSearchResult.data.stoplist.First<StopList>();
                request.appendParameter("stopId", list.stopId);
                request.request();
                StopSearchResult.data.stoplist.RemoveAt(0);
            }
            else
            {
                resultListBox.ItemsSource = viewList;
                indicator.IsVisible = false;
            }
        }
        #endregion

        private void resultListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (resultListBox.SelectedIndex > -1 && resultListBox.SelectedItem.GetType() == typeof(StopSearch))
            {
                ((StopSearch)resultListBox.SelectedItem).lineListPicker.Open();
                resultListBox.SelectedIndex = -1;
            }
        }

        public void lineSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListPicker)sender).SelectedIndex > -1)
            {
                busLine.Text = (string)((ListPicker)sender).SelectedItem;
                busLineSearch((string)((ListPicker)sender).SelectedItem,"0");
            }
        }

        public void stopTextClick(object sender, RoutedEventArgs e, string stopName)
        {
            operStr = "SearchLine";
            viewList = new List<Object>();
            LBHttpRequest request = new LBHttpRequest();
            request.httpSuccess += new LBHttpRequest.httpResquestEventHandler(request_httpSuccess);
            request.httpFaild += new LBHttpRequest.httpResquestEventHandler(request_httpFaild);
            request.httpError += new LBHttpRequest.httpResquestEventHandler(request_httpError);
            request.requestUrl = "/bus/stop!stoplist.action";
            request.appendParameter("stopName", stopName);
            request.responseType = typeof(BusStopLineResult);
            request.requestMethod = requestType.POST;
            request.request();
            busStop.Text = stopName;
            indicator.IsVisible = true;
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("本软件数据由第三方提供，仅供大家参考使用！由于没有UI，软件美观只能这样了。本功能主要针对快速查看公交车实时位置，以便您不错过任何一班武汉公交！相关问题和bug请联系扣扣:278792804");
        }

        private void oppositeLineSearch()
        {
            if (paraDirection.Text== "1")
                paraDirection.Text = "0";
            else
                paraDirection.Text = "1";
            busLineSearch(busLine.Text, paraDirection.Text);
        }

        private void oppositeButton_Click(object sender, RoutedEventArgs e)
        {
            oppositeClick();
        }

        private void barButtonRefreshList_Click(object sender, EventArgs e)
        {
            switch (operStr)
            {
                case "SearchBus":
                    busLineSearch(busLine.Text, paraDirection.Text);
                    break;
                case "SearchLine":
                    busStopSearch(busStop.Text);
                    break;
                default:
                    break;
            }
        }

    }
}