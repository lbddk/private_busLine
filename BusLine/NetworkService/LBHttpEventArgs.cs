using System;

namespace LBHttpPost
{
    /// <summary>
    /// Http请求参数类
    /// </summary>
    public class LBHttpEventArgs : EventArgs
    {
        #region 私有成员

        private string _result;
        private bool _is_error;

        #endregion

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public LBHttpEventArgs()
        {
            this.result = "";
            this.isError = false;
        }

        public LBHttpEventArgs(string result)
        {
            this.result = result;
            this.isError = false;
        }

        /// <summary>
        /// 结果字符串
        /// </summary>
        public string result
        {
            get { return _result; }
            set { _result = value; }
        }

        /// <summary>
        /// 是否错误
        /// </summary>
        public bool isError
        {
            get { return _is_error; }
            set { _is_error = value; }
        }
    }
}
