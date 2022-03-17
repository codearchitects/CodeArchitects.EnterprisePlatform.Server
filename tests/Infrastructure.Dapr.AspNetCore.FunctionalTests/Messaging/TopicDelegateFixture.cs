using CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Messaging.Fakes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Messaging;

public sealed class TopicDelegateFixture : IDisposable
{
  public TopicDelegateFixture()
  {
    Mock<Assembly> handlersAssemblyMock = new Mock<Assembly>(MockBehavior.Strict);
    handlersAssemblyMock
      .Setup(x => x.GetHashCode())
      .Returns(0);
    handlersAssemblyMock
      .Setup(x => x.GetTypes())
      .Returns(new[] { typeof(Message1Handler) });

    WebApplicationFactory appFactory = new WebApplicationFactory(handlersAssemblyMock.Object);

    Http = appFactory.CreateClient();
  }

  public HttpClient Http { get; }

  public void Dispose()
  {
    Http.Dispose();
  }

  private class WebApplicationFactory : WebApplicationFactory<Startup>
  {
    private readonly Assembly _handlersAssembly;

    public WebApplicationFactory(Assembly handlersAssembly)
    {
      _handlersAssembly = handlersAssembly;
    }

    protected override IHostBuilder? CreateHostBuilder()
    {
      return Host
        .CreateDefaultBuilder()
        .ConfigureWebHostDefaults(builder => builder
          .UseStartup(context => new Startup(context.Configuration, _handlersAssembly)));
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
      builder.UseContentRoot(Directory.GetCurrentDirectory());
      return base.CreateHost(builder);
    }
  }

  private class Startup
  {
    private readonly Assembly _handlersAssembly;

    public Startup(IConfiguration configuration, Assembly handlersAssembly)
    {
      Configuration = configuration;
      _handlersAssembly = handlersAssembly;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDaprInfrastructure(cfg => cfg.AddServiceOptions(Configuration))
        .AddMessageHandlers(_handlersAssembly);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseRouting();

      app.UseEndpoints(endpoints => endpoints.MapMessageHandlers());
    }
  }
}
