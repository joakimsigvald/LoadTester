using System.Net;

namespace Applique.LoadTester.Domain.Service;

public class RestCallResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public string Body { get; set; }
}