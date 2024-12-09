namespace Cs.HttpServer.Routing;

using System;
using System.Collections.Generic;
using Cs.Core.Util;
using static Cs.HttpServer.Enums;

internal sealed partial class Trie
{
    internal class Node
    {
        private readonly string key;
        private readonly ParameterNode[] parameterChildren = new ParameterNode[EnumUtil<ParamType>.Count];
        private readonly Dictionary<string, Node> children = new Dictionary<string, Node>();
        private RequestHandler? handler;

        public Node(string key)
        {
            this.key = key;
        }

        public bool AddHandler(string[] tokens, int tokenIndex, RequestHandler handler)
        {
            // terminate condition
            if (tokens.Length == tokenIndex)
            {
                if (this.handler != null)
                {
                    throw new Exception($"uri [{this.handler.Uri}] already has a handler.");
                }

                this.handler = handler;
                return true;
            }

            string current = tokens[tokenIndex];
            Node child = this.GetOrAddChild(current);
            return child.AddHandler(tokens, tokenIndex + 1, handler);
        }

        public RequestHandler? MatchHandler(string[] tokens, int tokenIndex, List<object> handlerParams)
        {
            // terminate condition
            if (tokens.Length == tokenIndex)
            {
                return this.handler;
            }

            string current = tokens[tokenIndex];

            Node? child = this.TryParseToParameter(current, handlerParams);
            if (child != null)
            {
                return child.MatchHandler(tokens, tokenIndex + 1, handlerParams);
            }

            if (this.children.TryGetValue(current, out child) == false)
            {
                return null;
            }

            return child.MatchHandler(tokens, tokenIndex + 1, handlerParams);
        }

        private Node? TryParseToParameter(string data, List<object> handlerParams)
        {
            foreach (var parameterNode in this.parameterChildren)
            {
                if (parameterNode == null)
                {
                    continue;
                }

                if (parameterNode.TryParse(data, out object? result))
                {
                    handlerParams.Add(result);
                    return parameterNode;
                }
            }

            return null;
        }

        private Node GetOrAddChild(string key)
        {
            if (ParameterNode.StringToParamType(key, out ParamType paramType))
            {
                int index = (int)paramType;
                if (this.parameterChildren[index] == null)
                {
                    this.parameterChildren[index] = new ParameterNode(key, paramType);
                }

                return this.parameterChildren[index];
            }

            if (this.children.TryGetValue(key, out Node? child) == false)
            {
                child = new Node(key);
                this.children.Add(key, child);
            }

            return child;
        }
    }
}
