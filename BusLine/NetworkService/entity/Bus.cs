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
    public class Bus
    {
        public int arrived { get; set; }//是否抵达
        public string carNo { get; set; }
        public double jingdu { get; set; }
        public int lineId { get; set; }
        public int order { get; set; }
        public string stopId { get; set; }        
        public double weidu { get; set; }
    }
}
