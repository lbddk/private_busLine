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
using System.Collections;

namespace BusLine.NetworkService
{
    public class BaseResult
    {
        #region 私有成员变量
        public string errmsg { get; set; }
        public string status { get; set; }

        public string sversion { get; set; }
        #endregion

        #region 
        public BaseResult()
        {
            this.errmsg = "";
            this.status = "";
            this.sversion = "";
        }
        #endregion
    }
}
