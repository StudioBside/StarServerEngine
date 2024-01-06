namespace Cs.HttpServer.Routing
{
    using System.Collections.Generic;
    using System.Net;
    using System.Reflection;
    using System.Threading.Tasks;
    using Cs.Logging;
    using static Cs.HttpServer.Enums;

    internal sealed class RequestHandler
    {
        private readonly string uri;
        private readonly MethodInfo methodInfo;

        private RequestHandler(string uri, MethodInfo methodInfo)
        {
            this.uri = uri;
            this.methodInfo = methodInfo;
        }

        public string Uri => this.uri;

        public static RequestHandler? Create(string uri, MethodInfo methodInfo)
        {
            var paramTypesInUri = new List<ParamType>();

            string[] tokens = uri.Split('/');
            foreach (var token in tokens)
            {
                if (Trie.ParameterNode.StringToParamType(token, out ParamType paramType))
                {
                    paramTypesInUri.Add(paramType);
                }
            }

            ParameterInfo[] methodParameters = methodInfo.GetParameters();
            if (methodParameters.Length == 0)
            {
                Log.Error($"invalid methdo signature.");
                return null;
            }

            // 첫 번째 파라미터는 HttpListenerContext 타입
            if (methodParameters[0].ParameterType != typeof(HttpListenerContext))
            {
                Log.Error($"1st parameter of request handler must be HttpListenerContext type. uri:{uri} method:{methodInfo.Name}");
                return null;
            }

            int uriParameterCount = methodParameters.Length - 1;
            // uri의 인자와 메서드의 인자 개수가 다르면 에러. 
            if (paramTypesInUri.Count != uriParameterCount)
            {
                Log.Error($"invalid request handler signature. uri:{uri} method:{methodInfo.Name}");
                return null;
            }

            for (int i = 0; i < uriParameterCount; ++i)
            {
                var paramType = paramTypesInUri[i];
                var systemType = methodParameters[i + 1].ParameterType;
                if (!Trie.ParameterNode.CheckParamTypeMatching(paramType, systemType))
                {
                    Log.Error($"request method signature has mismatched with routing uri. index:{i} uri:{uri} method:{methodInfo.Name}");
                    return null;
                }
            }

            return new RequestHandler(uri, methodInfo);
        }

        public async Task ExecuteAsync(object[] parameters)
        {
            var result = this.methodInfo.Invoke(null, parameters);
            if (result is Task task)
            {
                await task;
            }
        }
    }
}
