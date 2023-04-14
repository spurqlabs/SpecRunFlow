using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TechTalk.SpecFlow;

namespace SpurFlow.Utils
{
    public class APIUtil : RestSharp.RestClient
    {

        public APIUtil(string baseURL)
        {
            BaseUrl = new Uri(baseURL);
        }

        public override IRestResponse Execute(IRestRequest request)
        {
            IRestResponse response = null;
            var stopWatch = new Stopwatch();

            try
            {
                stopWatch.Start();
                response = base.Execute(request);
                stopWatch.Stop();
                return response;
            }
            catch (Exception e)
            {
            }
            finally
            {
                LogRequest(request, response, stopWatch.ElapsedMilliseconds);
            }

            return null;
        }

        public override IRestResponse<T> Execute<T>(IRestRequest request)
        {
            IRestResponse<T> response = null;
            var stopWatch = new Stopwatch();

            try
            {
                stopWatch.Start();
                response = base.Execute<T>(request);
                stopWatch.Stop();
                return response;
            }
            catch (Exception e)
            {
            }
            finally
            {
                LogRequest(request, response, stopWatch.ElapsedMilliseconds);
            }

            return response;
        }

        public void LogRequest(IRestRequest request, IRestResponse response, long durationMs)
        {
            APIInfo _apiInfo = new APIInfo();
            _apiInfo.RequestInfo = new RequestInfo();
            _apiInfo.ResponseInfo = new ResponseInfo();
            _apiInfo.RequestInfo.Resource = request.Resource;
            foreach (Parameter param in request.Parameters)
            {
                _apiInfo.RequestInfo.RequestParameters.Add(
                    new ParameterInfo()
                    {
                        ParameterName = param.Name,
                        ParameterValue = param.Value.ToString(),
                        ParameterType = param.Type.ToString()
                    });
            }
            _apiInfo.RequestInfo.Method = request.Method.ToString();
            _apiInfo.RequestInfo.Uri = base.BuildUri(request).ToString();
            _apiInfo.ResponseInfo.StatusCode = response.StatusCode.ToString();
            _apiInfo.ResponseInfo.Content = response.Content;
            foreach (Parameter param in response.Headers)
            {
                _apiInfo.ResponseInfo.Headers.Add(
                    new ParameterInfo()
                    {
                        ParameterName = param.Name,
                        ParameterValue = param.Value.ToString(),
                        ParameterType = param.Type.ToString()
                    });
            }
            if (response.ResponseUri != null)
            {
                _apiInfo.ResponseInfo.ResponseUri = response.ResponseUri.ToString();
            }
            if (response.ErrorMessage != null)
            {
                _apiInfo.ResponseInfo.ErrorMessage = response.ErrorMessage.ToString();
            }
            _apiInfo.Duration = durationMs;
            ScenarioContext.Current.StepContext.Set(_apiInfo, "APIInfo");

        }

        public class APIInfo
        {

            public RequestInfo RequestInfo { get; set; }
            public ResponseInfo ResponseInfo { get; set; }
            public long Duration { get; set; }
        }

        public class RequestInfo
        {
            public string Resource { get; set; }
            public List<ParameterInfo> RequestParameters { get; set; } = new List<ParameterInfo>();
            public string Method { get; set; }
            public string Uri { get; set; }

        }

        public class ParameterInfo
        {
            public string ParameterName { get; set; }
            public string ParameterValue { get; set; }
            public string ParameterType { get; set; }
        }

        public class ResponseInfo
        {
            public string StatusCode { get; set; }
            public string Content { get; set; }
            public List<ParameterInfo> Headers { get; set; } = new List<ParameterInfo>();
            public string ResponseUri { get; set; }
            public string ErrorMessage { get; set; }
        }

    }

}
