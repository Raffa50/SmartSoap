SmartSoap by Aldrigo Raffaele

The SmarSoap NuGet allows to simplify Soap API comunication
you only need to initialize a SoapClient either directly or
by registering the dependency injection. It requires IHttpClientFactory.
From version 1.0.2 you can register:

.AddSingleton<ISoapClientFactory, SoapClientFactory()

remember to register IHttpClientFactory !

now you can simply get a new SoapClient like the httpClient

var soapClient = soapClientFactory.Make();

and configure it like an httpClient, for example setting the base Url

soapClient.BaseUrl = new Uri("http://yoururl.com/something/");

you can also add custom HttpHeaders

soapClient.HttpHeaders.Add("myHeader", "value");

then, once you have mapped the models class you just inoke

var result = await soapClient.SendAsync<ResultType, RequestType>( "method", requestObj );

That's it!

The library also includes a better XmlSerializer, but you can use another or implement yourown
please report any bug/problem on this GitHub!