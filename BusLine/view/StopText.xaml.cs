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

namespace BusLine.view
{
    public partial class StopText : UserControl
    {
        public StopText()
        {
            InitializeComponent();
        }

        public delegate void stopNameClick(object sender, RoutedEventArgs e, string stopID);

        public event stopNameClick stopClick;

        public string stopName { get; set; }

        private void stopNameButton_Click(object sender, RoutedEventArgs e)
        {
            if (stopName.Trim() != "")
            {
                stopClick(sender, e, stopName.Trim());
            }
        }
    }
}
