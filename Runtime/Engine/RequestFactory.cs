﻿using System.Linq;
using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Runtime.External;

namespace Applique.LoadTester.Runtime.Engine
{
    internal class RequestFactory
    {
        private readonly Service _service;
        private readonly Endpoint _endpoint;
        private readonly IBindings _bindings;
        private readonly Step _step;

        public RequestFactory(Service service, Endpoint endpoint, IBindings bindings, Step step)
        {
            _service = service;
            _endpoint = endpoint;
            _bindings = bindings;
            _step = step;
        }

        public Request GetRequest(Header[] serviceHeaders)
            => new()
            {
                Url = GetUrl(_step.Args),
                Method = _endpoint.Method,
                Content = _bindings.CreateContent(_step.Body),
                Headers = serviceHeaders.Concat(_endpoint.Headers).Select(h => new Header
                {
                    Name = h.Name,
                    Value = _bindings.SubstituteVariables(h.Value)
                }).ToArray()
            };

        private string GetUrl(string args)
            => $"{GetPath(_service.BasePath)}{GetQuery(args)}";

        private string GetPath(string basePath)
            => $"{basePath}/{_bindings.SubstituteVariables(_endpoint.Path)}".Trim('/');

        private string GetQuery(string args) => $"?{_bindings.SubstituteVariables(args)}".TrimEnd('?');
    }
}