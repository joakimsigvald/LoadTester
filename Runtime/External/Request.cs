using Applique.LoadTester.Core.Design;

namespace Applique.LoadTester.Runtime.External
{
    public class Request
    {
        public string Url { get; set; }
        public string Method { get; set; }
        public string Content { get; set; }
        public Header[] Headers { get; set; }
    }
}