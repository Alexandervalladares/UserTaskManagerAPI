using Microsoft.EntityFrameworkCore;
using UserTaskManagerAPI.Data;
using UserTaskManagerAPI.Models.Entities;

namespace UserTaskManagerAPI.Repositories
{
    /// <summary>
    /// Interfaz que define las operaciones de acceso a datos para usuarios
    /// </summary>
    public interface IUserRepository
    {
        Task<UserEntity?> GetByIdAsync(int userId);
        Task<UserEntity?> GetByEmailAsync(string email);
        Task<IEnumerable<UserEntity>> GetAllAsync();
        Task<UserEntity> CreateAsync(UserEntity user);
        Task<UserEntity> UpdateAsync(UserEntity user);
        Task<bool> DeleteAsync(int userId);
        Task<bool> ExistsByEmailAsync(string email);
    }

    /// <summary>
    /// Repositorio que implementa las operaciones de acceso a datos para usuarios
    /// Maneja todas las interacciones con la tabla Users
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene un usuario por su ID incluyendo sus tareas
        /// </summary>
        /// <param name="userId">ID del usuario a buscar</param>
        /// <returns>Usuario encontrado o null si no existe</returns>
        public async Task<UserEntity?> GetByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Tasks) // Carga las tareas relacionadas
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        /// <summary>
        /// Busca un usuario por su dirección de correo electrónico
        /// Útil para verificar emails duplicados antes de crear/actualizar
        /// </summary>
        /// <param name="email">Email del usuario a buscar</param>
        /// <returns>Usuario encontrado o null</returns>
        public async Task<UserEntity?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.EmailAddress == email);
        }

        /// <summary>
        /// Obtiene todos los usuarios del sistema con sus tareas
        /// </summary>
        /// <returns>Lista de todos los usuarios</returns>
        public async Task<IEnumerable<UserEntity>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.Tasks)
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        /// <summary>
        /// Crea un nuevo usuario en la base de datos
        /// </summary>
        /// <param name="user">Entidad de usuario a crear</param>
        /// <returns>Usuario creado con su ID generado</returns>
        public async Task<UserEntity> CreateAsync(UserEntity user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        /// <summary>
        /// Actualiza los datos de un usuario existente
        /// </summary>
        /// <param name="user">Usuario con los datos actualizados</param>
        /// <returns>Usuario actualizado</returns>
        public async Task<UserEntity> UpdateAsync(UserEntity user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        /// <summary>
        /// Elimina un usuario y todas sus tareas asociadas (cascada)
        /// </summary>
        /// <param name="userId">ID del usuario a eliminar</param>
        /// <returns>True si se eliminó correctamente, False si no existía</returns>
        public async Task<bool> DeleteAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Verifica si ya existe un usuario con el email proporcionado
        /// Útil para prevenir duplicados antes de insertar/actualizar
        /// </summary>
        /// <param name="email">Email a verificar</param>
        /// <returns>True si el email ya está registrado</returns>
        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.EmailAddress == email);
        }
    }
}