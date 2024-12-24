# Aldrigos.SmartSoap

**Aldrigos.SmartSoap** is a .NET Standard 2.0 library for handling SOAP calls. It provides a simple and flexible SOAP client that supports XML serialization and deserialization, HTTP header handling, and SOAP content type configuration.

## Features

- Support for .NET Standard 2.0
- XML serialization and deserialization
- HTTP header management
- SOAP content type configuration (text/xml, application/soap+xml, application/xml)
- Logging of SOAP requests and responses

## Installation

To install **Aldrigos.SmartSoap** in your project, you can use NuGet Package Manager:
Install-Package Aldrigos.SmartSoap


## Usage

### Configuring the SoapClient

To start using **Aldrigos.SmartSoap**, you need to configure an instance of `SoapClient`:

```csharp
var soapClient = new SoapClient(httpClientFactory, xmlSerializer, logger)
{
    BaseUrl = new Uri("https://example.com/soap"),
    SoapContentType = SoapContentType.TextXml
};

soapClient.HttpHeaders.Add("Custom-Header", "HeaderValue");
await soapClient.SendAsync<MyResponse, MyRequest>("MySoapMethod", request);
```