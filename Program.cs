using Microsoft.EntityFrameworkCore;
using UserTaskManagerAPI.Data;
using UserTaskManagerAPI.Repositories;
using UserTaskManagerAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios en el contenedor de inyección de dependencias
builder.Services.AddControllers();

// Configuración de Swagger/OpenAPI para documentación de la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración del contexto de base de datos con SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        // Configuración de resiliencia para manejar fallos temporales de conexión
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    });
});

// Registro de repositorios - Capa de acceso a datos
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

// Registro de servicios - Capa de lógica de negocio
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITaskService, TaskService>();

// Configuración de CORS para permitir peticiones desde diferentes orígenes
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Aplicar migraciones automáticamente al iniciar la aplicación (útil para Docker)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Verifica si hay migraciones pendientes y las aplica
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
            Console.WriteLine("✓ Migraciones aplicadas correctamente");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al aplicar migraciones de base de datos");
    }
}

// Configuración del pipeline de peticiones HTTP
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "UserTask Manager API v1");
    options.RoutePrefix = "api-docs"; // Ruta personalizada según requerimientos
});

app.UseCors("AllowAll");

app.UseAuthorization();

// Mapeo de controladores a rutas
app.MapControllers();

// Endpoint de health check básico
app.MapGet("/", () => new
{
    Status = "Running",
    Service = "UserTask Manager API",
    Version = "1.0.0",
    Documentation = "/api-docs"
});

Console.WriteLine(" UserTask Manager API iniciada correctamente");
Console.WriteLine(" Documentación disponible en: /api-docs");

app.Run();