namespace Cs.HttpServer.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Reflection;
    using System.Threading.Tasks;
    using Cs.Core.Util;
    using Cs.Exception;
    using Cs.Logging;
    using static Cs.HttpServer.Enums;

    public sealed class Router
    {
        private readonly Trie[] trieList = new Trie[EnumUtil<MethodType>.Count];

        public Router()
        {
            for (int i = 0; i < this.trieList.Length; ++i)
            {
                this.trieList[i] = new Trie();
            }
        }

        public void RegisterController(Type controlleType)
        {
            foreach (MethodInfo methodInfo in controlleType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!methodInfo.IsStatic)
                {
                    continue;
                }

                var attribute = (RouteUrlAttribute?)Attribute.GetCustomAttribute(methodInfo, typeof(RouteUrlAttribute));
                if (attribute == null)
                {
                    continue;
                }

                var handler = RequestHandler.Create(attribute.Url, methodInfo);
                if (handler == null)
                {
                    Log.Error($"handler parameter mismatched. url:{attribute.Url} method:{attribute.Method} code:{attribute.FileLine}");
                    continue;
                }

                var trie = this.GetTrie(attribute.Method) ?? throw new NotImplementedException(attribute.Method.ToString());
                if (trie.Add(handler) == false)
                {
                    Log.Error($"routing same url repetedly. url:{attribute.Url} method:{attribute.Method} code:{attribute.FileLine}");
                }
            }
        }

        public async Task<bool> HandleAsync(HttpListenerContext context)
        {
            var request = context.Request;
            if (!Enum.TryParse(request.HttpMethod, ignoreCase: true, out MethodType methodType))
            {
                return false;
            }

            var trie = this.GetTrie(methodType);
            if (trie == null)
            {
                Log.Error($"unsupported http method:{request.HttpMethod}");
                return false;
            }

            var handlerArgs = new List<object> { context };
            RequestHandler? handler = trie.Find(request.Url?.LocalPath ?? string.Empty, handlerArgs);
            if (handler == null)
            {
                return false;
            }

            try
            {
                await handler.ExecuteAsync(handlerArgs.ToArray());
            }
            catch (Exception ex)
            {
                Log.Error(ExceptionUtil.FlattenInnerExceptions(ex));
                return false;
            }

            return true;
        }

        private Trie? GetTrie(MethodType methodType)
        {
            int index = (int)methodType;
            if (index < 0 || index >= this.trieList.Length)
            {
                return null;
            }

            return this.trieList[index];
        }
    }
}
