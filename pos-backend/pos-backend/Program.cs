using pos_backend.Models;
using pos_backend.Services.Impl;
using pos_backend.Services;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using pos_backend.Database;
using Newtonsoft.Json.Converters;
using JwtManagerHandler;
using Newtonsoft.Json;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.RespectBrowserAcceptHeader = true;
    options.ReturnHttpNotAcceptable = true;
})
.AddNewtonsoftJson(jsonOptions =>
{
    jsonOptions.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    jsonOptions.SerializerSettings.Converters.Add(new StringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDBSettings"));
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    MongoDBSettings settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddCustomJwtAuthentica();
builder.Services.AddSingleton<JwtTokenHandler>();

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ICashRegisterService, CashRegisterService>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
