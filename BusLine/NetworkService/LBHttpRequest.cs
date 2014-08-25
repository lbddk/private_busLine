using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Text;
using BusLine.NetworkService;
using System.Runtime.Serialization.Json;
using BusLine.NetworkService.result;

namespace LBHttpPost
{
    /// <summary>
    /// winphone7的HTTP封装类
    /// </summary>
    public class LBHttpRequest
    {
        #region 私有成员

        private string _request_url = null;
        private requestType _request_type;
        IDictionary<string, string> _parameter;
        private Type _response_type;

        #endregion

        /// <summary>
        /// Http请求指代
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">发送所带的参数</param>
        public delegate void httpResquestEventHandler(object sender, BaseResult result);

        /// <summary>
        /// Http请求完成事件
        /// </summary>
        public event httpResquestEventHandler httpSuccess;
        public event httpResquestEventHandler httpFaild;
        public event httpResquestEventHandler httpError;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <remarks>
        /// 默认的请求方式的GET
        /// </remarks>
        public LBHttpRequest()
        {
            _request_url = "";
            _parameter = new Dictionary<string, string>();
            _request_type = requestType.GET; //默认请求方式为GET方式
            _response_type = typeof(BaseResult);
        }

        /// <summary>
        /// 追加就参数
        /// </summary>
        /// <param name="key">进行追加的键</param>
        /// <param name="value">键对应的值</param>
        public void appendParameter(string key, string value)
        {
            _parameter.Add(key, value);
        }

        /// <summary>
        /// 触发HTTP请求完成方法
        /// </summary>
        /// <param name="e">事件参数</param>
        public void OnHttpCompleted(LBHttpEventArgs e)
        {
            try
            {
                string j = e.result.Trim();
                if (_response_type == typeof(BusStopSearchResult))
                {
                    BaseResult baseResult = (BaseResult)Activator.CreateInstance(typeof(BusStopLineResult));
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(BusStopLineResult));
                    using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(j.Substring(0, j.Length - 1))))
                    {
                        baseResult = (BaseResult)ser.ReadObject(ms);
                    }
                    if (((BusStopLineResult)baseResult).data == null || ((BusStopLineResult)baseResult).data.lines==null)
                    {
                        baseResult = (BaseResult)Activator.CreateInstance(_response_type);
                        ser = new DataContractJsonSerializer(_response_type);
                        using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(j.Substring(0, j.Length - 1))))
                        {
                            baseResult = (BaseResult)ser.ReadObject(ms);
                        }
                    }
                    if (baseResult.status.Trim() == "1"||baseResult.status.Trim()=="00")
                    {
                        if (this.httpSuccess != null)
                        {
                            this.httpSuccess(this, baseResult);
                        }
                    }
                    else
                    {
                        if (this.httpFaild != null)
                        {
                            this.httpError(this, baseResult);
                        }
                    }
                }
                else
                {
                    BaseResult baseResult = (BaseResult)Activator.CreateInstance(_response_type);
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(_response_type);
                    if (!j.Contains("{\"jsonr\":"))
                    {
                        using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(j)))
                        {
                            baseResult = (BaseResult)ser.ReadObject(ms);
                        }
                    }
                    else
                    {
                        j = j.Replace("{\"jsonr\":","");
                        using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(j.Substring(0, j.Length - 1))))
                        {
                            baseResult = (BaseResult)ser.ReadObject(ms);
                        }
                    }
                    if (baseResult.status.Trim() == "1" || baseResult.status.Trim() == "00")
                    {
                        if (this.httpSuccess != null)
                        {
                            this.httpSuccess(this, baseResult);
                        }
                    }
                    else
                    {
                        if (this.httpFaild != null)
                        {
                            this.httpError(this, baseResult);
                        }
                    }
                }
                
            }
            catch(Exception ex)
            {
                if (this.httpFaild != null)
                {
                    this.httpFaild(this, null);
                }
            }
        }

        /// <summary>
        /// 请求URL地址
        /// </summary>
        public string requestUrl
        {
            get { return _request_url; }
            set { _request_url = value; }
        }

        /// <summary>
        /// 请求方式
        /// </summary>
        public requestType requestMethod
        {
            get { return _request_type; }
            set { _request_type = value; }
        }

        /// <summary>
        /// 返回类型
        /// </summary>
        public Type responseType
        {
            get { return _response_type; }
            set { _response_type = value; }
        }

        /// <summary>
        /// 进行HTTP请求
        /// </summary>
        public void request()
        {
            if (this.requestUrl != null)
            {
                this.requestUrl = Constants.serverUrl + requestUrl;
            }
            if (this.requestMethod == requestType.GET)
            {
                this.getRequest();
            }
            else
            {
                this.postRequest();
            }
        }

        /// <summary>
        /// HTTP方式的GET请求
        /// </summary>
        /// <returns></returns>
        private void getRequest()
        {
            string strrequesturl = this.requestUrl;
            string parastring = this.getParemeterString();
            if (parastring.Length > 0)
            {
                strrequesturl += "?" + parastring;
            }
            Uri myurl = new Uri(strrequesturl);
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(myurl);
            webRequest.Method = "GET";
            webRequest.BeginGetResponse(new AsyncCallback(handleResponse), webRequest); //直接获取响应
            _parameter.Clear(); //清空参数列表
        }

        /// <summary>
        /// HTTP的POST请求
        /// </summary>
        /// <returns></returns>
        private void postRequest()
        {
            Uri myurl = new Uri(this.requestUrl);
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(myurl);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            webRequest.BeginGetRequestStream(new AsyncCallback(handlePostReady), webRequest);
        }

        /// <summary>
        /// 获取传递参数的字符串
        /// </summary>
        /// <returns>字符串</returns>
        private string getParemeterString()
        {
            string result = "";
            StringBuilder sb = new StringBuilder();
            bool hasParameter = false;
            string value = "";
            foreach (var item in _parameter)
            {
                if (!hasParameter)
                    hasParameter = true;
                value = UrlEncoder.Encode(item.Value); //对传递的字符串进行编码操作
                sb.Append(string.Format("{0}={1}&", item.Key, value));
            }
            if (hasParameter)
            {
                result = sb.ToString();
                int len = result.Length;
                result = result.Substring(0, --len); //将字符串尾的‘&’去掉
            }
            return result;

        }

        /// <summary>
        /// 异步请求回调函数
        /// </summary>
        /// <param name="asyncResult">异步请求参数</param>
        private void handlePostReady(IAsyncResult asyncResult)
        {
            HttpWebRequest webRequest = asyncResult.AsyncState as HttpWebRequest;
            using (Stream stream = webRequest.EndGetRequestStream(asyncResult))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    string parameterstring = this.getParemeterString();
                    if (parameterstring.Length > 0)
                    {
                        writer.Write(this.getParemeterString());
                        writer.Flush();
                    }
                }
            }
            webRequest.BeginGetResponse(new AsyncCallback(handleResponse), webRequest);
            _parameter.Clear();//清空参数列表
        }

        /// <summary>
        /// 异步响应回调函数
        /// </summary>
        /// <param name="asyncResult">异步请求参数</param>
        private void handleResponse(IAsyncResult asyncResult)
        {
            string result = "";
            bool iserror = false;
            try
            {
                HttpWebRequest webRequest = asyncResult.AsyncState as HttpWebRequest;
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.EndGetResponse(asyncResult);
                Stream streamResult = webResponse.GetResponseStream(); //获取响应流
                StreamReader reader = new StreamReader(streamResult);
                result = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                iserror = true;
                result = ex.Message;
            }
            finally
            {
                LBHttpEventArgs e = new LBHttpEventArgs();
                e.isError = iserror;
                e.result = result;
                //进行异步回调操作
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {

                    OnHttpCompleted(e);

                });
            }
        }
    }

    /// <summary>
    /// 枚举请求类型
    /// </summary>
    public enum requestType
    {
        /// <summary>
        /// GET请求
        /// </summary>
        GET,

        /// <summary>
        /// POST请求
        /// </summary>
        POST
    }
}
