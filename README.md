Owin.QueryStringToHeader
========================

A simple owin middleware component to turn a querystring parameter into a request header. This is useful when using token authentication with signalR.


Usage:
====

`app.QueryStringToHeader()`


Defaults:
====

QueryString parmeter name: authorization

Request header name: Authorization
