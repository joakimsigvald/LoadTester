﻿using System;
using System.Net.Http;

namespace LoadTester
{
    public class Service
    {
        public string Name { get; set; }
        public Uri BaseAddress => new Uri(BaseUrl);
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public Endpoint[] Endpoints { get; set; }

        public HttpClient CreateClient()
        {
            var client = new HttpClient
            {
                BaseAddress = BaseAddress
            };
            client.DefaultRequestHeaders.Add("apikey", ApiKey);
            return client;
        }
    }
}