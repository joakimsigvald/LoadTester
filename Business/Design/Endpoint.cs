using System;

namespace Applique.LoadTester.Business.Design
{
    public class Endpoint
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Method { get; set; }
        public Header[] Headers { get; set; } = Array.Empty<Header>();
    }
}