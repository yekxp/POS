using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Converters;
using JwtManagerHandler;
using Newtonsoft.Json;
using pos_backoffice_user_managment.Models;
using pos_backoffice_user_managment.Database;
using pos_backoffice_user_managment.Services;
using pos_backoffice_user_managment.Services.Impl;

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
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDBSettings"));
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    MongoDBSettings settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddCustomJwtAuthentica();
builder.Services.AddSingleton<JwtTokenHandler>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
