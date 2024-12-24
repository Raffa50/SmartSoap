# Aldrigos.SmartSoap.AspNet

Aldrigos.SmartSoap.AspNet is a middleware component for ASP.NET Core that facilitates the creation and handling of SOAP endpoints. It allows you to easily integrate SOAP-based web services into your ASP.NET Core applications.

## Features

- Automatic SOAP endpoint creation based on controllers.
- Customizable SOAP content types.
- Serialization and deserialization of SOAP messages.
- Integration with dependency injection.

## Installation

To install Aldrigos.SmartSoap.AspNet, add the following NuGet package to your project:


## Usage

### 1. Define SOAP Controllers

Create your SOAP controllers by decorating them with the `[SoapController]` attribute. Define your methods to handle SOAP requests.
```csharp
[SoapController("MySoapService"), ApiController] 
public class MySoapServiceController: ControllerBase { 
	public IActionResult MyMethod(MyRequest request) { // Handle the request and return a response } 
}
```


### 2. Configure Middleware

In your `Startup.cs` or `Program.cs`, configure the `SoapEndpointMiddleware` in the ASP.NET Core pipeline.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers()
        .Services
        .AddSoap(AppDomain.CurrentDomain.GetAssemblies());
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseHttpsRedirection();

    app.UseRouting();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });

    app.UseSoapEndpoint();
}
```