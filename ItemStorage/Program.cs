using System.Text.Json.Serialization;
using ItemStorage.Data;
using ItemStorage.Middleware;
using ItemStorage.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
.AddJsonOptions(x =>
{
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
}); 

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<DataContext>(c => 
    c.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IItemService, ItemService>();

builder.Services.AddScoped<IDataContextProvider>(
    x => new DataContextProvider(
        builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? throw new InvalidOperationException("Connection string not found")));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.MapControllers();

app.UseHttpsRedirection();
app.Run();