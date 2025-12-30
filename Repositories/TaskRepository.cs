using Microsoft.EntityFrameworkCore;
using UserTaskManagerAPI.Data;
using UserTaskManagerAPI.Models.Entities;

namespace UserTaskManagerAPI.Repositories
{
    /// <summary>
    /// Interfaz que define las operaciones de acceso a datos para tareas
    /// </summary>
    public interface ITaskRepository
    {
        Task<TaskEntity?> GetByIdAsync(int taskId);
        Task<IEnumerable<TaskEntity>> GetByUserIdAsync(int userId);
        Task<TaskEntity> CreateAsync(TaskEntity task);
        Task<TaskEntity> UpdateAsync(TaskEntity task);
        Task<bool> DeleteAsync(int taskId);
        Task<int> GetCompletedCountByUserAsync(int userId);
    }

    /// <summary>
    /// Repositorio que implementa las operaciones de acceso a datos para tareas
    /// Maneja todas las interacciones con la tabla Tasks
    /// </summary>
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene una tarea específica por su ID incluyendo datos del usuario
        /// </summary>
        /// <param name="taskId">ID de la tarea a buscar</param>
        /// <returns>Tarea encontrada o null si no existe</returns>
        public async Task<TaskEntity?> GetByIdAsync(int taskId)
        {
            return await _context.Tasks
                .Include(t => t.User) // Incluye datos del usuario propietario
                .FirstOrDefaultAsync(t => t.TaskId == taskId);
        }

        /// <summary>
        /// Obtiene todas las tareas asociadas a un usuario específico
        /// Ordenadas por fecha de creación (más recientes primero)
        /// </summary>
        /// <param name="userId">ID del usuario propietario</param>
        /// <returns>Lista de tareas del usuario</returns>
        public async Task<IEnumerable<TaskEntity>> GetByUserIdAsync(int userId)
        {
            return await _context.Tasks
                .Include(t => t.User)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreationDate) // Más recientes primero
                .ToListAsync();
        }

        /// <summary>
        /// Crea una nueva tarea en la base de datos
        /// </summary>
        /// <param name="task">Entidad de tarea a crear</param>
        /// <returns>Tarea creada con su ID generado</returns>
        public async Task<TaskEntity> CreateAsync(TaskEntity task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Recargar la entidad con datos de navegación
            await _context.Entry(task)
                .Reference(t => t.User)
                .LoadAsync();

            return task;
        }

        /// <summary>
        /// Actualiza los datos de una tarea existente
        /// </summary>
        /// <param name="task">Tarea con los datos actualizados</param>
        /// <returns>Tarea actualizada</returns>
        public async Task<TaskEntity> UpdateAsync(TaskEntity task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
            return task;
        }

        /// <summary>
        /// Elimina una tarea de la base de datos
        /// </summary>
        /// <param name="taskId">ID de la tarea a eliminar</param>
        /// <returns>True si se eliminó correctamente, False si no existía</returns>
        public async Task<bool> DeleteAsync(int taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null)
                return false;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Cuenta cuántas tareas completadas tiene un usuario
        /// Útil para estadísticas y reportes
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Número de tareas completadas</returns>
        public async Task<int> GetCompletedCountByUserAsync(int userId)
        {
            return await _context.Tasks
                .Where(t => t.UserId == userId && t.IsCompleted)
                .CountAsync();
        }
    }
}