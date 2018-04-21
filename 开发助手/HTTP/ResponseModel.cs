using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace 开发助手.HTTP
{
    public class ResponseModel
    {
        private WebHeaderCollection header;
        /// <summary>
        /// 返回的头部信息集合
        /// </summary>
        public WebHeaderCollection Header
        {
            get { return header; }
            set { header = value; }
        }
        private string html;
        /// <summary>
        /// 返回的文本内容
        /// </summary>
        public string Html
        {
            get { return html; }
            set { html = value; }
        }
        private Stream stream;
        /// <summary>
        /// 返回的流内容
        /// </summary>
        public Stream Stream
        {
            get { return stream; }
            set { stream = value; }
        }

    }
    public class HttpHelper
    {
        private string accept = "application/json,text/javascrip{过滤}t,text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
        private System.Net.CookieContainer cc = new System.Net.CookieContainer();
        private string contentType = "application/x-www-form-urlencoded";

        private int timeOut = 30000;
        public NameValueCollection Heads = new NameValueCollection();
        private bool AllowAutoRedirect = false;
        bool needReset = false;
        private System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("utf-8");

        public IWebProxy Proxy;
        private string[] userAgents = new string[] { "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022)", "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36 SE 2.X MetaSr 1.0" };

        private string userAgent
        {
            get
            {
                return this.userAgents[new Random().Next(0, this.userAgents.Length)];
            }
        }


        /// <summary>
        /// 设置下一次请求为自动重定向
        /// </summary>
        /// <param name="value"></param>
        public void SetAllowAutoRedirectOneTime(bool value)
        {
            AllowAutoRedirect = value;
            needReset = true;
        }

        /// <summary>
        /// 网页访问
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="isPost">是否Post</param>
        /// <param name="postData">Post数据内容</param>
        /// <param name="retType">返回类型0为文本,1为Stream</param>
        /// <param name="cookieContainer">cookie</param>
        /// <param name="refurl">Referer</param>
        /// <param name="_contentType">contentType</param>
        /// <param name="headers">请求头</param>
        /// <returns></returns>
        public ResponseModel HttpVisit(string url, bool isPost = false, string postData = null, int retType = 0, System.Net.CookieContainer cookieContainer = null, string refurl = null, string _contentType = null, NameValueCollection headers = null)
        {
            if (cookieContainer == null)
            {
                cookieContainer = this.cc;
            }

            if (!isPost)
            {
                return GetHtml(url, refurl, cookieContainer, _contentType, headers, retType);
            }


            ResponseModel model = new ResponseModel();

            ServicePointManager.Expect100Continue = true;

            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                byte[] bytes = System.Text.Encoding.Default.GetBytes(postData);
                request = (HttpWebRequest)WebRequest.Create(url);
                if (this.Proxy != null) request.Proxy = this.Proxy;
                request.CookieContainer = cookieContainer;
                request.Timeout = timeOut;
                if (string.IsNullOrEmpty(_contentType))
                {
                    request.ContentType = this.contentType;
                }
                else
                {
                    request.ContentType = _contentType;
                }

                if (string.IsNullOrEmpty(refurl))
                {
                    request.Referer = url;
                }
                else
                {
                    request.Referer = refurl;
                }


                request.AllowAutoRedirect = AllowAutoRedirect;
                request.Accept = this.accept;
                request.UserAgent = this.userAgent;

                if (headers != null)
                {
                    request.Headers.Add(Heads);
                    request.Headers.Add(headers);
                }
                else
                {
                    request.Headers.Add(Heads);
                }


                request.Method = isPost ? "POST" : "GET";
                request.ContentLength = bytes.Length;


                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();


                if (retType == 1)
                {
                    response = (HttpWebResponse)request.GetResponse();

                    model.Header = response.Headers;

                    Stream responseStream = response.GetResponseStream();

                    if (response.Cookies.Count > 0)
                    {
                        this.cc.Add(response.Cookies);
                    }

                    model.Stream = responseStream;
                    return model;

                }


                string str = string.Empty;
                response = (HttpWebResponse)request.GetResponse();

                model.Header = response.Headers;

                string encoding = "utf-8";

                if (!string.IsNullOrEmpty(response.CharacterSet))
                {
                    encoding = response.CharacterSet.ToLower();
                }
                else
                {
                    encoding = this.encoding.HeaderName;
                }

                if (response.ContentEncoding.ToLower().Contains("gzip"))
                {

                    using (GZipStream stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                    {
                        using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(encoding)))
                        {

                            str = reader.ReadToEnd();
                        }
                    }
                }
                else if (response.ContentEncoding.ToLower().Contains("deflate"))
                {
                    using (DeflateStream stream = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress))
                    {
                        using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(encoding)))
                        {

                            str = reader.ReadToEnd();
                        }

                    }
                }
                else
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(encoding)))
                        {

                            str = reader.ReadToEnd();
                        }
                    }
                }


                request.Abort();
                response.Close();
                request.Abort();
                if (response.Cookies.Count > 0)
                {
                    this.cc.Add(response.Cookies);
                }
                model.Html = str;
                return model;
            }
            catch (Exception ex)
            {
                if (request != null) request.Abort();
                if (response != null)
                {
                    response.Close();

                    return new ResponseModel() { Html = ex.Message, Header = response.Headers };
                }
                return new ResponseModel() { Html = ex.Message };
            }
            finally
            {
                if (needReset)
                {
                    AllowAutoRedirect = false;
                    needReset = false;
                }
            }
        }
        /// <summary>
        /// 清理string类型Cookie.剔除无用项返回结果为null时遇见错误.
        /// </summary>
        /// <param name="Cookies"></param>
        /// <returns></returns>
        public CookieCollection ClearCookie(string Cookies)
        {
            try
            {
                CookieCollection cookies = new CookieCollection();
                string rStr = string.Empty;
                Cookies = Cookies.Replace(";", "; ");
                Regex r = new Regex("(?<=,)(?<cookie>[^ ]+=(?!deleted;)[^;]+);");
                MatchCollection ms = r.Matches("," + Cookies);
                foreach (Match m in ms)
                {
                    string[] cookie = m.Groups["cookie"].Value.Split('=');

                    if (cookie.Length > 1)
                        cookies.Add(new Cookie(cookie[0], cookie[1]));

                }
                return cookies;
            }
            catch
            {
                return new CookieCollection();
            }
        }

        private ResponseModel GetHtml(string url, string refurl = null, System.Net.CookieContainer cookieContainer = null, string _contentType = "", NameValueCollection headers = null, int retType = 1)
        {
            if (cookieContainer == null)
            {
                cookieContainer = this.cc;
            }

            ResponseModel model = new ResponseModel();

            ServicePointManager.Expect100Continue = true;

            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                if (this.Proxy != null) request.Proxy = this.Proxy;
                request.CookieContainer = cookieContainer;
                request.Timeout = timeOut;
                if (string.IsNullOrEmpty(_contentType))
                {
                    request.ContentType = this.contentType;
                }
                else
                {
                    request.ContentType = _contentType;
                }

                if (string.IsNullOrEmpty(refurl))
                {
                    request.Referer = url;
                }
                else
                {
                    request.Referer = refurl;
                }


                request.AllowAutoRedirect = AllowAutoRedirect;
                request.Accept = this.accept;
                request.UserAgent = this.userAgent;

                if (headers != null)
                {
                    request.Headers.Add(Heads);
                    request.Headers.Add(headers);
                }
                else
                {
                    request.Headers.Add(Heads);
                }

                request.Method = "GET";


                if (retType == 1)
                {
                    response = (HttpWebResponse)request.GetResponse();

                    model.Header = response.Headers;

                    Stream responseStream = response.GetResponseStream();

                    if (response.Cookies.Count > 0)
                    {
                        this.cc.Add(response.Cookies);
                    }

                    model.Stream = responseStream;

                    return model;

                }

                string str = string.Empty;
                response = (HttpWebResponse)request.GetResponse();

                model.Header = response.Headers;

                string encoding = "utf-8";

                if (!string.IsNullOrEmpty(response.CharacterSet))
                {
                    encoding = response.CharacterSet.ToLower();
                }
                else
                {
                    encoding = this.encoding.HeaderName;
                }

                if (response.ContentEncoding.ToLower().Contains("gzip"))
                {

                    using (GZipStream stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                    {
                        using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(encoding)))
                        {

                            str = reader.ReadToEnd();
                        }
                    }
                }
                else if (response.ContentEncoding.ToLower().Contains("deflate"))
                {
                    using (DeflateStream stream = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress))
                    {
                        using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(encoding)))
                        {

                            str = reader.ReadToEnd();
                        }

                    }
                }
                else
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(encoding)))
                        {

                            str = reader.ReadToEnd();
                        }
                    }
                }

                if (response.Cookies.Count > 0)
                {
                    cookieContainer.Add(response.Cookies);
                }


                request.Abort();
                response.Close();

                model.Html = str;
                return model;
            }
            catch (Exception ex)
            {
                if (request != null) request.Abort();
                if (response != null)
                {
                    response.Close();

                    return new ResponseModel() { Html = ex.Message, Header = response.Headers };
                }
                return new ResponseModel() { Html = ex.Message };
            }
            finally
            {
                if (needReset)
                {
                    AllowAutoRedirect = false;
                    needReset = false;
                }
            }
        }

        private bool CheckValidationResult(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //直接通过HTTPS的证书请求
            return true;
        }

        public Stream GetStream(string url)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                if (this.Proxy != null) request.Proxy = this.Proxy;
                request.CookieContainer = this.cc;
                request.ContentType = this.contentType;
                //      request.ServicePoint.ConnectionLimit = this.maxTry;
                request.Timeout = 0x1388;
                request.Referer = url;
                request.Accept = this.accept;
                request.UserAgent = this.userAgent;
                request.Method = "GET";
                response = (HttpWebResponse)request.GetResponse();

                Stream responseStream = response.GetResponseStream();
                // this.currentTry--;
                if (response.Cookies.Count > 0)
                {
                    this.cc.Add(response.Cookies);
                }
                return responseStream;
            }
            catch (Exception ex)
            {
                //   if (this.currentTry <= this.maxTry) this.GetHtml(url, cookieContainer);
                //   this.currentTry--;
                if (request != null) request.Abort();
                if (response != null) response.Close();
                return null;
            }
        }


        #region String与CookieContainer互转
        /// <summary>
        /// 将String转CookieContainer
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public CookieContainer StringToCookie(string url, string cookie)
        {
            string[] arrCookie = cookie.Split(';');
            CookieContainer cookie_container = new CookieContainer();    //加载Cookie
            foreach (string sCookie in arrCookie)
            {
                if (sCookie.IndexOf("expires") > 0)
                    continue;
                cookie_container.SetCookies(new Uri(url), sCookie);
            }
            return cookie_container;
        }


        /// <summary>
        /// 将CookieContainer转换为string类型
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        public string GetCookieString()
        {
            System.Collections.Generic.List<Cookie> lstCookies = new System.Collections.Generic.List<Cookie>();
            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, cc, new object[] { });
            StringBuilder sb = new StringBuilder();
            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies)
                    {
                        sb.Append(c.Name).Append("=").Append(c.Value).Append(";");
                    }
            }
            return sb.ToString();
        }
        #endregion

    }
}