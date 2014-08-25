using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BusLine.NetworkService.entity;
using System.Collections.Generic;

namespace BusLine.NetworkService.result
{

    public class BusStopSearchData
    {
        public List<StopList> stoplist { get; set; }
    }

    public class BusStopSearchResult : BaseResult
    {
        public BusStopSearchData data;
    }
}
