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
using System.Collections.Generic;

using BusLine.NetworkService.entity;

namespace BusLine.NetworkService.result
{
    public class BusLineData
    {
        public List<Bus> busList { get; set; }
        public Lines line { get; set; }
        public List<Map> stations { get; set; }
    }

    public class BusLineResult : BaseResult
    {
        public BusLineData data;
    }
}
