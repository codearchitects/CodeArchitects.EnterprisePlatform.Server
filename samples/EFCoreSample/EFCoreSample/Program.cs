using CodeArchitects.Platform.Data.AdoNet.SQLServer;
using CodeArchitects.Platform.Data.AutoMapper;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;
using EFCoreSample;
using EFCoreSample.Domain.Model;
using EFCoreSample.Domain.Repositories;
using EFCoreSample.Infrastructure.Data;
using EFCoreSample.Infrastructure.Repositories;
using EFCoreSample.Mapping;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services
  .AddControllers()
  .AddNewtonsoftJson(opt =>
  {
    opt.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
  });

builder.Services.AddIdentityProfile();

string connectionString = builder.Configuration.GetConnectionString("SqlServer")!;

builder.Services.AddDbContext<DataContext>(options => options
  .UseSqlServer(connectionString)
  .LogTo(Console.WriteLine, LogLevel.Debug)
  .UseData(data => data
    .UseMultitenancy<MultitenancyDescriptor>()));

builder.Services.AddData(configuration => configuration
  .UseProvider<SQLServerProvider>(provider => provider.UseConnection(connectionString))
  .UseModel<DataConfiguration>());

builder.Services.AddData<DataContext>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();

builder.Services.AddAutoMapper(typeof(Program), typeof(AutoMapperTracking));

builder.Services
  .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
      ValidateIssuer = false,
      ValidateAudience = false,
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SigningKey"])),
      ClockSkew = TimeSpan.Zero,
      ValidateLifetime = true
    };
  });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo
  {
    Title = "EFCoreSample",
    Version = "v1"
  });

  c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
    In = ParameterLocation.Header,
    Description = "Insert **Bearer \\*space\\* JWT**",
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey
  });

  OpenApiSecurityScheme scheme = new OpenApiSecurityScheme
  {
    Reference = new OpenApiReference
    {
      Type = ReferenceType.SecurityScheme,
      Id = "Bearer"
    },
    Scheme = "oauth2",
    Name = "Bearer",
    In = ParameterLocation.Header
  };

  c.AddSecurityRequirement(new OpenApiSecurityRequirement
  {
    [scheme] = new List<string>()
  });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

using (IServiceScope scope = app.Services.CreateScope())
{
  using var context = scope.ServiceProvider.GetRequiredService<DataContext>();
  context.Database.Migrate();
}

app.Run();
