using Microsoft.EntityFrameworkCore;
using TC_Api;
using TC_Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Configurar conexión SQL Server y Lazy Loading Proxies (opcional)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseLazyLoadingProxies()
           .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Agregar servicios para Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseStaticFiles();

// Habilitar Swagger solo en entorno de desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Redirigir la raíz "/" a Swagger UI
    app.MapGet("/", (HttpContext context) =>
    {
        context.Response.Redirect("/swagger", permanent: false);
        return Task.CompletedTask;
    });
}

// Endpoint para mini app web con grilla
app.MapGet("/app", context =>
{
    context.Response.Redirect("/app.html");
    return Task.CompletedTask;
});


// Endpoints Minimal API para tarjetas
app.MapGet("/tarjetas", async (int? pageNumber, int? pageSize, AppDbContext db) =>
{
    int currentPage = pageNumber.GetValueOrDefault(1);
    int currentPageSize = pageSize.GetValueOrDefault(100);

    if (currentPage < 1) currentPage = 1;
    if (currentPageSize < 1) currentPageSize = 25;

    var totalRecords = await db.Tarjetas.CountAsync();

    var tarjetas = await db.Tarjetas
        .AsNoTracking()
        .Select(t => new {
            t.Id,
            t.NombreTitular,
            t.NumeroTarjeta,
            t.Estado,
            t.Activa
        })
        .Skip((currentPage - 1) * currentPageSize)
        .Take(currentPageSize)
        .ToListAsync();

    return Results.Ok(new
    {
        TotalRecords = totalRecords,
        PageNumber = currentPage,
        PageSize = currentPageSize,
        Results = tarjetas
    });
});


app.MapGet("/tarjetas/{id:int}", async (int id, AppDbContext db) =>
    await db.Tarjetas.FindAsync(id) is Tarjeta tarjeta ? Results.Ok(tarjeta) : Results.NotFound());

app.MapPost("/tarjetas", async (Tarjeta tarjeta, AppDbContext db) =>
{
    db.Tarjetas.Add(tarjeta);
    await db.SaveChangesAsync();
    return Results.Created($"/tarjetas/{tarjeta.Id}", tarjeta);
});

app.MapPut("/tarjetas/{id:int}", async (int id, Tarjeta updatedTarjeta, AppDbContext db) =>
{
    var tarjeta = await db.Tarjetas.FindAsync(id);
    if (tarjeta is null) return Results.NotFound();
    tarjeta.NombreTitular = updatedTarjeta.NombreTitular;
    tarjeta.NumeroTarjeta = updatedTarjeta.NumeroTarjeta;
    tarjeta.Estado = updatedTarjeta.Estado;
    tarjeta.Activa = updatedTarjeta.Activa;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/tarjetas/{id:int}", async (int id, AppDbContext db) =>
{
    var tarjeta = await db.Tarjetas.FindAsync(id);
    if (tarjeta is null) return Results.NotFound();
    db.Tarjetas.Remove(tarjeta);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
