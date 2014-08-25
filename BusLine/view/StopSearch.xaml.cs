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
    public partial class StopSearch : UserControl
    {
        public StopSearch()
        {
            InitializeComponent();
            lineList = new List<string>();
            start = false;
        }

        public delegate void listPickerSelectionChanged(object sender, SelectionChangedEventArgs e);

        public event listPickerSelectionChanged selectionChanged;

        public List<string> lineList { get; set; }
        public bool start { get; set; }

        public void lineListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (start)
            {
                selectionChanged(sender, e);
            }
        }


    }
}
