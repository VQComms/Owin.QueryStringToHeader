namespace Owin.QueryStringToHeader
{
    public static class QueryStringToHeaderExtension
    {
        public static IAppBuilder QueryStringToHeader(this IAppBuilder app, string queryStringParameterName = "authorization", string requestHeaderName = "Authorization")
        {
            return app.Use<QueryStringToHeader>(queryStringParameterName, requestHeaderName);
        }
    }
}
