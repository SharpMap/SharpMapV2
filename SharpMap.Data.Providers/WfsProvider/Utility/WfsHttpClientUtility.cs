using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security;

namespace SharpMap.Data.Providers.WfsProvider.Utility
{
    public class WfsHttpClientUtility
    {
            #region Fields and Properties

            private readonly NameValueCollection _requestHeaders;
            private byte[] _postData;
            private string _proxyUrl;

            private string _url;
            private HttpWebRequest _webRequest;
            private HttpWebResponse _webResponse;

            /// <summary>
            /// Gets ans sets the Url of the request.
            /// </summary>
            public string Url
            {
                get { return _url; }
                set { _url = value; }
            }

            /// <summary>
            /// Gets and sets the proxy Url of the request. 
            /// </summary>
            public string ProxyUrl
            {
                get { return _proxyUrl; }
                set { _proxyUrl = value; }
            }

            /// <summary>
            /// Sets the data of a HTTP POST request as byte array.
            /// </summary>
            public byte[] PostData
            {
                set { _postData = value; }
            }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="WfsHttpClientUtility"/> class.
            /// </summary>
            internal WfsHttpClientUtility()
            {
                _requestHeaders = new NameValueCollection();
            }

            #endregion

            #region Public Member

            /// <summary>
            /// Adds a HTTP header.
            /// </summary>
            /// <param name="name">The name of the header</param>
            /// <param name="value">The value of the header</param>
            public void AddHeader(string name, string value)
            {
                _requestHeaders.Add(name, value);
            }

            /// <summary>
            /// Performs a HTTP-GET or HTTP-POST request and returns a datastream for reading.
            /// </summary>
            public Stream GetDataStream()
            {
                if (string.IsNullOrEmpty(_url))
                    throw new ArgumentNullException("Request Url is not set!");

                // Free all resources of the previous request, if it hasn't been done yet...
                Close();

                try
                {
                    _webRequest = (HttpWebRequest)WebRequest.Create(_url);
                }
                catch (SecurityException ex)
                {
                    Trace.TraceError("An exception occured due to security reasons while initializing a request to " + _url +
                                     ": " + ex.Message);
                    throw ex;
                }
                catch (NotSupportedException ex)
                {
                    Trace.TraceError("An exception occured while initializing a request to " + _url + ": " + ex.Message);
                    throw ex;
                }

                _webRequest.Timeout = 90000;

                if (!string.IsNullOrEmpty(_proxyUrl))
                    _webRequest.Proxy = new WebProxy(_proxyUrl);

                try
                {
                    _webRequest.Headers.Add(_requestHeaders);

                    /* HTTP POST */
                    if (_postData != null)
                    {
                        _webRequest.ContentLength = _postData.Length;
                        _webRequest.Method = WebRequestMethods.Http.Post;
                        using (Stream requestStream = _webRequest.GetRequestStream())
                        {
                            requestStream.Write(_postData, 0, _postData.Length);
                        }
                    }
                    /* HTTP GET */
                    else
                        _webRequest.Method = WebRequestMethods.Http.Get;

                    _webResponse = (HttpWebResponse)_webRequest.GetResponse();
                    return _webResponse.GetResponseStream();
                }
                catch (Exception ex)
                {
                    Trace.TraceError("An exception occured during a HTTP request to " + _url + ": " + ex.Message);
                    throw ex;
                }
            }

            /// <summary>
            /// This method resets all configurations.
            /// </summary>
            public void Reset()
            {
                _url = null;
                _proxyUrl = null;
                _postData = null;
                _requestHeaders.Clear();
            }

            /// <summary>
            /// This method closes the WebResponse object.
            /// </summary>
            public void Close()
            {
                if (_webResponse != null)
                {
                    _webResponse.Close();
                    _webResponse.GetResponseStream().Dispose();
                }
            }

            #endregion
        }
}