using Microsoft.OpenApi.Models;
using TinyURL.Data;
using TinyURL.Interfaces;
using TinyURL.Utils;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var configuration = builder.Configuration;

builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        });
});

builder.Services.AddMvcCore().AddApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TinyUrl", Version = "v1" });
});

var config = configuration.GetSection(nameof(TinyURLConfig)).Get<TinyURLConfig>();

builder.Services.AddSingleton<TinyURLConfig>(config);
builder.Services.AddSingleton<IMongoRepository>(new MongoRepository(config.MongoDbConnection, config.MongoDbDatabaseName));
builder.Services.AddSingleton<IUrlMappingCache>(new UrlMappingCache(config.CacheMaxSize));




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TinyUrl V1");
    c.RoutePrefix = "swagger";
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.MapRazorPages();
app.Run();
