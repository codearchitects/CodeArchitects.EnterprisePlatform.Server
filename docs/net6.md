# .NET 6

The new template based on .NET 6 differs from the previous templates because of the introduction of top-level statements. In particular, the Startup class is no longer needed, since the application is configured directly in Program.cs. Nevertheless, the new template defines a file called App.cs that contains the methods that register the various services in the Dependency Injection container, so that each part of the configuration is identified by name and the code in Program.cs stays clean.

To use .NET 6, set version 6 of the .NET framework in the microservice yml.

```yml
target:
  framework: net
  version: 6
```

An example of a Program generated with the .NET 6 template follows.

```cs
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder
  .AddCaepServices()
  .AddAutoMapper()
  .AddControllers()
  .AddCors()
  .AddDaprInfrastructure()
  .AddSignalR();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

if (builder.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
  app.UseSwagger();
  app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ca.ShoppingCart.Business v1"));
}

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
  endpoints.MapControllers();
});

app.Run();
```

The implementations of the extension methods `AddCaepServices` and the others are in the App.cs file.
