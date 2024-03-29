﻿using System;
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

    public class BusStopLineData
    {
        public List<Lines> lines { get; set; }
        public StopDetail stopdetail { get; set; }
    }

    public class BusStopLineResult : BaseResult
    {
        public BusStopLineData data;
    }
}
