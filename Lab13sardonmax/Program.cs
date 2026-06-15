using Lab13sardonmax.Api;
using Lab13sardonmax.Application.Abstractions;
using Lab13sardonmax.Application.Reportes;
using Lab13sardonmax.Components;
using Lab13sardonmax.Infrastructure.Data;
using Lab13sardonmax.Infrastructure.Excel;
using Lab13sardonmax.Infrastructure.Repositories;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseStaticWebAssets();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<LaboratorioDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Laboratorio")));

builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<
    IQueryHandler<ObtenerTicketsQuery, IReadOnlyList<TicketReporteDto>>,
    ObtenerTicketsHandler>();
builder.Services.AddScoped<
    IQueryHandler<ObtenerActividadUsuariosQuery, IReadOnlyList<ActividadUsuarioReporteDto>>,
    ObtenerActividadUsuariosHandler>();
builder.Services.AddSingleton<IReporteExcelService, ClosedXmlReporteExcelService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LAB 13 - API de Reportes de Ticketera",
        Version = "v1",
        Description = "Dos reportes Excel basados en TicketeraBD, con repositorios, CQRS y ClosedXML."
    });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
        KnownNetworks = { },
        KnownProxies = { }
    });
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "LAB 13 - Ticketera v1");
    options.DocumentTitle = "LAB 13 - Swagger";
});

app.MapReportesEndpoints();
app.MapGet("/salud", () => Results.Ok(new
{
    estado = "correcto",
    aplicacion = "Lab13sardonmax",
    fechaUtc = DateTime.UtcNow
}))
.WithName("ComprobarSalud")
.WithTags("Despliegue");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LaboratorioDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();
