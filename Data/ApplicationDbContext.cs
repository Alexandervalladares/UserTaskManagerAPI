using Microsoft.EntityFrameworkCore;
using UserTaskManagerAPI.Models.Entities;

namespace UserTaskManagerAPI.Data
{
    /// <summary>
    /// Contexto de base de datos principal de la aplicación
    /// Maneja la conexión y configuración de las entidades con SQL Server
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Conjunto de entidades de usuarios en la base de datos
        /// </summary>
        public DbSet<UserEntity> Users { get; set; }

        /// <summary>
        /// Conjunto de entidades de tareas en la base de datos
        /// </summary>
        public DbSet<TaskEntity> Tasks { get; set; }

        /// <summary>
        /// Configura las relaciones y restricciones de las entidades
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la entidad User
            modelBuilder.Entity<UserEntity>(entity =>
            {
                // Índice único para el correo electrónico (no permite duplicados)
                entity.HasIndex(u => u.EmailAddress)
                      .IsUnique()
                      .HasDatabaseName("IX_Users_EmailAddress");

                // Configuración de propiedades por defecto
                entity.Property(u => u.RegistrationDate)
                      .HasDefaultValueSql("GETDATE()");

                // Relación uno a muchos: Un usuario tiene muchas tareas
                entity.HasMany(u => u.Tasks)
                      .WithOne(t => t.User)
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.Cascade); // Elimina tareas al eliminar usuario
            });

            // Configuración de la entidad Task
            modelBuilder.Entity<TaskEntity>(entity =>
            {
                // Configuración de propiedades por defecto
                entity.Property(t => t.CreationDate)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(t => t.IsCompleted)
                      .HasDefaultValue(false);

                // Índice para mejorar rendimiento en consultas por usuario
                entity.HasIndex(t => t.UserId)
                      .HasDatabaseName("IX_Tasks_UserId");

                // Índice compuesto para filtrar tareas completadas por usuario
                entity.HasIndex(t => new { t.UserId, t.IsCompleted })
                      .HasDatabaseName("IX_Tasks_UserId_IsCompleted");
            });
        }

        /// <summary>
        /// Sobrescribe el método SaveChanges para agregar lógica personalizada
        /// Establece automáticamente las fechas de creación y actualización
        /// </summary>
        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        /// <summary>
        /// Versión asíncrona de SaveChanges
        /// </summary>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Actualiza automáticamente las marcas de tiempo en las entidades
        /// - CreationDate/RegistrationDate: solo al crear
        /// - UpdatedAt: en cada modificación
        /// - CompletionDate: cuando cambia el estado de completitud
        /// </summary>
        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is UserEntity || e.Entity is TaskEntity)
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                // Para nuevos registros
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity is UserEntity user)
                    {
                        user.RegistrationDate = DateTime.UtcNow;
                    }
                    else if (entry.Entity is TaskEntity task)
                    {
                        task.CreationDate = DateTime.UtcNow;
                    }
                }

                // Para modificaciones: siempre actualizar UpdatedAt
                if (entry.State == EntityState.Modified)
                {
                    if (entry.Entity is UserEntity modifiedUser)
                    {
                        modifiedUser.UpdatedAt = DateTime.UtcNow;
                    }
                    else if (entry.Entity is TaskEntity modifiedTask)
                    {
                        modifiedTask.UpdatedAt = DateTime.UtcNow;

                        // Manejo especial de CompletionDate
                        if (modifiedTask.IsCompleted && !modifiedTask.CompletionDate.HasValue)
                        {
                            modifiedTask.CompletionDate = DateTime.UtcNow;
                        }
                        else if (!modifiedTask.IsCompleted && modifiedTask.CompletionDate.HasValue)
                        {
                            modifiedTask.CompletionDate = null;
                        }
                    }
                }
            }
        }
    }
}