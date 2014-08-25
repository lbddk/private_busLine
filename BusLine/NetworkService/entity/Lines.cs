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

namespace BusLine.NetworkService.entity
{
    public class Lines
    {
        public int direction { get; set; }
        public string endStopName { get; set; }

        public int leftStop { get; set; }
        public int leftStopNum { get; set; }
        public string nextStop { get; set; }

        public int proTime { get; set; }
        public DateTime firstTime { get; set; }
        public DateTime lastTime { get; set; }
        public string lineName { get; set; }
        public string lineNo { get; set; }
        public string startStopName { get; set; }
        public int stopsNum { get; set; }
        public bool opposite { get; set; }    //不是环线
    }
}
