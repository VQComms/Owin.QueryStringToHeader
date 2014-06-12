namespace Owin.QueryStringToHeader.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Linq;
    using Xunit;

    public class QueryStringToHeaderTests
    {
        [Fact]
        public void Should_Execute_Next_If_No_QueryString()
        {
            //Given
            var owinhttps = GetQueryStringToHeader(GetNextFunc());

            var environment = new Dictionary<string, object>
            {
                { "owin.RequestQueryString", "" },
                { "owin.RequestHeaders", new Dictionary<string, string[]>() },
                { "owin.RequestPath", "/" }
            };

            //When
            var task = owinhttps.Invoke(environment);

            //Then
            Assert.Equal(true, task.IsCompleted);
            Assert.Equal(123, ((Task<int>)task).Result);
        }

        [Fact]
        public void Should_Execute_Next_If_Header_Already_Exists()
        {
            //Given
            var owinhttps = GetQueryStringToHeader(GetNextFunc());

            var environment = new Dictionary<string, object>
            {
                { "owin.RequestQueryString", "foo=bar&authorization=my_secret_token"},
                { "owin.RequestHeaders", new Dictionary<string, string[]> { { "Authorization", new [] { "my_existing_secret_token" }}} },
                { "owin.RequestPath", "/"}
            };

            //When
            var task = owinhttps.Invoke(environment);

            var headers = (Dictionary<string, string[]>)environment["owin.RequestHeaders"];

            //Then
            Assert.Equal("my_existing_secret_token", headers["Authorization"].First());
            Assert.Equal(true, task.IsCompleted);
            Assert.Equal(123, ((Task<int>)task).Result);
        }

        [Fact]
        public void Should_Add_Header_If_QueryString_Parameter_Is_Available()
        {
            //Given
            var owinhttps = GetQueryStringToHeader(GetNextFunc());

            var environment = new Dictionary<string, object>
            {
                { "owin.RequestQueryString", "foo=bar&authorization=my_secret_token"},
                { "owin.RequestHeaders", new Dictionary<string, string[]>() },
                { "owin.RequestPath", "/"}
            };

            //When
            var task = owinhttps.Invoke(environment);

            var headers = (Dictionary<string, string[]>)environment["owin.RequestHeaders"];

            //Then
            Assert.Equal("my_secret_token", headers["Authorization"].First());
            Assert.Equal(true, task.IsCompleted);
            Assert.Equal(123, ((Task<int>)task).Result);
        }

        public Func<IDictionary<string, object>, Task> GetNextFunc()
        {
            return objects => Task.FromResult(123);
        }

        public QueryStringToHeader GetQueryStringToHeader(Func<IDictionary<string, object>, Task> nextFunc, string queryStringParam = "authorization", string requestHeader = "Authorization")
        {
            return new QueryStringToHeader(nextFunc, queryStringParam, requestHeader);
        }
    }
}
