using Saasu.API.Core.Framework;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Models.RequestFiltering;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Serialization;

namespace Saasu.API.Client.Framework
{
    public abstract class BaseProxy
    {
        private string _wsAccessKey;
        private int _fileId;
        private string _baseUri;
        private string _bearerToken; // for OAuth

        public BaseProxy()
        {
            _wsAccessKey = Config.Default.WsAccessKey;
            _fileId = Config.Default.FileUid;
            _baseUri = Config.Default.BaseUri;
            ContentType = RequestContentType.ApplicationJson;
            OperationMethod = HttpMethod.Get;
            PagingEnabled = true;

            SetSslCommunicationsHandling();
        }

        public BaseProxy(string bearerToken)
            : this()
        {
            AuthenticationMethod = AuthenticationType.OAuth;
            _bearerToken = bearerToken;
            _wsAccessKey = null;
            _fileId = Config.Default.FileUid;

            SetSslCommunicationsHandling();
        }

        public BaseProxy(string baseUri, string wsAccessKey, int fileUid)
        {
            _wsAccessKey = wsAccessKey;
            _fileId = fileUid;
            _baseUri = baseUri;
            ContentType = RequestContentType.ApplicationJson;
            OperationMethod = HttpMethod.Get;
            PagingEnabled = true;
            AuthenticationMethod = AuthenticationType.WsAccessKey;
            SetSslCommunicationsHandling();

        }

        public string WsAccessKey { get { return _wsAccessKey; } }
        public int FileId { get { return _fileId; } set { _fileId = value; } }
        public HttpMethod OperationMethod { get; set; }
        public bool PagingEnabled { get; set; }
        public string BearerToken { get { return _bearerToken; } set { _bearerToken = value; } }

        private void SetSslCommunicationsHandling()
        {
            if (!string.IsNullOrWhiteSpace(Config.Default.IgnoreSSLErrors) && Config.Default.IgnoreSSLErrors.ToLowerInvariant() == "true")
            {
                ServicePointManager.ServerCertificateValidationCallback +=
                                (sender, cert, chain, sslPolicyErrors) => true;
            }
        }


        /// <summary>
        /// Indicates the prefix used to access the service in the Url.
        /// </summary>
        /// <example>The API call for getting a contact by Id might be 'http://somehost/Contacts/1' where the Id
        /// is 1. The request prefix in this case is 'Contacts' and is consistent for all calls relating to Contacts</example>
        public abstract string RequestPrefix { get; }

        /// <summary>
        /// Indicates the postfix used to access the action method in the Url.
        /// </summary>
        /// <example>In the Url 'http://somehost/test@mailhost.com/reset-password' 'test@mailhost.com' is the identifier (Id) and 'reset-password' is the postfix, indicating the action that will be performed for the identifier</example>
        public string RequestPostfix { get; set; }

        public AuthenticationType AuthenticationMethod { get; set; }

        /// <summary>
        /// Indicates whether to request the response in Json, Xml etc..
        /// </summary>
        public RequestContentType ContentType { get; set; }

        private StringBuilder GetBaseUri(string apiMethod)
        {
            if (string.IsNullOrWhiteSpace(_baseUri))
            {
                throw new System.ArgumentNullException("Base Uri is null (not configured)");
            }
            var uri = new StringBuilder();
            uri.AppendFormat("{0}", _baseUri);
            if (!_baseUri.EndsWith("/"))
            {
                uri.Append("/");
            }
            if (!string.IsNullOrWhiteSpace(RequestPrefix))
            {
                uri.AppendFormat("{0}", RequestPrefix);
            }
            if (!string.IsNullOrWhiteSpace(apiMethod))
            {
                if (!uri.ToString().EndsWith("/"))
                {
                    uri.Append("/");
                }
                uri.AppendFormat("{0}", apiMethod);
            }
            if (!string.IsNullOrWhiteSpace(RequestPostfix))
            {
                if (!uri.ToString().EndsWith("/"))
                {
                    uri.Append("/");
                }
                uri.AppendFormat("{0}", RequestPostfix);
            }
            return uri;
        }

        protected void GetPaging(StringBuilder queryArgs, int? pageNumber, int? pageSize, out bool inclPageNumber, out bool inclPageSize)
        {
            if (pageNumber != null && pageNumber > 0)
            {
                AppendQueryArg(queryArgs, ApiConstants.PageQueryArg, pageNumber.ToString());
                inclPageNumber = false;
            }
            else
            {
                inclPageNumber = true;
            }

            if (pageSize != null && pageSize > 0)
            {
                AppendQueryArg(queryArgs, ApiConstants.PageSizeQueryArg, pageSize.ToString());
                inclPageSize = false;
            }
            else
            {
                inclPageSize = true;
            }
        }

        protected void AppendQueryArg(StringBuilder queryUrl, string arg, string value)
        {
            if (queryUrl.Length > 0)
            {
                queryUrl.Append("&");
            }
            else
            {
                queryUrl.Append("?");
            }
            queryUrl.AppendFormat("{0}={1}", arg, value);
        }

        protected virtual string GetRequestUri(string apiMethod, string queryArguments = null, bool inclDefaultPageNumber = true, bool inclDefaultPageSize = true)
        {
            return GetRequestUri(apiMethod, new RequestPagingFilter(), queryArguments, inclDefaultPageNumber, inclDefaultPageSize);
        }

        protected virtual string GetRequestUri(string apiMethod, HttpMethod httpMethod, string queryArguments = null, bool inclDefaultPageNumber = true, bool inclDefaultPageSize = true)
        {
            return GetRequestUri(apiMethod, null, httpMethod, queryArguments, inclDefaultPageNumber, inclDefaultPageSize);
        }

        protected virtual string GetRequestUri(string apiMethod, RequestPagingFilter pagingFilter, string queryArguments = null, bool inclDefaultPageNumber = true, bool inclDefaultPageSize = true)
        {
            return GetRequestUri(apiMethod, pagingFilter, HttpMethod.Get, queryArguments, inclDefaultPageNumber, inclDefaultPageSize);
        }

        protected virtual string GetRequestUri(string apiMethod, RequestPagingFilter pagingFilter, HttpMethod httpMethod, string queryArguments = null, bool inclDefaultPageNumber = true, bool inclDefaultPageSize = true)
        {
            if (pagingFilter == null)
            {
                pagingFilter = new RequestPagingFilter();
            }
            var uri = GetBaseUri(apiMethod);
            EmbedAuthorisationDetails(uri);
            if (httpMethod == HttpMethod.Get && PagingEnabled && (inclDefaultPageNumber || inclDefaultPageSize))
            {
                AppendConcatenationCharToUriBuilder(uri);

                if (inclDefaultPageNumber)
                {
                    uri.AppendFormat("{0}={1}", ApiConstants.PageQueryArg, pagingFilter.Page);

                    if (inclDefaultPageSize) uri.Append("&");
                }

                if (inclDefaultPageSize)
                {
                    uri.AppendFormat("{0}={1}", ApiConstants.PageSizeQueryArg, pagingFilter.PageSize);
                }
            }

            if (!string.IsNullOrWhiteSpace(queryArguments))
            {
                AppendConcatenationCharToUriBuilder(uri);
                uri.Append(queryArguments);
            }
            return uri.ToString();
        }

        private void AppendConcatenationCharToUriBuilder(StringBuilder uri)
        {
            if (uri.ToString().Contains("?"))
            {
                uri.Append("&");
            }
            else
            {
                uri.Append("?");
            }
        }

        private void EmbedAuthorisationDetails(StringBuilder uri)
        {
            AppendConcatenationCharToUriBuilder(uri);
            if (AuthenticationMethod == AuthenticationType.WsAccessKey)
            {
                uri.AppendFormat("{0}={1}&{2}={3}", ApiConstants.WsAccessKeyQueryArg, WsAccessKey, ApiConstants.FileQueryArg, FileId);
            }
            else
            {
                // This is not really authorisation details but we refine the file to be used here as we can have multiple files returned
                // in the scope if it is not specified.
                if (FileId > 0)
                {
                    uri.AppendFormat("{0}={1}", ApiConstants.FileQueryArg, FileId);
                }
            }
        }

        public virtual ProxyResponse GetResponse(string requestUrl)
        {
            var responseMsg = GetResponseMessage(requestUrl);
            var rawResponse = responseMsg.Content != null ? responseMsg.Content.ReadAsStringAsync().Result : string.Empty;
            return new ProxyResponse(rawResponse, responseMsg.IsSuccessStatusCode, responseMsg.StatusCode, responseMsg.ReasonPhrase);
        }

        public virtual ProxyResponse<T> GetResponse<T>(string requestUri) where T : class
        {
            return GetResponse<T, T>(requestUri, null);
        }

        public virtual ProxyResponse GetResponse<T>(string requestUri, T postData) where T : class
        {
            return GetResponse<object, T>(requestUri, postData);
        }

        public virtual ProxyResponse<R> GetResponse<T, R>(string requestUri, T postData)
            where T : class
            where R : class
        {
            HttpResponseMessage responseMsg;
            if (OperationMethod != HttpMethod.Put && OperationMethod != HttpMethod.Post)
            {
                responseMsg = GetResponseMessage<object>(requestUri, null);
            }
            else
            {
                responseMsg = GetResponseMessage<T>(requestUri, postData);
            }
            if (responseMsg.IsSuccessStatusCode)
            {
                if (responseMsg.Content != null)
                {
                    var textData = responseMsg.Content.ReadAsStringAsync().Result;
                    if (!string.IsNullOrWhiteSpace(textData))
                    {
                        var dto = Deserialise<R>(textData);
                        var proxResponse = new ProxyResponse<R>(textData, dto, true, responseMsg.StatusCode, responseMsg.ReasonPhrase);
                        return proxResponse;
                    }
                    return new ProxyResponse<R>(textData, default(R), true, responseMsg.StatusCode, responseMsg.ReasonPhrase);
                }
            }

            return new ProxyResponse<R>(responseMsg.Content.ReadAsStringAsync().Result, default(R), false, responseMsg.StatusCode, responseMsg.ReasonPhrase);

        }

        protected virtual System.Net.Http.HttpResponseMessage GetResponseMessage(string requestUri)
        {
            return GetResponseMessage<object>(requestUri, null);
        }
        protected virtual System.Net.Http.HttpResponseMessage GetResponseMessage<T>(string requestUri, T postData)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType.AsContentTypeString()));
            if (AuthenticationMethod == AuthenticationType.OAuth && !string.IsNullOrEmpty(_bearerToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
            }

            MediaTypeFormatter mediaFormatter;

            if (ContentType == RequestContentType.ApplicationXml)
            {
                mediaFormatter = new System.Net.Http.Formatting.XmlMediaTypeFormatter();

            }
            else
            {
                mediaFormatter = new System.Net.Http.Formatting.JsonMediaTypeFormatter();
            }


            HttpResponseMessage responseMsg = null;
            if (OperationMethod == HttpMethod.Get)
            {
                responseMsg = client.GetAsync(requestUri).Result;
            }
            else if (OperationMethod == HttpMethod.Delete && postData == null)
            {
                responseMsg = client.DeleteAsync(requestUri).Result;
            }
            else if (OperationMethod == HttpMethod.Head)
            {
                var rqstMsg = new HttpRequestMessage(HttpMethod.Head, requestUri);
                responseMsg = client.SendAsync(rqstMsg).Result;
            }
            else
            {
                //Note: Need to explicitly specify the content type here otherwise this call fails.
                if (OperationMethod == HttpMethod.Put)
                {
                    responseMsg = client.PutAsync<T>(requestUri, postData, mediaFormatter).Result;
                }
                else
                {
                    responseMsg = client.PostAsync<T>(requestUri, postData, mediaFormatter).Result;
                }
            }

            return responseMsg;
        }

        public T Deserialise<T>(string data) where T : class
        {
            T dto;
            if (ContentType == RequestContentType.ApplicationJson)
            {
                dto = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(data);
                //jsonSerializer = new JavaScriptSerializer();
                //dto = jsonSerializer.Deserialize<T>(data);
            }
            else
            {
                using (Stream stream = new MemoryStream())
                {
                    byte[] content = System.Text.Encoding.UTF8.GetBytes(data);
                    stream.Write(content, 0, data.Length);
                    stream.Position = 0;
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    return xmlSerializer.Deserialize(new StringReader(data)) as T;

                    // For data contract serializer
                    //DataContractSerializer deserializer = new DataContractSerializer(typeof(T), null, Int32.MaxValue, false, false, null, new AllowAllContractResolver());
                    //return deserializer.ReadObject(stream) as T;
                }
            }

            return dto;
        }


    }
}
