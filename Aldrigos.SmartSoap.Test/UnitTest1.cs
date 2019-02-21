using Aldrigos.SmartSoap;
using Aldrigos.SmartSoap.Test.DneModels;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tests
{
    [TestFixture]
    public class Tests
    {
        readonly IHttpClientFactory httpClientFactory;

        public Tests()
        {
            httpClientFactory = new ServiceCollection()
                .AddHttpClient()
                .BuildServiceProvider()
                .GetRequiredService<IHttpClientFactory>();
        }

        [Test]
        public async Task Test1()
        {
            var add = new Add() { intA = 1, intB = 1 };
            var client = new SoapClient(httpClientFactory) { BaseUrl = new Uri("http://www.dneonline.com/") };
            var resp = await client.SendAsync<AddResponse, Add>("calculator.asmx", add);
            Assert.That(resp.AddResult, Is.EqualTo(2));
        }
    }
}