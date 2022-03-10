using System;
using System.Linq;
using System.Threading.Tasks;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Assembly;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Service;
using Applique.LoadTester.Logic.Runtime.External;

namespace Applique.LoadTester.Logic.Runtime.Engine;

public class RestStepExecutor
{
    private readonly IRestCaller _restCaller;
    private readonly RequestFactory _requestFactory;
    private readonly IBindings _bindings;
    private readonly Service _service;

    public static RestStepExecutor Create(
        IRestCallerFactory restCallerFactory,
        ITestSuite suite,
        IStep step,
        IBindings bindings)
    {
        var (serviceName, endpointName) = ExtractPath(step);
        var service = suite.Services.Single(s => s.Name == serviceName);
        var restCaller = restCallerFactory.Create(service.BaseUrl);
        var requestFactory = CreateRequestFactory(service, bindings, endpointName, step);
        return new RestStepExecutor(restCaller, service, requestFactory, bindings);
    }

    private static (string serviceName, string endpointName) ExtractPath(IStep step)
    {
        var pair = step.Endpoint.Split('.');
        return (pair[0], pair[1]);
    }

    private static RequestFactory CreateRequestFactory(Service service, IBindings bindings, string endpointName, IStep step)
    {
        var endpoint = service.Endpoints.Single(ep => ep.Name == endpointName);
        return new RequestFactory(service, endpoint, bindings, step);
    }

    private RestStepExecutor(
        IRestCaller restCaller,
        Service service,
        RequestFactory requestFactory,
        IBindings bindings)
    {
        _restCaller = restCaller;
        _service = service;
        _requestFactory = requestFactory;
        _bindings = bindings;
    }

    public async Task<RestCallResponse> Execute(Header[] serviceHeaders)
    {
        try
        {
            var req = _requestFactory.GetRequest(serviceHeaders);
            var res = await _restCaller.Call(req);
            return res;
        }
        catch (TaskCanceledException tex)
        {
            Console.WriteLine($"{tex.Message}");
            return null;
        }
    }

    public Header[] CreateServiceHeaders()
    {
        var serviceHeaders = _service.Headers.Select(h => new Header
        {
            Name = h.Name,
            Value = _bindings.SubstituteVariables(h.Value)
        });
        return (_service.ApiKey is null
            ? serviceHeaders
            : serviceHeaders.Prepend(new Header { Name = _service.ApiKey.Name, Value = _service.ApiKey.Value }))
            .ToArray();
    }
}