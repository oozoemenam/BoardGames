using BoardGames.Data;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseSqlite("DataSource=BoardGames.sqlite3");
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(config =>
    {
        config.WithOrigins(builder.Configuration["AllowedOrigins"]!);
        config.AllowAnyHeader();
        config.AllowAnyMethod();
    });
    options.AddPolicy(name: "AnyOrigin", config =>
    {
        config.AllowAnyOrigin();
        config.AllowAnyHeader();
        config.AllowAnyMethod();
    });
});
builder.Services.AddApiVersioning(options =>
{
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
});
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "BoardGames", Version = "v1.0" });
    options.SwaggerDoc("v2", new OpenApiInfo { Title = "BoardGames", Version = "v2.0" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint($"/swagger/v1/swagger.json", $"BoardGames v1");
        options.SwaggerEndpoint($"/swagger/v2/swagger.json", $"BoardGames v2");
    });
}

if (app.Configuration.GetValue<bool>("UseDeveloperExceptionPage"))
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapGet("/error", () => Results.Problem()).RequireCors("AnyOrigin");

app.MapGet("v{version:ApiVersion}/error/test",
    [ApiVersion("1.0")]
[ApiVersion("2.0")]
[EnableCors("AnyOrigin")]
[ResponseCache(NoStore = true)]
() =>
    { throw new Exception("test"); });
app.MapControllers();

app.Run();
