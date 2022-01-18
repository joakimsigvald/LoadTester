using System;

namespace Applique.LoadTester.Business.Design
{
    public class Endpoint
    {
        public string Name { get; set; }
        public string Path { get; set; } = string.Empty;
        public string Method { get; set; } = "Get";
        public Header[] Headers { get; set; } = Array.Empty<Header>();
    }
}