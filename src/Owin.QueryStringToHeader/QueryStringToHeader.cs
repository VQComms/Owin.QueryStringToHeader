namespace Owin.QueryStringToHeader
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Text.RegularExpressions;

    public class QueryStringToHeader
    {
        private readonly string _queryStringParameterName;
        private readonly string _requestHeaderName;
        private readonly Func<IDictionary<string, object>, Task> _nextFunc;

        public QueryStringToHeader(Func<IDictionary<string, object>, Task> nextFunc, string queryStringParameterName, string requestHeaderName)
        {
            _nextFunc = nextFunc;
            _queryStringParameterName = queryStringParameterName;
            _requestHeaderName = requestHeaderName;
        }

        public Task Invoke(IDictionary<string, object> environment)
        {
            if (!environment.ContainsKey("owin.RequestPath"))
            {
                throw new ApplicationException("Invalid OWIN request. Expected owin.RequestPath, but not present.");
            }

            var querystring = (string)environment["owin.RequestQueryString"];

            // If no query string authorization value is set, nothing we can do
            if (!querystring.ToLowerInvariant().Contains(_queryStringParameterName.ToLowerInvariant()))
            {
                return _nextFunc(environment);
            }

            var token = ParseQueryString(querystring);

            var requestHeaders = (IDictionary<string, string[]>)environment["owin.RequestHeaders"];

            if (requestHeaders.All(x => x.Key != _requestHeaderName))
            {
                requestHeaders.Add(_requestHeaderName, new[] { token[_requestHeaderName] });
            }

            return _nextFunc(environment);
        }

        // Taken from StackOverflow. Do not want to use System.Web!
        // http://stackoverflow.com/a/1322960
        private static NameValueCollection ParseQueryString(string s)
        {
            var nvc = new NameValueCollection();

            // remove anything other than query string from url
            if (s.Contains("?"))
            {
                s = s.Substring(s.IndexOf('?') + 1);
            }

            foreach (string vp in Regex.Split(s, "&"))
            {
                string[] singlePair = Regex.Split(vp, "=");

                if (singlePair.Length == 2)
                {
                    nvc.Add(singlePair[0], singlePair[1]);
                }
                else
                {
                    // only one key with no value specified in query string
                    nvc.Add(singlePair[0], string.Empty);
                }
            }

            return nvc;
        }
    }
}
