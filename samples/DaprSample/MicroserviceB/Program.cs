var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
  .AddDaprInfrastructure(options => options
    .SetConfiguration(builder.Configuration))
  .AddMessaging(messaging => messaging
    .ScanAssembly(typeof(Program).Assembly))
  .AddStateStore();

builder.Services
  .AddEndpointsApiExplorer()
  .AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.MapMessageHandlers();

app.Run();
