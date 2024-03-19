using System.Net;
using DishesAPI.DbContexts;
using Microsoft.EntityFrameworkCore;
using MinimalAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// register the DbContext on the container, getting the connection string from appSettings   
builder.Services.AddDbContext<DishesDbContext>(o => o.UseSqlite(
    builder.Configuration["ConnectionStrings:DishesDBConnectionString"]));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}
app.UseHttpsRedirection();

app.RegisterDishesEndpoints();
app.RegisterIngredientsEndpoints();

// recreate & migrate the database on each run, for demo purposes
using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<DishesDbContext>();
    context.Database.EnsureDeleted();
    context.Database.Migrate();
}

app.Run();