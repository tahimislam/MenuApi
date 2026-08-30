using Microsoft.EntityFrameworkCore;
using MenuApi.Data;
using MenuApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MenuDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Auto-create the database/tables on startup (fine for learning; real projects use migrations)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MenuDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/menuitems", async (MenuDbContext db) =>
    await db.MenuItems.ToListAsync());

app.MapGet("/menuitems/{id}", async (int id, MenuDbContext db) =>
    await db.MenuItems.FindAsync(id) is MenuItem item ? Results.Ok(item) : Results.NotFound());

app.MapPost("/menuitems", async (MenuItem item, MenuDbContext db) =>
{
    db.MenuItems.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/menuitems/{item.Id}", item);
});

app.MapPut("/menuitems/{id}", async (int id, MenuItem input, MenuDbContext db) =>
{
    var item = await db.MenuItems.FindAsync(id);
    if (item is null) return Results.NotFound();
    item.Name = input.Name;
    item.Category = input.Category;
    item.Price = input.Price;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/menuitems/{id}", async (int id, MenuDbContext db) =>
{
    var item = await db.MenuItems.FindAsync(id);
    if (item is null) return Results.NotFound();
    db.MenuItems.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
